namespace Grupo18_Inmobiliaria.Models
{
    public interface IRepositorioPago : IRepositorio<Pago>
    {
        // Obtiene el historial completo de pagos (activos y anulados) de una reserva concreta
        IList<Pago> ObtenerPorReserva(int idReserva);

        // Cumple la regla de negocio: solo se edita el concepto del pago
        int ModificarConcepto(int idPago, ConceptoPago nuevoConcepto);

        // Cumple la regla de negocio: anulación con auditoría de usuario
        int Anular(int idPago, int idUsuarioAnulador);
    }
}