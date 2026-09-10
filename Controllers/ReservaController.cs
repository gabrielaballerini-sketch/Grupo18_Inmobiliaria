using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Grupo18_Inmobiliaria.Models;

namespace Grupo18_Inmobiliaria.Controllers
{
    public class ReservaController : Controller
    {
        private readonly IRepositorioReserva repo_Reserva;
        private readonly IRepositorioInquilino repo_Inquilino;
        private readonly IRepositorioInmueble repo_Inmueble;


        public ReservaController(
            IRepositorioReserva repoReserva,
            IRepositorioInquilino repoInquilino,
            IRepositorioInmueble repoInmueble)
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
                // TEMPORAL
                // Hasta que armemos Usuario.
                reserva.IdUsuario = 2;

                repo_Reserva.Alta(reserva);

                TempData["Mensaje"] = "Reserva creada con éxito.";

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
        public IActionResult DeleteConfirmed(int id, DateTime fechaCancelacion, bool pagaMultaNow)
        {
            var reserva = repo_Reserva.ObtenerPorId(id);
            if (reserva == null)
            {
                return NotFound();
            }

            // Validar si se esta cancelando antes de la fecha original
            if (fechaCancelacion < reserva.FechaFin)
            {
                // --- CALCULO DE LA MULTA ---
                double duracionTotalDias = (reserva.FechaFin - reserva.FechaInicio).TotalDays;
                double tiempoTranscurridoDias = (fechaCancelacion - reserva.FechaInicio).TotalDays;
                if (tiempoTranscurridoDias < 0) tiempoTranscurridoDias = 0;

                double diasRestantes = (reserva.FechaFin - fechaCancelacion).TotalDays;
                if (diasRestantes < 0) diasRestantes = 0;

                decimal alquilerRestante = (decimal)diasRestantes * reserva.MontoDiario;
                decimal multaCalculada = 0;

                if (tiempoTranscurridoDias < (duracionTotalDias / 2))
                {
                    // Menos de la mitad del tiempo cumplido -> 50% del restante
                    multaCalculada = alquilerRestante * 0.50m;
                }
                else
                {
                    // Más de la mitad del tiempo cumplido -> 25% del restante
                    multaCalculada = alquilerRestante * 0.25m;
                }

                // 2. REGLA DE NEGOCIO: Si hay multa y el operador indica que NO se pagó, frenamos la baja
                if (multaCalculada > 0 && !pagaMultaNow)
                {
                    TempData["Error"] = $"No se puede finalizar. Debe abonar la multa de ${multaCalculada:N2} en el momento.";
                    return View(reserva);
                }

                // 3. Guardamos los datos en la entidad
                reserva.FechaCancelacion = fechaCancelacion;
                reserva.Multa = multaCalculada;
            }

            try
            {
                // Ejecutamos la baja logica
                repo_Reserva.Baja(id);

                TempData["Mensaje"] = "Reserva dada de baja con éxito.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al dar de baja la reserva: " + ex.Message;
                return View(reserva);
            }

            return RedirectToAction(nameof(Index));
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
                reserva.IdUsuario = 1;

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


        public IActionResult Inactivos(
            int pagina = 1,
            int tamPagina = 10)
        {
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

            var inquilinos = repo_Inquilino.ObtenerActivos() ?? new List<Inquilino>();

            // filtra los que empiecen con o contengan el texto ingresado
            var resultado = inquilinos
                .Where(i => (i.Nombre + " " + i.Apellido).Contains(term, StringComparison.OrdinalIgnoreCase))
                .Select(i => new
                {
                    id = i.IdInquilino,
                    nombreCompleto = $"{i.Nombre} {i.Apellido} - DNI: {i.Dni}"
                })
                .Take(10) // Limitamos los resultados para que no cargue de mas
                .ToList();

            return Json(resultado);
        }

        // GET: Reserva/BuscarInmueblesPorDireccion?term=rivadavia
        [HttpGet]
        public IActionResult BuscarInmueblesPorDireccion(string term)
        {
            if (string.IsNullOrEmpty(term))
            {
                return Json(new List<object>());
            }

            var inmuebles = repo_Inmueble.ObtenerActivos() ?? new List<Inmueble>();

            var resultado = inmuebles
                .Where(i => i.Direccion.Contains(term, StringComparison.OrdinalIgnoreCase))
                .Select(i => new
                {
                    id = i.IdInmueble,
                    direccion = i.Direccion,
                    precio = i.PrecioAlquiler
                })
                .Take(10)
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



    }
}