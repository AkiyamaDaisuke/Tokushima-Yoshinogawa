namespace Tokushima_Yoshinogawa_WPF.Models
{
    internal sealed class JogadorPartida
    {
        public string Nome { get; set; } = string.Empty;
        public string Cidade { get; set; } = string.Empty;
        public int Yenes { get; set; }
        public int Terras { get; set; }
        public int Graos { get; set; }
        public int Populacao { get; set; }
        public int TerrasPlantadas { get; set; }
        public int Satisfacao { get; set; }
        public int Fazendas { get; set; }
        public bool TemQuartel { get; set; }
        public int Soldados { get; set; }
        public int Aliquota { get; set; } = 10;
    }
}
