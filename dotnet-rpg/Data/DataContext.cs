using dotnet_rpg.Models;

namespace dotnet_rpg.Data;

public class DataContext : DbContext
{
    public DbSet<Character> Characters => Set<Character>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Weapon> Weapons => Set<Weapon>();

    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }
}