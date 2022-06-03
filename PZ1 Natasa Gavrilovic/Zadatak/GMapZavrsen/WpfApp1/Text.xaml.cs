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
    /// Interaction logic for Text.xaml
    /// </summary>
    public partial class Text : Window
    {
        public Text()
        {
            InitializeComponent();
            BojaComboBox.ItemsSource = typeof(Colors).GetProperties().Select(p => new Boja(p));

            this.DataContext = this;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            string poruka = string.Empty;
            string greska = string.Empty;
            try
            {
                poruka = sadrzaj.Text;
                if (poruka == string.Empty)
                {
                    greska = "Morate ubaciti sadrzaj poruke.";
                }

                double Thickness = double.Parse(velicina.Text);

                if (Thickness > 0)
                {
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    greska = "Pozitvan broj veci od 0";
                    throw new Exception();

                }



            }
            catch 
            {
                MessageBox.Show("GRESKA: " + greska, "Error", MessageBoxButton.OK);
            }
           
        }
    }
}
