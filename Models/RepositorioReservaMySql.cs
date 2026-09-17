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

        // MODIFICACION
        public int Modificacion(Reserva reserva)
        {
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = """
                    UPDATE Reservas 
                    SET FechaInicio = @FechaInicio, 
                        FechaFin = @FechaFin, 
                        IdInmueble = @IdInmueble, 
                        IdInquilino = @IdInquilino 
                    WHERE IdReserva = @IdReserva;
                """;

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

        // OBTENER ACTIVOS Y VIGENTES
        public IList<Reserva> ObtenerActivos(int pagina = 1, int tamPagina = 10)
        {
            IList<Reserva> listaActivos = new List<Reserva>();

            using (var connection = new MySqlConnection(connectionString))
            {
                string query = """
                    SELECT r.IdReserva, r.IdUsuario, r.FechaInicio, r.FechaFin, r.MontoDiario, r.Estado,
                           r.Multa, r.FechaCancelacion, r.IdUsuarioCancelacion, u.UserName,
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
                    LEFT JOIN usuarios u ON r.IdUsuarioCancelacion = u.IdUsuario
                    WHERE r.Estado = 1 AND r.FechaFin >= NOW()
                    LIMIT @TamPagina OFFSET @Offset;
                """;

                using (var command = new MySqlCommand(query, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@TamPagina", tamPagina);
                    command.Parameters.AddWithValue("@Offset", (pagina - 1) * tamPagina);

                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            listaActivos.Add(MapearReserva(reader));
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

                UsuarioCancelacion = reader.IsDBNull(reader.GetOrdinal("IdUsuarioCancelacion"))
                    ? null
                    : new Usuario
                    {
                        IdUsuario = reader.GetInt32(reader.GetOrdinal("IdUsuarioCancelacion")),
                        UserName = reader.IsDBNull(reader.GetOrdinal("UserName")) ? "" : reader.GetString(reader.GetOrdinal("UserName"))
                    },

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
        public IList<Reserva> ObtenerInactivos(int pagina = 1, int tamPagina = 10)
        {
            IList<Reserva> listaInactivos = new List<Reserva>();

            using (var connection = new MySqlConnection(connectionString))
            {
                string query = """
                    SELECT r.IdReserva, r.IdUsuario, r.FechaInicio, r.FechaFin, r.MontoDiario, r.Estado,
                           r.Multa, r.FechaCancelacion, r.IdUsuarioCancelacion, u.UserName,
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
                    LEFT JOIN usuarios u ON r.IdUsuarioCancelacion = u.IdUsuario
                    WHERE r.Estado = 0
                    LIMIT @TamPagina OFFSET @Offset;
                """;

                using (var command = new MySqlCommand(query, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@TamPagina", tamPagina);
                    command.Parameters.AddWithValue("@Offset", (pagina - 1) * tamPagina);

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
                           r.Multa, r.FechaCancelacion, r.IdUsuarioCancelacion, u.UserName,
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
                    LEFT JOIN usuarios u ON r.IdUsuarioCancelacion = u.IdUsuario
                    WHERE r.IdReserva = @IdReserva;
                """;

                using (var command = new MySqlCommand(query, connection))
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
                bool esActivo = soloActivos ?? true;
                string condicionFecha = esActivo ? "FechaFin >= NOW()" : "FechaFin < NOW()";

                string sql = $"""
                    SELECT COUNT(IdReserva)
                    FROM Reservas
                    WHERE Estado = 1 AND {condicionFecha};
                """;

                using (var command = new MySqlCommand(sql, connection))
                {
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

                if (idReservaExcluir.HasValue)
                {
                    sql += " AND IdReserva <> @IdReservaExcluir;";
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

        // FINALIZAR ANTICIPADAMENTE
        public int FinalizarAnticipadamente(int idReserva, decimal multa, DateTime fechaCancelacion, int idUsuarioCancelacion)
        {
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = """
                    UPDATE Reservas
                    SET Multa = @Multa,
                        FechaCancelacion = @FechaCancelacion,
                        IdUsuarioCancelacion = @IdUsuarioCancelacion,
                        Estado = 0
                    WHERE IdReserva = @IdReserva;
                """;

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Multa", multa);
                    command.Parameters.AddWithValue("@FechaCancelacion", fechaCancelacion);
                    command.Parameters.AddWithValue("@IdUsuarioCancelacion", idUsuarioCancelacion);
                    command.Parameters.AddWithValue("@IdReserva", idReserva);

                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }

        // OBTENER FINALIZADAS
        public IList<Reserva> ObtenerFinalizadas(int pagina = 1, int tamPagina = 10)
        {
            IList<Reserva> listaFinalizadas = new List<Reserva>();

            using (var connection = new MySqlConnection(connectionString))
            {
                string query = """
                    SELECT r.IdReserva, r.IdUsuario, r.FechaInicio, r.FechaFin, r.MontoDiario, r.Estado,
                           r.Multa, r.FechaCancelacion, r.IdUsuarioCancelacion, u.UserName,
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
                    LEFT JOIN usuarios u ON r.IdUsuarioCancelacion = u.IdUsuario
                    WHERE r.Estado = 1 AND r.FechaFin < NOW()
                    LIMIT @TamPagina OFFSET @Offset;
                """;

                using (var command = new MySqlCommand(query, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@TamPagina", tamPagina);
                    command.Parameters.AddWithValue("@Offset", (pagina - 1) * tamPagina);

                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            listaFinalizadas.Add(MapearReserva(reader));
                        }
                    }
                }
            }

            return listaFinalizadas;
        }

      public IList<Inmueble> ObtenerInmueblesMasReservados365Dias(int pagina = 1, int tamPagina = 10)
{
    IList<Inmueble> lista = new List<Inmueble>();

    using (var connection = new MySqlConnection(connectionString))
    {
        string sql = """
            SELECT 
                inm.IdInmueble,
                inm.Direccion,
                inm.Capacidad,
                inm.PrecioAlquiler,
                COUNT(r.IdReserva) AS CantidadReservas
            FROM Reservas r
            INNER JOIN inmuebles inm 
                ON r.IdInmueble = inm.IdInmueble
            WHERE r.FechaInicio >= DATE_SUB(NOW(), INTERVAL 365 DAY)
            GROUP BY 
                inm.IdInmueble,
                inm.Direccion,
                inm.Capacidad,
                inm.PrecioAlquiler
            ORDER BY CantidadReservas DESC;
        """;

        using (var command = new MySqlCommand(sql, connection))
        {
            command.CommandType = CommandType.Text;

            connection.Open();

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    lista.Add(new Inmueble
                    {
                        IdInmueble = reader.GetInt32(
                            reader.GetOrdinal("IdInmueble")),

                        Direccion = reader.GetString(
                            reader.GetOrdinal("Direccion")),

                        Capacidad = reader.GetInt32(
                            reader.GetOrdinal("Capacidad")),

                        PrecioAlquiler = reader.GetDecimal(
                            reader.GetOrdinal("PrecioAlquiler")),
                        CantidadReservas = reader.GetInt32(reader.GetOrdinal("CantidadReservas"))
                    });
                }
            }
        }
    }

    return lista;
}

    }
}