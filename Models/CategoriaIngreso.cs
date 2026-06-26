using System.Collections.Generic;

namespace Zenimint_Funds.Models
{
    internal class CategoriaIngreso
    {
        public int Id { get; set; }
        public required string Nombre { get; set; } // Ej. "Quincena", "Venta Sistema"
        public required string Icono { get; set; }
        public required string ColorHex { get; set; }

        public List<Ingreso> HistorialIngresos { get; set; } = [];
    }
}
