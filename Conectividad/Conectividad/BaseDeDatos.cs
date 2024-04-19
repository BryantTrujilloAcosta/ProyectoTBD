using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace Conectividad
{
    class BaseDeDatos
    {
        private string connectionString;
        private SqlConnection connection;
        public BaseDeDatos(string servidor, string baseDeDatos, string usuario, string contraseña)
        {
            this.connectionString = $"Data Source={servidor};Initial Catalog={baseDeDatos};User ID={usuario};Password={contraseña};";
            this.connection = new SqlConnection(connectionString);
        }

        public bool TestConnection()
        {
            try
            {
                connection.Open();
                connection.Close();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al conectar a la base de datos:" + ex.Message);
                return false;
            }
        }
        public SqlConnection GetConnection()
        {
            if (TestConnection())
            {
                return connection;
            }
            else
            {
                return null;
            }
        }
        public void CloseConnection()
        {
            if (connection.State == ConnectionState.Open)
            {
                connection.Close();
            }
        }

        public DataTable Innerjoin()
        {

            DataTable dataTable = new DataTable();
            try
            {

                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }

                using (SqlCommand cmd = new SqlCommand("SELECT artid, artnombre, artdescripcion, artprecio, famnombre FROM articulos a INNER JOIN familias f ON a.famid = f.famid", connection))
                {

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        dataTable.Load(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al consultar datos " + ex.Message);
            }
            finally
            {
                CloseConnection();
            }
            return dataTable;
        }

    }
}
