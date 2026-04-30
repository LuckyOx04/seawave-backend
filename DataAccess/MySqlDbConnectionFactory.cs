using System.Data;
using MySqlConnector;

namespace DataAccess;

public class MySqlDbConnectionFactory(string connectionString) : IDbConnectionFactory
{
    public IDbConnection CreateConnection() => new MySqlConnection(connectionString);
}