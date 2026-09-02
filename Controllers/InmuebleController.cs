using Microsoft.AspNetCore.Mvc;
using Grupo18_Inmobiliaria.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Grupo18_Inmobiliaria.Controllers
{
    public class InmuebleController : Controller
    {
         private readonly RepositorioInmuebleMySql repo_Inmueble;
        private readonly RepositorioPropietarioMySql repo_Propietario;
        private readonly RepositorioTipoInmuebleMySql repo_Tipo;

        public InmuebleController(RepositorioInmuebleMySql repoInmueble,RepositorioPropietarioMySql repoPropietario,RepositorioTipoInmuebleMySql repoTipo)
        {
            this.repo_Inmueble = repoInmueble;
            this.repo_Propietario=repoPropietario;
            this.repo_Tipo=repoTipo;

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
                TempData["Error"] = "Error al crear el inmueble: " + ex.Message;
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
        [HttpPost]
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
                TempData["Error"] = "Error al dar de baja el inmueble.";
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
        TempData["Mensaje"] = "Propietario reactivado con éxito.";
    }
    catch (Exception ex)
    {
        TempData["Error"] = "Error al reactivar el propietario.";
    }

    // Redirige siempre de vuelta a la lista de inactivos o a Index
    return RedirectToAction(nameof(Inactivos));
}


 // --- MÉTODO AUXILIAR PARA SELECTS ---
        private void CargarDesplegables(int selectedPropietario = 0, int selectedTipo = 0)
        {
            var propietarios = repo_Propietario.ObtenerActivos();
            var tipos = repo_Tipo.ObtenerActivos();

            ViewBag.Propietarios = new SelectList(
                propietarios.Select(p => new { Id = p.IdPropietario, NombreCompleto = $"{p.Nombre} {p.Apellido}" }),
                "Id", "NombreCompleto", selectedPropietario);

            ViewBag.TiposInmueble = new SelectList(
                tipos, "IdTipoInmueble", "Descripcion", selectedTipo);
        }






    }



}
