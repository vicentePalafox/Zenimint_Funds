using System.Collections.Generic;

namespace Zenimint_Funds.Models
{
    internal class CategoriaGasto
    {
        public int Id { get; set; }
        public required string Nombre { get; set; }
        public required string Icono { get; set; }
        public required string ColorHex { get; set; }

        public List<Gasto> HistorialGastos { get; set; } = [];
    }
}
