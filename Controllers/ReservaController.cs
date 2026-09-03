using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Grupo18_Inmobiliaria.Models;

namespace Grupo18_Inmobiliaria.Controllers
{
    public class ReservaController : Controller
    {
        private readonly RepositorioReservaMySql repo_Reserva;
        private readonly RepositorioInquilinoMySql repo_Inquilino;
        private readonly RepositorioInmuebleMySql repo_Inmueble;


        public ReservaController(
            RepositorioReservaMySql repoReserva,
            RepositorioInquilinoMySql repoInquilino,
            RepositorioInmuebleMySql repoInmueble)
        {
            this.repo_Reserva = repoReserva;
            this.repo_Inquilino = repoInquilino;
            this.repo_Inmueble = repoInmueble;
        }


      
        // INDEX - RESERVAS ACTIVAS
    

        public IActionResult Index(int pagina = 1, int tamPagina = 10)
        {
            try
            {
                var lista =repo_Reserva.ObtenerActivos( pagina,tamPagina);

                int totalRegistros =repo_Reserva.ObtenerCantidad(true);

                ViewBag.PaginaActual = pagina;

                ViewBag.TotalPaginas = (int)Math.Ceiling((double)totalRegistros / tamPagina);

                return View(lista);
            }
            catch (Exception ex)
            {
                return Content("ERROR: " + ex.Message);
            }
        }


       
        // CREATE - GET
       
public IActionResult Create()
{
   
CargarDesplegables();


    return View();
}

       
        // CREATE - POST
     

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Reserva reserva)
        {
            foreach (var key in ModelState.Keys
             .Where(k =>
                 k.StartsWith("Inquilino") ||
                 k.StartsWith("Inmueble") ||
                 k.StartsWith("Usuario") ||
                 k.StartsWith("PagosEfectuados"))
             .ToList())
                    {
                        ModelState.Remove(key);
                    }


          


            if (reserva.IdInquilino <= 0)
            {
                ModelState.AddModelError("IdInquilino","Debe seleccionar un inquilino.");
            }


           
            
            // Verificar que haya un inmueble seleccionado
         

            if (reserva.IdInmueble <= 0)
            {
                ModelState.AddModelError("IdInmueble","Debe seleccionar un inmueble.");
            }


          
           
            // La fecha de inicio debe ser anterior
            // a la fecha de finalización


            
            if (reserva.FechaInicio < DateTime.Now)
            {
                
                  ModelState.AddModelError( "FechaInicio","La fecha de inicio no debe ser anterior a la fecha actual.");

            }

          

            if (reserva.FechaInicio >= reserva.FechaFin)
            {
                ModelState.AddModelError("FechaFin","La fecha de finalización debe ser posterior a la fecha de inicio." );
            }


            
           
            // Verificar que el inmueble no esté reservado
            // durante ese período
          

            if (reserva.IdInmueble > 0 && reserva.FechaInicio < reserva.FechaFin)
            {
                bool existeReserva = repo_Reserva.ExisteReservaEnFechas(
                        reserva.IdInmueble,
                        reserva.FechaInicio,
                        reserva.FechaFin
                    );

                if (existeReserva)
                {
                    ModelState.AddModelError( "IdInmueble","El inmueble ya está reservado durante ese período.");
                }
            }



            // SI HAY ERRORES


            if (!ModelState.IsValid)
            {
                

                CargarDesplegables(reserva.IdInquilino, reserva.IdInmueble);

                return View(reserva);
            }



            // GUARDAR


            try
            {
                // TEMPORAL
                // Hasta que armemos Usuario.
                reserva.IdUsuario = 1;

                repo_Reserva.Alta(reserva);

                TempData["Mensaje"] ="Reserva creada con éxito.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al crear la reserva: " + ex.Message;

                CargarDesplegables(reserva.IdInquilino,reserva.IdInmueble );

                return View(reserva);
            }
        }


              
        // DELETE - GET
       

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var reserva = repo_Reserva.ObtenerPorId(id);

            if (reserva == null)
            {
                return NotFound();
            }

            return View(reserva);
        }


        // DELETE - POST
       

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                repo_Reserva.Baja(id);

                TempData["Mensaje"] = "Reserva dada de baja con éxito.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al dar de baja la reserva: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }


       
        // INACTIVOS
     

        public IActionResult Inactivos(
            int pagina = 1,
            int tamPagina = 10)
        {
            try
            {
                var inactivos = repo_Reserva.ObtenerInactivos( pagina, tamPagina);

                int totalRegistros = repo_Reserva.ObtenerCantidad(false);

                ViewBag.PaginaActual = pagina;

                ViewBag.TotalPaginas = (int)Math.Ceiling( (double)totalRegistros / tamPagina);

                return View(inactivos);
            }
            catch (Exception ex)
            {
                return Content("ERROR: " + ex.Message);
            }
        }


       
        // REACTIVAR
       

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Reactivar(int id)
        {
            try
            {
                repo_Reserva.Reactivar(id);

                TempData["Mensaje"] = "Reserva reactivada con éxito.";
            }
            catch (Exception ex)
            {
                TempData["Error"] ="Error al reactivar la reserva: " + ex.Message;
            }

            return RedirectToAction(nameof(Inactivos));
        }



// cargar monto a la vista x dia
[HttpGet]
public IActionResult ObtenerPrecioInmueble(int id)
{
    var inmueble = repo_Inmueble.ObtenerPorId(id);
    if (inmueble == null) return NotFound();
    return Json(new { precio = inmueble.PrecioAlquiler });
}



        
        // DESPLEGABLES
    

        private void CargarDesplegables(
            int selectedInquilino = 0,
            int selectedInmueble = 0)
        {
           
            // INQUILINOS ACTIVOS
           

            var inquilinos = repo_Inquilino.ObtenerActivos()?? new List<Inquilino>();

            ViewBag.Inquilinos =new SelectList(inquilinos.Select(i => new
                    {
                        Id = i.IdInquilino,
                        NombreCompleto =
                            $"{i.Nombre} {i.Apellido}"
                    }),
                    "Id",
                    "NombreCompleto",
                    selectedInquilino
                );


           
            // INMUEBLES ACTIVOS
         

            var inmuebles = repo_Inmueble.ObtenerActivos()?? new List<Inmueble>();

            ViewBag.Inmuebles = new SelectList(inmuebles.Select(i => new
                    {
                        Id = i.IdInmueble,
                        Descripcion =
                            $"{i.Direccion}"
                    }),
                    "Id",
                    "Descripcion",
                    selectedInmueble
                );
        }
    }
}