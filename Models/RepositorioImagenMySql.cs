using MySql.Data.MySqlClient;
using System.Data;

namespace Grupo18_Inmobiliaria.Models
{
    public class RepositorioImagenMySql : RepositorioBase, IRepositorioImagen
    {
        public RepositorioImagenMySql(IConfiguration configuration) : base(configuration)
        {
        }

        public int Alta(Imagen imagen)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = @"INSERT INTO imagenes (inmueble_id, url) 
                                VALUES (@inmuebleId, @url);
                                SELECT LAST_INSERT_ID();";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@inmuebleId", imagen.IdInmueble);
                    command.Parameters.AddWithValue("@url", imagen.Url);

                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                    imagen.IdImagen = res; // Corregido: se asigna al objeto imagen
                }
            }
            return res;
        }

        public int Eliminar(int id)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = @"DELETE FROM imagenes WHERE id = @id";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }

        public Imagen? ObtenerPorId(int id)
        {
            Imagen? imagen = null;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = @"SELECT id, inmueble_id, url 
                                FROM imagenes 
                                WHERE id = @id";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            imagen = new Imagen
                            {
                                IdImagen = reader.GetInt32("id"),
                                IdInmueble = reader.GetInt32("inmueble_id"),
                                Url = reader.GetString("url")
                            };
                        }
                    }
                }
            }
            return imagen;
        }

        public IList<Imagen> ObtenerPorInmueble(int inmuebleId)
        {
            IList<Imagen> lista = new List<Imagen>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = @"SELECT id, inmueble_id, url 
                                FROM imagenes 
                                WHERE inmueble_id = @inmuebleId";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@inmuebleId", inmuebleId);
                    connection.Open();

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Imagen
                            {
                                IdImagen = reader.GetInt32("id"),
                                IdInmueble = reader.GetInt32("inmueble_id"), // Corregido: IdInmueble
                                Url = reader.GetString("url")
                            });
                        }
                    }
                }
            }
            return lista;
        }
    }
}