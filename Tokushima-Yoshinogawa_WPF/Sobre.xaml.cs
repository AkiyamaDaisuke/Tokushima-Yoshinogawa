using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Tokushima_Yoshinogawa_WPF
{
    /// <summary>
    /// Lógica interna para Sobre.xaml
    /// </summary>
    public partial class Sobre : Window
    {
        public Sobre()
        {
            InitializeComponent();
        }

        private void Voltar_Click(object sender, RoutedEventArgs e)
        {
            var volta = new MainWindow();
            volta.Show();
            this.Close();
        }
    }
}
