
namespace Grupo18_Inmobiliaria.Models
{
	public interface IRepositorioInmueble : IRepositorio<Inmueble>
	{
		int ModificarPortada(int IdInmueble, string ruta);
		IList<Inmueble> BuscarPorPropietario(int idPropietario);

        public IList<Inmueble> ObtenerDisponiblesEntreFechas(DateTime fechaInicio, DateTime fechaFin);

	}
}
