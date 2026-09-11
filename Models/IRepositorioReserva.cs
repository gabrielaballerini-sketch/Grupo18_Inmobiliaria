namespace Grupo18_Inmobiliaria.Models;

public interface IRepositorioReserva : IRepositorio<Reserva>
{
    bool ExisteReservaEnFechas(int idInmueble, DateTime fechaInicio, DateTime fechaFin, int? idReservaExcluir = null);

int FinalizarAnticipadamente(int idReserva, decimal multa, DateTime fechaCancelacion);

}