using Airport.Api.Database;
using Airport.Api.DTOs;
using Airport.Api.Exceptions;
using Airport.Api.Models;
using Airport.Api.Repositories;
using Airport.Api.Services;
using Microsoft.EntityFrameworkCore;

namespace Airport.Tests.Services;

public class TerminalInsertServiceTests : IDisposable
{
    private readonly AirportDbContext _context;
    private readonly HubRepository _hubRepository;
    private readonly TerminalRepository _terminalRepository;
    private readonly TerminalInsertService _terminalInsertService;

    public TerminalInsertServiceTests()
    {
        DbContextOptions<AirportDbContext> options = new DbContextOptionsBuilder<AirportDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AirportDbContext(options);
        _hubRepository = new HubRepository(_context);
        _terminalRepository = new TerminalRepository(_context);
        _terminalInsertService = new TerminalInsertService(_terminalRepository, _hubRepository);
    }

    [Fact]
    public async Task InsertTerminalAsync_WithValidHubId_ShouldReturnTerminalId()
    {
        // Arrange
        Hub hub = new()
        {
            Name = "Aeroporto Internacional de São Paulo",
            Code = "GRU",
            Location = new NetTopologySuite.Geometries.Point(-46.469, -23.432) { SRID = 4326 }
        };
        _context.Hubs.Add(hub);
        await _context.SaveChangesAsync();

        TerminalInsertDto terminalDto = new(HubId: hub.HubId);

        // Act
        int result = await _terminalInsertService.InsertTerminalAsync(terminalDto);

        // Assert
        Assert.True(result > 0);

        Terminal? terminalInDb = await _context.Terminals.FindAsync(result);
        Assert.NotNull(terminalInDb);
        Assert.Equal(hub.HubId, terminalInDb.HubId);
    }

    [Fact]
    public async Task InsertTerminalAsync_WithNonExistentHubId_ShouldThrowEntityNotFoundException()
    {
        // Arrange
        TerminalInsertDto terminalDto = new(HubId: 999);

        // Act & Assert
        EntityNotFoundException exception = await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _terminalInsertService.InsertTerminalAsync(terminalDto)
        );

        Assert.Equal("Hub not found", exception.Message);
    }

    [Fact]
    public async Task InsertTerminalAsync_WithValidData_ShouldPersistToDatabase()
    {
        // Arrange
        Hub hub = new()
        {
            Name = "Aeroporto de Congonhas",
            Code = "CGH",
            Location = new NetTopologySuite.Geometries.Point(-46.656, -23.626) { SRID = 4326 }
        };
        _context.Hubs.Add(hub);
        await _context.SaveChangesAsync();

        TerminalInsertDto terminalDto = new(HubId: hub.HubId);

        // Act
        int terminalId = await _terminalInsertService.InsertTerminalAsync(terminalDto);

        // Assert
        Terminal? savedTerminal = await _context.Terminals.FirstOrDefaultAsync(t => t.TerminalId == terminalId);
        Assert.NotNull(savedTerminal);
        Assert.Equal(hub.HubId, savedTerminal.HubId);
    }

    [Fact]
    public async Task InsertTerminalAsync_MultipleTerminalsForSameHub_ShouldGenerateDifferentIds()
    {
        // Arrange
        Hub hub = new()
        {
            Name = "Aeroporto Internacional de Brasília",
            Code = "BSB",
            Location = new NetTopologySuite.Geometries.Point(-47.920, -15.869) { SRID = 4326 }
        };
        _context.Hubs.Add(hub);
        await _context.SaveChangesAsync();

        TerminalInsertDto terminal1Dto = new(HubId: hub.HubId);
        TerminalInsertDto terminal2Dto = new(HubId: hub.HubId);

        // Act
        int terminalId1 = await _terminalInsertService.InsertTerminalAsync(terminal1Dto);
        int terminalId2 = await _terminalInsertService.InsertTerminalAsync(terminal2Dto);

        // Assert
        Assert.NotEqual(terminalId1, terminalId2);
        Assert.Equal(2, await _context.Terminals.CountAsync());
    }

    [Fact]
    public async Task InsertTerminalAsync_TerminalsForDifferentHubs_ShouldCreateCorrectly()
    {
        // Arrange
        Hub hub1 = new()
        {
            Name = "Aeroporto do Galeão",
            Code = "GIG",
            Location = new NetTopologySuite.Geometries.Point(-43.250, -22.809) { SRID = 4326 }
        };
        Hub hub2 = new()
        {
            Name = "Aeroporto de Confins",
            Code = "CNF",
            Location = new NetTopologySuite.Geometries.Point(-43.972, -19.624) { SRID = 4326 }
        };
        _context.Hubs.AddRange(hub1, hub2);
        await _context.SaveChangesAsync();

        TerminalInsertDto terminal1Dto = new(HubId: hub1.HubId);
        TerminalInsertDto terminal2Dto = new(HubId: hub2.HubId);

        // Act
        int terminalId1 = await _terminalInsertService.InsertTerminalAsync(terminal1Dto);
        int terminalId2 = await _terminalInsertService.InsertTerminalAsync(terminal2Dto);

        // Assert
        Terminal? terminal1 = await _context.Terminals.FindAsync(terminalId1);
        Terminal? terminal2 = await _context.Terminals.FindAsync(terminalId2);

        Assert.NotNull(terminal1);
        Assert.NotNull(terminal2);
        Assert.Equal(hub1.HubId, terminal1.HubId);
        Assert.Equal(hub2.HubId, terminal2.HubId);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}
