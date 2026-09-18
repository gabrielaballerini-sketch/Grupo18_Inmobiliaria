using Grupo18_Inmobiliaria.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Text;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Grupo18_Inmobiliaria.Controllers
{
    [Authorize]
    public class UsuarioController : Controller
    {
        private readonly IRepositorioUsuario repositorioUsuario;
        private readonly IConfiguration configuration;
        private readonly IWebHostEnvironment environment;

        public UsuarioController(
            IRepositorioUsuario repositorio,
            IConfiguration configuration,
            IWebHostEnvironment environment)
        {
            this.repositorioUsuario = repositorio;
            this.configuration = configuration;
            this.environment = environment;
        }

        [Authorize(Roles = "Administrativo")]
        // GET: Usuario
        public IActionResult Index(int pagina = 1, int tamPagina = 10)
        {
            if (pagina < 1) pagina = 1;
            if (tamPagina < 1) tamPagina = 10;

            var usuarios = repositorioUsuario.ObtenerActivos(pagina, tamPagina);
            var cantidad = repositorioUsuario.ObtenerCantidad(true);

            ViewBag.Pagina = pagina;
            ViewBag.TamPagina = tamPagina;
            ViewBag.Cantidad = cantidad;
            ViewBag.TotalPaginas = (int)Math.Ceiling(cantidad / (double)tamPagina);

            return View(usuarios);
        }

        [Authorize(Roles = "Administrativo")]
        // GET: Usuario/Inactivos
        public IActionResult Inactivos(int pagina = 1, int tamPagina = 10)
        {
            if (pagina < 1) pagina = 1;
            if (tamPagina < 1) tamPagina = 10;

            var usuarios = repositorioUsuario.ObtenerInactivos(pagina, tamPagina);
            var cantidad = repositorioUsuario.ObtenerCantidad(false);

            ViewBag.Pagina = pagina;
            ViewBag.TamPagina = tamPagina;
            ViewBag.Cantidad = cantidad;
            ViewBag.TotalPaginas = (int)Math.Ceiling(cantidad / (double)tamPagina);

            return View(usuarios);
        }

        [Authorize(Roles = "Administrativo")]
        // GET: Usuario/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Usuario/Create
        [Authorize(Roles = "Administrativo")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Usuario usuario)
        {
            ModelState.Remove("ListaReservas");
            ModelState.Remove("AvatarFile");

            if (string.IsNullOrWhiteSpace(usuario.UserName))
            {
                ModelState.AddModelError("UserName", "El nombre de usuario es obligatorio.");
            }

            if (string.IsNullOrWhiteSpace(usuario.Password))
            {
                ModelState.AddModelError("Password", "La contraseña es obligatoria.");
            }

            if (!ModelState.IsValid)
            {
                return View(usuario);
            }

            var usuarioExistente = repositorioUsuario.ObtenerPorUserName(usuario.UserName);
            if (usuarioExistente != null)
            {
                ModelState.AddModelError("UserName", "El nombre de usuario ya existe.");
                return View(usuario);
            }

            // Subida de foto de perfil opcional en la creación
            if (usuario.AvatarFile != null && usuario.AvatarFile.Length > 0)
            {
                usuario.Avatar = GuardarAvatar(usuario.AvatarFile);
            }

            usuario.Password = HashPassword(usuario.Password);

            repositorioUsuario.Alta(usuario);
            TempData["Mensaje"] = "Usuario creado correctamente.";

            return RedirectToAction(nameof(Index));
        }

        // GET: Usuario/Edit/5
        [Authorize(Roles = "Administrativo")]
        [HttpGet]
        public IActionResult Edit(int id)
        {
            if (id <= 0) return NotFound();

            var usuario = repositorioUsuario.ObtenerPorId(id);
            if (usuario == null) return NotFound();

            return View(usuario);
        }

        // POST: Usuario/Edit/5
        [Authorize(Roles = "Administrativo")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Usuario usuario)
        {
            if (id != usuario.IdUsuario) return BadRequest();

            ModelState.Remove("ListaReservas");
            ModelState.Remove("AvatarFile");
            ModelState.Remove("Password"); // La contraseña es opcional al editar

            if (string.IsNullOrWhiteSpace(usuario.UserName))
            {
                ModelState.AddModelError("UserName", "El nombre de usuario es obligatorio.");
            }

            if (!ModelState.IsValid)
            {
                return View(usuario);
            }

            var usuarioExistente = repositorioUsuario.ObtenerPorUserName(usuario.UserName);
            if (usuarioExistente != null && usuarioExistente.IdUsuario != usuario.IdUsuario)
            {
                ModelState.AddModelError("UserName", "El nombre de usuario ya existe.");
                return View(usuario);
            }

            // Mantener o actualizar Avatar
            var usuarioDb = repositorioUsuario.ObtenerPorId(id);
            if (usuarioDb == null) return NotFound();

            if (usuario.AvatarFile != null && usuario.AvatarFile.Length > 0)
            {
                EliminarAvatarAnterior(usuarioDb.Avatar);
                usuario.Avatar = GuardarAvatar(usuario.AvatarFile);
            }
            else
            {
                usuario.Avatar = usuarioDb.Avatar;
            }

            // Mantener o actualizar Contraseña
            if (string.IsNullOrWhiteSpace(usuario.Password))
            {
                usuario.Password = usuarioDb.Password;
            }
            else
            {
                usuario.Password = HashPassword(usuario.Password);
            }

            repositorioUsuario.Modificacion(usuario);
            TempData["Mensaje"] = "Usuario modificado correctamente.";

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Administrativo")]
        [HttpGet]
        public IActionResult Baja(int id)
        {
            if (id <= 0) return BadRequest();

            var usuario = repositorioUsuario.ObtenerPorId(id);
            if (usuario == null) return NotFound();

            return View(usuario);
        }

        // POST: Usuario/Delete/5
        [Authorize(Roles = "Administrativo")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            if (id <= 0) return BadRequest();

            var usuario = repositorioUsuario.ObtenerPorId(id);
            if (usuario == null) return NotFound();

            repositorioUsuario.Baja(id);
            return RedirectToAction(nameof(Index));
        }

        // POST: Usuario/Reactivar/5
        [Authorize(Roles = "Administrativo")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Reactivar(int id)
        {
            if (id <= 0) return BadRequest();

            var usuario = repositorioUsuario.ObtenerPorId(id);
            if (usuario == null) return NotFound();

            repositorioUsuario.Reactivar(id);
            return RedirectToAction(nameof(Inactivos));
        }

        [Authorize]
        [HttpGet]
        public IActionResult Perfil()
        {
            string? idUsuario = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(idUsuario) || !int.TryParse(idUsuario, out int id))
            {
                return RedirectToAction("Login", "Cuenta");
            }

            var usuario = repositorioUsuario.ObtenerPorId(id);
            if (usuario == null) return NotFound();

            return View(usuario);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Perfil(Usuario usuario)
        {
            string? idUsuario = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(idUsuario) || !int.TryParse(idUsuario, out int id))
            {
                return RedirectToAction("Login", "Cuenta");
            }

            Usuario? usuarioActual = repositorioUsuario.ObtenerPorId(id);
            if (usuarioActual == null) return NotFound();

            // Preservar datos protegidos
            usuario.IdUsuario = usuarioActual.IdUsuario;
            usuario.RolUsuario = usuarioActual.RolUsuario;
            usuario.Estado = usuarioActual.Estado;

            if (string.IsNullOrWhiteSpace(usuario.UserName))
            {
                usuario.UserName = usuarioActual.UserName;
            }

            // --- LÓGICA DE FOTO DE PERFIL / AVATAR ---
            if (usuario.AvatarFile != null && usuario.AvatarFile.Length > 0)
            {
                // Eliminar foto previa si existe
                EliminarAvatarAnterior(usuarioActual.Avatar);

                // Guardar la nueva foto y asignar URL
                usuario.Avatar = GuardarAvatar(usuario.AvatarFile);
            }
            else
            {
                // Mantiene el avatar actual
                usuario.Avatar = usuarioActual.Avatar;
            }

            // --- LÓGICA DE CONTRASEÑA ---
            bool quiereCambiarPassword =
                !string.IsNullOrWhiteSpace(usuario.PasswordActual) ||
                !string.IsNullOrWhiteSpace(usuario.NuevaPassword) ||
                !string.IsNullOrWhiteSpace(usuario.ConfirmarPassword);

            if (!quiereCambiarPassword)
            {
                usuario.Password = usuarioActual.Password;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(usuario.PasswordActual))
                    ModelState.AddModelError("PasswordActual", "Debe ingresar su contraseña actual.");

                if (string.IsNullOrWhiteSpace(usuario.NuevaPassword))
                    ModelState.AddModelError("NuevaPassword", "Debe ingresar una nueva contraseña.");

                if (string.IsNullOrWhiteSpace(usuario.ConfirmarPassword))
                    ModelState.AddModelError("ConfirmarPassword", "Debe confirmar la nueva contraseña.");

                if (!string.IsNullOrWhiteSpace(usuario.NuevaPassword) &&
                    !string.IsNullOrWhiteSpace(usuario.ConfirmarPassword) &&
                    usuario.NuevaPassword != usuario.ConfirmarPassword)
                {
                    ModelState.AddModelError("ConfirmarPassword", "Las nuevas contraseñas no coinciden.");
                    TempData["Error"] = "Las nuevas contraseñas no coinciden.";
                }

                if (!string.IsNullOrWhiteSpace(usuario.PasswordActual))
                {
                    string hashedPasswordActual = HashPassword(usuario.PasswordActual);

                    if (hashedPasswordActual != usuarioActual.Password)
                    {
                        ModelState.AddModelError("PasswordActual", "La contraseña actual es incorrecta.");
                        TempData["Error"] = "Verifique la contraseña actual.";
                    }
                }

                if (!ModelState.IsValid)
                {
                    return View(usuario);
                }

                usuario.Password = HashPassword(usuario.NuevaPassword);
            }

            repositorioUsuario.Modificacion(usuario);

            // Actualizar Claim de Name y Claim de Avatar/Foto si aplica en las Cookie
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString()),
        new Claim(ClaimTypes.Name, usuario.UserName),
        new Claim(ClaimTypes.Role, usuario.RolUsuario.ToString()), // Asegúrate de mantener el rol
        new Claim("Avatar", usuario.Avatar ?? "") // Si guardas el avatar en los claims para mostrarlo arriba
    };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            // Se pasa el Esquema como primer parámetro
            await HttpContext.SignInAsync(
                "CookieAuth",
                new ClaimsPrincipal(claimsIdentity)
            );

            TempData["Mensaje"] = "Perfil actualizado correctamente.";
            return RedirectToAction("Perfil");
        }

        // --- MÉTODOS AUXILIARES ---

        private string HashPassword(string password)
        {
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: Encoding.ASCII.GetBytes(configuration["Salt"] ?? ""),
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 1000,
                numBytesRequested: 256 / 8
            ));
        }

        private string GuardarAvatar(IFormFile file)
        {
            string folderPath = Path.Combine(environment.WebRootPath, "uploads", "avatars");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string extension = Path.GetExtension(file.FileName);
            string uniqueFileName = $"{Guid.NewGuid()}{extension}";
            string filePath = Path.Combine(folderPath, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            return $"/uploads/avatars/{uniqueFileName}";
        }

        private void EliminarAvatarAnterior(string? avatarUrl)
        {
            if (!string.IsNullOrEmpty(avatarUrl))
            {
                string relativePath = avatarUrl.TrimStart('/');
                string fullPath = Path.Combine(environment.WebRootPath, relativePath);
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
            }
        }
    }
}