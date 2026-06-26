using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Zenimint_Funds.Views;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Zenimint_Funds
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DashboardPage : Page
    {
        public DashboardPage()
        {
            InitializeComponent();
        }

        private void NavView_Loaded(object sender, RoutedEventArgs e)
        {
            // Seleccionamos el primer elemento (Resumen) por defecto al entrar
            NavView.SelectedItem = NavView.MenuItems[0];
        }

        private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            // Verificamos si el usuario hizo clic en el botón de Configuración (engrane) que viene por defecto
            if (args.IsSettingsSelected)
            {
                // Aquí podrías navegar a una página de SettingsPage si la tuvieras
                ContentFrame.Navigate(typeof(SettingsPage));
                return;
            }

            // Obtenemos el elemento que fue seleccionado
            var itemSeleccionado = args.SelectedItem as NavigationViewItem;

            if (itemSeleccionado != null)
            {
                // Usamos el 'Tag' que definimos en el XAML para saber a dónde ir
                switch (itemSeleccionado.Tag.ToString())
                {
                    case "Resumen":
                        ContentFrame.Navigate(typeof(ResumenPage));
                        break;
                    case "Tarjetas":
                        ContentFrame.Navigate(typeof(TarjetaPage));
                        break;
                    case "Ingresos":
                        ContentFrame.Navigate(typeof(IngresosPage));
                        break;
                    case "Gastos":
                        ContentFrame.Navigate(typeof(GastosPage));
                        break;
                }
            }
        }
    }
}
