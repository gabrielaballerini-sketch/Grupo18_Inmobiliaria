using Grupo18_Inmobiliaria.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Grupo18_Inmobiliaria.Controllers
{

    public class PagoController : Controller
    {
        private readonly IRepositorioPago repoPago;
        private readonly IRepositorioReserva repoReserva;

        public PagoController(IRepositorioPago repoPago, IRepositorioReserva repoReserva)
        {
            this.repoPago = repoPago;
            this.repoReserva = repoReserva;
        }

        // GET: Pago/PorReserva/5 (Listado de pagos de una reserva específica)
        public IActionResult PorReserva(int id)
        {
            var reserva = repoReserva.ObtenerPorId(id);
            if (reserva == null)
            {
                TempData["Error"] = "La reserva especificada no existe.";
                return RedirectToAction("Index", "Reserva");
            }

            ViewBag.Reserva = reserva;
            var listaPagos = repoPago.ObtenerPorReserva(id);
            return View(listaPagos);
        }

        // GET: Pago/Crear/5 (Formulario para registrar un pago asociado a la reserva)
        public IActionResult Crear(int id)
        {
            var reserva = repoReserva.ObtenerPorId(id);
            if (reserva == null)
            {
                TempData["Error"] = "La reserva especificada no existe.";
                return RedirectToAction("Index", "Reserva");
            }

            ViewBag.Reserva = reserva;
            var pago = new Pago
            {
                IdReserva = id,
                FechaPago = DateTime.Now
            };

            return View(pago);
        }

        // POST: Pago/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Crear(Pago pago)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Obtenemos el Id del usuario autenticado en la sesión
                    int idUsuarioActual = int.Parse(User.FindFirst("IdUsuario")?.Value ?? "1");
                    pago.IdUsuarioCreador = idUsuarioActual;

                    repoPago.Alta(pago);
                    TempData["Mensaje"] = "El pago fue registrado con éxito.";
                    return RedirectToAction(nameof(PorReserva), new { id = pago.IdReserva });
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Ocurrió un error al registrar el pago: " + ex.Message;
                }
            }

            ViewBag.Reserva = repoReserva.ObtenerPorId(pago.IdReserva);
            return View(pago);
        }

        // POST: Pago/Anular/5
        [HttpPost]
        [ValidateAntiForgeryToken]
      
        public IActionResult Anular(int idPago, int idReserva)
        {
            try
            {
                int idUsuarioActual = int.Parse(User.FindFirst("IdUsuario")?.Value ?? "1");
                int lineasAfectadas = repoPago.Anular(idPago, idUsuarioActual);

                if (lineasAfectadas > 0)
                {
                    TempData["Mensaje"] = "El pago fue anulado correctamente.";
                }
                else
                {
                    TempData["Error"] = "No se pudo anular el pago.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Ocurrió un error al anular el pago: " + ex.Message;
            }

            return RedirectToAction(nameof(PorReserva), new { id = idReserva });
        }


[HttpPost]
public IActionResult EditarConceptoAjax(int idPago, int conceptoPago)
{
    try
    {
        // 1. Casteamos el valor numérico al Enum
        var nuevoConceptoEnum = (ConceptoPago)conceptoPago;

        // 2. Llamamos al método de tu repositorio
        int filasAfectadas = repoPago.ModificarConcepto(idPago, nuevoConceptoEnum);

        if (filasAfectadas > 0)
        {
            return Json(new { 
                success = true, 
                nuevoConceptoTexto = nuevoConceptoEnum.ToString() 
            });
        }

        return Json(new { success = false, message = "No se encontró el pago especificado." });
    }
    catch (Exception ex)
    {
        return Json(new { success = false, message = ex.Message });
    }
}











    }
}