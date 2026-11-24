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
        public static int Florins { get; set; } = 1000; //Seria florins, mas lembrei que estamos no japão e lá usamos yenes. não alterei aqui para yenes, apenas na interface do usuario
        public static int Terras { get; set; } = 20;
        public static int Graos { get; set; } = 30;
        public static int Populacao { get; set; } = 20;
        public static int TerrasPlantadas { get; set; }
        public static int Rodada { get; set; }
        public static int Satisfacao { get; set; } = 65;

    }
}
