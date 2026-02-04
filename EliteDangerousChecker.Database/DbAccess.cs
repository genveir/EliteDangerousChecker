using Microsoft.Data.SqlClient;

namespace EliteDangerousChecker.Database;
internal static class DbAccess
{
    private const string ConnectionString = "Data Source=PEACE\\LOVELINESS;Initial Catalog=Elite;Integrated Security=True;TrustServerCertificate=True;";

    public static SqlConnection GetOpenConnection()
    {
        var connection = new SqlConnection(ConnectionString);
        connection.Open();
        return connection;
    }
}
