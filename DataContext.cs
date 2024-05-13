using Microsoft.EntityFrameworkCore;

namespace ebooks_dotnet7_api;

/// <summary>
/// Represents the data context for the application.
/// </summary>
public class DataContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the DataContext class.
    /// </summary>
    /// <param name="options"></param>
    public DataContext(DbContextOptions<DataContext> options)
        : base(options) { }

    /// <summary>
    /// Represents the eBooks table in the database.
    /// </summary>
    public DbSet<EBook> EBooks { get; set; }
}
