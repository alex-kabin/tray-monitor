using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Logging.Core;

namespace Sensor.HandsFreeBattery
{
    class ServiceLayerConnection
    {
        private static readonly ILogger Log = LoggerFactory.GetLogger(nameof(ServiceLayerConnection));

        private readonly Stream _stream;

        public ServiceLayerConnection(Stream stream) {
            _stream = stream;
        }

        private void AssertCommand(string command, string expected) {
            if (!command.StartsWith(expected)) {
                throw new InvalidOperationException($"Expected command '{expected}' but received '{command}'");
            }
        }

        public async Task Establish(CancellationToken ct) {
            string command;

            // AT+BRSF=...
            command = await AtCommand.Receive(_stream, ct);
            AssertCommand(command, "AT+BRSF=");
            HandsFreeFeatures features = 0;
            var value = command.Substring("AT+BRSF=".Length);
            if (int.TryParse(value, out int f)) {
                features = (HandsFreeFeatures) f;
            }
            else {
                throw new InvalidOperationException("Failed reading HandsFree device features: "+value);
            }
            
            Log.Info("HandsFree device supports features: "+features);
            bool deviceSupportsIndicators = features.HasFlag(HandsFreeFeatures.HF_INDICATORS);
            if (!deviceSupportsIndicators) {
                Log.Warn($"HandsFree device does not support standard {HandsFreeFeatures.HF_INDICATORS} feature");
            }

            await AtCommand.Send(_stream, "+BRSF:"+(int)AudioGatewayFeatures.HF_INDICATORS, ct);
            await AtCommand.Send(_stream, "OK", ct);

            // AT+CIND=?
            command = await AtCommand.Receive(_stream, ct);
            AssertCommand(command, "AT+CIND=?");
            await AtCommand.Send(_stream, "+CIND:0", ct);
            await AtCommand.Send(_stream, "OK", ct);

            // AT+CIND?
            command = await AtCommand.Receive(_stream, ct);
            AssertCommand(command, "AT+CIND?");
            await AtCommand.Send(_stream, "+CIND:1,0", ct);
            await AtCommand.Send(_stream, "OK", ct);

            // AT+CMER
            command = await AtCommand.Receive(_stream, ct);
            AssertCommand(command, "AT+CMER");
            await AtCommand.Send(_stream, "OK", ct);

             
            command = await AtCommand.Receive(_stream, ct);
            
            // AT+CHLD=?
            if (command.StartsWith("AT+CHLD=?")) {
                await AtCommand.Send(_stream, "+CHLD:0", ct);
                await AtCommand.Send(_stream, "OK", ct);
                return;
            }
            
            // AT+BIND=...
            AssertCommand(command, "AT+BIND=");
            if (command.StartsWith("AT+BIND=")) {
                await AtCommand.Send(_stream, "OK", ct);
            }
            
            // AT+BIND=?
            command = await AtCommand.Receive(_stream, ct);
            AssertCommand(command, "AT+BIND=?");
            await AtCommand.Send(_stream, "+BIND:2", ct);
            await AtCommand.Send(_stream, "OK", ct);

            // AT+BIND?
            command = await AtCommand.Receive(_stream, ct);
            AssertCommand(command, "AT+BIND?");
            await AtCommand.Send(_stream, "+BIND:2,1", ct);
            await AtCommand.Send(_stream, "OK", ct);
        }
    }
}
