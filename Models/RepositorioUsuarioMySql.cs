using Grupo18_Inmobiliaria.Models;
using  System.Data;
using MySqlConnector;
using System.Reflection.Metadata.Ecma335;

namespace Grupo18_Inmobiliaria.Models{
public class RepositorioUsuarioMySql : RepositorioBase{

    public RepositorioUsuarioMySql(IConfiguration configuration): base(configuration)
        {
            
        }


public int Alta(Usuario usuario)
        {
            int res= -1;

            using var connection=new MySqlConnection(connectionString);

            connection.Open();
            string sql="""
            INSERT INTO Usuarios(UserName,Password,RolUsuario,Estado)
            VALUES(@UserName,@Password,@RolUsuario,@Estado);

            SELECT LAST_INSERT_ID();
            """;
            using var command=new MySqlCommand(sql,connection);
            command.CommandType=CommandType.Text;

            command.Parameters.AddWithValue("@UserName",usuario.UserName);
            command.Parameters.AddWithValue("@Password",usuario.Password);
            command.Parameters.AddWithValue("@RolUsuario",usuario.RolUsuario);
            command.Parameters.AddWithValue("@Estado",usuario.Estado);

            res=Convert.ToInt32(command.ExecuteScalar());
            
            usuario.IdUsuario=res;

            return res;
            }
            public int Baja (int id)
        {
            int res =-1;
            using var connection=new MySqlConnection(connectionString);
            connection.Open();
            string sql="""
            UPDATE Usuarios
            SET Estado= false
            WHERE IdUsuario= @IdUsuario
            """;
            using var command=new MySqlCommand(sql,connection);
            command.CommandType=CommandType.Text;

            command.Parameters.AddWithValue("@IdUsuario",id);

            res=command.ExecuteNonQuery();
            return res;
        }


            
}
};
