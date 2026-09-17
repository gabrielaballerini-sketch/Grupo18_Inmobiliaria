
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

        public UsuarioController(IRepositorioUsuario repositorio,
            IConfiguration configuration)
        {
            this.repositorioUsuario = repositorio;
            this.configuration = configuration;
        }
        [Authorize(Roles = "Administrativo")]
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
        [Authorize(Roles = "Administrativo")]
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


            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: usuario.Password,
            salt: Encoding.ASCII.GetBytes(configuration["Salt"] ?? ""),
            prf: KeyDerivationPrf.HMACSHA1,
            iterationCount: 1000,
            numBytesRequested: 256 / 8
               )
            );

            usuario.Password = hashed;


            repositorioUsuario.Alta(usuario);
            TempData["Mensaje"] = "Usuario creado correctamente.";

            return RedirectToAction(nameof(Index));
        }

        // GET: Usuario/Edit/5
        [Authorize(Roles = "Administrativo")]
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
        [Authorize(Roles = "Administrativo")]
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

            if (string.IsNullOrWhiteSpace(usuario.Password))
            {
                // No cambió la contraseña
                usuario.Password = usuarioExistente.Password;
            }
            else
            {
                // Cambió la contraseña → la hasheamos
                string hashed = Convert.ToBase64String(
                    KeyDerivation.Pbkdf2(
                        password: usuario.Password,
                        salt: Encoding.ASCII.GetBytes(configuration["Salt"] ?? ""),
                        prf: KeyDerivationPrf.HMACSHA1,
                        iterationCount: 1000,
                        numBytesRequested: 256 / 8
                    )
                );

                usuario.Password = hashed;
            }

            repositorioUsuario.Modificacion(usuario);
            TempData["Mensaje"] = "Usuario modificado correctamente.";

            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = "Administrativo")]
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
        [Authorize(Roles = "Administrativo")]
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
        [Authorize(Roles = "Administrativo")]
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
        [Authorize]
        [HttpGet]
        public IActionResult Perfil()
        {
            string? idUsuario = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(idUsuario))
            {
                return RedirectToAction("Login", "Cuenta");
            }

            if (!int.TryParse(idUsuario, out int id))
            {
                return RedirectToAction("Login", "Cuenta");
            }

            var usuario = repositorioUsuario.ObtenerPorId(id);

            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]



        public async Task<IActionResult> Perfil(Usuario usuario)
        {
            // 1. Obtener el ID del usuario logueado desde el Claim
            string? idUsuario =
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(idUsuario))
            {
                return RedirectToAction("Login", "Cuenta");
            }

            if (!int.TryParse(idUsuario, out int id))
            {
                return RedirectToAction("Login", "Cuenta");
            }

         
            Usuario? usuarioActual =
                repositorioUsuario.ObtenerPorId(id);

            if (usuarioActual == null)
            {
                return NotFound();
            }

            
            usuario.IdUsuario = usuarioActual.IdUsuario;
            usuario.RolUsuario = usuarioActual.RolUsuario;
            usuario.Estado = usuarioActual.Estado;

            
            if (string.IsNullOrWhiteSpace(usuario.UserName))
            {
                usuario.UserName = usuarioActual.UserName;
            }

         

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
                {
                    ModelState.AddModelError(
                        "PasswordActual",
                        "Debe ingresar su contraseña actual."
                    );
                }

                if (string.IsNullOrWhiteSpace(usuario.NuevaPassword))
                {
                    ModelState.AddModelError(
                        "NuevaPassword",
                        "Debe ingresar una nueva contraseña."
                    );
                }

                if (string.IsNullOrWhiteSpace(usuario.ConfirmarPassword))
                {
                    ModelState.AddModelError(
                        "ConfirmarPassword",
                        "Debe confirmar la nueva contraseña."
                    );
                }

       

                if (!string.IsNullOrWhiteSpace(usuario.NuevaPassword) &&
                    !string.IsNullOrWhiteSpace(usuario.ConfirmarPassword))
                {
                    if (usuario.NuevaPassword != usuario.ConfirmarPassword)
                    {
                        ModelState.AddModelError(
                            "ConfirmarPassword",
                            "Las nuevas contraseñas no coinciden."
                        );
                    }
                }

                if (!string.IsNullOrWhiteSpace(usuario.PasswordActual))
                {
                    string hashedPasswordActual =
                        Convert.ToBase64String(
                            KeyDerivation.Pbkdf2(
                                password: usuario.PasswordActual,
                                salt: Encoding.ASCII.GetBytes(
                                    configuration["Salt"] ?? ""
                                ),
                                prf: KeyDerivationPrf.HMACSHA1,
                                iterationCount: 1000,
                                numBytesRequested: 256 / 8
                            )
                        );

                    if (hashedPasswordActual != usuarioActual.Password)
                    {
                        ModelState.AddModelError(
                            "PasswordActual",
                            "La contraseña actual es incorrecta."
                            
                        );
                        TempData["Error"] =
                        "verifique la contraseña actual";

                    }
                }

        
                if (!ModelState.IsValid)
                {
                    return View(usuario);
                }

     
                usuario.Password =
                    Convert.ToBase64String(
                        KeyDerivation.Pbkdf2(
                            password: usuario.NuevaPassword,
                            salt: Encoding.ASCII.GetBytes(
                                configuration["Salt"] ?? ""
                            ),
                            prf: KeyDerivationPrf.HMACSHA1,
                            iterationCount: 1000,
                            numBytesRequested: 256 / 8
                        )
                    );
            }

         
            repositorioUsuario.Modificacion(usuario);

         
            var identity = User.Identity as ClaimsIdentity;

            if (identity != null)
            {
                var claimNombre =
                    identity.FindFirst(ClaimTypes.Name);

                if (claimNombre != null)
                {
                    identity.RemoveClaim(claimNombre);
                }

                identity.AddClaim(
                    new Claim(
                        ClaimTypes.Name,
                        usuario.UserName
                    )
                );

                await HttpContext.SignInAsync(
                    "CookieAuth",
                    new ClaimsPrincipal(identity)
                );
            }

            TempData["Mensaje"] =
                "Perfil actualizado correctamente.";

            return RedirectToAction("Perfil");
        }
    }
}