using System.ComponentModel.DataAnnotations;
namespace Grupo18_Inmobiliaria.Models
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
        public List<Inmueble> ListaInmuebles { get; set; } = new List<Inmueble>();

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