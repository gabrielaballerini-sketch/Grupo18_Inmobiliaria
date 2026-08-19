using System.ComponentModel.DataAnnotations;
namespace Inmobiliaria.Models
{
    public class Pago
    {

        public int Id { get; set; }
        [Required]
        public decimal PagoPArcial { get; set; }
        [Required]
        public decimal PagoTotal { get; set; }
        [Required]
        public DateTime FechaPago { get; set; }
        public TipoPago TipoPago { get; set; }
        public Reserva Reserva { get; set; } = new Reserva();
        public int IdReserva { get; set; }
    }
}