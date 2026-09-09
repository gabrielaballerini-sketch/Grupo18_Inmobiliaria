using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
namespace Grupo18_Inmobiliaria.Models
{
    public class Imagen
    {
        [Required]
        public int IdImagen {get; set;}
        [Required]
        public string Url {get;set;}="";
        [Required]
        public int IdInmueble {get;set;}
        [NotMapped]
        public IFormFile? Archivo {get;set;}
    }
}