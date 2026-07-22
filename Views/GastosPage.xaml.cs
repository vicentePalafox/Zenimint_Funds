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
    public sealed partial class GastosPage : Page
    {
        public GastosPage()
        {
            InitializeComponent();
        }

        private void CargarCatalogos()
        {
            try
            {
                using var context = new FinanzasContext();
                cmbTarjeta.ItemsSource = context.TarjetaCredito.ToList();
                cmbDeudor.ItemsSource = context.Deudores.ToList();
            }
            catch (Exception ex)
            {
                MostrarMensaje("ERROR", " No se pudieron cargar los catálogos." + ex.Message, InfoBarSeverity.Error);
            }
        }

        private void CargarGastos()
        {
            try
            {
                using (var context = new FinanzasContext())
                {
                    listaGastos.ItemsSource = context.Gastos
                        .Include(g => g.Tarjeta)
                        .Include(g => g.PersonaDeudora)
                        .OrderByDescending(g => g.Fecha)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje("ERROR", " No se pudieron cargar los catálogos." + ex.Message, InfoBarSeverity.Error);
            }
        }

        private void MetodoPago_Changed(object sender, RoutedEventArgs e)
        {
            // Validamos que los controles ya existan en memoria para evitar errores al cargar
            if (cmbTarjeta == null || tsMSI == null) return;

            if (optTarjeta.IsChecked == true)
            {
                cmbTarjeta.Visibility = Visibility.Visible;
                tsMSI.Visibility = Visibility.Visible;
            }
            else
            {
                cmbTarjeta.Visibility = Visibility.Collapsed;
                tsMSI.Visibility = Visibility.Collapsed;
                tsMSI.IsOn = false; // Apagamos los MSI si regresa a efectivo
            }
        }

        private void tsMSI_Toggled(object sender, RoutedEventArgs e)
        {
            numMeses.Visibility = tsMSI.IsOn ? Visibility.Visible : Visibility.Collapsed;
        }

        private void tsPrestamo_Toggled(object sender, RoutedEventArgs e)
        {
            panelDeudor.Visibility = tsPrestamo.IsOn ? Visibility.Visible : Visibility.Collapsed;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            CargarCatalogos();
            CargarGastos();
        }

        private async void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            string concepto = txtConcepto.Text;
            double monto = double.IsNaN(numMonto.Value) ? 0 : numMonto.Value;

            try
            {
                if (string.IsNullOrWhiteSpace(concepto) || monto <= 0)
                {
                    MostrarMensaje("Error", "Ingresa un concepto y un monto válido.", InfoBarSeverity.Error);
                    return;
                }

                using (var context = new FinanzasContext())
                {
                    var gasto = new Gasto
                    {
                        Concepto = concepto,
                        MontoTotal = (decimal)monto,
                        Fecha = DateTime.Now
                    };

                    if (optTarjeta.IsChecked == true)
                    {
                        TarjetaCredito? tarjetaSeleccionada = cmbTarjeta.SelectedItem as TarjetaCredito;
                        if (tarjetaSeleccionada == null)
                        {
                            MostrarMensaje("Error", "Selecciona una tarjeta de crédito.", InfoBarSeverity.Error);
                            return;
                        }
                        gasto.TarjetaCreditoId = tarjetaSeleccionada.Id;

                        gasto.PlazoMeses = tsMSI.IsOn ? (int)numMeses.Value : 1;
                    }
                    else
                    {
                        gasto.TarjetaCreditoId = null;
                        gasto.PlazoMeses = 1;
                    }

                    if (tsPrestamo.IsOn)
                    {
                        Deudor? deudorSeleccionado = cmbDeudor.SelectedItem as Deudor;
                        if (deudorSeleccionado == null)
                        {
                            MostrarMensaje("Error", "Selecciona un deudor.", InfoBarSeverity.Error);
                            return;
                        }
                        gasto.DeudorId = deudorSeleccionado.Id;
                    }
                    else
                    {
                        gasto.DeudorId = null;
                    }

                    context.Gastos.Add(gasto);
                    await context.SaveChangesAsync();

                    CargarGastos();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private void MostrarMensaje(string titulo, string mensaje, InfoBarSeverity severidad)
        {
            Notificacion.Title = titulo;
            Notificacion.Message = mensaje;
            Notificacion.Severity = severidad;
            Notificacion.IsOpen = true;
        }
    }
}
