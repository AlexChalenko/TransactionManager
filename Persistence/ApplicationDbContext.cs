using Domain.Models;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

namespace Persistence;

public class ApplicationDbContext : DbContext
{
    public DbSet<Transaction> Transactions { get; set; }
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            Batteries_V2.Init();
            optionsBuilder.UseSqlite("Data Source=transactions.db");
        }
    }
}
