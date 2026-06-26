using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Linq;
using Zenimint_Funds.Data;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Zenimint_Funds.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TarjetaPage : Page
    {
        public TarjetaPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            CargarTarjetas();
        }

        private void CargarTarjetas()
        {
            try
            {
                using (var context = new FinanzasContext())
                {
                    listaTarjetas.ItemsSource = context.TarjetaCredito.ToList();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
