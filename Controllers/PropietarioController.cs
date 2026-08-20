using Microsoft.AspNetCore.Mvc;
using Grupo18_Inmobiliaria.Models;

namespace Grupo18_Inmobiliaria.Controllers
{
    public class PropietarioController : Controller
    {
        private readonly RepositorioPropietarioMySql repo;

        public PropietarioController(RepositorioPropietarioMySql repo)
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

// GET: Propietario/Index
       public IActionResult Index()
{
    try
    {
        var lista = repo.ObtenerTodos();
        return View(lista);
    }
    catch (Exception ex)
    {
        return Content("ERROR: " + ex.Message);
    }
}
       



        // POST: Propietario/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Propietario propietario)
        {
            if (!ModelState.IsValid)
            {
                return View(propietario);
            }

            repo.Alta(propietario);
            return RedirectToAction(nameof(Index));
        }

        // --- MODIFICACIÓN (EDIT) ---

        // GET: Propietario/Edit/5
        [HttpGet]
        public IActionResult Edit(int id)
        {
            // Pasa un objeto con el ID cargado para editar en la vista
            var propietario = new Propietario { IdPropietario = id };
            return View(propietario);
        }

        // POST: Propietario/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Propietario propietario)
        {
            if (id != propietario.IdPropietario)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(propietario);
            }

            repo.Modificacion(propietario);
            return RedirectToAction("Index", "Home");
        }

        // --- BAJA LÓGICA (DELETE) ---

        // GET: Propietario/Delete/5
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var propietario = new Propietario { IdPropietario = id };
            return View(propietario);
        }

        // POST: Propietario/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            repo.Baja(id); // Ejecuta el UPDATE Estado = 0 en MySQL
            return RedirectToAction("Index", "Home");
        }
    }
}