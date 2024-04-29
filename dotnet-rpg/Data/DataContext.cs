namespace dotnet_rpg.Data;

public class DataContext : DbContext
{
    public DbSet<Character> Characters => Set<Character>();
    
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
        
    }
}