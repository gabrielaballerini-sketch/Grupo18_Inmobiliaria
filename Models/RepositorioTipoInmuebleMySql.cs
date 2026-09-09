using Grupo18_Inmobiliaria.Models;
using System.Data;
using MySqlConnector;

namespace Grupo18_Inmobiliaria.Models
{

    public class RepositorioTipoInmuebleMySql : RepositorioBase, IRepositorio<TipoInmueble>
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
                VALUES (@Descripcion, 1);
                SELECT LAST_INSERT_ID();";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@Descripcion", tipo.Descripcion);

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
                string sql = @"UPDATE tipoinmueble SET Estado = 0 WHERE IdTipoInmueble = @IdTipoInmueble;";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@IdTipoInmueble", id);

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
                SET Descripcion= @Descripcion
                WHERE IdTipoInmueble = @IdTipoInmueble;";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@Descripcion", tipo.Descripcion);
                    command.Parameters.AddWithValue("@IdTipoInmueble", tipo.IdTipoInmueble);

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
                string sql = @"UPDATE tipoinmueble SET estado = 1 WHERE IdTipoInmueble = @IdTipoInmueble;";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@IdTipoInmueble", id);

                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }






  // OBTENER TODOS los activos
        public IList<TipoInmueble> ObtenerActivos(int pagina = 1, int tamPagina = 10)
        {
           IList<TipoInmueble> listaActivos = new List<TipoInmueble>();

            using (var connection = new MySqlConnection(connectionString))
            {
                string query = $@"SELECT IdTipoInmueble, Descripcion, Estado 
                                FROM TipoInmueble 
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
                            var tipo2 = new TipoInmueble
                            {
                                IdTipoInmueble = reader.GetInt32(reader.GetOrdinal(nameof(TipoInmueble.IdTipoInmueble))),
                                Descripcion = reader.GetString(reader.GetOrdinal(nameof(TipoInmueble.Descripcion))),
                                Estado = reader.GetBoolean(reader.GetOrdinal(nameof(TipoInmueble.Estado))),
                               
                            };
                            listaActivos.Add(tipo2);
                        }
                    }
                }
            }

            return listaActivos;
        }




 public TipoInmueble? ObtenerPorId(int id)
        {
            TipoInmueble? tipoInmueble = null;

            using (var connection = new MySqlConnection(connectionString))
            {
                string query = @$"SELECT IdTipoInmueble, Descripcion, Estado 
                                FROM tipoinmueble 
                                WHERE IdTipoInmueble = @IdTIpoInmueble AND Estado = 1;";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdTIpoInmueble", id);
                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            tipoInmueble = new TipoInmueble
                            {
                                IdTipoInmueble = reader.GetInt32(reader.GetOrdinal(nameof(TipoInmueble.IdTipoInmueble))),
                                Descripcion = reader.GetString(reader.GetOrdinal(nameof(TipoInmueble.Descripcion))),
                                    
                                Estado = reader.GetBoolean(reader.GetOrdinal(nameof(TipoInmueble.Estado)))
                            };
                        }
                    }
                }
            }

            return tipoInmueble;
        }



public IList<TipoInmueble> ObtenerInactivos(int pagina = 1, int tamPagina = 10)
{
    var listaInactivos = new List<TipoInmueble>();

    using (var connection = new MySqlConnection(connectionString))
    {
        string query = $@"SELECT IdTipoInmueble, Descripcion, Estado 
                        FROM tipoinmueble 
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
                    var tipoInmueble2 = new TipoInmueble
                    {
                        IdTipoInmueble = reader.GetInt32(reader.GetOrdinal(nameof(TipoInmueble.IdTipoInmueble))),
                        Descripcion = reader.GetString(reader.GetOrdinal(nameof(TipoInmueble.Descripcion))),
                         Estado = reader.GetBoolean(reader.GetOrdinal(nameof(TipoInmueble.Estado)))
                    };
                    listaInactivos.Add(tipoInmueble2);
                }
            }
        }
    }

    return listaInactivos;
}



public int ObtenerCantidad(bool? soloActivos = true)
{
    int res = 0;
    using (var connection = new MySqlConnection(connectionString))
    {
        string sql = "SELECT COUNT(IdTipoInmueble) FROM tipoinmueble";
        
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



