using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Printing;
using System.Windows;
using MainPower.DrawingsDatabase.Gui.Properties;

namespace MainPower.DrawingsDatabase.Gui
{
    /// <summary>
    /// Interaction logic for GuiSettings.xaml
    /// </summary>
    public sealed partial class GuiSettings
    {
        public GuiSettings()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string[] subs = Settings.Default.Substations.Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries);

            foreach (string sub in subs)
            {
                txtSubs.AppendText(sub + Environment.NewLine);
            }

            if (Settings.Default.PageSize == PaperKind.A3)
            {
                radioA3.IsChecked = true;
            }
            else
            {
                radioA4.IsChecked = true;
            }
            if (Settings.Default.PageOrientation == PageOrientation.Portrait)
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
            var subs = new List<string>();

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
                Settings.Default["PageSize"] = PaperKind.A3;
            }
            else
            {
                Settings.Default["PageSize"] = PaperKind.A4;
            }

            if (radioPortrait.IsChecked ?? false)
            {
                Settings.Default["PageOrientation"] = PageOrientation.Portrait;
            }
            else
            {
                Settings.Default["PageOrientation"] = PageOrientation.Landscape;
            }

            Settings.Default["Substations"] = setSubs;
            Settings.Default.Save();

            Close();
        }
    }
}