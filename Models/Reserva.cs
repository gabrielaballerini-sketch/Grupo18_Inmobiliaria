using System.ComponentModel.DataAnnotations;

namespace Grupo18_Inmobiliaria.Models
{
    public class Reserva : IValidatableObject
    {
        [Key]
        public int IdReserva { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un inquilino.")]
       
        public int IdInquilino { get; set; }

        public Inquilino Inquilino { get; set; } = new Inquilino();

        [Required(ErrorMessage = "Debe seleccionar un inmueble.")]
    
     
        public int IdInmueble { get; set; }

        public Inmueble Inmueble { get; set; } = new Inmueble();

        [Required(ErrorMessage = "El monto diario es obligatorio.")]
        [Range(0.01, 999999999.99, ErrorMessage = "El monto diario debe ser mayor a 0.")]
        [DataType(DataType.Currency)]
        [Display(Name = "Monto Diario")]
        public decimal MontoDiario { get; set; }

        [Required(ErrorMessage = "La fecha de inicio es obligatoria.")]
        [DataType(DataType.DateTime)]
        [Display(Name = "Fecha de Inicio")]
        public DateTime FechaInicio { get; set; }

        [Required(ErrorMessage = "La fecha de finalización es obligatoria.")]
        [DataType(DataType.DateTime)]
        [Display(Name = "Fecha de Finalización")]
        public DateTime FechaFin { get; set; }

        [Display(Name = "Monto de Multa")]
        [Range(0, 999999999.99, ErrorMessage = "La multa no puede ser un valor negativo.")]
        public decimal? Multa { get; set; }

        [Display(Name = "Fecha de Cancelación")]
        [DataType(DataType.DateTime)]
        public DateTime? FechaCancelacion { get; set; }

        public bool Estado { get; set; } = true;

        [Required(ErrorMessage = "El usuario que registra la reserva es obligatorio.")]
        public int IdUsuario { get; set; }

        public Usuario Usuario { get; set; } = new Usuario();

        public List<Pago> PagosEfectuados { get; set; } = new List<Pago>();

        // 💡 Validaciones avanzadas para comparar fechas entre sí
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (FechaFin <= FechaInicio)
            {
                yield return new ValidationResult(
                    "La fecha de finalización debe ser posterior a la fecha de inicio.",
                    new[] { nameof(FechaFin) }
                );
            }
        }
    }
}