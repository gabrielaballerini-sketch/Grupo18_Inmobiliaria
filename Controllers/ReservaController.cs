using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Grupo18_Inmobiliaria.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Grupo18_Inmobiliaria.Controllers
{
    [Authorize]
    public class ReservaController : Controller
    {
        private readonly IRepositorioReserva repo_Reserva;
        private readonly IRepositorioInquilino repo_Inquilino;
        private readonly IRepositorioInmueble repo_Inmueble;
        private readonly IRepositorioPago repo_Pago;


        public ReservaController(
            IRepositorioReserva repoReserva,
            IRepositorioInquilino repoInquilino,
            IRepositorioInmueble repoInmueble,
            IRepositorioPago repoPago)
        {
            this.repo_Reserva = repoReserva;
            this.repo_Inquilino = repoInquilino;
            this.repo_Inmueble = repoInmueble;
            this.repo_Pago = repoPago;
        }



        // INDEX - RESERVAS ACTIVAS


        public IActionResult Index(int pagina = 1, int tamPagina = 10)
        {
            if (pagina < 1)
                pagina = 1;

            if (tamPagina <= 0)
                tamPagina = 10;
            try
            {
                var lista = repo_Reserva.ObtenerActivos(pagina, tamPagina);

                int totalRegistros = repo_Reserva.ObtenerCantidad(true);

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
[HttpGet]
public IActionResult Create(int? idInmueble, DateTime? desde, DateTime? hasta)
{
    var reserva = new Reserva();

    if (desde.HasValue) reserva.FechaInicio = desde.Value;
    if (hasta.HasValue) reserva.FechaFin = hasta.Value;

    //  viene un inmueble desde la vista de búsqueda
    if (idInmueble.HasValue && idInmueble.Value > 0)
    {
        var inmueble = repo_Inmueble.ObtenerPorId(idInmueble.Value);
        if (inmueble != null)
        {
            reserva.IdInmueble = inmueble.IdInmueble;
            reserva.MontoDiario = inmueble.PrecioAlquiler;
            
            
            // Pasamos el porcentaje para que el JS de la vista calcule la seña automáticamente
            ViewBag.PorcentajeReserva = inmueble.PorcentajeReserva;
            
          
            reserva.Inmueble = inmueble; 
        }
    }

    // Cargas tus desplegables normalmente pasando el id preseleccionado
    CargarDesplegables(0, reserva.IdInmueble);

    return View(reserva);
}





        // CREATE - POST

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Reserva reserva, int medioPagoSenia = 1)
        {
            // 💡 Forzar los horarios fijos de Check-in (15:00) y Check-out (10:00)
            reserva.FechaInicio = reserva.FechaInicio.Date.AddHours(15);
            reserva.FechaFin = reserva.FechaFin.Date.AddHours(10);

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
                ModelState.AddModelError("IdInquilino", "Debe seleccionar un inquilino.");
            }

            // Verificar que haya un inmueble seleccionado

            if (reserva.IdInmueble <= 0)
            {
                ModelState.AddModelError("IdInmueble", "Debe seleccionar un inmueble.");
            }

            // La fecha de inicio debe ser anterior
            // a la fecha de finalización

            if (reserva.FechaInicio < DateTime.Now)
            {

                ModelState.AddModelError("FechaInicio", "La fecha de inicio no debe ser anterior a la fecha actual.");

            }

            if (reserva.FechaInicio >= reserva.FechaFin)
            {
                ModelState.AddModelError("FechaFin", "La fecha de finalización debe ser posterior a la fecha de inicio.");
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
                    ModelState.AddModelError("IdInmueble", "El inmueble ya está reservado durante ese período.");
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
                //Valida de forma segura el claim del usuario para evitar el NullReferenceException
                var claimUsuario = User.FindFirst(ClaimTypes.NameIdentifier);
                if (claimUsuario == null)
                {
                    TempData["Error"] = "No se pudo identificar al usuario logueado. Inicie sesión nuevamente.";
                    CargarDesplegables(reserva.IdInquilino, reserva.IdInmueble);
                    return View(reserva);
                }

                reserva.IdUsuario = int.Parse(claimUsuario.Value);

                // Obtener el inmueble directamente desde la BD
                var inmueble = repo_Inmueble.ObtenerPorId(reserva.IdInmueble);

                if (inmueble == null)
                {
                    ModelState.AddModelError(
                        "IdInmueble",
                        "El inmueble seleccionado no existe."
                    );

                    CargarDesplegables(
                        reserva.IdInquilino,
                        reserva.IdInmueble
                    );

                    return View(reserva);
                }

                // El monto diario siempre debe salir del inmueble,
                // no confiar en el valor enviado por el navegador.
                reserva.MontoDiario = inmueble.PrecioAlquiler;

               
               
                // Crear reserva
                repo_Reserva.Alta(reserva);


                // Calcular importe de la seña
                int cantidadDias =
                    (reserva.FechaFin.Date - reserva.FechaInicio.Date).Days;

                decimal montoTotal =
                    cantidadDias * reserva.MontoDiario;

                decimal importeSenia =
                    montoTotal * inmueble.PorcentajeReserva / 100m;


                // Registrar la seña
                if (importeSenia > 0)
                {
                    var pagoSenia = new Pago
                    {
                        IdReserva = reserva.IdReserva,
                        FechaPago = DateTime.Now,
                        Importe = importeSenia,
                        ConceptoPago = ConceptoPago.Senia,
                        MedioPago = (MedioPago)medioPagoSenia,
                        Estado = true,
                        IdUsuarioCreador = reserva.IdUsuario
                    };

                    repo_Pago.Alta(pagoSenia);
                }


                TempData["Mensaje"] =
                    "Reserva creada y seña registrada correctamente.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al crear la reserva: " + ex.Message;
                CargarDesplegables(reserva.IdInquilino, reserva.IdInmueble);
                return View(reserva);
            }
        }


        // DELETE - GET

        [Authorize(Roles = "Administrativo")]
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

        [Authorize(Roles = "Administrativo")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var reserva = repo_Reserva.ObtenerPorId(id);

            if (reserva == null)
            {
                return NotFound();
            }

            try
            {
                repo_Reserva.Baja(id);

                TempData["Mensaje"] = "Reserva dada de baja con éxito.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al dar de baja la reserva: " + ex.Message;

                return RedirectToAction(nameof(Index));
            }
        }

        // EDIT - GET

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var reserva = repo_Reserva.ObtenerPorId(id);

            if (reserva == null)
            {
                return NotFound();
            }

            return View(reserva);
        }

        // EDIT - POST

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Reserva reserva)
        {
            if (id != reserva.IdReserva)
            {
                return NotFound();
            }

            // Forzar horarios fijos
            reserva.FechaInicio = reserva.FechaInicio.Date.AddHours(15);
            reserva.FechaFin = reserva.FechaFin.Date.AddHours(10);

            // Limpieza de ModelState para propiedades de navegación (igual que en Create)
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
                ModelState.AddModelError("IdInquilino", "Debe seleccionar un inquilino.");
            }

            if (reserva.IdInmueble <= 0)
            {
                ModelState.AddModelError("IdInmueble", "Debe seleccionar un inmueble.");
            }

            if (reserva.FechaInicio >= reserva.FechaFin)
            {
                ModelState.AddModelError("FechaFin", "La fecha de finalización debe ser posterior a la fecha de inicio.");
            }

            // Verificar que el inmueble no esté reservado durante ese período 
            // OJO: Le pasamos el 'reserva.IdReserva' al final para que excluya a la propia reserva de la validación
            if (reserva.IdInmueble > 0 && reserva.FechaInicio < reserva.FechaFin)
            {
                bool existeReserva = repo_Reserva.ExisteReservaEnFechas(
                        reserva.IdInmueble,
                        reserva.FechaInicio,
                        reserva.FechaFin,
                        reserva.IdReserva
                    );

                if (existeReserva)
                {
                    ModelState.AddModelError("IdInmueble", "El inmueble ya está reservado durante ese período.");
                }
            }

            // SI HAY ERRORES

            if (!ModelState.IsValid)
            {
                return View(reserva);
            }

            // GUARDAR CAMBIOS

            try
            {
                // Asignamos o mantenemos el usuario (podes ajustarlo si usas autenticación)
                int idUsuario = int.Parse(
                 User.FindFirst("IdUsuario")!.Value
                 );

                reserva.IdUsuario = idUsuario;

                repo_Reserva.Modificacion(reserva);

                TempData["Mensaje"] = "Reserva modificada con éxito.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al modificar la reserva: " + ex.Message;

                return View(reserva);
            }
        }

        // INACTIVOS

        [Authorize(Roles = "Administrativo")]
        public IActionResult Inactivos(int pagina = 1, int tamPagina = 10)
        {
            if (pagina < 1)
                pagina = 1;

            if (tamPagina <= 0)
                tamPagina = 10;
            try
            {
                var inactivos = repo_Reserva.ObtenerInactivos(pagina, tamPagina);

                int totalRegistros = repo_Reserva.ObtenerCantidad(false);

                ViewBag.PaginaActual = pagina;

                ViewBag.TotalPaginas = (int)Math.Ceiling((double)totalRegistros / tamPagina);

                return View(inactivos);
            }
            catch (Exception ex)
            {
                return Content("ERROR: " + ex.Message);
            }
        }

        // REACTIVAR

        [Authorize(Roles = "Administrativo")]
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
                TempData["Error"] = "Error al reactivar la reserva: " + ex.Message;
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

        public IActionResult Finalizadas(int pagina = 1, int tamPagina = 10)
        {
            if (pagina < 1)
                pagina = 1;

            if (tamPagina <= 0)
                tamPagina = 10;
            try
            {
                var lista = repo_Reserva.ObtenerFinalizadas(pagina, tamPagina);

                int totalRegistros = repo_Reserva.ObtenerCantidad(false);

                ViewBag.PaginaActual = pagina;
                ViewBag.TotalPaginas = (int)Math.Ceiling((double)totalRegistros / tamPagina);

                return View(lista);
            }
            catch (Exception ex)
            {
                return Content("ERROR: " + ex.Message);
            }
        }

        // DESPLEGABLES
        private void CargarDesplegables(
            int selectedInquilino = 0,
            int selectedInmueble = 0)
        {

            // INQUILINOS ACTIVOS
            var inquilinos = repo_Inquilino.ObtenerActivos() ?? new List<Inquilino>();

            ViewBag.Inquilinos = new SelectList(inquilinos.Select(i => new
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
            var inmuebles = repo_Inmueble.ObtenerActivos() ?? new List<Inmueble>();

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

        // GET: Reserva/BuscarInquilinosPorNombre?term=na
        [HttpGet]
        public IActionResult BuscarInquilinosPorNombre(string term)
        {
            if (string.IsNullOrEmpty(term))
            {
                return Json(new List<object>());
            }

            var inquilinos = repo_Inquilino.BuscarPorNombre(term);

            var resultado = inquilinos
                .Select(i => new
                {
                    id = i.IdInquilino,
                    nombreCompleto = $"{i.Nombre} {i.Apellido} - DNI: {i.Dni}"
                })
                .ToList();

            return Json(resultado);
        }

        // GET: Reserva/BuscarInmueblesPorDireccion?term=rivadavia
        [HttpGet]
        [HttpGet]
        public IActionResult BuscarInmueblesPorDireccion(string term)
        {
            if (string.IsNullOrEmpty(term))
            {
                return Json(new List<object>());
            }

            var inmuebles = repo_Inmueble.BuscarPorDireccion(term);

            var resultado = inmuebles
                .Select(i => new
                {
                    id = i.IdInmueble,
                    direccion = i.Direccion,
                    precio = i.PrecioAlquiler,
                    porcentajeReserva = i.PorcentajeReserva
                })
                .ToList();

            return Json(resultado);
        }


        // PARA VUE
        // GET: Reserva/ObtenerJson/5
        [HttpGet]
        public IActionResult ObtenerJson(int id)
        {
            var reserva = repo_Reserva.ObtenerPorId(id);
            if (reserva == null) return NotFound();

            return Json(new
            {
                idReserva = reserva.IdReserva,
                fechaInicio = reserva.FechaInicio.ToString("yyyy-MM-dd"),
                fechaFin = reserva.FechaFin.ToString("yyyy-MM-dd"),
                montoDiario = reserva.MontoDiario,
                estado = reserva.Estado,
                inquilino = reserva.Inquilino != null ? new
                {
                    idInquilino = reserva.Inquilino.IdInquilino,
                    nombre = reserva.Inquilino.Nombre,
                    apellido = reserva.Inquilino.Apellido,
                    dni = reserva.Inquilino.Dni,
                    telefono = reserva.Inquilino.Telefono,
                    email = reserva.Inquilino.Email
                } : null,
                inmueble = reserva.Inmueble != null ? new
                {
                    idInmueble = reserva.Inmueble.IdInmueble,
                    direccion = reserva.Inmueble.Direccion,
                    precioAlquiler = reserva.Inmueble.PrecioAlquiler,
                    capacidad = reserva.Inmueble.Capacidad
                } : null
            });
        }





        // GET: Reserva/Details/5
        [HttpGet]
        public IActionResult Details(int id)
        {
            var reserva = repo_Reserva.ObtenerPorId(id);
            if (reserva == null) return NotFound();
            return View(reserva);
        }

        // GET: Reserva/DetalleBaja/5
        public IActionResult DetailsBajas(int id)
        {
            var reserva = repo_Reserva.ObtenerPorId(id); // Debe traer el Inquilino, Inmueble y la lista de Pagos

            if (reserva == null)
            {
                TempData["Error"] = "No se encontró la reserva solicitada.";
                return RedirectToAction("Inactivos");
            }

            return View(reserva);
        }


        // GET: Reservas/Extender/5
        [HttpGet]
        public IActionResult Extender(int id)
        {
            var reserva = repo_Reserva.ObtenerPorId(id);

            if (reserva == null)
            {
                return NotFound();
            }

            if (!reserva.Estado)
            {
                TempData["Error"] = "No se puede extender una reserva que está dada de baja.";
                return RedirectToAction(nameof(Index));
            }

            // Opcional: Validar que no se extienda una reserva que ya finalizó en el pasado
            if (reserva.FechaFin < DateTime.Now)
            {
                TempData["Error"] = "No se puede extender una reserva cuya fecha de fin ya transcurrió.";
                return RedirectToAction(nameof(Index));
            }

            return View(reserva);
        }





        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Extender(
            int id,
            DateTime fechaFin,
            decimal montoDiario)
        {
            // Buscamos la reserva ORIGINAL.
            var reservaOriginal = repo_Reserva.ObtenerPorId(id);

            if (reservaOriginal == null)
            {
                return NotFound();
            }

            if (!reservaOriginal.Estado)
            {
                TempData["Error"] =
                    "No se puede extender una reserva que está dada de baja.";

                return RedirectToAction(nameof(Index));
            }

            // VALIDAR MONTO

            if (montoDiario <= 0)
            {
                ModelState.AddModelError(
                    "montoDiario",
                    "El monto diario debe ser mayor a 0.");
            }

            // NUEVA FECHA DE INICIO
            // La nueva reserva comienza cuando
            // termina la reserva original.

            DateTime nuevaFechaInicio =
                reservaOriginal.FechaFin.Date.AddHours(15);

            // NUEVA FECHA DE FIN

            DateTime nuevaFechaFin =
                fechaFin.Date.AddHours(10);

            if (nuevaFechaFin <= nuevaFechaInicio)
            {
                ModelState.AddModelError(
                    "fechaFin",
                    "La nueva fecha de finalización debe ser posterior a la fecha de inicio.");
            }

            // VERIFICAR DISPONIBILIDAD

            if (nuevaFechaFin > nuevaFechaInicio)
            {
                bool existeReserva =
                    repo_Reserva.ExisteReservaEnFechas(
                        reservaOriginal.IdInmueble,
                        nuevaFechaInicio,
                        nuevaFechaFin);

                if (existeReserva)
                {
                    ModelState.AddModelError(
                        "fechaFin",
                        "El inmueble ya está reservado durante el período de extensión.");
                }
            }

            // SI HAY ERRORES
            if (!ModelState.IsValid)
            {
                reservaOriginal.MontoDiario = montoDiario;
                reservaOriginal.FechaInicio = nuevaFechaInicio;
                reservaOriginal.FechaFin = nuevaFechaFin;

                return View(reservaOriginal);
            }

            // USUARIO LOGUEADO
            var claimUsuario =
                User.FindFirst(ClaimTypes.NameIdentifier);

            if (claimUsuario == null)
            {
                TempData["Error"] =
                    "No se pudo identificar al usuario logueado.";

                return RedirectToAction(nameof(Index));
            }

            int idUsuario = int.Parse(claimUsuario.Value);

            // CREAR NUEVA RESERVA
            var nuevaReserva = new Reserva
            {
                IdInquilino = reservaOriginal.IdInquilino,

                IdInmueble = reservaOriginal.IdInmueble,

                MontoDiario = montoDiario,

                FechaInicio = nuevaFechaInicio,

                FechaFin = nuevaFechaFin,

                Estado = true,

                IdUsuario = idUsuario
            };

            // GUARDAR
            try
            {
                repo_Reserva.Alta(nuevaReserva);

                TempData["Mensaje"] =
                    $"Reserva extendida correctamente. " +
                    $"Se creó la nueva reserva N° {nuevaReserva.IdReserva}.";

                return RedirectToAction(nameof(Details),
                    new { id = nuevaReserva.IdReserva });
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    "Error al extender la reserva: " + ex.Message;

                return View(reservaOriginal);
            }
        }



        // FINALIZACION ANTICIPADA
        [HttpGet]
        public IActionResult Finalizar(int id)
        {
            var reserva = repo_Reserva.ObtenerPorId(id);

            if (reserva == null)
            {
                return NotFound();
            }

            if (!reserva.Estado)
            {
                TempData["Error"] =
                    "No se puede finalizar una reserva que ya está dada de baja.";

                return RedirectToAction(nameof(Index));
            }
            if (DateTime.Now < reserva.FechaInicio)
            {
                TempData["Error"] =
                    "No se puede realizar una finalización anticipada antes de que comience la reserva.";

                return RedirectToAction(nameof(Details), new { id });
            }

            return View(reserva);
        }


        // FINALIZACION ANTICIPADA
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Finalizar(int id, DateTime fechaFinalizacion, bool pagaMultaNow, int medioPago)
        {

            // 1. BUSCAR LA RESERVA
            var reserva = repo_Reserva.ObtenerPorId(id);

            if (reserva == null)
            {
                return NotFound();
            }

            // 2. VERIFICAR QUE ESTÉ ACTIVA
            if (!reserva.Estado)
            {
                TempData["Error"] =
                    "La reserva ya se encuentra dada de baja.";

                return RedirectToAction(nameof(Index));
            }

            // 3. NORMALIZAR FECHA DE FINALIZACIÓN
            DateTime fechaCancelacionLimpia =
                fechaFinalizacion.Date.AddHours(10);

            // 4. VALIDAR FECHA
            if (fechaCancelacionLimpia <= reserva.FechaInicio)
            {
                ModelState.AddModelError(
                    "fechaFinalizacion",
                    "La fecha de finalización debe ser posterior a la fecha de inicio.");
            }

            if (fechaCancelacionLimpia >= reserva.FechaFin)
            {
                ModelState.AddModelError(
                    "fechaFinalizacion",
                    "La fecha indicada no corresponde a una finalización anticipada. " +
                    "Debe ser anterior a la fecha original de finalización.");
            }

            // 5. CALCULAR MULTA
            decimal multaCalculada = 0;

            if (ModelState.IsValid)
            {
                int duracionTotalDias =
                    (reserva.FechaFin.Date - reserva.FechaInicio.Date).Days;

                int tiempoTranscurridoDias =
                    (fechaCancelacionLimpia.Date - reserva.FechaInicio.Date).Days;

                if (tiempoTranscurridoDias < 0)
                {
                    tiempoTranscurridoDias = 0;
                }

                int diasRestantes =
                    (reserva.FechaFin.Date - fechaCancelacionLimpia.Date).Days;

                if (diasRestantes < 0)
                {
                    diasRestantes = 0;
                }

                decimal alquilerRestante =
                    diasRestantes * reserva.MontoDiario;


                // Menos de la mitad del tiempo cumplido
                // → 50% del alquiler restante

                if (tiempoTranscurridoDias <
                    ((double)duracionTotalDias / 2))
                {
                    multaCalculada =
                        alquilerRestante * 0.50m;
                }
                else
                {
                    // Mitad o más del tiempo cumplido
                    // → 25% del alquiler restante

                    multaCalculada =
                        alquilerRestante * 0.25m;
                }
            }

            // 6. SI HAY ERRORES, VOLVER A LA VISTA
            if (!ModelState.IsValid)
            {
                reserva.Multa = multaCalculada;
                reserva.FechaCancelacion = fechaCancelacionLimpia;

                return View(reserva);
            }

            // 7. OBTENER USUARIO LOGUEADO
            var claimUsuario =
                User.FindFirst(ClaimTypes.NameIdentifier);

            if (claimUsuario == null)
            {
                TempData["Error"] =
                    "No se pudo identificar al usuario logueado. " +
                    "Inicie sesión nuevamente.";

                return RedirectToAction(nameof(Index));
            }

            int idUsuarioCancelacion;

            if (!int.TryParse(
                claimUsuario.Value,
                out idUsuarioCancelacion))
            {
                TempData["Error"] =
                    "El usuario logueado no tiene un identificador válido.";

                return RedirectToAction(nameof(Index));
            }

            // 8. SI HAY MULTA, DEBE PAGARSE
            if (multaCalculada > 0 && !pagaMultaNow)
            {
                TempData["Error"] =
                    $"No se puede finalizar la reserva. " +
                    $"Debe abonar la multa de ${multaCalculada:N2}.";

                reserva.Multa = multaCalculada;
                reserva.FechaCancelacion = fechaCancelacionLimpia;

                return View(reserva);
            }

            // 9. GUARDAR FINALIZACIÓN
            try
            {
                repo_Reserva.FinalizarAnticipadamente(
                    id,
                    multaCalculada,
                    fechaCancelacionLimpia,
                    idUsuarioCancelacion);

                // 10. REGISTRAR PAGO DE MULTA
                if (multaCalculada > 0 && pagaMultaNow)
                {
                    var pagoMulta = new Pago
                    {
                        IdReserva = reserva.IdReserva,
                        FechaPago = DateTime.Now,
                        Importe = multaCalculada,
                        ConceptoPago = ConceptoPago.Multa,
                        MedioPago = (MedioPago)medioPago,
                        Estado = true,
                        IdUsuarioCreador = idUsuarioCancelacion
                    };

                    repo_Pago.Alta(pagoMulta);
                }

                // 11. MENSAJE
                if (multaCalculada > 0)
                {
                    TempData["Mensaje"] =
                        $"Reserva finalizada correctamente. " +
                        $"Se registró el pago de la multa de ${multaCalculada:N2}.";
                }
                else
                {
                    TempData["Mensaje"] =
                        "Reserva finalizada correctamente.";
                }

                return RedirectToAction(
                    nameof(Details),
                    new { id = reserva.IdReserva });
            }
            catch (Exception ex)
            {

                TempData["Error"] =
                    "Error al finalizar la reserva: " + ex.Message;

                reserva.Multa = multaCalculada;
                reserva.FechaCancelacion = fechaCancelacionLimpia;

                return View(reserva);
            }
        }
        [HttpGet]
        public IActionResult MasReservados()
        {
            var inmuebles = repo_Reserva.ObtenerInmueblesMasReservados365Dias();

            return View(inmuebles);
        }

[HttpGet]
public IActionResult ProximasATerminar(int? dias)
{
    IEnumerable<Reserva> reservas = new List<Reserva>();

    // Solo consultamos si el usuario ingresó un valor válido (ej. 20)
    if (dias.HasValue && dias.Value > 0)
    {
        reservas = repo_Reserva.ObtenerProximasATerminar(dias.Value);
        ViewBag.DiasSeleccionados = dias.Value;
    }

    return View(reservas);
}







    }
}