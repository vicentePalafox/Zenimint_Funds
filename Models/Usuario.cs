namespace Zenimint_Funds.Models
{
    internal class Usuario
    {
        // EF Core automáticamente asume que una propiedad llamada "Id" es la Primary Key autoincrementable.
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
