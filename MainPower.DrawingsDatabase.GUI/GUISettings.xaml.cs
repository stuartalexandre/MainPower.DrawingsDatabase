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
using System.Drawing.Printing;
using System.Printing;

namespace MainPower.DrawingsDatabase.Gui
{
    /// <summary>
    /// Interaction logic for GuiSettings.xaml
    /// </summary>
    public sealed partial class GuiSettings : Window
    {
        public GuiSettings()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string[] subs = Properties.Settings.Default.Substations.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string sub in subs)
            {
                txtSubs.AppendText(sub + Environment.NewLine);
            }

            if (Properties.Settings.Default.PageSize == PaperKind.A3)
            {
                radioA3.IsChecked = true;
            }
            else
            {
                radioA4.IsChecked = true;
            }
            if (Properties.Settings.Default.PageOrientation == PageOrientation.Portrait)
            {
                radioPortrait.IsChecked = true;
            }
            else
            {
                radioLandscape.IsChecked = true;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            List<string> subs = new List<string>();
            
            for (int i = 0; i < txtSubs.LineCount; i++)
            {
                string sub = txtSubs.GetLineText(i);
                if (!String.IsNullOrEmpty(sub))
                {
                    subs.Add(sub);
                }
            }

            string setSubs = "";

            foreach (string sub in subs)
            {
                setSubs += sub.Trim() + ";";
            }

            if (radioA3.IsChecked ?? false)
            {
                Properties.Settings.Default["PageSize"] = PaperKind.A3;
            }
            else
            {
                Properties.Settings.Default["PageSize"] = PaperKind.A4;
            }

            if (radioPortrait.IsChecked ?? false)
            {
                Properties.Settings.Default["PageOrientation"] = PageOrientation.Portrait;
            }
            else
            {
                Properties.Settings.Default["PageOrientation"] = PageOrientation.Landscape;
            }

            Properties.Settings.Default["Substations"] = setSubs;
            Properties.Settings.Default.Save();

            this.Close();
        }
    }
}
