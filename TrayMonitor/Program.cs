using System;
using System.Windows.Forms;

namespace TrayMonitor
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args) {
            CommandLineArgs config;
            try {
                config = new CommandLineArgs(args);
            }
            catch (OperationCanceledException) {
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            try {
                var app = new App(config);
                Application.Run(app);
            }
            catch (Exception ex) {
                MessageBox.Show(
                        $"{ex.Message}\n\n{ex}", 
                        $"{AppInfo.Title}: Error", 
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                );
            }
        }
    }
}
