using System.ComponentModel.DataAnnotations;
namespace Grupo18_Inmobiliaria.Models
{
    public class Pago
    {

        public int IdPago { get; set; }
        [Required]
        public decimal PagoParcial { get; set; }
        [Required]
        public decimal PagoTotal { get; set; }
        [Required]
        public DateTime FechaPago { get; set; }
        public TipoPago TipoPago { get; set; }
        public Reserva Reserva { get; set; } = new Reserva();
        public int IdReserva { get; set; }

        public bool Estado { get; set; } = true;
    }
}