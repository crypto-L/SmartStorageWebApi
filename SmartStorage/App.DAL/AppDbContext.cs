using App.Domain;
using Microsoft.EntityFrameworkCore;

namespace App.DAL;

public class AppDbContext : DbContext
{
    public DbSet<Admin> Admins { get; set; } = default!;
    public DbSet<Item> Items { get; set; } = default!;
    public DbSet<Storage> Storages { get; set; } = default!;
    public DbSet<User> Users { get; set; } = default!;

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }
}