using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Linq;
using Zenimint_Funds.Data;
using Zenimint_Funds.Models;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Zenimint_Funds.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class IngresosPage : Page
    {
        public IngresosPage()
        {
            InitializeComponent();
        }

        private void CargarDatos()
        {
            try
            {
                using (var context = new FinanzasContext())
                {
                    // 1. Llenamos las categorías en el ComboBox
                    cmbCategoria.ItemsSource = context.CategoriasIngreso.ToList();

                    // 2. Llenamos el historial de ingresos
                    // Usamos .Include() para poder acceder al Icono de la Categoría en el XAML
                    listaIngresos.ItemsSource = context.Ingresos
                                                       .Include(i => i.Categoria)
                                                       .OrderByDescending(i => i.Fecha)
                                                       .ToList();
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error al cargar datos", ex.Message, InfoBarSeverity.Error);
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            CargarDatos();
        }

        private async void btnGuardarIngreso_Click(object sender, RoutedEventArgs e)
        {
            string concepto = txtConcepto.Text;
            double monto = double.IsNaN(numMonto.Value) ? 0 : numMonto.Value;
            var categoriaSeleccionada = cmbCategoria.SelectedItem as CategoriaIngreso;

            // Validación de los campos
            if (string.IsNullOrWhiteSpace(concepto) || monto <= 0 || categoriaSeleccionada == null)
            {
                MostrarMensaje("Error", "Llena el concepto, un monto mayor a cero y selecciona una categoría.", InfoBarSeverity.Error);
                return;
            }

            try
            {
                using (var context = new FinanzasContext())
                {
                    // Creamos la transacción real
                    var nuevoIngreso = new Ingreso
                    {
                        Concepto = concepto,
                        Monto = (decimal)monto,
                        Fecha = DateTime.Now,
                        CategoriaIngresoId = categoriaSeleccionada.Id
                    };

                    context.Ingresos.Add(nuevoIngreso);
                    await context.SaveChangesAsync();
                }

                // Limpiamos los controles para una nueva captura
                txtConcepto.Text = "";
                numMonto.Value = double.NaN;
                cmbCategoria.SelectedIndex = -1;

                MostrarMensaje("¡Éxito!", "El ingreso se ha sumado a tu saldo disponible.", InfoBarSeverity.Success);

                // Refrescamos la lista para ver la nueva transacción
                CargarDatos();
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error de Base de Datos", ex.Message, InfoBarSeverity.Error);
            }
        }

        private void MostrarMensaje(string titulo, string msj, InfoBarSeverity severity)
        {
            Notificacion.Title = titulo;
            Notificacion.Message = msj;
            Notificacion.Severity = severity;
            Notificacion.IsOpen = true;
        }
    }
}
