using Microsoft.AspNetCore.Mvc;
using Grupo18_Inmobiliaria.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Grupo18_Inmobiliaria.Controllers
{
    public class InmuebleController : Controller
    {
        private readonly IRepositorioInmueble repo_Inmueble;
        private readonly IRepositorioPropietario repo_Propietario;
        private readonly IRepositorio<TipoInmueble> repo_Tipo;
        private readonly IRepositorioImagen repo_Imagen;

        public InmuebleController(IRepositorioInmueble repoInmueble, IRepositorioPropietario repoPropietario, IRepositorio<TipoInmueble> repoTipo, IRepositorioImagen repoImagen)
        {
            this.repo_Inmueble = repoInmueble;
            this.repo_Propietario = repoPropietario;
            this.repo_Tipo = repoTipo;
            this.repo_Imagen = repoImagen;

        }

        public IActionResult Index(int pagina = 1, int tamPagina = 10)
        {
            try
            {
                var lista = repo_Inmueble.ObtenerActivos(pagina, tamPagina);
                int totalRegistros = repo_Inmueble.ObtenerCantidad(true);

                ViewBag.PaginaActual = pagina;
                ViewBag.TotalPaginas = (int)Math.Ceiling((double)totalRegistros / tamPagina);

                return View(lista);
            }
            catch (Exception ex)
            {
                return Content("ERROR: " + ex.Message);
            }
        }




        // --- ALTA (CREATE) ---

        // GET: Inmueble/Create
        [HttpGet]
        public IActionResult Create()
        {
            CargarDesplegables();
            return View();
        }

        // POST: Inmueble/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Inmueble inmueble)
        {
            ModelState.Remove("Propietario");
            ModelState.Remove("TipoInmueble");
            ModelState.Remove("ListaReservas");

            if (!ModelState.IsValid)
            {
                CargarDesplegables(inmueble.IdPropietario, inmueble.IdTipoInmueble);
                return View(inmueble);
            }

            try
            {
                repo_Inmueble.Alta(inmueble);
                TempData["Mensaje"] = "Inmueble creado con éxito.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al crear el inmueble: " + ex;
                CargarDesplegables(inmueble.IdPropietario, inmueble.IdTipoInmueble);
                return View(inmueble);
            }
        }





        // --- MODIFICACIÓN (EDIT) ---

        // GET: Inmueble/Edit/5
        [HttpGet]
        public IActionResult Edit(int id)
        {
            // Pasa un objeto con el ID cargado para editar en la vista
            var Inmueble = repo_Inmueble.ObtenerPorId(id);
            if (Inmueble == null)
            {
                return NotFound();
            }

            CargarDesplegables(Inmueble.IdPropietario, Inmueble.IdTipoInmueble);
            return View(Inmueble);
        }




        // POST: Inmueble/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Inmueble inmueble)
        {
            if (id != inmueble.IdInmueble)
            {
                return NotFound();
            }

            ModelState.Remove("Propietario");
            ModelState.Remove("TipoInmueble");
            ModelState.Remove("ListaReservas");

            if (!ModelState.IsValid)
            {
                CargarDesplegables(inmueble.IdPropietario, inmueble.IdTipoInmueble);
                return View(inmueble);
            }

            try
            {
                repo_Inmueble.Modificacion(inmueble);
                TempData["Mensaje"] = "Inmueble modificado con éxito.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al modificar el inmueble: " + ex.Message;
                CargarDesplegables(inmueble.IdPropietario, inmueble.IdTipoInmueble);
                return View(inmueble);
            }
        }




        // --- BAJA LÓGICA (DELETE) ---

        // GET:  Inmueble /Delete/5
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var Inmueble = repo_Inmueble.ObtenerPorId(id);
            if (Inmueble == null)
            {
                return NotFound();
            }

            return View(Inmueble);
        }



        // POST: Inmueble/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                repo_Inmueble.Baja(id);
                TempData["Mensaje"] = "Inmueble dado de baja con éxito.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al dar de baja el inmueble." + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Inmueble/Inactivos
        public IActionResult Inactivos(int pagina = 1, int tamPagina = 10)
        {
            try
            {
                var inactivos = repo_Inmueble.ObtenerInactivos(pagina, tamPagina);


                int totalRegistros = repo_Inmueble.ObtenerCantidad(false);

                ViewBag.PaginaActual = pagina;
                ViewBag.TotalPaginas = (int)Math.Ceiling((double)totalRegistros / tamPagina);

                return View(inactivos);
            }
            catch (Exception ex)
            {
                return Content("ERROR: " + ex.Message);
            }
        }

        // POST: Inmueble/Reactivar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Reactivar(int id)
        {
            try
            {
                repo_Inmueble.Reactivar(id);
                TempData["Mensaje"] = "Inmueble reactivado con éxito.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al reactivar el inmueble." + ex.Message;
            }

            // Redirige siempre de vuelta a la lista de inactivos o a Index
            return RedirectToAction(nameof(Inactivos));
        }


        // --- MÉTODO AUXILIAR PARA SELECTS ---
        private void CargarDesplegables(int selectedPropietario = 0, int selectedTipo = 0)
        {
            var propietarios = repo_Propietario.ObtenerActivos() ?? new List<Propietario>();
            var tipos = repo_Tipo.ObtenerActivos() ?? new List<TipoInmueble>();

            ViewBag.Propietarios = new SelectList(
                propietarios.Select(p => new { Id = p.IdPropietario, NombreCompleto = $"{p.Nombre} {p.Apellido}" }),
                "Id", "NombreCompleto", selectedPropietario);

            ViewBag.TiposInmueble = new SelectList(
                tipos, "IdTipoInmueble", "Descripcion", selectedTipo);
        }

        //vue
        // GET: Inmueble/PorPropietario/5
        [HttpGet]
        public IActionResult PorPropietario(int id)
        {
            //filtra los inmuebles de este propietario
            var lista = repo_Inmueble.BuscarPorPropietario(id);
            return Json(lista);
        }

        // GET: Inmueble/ObtenerJson/5
        [HttpGet]
        public IActionResult ObtenerJson(int id)
        {
            var inmueble = repo_Inmueble.ObtenerPorId(id);
            if (inmueble == null) return NotFound();

            return Json(new
            {
                idInmueble = inmueble.IdInmueble,
                direccion = inmueble.Direccion,
                capacidad = inmueble.Capacidad,
                latitud = inmueble.Latitud,
                longitud = inmueble.Longitud,
                precioAlquiler = inmueble.PrecioAlquiler,
                porcentajeReserva = inmueble.PorcentajeReserva,
                estado = inmueble.Estado,
                tipoInmueble = inmueble.TipoInmueble != null ? new
                {
                    idTipoInmueble = inmueble.TipoInmueble.IdTipoInmueble,
                    descripcion = inmueble.TipoInmueble.Descripcion
                } : null,
                propietario = inmueble.Propietario != null ? new
                {
                    idPropietario = inmueble.Propietario.IdPropietario,
                    nombre = inmueble.Propietario.Nombre,
                    apellido = inmueble.Propietario.Apellido,
                    dni = inmueble.Propietario.Dni,
                    telefono = inmueble.Propietario.Telefono,
                    email = inmueble.Propietario.Email
                } : null
            });
        }
        [HttpGet]
        public IActionResult BuscarDisponibles(DateTime fechaInicio, DateTime fechaFin)
        {
            var inmuebles = repo_Inmueble.ObtenerDisponiblesEntreFechas(fechaInicio, fechaFin);
            return Json(inmuebles);
        }




        [HttpGet]
        public IActionResult Disponibles()
        {
            return View();
        }




        [HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> CreateAjax(Inmueble inmueble, List<IFormFile> imagenes, [FromServices] IWebHostEnvironment environment, [FromServices] IRepositorioImagen repositorioImagen)
{
    // Limpiamos del ModelState los objetos de navegación que vienen nulos en el POST
    ModelState.Remove("Propietario");
    ModelState.Remove("TipoInmueble");
    ModelState.Remove("Imagenes");
    ModelState.Remove("ImagenUrl");

    if (!ModelState.IsValid)
    {
        var errores = ModelState
            .Where(x => x.Value.Errors.Count > 0)
            .Select(x => new {
                Campo = x.Key,
                Errores = x.Value.Errors.Select(e => e.ErrorMessage).ToList()
            });

        return BadRequest(errores);
    }

    try
    {
        // 1. Guardar el inmueble en la BD y obtener su ID recién generado
        int nuevoId = repo_Inmueble.Alta(inmueble);

        // 2. Si se adjuntaron fotos, crear carpeta, guardarlas en wwwroot y registrar la portada
        if (imagenes != null && imagenes.Count > 0)
        {
            string wwwPath = environment.WebRootPath;
            string path = Path.Combine(wwwPath, "Uploads", "Inmuebles", nuevoId.ToString());

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string? primeraUrl = null;

            foreach (var file in imagenes)
            {
                if (file.Length > 0)
                {
                    var extension = Path.GetExtension(file.FileName);
                    var nombreArchivo = $"{Guid.NewGuid()}{extension}";
                    var rutaArchivo = Path.Combine(path, nombreArchivo);

                    using (var stream = new FileStream(rutaArchivo, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    var url = $"/Uploads/Inmuebles/{nuevoId}/{nombreArchivo}";

                    // Guardar en la tabla de imágenes asociada al inmueble
                    Imagen imagen = new Imagen
                    {
                        IdInmueble = nuevoId,
                        Url = url
                    };
                    repositorioImagen.Alta(imagen);

                    // La primera imagen subida queda como portada
                    if (primeraUrl == null)
                    {
                        primeraUrl = url;
                    }
                }
            }

            if (primeraUrl != null)
            {
                repo_Inmueble.ModificarPortada(nuevoId, primeraUrl);
            }
        }

        return Ok();
    }
    catch (Exception ex)
    {
        return BadRequest("Ocurrió un error en el servidor: " + ex.Message);
    }
}



        [HttpGet]
public IActionResult Details(int id)
{
    var inmueble = repo_Inmueble.ObtenerPorId(id);
    if (inmueble == null)
    {
        return NotFound();
    }

    inmueble.ListaImagenes = repo_Imagen.ObtenerPorInmueble(id);

    return View(inmueble);
}



    }

}
