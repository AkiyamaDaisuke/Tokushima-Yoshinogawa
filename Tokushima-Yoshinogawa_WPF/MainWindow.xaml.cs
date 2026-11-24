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

namespace Tokushima_Yoshinogawa_WPF
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

        private void Jogar_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Sobre_Click(object sender, RoutedEventArgs e)
        {
            var sobre = new Sobre();
            sobre.Show();
            this.Close();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            var fmsg = new Mensagens(2, "Deseja encerrar o Jogo?");
            bool? fRetorno = fmsg.ShowDialog();

            if (fRetorno == true)
            {
                Application.Current.Shutdown();
            }

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var fmsg = new Mensagens(2, "Deseja encerrar o Jogo?");
            bool? fRetorno = fmsg.ShowDialog();

            if (fRetorno == true)
            {
                Application.Current.Shutdown();
            }
            if (fRetorno != true)
            {
                e.Cancel = true;   // impede a janela de fechar
            }
        }
    }
}