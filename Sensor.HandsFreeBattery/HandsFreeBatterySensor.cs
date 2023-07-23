using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using InTheHand.Net;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;
using Logging.Core;
using Sensor.Core;

namespace Sensor.HandsFreeBattery
{
    public class HandsFreeBatterySensor : SensorBase, IConfigurable, IDisposable
    {
        private static readonly ILogger Log = LoggerFactory.GetLogger(nameof(HandsFreeBatterySensor));
        
        private const int CONNECT_ATTEMPTS = 10;
        private const int CONNECT_ATTEMPT_DELAY = 5000; //ms
        
        private string _deviceName;
        private BluetoothClient _client;
        private BluetoothDeviceInfo _device;
        private NetworkStream _stream;
        private CancellationTokenSource _cancellation;
        
        public HandsFreeBatterySensor() {
        }

        public void Configure(IEnumerable<(string, string)> parameters) {
            foreach ((string key, string value) in parameters) {
                if (string.Equals(key, "DeviceName", StringComparison.InvariantCultureIgnoreCase)) {
                    Title = _deviceName = value;
                }
            }
        }

        private Task<BluetoothDeviceInfo[]> DiscoverDevices(int maxCount, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            
            var tcs = new TaskCompletionSource<BluetoothDeviceInfo[]>();
            var bluetoothClient = new BluetoothClient();
            void Callback(IAsyncResult ar) {
                using (bluetoothClient) {
                    if (cancellationToken.IsCancellationRequested) {
                        tcs.TrySetCanceled(cancellationToken);
                        return;
                    }
                    try {
                        var result = bluetoothClient.EndDiscoverDevices(ar);
                        tcs.TrySetResult(result);
                    }
                    catch (Exception ex) {
                        tcs.TrySetException(ex);
                    }
                }
            }
            bluetoothClient.BeginDiscoverDevices(maxCount, true, true, false, false, Callback,null);
            return tcs.Task;
        }
        
        private Task<BluetoothClient> ConnectEndpoint(BluetoothEndPoint bluetoothEndPoint, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            
            var bluetoothClient = new BluetoothClient();
            var tcs = new TaskCompletionSource<BluetoothClient>();

            void Callback(IAsyncResult ar) {
                if (cancellationToken.IsCancellationRequested) {
                    bluetoothClient.Dispose();
                    tcs.TrySetCanceled(cancellationToken);
                    return;
                }

                try {
                    bluetoothClient.EndConnect(ar);
                    tcs.TrySetResult(bluetoothClient);
                }
                catch (Exception ex) {
                    bluetoothClient.Dispose();
                    tcs.TrySetException(ex);
                }
            }

            bluetoothClient.BeginConnect(bluetoothEndPoint, Callback,null);
            return tcs.Task;
        }

        private async Task ConnectDevice(CancellationToken cancellationToken) {
            int count = 0;
            do {
                Log.Debug("Discovering bluetooth devices");
                var devices = await DiscoverDevices(10, cancellationToken);
                
                Log.Debug($"Found {devices.Length} device(s)");

                var filteredDevices =
                        string.IsNullOrEmpty(_deviceName)
                                ? devices
                                : devices.Where(d => string.Equals(_deviceName, d.DeviceName, StringComparison.OrdinalIgnoreCase));

                _client = null;
                _device = null;
                _stream = null;

                foreach (var device in filteredDevices) {
                    cancellationToken.ThrowIfCancellationRequested();
                    var bluetoothEndPoint = new BluetoothEndPoint(device.DeviceAddress, BluetoothService.Handsfree);

                    Log.Debug($"Connecting to '{device.DeviceName}' at address {device.DeviceAddress}");
                    try {
                        _client = await ConnectEndpoint(bluetoothEndPoint, cancellationToken);
                    }
                    catch (OperationCanceledException) {
                        throw;
                    }
                    catch (Exception ex) {
                        Log.Error($"Failed connecting to bluetooth endpoint {bluetoothEndPoint}", ex);
                        continue;
                    }

                    try {
                        _stream = _client.GetStream();
                    }
                    catch (Exception ex) {
                        Log.Error($"Failed obtaining data stream", ex);
                        _client.Dispose();
                        _client = null;
                        continue;
                    }

                    _device = device;
                    Log.Info($"Connected to '{device.DeviceName}'");
                    return;
                }

                if (_stream == null) {
                    if (++count < CONNECT_ATTEMPTS) {
                        Log.Debug($"Failed connecting to HandsFree device, will retry after {CONNECT_ATTEMPT_DELAY}ms");
                        await Task.Delay(TimeSpan.FromMilliseconds(CONNECT_ATTEMPT_DELAY), cancellationToken);
                    }
                    else {
                        throw new SensorException($"Failed connecting to HandsFree device after {CONNECT_ATTEMPTS} attempts");            
                    }
                }
            } while (true);
        }

        private async Task EstablishConversation(CancellationToken cancellationToken) {
            Log.Debug("Establishing service layer connection...");
            var slc = new ServiceLayerConnection(_stream);
            try {
                await slc.Establish(cancellationToken);
                Log.Info("Service layer connection established");
            }
            catch (OperationCanceledException) {
                throw;
            }
            catch (Exception ex) {
                throw;
            }
        }

