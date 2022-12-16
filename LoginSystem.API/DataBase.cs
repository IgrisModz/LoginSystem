using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginSystem.API
{
    internal class DataBase
    {
        private IDbCommand Command { get; set; }
        private static DataBase _instance;
        private static readonly object _lock = new object();
        internal static DataBase Instance
        {
            get
            {
                lock(_lock)
                {
                    if (_instance == null)
                        _instance = new DataBase();
                    return _instance;
                }
            }
        }

        private DataBase()
        {
        }

        internal bool AdminKeyExist(string adminKey)
        {
            bool result = false;
            Command = new SqlCommand("SELECT * FROM LicenseAdmins WHERE License=@adminKey", Connection.Instance);
            Command.Parameters.Add(new SqlParameter("@adminKey", SqlDbType.VarChar) { Value = adminKey });
            Connection.Instance.Open();
            IDataReader reader = Command.ExecuteReader();
            while (reader.Read())
            {
                if (reader["AdminID"].ToString() == string.Empty)
                {
                    result = true;
                    break;
                }
            }
            reader.Close();
            Command.Dispose();
            Connection.Instance.Close();
            return result;
        }

        internal bool UserExist(string username)
        {
            bool state;
            Command = new SqlCommand("SELECT * FROM Clients WHERE Username=@username", Connection.Instance);
            Command.Parameters.Add(new SqlParameter("@username", SqlDbType.VarChar) { Value = username });
            Connection.Instance.Open();
            IDataReader reader = Command.ExecuteReader();
            state = reader.Read();
            reader.Close();
            Command.Dispose();
            Connection.Instance.Close();
            return state;
        }

        internal bool EmailExist(string email)
        {
            bool state;
            Command = new SqlCommand("SELECT * FROM Clients WHERE Email=@email", Connection.Instance);
            Command.Parameters.Add(new SqlParameter("@email", SqlDbType.VarChar) { Value = email });
            Connection.Instance.Open();
            IDataReader reader = Command.ExecuteReader();
            state = reader.Read();
            reader.Close();
            Command.Dispose();
            Connection.Instance.Close();
            return state;
        }

        internal bool LicenseExist(string license)
        {
            bool result = false;
            SqlCommand command = new SqlCommand("SELECT * FROM LicenseClients WHERE License=@license", Connection.Instance);
            command.Parameters.Add(new SqlParameter("@license", SqlDbType.VarChar) { Value = license });
            Connection.Instance.Open();
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                if (reader["IdClient"].ToString() == string.Empty)
                {
                    result = true;
                    break;
                }
                else
                {
                    result = false;
                }
            }
            reader.Close();
            command.Dispose();
            Connection.Instance.Close();
            return result;
        }

        internal void AddClient(string adminKey, string name, string password, string email, string ipAddress, string hwid, string license)
        {
            Command = new SqlCommand("INSERT INTO Clients (Username, Password, Email, License, HWID, IPAddress, ExpirationDate, Project) OUTPUT INSERTED.ID values(@name, @password, @email, @license, @hwid, @ipAddress, @expirationDate, @adminKey)", Connection.Instance);
            Command.Parameters.Add(new SqlParameter("@name", SqlDbType.VarChar) { Value = name });
            Command.Parameters.Add(new SqlParameter("@password", SqlDbType.VarChar) { Value = password });
            Command.Parameters.Add(new SqlParameter("@email", SqlDbType.VarChar) { Value = email });
            Command.Parameters.Add(new SqlParameter("@ipAddress", SqlDbType.VarChar) { Value = ipAddress });
            Command.Parameters.Add(new SqlParameter("@hwid", SqlDbType.VarChar) { Value = hwid });
            Command.Parameters.Add(new SqlParameter("@license", SqlDbType.VarChar) { Value = license });
            Command.Parameters.Add(new SqlParameter("@expirationDate", SqlDbType.DateTime) { Value = DateTime.MaxValue });
            Command.Parameters.Add(new SqlParameter("@adminKey", SqlDbType.VarChar) { Value = adminKey });
            Connection.Instance.Open();
            int id = Convert.ToInt32(Command.ExecuteScalar());
            Command.Dispose();
            Command = new SqlCommand("UPDATE LicenseClients SET IdClient=@id WHERE License=@license", Connection.Instance);
            Command.Parameters.Add(new SqlParameter("@id", SqlDbType.Int) { Value = id });
            Command.Parameters.Add(new SqlParameter("@license", SqlDbType.VarChar) { Value = license });
            Command.ExecuteNonQuery();
            Command.Dispose();
            Connection.Instance.Close();
        }
    }
}
