
using Airport.Api.Models;

using Microsoft.EntityFrameworkCore;

namespace Airport.Api.Database;

public class AirportDbContext(DbContextOptions<AirportDbContext> options) : DbContext(options)
{
    public DbSet<Gate> Gates { get; set; }
    public DbSet<Terminal> Terminals { get; set; }
    public DbSet<Hub> Hubs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<Terminal>()
            .HasOne(e => e.Hub)
            .WithMany(e => e.Terminals)
            .HasForeignKey(e => e.HubId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Gate>()
            .HasOne(e => e.Terminal)
            .WithMany(e => e.Gates)
            .HasForeignKey(e => e.TerminalId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}