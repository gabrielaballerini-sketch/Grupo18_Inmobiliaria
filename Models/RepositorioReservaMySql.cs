using Grupo18_Inmobiliaria.Models;
using System.Data;
using MySqlConnector;

namespace Grupo18_Inmobiliaria.Models
{
    public class RepositorioReservaMySql : RepositorioBase, IRepositorioReserva
    {
        public RepositorioReservaMySql(IConfiguration configuration) : base(configuration)
        {

        }


        // ALTA


        public int Alta(Reserva reserva)
        {
            int res = -1;

            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = """
                    INSERT INTO Reservas
                    (
                        MontoDiario,
                        FechaInicio,
                        FechaFin,
                        Estado,
                        IdInquilino,
                        IdInmueble,
                        IdUsuario
                    )
                    VALUES
                    (
                        @MontoDiario,
                        @FechaInicio,
                        @FechaFin,
                        1,
                        @IdInquilino,
                        @IdInmueble,
                        @IdUsuario
                    );

                    SELECT LAST_INSERT_ID();
                """;

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;

                    command.Parameters.AddWithValue("@MontoDiario", reserva.MontoDiario);

                    command.Parameters.AddWithValue("@FechaInicio", reserva.FechaInicio);

                    command.Parameters.AddWithValue("@FechaFin", reserva.FechaFin);



                    command.Parameters.AddWithValue("@IdInquilino", reserva.IdInquilino);

                    command.Parameters.AddWithValue("@IdInmueble", reserva.IdInmueble);

                    command.Parameters.AddWithValue("@IdUsuario", reserva.IdUsuario);

                    connection.Open();





                    res = Convert.ToInt32(command.ExecuteScalar());

                    reserva.IdReserva = res;


                }
            }

