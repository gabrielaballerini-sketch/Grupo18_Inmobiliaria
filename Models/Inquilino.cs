using System.ComponentModel.DataAnnotations;
namespace Grupo18_Inmobiliaria.Models
{
    public class Inquilino
    {
        public int IdInquilino { get; set; } 
        public string Nombre { get; set; } = "";
        [Required]
        public string Apellido { get; set; } = "";
        [Required]
        public string Dni { get; set; } = "";
        [Display(Name = "Teléfono")]
        public string Telefono { get; set; } = "";
        [Required, EmailAddress]
        public string Email { get; set; } = "";

        public bool Estado { get; set; } = true;

        public List<Reserva> ListaReservas { get; set; } = new List<Reserva>();
        public override string ToString()
        {
            var res = $"{Nombre} {Apellido}";
            if (!String.IsNullOrEmpty(Dni))
            {
                res += $" ({Dni})";
            }
            return res;
        }


    }
}