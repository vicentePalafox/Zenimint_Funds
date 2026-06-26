using System;

namespace Zenimint_Funds.Models
{
    class Gasto
    {
        public int Id { get; set; }
        public int CategoriaGastoId { get; set; }
        public string Concepto { get; set; }
        public decimal MontoTotal { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
        public int PlazoMeses { get; set; }

        public decimal MontoMensual => MontoTotal / PlazoMeses;

        public int? TarjetaCreditoId { get; set; }
        public TarjetaCredito Tarjeta { get; set; }

        public int? DeudorId { get; set; }
        public Deudor PersonaDeudora { get; set; }

        public CategoriaGasto Categoria { get; set; }
    }
}
