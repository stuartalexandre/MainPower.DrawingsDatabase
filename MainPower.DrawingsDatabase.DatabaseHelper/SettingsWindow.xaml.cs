using System.Windows;
using MainPower.DrawingsDatabase.DatabaseHelper.Properties;

namespace MainPower.DrawingsDatabase.DatabaseHelper
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public sealed partial class SettingsWindow
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            txtConnectionString.Text = Settings.Default.DrawingsConnectionString;
        }

        private void BtnOkClick(object sender, RoutedEventArgs e)
        {
            Settings.Default["DrawingsConnectionString"] = txtConnectionString.Text;
            Settings.Default.Save();
            Close();
        }
    }
}