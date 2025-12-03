using System;
using System.ComponentModel.DataAnnotations;

namespace PedidosManejo.Models
{
    public class ManageIndexViewModel
    {
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }

        [Display(Name = "Correo Electrónico")]
        public string CorreoElectronico { get; set; }

        [Display(Name = "Fecha de Registro")]
        public DateTime? FechaRegistro { get; set; }

        [Display(Name = "Rol")]
        public string Rol { get; set; }
    }
}