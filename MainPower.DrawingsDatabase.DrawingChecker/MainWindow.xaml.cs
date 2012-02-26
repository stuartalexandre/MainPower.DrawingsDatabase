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
using MPDrawing = MainPower.DrawingsDatabase.DatabaseHelper.Drawing;
using MainPower.DrawingsDatabase.DatabaseHelper;

namespace MainPower.DrawingsDatabase.DrawingChecker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            listBox1.Items.Clear();
            string[] numbers = textBox1.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string str in numbers)
            {
                DrawingsDataContext dc = DBCommon.NewDC;
                var dwg = from d in dc.Drawings where d.Number == str select d;
                ListBoxItem lbi = new ListBoxItem();
                lbi.Content = str;
                if (dwg.Count() > 0)
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
