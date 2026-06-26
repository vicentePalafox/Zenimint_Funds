using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Linq;
using Windows.Security.Credentials.UI;
using Zenimint_Funds.Data;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Zenimint_Funds
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private async void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string usuario = txtUsuario.Text;
            string password = txtPassword.Password;

            if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(password))
            {
                lblError.Text = "Por favor llena todos los campos";
                lblError.Visibility = Visibility.Visible;
                return;
            }

            using (var context = new FinanzasContext())
            {
                // Consulta con Entity Framework Core
                var userDb = await context.Usuarios
                    .FirstOrDefaultAsync(u => u.Username == usuario && u.Password == password);

                if (userDb != null)
                {
                    // ÉXITO
                    lblError.Visibility = Visibility.Collapsed;

                    // Navegamos al Dashboard usando el Frame de la página actual
                    this.Frame.Navigate(typeof(DashboardPage));
                }
                else
                {
                    // ERROR
                    lblError.Text = "Usuario o contraseña incorrectos";
                    lblError.Visibility = Visibility.Visible;
                }
            }
        }

        private void txtPassword_KeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                // Llamamos al evento Click del botón de login directamente
                btnLogin_Click(sender, e);
            }
        }

        private async void btnHello_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var disponible = await UserConsentVerifier.CheckAvailabilityAsync();

                if (disponible == UserConsentVerifierAvailability.Available)
                {
                    var resultado = await UserConsentVerifier.RequestVerificationAsync("Confirma tu identidad para continuar");
                    if (resultado == UserConsentVerificationResult.Verified)
                    {

                        // Navegamos al Dashboard usando el Frame de la página actual
                        this.Frame.Navigate(typeof(DashboardPage));
                    }
                    else
                    {
                        lblError.Text = "No se pudo verificar tu identidad.";
                        lblError.Visibility = Visibility.Visible;
                    }
                }
                else
                {
                    lblError.Text = "La verificación biométrica no está disponible en este dispositivo.";
                    lblError.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                lblError.Text = "Error al invocar Windows Hello: " + ex.Message;
                lblError.Visibility = Visibility.Visible;
            }
        }
    }
}
