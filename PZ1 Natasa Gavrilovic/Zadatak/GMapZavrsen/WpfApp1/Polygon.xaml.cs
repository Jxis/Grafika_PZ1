using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WpfApp1.Common;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for Polygon.xaml
    /// </summary>
    public partial class Polygon : Window
    {
        public Polygon()
        {
            InitializeComponent();

            BojaComboBox.ItemsSource = typeof(Colors).GetProperties().Select(p => new Boja(p));
            ColorComboBox.ItemsSource = typeof(Colors).GetProperties().Select(p => new Boja(p));
            TextComboBox.ItemsSource = typeof(Colors).GetProperties().Select(p => new Boja(p));
            ColorComboBox.SelectedValue = Colors.AliceBlue;
            BojaComboBox.SelectedValue = Colors.AliceBlue;

            this.DataContext = this;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            string poruka = string.Empty;
            try
            {
                poruka = "Broj";
                double Thickness = double.Parse(debljina.Text);
                string Tekst = textContenct.Text.ToString();

                if (Thickness > 0)
                {
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    poruka = "Pozitvan broj veci od 0";
                    throw new Exception();

                }

            }
            catch
            {
                MessageBox.Show("Border Thickness mora biti " + poruka, "Error", MessageBoxButton.OK);
            }
        }
    }
}
