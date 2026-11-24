using Avalonia.Controls;
using Avalonia.Interactivity;

namespace DatabasesLab;

/// <summary>
/// The lost and found desk. The window asks the database and shows what it says.
/// It never keeps its own copy of the items.
/// </summary>
public partial class MainWindow : Window
{
    private readonly LostAndFoundDb db;

    /// <summary>
    /// Opens the desk on lostandfound.db, next to the program. (Given)
    /// </summary>
    public MainWindow() : this("lostandfound.db")
    {
    }

    /// <summary>
    /// Opens the desk on any database file. The lab check uses this one. (Given, plus Part C)
    /// </summary>
    /// <param name="databasePath">The path to the SQLite database file.</param>
    public MainWindow(string databasePath)
    {
        InitializeComponent();
        db = new LostAndFoundDb(databasePath);

        db.CreateTable();
        ShowItems();
    }

    // ---------------------------------------------------------------
    // Part C: show it
    // ---------------------------------------------------------------

    /// <summary>
    /// Refreshes the list and count using the items stored in the database.
    /// </summary>
    private void ShowItems()
    {
        ItemsList.Items.Clear();

        foreach (FoundItem item in db.All())
        {
            ItemsList.Items.Add(item);
        }

        CountText.Text = $"Waiting to be claimed: {db.Count()}";
    }

    /// <summary>
    /// Adds a found item, clears the input boxes, and refreshes the list.
    /// </summary>
    /// <param name="sender">The button that raised the event.</param>
    /// <param name="e">The button click event arguments.</param>
    private void AddButton_Click(object? sender, RoutedEventArgs e)
    {
        try
        {
            FoundItem item = db.Add(DescriptionBox.Text ?? "", LocationBox.Text ?? "");

            DescriptionBox.Text = "";
            LocationBox.Text = "";
            StatusText.Text = $"Added: {item}";

            ShowItems();
        }
        catch (ArgumentException ex)
        {
            StatusText.Text = ex.Message;
        }
    }

    /// <summary>
    /// Claims the selected item and refreshes the list.
    /// </summary>
    /// <param name="sender">The button that raised the event.</param>
    /// <param name="e">The button click event arguments.</param>
    private void ClaimButton_Click(object? sender, RoutedEventArgs e)
    {
        FoundItem? item = ItemsList.SelectedItem as FoundItem;

        if (item == null)
        {
            StatusText.Text = "Select an item to claim";
            return;
        }

        if (db.Claim(item.Id))
        {
            StatusText.Text = $"Claimed: {item}";
        }
        else
        {
            StatusText.Text = "That item was already claimed.";
        }

        ShowItems();
    }

    // TODO Exercise 3: show only the items that match the search box.
    private void SearchBox_TextChanged(object? sender, TextChangedEventArgs e)
    {
        throw new NotImplementedException("TODO Exercise 3: SearchBox_TextChanged");
    }
}
