using System.Windows.Forms;

namespace TrayMonitor
{
    public partial class AboutForm : Form
    {
        public AboutForm() {
            InitializeComponent();
            Text = $"{AppInfo.Title}: About";
            label1.Text = $"{AppInfo.Title} {AppInfo.Version}";
            textBox1.Text = $"{AppInfo.CommandLine}";
        }
    }
}