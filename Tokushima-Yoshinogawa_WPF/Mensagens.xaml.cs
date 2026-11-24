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
    /// Lógica interna para Mensagens.xaml
    /// </summary>
    public partial class Mensagens : Window
    {
        public Mensagens(int caso, string mensagem)
        {
            InitializeComponent();

            Mensagem.Text = mensagem;

            if (caso == 1)
            {
                BotaoOk.HorizontalAlignment = HorizontalAlignment.Center;
                BotaoCancel.Visibility = Visibility.Collapsed;
            }
            else if (caso == 2)
            {
                BotaoCancel.Visibility = Visibility.Visible;
                BotaoOk.Visibility = Visibility.Visible;
            }
            else
            {
                BotaoOk.Content = "Sim";
                BotaoCancel.Content = "Não";
            }
        }

        private void BotaoOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void BotaoCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
