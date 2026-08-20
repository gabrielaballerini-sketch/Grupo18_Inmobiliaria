using Microsoft.Extensions.Configuration;

namespace Grupo18_Inmobiliaria.Models
{
    public abstract class RepositorioBase
    {
        protected readonly IConfiguration configuration;
        protected readonly string connectionString;

        protected RepositorioBase(IConfiguration configuration)
        {
            this.configuration = configuration;

            connectionString = configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Falta la connection string DefaultConnection");
        }
    }
}