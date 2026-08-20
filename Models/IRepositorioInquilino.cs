using Grupo18_Inmobiliaria.Models;

namespace Grupo18_Inmobiliaria.Models
{
    public interface IRepositorioInquilino : IRepositorio<Inquilino>
    {
        IList<Inquilino> BuscarPorNombre(string nombre);
    }
}