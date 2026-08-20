using System.ComponentModel.DataAnnotations;
namespace Grupo18_Inmobiliaria.Models
{
    public class Inmueble
    {
        public int Id { get; set; }
        [Display(Name = "Dirección")]
        [Required(ErrorMessage = "La dirección es requerida")]
        public string Direccion { get; set; } = "";
        [Required]
        public int Capacidad { get; set; }
        [Required]
        public TipoInmueble TipoInmueble { get; set; }
        public float Coordenadas { get; set; }
        public decimal PrecioAlquiler { get; set; }
        public int IdPropietario { get; set; }
        public Propietario Propietario { get; set; } = new Propietario();
        public bool Estado { get; set; }
        public List<string> ListaReservas { get; set; } = new List<string>();

    }
}