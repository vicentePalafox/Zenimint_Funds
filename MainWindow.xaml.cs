using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System.IO;
using System.Threading.Tasks;
using Windows.Graphics;
using Windows.Storage;
using Zenimint_Funds.Data;
using Zenimint_Funds.Models;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Zenimint_Funds
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        // 1. Definimos la ruta de la base de datos de forma segura para WinUI
        private string _dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "finanzas.db");

        public MainWindow()
        {
            InitializeComponent();
            Title = "Zenimint Founds - Acceso";

            WindowSize();

            // Esto "enciende" el motor nativo de SQLite para WinUI 3
            SQLitePCL.Batteries_V2.Init();
            _ = InicializarBaseDeDatosAsync();

            ContenedorPrincipal.Navigate(typeof(LoginPage));
        }

        private void WindowSize()
        {
            int ancho = 1824;
            int alto = 1080;

            var displayArea = DisplayArea.GetFromWindowId(AppWindow.Id, DisplayAreaFallback.Primary);

            // 3. Calculamos el centro exacto (Matemática básica: (Pantalla - Ventana) / 2)
            int posicionX = (displayArea.WorkArea.Width - ancho) / 2;
            int posicionY = (displayArea.WorkArea.Height - alto) / 2;

            // 4. Aplicamos el tamaño y la posición al mismo tiempo
            this.AppWindow.MoveAndResize(new RectInt32(posicionX, posicionY, ancho, alto));
        }

        private async Task InicializarBaseDeDatosAsync()
        {
            using var context = new FinanzasContext();
            // Crea el archivo .db y las tablas si no existen
            //await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();

            // Inserta el usuario admin si la tabla está vacía
            if (!await context.Usuarios.AnyAsync())
            {
                context.Usuarios.Add(new Usuario { Username = "admin", Password = "1234" });
                await context.SaveChangesAsync();
            }
        }
    }
}