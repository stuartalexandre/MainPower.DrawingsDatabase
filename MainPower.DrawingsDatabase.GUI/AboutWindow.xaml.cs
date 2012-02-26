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
using System.Windows.Shapes;
using HC.Utils;
//using System.Reflection;
using MainPower.DrawingsDatabase.DatabaseHelper;

namespace MainPower.DrawingsDatabase.Gui
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public sealed partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            lblGUIAssemblyVersion.Content = new AssemblyInfoHelper(this.GetType()).AssemblyVersion;
            lblGUIFileVersion.Content = new AssemblyInfoHelper(this.GetType()).FileVersion;
            lblDBHAssemblyVersion.Content = new AssemblyInfoHelper(typeof(DBCommon)).AssemblyVersion;
            lblDBHFileVersion.Content = new AssemblyInfoHelper(typeof(DBCommon)).FileVersion;
        }
    }
}
