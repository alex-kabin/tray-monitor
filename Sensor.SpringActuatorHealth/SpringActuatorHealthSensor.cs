using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Logging.Core;
using RestSharp;
using Sensor.Core;

namespace Sensor.SpringActuatorHealth
{
    public class SpringActuatorHealthSensor : SensorBase
    {
        private static readonly ILogger Log = LoggerFactory.GetLogger(nameof(SpringActuatorHealthSensor));
        
        private readonly RestClient _restClient;
        private string _url;
        private TimeSpan _period = TimeSpan.FromSeconds(10);
        private TimeSpan _timeout = TimeSpan.FromSeconds(20);
        private CancellationTokenSource _cancellation;

        public SpringActuatorHealthSensor() {
            _restClient = new RestClient {
                    RemoteCertificateValidationCallback = (sender, certificate, chain, errors) => true
            };
        }

        public override void Configure(IEnumerable<(string, string)> parameters) {
            foreach ((string key, string value) in parameters) {
                if (string.Equals(key, "URL", StringComparison.OrdinalIgnoreCase)) {
                    Title = _url = value;
                }
                if (string.Equals(key, "Period", StringComparison.OrdinalIgnoreCase)) {
                    _period = TimeSpan.Parse(value);
                }
                if (string.Equals(key, "Timeout", StringComparison.OrdinalIgnoreCase)) {
                    _timeout = TimeSpan.Parse(value);
                }
            }
        }

        public override async Task Connect(CancellationToken cancellationToken) {
            if (String.IsNullOrEmpty(_url)) {
                throw new InvalidOperationException("URL must be set");
            }
            if (_state != SensorState.Offline) {
                return;
            }

            Error = null;
            _state = SensorState.Connecting;
            
            _cancellation?.Dispose();
            _cancellation = new CancellationTokenSource();
#pragma warning disable 4014
            Task.Factory.StartNew(
                    () => CheckHealth(_cancellation.Token), 
                    _cancellation.Token, 
                    TaskCreationOptions.LongRunning, 
                    TaskScheduler.Default
            );
#pragma warning restore 4014

            State = SensorState.Online;
        }

        private async void CheckHealth(CancellationToken cancellationToken) {
            try {
                while (!cancellationToken.IsCancellationRequested) {
                    Log.Debug($"Requesting health endpoint {_url}");
                    var request = new RestRequest(_url) {
                            Timeout = (int)_timeout.TotalMilliseconds
                    };
                    var response = await _restClient.ExecuteGetAsync<SpringActuatorHealthInfo>(request, cancellationToken);
                    if (response.Data != null) {
                        Value = response.Data.Status == SpringActuatorHealthStatus.UP;
                        Log.Info($"The server responded with HTTP code {(int) response.StatusCode} ({response.StatusCode}); result={response.Data.Status}");
                    }
                    else {
                        if (response.ErrorException == null) {
                            Log.Warn(
                                    $"The server responded with HTTP code {(int) response.StatusCode} ({response.StatusCode})",
                                    response.Content);
                            Error = new SensorException("Bad response");
                        } else {
                            Log.Warn($"Connection failure: {response.ErrorMessage}", response.ErrorException);
                            Error = new SensorException("Connection failure", response.ErrorException);
                        }
                    }

                    await Task.Delay(_period, cancellationToken);
                }
            }
            catch (OperationCanceledException) {
                Error = null;
                Log.Debug("Check canceled");
            }
            catch (Exception ex) {
                Log.Error("Check failure", ex);
                Error = new SensorException("Check failure", ex);
            }

            Value = null;
            State = SensorState.Offline;
        }

        public override async Task Disconnect(CancellationToken cancellationToken) {
            if (State != SensorState.Online) {
                return;
            }

            Log.Debug("Disconnecting...");
            State = SensorState.Disconnecting;

            if (_cancellation != null) {
                _cancellation.Cancel();
                _cancellation.Dispose();
                _cancellation = null;
                await Task.Delay(TimeSpan.FromMilliseconds(500), cancellationToken);
            }

            Log.Info("Disconnected");
        }
    }
}