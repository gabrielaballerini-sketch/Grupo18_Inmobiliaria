using MySql.Data.MySqlClient;

namespace Grupo18_Inmobiliaria.Models
{
    public class RepositorioPagoMySql : RepositorioBase, IRepositorioPago
    {
        public RepositorioPagoMySql(IConfiguration configuration) : base(configuration)
        {
        }

        public int Alta(Pago pago)
        {
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"
                    INSERT INTO pagos (Importe, FechaPago, ConceptoPago, MedioPago, IdReserva, Estado, IdUsuarioCreador)
                    VALUES (@Importe, @FechaPago, @ConceptoPago, @MedioPago, @IdReserva, 1, @IdUsuarioCreador);
                    SELECT LAST_INSERT_ID();";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Importe", pago.Importe);
                    command.Parameters.AddWithValue("@FechaPago", pago.FechaPago);
                    command.Parameters.AddWithValue("@ConceptoPago", (int)pago.ConceptoPago);
                    command.Parameters.AddWithValue("@MedioPago", (int)pago.MedioPago);
                    command.Parameters.AddWithValue("@IdReserva", pago.IdReserva);
                    command.Parameters.AddWithValue("@IdUsuarioCreador", pago.IdUsuarioCreador);

                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                    pago.IdPago = res;
                }
            }
            return res;
        }

        public int Baja(int id)
        {
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = "UPDATE pagos SET Estado = 0 WHERE IdPago = @id;";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }

        public int Modificacion(Pago pago)
        {
            return ModificarConcepto(pago.IdPago, pago.ConceptoPago);
        }

        public int Reactivar(int id)
        {
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = "UPDATE pagos SET Estado = 1, IdUsuarioAnulador = NULL WHERE IdPago = @id;";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }

        public Pago? ObtenerPorId(int id)
        {
            Pago? pago = null;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"
                    SELECT p.IdPago, p.Importe, p.FechaPago, p.ConceptoPago, p.MedioPago, 
                           p.IdReserva, p.Estado, p.IdUsuarioCreador, p.IdUsuarioAnulador,
                           uc.UserName AS CreadorUserName,
                           ua.UserName AS AnuladorUserName
                    FROM pagos p
                    LEFT JOIN usuarios uc ON p.IdUsuarioCreador = uc.IdUsuario
                    LEFT JOIN usuarios ua ON p.IdUsuarioAnulador = ua.IdUsuario
                    WHERE p.IdPago = @id;";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            pago = MapearPago(reader);
                        }
                    }
                }
            }
            return pago;
        }

        public IList<Pago> ObtenerActivos(int pagina = 1, int tamPagina = 10)
        {
            return ObtenerPaginados(soloActivos: true, pagina, tamPagina);
        }

        public IList<Pago> ObtenerInactivos(int pagina = 1, int tamPagina = 10)
        {
            return ObtenerPaginados(soloActivos: false, pagina, tamPagina);
        }

        public int ObtenerCantidad(bool? soloActivos = true)
        {
            int cantidad = 0;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = "SELECT COUNT(*) FROM pagos";
                if (soloActivos.HasValue)
                {
                    sql += soloActivos.Value ? " WHERE Estado = 1" : " WHERE Estado = 0";
                }

                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    cantidad = Convert.ToInt32(command.ExecuteScalar());
                }
            }
            return cantidad;
        }

        public IList<Pago> ObtenerPorReserva(int idReserva)
        {
            var lista = new List<Pago>();
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"
                    SELECT p.IdPago, p.Importe, p.FechaPago, p.ConceptoPago, p.MedioPago, 
                           p.IdReserva, p.Estado, p.IdUsuarioCreador, p.IdUsuarioAnulador,
                           uc.UserName AS CreadorUserName,
                           ua.UserName AS AnuladorUserName
                    FROM pagos p
                    LEFT JOIN usuarios uc ON p.IdUsuarioCreador = uc.IdUsuario
                    LEFT JOIN usuarios ua ON p.IdUsuarioAnulador = ua.IdUsuario
                    WHERE p.IdReserva = @idReserva
                    ORDER BY p.FechaPago DESC;";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@idReserva", idReserva);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(MapearPago(reader));
                        }
                    }
                }
            }
            return lista;
        }

        public int ModificarConcepto(int idPago, ConceptoPago nuevoConcepto)
        {
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = "UPDATE pagos SET ConceptoPago = @ConceptoPago WHERE IdPago = @IdPago;";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@ConceptoPago", (int)nuevoConcepto);
                    command.Parameters.AddWithValue("@IdPago", idPago);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }

        public int Anular(int idPago, int idUsuarioAnulador)
        {
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = "UPDATE pagos SET Estado = 0, IdUsuarioAnulador = @IdUsuarioAnulador WHERE IdPago = @IdPago;";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@IdUsuarioAnulador", idUsuarioAnulador);
                    command.Parameters.AddWithValue("@IdPago", idPago);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }

        private IList<Pago> ObtenerPaginados(bool soloActivos, int pagina, int tamPagina)
        {
            var lista = new List<Pago>();
            int offset = (pagina - 1) * tamPagina;

            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"
                    SELECT p.IdPago, p.Importe, p.FechaPago, p.ConceptoPago, p.MedioPago, 
                           p.IdReserva, p.Estado, p.IdUsuarioCreador, p.IdUsuarioAnulador,
                           uc.UserName AS CreadorUserName,
                           ua.UserName AS AnuladorUserName
                    FROM pagos p
                    LEFT JOIN usuarios uc ON p.IdUsuarioCreador = uc.IdUsuario
                    LEFT JOIN usuarios ua ON p.IdUsuarioAnulador = ua.IdUsuario
                    WHERE p.Estado = @estado
                    ORDER BY p.FechaPago DESC
                    LIMIT @tamPagina OFFSET @offset;";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@estado", soloActivos ? 1 : 0);
                    command.Parameters.AddWithValue("@tamPagina", tamPagina);
                    command.Parameters.AddWithValue("@offset", offset);

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(MapearPago(reader));
                        }
                    }
                }
            }
            return lista;
        }

        private static Pago MapearPago(MySqlDataReader reader)
        {
            var pago = new Pago
            {
                IdPago = reader.GetInt32("IdPago"),
                Importe = reader.GetDecimal("Importe"),
                FechaPago = reader.GetDateTime("FechaPago"),
                ConceptoPago = (ConceptoPago)reader.GetInt32("ConceptoPago"),
                MedioPago = (MedioPago)reader.GetInt32("MedioPago"),
                IdReserva = reader.GetInt32("IdReserva"),
                Estado = reader.GetBoolean("Estado"),
                IdUsuarioCreador = reader.GetInt32("IdUsuarioCreador"),
                IdUsuarioAnulador = reader.IsDBNull(reader.GetOrdinal("IdUsuarioAnulador"))
                                    ? null
                                    : reader.GetInt32("IdUsuarioAnulador")
            };

            if (!reader.IsDBNull(reader.GetOrdinal("CreadorUserName")))
            {
                pago.UsuarioCreador = new Usuario
                {
                    IdUsuario = pago.IdUsuarioCreador,
                    UserName = reader.GetString("CreadorUserName")
                };
            }

            if (pago.IdUsuarioAnulador.HasValue && !reader.IsDBNull(reader.GetOrdinal("AnuladorUserName")))
            {
                pago.UsuarioAnulador = new Usuario
                {
                    IdUsuario = pago.IdUsuarioAnulador.Value,
                    UserName = reader.GetString("AnuladorUserName")
                };
            }

            return pago;
        }
    }
}