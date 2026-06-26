namespace Zenimint_Funds.Models
{
    internal class TarjetaCredito
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public decimal LimiteCredito { get; set; }
        public int DiaCorte { get; set; }
        public int DiaPago { get; set; }
        public string ColorFondo { get; set; } = string.Empty;
    }
}
