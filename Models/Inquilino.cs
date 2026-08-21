using System.ComponentModel.DataAnnotations;
namespace Grupo18_Inmobiliaria.Models
{
    public class Inquilino
    {
        public int IdInquilino { get; set; } 
        [Required] 
        [RegularExpression(@"^[a-zA-ZñÑ\s]+$",
         ErrorMessage ="El nombre solo puede contener letras y espacios")]
        public string Nombre { get; set; } = "";
        [Required] 
        [RegularExpression(@"^[a-zA-ZñÑ\s]+$",
        ErrorMessage ="El apellido solo puede tener letras y espacios")]
        public string Apellido { get; set; } = "";
        [Required] 
        [Range(10000000, 99999999,
         ErrorMessage = "El DNI debe tener 8 números.")]
        public string Dni { get; set; } = "";
        [Required] 
        [RegularExpression(@"^\d{10}$",
        ErrorMessage ="El telefono es numerico y puede tener 10 digitos ")]
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