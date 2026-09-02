using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
namespace Grupo18_Inmobiliaria.Models
{
    public class Inmueble
    {
        public int IdInmueble { get; set; }

        [Display(Name = "Dirección")]
        [Required(ErrorMessage = "La dirección es requerida")]
        public string Direccion { get; set; } = "";

        [Range(1, int.MaxValue, ErrorMessage = "La capacidad debe ser mayor a 0")]
        public int Capacidad { get; set; }

        [ValidateNever]
        public TipoInmueble TipoInmueble { get; set; } = new TipoInmueble();

        [Required(ErrorMessage = "Debe seleccionar un tipo de inmueble")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un tipo de inmueble válido")]
        public int IdTipoInmueble { get; set; }

        [Required(ErrorMessage = "La longitud debe estar entre -180 y 180")]
        [Range(-180, 180, ErrorMessage = "La longitud debe ser entre -180 y 180")]
        public decimal Longitud { get; set; }

        [Required(ErrorMessage = "La latitud debe estar entre -90 y 90")]
        [Range(-90, 90, ErrorMessage = "La latitud debe ser entre -90 y 90")]
        public decimal Latitud { get; set; }

        [Required(ErrorMessage = "El precio de alquiler es requerido")]
        public decimal PrecioAlquiler { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un propietario")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un propietario válido")]
        public int IdPropietario { get; set; }

        [ValidateNever]
        public Propietario Propietario { get; set; } = new Propietario();

        public bool Estado { get; set; } = true;
        public List<string> ListaReservas { get; set; } = new List<string>();

    }
}