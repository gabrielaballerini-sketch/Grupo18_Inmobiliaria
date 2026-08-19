using System.ComponentModel.DataAnnotations;
namespace Inmobiliaria.Models
{
    public class Reserva
    {
        public int Id { get; set; }
        public Inquilino Inquilino { get; set; } = new Inquilino();
        public int IdInquilino { get; set; }
        public Inmueble Inmueble { get; set; } = new Inmueble();
        public int IdInmueble { get; set; }
        public decimal MontoDiario { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public List<string> PagosEfectuados { get; set; } = new List<string>();

    }
}