        private void Reset() {
            Disposable.Destroy(ref _stream);
            Disposable.Destroy(ref _client);
            
            _device = null;
            Title = _deviceName;
            Status = SensorStatus.Offline;
        }

        public override async Task Connect(CancellationToken cancellationToken) {
            if (Status != SensorStatus.Offline) {
                return;
            }

            Error = null;
            Status = SensorStatus.Connecting;

            try {
                await ConnectDevice(cancellationToken);
                Title = _device?.DeviceName;
                await EstablishConversation(cancellationToken);
            }
            catch (Exception ex) {
                if (ex is OperationCanceledException) {
                    Log.Debug("Connection canceled");
                } else {
                    Log.Error("Connection failure", ex);
                    Error = ex as SensorException ?? new SensorException("Connection failure", ex);
                }
                Reset();
                throw;
            }

            _cancellation?.Dispose();
            _cancellation = new CancellationTokenSource();
#pragma warning disable 4014
            Task.Factory.StartNew(
                    () => DoConversation(_cancellation.Token), 
                    _cancellation.Token, 
                    TaskCreationOptions.LongRunning, 
                    TaskScheduler.Default
            );
#pragma warning restore 4014

            Status = SensorStatus.Online;
        }

        private async void DoConversation(CancellationToken cancellationToken) {
            try {
                bool supportsAppleSpecificCommands = false;
                while (!cancellationToken.IsCancellationRequested) {
                    string command = await AtCommand.Receive(_stream, cancellationToken);
                    if (string.IsNullOrEmpty(command)) {
                        Log.Warn("Connection lost");
                        break;
                    }

                    string value;
                    if (AtCommand.Is(command, "AT+BIEV=", out value)) {
                        string[] args = value.Split(',');
                        object val = null;
                        if (args.Length == 2) {
                            if (int.TryParse(args[0], out int indicator) && indicator == 2 /* HF Battery Indicator */) {
                                if (int.TryParse(args[1], out int v)) {
                                    val = v;
                                }
                            }
                        }
                        Value = val;
                    }
                    else if (AtCommand.Is(command, "AT+XAPL=", out value)) {
                        supportsAppleSpecificCommands = true;
                        string[] args = value.Split(',');
                        if (args.Length == 2) {
                            if (int.TryParse(args[1], out int f)) {
                                AppleAccessoryFeatures features = (AppleAccessoryFeatures) f;
                                Log.Info($"Device supports Apple accessory features: {features}");
                                //await AtCommand.Send(_stream, "OK", cancellationToken);
                                //await AtCommand.Send(_stream, "+XAPL=iPhone,"+(int)AppleAccessoryFeatures.NONE, cancellationToken);
                            }
                        }
                    }
                    else if (AtCommand.Is(command, "AT+IPHONEACCEV=", out value)) {
                        int[] args = null;
                        try {
                            args = value.Split(',').Select(int.Parse).ToArray();
                        }
                        catch (Exception ex) {
                            Log.Warn($"Failed parsing command value '{value}'", ex);
                        }

                        if (args != null) {
                            int count = args[0];
                            int i = 0;
                            while (count > 0 && args.Length >= 3 + 2 * i) {
                                int key = args[1 + 2 * i];
                                int val = args[2 + 2 * i];
                                switch (key) {
                                    case 1: // Battery Level
                                        Value = 10 * val + 5;
                                        break;
                                    case 2: // Dock State
                                        Log.Info($"Accessory is {(val == 0 ? "undocked" : "docked")}");
                                        break;
                                }

                                i++;
                                count--;
                            }
                        }
                    }
                    // else if (command.StartsWith("AT+BTRH?")) {
                    //     await AtCommand.Send(_stream, "+BTRH:2", cancellationToken);
                    // }

                    // answer on any command with OK
                    if (AtCommand.Is(command, "AT+")) {
                        await AtCommand.Send(_stream, "OK", cancellationToken);
                    }
                }
            }
            catch (OperationCanceledException) {
                Error = null;
                Log.Debug("Canceled");
            }
            catch (Exception ex) {
                Log.Error("Conversation failure", ex);
                Error = new SensorException("Conversation failure", ex);
            }
            Reset();
        }

        public override async Task Disconnect(CancellationToken cancellationToken) {
            if (Status != SensorStatus.Online) {
                return;
            }

            Log.Debug("Disconnecting...");
            Status = SensorStatus.Disconnecting;

            // close stream to interrupt all read awaitings
            Disposable.Destroy(ref _stream);
            
            if (_cancellation != null) {
                _cancellation.Cancel();
                _cancellation.Dispose();
                _cancellation = null;
                await Task.Delay(TimeSpan.FromMilliseconds(1000), cancellationToken);
            }

            Log.Info("Disconnected");
        }

        public void Dispose() {
            if (_cancellation != null) {
                _cancellation.Cancel();
                _cancellation.Dispose();
                _cancellation = null;
            }
            Reset();
        }
    }
}