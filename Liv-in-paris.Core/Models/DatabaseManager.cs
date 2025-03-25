using System.Data;
using System.Data.SQLite;
namespace Liv_in_paris.Core.Models;

public class DatabaseManager
{
    private readonly string connectionString;

    public DatabaseManager(string dbPath)
    {
        connectionString = $"Data Source={dbPath};Version = 3;";
    }
    

    public DataTable ExecuteQuery(string query)
    {
        using var connection = new SQLiteConnection(connectionString);
        connection.Open();
        
        using var command = new SQLiteCommand(query, connection);
        using var adapter = new SQLiteDataAdapter(command);
        
        var table = new DataTable();
        adapter.Fill(table);
        return table;
    }

    public int ExecuteNonQuery(string query)
    {
        using var connection = new SQLiteConnection(connectionString);
        connection.Open();
        
        using var command = new SQLiteCommand(query, connection);
        return command.ExecuteNonQuery();
    }
}