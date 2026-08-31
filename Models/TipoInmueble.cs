using System.ComponentModel.DataAnnotations;
namespace Grupo18_Inmobiliaria.Models
{
    

    public class TipoInmueble
    {
        public int IdTipoInmueble {get; set;}

        public string Descripcion {get; set;}="";

        public bool Estado {get; set;}=true;

    }
}