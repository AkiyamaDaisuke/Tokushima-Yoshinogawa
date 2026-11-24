using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using Tokushima_Yoshinogawa_WPF.Models;

namespace Tokushima_Yoshinogawa_WPF
{
    public partial class Jogando : Window
    {
        private readonly List<JogadorPartida> _jogadores = [];
        private int _jogadorAtivo;
        private int _rodadaAtual = 1;
        private int _limiteRodadas;

        public Jogando()
        {
            InitializeComponent();
        }

        private void Comecar_Click(object sender, RoutedEventArgs e)
        {
            var nome1 = CampoJogador.Text.Trim();
            var cidade1 = CampoCidade.Text.Trim();
            var nome2 = CampoJogador2.Text.Trim();
            var cidade2 = CampoCidade2.Text.Trim();
            if (new[] { nome1, cidade1, nome2, cidade2 }.Any(string.IsNullOrWhiteSpace))
            {
                MessageBox.Show("Preencha o nome e a cidade dos dois jogadores.", "Dados incompletos",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (SelecaoDificuldade.SelectedItem is not ComboBoxItem dificuldadeItem ||
                SelecaoRodadas.SelectedItem is not ComboBoxItem rodadasItem ||
                !int.TryParse(rodadasItem.Tag?.ToString(), out _limiteRodadas))
                return;

            var dificuldade = dificuldadeItem.Content?.ToString() ?? "Normal";
            var satisfacao = dificuldade switch { "Fácil" => 80, "Difícil" => 50, _ => 65 };
            var yenes = dificuldade switch { "Fácil" => 1200, "Difícil" => 800, _ => 1000 };
            var populacao = dificuldade == "Difícil" ? 15 : 20;
            _jogadores.Clear();
            _jogadores.Add(NovoJogador(nome1, cidade1, satisfacao, yenes, populacao));
            _jogadores.Add(NovoJogador(nome2, cidade2, satisfacao, yenes, populacao));
            _jogadorAtivo = 0;
            _rodadaAtual = 1;
            UserData.Dificuldade = dificuldade;
            ResumoDificuldade.Text = $"Dificuldade: {dificuldade}  |  Partida de {_limiteRodadas} rodadas";
            CarregarJogadorAtivo();
            MensagemMercado.Text = "Negocie usando os Yenes e o estoque desta cidade.";
            MensagemRodada.Text = "Plante, ajuste impostos e encerre seu turno. Cada jogador joga uma vez por rodada.";
            MensagemDefesa.Text = "Construa fazendas e quartéis para desenvolver e defender sua cidade.";
            Configuracao.Visibility = Visibility.Collapsed;
            ResumoPartida.Visibility = Visibility.Visible;
        }

        private static JogadorPartida NovoJogador(string nome, string cidade, int satisfacao, int yenes, int populacao) => new()
        {
            Nome = nome,
            Cidade = cidade,
            Satisfacao = satisfacao,
            Yenes = yenes,
            Terras = 20,
            Graos = 30,
            Populacao = populacao,
            Aliquota = 10
        };

        private void Mercado_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button { Tag: string acao } ||
                !int.TryParse(QuantidadePara(acao).Text, NumberStyles.None, CultureInfo.InvariantCulture, out var quantidade) || quantidade <= 0)
            {
                MensagemMercado.Text = "Informe uma quantidade inteira maior que zero.";
                return;
            }

            var graos = acao.StartsWith("graos", StringComparison.Ordinal);
            var comprar = acao.EndsWith("comprar", StringComparison.Ordinal);
            var valor = (long)quantidade * (graos ? (comprar ? 10 : 7) : (comprar ? 100 : 70));
            var estoque = graos ? UserData.Graos : UserData.Terras;
            if (comprar)
            {
                if (valor > UserData.Florins || valor > int.MaxValue)
                {
                    MensagemMercado.Text = "Yenes insuficientes para essa compra.";
                    return;
                }
                UserData.Florins -= (int)valor;
                if (graos) UserData.Graos += quantidade;
                else UserData.Terras += quantidade;
                MensagemMercado.Text = $"Compra realizada: {quantidade} {(graos ? "grãos" : "terras")} por {valor} Yenes.";
            }
            else
            {
                if (quantidade > estoque || (!graos && UserData.Terras - quantidade < UserData.TerrasPlantadas))
                {
                    MensagemMercado.Text = graos ? $"Estoque insuficiente. Você possui {estoque} grãos." : $"Estoque insuficiente ou há terras plantadas. Terras livres: {UserData.Terras - UserData.TerrasPlantadas}.";
                    return;
                }
                if (valor > int.MaxValue - UserData.Florins)
                {
                    MensagemMercado.Text = "Essa venda excede o limite de Yenes do jogo.";
                    return;
                }
                UserData.Florins += (int)valor;
                if (graos) UserData.Graos -= quantidade;
                else UserData.Terras -= quantidade;
                MensagemMercado.Text = $"Venda realizada: {quantidade} {(graos ? "grãos" : "terras")} por {valor} Yenes.";
            }
            AtualizarEconomia();
        }

