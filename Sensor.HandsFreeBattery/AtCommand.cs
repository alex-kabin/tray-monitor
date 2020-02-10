using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Logging.Core;

namespace Sensor.HandsFreeBattery
{
    static class AtCommand
    {
        private static readonly ILogger Log = LoggerFactory.GetLogger(nameof(AtCommand));

        private static readonly byte[] CR_LF = { 13, 10 };

        public static bool Is(string command, string prefix) {
            return command.StartsWith(prefix, StringComparison.OrdinalIgnoreCase);
        }

        public static bool Is(string command, string prefix, out string value) {
            if (Is(command, prefix)) {
                value = command.Substring(prefix.Length);
                return true;
            }
            else {
                value = null;
                return false;
            }
        }

        public static async Task Send(Stream stream, string text, CancellationToken cancellationToken) {
            Log.Debug("> " + text);
            
            await WriteBytesAsync(stream, CR_LF, cancellationToken);
            await WriteBytesAsync(stream, Encoding.ASCII.GetBytes(text), cancellationToken);
            await WriteBytesAsync(stream, CR_LF, cancellationToken);
            
            /*
            await WriteByteAsync(stream, 13, cancellationToken);
            await WriteByteAsync(stream, 10, cancellationToken);
            
            var buf = Encoding.ASCII.GetBytes(text);
            await stream.WriteAsync(buf, 0, buf.Length, cancellationToken);
            
            await WriteByteAsync(stream, 13, cancellationToken);
            await WriteByteAsync(stream, 10, cancellationToken);
            */
        }

        public static async Task<string> Receive(Stream stream, CancellationToken cancellationToken) {
            var text= await ReadLine(stream, cancellationToken);
            Log.Debug("< " + text);
            return text;
        }

        private static async Task<string> ReadLine(Stream stream, CancellationToken cancellationToken) {
            var sb = new StringBuilder();
            int ch;
            while ((ch = await ReadByteAsync(stream, cancellationToken)) >= 0) {
                if (ch == 13) {
                    return sb.ToString();
                }
                sb.Append((char) ch);
            }
            return null;
        }

        private static async Task<int> ReadByteAsync(Stream stream, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            
            byte[] buf = new byte[1];
            int count;
            try {
                count = await stream.ReadAsync(buf, 0, 1, cancellationToken);
            }
            catch (ObjectDisposedException) {
                if (cancellationToken.IsCancellationRequested) {
                    cancellationToken.ThrowIfCancellationRequested();
                }
                throw;
            }

            return count > 0 ? buf[0] : -1;
        }
        
        private static Task WriteByteAsync(Stream stream, byte b, CancellationToken cancellationToken) {
            return stream.WriteAsync(new [] { b }, 0, 1, cancellationToken);
        }
        
        private static Task WriteBytesAsync(Stream stream, byte[] buf, CancellationToken cancellationToken) {
            return stream.WriteAsync(buf, 0, buf.Length, cancellationToken);
        }
    }
}
