using Grupo18_Inmobiliaria.Models;
using System.Data;
using MySqlConnector;

namespace Grupo18_Inmobiliaria.Models
{
    public class RepositorioReservaMySql : RepositorioBase
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





                    res = Convert.ToInt32( command.ExecuteScalar() );
                       
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

        // MODIFICACION
         public int Modificacion(Reserva reserva)
        {
            int res = -1;

            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = """
                    UPDATE Reservas
                    SET
                        MontoDiario = @MontoDiario,
                        FechaInicio = @FechaInicio,
                        FechaFin = @FechaFin,
                        IdInquilino = @IdInquilino,
                        IdInmueble = @IdInmueble,
                        IdUsuario = @IdUsuario
                    WHERE IdReserva = @IdReserva;
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
     

        public IList<Reserva> ObtenerActivos( int pagina = 1, int tamPagina = 10)
        {
            IList<Reserva> listaActivos = new List<Reserva>();

            using (var connection = new MySqlConnection(connectionString))
            {
                string query = $"""
                    SELECT
                        r.IdReserva,
                        r.MontoDiario,
                        r.FechaInicio,
                        r.FechaFin,
                        r.Estado,
                        r.IdInquilino,
                        r.IdInmueble,
                        r.IdUsuario

                    FROM Reservas r

                    WHERE r.Estado = 1

                    LIMIT {tamPagina}
                    OFFSET {(pagina - 1) * tamPagina};
                """;

                using (var command = new MySqlCommand( query, connection))
                   
                {
                    command.CommandType = CommandType.Text;

                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            listaActivos.Add( MapearReserva(reader)
                               
                            );
                        }
                    }
                }
            }

            return listaActivos;
        }


     
        // MAPEAR RESERVA
       

        private Reserva MapearReserva( MySqlDataReader reader)
        {
            return new Reserva
            {
                IdReserva = reader.GetInt32(reader.GetOrdinal("IdReserva")),
                MontoDiario = reader.GetDecimal(reader.GetOrdinal("MontoDiario")),
                FechaInicio = reader.GetDateTime(reader.GetOrdinal("FechaInicio")),

                FechaFin = reader.GetDateTime(reader.GetOrdinal("FechaFin")),

                Estado = reader.GetBoolean(reader.GetOrdinal("Estado")),

                IdInquilino = reader.GetInt32(reader.GetOrdinal("IdInquilino")),

                IdInmueble = reader.GetInt32(reader.GetOrdinal("IdInmueble")),
                IdUsuario = reader.GetInt32(reader.GetOrdinal("IdUsuario"))

            };
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
                    SELECT
                        r.IdReserva,
                        r.MontoDiario,
                        r.FechaInicio,
                        r.FechaFin,
                        r.Estado,
                        r.IdInquilino,
                        r.IdInmueble,
                        r.IdUsuario

                    FROM Reservas r

                    WHERE r.Estado = 0

                    LIMIT {tamPagina}
                    OFFSET {(pagina - 1) * tamPagina};
                """;

                using (var command = new MySqlCommand( query,connection))
                  {
                    command.CommandType = CommandType.Text;

                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            listaInactivos.Add(  MapearReserva(reader));
                              
                            
                        }
                    }
                }
            }

            return listaInactivos;
        }


       
        // OBTENER POR ID
        

        public Reserva? ObtenerPorId(int id)
        {
            Reserva? reserva = null;

            using (var connection = new MySqlConnection(connectionString))
            {
                string query = """
                    SELECT
                        r.IdReserva,
                        r.MontoDiario,
                        r.FechaInicio,
                        r.FechaFin,
                        r.Estado,
                        r.IdInquilino,
                        r.IdInmueble,
                        r.IdUsuario

                    FROM Reservas r

                    WHERE r.IdReserva = @IdReserva;
                """;

                using (var command = new MySqlCommand(
                    query,
                    connection))
                {
                    command.Parameters.AddWithValue( "@IdReserva",  id );
                      
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
     

        public int ObtenerCantidad( bool? soloActivos = true)
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

                using (var command = new MySqlCommand(sql,connection))
                 
                   
                {
                    if (soloActivos.HasValue)
                    {
                        command.Parameters.AddWithValue( "@Estado",  soloActivos.Value ? 1 : 0 );
                           
                     }

                    command.CommandType = CommandType.Text;

                    connection.Open();

                    res = Convert.ToInt32( command.ExecuteScalar() );
                       
                   
                }
            }

            return res;
        }
    
    // VERIFICAR SI EXISTE UNA RESERVA EN UN PERIODO

public bool ExisteReservaEnFechas(int idInmueble,DateTime fechaInicio,DateTime fechaFin,int? idReservaExcluir = null)
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

            command.Parameters.AddWithValue("@IdInmueble",idInmueble);

            command.Parameters.AddWithValue("@FechaInicio",fechaInicio );

            command.Parameters.AddWithValue( "@FechaFin",fechaFin);

            if (idReservaExcluir.HasValue)
            {
                command.Parameters.AddWithValue( "@IdReservaExcluir",idReservaExcluir.Value );
            }

            connection.Open();

            int cantidad =Convert.ToInt32(command.ExecuteScalar());

            existe = cantidad > 0;
        }
    }

    return existe;
}
}
}


