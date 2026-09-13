using System.ComponentModel.DataAnnotations;
namespace Grupo18_Inmobiliaria.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage ="el usuario es obligatorio")]
        public string UserName{get;set;}="";
        [Required(ErrorMessage ="La contraseña es obligatoria")]
        [DataType(DataType.Password)]
        public string Password{get; set;}="";
    }
}