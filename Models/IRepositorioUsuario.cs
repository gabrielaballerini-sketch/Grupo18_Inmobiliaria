namespace Grupo18_Inmobiliaria.Models;

public interface IRepositorioUsuario : IRepositorio<Usuario>
{
    Usuario? ObtenerPorUserName(string userName);
}