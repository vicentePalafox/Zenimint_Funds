using Microsoft.EntityFrameworkCore;
using System.IO;
using Windows.Storage;
using Zenimint_Funds.Models;

namespace Zenimint_Funds.Data
{
    internal class FinanzasContext : DbContext
    {
        // Esta propiedad representa tu tabla 'Usuarios'
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<CategoriaIngreso> CategoriasIngreso { get; set; }
        public DbSet<CategoriaGasto> CategoriasGasto { get; set; }
        public DbSet<Ingreso> Ingresos { get; set; }
        public DbSet<TarjetaCredito> TarjetaCredito { get; set; }
        public DbSet<Gasto> Gastos { get; set; }
        public DbSet<Deudor> Deudores { get; set; }

        // Aquí le decimos a EF Core DÓNDE está la base de datos y QUÉ motor usar (SQLite)
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "finanzas.db");
            optionsBuilder.UseSqlite($"Data Source={dbPath}");
        }
    }
}