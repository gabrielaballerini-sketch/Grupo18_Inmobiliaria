using Microsoft.AspNetCore.Mvc;
using Grupo18_Inmobiliaria.Models;
using Microsoft.AspNetCore.Authorization;

namespace Grupo18_Inmobiliaria.Controllers
{
    [Authorize]
    public class InquilinoController : Controller
    {
        private readonly IRepositorioInquilino repo;

        public InquilinoController(IRepositorioInquilino repo)
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
        public IActionResult Index(int pagina = 1, int tamPagina = 10)
        {
            if (pagina < 1)
                pagina = 1;

            if (tamPagina <= 0)
                tamPagina = 10;
            try
            {
                var lista = repo.ObtenerActivos(pagina, tamPagina);

                int totalRegistros = repo.ObtenerCantidad(true);

                ViewBag.PaginaActual = pagina;
                ViewBag.TotalPaginas = (int)Math.Ceiling((double)totalRegistros / tamPagina);


                return View(lista);
            }
            catch (Exception ex)
            {
                return Content("ERROR: " + ex.ToString());
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
            if (repo.ObtenerPorDni(inquilino.Dni))
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
        [Authorize(Roles ="Administrativo")]
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var inquilino = repo.ObtenerPorId(id);


            if (inquilino == null)
            {
                return NotFound();
            }



            return View(inquilino);
        }

        // POST: Inquilino/Delete/5
        [Authorize(Roles ="Administrativo")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            repo.Baja(id); // Ejecuta el UPDATE Estado = 0 en MySQL

            TempData["Mensaje"] = "Inquilino eliminado con éxito";
            return RedirectToAction("Index");
        }

        [Authorize(Roles ="Administrativo")]
        public IActionResult Inactivos(int pagina = 1, int tamPagina = 10)
             {
            if (pagina < 1)
                pagina = 1;

            if (tamPagina <= 0)
                tamPagina = 10;
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


        // POST: Inquilinos/Reactivar/5
        [Authorize(Roles ="Administrativo")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Reactivar(int id)
        {
            try
            {
                repo.Reactivar(id);
                TempData["Mensaje"] = "Inquilino reactivado con éxito.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al reactivar el inquilino." + ex.Message;
            }

            // Redirige siempre de vuelta a la lista de inactivos o a Index
            return RedirectToAction(nameof(Inactivos));
        }




    }
}