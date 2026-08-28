using System.Data;
using Grupo18_Inmobiliaria.Models;
using Microsoft.Extensions.Configuration;
using MySqlConnector;

namespace Grupo18_Inmobiliaria.Models
{
    public class RepositorioInquilinoMySql : RepositorioBase
    {
        public RepositorioInquilinoMySql(IConfiguration configuration) : base(configuration)
        {
        }

        // ALTA
        public int Alta(Inquilino i)
        {
            int res = -1;

            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"
                    INSERT INTO Inquilinos
                    (Nombre, Apellido, Dni, Telefono, Email, Estado)
                    VALUES
                    (@nombre, @apellido, @dni, @telefono, @email, 1);

                    SELECT LAST_INSERT_ID();
                ";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;

                    command.Parameters.AddWithValue("@nombre", i.Nombre);
                    command.Parameters.AddWithValue("@apellido", i.Apellido);
                    command.Parameters.AddWithValue("@dni", i.Dni);
                    command.Parameters.AddWithValue("@telefono", i.Telefono ?? string.Empty);
                    command.Parameters.AddWithValue("@email", i.Email);

                    connection.Open();

                    res = Convert.ToInt32(command.ExecuteScalar());
                    i.IdInquilino = res;
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
                    UPDATE Inquilinos
                    SET Estado = false
                    WHERE IdInquilino = @id
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
        public int Modificacion(Inquilino i)
        {
            int res = -1;

            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"
                    UPDATE Inquilinos
                    SET
                        Nombre = @nombre,
                        Apellido = @apellido,
                        Dni = @dni,
                        Telefono = @telefono,
                        Email = @email
                    WHERE IdInquilino = @id
                ";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;

                    command.Parameters.AddWithValue("@nombre", i.Nombre);
                    command.Parameters.AddWithValue("@apellido", i.Apellido);
                    command.Parameters.AddWithValue("@dni", i.Dni);
                    command.Parameters.AddWithValue("@telefono", i.Telefono ?? string.Empty);
                    command.Parameters.AddWithValue("@email", i.Email);
                    command.Parameters.AddWithValue("@id", i.IdInquilino);

                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }

            return res;
        }

        // OBTENER TODOS los activos
        public IList<Inquilino> ObtenerActivos(int pagina = 1, int tamPagina = 10)
        {
           IList<Inquilino> inquilinos = new List<Inquilino>();

            using (var connection = new MySqlConnection(connectionString))
            {
                string query = $@"
                    SELECT IdInquilino, Nombre, Apellido, Dni, Telefono, Email, Estado
                    FROM Inquilinos
                    WHERE Estado = 1
                    LIMIT {tamPagina} OFFSET {(pagina - 1) * tamPagina};";
                ;

                using (var command = new MySqlCommand(query, connection))
                {

                      command.CommandType = CommandType.Text;
                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var inquilino = new Inquilino
                            {
                                IdInquilino = reader.GetInt32(
                                    reader.GetOrdinal(nameof(Inquilino.IdInquilino))),

                                Nombre = reader.GetString(
                                    reader.GetOrdinal(nameof(Inquilino.Nombre))),

                                Apellido = reader.GetString(
                                    reader.GetOrdinal(nameof(Inquilino.Apellido))),

                                Dni = reader.GetString(
                                    reader.GetOrdinal(nameof(Inquilino.Dni))),

                                Telefono = reader.IsDBNull(
                                    reader.GetOrdinal(nameof(Inquilino.Telefono)))
                                    ? string.Empty
                                    : reader.GetString(
                                        reader.GetOrdinal(nameof(Inquilino.Telefono))),

                                Email = reader.GetString(
                                    reader.GetOrdinal(nameof(Inquilino.Email))),

                                Estado = reader.GetBoolean(
                                    reader.GetOrdinal(nameof(Inquilino.Estado)))
                            };

                            inquilinos.Add(inquilino);
                        }
                    }
                }
            }

