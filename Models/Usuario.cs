using System.ComponentModel.DataAnnotations;
namespace Grupo18_Inmobiliaria.Models
{
    public class Usuario
    {
        public int IdUsuario { get; set; }
        public string UserName { get; set; } = "";

        public string Password { get; set; } = "";
        public RolUsuario RolUsuario { get; set; }

        public bool Estado { get; set; } = true;
        public List<Reserva> ListaReservas { get; set; } = new List<Reserva>();

        public string? PasswordActual { get; set; } 

        public string? NuevaPassword { get; set; } 

        public string? ConfirmarPassword { get; set; } 

    }
}