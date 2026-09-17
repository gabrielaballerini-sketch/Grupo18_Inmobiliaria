
using System.Security.Claims;
using Grupo18_Inmobiliaria.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Text;

namespace Grupo18_Inmobiliaria.Controllers
{
    public class CuentaController : Controller
    {
        private readonly IRepositorioUsuario repositorioUsuario;
        private readonly IConfiguration configuration;

        public CuentaController(IRepositorioUsuario repositorioUsuario,IConfiguration configuration)
        {
            this.repositorioUsuario = repositorioUsuario;
            this.configuration=configuration;
        }
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index","Home");
            }

            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string userName, string password)
        {
            if (string.IsNullOrWhiteSpace(userName) ||
                string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Debe ingresar usuario y contraseña.";
                return View();
            }

            Usuario? usuario = repositorioUsuario.ObtenerPorUserName(userName);

            if (usuario == null)
            {
                ViewBag.Error = "Usuario o contraseña incorrectos.";
                return View();
            }

            if (!usuario.Estado)
            {
                ViewBag.Error = "El usuario se encuentra inactivo.";
                return View();
            }
             

            var claims = new List<Claim>
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    usuario.IdUsuario.ToString()
                ),

                new Claim(
                    ClaimTypes.Name,
                    usuario.UserName
                ),

                new Claim(
                     ClaimTypes.Role,
                    usuario.RolUsuario.ToString()
                )
            };

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("CookieAuth",principal);

            return RedirectToAction("Index","Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("CookieAuth");

            return RedirectToAction("Login", "Cuenta");
        }

        [HttpGet]
        public IActionResult AccesoDenegado()
        {
            return View();
        }
        
    }
}

