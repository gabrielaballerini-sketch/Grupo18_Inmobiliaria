using Grupo18_Inmobiliaria.Models;
using System.Data;
using MySqlConnector;
namespace Grupo18_Inmobiliaria.Models
{
    public class RepositorioInmuebleMySql : RepositorioBase
    {
           public RepositorioInmuebleMySql(IConfiguration configuration) : base(configuration)
        {
            
        }
        public int Alta(Inmueble inmueble)
        {
            int res =0;

            using (var connection= new MySqlConnection(connectionString))
            {
                string sql="""
                INSERT Into Inmuebles(Direccion,Capacidad,PrecioAlquiler,IdPropietario,Estado,IdTipoInmueble,Latitud,Longitud)
                VALUES(@Direccion,@Capacidad,@PrecioAlquiler,@IdPropietario,1,@IdTipoInmueble,@Latitud,@Longitud)
                SELECT LAST_INSERT_ID();
                """;
                using (var command=new MySqlCommand(sql,connection))
                {
                    command.CommandType = CommandType.Text;

                    command.Parameters.AddWithValue("@Direccion", inmueble.Direccion);
                    command.Parameters.AddWithValue("@Capacidad", inmueble.Capacidad);
                    command.Parameters.AddWithValue("@PrecioAlquiler", inmueble.PrecioAlquiler);
                    command.Parameters.AddWithValue("@IdPropietario", inmueble.IdPropietario);
                    command.Parameters.AddWithValue("@IdTipoInmueble", inmueble.IdTipoInmueble);
                    command.Parameters.AddWithValue("@Latitud", inmueble.Latitud);
                    command.Parameters.AddWithValue("@Longitud", inmueble.Longitud);
                    
                    connection.Open();

                     res = Convert.ToInt32(command.ExecuteScalar());
                    inmueble.IdInmueble = res;

                }
                
            }
            return res;
        }
    }
    

}