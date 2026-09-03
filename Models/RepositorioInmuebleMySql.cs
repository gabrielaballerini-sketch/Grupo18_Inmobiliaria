using Grupo18_Inmobiliaria.Models;
using System.Data;
using MySqlConnector;
using Microsoft.AspNetCore.Mvc.Razor;

namespace Grupo18_Inmobiliaria.Models
{
    public class RepositorioInmuebleMySql : RepositorioBase
    {
        public RepositorioInmuebleMySql(IConfiguration configuration) : base(configuration)
        {

        }
        public int Alta(Inmueble inmueble)
        {
            int res = -1;

            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = """
                INSERT Into inmuebles(Direccion,Capacidad,PrecioAlquiler,IdPropietario,Estado,IdTipoInmueble,Latitud,Longitud)
                VALUES(@Direccion,@Capacidad,@PrecioAlquiler,@IdPropietario,1,@IdTipoInmueble,@Latitud,@Longitud);
                SELECT LAST_INSERT_ID();
                """;
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;

                    command.Parameters.AddWithValue("@Direccion", inmueble.Direccion);
                    command.Parameters.AddWithValue("@Capacidad", inmueble.Capacidad);
                    command.Parameters.AddWithValue("@PrecioAlquiler", inmueble.PrecioAlquiler);
                    command.Parameters.AddWithValue("@IdPropietario", inmueble.IdPropietario);
                    command.Parameters.AddWithValue("@IdTipoInmueble", inmueble.IdTipoInmueble);
                    command.Parameters.AddWithValue("@Latitud", (object?)inmueble.Latitud ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Longitud", (object?)inmueble.Longitud ?? DBNull.Value);

                    connection.Open();

                    res = Convert.ToInt32(command.ExecuteScalar());
                    inmueble.IdInmueble = res;

                }

            }
            return res;
        }

        public int Baja(int id)
        {
            int res = -1;

            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @" UPDATE inmuebles  SET estado = 0 WHERE IdInmueble = @IdInmueble";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@IdInmueble", id);

                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }

        public int Modificacion(Inmueble inmueble)
        {
            int res = -1;

            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"UPDATE inmuebles
                SET 
                    Direccion = @direccion,
                    Capacidad = @capacidad,
                    Latitud = @latitud,
                    Longitud = @longitud,
                    PrecioAlquiler = @precioAlquiler,
                    IdPropietario = @idPropietario,
                    IdTipoInmueble = @idTipoInmueble
                WHERE IdInmueble = @IdInmueble;";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;

                    command.Parameters.AddWithValue("@direccion", inmueble.Direccion);
                    command.Parameters.AddWithValue("@capacidad", inmueble.Capacidad);
                    command.Parameters.AddWithValue("@latitud", (object?)inmueble.Latitud ?? DBNull.Value);
                    command.Parameters.AddWithValue("@longitud", (object?)inmueble.Longitud ?? DBNull.Value);
                    command.Parameters.AddWithValue("@precioAlquiler", inmueble.PrecioAlquiler);
                    command.Parameters.AddWithValue("@idPropietario", inmueble.IdPropietario);
                    command.Parameters.AddWithValue("@idTipoInmueble", inmueble.IdTipoInmueble);
                    command.Parameters.AddWithValue("@IdInmueble", inmueble.IdInmueble);

                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }

        public int Reactivar(int id)
        {
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"UPDATE inmuebles SET estado = 1 WHERE IdInmueble = @IdInmueble;";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@IdInmueble", id);

                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }
   

  // OBTENER TODOS los activos
        public IList<Inmueble> ObtenerActivos(int pagina = 1, int tamPagina = 10)
        {
           IList<Inmueble> listaActivos = new List<Inmueble>();

            using (var connection = new MySqlConnection(connectionString))
            {

             string query = $@"
                    SELECT i.IdInmueble, i.Direccion, i.Capacidad, i.Latitud, i.Longitud, i.PrecioAlquiler, i.Estado,
                           i.IdPropietario, p.Nombre AS PropNombre, p.Apellido AS PropApellido,
                           i.IdTipoInmueble, t.Descripcion AS TipoDescripcion
                    FROM inmuebles i
                     JOIN propietarios p ON i.IdPropietario = p.IdPropietario
                     JOIN tipoinmueble t ON i.IdTipoInmueble = t.IdTipoInmueble
                    WHERE i.Estado = 1
                    LIMIT {tamPagina} OFFSET {(pagina - 1) * tamPagina};";

            

                using (var command = new MySqlCommand(query, connection))
                 {
                   command.CommandType = CommandType.Text;
                   connection.Open();

        
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            
                            listaActivos.Add(MapearInmueble(reader));
                        }
                    }
                }
            }

            return listaActivos;
        }


