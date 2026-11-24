using System;
using System.Collections.Generic;
using System.Text;

namespace Tokushima_Yoshinogawa_WPF.Models
{
    internal class UserData
    {
        public static string NomeJogador { get; set; } = string.Empty;
        public static string NomeCidade { get; set; } = string.Empty;
        public static string Dificuldade { get; set; } = "Normal";
        public static int SatisfacaoInicial { get; set; } = 65;

    }
}
