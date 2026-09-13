
using Grupo18_Inmobiliaria.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Grupo18_Inmobiliaria.Controllers
{
    [Authorize(Roles = "Administrativo")]
    public class UsuarioController : Controller
    {
        private readonly IRepositorioUsuario repositorioUsuario;

        public UsuarioController(IRepositorioUsuario repositorioUsuario)
        {
            this.repositorioUsuario = repositorioUsuario;
        }

        // GET: Usuario
        public IActionResult Index(int pagina = 1, int tamPagina = 10)
        {
            if (pagina < 1)
                pagina = 1;

            if (tamPagina < 1)
                tamPagina = 10;

            var usuarios = repositorioUsuario.ObtenerActivos(
                pagina,
                tamPagina
            );

            var cantidad = repositorioUsuario.ObtenerCantidad(true);

            ViewBag.Pagina = pagina;
            ViewBag.TamPagina = tamPagina;
            ViewBag.Cantidad = cantidad;
            ViewBag.TotalPaginas = (int)Math.Ceiling(
                cantidad / (double)tamPagina
            );

            return View(usuarios);
        }

        // GET: Usuario/Inactivos
        public IActionResult Inactivos(int pagina = 1, int tamPagina = 10)
        {
            if (pagina < 1)
                pagina = 1;

            if (tamPagina < 1)
                tamPagina = 10;

            var usuarios = repositorioUsuario.ObtenerInactivos(
                pagina,
                tamPagina
            );

            var cantidad = repositorioUsuario.ObtenerCantidad(false);

            ViewBag.Pagina = pagina;
            ViewBag.TamPagina = tamPagina;
            ViewBag.Cantidad = cantidad;
            ViewBag.TotalPaginas = (int)Math.Ceiling(
                cantidad / (double)tamPagina
            );

            return View(usuarios);
        }

        // GET: Usuario/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Usuario/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Usuario usuario)
        {
            ModelState.Remove("ListaReservas");

            if (string.IsNullOrWhiteSpace(usuario.UserName))
            {
                ModelState.AddModelError(
                    "UserName",
                    "El nombre de usuario es obligatorio."
                );
            }

            if (string.IsNullOrWhiteSpace(usuario.Password))
            {
                ModelState.AddModelError(
                    "Password",
                    "La contraseña es obligatoria."
                );
            }

            if (!ModelState.IsValid)
            {
                return View(usuario);
            }

            var usuarioExistente =
                repositorioUsuario.ObtenerPorUserName(usuario.UserName);

            if (usuarioExistente != null)
            {
                ModelState.AddModelError(
                    "UserName",
                    "El nombre de usuario ya existe."
                );

                return View(usuario);
            }

            usuario.Estado = true;

            repositorioUsuario.Alta(usuario);

            return RedirectToAction(nameof(Index));
        }

        // GET: Usuario/Edit/5
        [HttpGet]
        public IActionResult Edit(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            var usuario = repositorioUsuario.ObtenerPorId(id);

            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // POST: Usuario/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Usuario usuario)
        {
            if (id != usuario.IdUsuario)
            {
                return BadRequest();
            }

            ModelState.Remove("ListaReservas");

            if (string.IsNullOrWhiteSpace(usuario.UserName))
            {
                ModelState.AddModelError(
                    "UserName",
                    "El nombre de usuario es obligatorio."
                );
            }

            if (string.IsNullOrWhiteSpace(usuario.Password))
            {
                ModelState.AddModelError(
                    "Password",
                    "La contraseña es obligatoria."
                );
            }

            if (!ModelState.IsValid)
            {
                return View(usuario);
            }

            var usuarioExistente =
                repositorioUsuario.ObtenerPorUserName(usuario.UserName);

            if (usuarioExistente != null &&
                usuarioExistente.IdUsuario != usuario.IdUsuario)
            {
                ModelState.AddModelError(
                    "UserName",
                    "El nombre de usuario ya existe."
                );

                return View(usuario);
            }

            repositorioUsuario.Modificacion(usuario);

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult Baja(int id)
        {
            if (id <= 0)
                return BadRequest();

            var usuario = repositorioUsuario.ObtenerPorId(id);

            if (usuario == null)
                return NotFound();

            return View(usuario);
        }

        // POST: Usuario/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            var usuario = repositorioUsuario.ObtenerPorId(id);

            if (usuario == null)
            {
                return NotFound();
            }

            repositorioUsuario.Baja(id);

            return RedirectToAction(nameof(Index));
        }

        // POST: Usuario/Reactivar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Reactivar(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            var usuario = repositorioUsuario.ObtenerPorId(id);

            if (usuario == null)
            {
                return NotFound();
            }

            repositorioUsuario.Reactivar(id);

            return RedirectToAction(nameof(Inactivos));
        }
    }
}

