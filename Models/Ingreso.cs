using System;

namespace Zenimint_Funds.Models
{
    internal class Ingreso
    {
        public int Id { get; set; }
        public string Concepto { get; set; } = string.Empty;
        public decimal Monto { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;

        // LLAVE FORÁNEA: Así le decimos a qué categoría pertenece este dinero
        public int CategoriaIngresoId { get; set; }

        // PROPIEDAD DE NAVEGACIÓN: Nos permite acceder a los datos visuales 
        // (Nombre, Color, Icono) directamente sin hacer un inner join manual.
        public CategoriaIngreso Categoria { get; set; }

        // --- SOLUCIÓN PARA WINUI 3 ---
        // C# formatea el texto perfectamente y el XAML solo lo lee

        public string FechaFormateada => Fecha.ToString("dd/MM/yyyy");

        // La "N2" le pone comas a los miles y dos decimales (Ej. + $1,500.00)
        public string MontoFormateado => $"+ ${Monto:N2}";
    }
}
