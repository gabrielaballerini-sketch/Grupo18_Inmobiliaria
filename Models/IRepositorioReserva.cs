namespace Grupo18_Inmobiliaria.Models;

public interface IRepositorioReserva : IRepositorio<Reserva>
{
    bool ExisteReservaEnFechas(int idInmueble, DateTime fechaInicio, DateTime fechaFin, int? idReservaExcluir = null);

    int FinalizarAnticipadamente(int idReserva, decimal multa, DateTime fechaCancelacion, int IdUsuarioCancelacion);
    IList<Reserva> ObtenerFinalizadas(int pagina = 1, int tamPagina = 10);
     IList<Inmueble> ObtenerInmueblesMasReservados365Dias(int pagina = 1, int tamPagina = 10);

     IEnumerable<Reserva> ObtenerProximasATerminar(int dias);

}