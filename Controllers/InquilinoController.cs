using Microsoft.AspNetCore.Mvc;
using Grupo18_Inmobiliaria.Models;

namespace Grupo18_Inmobiliaria.Controllers
{
    public class InquilinoController : Controller
    {
        private readonly RepositorioInquilinoMySql repo;

        public InquilinoController(RepositorioInquilinoMySql repo)
        {
            this.repo = repo;
        }

        // --- ALTA (CREATE) ---

        // GET: Inquilino/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // GET: Inquilino/Index
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

        // POST: Inquilino/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Inquilino inquilino)
        {
            if (!ModelState.IsValid)
            {
                return View(inquilino);
            }
            if (repo.ObtenerporDni(inquilino.Dni))
            {
                ModelState.AddModelError("Dni", "El dni ya esta registrado");
                return View(inquilino);
            }

            repo.Alta(inquilino);
            return RedirectToAction(nameof(Index));
        }

        // --- MODIFICACIÓN (EDIT) ---

        // GET: Inquilino/Edit/5
        [HttpGet]
        public IActionResult Edit(int id)
        {
            // Pasa un objeto con el ID cargado para editar en la vista
            var inquilino = repo.ObtenerPorId(id);
            if (inquilino == null)
            {
                return NotFound();
            }

            return View(inquilino);
        }

        // POST: Inquilino/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Inquilino inquilino)
        {
            if (id != inquilino.IdInquilino)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(inquilino);
            }

            repo.Modificacion(inquilino);

            TempData["Mensaje"] = "Inquilino modificado con éxito";
            return RedirectToAction("Index");
        }

        // --- BAJA LÓGICA (DELETE) ---

        // GET: Inquilino/Delete/5
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var inquilino = new Inquilino
            {
                IdInquilino = id
            };

            return View(inquilino);
        }

        // POST: Inquilino/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            repo.Baja(id); // Ejecuta el UPDATE Estado = 0 en MySQL

            return RedirectToAction("Index", "Home");
        }
    }
}