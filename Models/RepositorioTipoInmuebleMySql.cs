using Grupo18_Inmobiliaria.Models;
using System.Data;
using MySqlConnector;

namespace Grupo18_Inmobiliaria.Models
{

    public class RepositorioTipoInmuebleMySql : RepositorioBase
    {

        public RepositorioTipoInmuebleMySql(IConfiguration configuration) : base(configuration)
        {
        }
        public int Alta(TipoInmueble tipo)
        {
            int res = -1;

            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"INSERT INTO tipoinmueble (Descripcion, Estado)
                VALUES (@descripcion, 1);
                SELECT LAST_INSERT_ID();";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@descripcion", tipo.Descripcion);

                    connection.Open();

                    res = Convert.ToInt32(command.ExecuteScalar());
                    tipo.IdTipoInmueble = res;
                }
            }
            return res;
        }

        public int Baja(int id)
        {
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"UPDATE tipoinmueble SET Estado = 0 WHERE idTipoInmueble = @id;";

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

        public int Modificacion(TipoInmueble tipo)
        {
            int res = -1;

            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"UPDATE tipoinmueble 
                SET Descripcion= @descripcion
                WHERE idTipoInmueble = @id;";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@descripcion", tipo.Descripcion);
                    command.Parameters.AddWithValue("@id", tipo.IdTipoInmueble);

                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }
    }
}