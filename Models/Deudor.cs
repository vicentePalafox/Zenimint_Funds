using System.Collections.Generic;

namespace Zenimint_Funds.Models
{
    class Deudor
    {
        public int Id { get; set; }
        public string Nombre { get; set; }

        public List<Gasto> GastosAsignados { get; set; }
    }
}
