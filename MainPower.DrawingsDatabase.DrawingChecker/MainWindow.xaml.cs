using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MainPower.DrawingsDatabase.DatabaseHelper;
using Drawing = MainPower.DrawingsDatabase.DatabaseHelper.Drawing;

namespace MainPower.DrawingsDatabase.DrawingChecker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            listBox1.Items.Clear();
            string[] numbers = textBox1.Text.Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);
            foreach (string str in numbers)
            {
                DrawingsDataContext dc = DBCommon.NewDC;
                IQueryable<Drawing> dwg = from d in dc.Drawings where d.Number == str select d;
                var lbi = new ListBoxItem();
                lbi.Content = str;
                if (dwg.Any())
                {
                    lbi.Background = Brushes.LightGreen;
                }
                else
                {
                    lbi.Background = Brushes.Yellow;
                }
                listBox1.Items.Add(lbi);
            }
        }
    }
}