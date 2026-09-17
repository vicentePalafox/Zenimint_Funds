using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Zenimint_Funds.Data;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Zenimint_Funds.Views
{
    public class AlertaTarjeta
    {
        public string Titulo { get; set; }
        public string Mensaje { get; set; }
        public Microsoft.UI.Xaml.Controls.InfoBarSeverity Severidad { get; set; }
    }

    public class ResumenTarjeta
    {
        public string Nombre { get; set; }
        public double Porcentaje { get; set; }
        public string TextoDetalle { get; set; }
        public Microsoft.UI.Xaml.Media.SolidColorBrush ColorBarra { get; set; }
    }

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ResumenPage : Page
    {
        public ResumenPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            CalcularResumen();
        }

        private void cmbFiltroMes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Cada vez que el usuario cambia el menú, recalculamos todo
            CalcularResumen();
        }

        private void CalcularResumen()
        {
            // Prevenimos que se ejecute si la página aún no carga completamente
            if (cmbFiltroMes == null || txtSaldoEfectivo == null) return;

            DateTime fechaInicio = DateTime.MinValue;
            DateTime fechaFin = DateTime.MaxValue;
            DateTime hoy = DateTime.Now;

            int filtroSeleccionado = cmbFiltroMes.SelectedIndex;

            if (filtroSeleccionado == 0) // Mes Actual
            {
                fechaInicio = new DateTime(hoy.Year, hoy.Month, 1);
                fechaFin = fechaInicio.AddMonths(1).AddDays(-1);
            }
            else if (filtroSeleccionado == 1) // Mes Anterior
            {
                DateTime mesAnterior = hoy.AddMonths(-1);
                fechaInicio = new DateTime(mesAnterior.Year, mesAnterior.Month, 1);
                fechaFin = fechaInicio.AddMonths(1).AddDays(-1);
            }

            using (var context = new FinanzasContext())
            {
                CalcularAlertas(context);
                CalcularTermometros(context);

                // Ingresos del periodo
                var totalIngresos = context.Ingresos
                    .Where(i => i.Fecha >= fechaInicio && i.Fecha <= fechaFin)
                    .Sum(i => (double)i.Monto);

                // Gastos en efectivo del periodo
                var gastosEfectivo = context.Gastos
                    .Where(g => g.TarjetaCreditoId == null && g.Fecha >= fechaInicio && g.Fecha <= fechaFin)
                    .Sum(g => (double)g.MontoTotal);

                var saldoDisponible = totalIngresos - gastosEfectivo;
                txtSaldoEfectivo.Text = string.Format("{0:C2}", saldoDisponible);

                // Deuda en Tarjetas (La deuda de tarjeta normalmente queremos verla acumulada, 
                // pero puedes aplicarle el filtro si prefieres ver solo lo gastado este mes)
                var deudaTarjetas = context.Gastos
                    .Where(g => g.TarjetaCreditoId != null && g.Fecha >= fechaInicio && g.Fecha <= fechaFin)
                    .Sum(g => (double)g.MontoTotal);

                txtDeudaTotal.Text = string.Format("-{0:C2}", deudaTarjetas);

                // 3. Obtener saldos por Deudor
                var saldosDeudores = context.Deudores
                    .Include(d => d.GastosAsignados)
                    .Select(d => new
                    {
                        Nombre = d.Nombre,
                        TotalDeuda = d.GastosAsignados.Sum(g => g.MontoTotal)
                    })
                    .Where(d => d.TotalDeuda > 0)
                    .ToList();

                listaDeudores.ItemsSource = saldosDeudores;

                // 4. Configurar Gráfica de Tarjetas
                ConfigurarGrafica(context);
            }
        }

        private void ConfigurarGrafica(FinanzasContext context)
        {
            // Agrupamos los gastos por nombre de tarjeta
            var datosGrafica = context.Gastos
                .Where(g => g.TarjetaCreditoId != null)
                .Include(g => g.Tarjeta)
                .GroupBy(g => g.Tarjeta.Nombre)
                .Select(grupo => new
                {
                    Nombre = grupo.Key,
                    Total = (double)grupo.Sum(g => g.MontoTotal)
                }).ToList();

            // Configuramos LiveCharts2 (Necesitas instalar el NuGet: LiveChartsCore.SkiaSharpView.WinUI)
            chartTarjetas.Series = datosGrafica.Select(d => new ColumnSeries<double>
            {
                Name = d.Nombre,
                Values = new double[] { d.Total }
            }).Cast<ISeries>().ToArray();
        }

        private void CalcularTermometros(FinanzasContext context)
        {
            var resumenTarjetas = new System.Collections.Generic.List<ResumenTarjeta>();
            var tarjetas = context.TarjetaCredito.ToList();

            foreach (var tarjeta in tarjetas)
            {
                // Sumamos cuánto debemos en esta tarjeta
                var deuda = context.Gastos
                    .Where(g => g.TarjetaCreditoId == tarjeta.Id)
                    .Sum(g => (double)g.MontoTotal);

                // Necesitamos asegurar que el límite sea mayor a 0 para no causar un error de división por cero
                if (tarjeta.LimiteCredito > 0)
                {
                    // Regla de 3 simple para sacar el porcentaje
                    double porcentaje = (deuda / (double)tarjeta.LimiteCredito) * 100;

                    // Motor de color inteligente
                    var color = Microsoft.UI.Colors.MediumSeaGreen; // Verde por defecto (menos del 50%)

                    if (porcentaje >= 80)
                    {
                        color = Microsoft.UI.Colors.Crimson; // Rojo (Peligro)
                    }
                    else if (porcentaje >= 50)
                    {
                        color = Microsoft.UI.Colors.Orange; // Naranja (Precaución)
                    }

                    resumenTarjetas.Add(new ResumenTarjeta
                    {
                        Nombre = tarjeta.Nombre,
                        Porcentaje = porcentaje,
                        TextoDetalle = $"{deuda:C2} / {tarjeta.LimiteCredito:C2}",
                        ColorBarra = new Microsoft.UI.Xaml.Media.SolidColorBrush(color)
                    });
                }
            }

            // Dibujamos las barras en la pantalla
            listaTermometros.ItemsSource = resumenTarjetas;
        }

        private void CalcularAlertas(FinanzasContext context)
        {
            var alertas = new System.Collections.Generic.List<AlertaTarjeta>();
            var tarjetas = context.TarjetaCredito.ToList();
            DateTime hoy = DateTime.Now.Date; // Solo nos importa el día, no la hora

            foreach (var tarjeta in tarjetas)
            {
                // 1. Calculamos la fecha exacta del próximo pago
                DateTime proximoPago;

                // Prevención de error por si el día de pago es 31 y el mes actual tiene 30 días
                int diasEnMesActual = DateTime.DaysInMonth(hoy.Year, hoy.Month);
                int diaPagoAjustado = Math.Min(tarjeta.DiaPago, diasEnMesActual);

                if (hoy.Day <= diaPagoAjustado)
                {
                    // El pago es este mismo mes
                    proximoPago = new DateTime(hoy.Year, hoy.Month, diaPagoAjustado);
                }
                else
                {
                    // El pago ya pasó este mes, brincamos al mes siguiente
                    DateTime mesSiguiente = hoy.AddMonths(1);
                    int diasEnMesSiguiente = DateTime.DaysInMonth(mesSiguiente.Year, mesSiguiente.Month);
                    proximoPago = new DateTime(mesSiguiente.Year, mesSiguiente.Month, Math.Min(tarjeta.DiaPago, diasEnMesSiguiente));
                }

                // 2. ¿Cuántos días faltan?
                int diasParaVencer = (proximoPago - hoy).Days;

                // 3. Si faltan 5 días o menos (y no es un número negativo), disparamos la alerta
                if (diasParaVencer <= 5 && diasParaVencer >= 0)
                {
                    // Calculamos cuánto debemos en esta tarjeta específica
                    var deudaTarjeta = context.Gastos
                        .Where(g => g.TarjetaCreditoId == tarjeta.Id)
                        .Sum(g => (double)g.MontoTotal);

                    // Solo avisamos si realmente hay deuda
                    if (deudaTarjeta > 0)
                    {
                        alertas.Add(new AlertaTarjeta
                        {
                            Titulo = $"¡Pago próximo: {tarjeta.Nombre}!",
                            Mensaje = $"Tu fecha límite es en {diasParaVencer} día(s). Tienes consumos por {deudaTarjeta:C2}.",
                            Severidad = diasParaVencer <= 2 ? InfoBarSeverity.Error : InfoBarSeverity.Warning
                        });
                    }
                }
            }

            // Dibujamos las alertas en la pantalla
            listaAlertas.ItemsSource = alertas;
        }
    }
}