        private TextBox QuantidadePara(string acao) => acao.StartsWith("graos", StringComparison.Ordinal) ? QuantidadeGraos : QuantidadeTerras;

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
            MensagemRodada.Text = $"Plantio realizado em {quantidade} terra(s). A colheita acontece ao fechar seu turno.";
            AtualizarEconomia();
        }

        private void ConstruirFazenda_Click(object sender, RoutedEventArgs e)
        {
            const int custo = 250;
            if (UserData.Florins < custo)
            {
                MensagemDefesa.Text = "Yenes insuficientes para construir uma fazenda (250 Yenes).";
                return;
            }
            UserData.Florins -= custo;
            UserData.Fazendas++;
            MensagemDefesa.Text = "Fazenda construída. Cada terra plantada passa a render um grão extra.";
            AtualizarEconomia();
        }

        private void ConstruirQuartel_Click(object sender, RoutedEventArgs e)
        {
            const int custo = 400;
            if (UserData.TemQuartel)
            {
                MensagemDefesa.Text = "Sua cidade já possui um quartel.";
                return;
            }
            if (UserData.Florins < custo)
            {
                MensagemDefesa.Text = "Yenes insuficientes para construir um quartel (400 Yenes).";
                return;
            }
            UserData.Florins -= custo;
            UserData.TemQuartel = true;
            MensagemDefesa.Text = "Quartel construído. Agora você pode recrutar até 20 soldados.";
            AtualizarEconomia();
        }

