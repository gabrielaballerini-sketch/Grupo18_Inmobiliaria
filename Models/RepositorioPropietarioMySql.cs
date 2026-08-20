using System.Data;
using Grupo18_Inmobiliaria.Models;
using Microsoft.Extensions.Configuration;
using MySqlConnector;

namespace Grupo18_Inmobiliaria.Models
{
    public class RepositorioPropietarioMySql : RepositorioBase
    {
        public RepositorioPropietarioMySql(IConfiguration configuration) : base(configuration)
        {
        }

        // ALTA
        public int Alta(Propietario p)
        {
            int res = -1;

            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"
                    INSERT INTO Propietarios
                    (Nombre, Apellido, Dni, Telefono, Email, Estado)
                    VALUES
                    (@nombre, @apellido, @dni, @telefono, @email, 1);

                    SELECT LAST_INSERT_ID();
                ";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;

                    command.Parameters.AddWithValue("@nombre", p.Nombre);
                    command.Parameters.AddWithValue("@apellido", p.Apellido);
                    command.Parameters.AddWithValue("@dni", p.Dni);
                    command.Parameters.AddWithValue("@telefono", p.Telefono ?? string.Empty);
                    command.Parameters.AddWithValue("@email", p.Email);

                    connection.Open();

                    res = Convert.ToInt32(command.ExecuteScalar());
                    p.IdPropietario = res;
                }
            }

            return res;
        }

        // BAJA LÓGICA
        public int Baja(int id)
        {
            int res = -1;

            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"
                    UPDATE Propietarios 
                    SET Estado = false
                    WHERE IdPropietario = @id
                ";

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

        // MODIFICACIÓN
        public int Modificacion(Propietario p)
        {
            int res = -1;

            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"
                    UPDATE Propietarios
                    SET
                        Nombre = @nombre,
                        Apellido = @apellido,
                        Dni = @dni,
                        Telefono = @telefono,
                        Email = @email
                    WHERE IdPropietario = @id
                ";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;

                    command.Parameters.AddWithValue("@nombre", p.Nombre);
                    command.Parameters.AddWithValue("@apellido", p.Apellido);
                    command.Parameters.AddWithValue("@dni", p.Dni);
                    command.Parameters.AddWithValue("@telefono", p.Telefono ?? string.Empty);
                    command.Parameters.AddWithValue("@email", p.Email);
                    command.Parameters.AddWithValue("@id", p.IdPropietario);

                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }

            return res;
        }

        // OBTENER TODOS
        public List<Propietario> ObtenerTodos()
        {
            var propietarios = new List<Propietario>();

            using (var connection = new MySqlConnection(connectionString))
            {
                string query = @"SELECT IdPropietario, Nombre, Apellido, Dni, Telefono, Email, Estado 
                                FROM Propietarios 
                                WHERE Estado = 1;";

                using (var command = new MySqlCommand(query, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var propietario = new Propietario
                            {
                                IdPropietario = reader.GetInt32(reader.GetOrdinal(nameof(Propietario.IdPropietario))),
                                Nombre = reader.GetString(reader.GetOrdinal(nameof(Propietario.Nombre))),
                                Apellido = reader.GetString(reader.GetOrdinal(nameof(Propietario.Apellido))),
                                Dni = reader.GetString(reader.GetOrdinal(nameof(Propietario.Dni))),
                                Telefono = reader.IsDBNull(reader.GetOrdinal(nameof(Propietario.Telefono)))
                                    ? string.Empty
                                    : reader.GetString(reader.GetOrdinal(nameof(Propietario.Telefono))),
                                Email = reader.GetString(reader.GetOrdinal(nameof(Propietario.Email))),
                                Estado = reader.GetBoolean(reader.GetOrdinal(nameof(Propietario.Estado)))
                            };
                            propietarios.Add(propietario);
                        }
                    }
                }
            }

            return propietarios;
        }

        // OBTENER POR ID (AHORA DENTRO DE LA CLASE)
        public Propietario? ObtenerPorId(int id)
        {
            Propietario? propietario = null;

            using (var connection = new MySqlConnection(connectionString))
            {
                string query = @"SELECT IdPropietario, Nombre, Apellido, Dni, Telefono, Email, Estado 
                                FROM Propietarios 
                                WHERE IdPropietario = @id AND Estado = 1;";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            propietario = new Propietario
                            {
                                IdPropietario = reader.GetInt32(reader.GetOrdinal(nameof(Propietario.IdPropietario))),
                                Nombre = reader.GetString(reader.GetOrdinal(nameof(Propietario.Nombre))),
                                Apellido = reader.GetString(reader.GetOrdinal(nameof(Propietario.Apellido))),
                                Dni = reader.GetString(reader.GetOrdinal(nameof(Propietario.Dni))),
                                Telefono = reader.IsDBNull(reader.GetOrdinal(nameof(Propietario.Telefono)))
                                    ? string.Empty
                                    : reader.GetString(reader.GetOrdinal(nameof(Propietario.Telefono))),
                                Email = reader.GetString(reader.GetOrdinal(nameof(Propietario.Email))),
                                Estado = reader.GetBoolean(reader.GetOrdinal(nameof(Propietario.Estado)))
                            };
                        }
                    }
                }
            }

            return propietario;
        }
    }
}