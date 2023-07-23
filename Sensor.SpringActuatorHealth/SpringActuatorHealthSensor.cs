using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Logging.Core;
using RestSharp;
using Sensor.Core;

namespace Sensor.SpringActuatorHealth
{
    public class SpringActuatorHealthSensor : SensorBase, IConfigurable
    {
        private static readonly ILogger Log = LoggerFactory.GetLogger(nameof(SpringActuatorHealthSensor));
        
        private readonly RestClient _restClient;
        private string _url;
        private TimeSpan _period = TimeSpan.FromSeconds(10);
        private TimeSpan _timeout = TimeSpan.FromSeconds(20);
        private CancellationTokenSource _cancellation;

        public SpringActuatorHealthSensor()
        {
            var restClientOptions = new RestClientOptions {
                RemoteCertificateValidationCallback = (sender, certificate, chain, errors) => true
            };
            _restClient = new RestClient(restClientOptions) { };
        }

        public void Configure(IEnumerable<(string, string)> parameters) {
            foreach ((string key, string value) in parameters) {
                if (string.Equals(key, "URL", StringComparison.OrdinalIgnoreCase)) {
                    Title = _url = value;
                }
                else if (string.Equals(key, "Period", StringComparison.OrdinalIgnoreCase)) {
                    _period = TimeSpan.Parse(value);
                }
                else if (string.Equals(key, "Timeout", StringComparison.OrdinalIgnoreCase)) {
                    _timeout = TimeSpan.Parse(value);
                }
            }
        }

        public override async Task Connect(CancellationToken cancellationToken) {
            if (string.IsNullOrEmpty(_url)) {
                throw new InvalidOperationException("URL must be set");
            }
            if (Status != SensorStatus.Offline) {
                return;
            }
            
            Status = SensorStatus.Connecting;
            Error = null;
            
            _cancellation?.Dispose();
            _cancellation = new CancellationTokenSource();

            Task.Factory.StartNew(
                    () => CheckHealth(_cancellation.Token), 
                    _cancellation.Token, 
                    TaskCreationOptions.LongRunning, 
                    TaskScheduler.Default
            );

            Status = SensorStatus.Online;
        }

        private async void CheckHealth(CancellationToken cancellationToken) {
            try {
                while (true) {
                    cancellationToken.ThrowIfCancellationRequested();
                    
                    Log.Debug($"Requesting health endpoint {_url}");
                    var request = new RestRequest(_url) {
                            Timeout = (int)_timeout.TotalMilliseconds
                    };
                    var response = await _restClient.ExecuteGetAsync<SpringActuatorHealthInfo>(request, cancellationToken);
                    if (response.Data != null) {
                        Value = "up".Equals(response.Data.Status, StringComparison.OrdinalIgnoreCase);
                        Log.Info($"The server responded with HTTP code {(int) response.StatusCode} ({response.StatusCode}); result={response.Data.Status}");
                    }
                    else {
                        if (response.ErrorException == null) {
                            Log.Warn($"The server responded with HTTP code {(int) response.StatusCode} ({response.StatusCode})", response.Content);
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
                Log.Debug("Check canceled");
                Error = null;
                Status = SensorStatus.Offline;
            }
            catch (Exception ex) {
                Log.Error("Check failure", ex);
                Status = SensorStatus.Offline;
                Error = new SensorException("Check failure", ex);
            }
        }

        public override async Task Disconnect(CancellationToken cancellationToken) {
            if (Status != SensorStatus.Online) {
                return;
            }

            Log.Debug("Disconnecting...");
            Status = SensorStatus.Disconnecting;

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