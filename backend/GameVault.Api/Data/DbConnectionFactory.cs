using Npgsql;

namespace GameVault.Api.Data;

/* DbConnectionFactory
 *
 * Takes the connection information from caller (likely Program.cs) to set
 * up connection to the postgreSQL database
 *
 * params:
 * connectionString:    string containing semicolon delimited data for
 *                      Npgsql to create connection 
 */
public class DbConnectionFactory
{
    private readonly string _connectionString;

    public DbConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    public NpgsqlConnection CreateConnection()
    {
        return new NpgsqlConnection(_connectionString);
    }
}
