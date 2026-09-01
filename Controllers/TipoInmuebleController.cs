using Microsoft.AspNetCore.Mvc;
using Grupo18_Inmobiliaria.Models;
using Microsoft.JSInterop.Infrastructure;

namespace Grupo18_Inmobiliaria.Controllers
{
    public class TipoInmuebleController : Controller
    {
        private readonly RepositorioTipoInmuebleMySql repo;

        public TipoInmuebleController(RepositorioTipoInmuebleMySql repo)
        {
            this.repo = repo;
        }

        // --- ALTA (CREATE) ---

        // GET: Propietario/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // GET: tipoInmueble/Index
        public IActionResult Index(int pagina = 1, int tamPagina = 10)
{
    try
    {

//  Pedimos los registros para la página solicitada
        var lista = repo.ObtenerActivos(pagina, tamPagina);

//  Contamos la cantidad tde activos, con true 
        int totalRegistros = repo.ObtenerCantidad(true); 


// Pasamos los datos a la vista para construir el paginador
        ViewBag.PaginaActual = pagina;
        ViewBag.TotalPaginas = (int)Math.Ceiling((double)totalRegistros / tamPagina);

        return View(lista);
    }
    catch (Exception ex)
    {
        return Content("ERROR: " + ex.Message);
    }
}


        // POST: tipoInmueble/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(TipoInmueble tipoInmueble)
        {
            if (!ModelState.IsValid)
            {
                return View(tipoInmueble);
            }
                   
                
            repo.Alta(tipoInmueble);
            return RedirectToAction(nameof(Index));
        }

        // --- MODIFICACIÓN (EDIT) ---

        // GET: tipoInmueble/Edit/5
        [HttpGet]
        public IActionResult Edit(int id)
        {
            // Pasa un objeto con el ID cargado para editar en la vista
            var tipoInmueble = repo.ObtenerPorId(id);
            if (tipoInmueble == null)
            {
                return NotFound();
            }
            return View(tipoInmueble);
        }

        // POST: Propietario/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, TipoInmueble tipoInmueble)
        {
            if (id != tipoInmueble.IdTipoInmueble)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(tipoInmueble);
            }

            repo.Modificacion(tipoInmueble);

            TempData["Mensaje"] = "Tipo de inmueble modificado con éxito";
            return RedirectToAction("Index");
        }

        // --- BAJA LÓGICA (DELETE) ---

        // GET: Tipo inmueble /Delete/5
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var tipoInmueble = repo.ObtenerPorId(id);
          if (tipoInmueble == null)
            {
              return NotFound();
             }

            return View(tipoInmueble);
        }

        // POST: TipoInmueble/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            repo.Baja(id); // Ejecuta el UPDATE Estado = 0 en MySQL
            return RedirectToAction("Index");
        }

// GET: TipoInmueble/Inactivos
public IActionResult Inactivos(int pagina = 1, int tamPagina = 10)
{
    try
    {
        var inactivos = repo.ObtenerInactivos(pagina, tamPagina);
        
        
        int totalRegistros = repo.ObtenerCantidad(false); 

        ViewBag.PaginaActual = pagina;
        ViewBag.TotalPaginas = (int)Math.Ceiling((double)totalRegistros / tamPagina);

        return View(inactivos);
    }
    catch (Exception ex)
    {
        return Content("ERROR: " + ex.Message);
    }
}

// POST: TipoInmueble/Reactivar/5
[HttpPost]
[ValidateAntiForgeryToken]
public IActionResult Reactivar(int id)
{
    try
    {
      repo.Reactivar(id);
        TempData["Mensaje"] = "Propietario reactivado con éxito.";
    }
    catch (Exception ex)
    {
        TempData["Error"] = "Error al reactivar el propietario.";
    }

    // Redirige siempre de vuelta a la lista de inactivos o a Index
    return RedirectToAction(nameof(Inactivos));
}











    }



}
