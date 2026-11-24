using Microsoft.Data.Sqlite;

namespace DatabasesLab;

/// <summary>
/// Saves and loads found items in a SQLite database file.
/// Every method opens its own connection and closes it before it returns.
/// </summary>
public class LostAndFoundDb
{
    private readonly string connectionString;

    /// <summary>
    /// Remembers which database file to use. Nothing is opened yet. (Given)
    /// </summary>
    /// <param name="path">The path to the SQLite database file.</param>
    public LostAndFoundDb(string path)
    {
        connectionString = "Data Source=" + path;
    }

    // ---------------------------------------------------------------
    // Part A: save it
    // ---------------------------------------------------------------

    /// <summary>
    /// Opens and returns a connection to the SQLite database.
    /// </summary>
    /// <returns>The open database connection.</returns>
    private SqliteConnection Open()
    {
        SqliteConnection connection = new SqliteConnection(connectionString);
        connection.Open();
        return connection;
    }

    /// <summary>
    /// Creates the items table if it does not already exist.
    /// </summary>
    public void CreateTable()
    {
        using SqliteConnection connection = Open();
        using SqliteCommand command = connection.CreateCommand();

        command.CommandText = """
            CREATE TABLE IF NOT EXISTS items (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                description TEXT NOT NULL,
                location TEXT NOT NULL
            )
            """;

        command.ExecuteNonQuery();
    }

    /// <summary>
    /// Adds a found item to the database and returns it with its assigned ID.
    /// </summary>
    /// <param name="description">Description of the found item.</param>
    /// <param name="location">Location where the item was found.</param>
    /// <returns>The saved item with its database assigned ID.</returns>
    public FoundItem Add(string description, string location)
    {
        FoundItem item = new FoundItem(0, description, location);

        using SqliteConnection connection = Open();
        using SqliteCommand command = connection.CreateCommand();

        command.CommandText = """ 
            INSERT INTO items (description, location)
            VALUES (@description, @location)
            RETURNING id;
            """;

        command.Parameters.AddWithValue("@description", item.Description);
        command.Parameters.AddWithValue("@location", item.Location);

        int id = Convert.ToInt32(command.ExecuteScalar());

        return new FoundItem(id, item.Description, item.Location);
    }

    /// <summary>
    /// Deletes a found item from the database using its ID.
    /// </summary>
    /// <param name="id">The ID of the item to claim.</param>
    /// <returns>True if an item was deleted; otherwise, false.</returns>
    public bool Claim(int id)
    {
        using SqliteConnection connection = Open();
        using SqliteCommand command = connection.CreateCommand();

        command.CommandText = "DELETE FROM items WHERE id = @id";
        command.Parameters.AddWithValue("@id", id);

        int rowsDeleted = command.ExecuteNonQuery();

        return rowsDeleted == 1;
    }

    // ---------------------------------------------------------------
    // Part B: read it back
    // ---------------------------------------------------------------

    /// <summary>
    /// Reads the query results and creates a list of found items.
    /// </summary>
    /// <param name="command">The SQL command used to retrieve the items.</param>
    /// <returns>A list of found items from query results.</returns>
    private static List<FoundItem> ReadItems(SqliteCommand command)
    {
        List<FoundItem> items = new List<FoundItem>();

        using SqliteDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            int id = reader.GetInt32(0);
            string description = reader.GetString(1);
            string location = reader.GetString(2);

            items.Add(new FoundItem(id, description, location));
        }

        return items;
    }

    /// <summary>
    /// Returns all found items from the database, oldest first.
    /// </summary>
    /// <returns>A list of all found items.</returns>
    public List<FoundItem> All()
    {
        using SqliteConnection connection = Open();
        using SqliteCommand command = connection.CreateCommand();

        command.CommandText = "SELECT id, description, location FROM items ORDER BY id";

        return ReadItems(command);
    }

    /// <summary>
    /// Counts the number of items currently stored in the database.
    /// </summary>
    /// <returns>The number of items waiting to be claimed.</returns>
    public int Count()
    {
        using SqliteConnection connection = Open();
        using SqliteCommand command = connection.CreateCommand();

        command.CommandText = "SELECT COUNT(*) FROM items";

        return Convert.ToInt32(command.ExecuteScalar());
    }

    /// <summary>
    /// Searches for found items by description or location, oldest first.
    /// </summary>
    /// <param name="word">The word to search for.</param>
    /// <returns>A list of matching found items.</returns>
    public List<FoundItem> Search(string word)
    {
        using SqliteConnection connection = Open();
        using SqliteCommand command = connection.CreateCommand();

        command.CommandText = """
            SELECT id, description, location FROM items
            WHERE description LIKE @pattern OR location LIKE @pattern
            ORDER BY id
            """;

        command.Parameters.AddWithValue("@pattern", "%" + word.Trim() + "%");

        return ReadItems(command);
    }
}
