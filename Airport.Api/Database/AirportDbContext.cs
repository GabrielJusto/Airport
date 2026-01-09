
using Airport.Api.Models;

using Microsoft.EntityFrameworkCore;

namespace Airport.Api.Database;

public class AirportDbContext(DbContextOptions<AirportDbContext> options) : DbContext(options)
{
    public DbSet<Gate> Gates { get; set; }
    public DbSet<Terminal> Terminals { get; set; }
    public DbSet<Hub> Hubs { get; set; }
    public DbSet<GateEvent> GateEvents { get; set; }
    public DbSet<GateEventType> GateEventTypes { get; set; }

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

        modelBuilder.Entity<GateEventType>()
            .HasData(
                new GateEventType() { GateEventTypeId = 1, Description = "Departure" },
                new GateEventType() { GateEventTypeId = 2, Description = "Arrival" }
            );

        modelBuilder.Entity<GateEvent>()
            .HasOne(e => e.Gate)
            .WithMany(e => e.GateEvents)
            .HasForeignKey(e => e.GateId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<GateEvent>()
            .HasOne(e => e.GateEventType)
            .WithMany()
            .HasForeignKey(e => e.GateEventTypeId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}