        private void Recrutar_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(QuantidadeSoldados.Text, NumberStyles.None, CultureInfo.InvariantCulture, out var quantidade) || quantidade <= 0)
            {
                MensagemDefesa.Text = "Informe uma quantidade inteira de soldados maior que zero.";
                return;
            }
            if (!UserData.TemQuartel)
            {
                MensagemDefesa.Text = "Construa um quartel antes de recrutar soldados.";
                return;
            }
            if (UserData.Soldados + quantidade > 20)
            {
                MensagemDefesa.Text = "O quartel comporta no máximo 20 soldados.";
                return;
            }
            var custo = (long)quantidade * 60;
            if (custo > UserData.Florins)
            {
                MensagemDefesa.Text = "Yenes insuficientes para esse recrutamento.";
                return;
            }
            UserData.Florins -= (int)custo;
            UserData.Soldados += quantidade;
            MensagemDefesa.Text = $"{quantidade} soldado(s) recrutado(s) por {custo} Yenes.";
            AtualizarEconomia();
        }

        private void FecharRodada_Click(object sender, RoutedEventArgs e)
        {
            var imposto = AliquotaAtual();
            var colheita = UserData.TerrasPlantadas * (3 + UserData.Fazendas);
            UserData.Graos = (int)Math.Min(int.MaxValue, (long)UserData.Graos + colheita);
            UserData.TerrasPlantadas = 0;
            var consumo = Math.Max(1, (UserData.Populacao + 9) / 10);
            var arrecadacao = UserData.Populacao * imposto / 10;
            var manutencao = UserData.Soldados * 2;
            UserData.Florins = Math.Max(0, UserData.Florins + arrecadacao - manutencao);

            if (imposto >= 20) UserData.Satisfacao = Math.Max(0, UserData.Satisfacao - 3);
            else if (imposto <= 5) UserData.Satisfacao = Math.Min(100, UserData.Satisfacao + 1);
            if (UserData.Soldados > 0) UserData.Satisfacao = Math.Min(100, UserData.Satisfacao + Math.Min(2, UserData.Soldados / 5));

            var cresceu = false;
            if (UserData.Graos >= consumo + 5)
            {
                UserData.Graos -= consumo;
                UserData.Populacao++;
                cresceu = true;
            }
            else if (UserData.Graos >= consumo) UserData.Graos -= consumo;
            else
            {
                UserData.Graos = 0;
                UserData.Populacao = Math.Max(1, UserData.Populacao - 1);
                UserData.Satisfacao = Math.Max(0, UserData.Satisfacao - 5);
            }

            SalvarJogadorAtivo();
            if (_jogadorAtivo == 0)
            {
                _jogadorAtivo = 1;
                CarregarJogadorAtivo();
                MensagemRodada.Text = $"Turno de {_jogadores[1].Nome}. Rodada {_rodadaAtual}/{_limiteRodadas}; colheita anterior: {colheita}, impostos: {arrecadacao} Yenes.";
                MensagemDefesa.Text = "Agora é a vez da segunda cidade.";
                return;
            }

            if (_rodadaAtual >= _limiteRodadas)
            {
                EncerrarPartida();
                return;
            }

            _rodadaAtual++;
            _jogadorAtivo = 0;
            UserData.Rodada = _rodadaAtual;
            CarregarJogadorAtivo();
            MensagemRodada.Text = $"Rodada {_rodadaAtual} de {_limiteRodadas} iniciada. Colheita: {colheita}, impostos: {arrecadacao} Yenes, manutenção militar: {manutencao} Yenes." + (cresceu ? " A população cresceu em 1." : string.Empty);
            MensagemDefesa.Text = "Seu turno começou.";
        }

        private int AliquotaAtual() => AliquotaImposto.SelectedItem is ComboBoxItem item && int.TryParse(item.Tag?.ToString(), out var taxa) ? taxa : 10;

        private void AliquotaImposto_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsInitialized && ResumoPartida.Visibility == Visibility.Visible && _jogadores.Count > 0)
                AtualizarEconomia();
        }

        private void CarregarJogadorAtivo()
        {
            var jogador = _jogadores[_jogadorAtivo];
            UserData.NomeJogador = jogador.Nome;
            UserData.NomeCidade = jogador.Cidade;
            UserData.Florins = jogador.Yenes;
            UserData.Terras = jogador.Terras;
            UserData.Graos = jogador.Graos;
            UserData.Populacao = jogador.Populacao;
            UserData.TerrasPlantadas = jogador.TerrasPlantadas;
            UserData.Satisfacao = jogador.Satisfacao;
            UserData.SatisfacaoInicial = jogador.Satisfacao;
            UserData.Fazendas = jogador.Fazendas;
            UserData.TemQuartel = jogador.TemQuartel;
            UserData.Soldados = jogador.Soldados;
            UserData.Rodada = _rodadaAtual;
            AliquotaImposto.SelectedIndex = jogador.Aliquota switch { 5 => 0, 20 => 2, _ => 1 };
            ResumoJogador.Text = $"Jogador: {jogador.Nome}";
            NomeCidade.Text = $"Cidade: {jogador.Cidade}";
            TurnoTexto.Text = $"Vez de {jogador.Nome}  |  Rodada {_rodadaAtual}/{_limiteRodadas}";
            BotaoFecharRodada.Content = $"Encerrar turno de {jogador.Nome}";
            AtualizarEconomia();
        }

        private void SalvarJogadorAtivo()
        {
            var jogador = _jogadores[_jogadorAtivo];
            jogador.Yenes = UserData.Florins;
            jogador.Terras = UserData.Terras;
            jogador.Graos = UserData.Graos;
            jogador.Populacao = UserData.Populacao;
            jogador.TerrasPlantadas = UserData.TerrasPlantadas;
            jogador.Satisfacao = UserData.Satisfacao;
            jogador.Fazendas = UserData.Fazendas;
            jogador.TemQuartel = UserData.TemQuartel;
            jogador.Soldados = UserData.Soldados;
            jogador.Aliquota = AliquotaAtual();
        }

        private void EncerrarPartida()
        {
            var primeiro = _jogadores[0];
            var segundo = _jogadores[1];
            var comparacao = primeiro.Satisfacao.CompareTo(segundo.Satisfacao);
            if (comparacao == 0) comparacao = primeiro.Soldados.CompareTo(segundo.Soldados);
            if (comparacao == 0) comparacao = primeiro.Yenes.CompareTo(segundo.Yenes);
            var resultado = comparacao == 0 ? "A partida terminou empatada!" : $"{(comparacao > 0 ? primeiro.Nome : segundo.Nome)} venceu!";
            ResultadoTexto.Text = $"{resultado}\n\n{primeiro.Cidade} ({primeiro.Nome}): {primeiro.Satisfacao}% de satisfação, {primeiro.Soldados} soldados e {primeiro.Yenes:N0} Yenes.\n\n{segundo.Cidade} ({segundo.Nome}): {segundo.Satisfacao}% de satisfação, {segundo.Soldados} soldados e {segundo.Yenes:N0} Yenes.\n\nA classificação usa satisfação; em empate, soldados e depois Yenes.";
            TelaVitoria.Visibility = Visibility.Visible;
        }

        private void AtualizarEconomia()
        {
            SaldoTexto.Text = $"Saldo: {UserData.Florins:N0} Yenes";
            EstoqueTexto.Text = $"Estoque: {UserData.Graos:N0} grãos  |  {UserData.Terras:N0} terras";
            AgriculturaTexto.Text = $"População: {UserData.Populacao:N0} habitantes  |  Terras plantadas: {UserData.TerrasPlantadas:N0}/{UserData.Terras:N0}  |  Rodada: {_rodadaAtual}";
            GovernoTexto.Text = $"População: {UserData.Populacao:N0}  |  Satisfação: {UserData.Satisfacao}%  |  Imposto: {AliquotaAtual()}%";
            DefesaTexto.Text = $"Fazendas: {UserData.Fazendas}  |  Quartel: {(UserData.TemQuartel ? "construído" : "não construído")}  |  Soldados: {UserData.Soldados}/20";
            ResumoSatisfacao.Text = $"Satisfação: {UserData.Satisfacao}%";
        }

        private void Voltar_Click(object sender, RoutedEventArgs e)
        {
            var menu = new MainWindow();
            menu.Show();
            Close();
        }
    }
}
