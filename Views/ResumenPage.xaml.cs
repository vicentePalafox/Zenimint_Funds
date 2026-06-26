using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Zenimint_Funds.Data;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Zenimint_Funds.Views
{
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

        private void CalcularResumen()
        {
            using (var context = new FinanzasContext())
            {
                // 1. Calcular Saldo Efectivo (Ingresos - Gastos pagados en Efectivo)
                var totalIngresos = context.Ingresos.Sum(i => i.Monto);
                var gastosEfectivo = context.Gastos
                    .Where(g => g.TarjetaCreditoId == null)
                    .Sum(g => g.MontoTotal);

                var saldoDisponible = totalIngresos - gastosEfectivo;
                txtSaldoEfectivo.Text = string.Format("{0:C2}", saldoDisponible);

                // 2. Calcular Deuda Total (Suma de todos los gastos con tarjeta)
                var deudaTarjetas = context.Gastos
                    .Where(g => g.TarjetaCreditoId != null)
                    .Sum(g => g.MontoTotal);

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
    }
}
