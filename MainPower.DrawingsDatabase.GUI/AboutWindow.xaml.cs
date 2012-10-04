using System.Windows;
using MainPower.DrawingsDatabase.DatabaseHelper;
//using System.Reflection;

namespace MainPower.DrawingsDatabase.Gui
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public sealed partial class AboutWindow
    {
        public AboutWindow()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            lblGUIAssemblyVersion.Content = new AssemblyInfoHelper(GetType()).AssemblyVersion;
            lblGUIFileVersion.Content = new AssemblyInfoHelper(GetType()).FileVersion;
            lblDBHAssemblyVersion.Content = new AssemblyInfoHelper(typeof (DBCommon)).AssemblyVersion;
            lblDBHFileVersion.Content = new AssemblyInfoHelper(typeof (DBCommon)).FileVersion;
        }
    }
}