private Inmueble MapearInmueble(MySqlDataReader reader)
        {
            return new Inmueble
            {
                IdInmueble = reader.GetInt32(reader.GetOrdinal("IdInmueble")),
                Direccion = reader.GetString(reader.GetOrdinal("Direccion")),
                Capacidad = reader.GetInt32(reader.GetOrdinal("Capacidad")),
                Latitud = reader.GetDecimal(reader.GetOrdinal("Latitud")) ,
                Longitud = reader.GetDecimal(reader.GetOrdinal("Longitud")) ,
                PrecioAlquiler = reader.GetDecimal(reader.GetOrdinal("PrecioAlquiler")),
                Estado = reader.GetBoolean(reader.GetOrdinal("Estado")),
                IdPropietario = reader.GetInt32(reader.GetOrdinal("IdPropietario")),
                IdTipoInmueble = reader.GetInt32(reader.GetOrdinal("IdTipoInmueble")),
                
                // Mapeo de objetos anidados/relacionados:
                Propietario = new Propietario
                {
                    IdPropietario = reader.GetInt32(reader.GetOrdinal("IdPropietario")),
                    Nombre = reader.GetString(reader.GetOrdinal("PropNombre")),
                    Apellido = reader.GetString(reader.GetOrdinal("PropApellido"))
                },
                TipoInmueble = new TipoInmueble
                {
                    IdTipoInmueble = reader.GetInt32(reader.GetOrdinal("IdTipoInmueble")),
                    Descripcion = reader.GetString(reader.GetOrdinal("TipoDescripcion"))
                }
            };
        }



 public IList<Inmueble> ObtenerInactivos(int pagina = 1, int tamPagina = 10)
        {
           IList<Inmueble> listaInactivos = new List<Inmueble>();

            using (var connection = new MySqlConnection(connectionString))
            {

             string query = $@"
                    SELECT i.IdInmueble, i.Direccion, i.Capacidad, i.Latitud, i.Longitud, i.PrecioAlquiler, i.Estado,
                           i.IdPropietario, p.Nombre AS PropNombre, p.Apellido AS PropApellido,
                           i.IdTipoInmueble, t.Descripcion AS TipoDescripcion
                    FROM inmuebles i
                     JOIN propietarios p ON i.IdPropietario = p.IdPropietario
                     JOIN tipoinmueble t ON i.IdTipoInmueble = t.IdTipoInmueble
                    WHERE i.Estado = 0
                    LIMIT {tamPagina} OFFSET {(pagina - 1) * tamPagina};";

            

                using (var command = new MySqlCommand(query, connection))
                 {
                   command.CommandType = CommandType.Text;
                   connection.Open();

        
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            
                            listaInactivos.Add(MapearInmueble(reader));
                        }
                    }
                }
            }

            return listaInactivos;
        }





 public Inmueble? ObtenerPorId(int id)
        {
            Inmueble? Inmueble = null;

            using (var connection = new MySqlConnection(connectionString))
            {
                string query = @"
            SELECT i.IdInmueble, i.Direccion, i.Capacidad, i.Latitud, i.Longitud, i.PrecioAlquiler, i.Estado,
                   i.IdPropietario, p.Nombre AS PropNombre, p.Apellido AS PropApellido,
                   i.IdTipoInmueble, t.Descripcion AS TipoDescripcion
            FROM inmuebles i
             JOIN propietarios p ON i.IdPropietario = p.IdPropietario
             JOIN tipoinmueble t ON i.IdTipoInmueble = t.IdTipoInmueble
            WHERE i.IdInmueble = @IdInmueble;";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdInmueble", id);
                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                          Inmueble=MapearInmueble(reader);
                            
                        }
                    }
                }
            }

            return Inmueble;
        }



public int ObtenerCantidad(bool? soloActivos = true)
{
    int res = 0;
    using (var connection = new MySqlConnection(connectionString))
    {
        string sql = "SELECT COUNT(IdInmueble) FROM inmuebles";
        
        if (soloActivos.HasValue)
        {
            sql += " WHERE Estado = @estado";
        }

        using (var command = new MySqlCommand(sql, connection))
        {
            if (soloActivos.HasValue)
            {
                command.Parameters.AddWithValue("@estado", soloActivos.Value ? 1 : 0);
            }

            command.CommandType = CommandType.Text;
            connection.Open();
            
            // Para un COUNT único podés usar ExecuteScalar directamente en vez del DataReader:
            res = Convert.ToInt32(command.ExecuteScalar());
        }
    }
    return res;
}
              
   
    }


}