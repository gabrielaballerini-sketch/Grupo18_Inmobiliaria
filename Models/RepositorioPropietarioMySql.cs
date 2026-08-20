using System.Data;
using Grupo18_Inmobiliaria.Models;
using Microsoft.Extensions.Configuration;
using MySqlConnector;

namespace Grupo18_Inmobiliaria.Models
{
    public class RepositorioPropietarioMySql
        : RepositorioBase
    {
        public RepositorioPropietarioMySql(
            IConfiguration configuration
        ) : base(configuration)
        {
        }

        // ALTA
        public int Alta(Propietario p)
        {
            int res = -1;

            using (var connection =
                   new MySqlConnection(connectionString))
            {
                string sql = @"
                    INSERT INTO Propietarios
                    (Nombre, Apellido, Dni, Telefono, Email)
                    VALUES
                    (@nombre, @apellido, @dni, @telefono, @email);

                    SELECT LAST_INSERT_ID();
                ";

                using (var command =
                       new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;

                    command.Parameters.AddWithValue(
                        "@nombre", p.Nombre);

                    command.Parameters.AddWithValue(
                        "@apellido", p.Apellido);

                    command.Parameters.AddWithValue(
                        "@dni", p.Dni);

                    command.Parameters.AddWithValue(
                        "@telefono", p.Telefono);

                    command.Parameters.AddWithValue(
                        "@email", p.Email);

                    connection.Open();

                    res = Convert.ToInt32(
                        command.ExecuteScalar());

                    p.IdPropietario = res;
                }
            }

            return res;
        }


        // BAJA
        public int Baja(int id)
        {
            int res = -1;

            using (var connection =
                   new MySqlConnection(connectionString))
            {
                string sql = @"
                    DELETE FROM Propietarios
                    WHERE IdPropietario = @id
                ";

                using (var command =
                       new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;

                    command.Parameters.AddWithValue(
                        "@id", id);

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

            using (var connection =
                   new MySqlConnection(connectionString))
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

                using (var command =
                       new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;

                    command.Parameters.AddWithValue(
                        "@nombre", p.Nombre);

                    command.Parameters.AddWithValue(
                        "@apellido", p.Apellido);

                    command.Parameters.AddWithValue(
                        "@dni", p.Dni);

                    command.Parameters.AddWithValue(
                        "@telefono", p.Telefono);

                    command.Parameters.AddWithValue(
                        "@email", p.Email);

                    command.Parameters.AddWithValue(
                        "@id", p.IdPropietario);

                    connection.Open();

                    res = command.ExecuteNonQuery();
                }
            }

            return res;
        }

  }
            }

       
