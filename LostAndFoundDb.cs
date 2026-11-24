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

    // TODO Exercise 1: delete the item with this id. Return true when a row was deleted.
    public bool Claim(int id)
    {
        throw new NotImplementedException("TODO Exercise 1: Claim");
    }

    // ---------------------------------------------------------------
    // Part B: read it back
    // ---------------------------------------------------------------

    // TODO Part B (demo): run the command's SELECT and turn every row into a FoundItem.
    private static List<FoundItem> ReadItems(SqliteCommand command)
    {
        throw new NotImplementedException("TODO Part B: ReadItems");
    }

    // TODO Part B (demo): return every item, oldest first.
    public List<FoundItem> All()
    {
        throw new NotImplementedException("TODO Part B: All");
    }

    // TODO Part B (demo): return how many items are waiting to be claimed.
    public int Count()
    {
        throw new NotImplementedException("TODO Part B: Count");
    }

    // TODO Exercise 2: return items whose description or location contains the word, oldest first.
    public List<FoundItem> Search(string word)
    {
        throw new NotImplementedException("TODO Exercise 2: Search");
    }
}
