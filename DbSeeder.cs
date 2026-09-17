using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Text;
using Grupo18_Inmobiliaria.Models;

namespace Grupo18_Inmobiliaria
{
    public static class DbSeeder
    {
        public static void Seed(IServiceProvider serviceProvider)
        {
            var repo = serviceProvider.GetRequiredService<IRepositorioUsuario>();
            var config = serviceProvider.GetRequiredService<IConfiguration>();

            // Verificamos si existe el usuario administrador
            if (repo.ObtenerPorUserName("admin@inmobiliaria.com") == null)
            {
                string salt = config["Salt"] ?? "";
                string passwordPlana = "admin1234";

                string passwordHashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: passwordPlana,
                    salt: Encoding.ASCII.GetBytes(salt),
                    prf: KeyDerivationPrf.HMACSHA1,
                    iterationCount: 1000,
                    numBytesRequested: 256 / 8
                ));

                var admin = new Usuario
                {
                    UserName = "admin@inmobiliaria.com",
                    Password = passwordHashed,
                    RolUsuario = (RolUsuario)1, 
                    Estado = true
                };

                repo.Alta(admin);
            }
        }
    }


 }