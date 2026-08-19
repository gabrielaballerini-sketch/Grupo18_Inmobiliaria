using System.ComponentModel.DataAnnotations;
namespace Inmobiliaria.Models
{

    public class Propietario
    {
        [Key]
        [Display(Name = "Código Int.")]
        public int IdPropietario { get; set; }
        [Required]
        public string Nombre { get; set; } = "";
        [Required]
        public string Apellido { get; set; } = "";
        [Required]
        public string Dni { get; set; } = "";
        [Display(Name = "Teléfono")]
        public string Telefono { get; set; } = "";
        [Required, EmailAddress]
        public string Email { get; set; } = "";
        public List<string> ListaInmuebles { get; set; } = new List<string>();

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