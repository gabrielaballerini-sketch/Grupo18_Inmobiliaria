using System.ComponentModel.DataAnnotations;

namespace Grupo18_Inmobiliaria.Models
{
    public enum ConceptoPago
    {
        Senia = 1,
       PagoTotal = 2,
        Multa = 3,

    }



    public enum MedioPago
    {
        Efectivo,
        Debito,
        Credito,
        Transferenia
    }





    public class Pago
    {
        public int IdPago { get; set; }

        [Required(ErrorMessage = "El importe es obligatorio.")]
        [Range(0.01, 99999999.99, ErrorMessage = "El importe debe ser mayor a 0.")]
        public decimal Importe { get; set; } 

        [Required(ErrorMessage = "La fecha de pago es obligatoria.")]
        public DateTime FechaPago { get; set; } = DateTime.Now;

        [Required]
       
        public ConceptoPago ConceptoPago { get; set; } 

        public MedioPago MedioPago { get; set; }

        [Required]
        public int IdReserva { get; set; }
        public Reserva? Reserva { get; set; }

        public bool Estado { get; set; } = true; 

        // Campos de auditoría 
        public int IdUsuarioCreador { get; set; }
        public Usuario? UsuarioCreador { get; set; }

        public int? IdUsuarioAnulador { get; set; }
        public Usuario? UsuarioAnulador { get; set; }
    }
}