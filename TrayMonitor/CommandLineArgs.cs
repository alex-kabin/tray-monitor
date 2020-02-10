using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace TrayMonitor
{
    public class CommandLineArgs : IConfiguration
    {
        public bool AutoConnect { get; private set; }
        public string SensorTypeName { get; private set; }
        public List<(string, string)> SensorProperties { get; private set; }
        public string IndicatorTypeName { get; private set; }
        public List<(string, string)> IndicatorProperties { get; private set; }

        public CommandLineArgs(string[] args) {
            Parse(args);
            Validate();
        }

        private static (string, string) ParseKeyValue(string arg) {
            int idx = arg.IndexOf('=');
            if (idx < 0) {
                throw new ApplicationException($"Bad key-value parameter: {arg}");
            }
            string key = arg.Substring(0, idx);
            string value = arg.Substring(idx + 1);
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value)) {
                throw new ApplicationException($"Bad key-value parameter: {arg}");
            }
            return (key, value);
        }

        private void Validate() {
            if (string.IsNullOrWhiteSpace(SensorTypeName)) {
                throw new ApplicationException("Sensor type must be specified");
            }
            if (string.IsNullOrWhiteSpace(IndicatorTypeName)) {
                throw new ApplicationException("Indicator type must be specified");
            }
        }

        private void Parse(string[] args) {
            if (args == null || args.Length == 0) {
                ShowHelpAndAbort();
            }

            Action<string, int> context = null;
            int index = 0;
            foreach (var arg in args) {
                if (string.Equals(arg, "-s") || string.Equals(arg, "--sensor")) {
                    context = _sensor;
                    index = -1;
                }
                else if (string.Equals(arg, "-i") || string.Equals(arg, "--indicator")) {
                    context = _indicator;
                    index = -1;
                }
                else if (string.Equals(arg, "-c") || string.Equals(arg, "--connect")) {
                    AutoConnect = true;
                }
                else if (string.Equals(arg, "-h") || string.Equals(arg, "--help")) {
                    ShowHelpAndAbort();
                }
                else if (context != null) {
                    context(arg, ++index);
                }
            }
        }

        private void ShowHelpAndAbort() {
            MessageBox.Show(
                    $"Command line args: [-c|--connect] -s|--sensor \"SensorType\" \"Key=Value\"* -i|--indicator \"IndicatorType\" \"Key=Value\"*", 
                    $"{AppInfo.Title}: Help", 
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
            );
            throw new OperationCanceledException();
        }

        private void _sensor(string arg, int index) {
            if (index == 0) {
                SensorTypeName = arg;
                SensorProperties = new List<(string, string)>();
            }
            else {
                var keyValue = ParseKeyValue(arg);
                SensorProperties.Add(keyValue);
            }
        }

        private void _indicator(string arg, int index) {
            if (index == 0) {
                IndicatorTypeName = arg;
                IndicatorProperties = new List<(string, string)>();
            }
            else {
                var keyValue = ParseKeyValue(arg);
                IndicatorProperties.Add(keyValue);
            }
        }
    }
}
