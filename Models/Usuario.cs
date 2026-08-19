using System.ComponentModel.DataAnnotations;
namespace Inmobiliaria.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string UserName { get; set; } = "";

        public string Password { get; set; } = "";
        public RoLUsuario RoLusuario { get; set; }
        public List<Reserva> ListaReservas { get; set; } = new List<Reserva>();

    }
}