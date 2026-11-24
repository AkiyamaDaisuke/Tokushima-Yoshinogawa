using System.Windows;
using System.Windows.Controls;
using System.Globalization;
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
            UserData.Florins = dificuldade switch
            {
                "Fácil" => 1200,
                "Difícil" => 800,
                _ => 1000
            };
            UserData.Terras = 20;
            UserData.Graos = 30;
            UserData.Populacao = dificuldade == "Difícil" ? 15 : 20;
            UserData.TerrasPlantadas = 0;
            UserData.Rodada = 1;
            UserData.Satisfacao = satisfacaoInicial;

            ResumoJogador.Text = $"Jogador: {jogador}";
            NomeCidade.Text = $"Cidade: {cidade}";
            ResumoDificuldade.Text = $"Dificuldade: {dificuldade}";
            ResumoSatisfacao.Text = $"Satisfação inicial: {satisfacaoInicial}%";
            MensagemMercado.Text = "Use os botões para negociar. As operações atualizam o saldo e o estoque imediatamente.";
            MensagemRodada.Text = "Plante grãos, defina os impostos e feche a rodada para colher, alimentar a população e arrecadar.";
            AtualizarEconomia();

            Configuracao.Visibility = Visibility.Collapsed;
            ResumoPartida.Visibility = Visibility.Visible;
        }

        private void Mercado_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button { Tag: string acao } ||
                !int.TryParse(QuantidadePara(acao).Text, NumberStyles.None, CultureInfo.InvariantCulture, out var quantidade) ||
                quantidade <= 0)
            {
                MensagemMercado.Text = "Informe uma quantidade inteira maior que zero.";
                return;
            }

            var graos = acao.StartsWith("graos", StringComparison.Ordinal);
            var comprar = acao.EndsWith("comprar", StringComparison.Ordinal);
            var preco = graos ? (comprar ? 10 : 7) : (comprar ? 100 : 70);
            var valorTotal = (long)quantidade * preco;
            var estoque = graos ? UserData.Graos : UserData.Terras;

            if (comprar)
            {
                if (valorTotal > UserData.Florins)
                {
                    MensagemMercado.Text = "Yenes insuficientes para essa compra.";
                    return;
                }

                UserData.Florins -= (int)valorTotal;
                if (graos) UserData.Graos += quantidade;
                else UserData.Terras += quantidade;
                MensagemMercado.Text = $"Compra realizada: {quantidade} {(graos ? "unidades de grãos" : "terras")} por {valorTotal} Yenes.";
            }
            else
            {
                if (quantidade > estoque || (!graos && UserData.Terras - quantidade < UserData.TerrasPlantadas))
                {
                    MensagemMercado.Text = graos
                        ? $"Estoque insuficiente. Você possui {estoque} unidades de grãos."
                        : $"Estoque insuficiente ou há terras plantadas. Você possui {UserData.Terras - UserData.TerrasPlantadas} terras livres.";
                    return;
                }

                UserData.Florins += (int)valorTotal;
                if (graos) UserData.Graos -= quantidade;
                else UserData.Terras -= quantidade;
                MensagemMercado.Text = $"Venda realizada: {quantidade} {(graos ? "unidades de grãos" : "terras")} por {valorTotal} Yenes.";
            }

            AtualizarEconomia();
        }

        private TextBox QuantidadePara(string acao) => acao.StartsWith("graos", StringComparison.Ordinal)
            ? QuantidadeGraos
            : QuantidadeTerras;

        private void AtualizarEconomia()
        {
            SaldoTexto.Text = $"Saldo: {UserData.Florins:N0} Yenes";
            EstoqueTexto.Text = $"Estoque: {UserData.Graos:N0} grãos  |  {UserData.Terras:N0} terras";
            AgriculturaTexto.Text = $"População: {UserData.Populacao:N0} habitantes  |  Terras plantadas: {UserData.TerrasPlantadas:N0} / {UserData.Terras:N0}  |  Rodada: {UserData.Rodada}";
            GovernoTexto.Text = $"População: {UserData.Populacao:N0}  |  Satisfação: {UserData.Satisfacao}%  |  Imposto atual: {AliquotaAtual()}%";
            ResumoSatisfacao.Text = $"Satisfação: {UserData.Satisfacao}%";
        }

        private void Plantar_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(QuantidadePlantio.Text, NumberStyles.None, CultureInfo.InvariantCulture, out var quantidade) || quantidade <= 0)
            {
                MensagemRodada.Text = "Informe uma quantidade inteira de terras maior que zero.";
                return;
            }

            var terrasLivres = UserData.Terras - UserData.TerrasPlantadas;
            if (quantidade > terrasLivres || quantidade > UserData.Graos)
            {
                MensagemRodada.Text = $"Recursos insuficientes. Terras livres: {terrasLivres}; grãos para sementes: {UserData.Graos}.";
                return;
            }

            UserData.TerrasPlantadas += quantidade;
            UserData.Graos -= quantidade;
            MensagemRodada.Text = $"Plantio realizado em {quantidade} terra(s). A colheita acontece ao fechar a rodada.";
            AtualizarEconomia();
        }

        private void FecharRodada_Click(object sender, RoutedEventArgs e)
        {
            var imposto = AliquotaAtual();
            var colheita = UserData.TerrasPlantadas * 3;
            UserData.Graos += colheita;
            UserData.TerrasPlantadas = 0;

            var consumo = Math.Max(1, (UserData.Populacao + 9) / 10);
            var arrecadacao = UserData.Populacao * imposto / 10;
            UserData.Florins += arrecadacao;
            if (imposto >= 20)
                UserData.Satisfacao = Math.Max(0, UserData.Satisfacao - 3);
            else if (imposto <= 5)
                UserData.Satisfacao = Math.Min(100, UserData.Satisfacao + 1);

            var cresceu = false;
            if (UserData.Graos >= consumo + 5)
            {
                UserData.Graos -= consumo;
                UserData.Populacao++;
                cresceu = true;
            }
            else if (UserData.Graos >= consumo)
            {
                UserData.Graos -= consumo;
            }
            else
            {
                UserData.Graos = 0;
                UserData.Populacao = Math.Max(1, UserData.Populacao - 1);
                UserData.Satisfacao = Math.Max(0, UserData.Satisfacao - 5);
            }

            UserData.Rodada++;
            MensagemRodada.Text = $"Rodada concluída: colheita de {colheita} grãos, consumo de {consumo} grãos e arrecadação de {arrecadacao} Yenes." +
                                  (cresceu ? " A população cresceu em 1 habitante." : string.Empty);
            AtualizarEconomia();
        }

        private int AliquotaAtual()
        {
            return AliquotaImposto.SelectedItem is ComboBoxItem item &&
                   int.TryParse(item.Tag?.ToString(), out var aliquota) ? aliquota : 10;
        }

        private void Voltar_Click(object sender, RoutedEventArgs e)
        {
            var menu = new MainWindow();
            menu.Show();
            Close();
        }
    }
}
