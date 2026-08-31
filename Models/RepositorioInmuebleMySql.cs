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
            int res = -1;

            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = """
                INSERT Into Inmuebles(Direccion,Capacidad,PrecioAlquiler,IdPropietario,Estado,IdTipoInmueble,Latitud,Longitud)
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
                string sql = @" UPDATE inmuebles  SET estado = 0 WHERE IdInmueble = @id";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@id", id);

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
                WHERE IdInmueble = @id;";

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
                    command.Parameters.AddWithValue("@id", inmueble.IdInmueble);

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
                string sql = @"UPDATE inmuebles SET estado = 1 WHERE IdInmueble = @id;";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@id", id);

                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }
    }
}