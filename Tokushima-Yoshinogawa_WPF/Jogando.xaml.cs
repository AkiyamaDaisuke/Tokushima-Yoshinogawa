using System.Windows;
using System.Windows.Controls;
using Tokushima_Yoshinogawa_WPF.Models;

namespace Tokushima_Yoshinogawa_WPF
{
    public partial class Jogando : Window
    {
        public Jogando()
        {
            InitializeComponent();
        }

        private void Comecar_Click(object sender, RoutedEventArgs e)
        {
            var jogador = CampoJogador.Text.Trim();
            var cidade = CampoCidade.Text.Trim();

            if (string.IsNullOrWhiteSpace(jogador) || string.IsNullOrWhiteSpace(cidade))
            {
                MessageBox.Show("Informe o nome do jogador e o nome da cidade para continuar.",
                    "Dados incompletos", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (SelecaoDificuldade.SelectedItem is not ComboBoxItem item)
                return;

            var dificuldade = item.Content?.ToString() ?? "Normal";
            var satisfacaoInicial = dificuldade switch
            {
                "Fácil" => 80,
                "Difícil" => 50,
                _ => 65
            };

            UserData.NomeJogador = jogador;
            UserData.NomeCidade = cidade;
            UserData.Dificuldade = dificuldade;
            UserData.SatisfacaoInicial = satisfacaoInicial;

            ResumoJogador.Text = $"Jogador: {jogador}";
            NomeCidade.Text = $"Cidade: {cidade}";
            ResumoDificuldade.Text = $"Dificuldade: {dificuldade}";
            ResumoSatisfacao.Text = $"Satisfação inicial: {satisfacaoInicial}%";

            Configuracao.Visibility = Visibility.Collapsed;
            ResumoPartida.Visibility = Visibility.Visible;
        }

        private void Voltar_Click(object sender, RoutedEventArgs e)
        {
            var menu = new MainWindow();
            menu.Show();
            Close();
        }
    }
}