            return res;
        }


        // BAJA LOGICA


        public int Baja(int id)
        {
            int res = -1;

            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = """
                    UPDATE Reservas
                    SET Estado = 0
                    WHERE IdReserva = @IdReserva
                """;

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;

                    command.Parameters.AddWithValue("@IdReserva", id);

                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }

            return res;
        }

        public int Modificacion(Reserva reserva)
        {
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"UPDATE reservas 
                       SET FechaInicio = @FechaInicio, 
                           FechaFin = @FechaFin, 
                           IdInmueble = @IdInmueble, 
                           IdInquilino = @IdInquilino 
                       WHERE IdReserva = @IdReserva;";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@FechaInicio", reserva.FechaInicio);
                    command.Parameters.AddWithValue("@FechaFin", reserva.FechaFin);
                    command.Parameters.AddWithValue("@IdInmueble", reserva.IdInmueble);
                    command.Parameters.AddWithValue("@IdInquilino", reserva.IdInquilino);
                    command.Parameters.AddWithValue("@IdReserva", reserva.IdReserva);

                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }





        // REACTIVAR

        public int Reactivar(int id)
        {
            int res = -1;

            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = """
                    UPDATE Reservas
                    SET Estado = 1
                    WHERE IdReserva = @IdReserva;
                """;

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;

                    command.Parameters.AddWithValue("@IdReserva", id);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }

            return res;
        }



        // OBTENER ACTIVOS


        public IList<Reserva> ObtenerActivos(int pagina = 1, int tamPagina = 10)
        {
            IList<Reserva> listaActivos = new List<Reserva>();

            using (var connection = new MySqlConnection(connectionString))
            {
                string query = $"""
                   SELECT r.IdReserva, r.IdUsuario, r.FechaInicio, r.FechaFin, r.MontoDiario, r.Estado,
                    r.Multa, r.FechaCancelacion,
                    r.IdInquilino, 
                    iq.Nombre AS InqNombre, 
                    iq.Apellido AS InqApellido, 
                    iq.Dni AS InqDni, 
                    iq.Telefono AS InqTelefono, 
                     iq.Email AS InqEmail,
                     r.IdInmueble, 
                    inm.Direccion AS InmDireccion, 
                    inm.Capacidad AS InmCapacidad, 
                     inm.PrecioAlquiler AS InmPrecioAlquiler
                    FROM Reservas r
                    JOIN inquilinos iq ON r.IdInquilino = iq.IdInquilino
                    JOIN inmuebles inm ON r.IdInmueble = inm.IdInmueble
                    WHERE r.Estado = 1
                    LIMIT {tamPagina}
                    OFFSET {(pagina - 1) * tamPagina};
                """;

                using (var command = new MySqlCommand(query, connection))

                {
                    command.CommandType = CommandType.Text;

                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            listaActivos.Add(MapearReserva(reader)

                            );
                        }
                    }
                }
            }

            return listaActivos;
        }



        // MAPEAR RESERVA


        private Reserva MapearReserva(MySqlDataReader reader)
        {
            var reserva = new Reserva
            {
                IdReserva = reader.GetInt32(reader.GetOrdinal("IdReserva")),
                MontoDiario = reader.GetDecimal(reader.GetOrdinal("MontoDiario")),
                FechaInicio = reader.GetDateTime(reader.GetOrdinal("FechaInicio")),
                FechaFin = reader.GetDateTime(reader.GetOrdinal("FechaFin")),
                Estado = reader.GetBoolean(reader.GetOrdinal("Estado")),
                IdInquilino = reader.GetInt32(reader.GetOrdinal("IdInquilino")),
                IdInmueble = reader.GetInt32(reader.GetOrdinal("IdInmueble")),
                IdUsuario = reader.GetInt32(reader.GetOrdinal("IdUsuario")),


                Multa = reader.IsDBNull(reader.GetOrdinal("Multa")) 
                ? null 
                : reader.GetDecimal(reader.GetOrdinal("Multa")),
                
                 FechaCancelacion = reader.IsDBNull(reader.GetOrdinal("FechaCancelacion")) 
                          ? null 
                          : reader.GetDateTime(reader.GetOrdinal("FechaCancelacion")),


                Inquilino = new Inquilino
                {
                    IdInquilino = reader.GetInt32(reader.GetOrdinal("IdInquilino")),
                    Nombre = reader.GetString(reader.GetOrdinal("InqNombre")),
                    Apellido = reader.GetString(reader.GetOrdinal("InqApellido")),
                    Dni = reader.IsDBNull(reader.GetOrdinal("InqDni")) ? "" : reader.GetString(reader.GetOrdinal("InqDni")),
                    Telefono = reader.IsDBNull(reader.GetOrdinal("InqTelefono")) ? "" : reader.GetString(reader.GetOrdinal("InqTelefono")),
                    Email = reader.IsDBNull(reader.GetOrdinal("InqEmail")) ? "" : reader.GetString(reader.GetOrdinal("InqEmail"))
                },


                Inmueble = new Inmueble
                {
                    IdInmueble = reader.GetInt32(reader.GetOrdinal("IdInmueble")),
                    Direccion = reader.GetString(reader.GetOrdinal("InmDireccion")),
                    Capacidad = reader.GetInt32(reader.GetOrdinal("InmCapacidad")),
                    PrecioAlquiler = reader.GetDecimal(reader.GetOrdinal("InmPrecioAlquiler"))
                }
            };

            return reserva;
        }



        // OBTENER INACTIVOS


        public IList<Reserva> ObtenerInactivos(
            int pagina = 1,
            int tamPagina = 10)
        {
            IList<Reserva> listaInactivos = new List<Reserva>();

            using (var connection = new MySqlConnection(connectionString))
            {
                string query = $"""
                    SELECT r.IdReserva, r.IdUsuario, r.FechaInicio, r.FechaFin, r.MontoDiario, r.Estado,
                    r.Multa, r.FechaCancelacion,
                    r.IdInquilino, 
                    iq.Nombre AS InqNombre, 
                    iq.Apellido AS InqApellido, 
                    iq.Dni AS InqDni, 
                    iq.Telefono AS InqTelefono, 
                     iq.Email AS InqEmail,
                     r.IdInmueble, 
                    inm.Direccion AS InmDireccion, 
                    inm.Capacidad AS InmCapacidad, 
                     inm.PrecioAlquiler AS InmPrecioAlquiler
                    FROM Reservas r
                     JOIN inquilinos iq ON r.IdInquilino = iq.IdInquilino
                    JOIN inmuebles inm ON r.IdInmueble = inm.IdInmueble
                    WHERE r.Estado = 0
                    LIMIT {tamPagina}
                    OFFSET {(pagina - 1) * tamPagina};
                """;

                using (var command = new MySqlCommand(query, connection))
                {
                    command.CommandType = CommandType.Text;

                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            listaInactivos.Add(MapearReserva(reader));


                        }
                    }
                }
            }

            return listaInactivos;
        }



        // OBTENER POR ID


        public Reserva ObtenerPorId(int id)
        {
            Reserva? reserva = null;

            using (var connection = new MySqlConnection(connectionString))
            {
                string query = """
                    SELECT r.IdReserva, r.IdUsuario, r.FechaInicio, r.FechaFin, r.MontoDiario, r.Estado,
                    r.Multa, r.FechaCancelacion,
                    r.IdInquilino, 
                    iq.Nombre AS InqNombre, 
                    iq.Apellido AS InqApellido, 
                    iq.Dni AS InqDni, 
                    iq.Telefono AS InqTelefono, 
                     iq.Email AS InqEmail,
                     r.IdInmueble, 
                    inm.Direccion AS InmDireccion, 
                    inm.Capacidad AS InmCapacidad, 
                     inm.PrecioAlquiler AS InmPrecioAlquiler
                    FROM Reservas r
                     JOIN inquilinos iq ON r.IdInquilino = iq.IdInquilino
                    JOIN inmuebles inm ON r.IdInmueble = inm.IdInmueble

                    WHERE r.IdReserva = @IdReserva;
                """;

                using (var command = new MySqlCommand(
                    query,
                    connection))
                {
                    command.Parameters.AddWithValue("@IdReserva", id);

                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            reserva = MapearReserva(reader);
                        }
                    }
                }
            }

            return reserva;
        }



        // OBTENER CANTIDAD


        public int ObtenerCantidad(bool? soloActivos = true)
        {
            int res = 0;

            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = """
                    SELECT COUNT(IdReserva)
                    FROM Reservas
                """;

                if (soloActivos.HasValue)
                {
                    sql += " WHERE Estado = @Estado";
                }

                using (var command = new MySqlCommand(sql, connection))


                {
                    if (soloActivos.HasValue)
                    {
                        command.Parameters.AddWithValue("@Estado", soloActivos.Value ? 1 : 0);

                    }

                    command.CommandType = CommandType.Text;

                    connection.Open();

                    res = Convert.ToInt32(command.ExecuteScalar());


                }
            }

            return res;
        }

        // VERIFICAR SI EXISTE UNA RESERVA EN UN PERIODO

        public bool ExisteReservaEnFechas(int idInmueble, DateTime fechaInicio, DateTime fechaFin, int? idReservaExcluir = null)
        {
            bool existe = false;

            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = """
            SELECT COUNT(*)
            FROM Reservas
            WHERE IdInmueble = @IdInmueble
            AND Estado = 1
            AND FechaInicio < @FechaFin
            AND FechaFin > @FechaInicio
        """;

                // Si estamos editando una reserva,
                // excluimos la propia reserva de la búsqueda.
                if (idReservaExcluir.HasValue)
                {
                    sql += " AND IdReserva <> @IdReservaExcluir";
                }

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;

                    command.Parameters.AddWithValue("@IdInmueble", idInmueble);

                    command.Parameters.AddWithValue("@FechaInicio", fechaInicio);

                    command.Parameters.AddWithValue("@FechaFin", fechaFin);

                    if (idReservaExcluir.HasValue)
                    {
                        command.Parameters.AddWithValue("@IdReservaExcluir", idReservaExcluir.Value);
                    }

                    connection.Open();

                    int cantidad = Convert.ToInt32(command.ExecuteScalar());

                    existe = cantidad > 0;
                }
            }

            return existe;
        }


public int FinalizarAnticipadamente(int idReserva, decimal multa, DateTime fechaCancelacion)
{
    int res = -1;
    using (var connection = new MySqlConnection(connectionString))
    {
        string sql = @"
            UPDATE Reservas
            SET Multa = @Multa,
                FechaCancelacion = @FechaCancelacion,
                Estado = 0
            WHERE IdReserva = @IdReserva;";

        using (var command = new MySqlCommand(sql, connection))
        {
            command.Parameters.AddWithValue("@Multa", multa);
            command.Parameters.AddWithValue("@FechaCancelacion", fechaCancelacion);
            command.Parameters.AddWithValue("@IdReserva", idReserva);

            connection.Open();
            res = command.ExecuteNonQuery();
        }
    }
    return res;
}











    }
}


