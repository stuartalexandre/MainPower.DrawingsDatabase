using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MainPower.DrawingsDatabase.DatabaseHelper.Properties;
using Microsoft.Win32;

namespace MainPower.DrawingsDatabase.DatabaseHelper
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public sealed partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtConnectionString.Text = Settings.Default.DrawingsConnectionString;
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default["DrawingsConnectionString"] = txtConnectionString.Text;
            Settings.Default.Save();
            this.Close();
        }

    }
}