            return inquilinos;
        }

        // OBTENER POR ID
        public Inquilino? ObtenerPorId(int id)
        {
            Inquilino? inquilino = null;

            using (var connection = new MySqlConnection(connectionString))
            {
                string query = @"
                    SELECT IdInquilino, Nombre, Apellido, Dni, Telefono, Email, Estado
                    FROM Inquilinos
                    WHERE IdInquilino = @id AND Estado = 1;
                ";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            inquilino = new Inquilino
                            {
                                IdInquilino = reader.GetInt32(
                                    reader.GetOrdinal(nameof(Inquilino.IdInquilino))),

                                Nombre = reader.GetString(
                                    reader.GetOrdinal(nameof(Inquilino.Nombre))),

                                Apellido = reader.GetString(
                                    reader.GetOrdinal(nameof(Inquilino.Apellido))),

                                Dni = reader.GetString(
                                    reader.GetOrdinal(nameof(Inquilino.Dni))),

                                Telefono = reader.IsDBNull(
                                    reader.GetOrdinal(nameof(Inquilino.Telefono)))
                                    ? string.Empty
                                    : reader.GetString(
                                        reader.GetOrdinal(nameof(Inquilino.Telefono))),

                                Email = reader.GetString(
                                    reader.GetOrdinal(nameof(Inquilino.Email))),

                                Estado = reader.GetBoolean(
                                    reader.GetOrdinal(nameof(Inquilino.Estado)))
                            };
                        }
                    }
                }
            }

            return inquilino;
        }

        public bool ObtenerporDni(string dni)
        {

            using (var connection = new MySqlConnection(connectionString))
            {
                string query = @"SELECT COUNT(*) FROM Inquilinos WHERE Dni = @dni";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@dni", dni);
                    connection.Open();
                    int cantidad = Convert.ToInt32(command.ExecuteScalar());
                    return cantidad > 0;

                }
            }

        }

        // REACTIVACIÓN / ALTA LÓGICA
public int Reactivar(int id)
{
    int res = -1;


    using (var connection = new MySqlConnection(connectionString))
   
  {
        string sql = @"
            UPDATE Inquilinos
            SET Estado = true

            WHERE IdInquilino = @id
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


public IList<Inquilino> ObtenerInactivos(int pagina = 1, int tamPagina = 10)
{
    var inquilinos = new List<Inquilino>();

    using (var connection = new MySqlConnection(connectionString))
    {
        string query = $@"SELECT IdInquilino, Nombre, Apellido, Dni, Telefono, Email, Estado 
                        FROM Inquilinos 
                        WHERE Estado = 0
                         LIMIT {tamPagina} OFFSET {(pagina - 1) * tamPagina};";


        using (var command = new MySqlCommand(query, connection))


        {
             command.CommandType = CommandType.Text;
            connection.Open();
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var inquilino = new Inquilino
                    {
                        IdInquilino = reader.GetInt32(reader.GetOrdinal(nameof(Inquilino.IdInquilino))),
                        Nombre = reader.GetString(reader.GetOrdinal(nameof(Inquilino.Nombre))),
                        Apellido = reader.GetString(reader.GetOrdinal(nameof(Inquilino.Apellido))),
                        Dni = reader.GetString(reader.GetOrdinal(nameof(Inquilino.Dni))),
                        Telefono = reader.IsDBNull(reader.GetOrdinal(nameof(Inquilino.Telefono)))
                            ? string.Empty
                            : reader.GetString(reader.GetOrdinal(nameof(Inquilino.Telefono))),
                        Email = reader.GetString(reader.GetOrdinal(nameof(Inquilino.Email))),
                        Estado = reader.GetBoolean(reader.GetOrdinal(nameof(Inquilino.Estado)))
                    };
                    inquilinos.Add(inquilino);
                }
            }
        }
    }

    return inquilinos;
}


public int ObtenerCantidad(bool? soloActivos = true)
{
    int res = 0;
    using (var connection = new MySqlConnection(connectionString))
    {
        string sql = "SELECT COUNT(IdInquilino) FROM Inquilinos";
        
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