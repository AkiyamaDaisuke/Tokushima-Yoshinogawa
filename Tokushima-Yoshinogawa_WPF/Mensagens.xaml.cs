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
                BotaoOk.Visibility = Visibility.Visible;
                BotaoOk.HorizontalAlignment = HorizontalAlignment.Center;
                BotaoCancelar.Visibility = Visibility.Collapsed;
            }
            else if (caso == 2)
            {
                BotaoOk.Visibility = Visibility.Visible;
                BotaoCancelar.Visibility = Visibility.Visible;
            }
            else
            {
                BotaoOk.Content = "Sim";
                BotaoCancelar.Content = "Não";
            }
        }

        private void BotaoOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void BotaoCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
