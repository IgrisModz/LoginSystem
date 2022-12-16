using System.Data;
using System.Data.SqlClient;

namespace LoginSystem.API
{
    internal sealed class Connection
    {
        private static IDbConnection _instance;
        private static readonly object _lock = new object();
        internal static SqlConnection Instance
        {
            get
            {
                lock(_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new SqlConnection(@"Data Source=(localdb)\LoginSystem;Integrated Security=True");
                    }
                    return (SqlConnection)_instance;
                }
            }
        }

        private Connection()
        {
        }
    }
}
