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
    public sealed partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            CargarTodo();
        }

        private void CargarTodo()
        {
            using var context = new FinanzasContext();
            listaIngresos.ItemsSource = context.CategoriasIngreso.ToList();
            listaGastos.ItemsSource = context.CategoriasGasto.ToList();
            listaTarjetas.ItemsSource = context.TarjetaCredito.ToList();
            listaDeudores.ItemsSource = context.Deudores.ToList();
        }

        private async void btnGuardarIngreso_Click(object sender, RoutedEventArgs e)
        {
            string nombre = txtNuevoIngreso.Text;
            string? icono = (cmbIcono.SelectedItem as ComboBoxItem)?.Tag.ToString();
            string? colorHex = (cmbColor.SelectedItem as ComboBoxItem)?.Tag.ToString();

            if (string.IsNullOrWhiteSpace(nombre))
            {
                MostrarMensaje("Error", "El nombre de la categoría no puede estar vacío.", InfoBarSeverity.Error);
                return;
            }

            try
            {
                using (var context = new FinanzasContext())
                {
                    var categoria = new CategoriaIngreso
                    {
                        Nombre = nombre,
                        Icono = icono,
                        ColorHex = colorHex
                    };
                    context.CategoriasIngreso.Add(categoria);
                    await context.SaveChangesAsync();
                }

                txtNuevoIngreso.Text = "";
                cmbIcono.SelectedIndex = 0;
                cmbColor.SelectedIndex = 0;
                MostrarMensaje("Éxito", "Categoría de ingreso guardada correctamente.", InfoBarSeverity.Success);

                CargarTodo();
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error", $"Ocurrió un error al guardar la categoría: {ex.Message}", InfoBarSeverity.Error);
            }
        }

        public void btnGuardarGasto_Click(object sender, RoutedEventArgs e)
        {
            string nombre = txtNuevoGasto.Text;
            string? icono = (cmbIconoGasto.SelectedItem as ComboBoxItem)?.Tag.ToString();
            string? colorHex = (cmbColorGasto.SelectedItem as ComboBoxItem)?.Tag.ToString();

            if (string.IsNullOrWhiteSpace(nombre))
            {
                MostrarMensaje("Error", "El nombre de la categoría no puede estar vacío.", InfoBarSeverity.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(icono))
            {
                MostrarMensaje("Error", "Selecciona un icono representativo.", InfoBarSeverity.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(colorHex))
            {
                MostrarMensaje("Error", "Selecciona un color para la etiqueta.", InfoBarSeverity.Error);
                return;
            }

            try
            {
                using (var context = new FinanzasContext())
                {
                    var categoria = new CategoriaGasto
                    {
                        Nombre = nombre,
                        Icono = icono,
                        ColorHex = colorHex
                    };
                    context.CategoriasGasto.Add(categoria);
                    context.SaveChanges();
                }
                txtNuevoGasto.Text = "";
                cmbIconoGasto.SelectedIndex = 0;
                cmbColorGasto.SelectedIndex = 0;
                MostrarMensaje("Éxito", "Categoría de Gasto Registrada.", InfoBarSeverity.Success);
                CargarTodo();
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error", $"Ocurrió un error al guardar la categoría: {ex.Message}", InfoBarSeverity.Error);
            }
        }

        public async void btnGuardarTarjeta_Click(object sender, RoutedEventArgs e)
        {
            string nombre = txtNombre.Text;
            double limite = double.IsNaN(numLimite.Value) ? 0 : numLimite.Value;
            double corte = double.IsNaN(numDiaCorte.Value) ? 0 : numDiaCorte.Value;
            double pago = double.IsNaN(numDiaPago.Value) ? 0 : numDiaPago.Value;
            string? color = (cmbColor.SelectedItem as ComboBoxItem)?.Tag.ToString();

            if (string.IsNullOrWhiteSpace(nombre) || limite <= 0 || corte <= 0 || pago <= 0)
            {
                lblErrorModal.Text = "Llena todos los campos con valores válidos.";
                lblErrorModal.Visibility = Visibility.Visible;

                return;
            }

            try
            {
                using (var context = new FinanzasContext())
                {
                    var tarjeta = new TarjetaCredito()
                    {
                        Nombre = nombre,
                        LimiteCredito = (decimal)limite,
                        DiaCorte = (int)corte,
                        DiaPago = (int)pago,
                        ColorFondo = color
                    };

                    context.TarjetaCredito.Add(tarjeta);
                    await context.SaveChangesAsync();
                }

                txtNombre.Text = string.Empty;
                numLimite.Value = 0;
                numDiaCorte.Value = 0;
                numDiaPago.Value = 0;

                MostrarMensaje("Éxito", "Tarjeta de Crédito Registrada.", InfoBarSeverity.Success);
                CargarTodo();
            }
            catch (Exception ex)
            {
                lblErrorModal.Text = "Error: " + ex.Message;
                lblErrorModal.Visibility = Visibility.Visible;
            }
        }

        private void btnGuardarDeudor_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtNombreDeudor.Text))
            {
                using (var context = new FinanzasContext())
                {
                    context.Deudores.Add(new Deudor { Nombre = txtNombreDeudor.Text });
                    context.SaveChangesAsync();
                }

                MostrarMensaje("Éxito", "Deudor Registrado.", InfoBarSeverity.Success);
                CargarTodo();
                txtNombreDeudor.Text = "";
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
