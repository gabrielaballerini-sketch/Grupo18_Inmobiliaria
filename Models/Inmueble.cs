using System.ComponentModel.DataAnnotations;
namespace Grupo18_Inmobiliaria.Models
{
    public class Inmueble
    {
        public int IdInmueble { get; set; }
        [Display(Name = "Dirección")]
        [Required(ErrorMessage = "La dirección es requerida")]
        public string Direccion { get; set; } = "";
        [Required]
        public int Capacidad { get; set; }
        [Required]
        public TipoInmueble TipoInmueble { get; set; }=new TipoInmueble();
        public int IdTipoInmueble {get; set;}
        [Required]
        [Range ( -180, 180,ErrorMessage ="La longitud debe ser entre -180 y 180")]
        public decimal Longitud { get; set; }
        [Required]
        [Range ( -90, 90,ErrorMessage ="La latitud debe ser entre -90 y 90")]

         public decimal Latitud { get; set; }
        public decimal PrecioAlquiler { get; set; }
        public int IdPropietario { get; set; }
        public Propietario Propietario { get; set; } = new Propietario();
        public bool Estado { get; set; }=true;
        public List<string> ListaReservas { get; set; } = new List<string>();

    }
}