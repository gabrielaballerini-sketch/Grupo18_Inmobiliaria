using Grupo18_Inmobiliaria.Models;
using System.Data;
using MySqlConnector;


namespace Grupo18_Inmobiliaria.Models
{
    public class RepositorioUsuarioMySql : RepositorioBase, IRepositorio<Usuario>
    {

        public RepositorioUsuarioMySql(IConfiguration configuration) : base(configuration)
        {

        }


        public int Alta(Usuario usuario)
        {
            int res = -1;

            using var connection = new MySqlConnection(connectionString);

            string sql = """
            INSERT INTO Usuarios(UserName,Password,RolUsuario,Estado)
            VALUES(@UserName,@Password,@RolUsuario,@Estado);

            SELECT LAST_INSERT_ID();
            """;
            using var command = new MySqlCommand(sql, connection);
            command.CommandType = CommandType.Text;

            command.Parameters.AddWithValue("@UserName", usuario.UserName);
            command.Parameters.AddWithValue("@Password", usuario.Password);
            command.Parameters.AddWithValue("@RolUsuario", (int)usuario.RolUsuario);
            command.Parameters.AddWithValue("@Estado", usuario.Estado);

            connection.Open();

            res = Convert.ToInt32(command.ExecuteScalar());

            usuario.IdUsuario = res;

            return res;
        }
        public int Baja(int id)
        {
            int res = -1;
            using var connection = new MySqlConnection(connectionString);

            string sql = """
            UPDATE Usuarios
            SET Estado= false
            WHERE IdUsuario= @IdUsuario
            """;
            using var command = new MySqlCommand(sql, connection);
            command.CommandType = CommandType.Text;

            command.Parameters.AddWithValue("@IdUsuario", id);

            connection.Open();
            res = command.ExecuteNonQuery();
            return res;
        }

        public int Modificacion(Usuario usuario)
        {
            int res = -1;
            using var connection = new MySqlConnection(connectionString);

            string sql = """
                UPDATE Usuarios
                SET UserName = @UserName,
                    Password = @Password,
                    RolUsuario = @RolUsuario,
                    Estado = @Estado
                WHERE IdUsuario = @IdUsuario;
                """;
            using var command = new MySqlCommand(sql, connection);
            command.CommandType = CommandType.Text;

            command.Parameters.AddWithValue("@UserName", usuario.UserName);
            command.Parameters.AddWithValue("@Password", usuario.Password);
            command.Parameters.AddWithValue("@RolUsuario", (int)usuario.RolUsuario); // Casteo del Enum a int
            command.Parameters.AddWithValue("@Estado", usuario.Estado);
            command.Parameters.AddWithValue("@IdUsuario", usuario.IdUsuario);

            connection.Open();
            res = command.ExecuteNonQuery();
            return res;
        }

        public int Reactivar(int id)
        {
            int res = -1;
            using var connection = new MySqlConnection(connectionString);

            string sql = """
                UPDATE Usuarios
                SET Estado = true
                WHERE IdUsuario = @IdUsuario;
                """;
            using var command = new MySqlCommand(sql, connection);
            command.CommandType = CommandType.Text;

            command.Parameters.AddWithValue("@IdUsuario", id);
            connection.Open();
            res = command.ExecuteNonQuery();
            return res;
        }

        public IList<Usuario> ObtenerActivos(int pagina = 1, int tamPagina = 10)
        {
            IList<Usuario> res = new List<Usuario>();
            using var connection = new MySqlConnection(connectionString);


            string sql = $"""
                     SELECT IdUsuario, UserName, Password, RolUsuario, Estado
                     FROM Usuarios
                     WHERE Estado = 1
                     LIMIT {tamPagina} 
                    OFFSET {(pagina - 1) * tamPagina};
                    """;
            using var command = new MySqlCommand(sql, connection);

            connection.Open();
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                res.Add(MapearUsuario(reader));
            }
            return res;
        }

        public IList<Usuario> ObtenerInactivos(int pagina = 1, int tamPagina = 10)
        {
            IList<Usuario> res = new List<Usuario>();
            using var connection = new MySqlConnection(connectionString);


            string sql = $"""
                 SELECT IdUsuario, UserName, Password, RolUsuario, Estado
                FROM Usuarios
                WHERE Estado = 0
                LIMIT {tamPagina} 
                OFFSET {(pagina - 1) * tamPagina};
             """;
            using var command = new MySqlCommand(sql, connection);

            connection.Open();
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                res.Add(MapearUsuario(reader));
            }
            return res;
        }

        public int ObtenerCantidad(bool? soloActivos = true)
        {
            int res = 0;
            using var connection = new MySqlConnection(connectionString);


            string sql = "SELECT COUNT(*) FROM Usuarios";
            if (soloActivos.HasValue)
            {
                sql += " WHERE Estado = @Estado;";
            }

            using var command = new MySqlCommand(sql, connection);
            if (soloActivos.HasValue)
            {
                command.Parameters.AddWithValue("@Estado", soloActivos.Value);
            }

            connection.Open();
            res = Convert.ToInt32(command.ExecuteScalar());
            return res;
        }

        public Usuario ObtenerPorId(int id)
        {
            Usuario? usuario = null;
            using var connection = new MySqlConnection(connectionString);

            string sql = """
                SELECT IdUsuario, UserName, Password, RolUsuario, Estado
                FROM Usuarios
                WHERE IdUsuario = @IdUsuario;
                """;
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@IdUsuario", id);

            connection.Open();
            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                usuario = MapearUsuario(reader);
            }
            return usuario;
        }

        private Usuario MapearUsuario(MySqlDataReader reader)
        {
            return new Usuario
            {
                IdUsuario = reader.GetInt32("IdUsuario"),
                UserName = reader.GetString("UserName"),
                Password = reader.GetString("Password"),
                RolUsuario = (RoLUsuario)reader.GetInt32("RolUsuario"), // Casteo del int de la BD al Enum
                Estado = reader.GetBoolean("Estado"),
                ListaReservas = new List<Reserva>() // Inicializada vacía 
            };
        }

    }
};
