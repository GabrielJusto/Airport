using Airport.Api.Database;
using Airport.Api.DTOs;
using Airport.Api.Exceptions;
using Airport.Api.Models;
using Airport.Api.Repositories;
using Airport.Api.Services;
using Microsoft.EntityFrameworkCore;

namespace Airport.Tests.Services;

public class GateServiceTests : IDisposable
{
    private readonly AirportDbContext _context;
    private readonly GateRepository _gateRepository;
    private readonly HubRepository _hubRepository;
    private readonly TerminalRepository _terminalRepository;
    private readonly GateService _gateService;

    public GateServiceTests()
    {
        DbContextOptions<AirportDbContext> options = new DbContextOptionsBuilder<AirportDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AirportDbContext(options);
        _gateRepository = new GateRepository(_context);
        _hubRepository = new HubRepository(_context);
        _terminalRepository = new TerminalRepository(_context);
        _gateService = new GateService(_gateRepository, _terminalRepository);
    }

    [Fact]
    public async Task InsertGate_WithValidData_ShouldReturnGateId()
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

        Terminal terminal = new()
        {
            HubId = hub.HubId
        };
        _context.Terminals.Add(terminal);
        await _context.SaveChangesAsync();

        GateInsertDto gateDto = new(
            Jetways: 2,
            MaxPassengersPerHour: 500,
            TerminalId: terminal.TerminalId
        );

        // Act
        int result = await _gateService.InsertGate(gateDto);

        // Assert
        Assert.True(result > 0);

        Gate? gateInDb = await _context.Gates.FindAsync(result);
        Assert.NotNull(gateInDb);
        Assert.Equal(2, gateInDb.Jetways);
        Assert.Equal(500, gateInDb.MaxPassengersPerHour);
        Assert.Equal(terminal.TerminalId, gateInDb.TerminalId);
    }

    [Fact]
    public async Task InsertGate_WithNonExistentTerminalId_ShouldThrowEntityNotFoundException()
    {
        // Arrange
        GateInsertDto gateDto = new(
            Jetways: 2,
            MaxPassengersPerHour: 500,
            TerminalId: 999
        );

        // Act & Assert
        EntityNotFoundException exception = await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _gateService.InsertGate(gateDto)
        );

        Assert.Equal("Terminal not found!", exception.Message);
    }

    [Fact]
    public async Task InsertGate_WithValidData_ShouldPersistToDatabase()
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

        Terminal terminal = new()
        {
            HubId = hub.HubId
        };
        _context.Terminals.Add(terminal);
        await _context.SaveChangesAsync();

        GateInsertDto gateDto = new(
            Jetways: 3,
            MaxPassengersPerHour: 750,
            TerminalId: terminal.TerminalId
        );

        // Act
        int gateId = await _gateService.InsertGate(gateDto);

        // Assert
        Gate? savedGate = await _context.Gates.FirstOrDefaultAsync(g => g.GateId == gateId);
        Assert.NotNull(savedGate);
        Assert.Equal(3, savedGate.Jetways);
        Assert.Equal(750, savedGate.MaxPassengersPerHour);
        Assert.Equal(terminal.TerminalId, savedGate.TerminalId);
    }

    [Fact]
    public async Task InsertGate_MultipleGatesForSameTerminal_ShouldGenerateDifferentIds()
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

        Terminal terminal = new()
        {
            HubId = hub.HubId
        };
        _context.Terminals.Add(terminal);
        await _context.SaveChangesAsync();

        GateInsertDto gateDto1 = new(
            Jetways: 1,
            MaxPassengersPerHour: 300,
            TerminalId: terminal.TerminalId
        );

        GateInsertDto gateDto2 = new(
            Jetways: 2,
            MaxPassengersPerHour: 600,
            TerminalId: terminal.TerminalId
        );

        // Act
        int gateId1 = await _gateService.InsertGate(gateDto1);
        int gateId2 = await _gateService.InsertGate(gateDto2);

        // Assert
        Assert.NotEqual(gateId1, gateId2);
        Assert.True(gateId1 > 0);
        Assert.True(gateId2 > 0);

        Gate? gate1 = await _context.Gates.FindAsync(gateId1);
        Gate? gate2 = await _context.Gates.FindAsync(gateId2);

        Assert.NotNull(gate1);
        Assert.NotNull(gate2);
        Assert.Equal(1, gate1.Jetways);
        Assert.Equal(2, gate2.Jetways);
    }

    [Fact]
    public async Task InsertGate_WithZeroJetways_ShouldInsertSuccessfully()
    {
        // Arrange
        Hub hub = new()
        {
            Name = "Aeroporto do Galeão",
            Code = "GIG",
            Location = new NetTopologySuite.Geometries.Point(-43.250, -22.809) { SRID = 4326 }
        };
        _context.Hubs.Add(hub);
        await _context.SaveChangesAsync();

        Terminal terminal = new()
        {
            HubId = hub.HubId
        };
        _context.Terminals.Add(terminal);
        await _context.SaveChangesAsync();

        GateInsertDto gateDto = new(
            Jetways: 0,
            MaxPassengersPerHour: 200,
            TerminalId: terminal.TerminalId
        );

        // Act
        int result = await _gateService.InsertGate(gateDto);

        // Assert
        Assert.True(result > 0);

        Gate? gateInDb = await _context.Gates.FindAsync(result);
        Assert.NotNull(gateInDb);
        Assert.Equal(0, gateInDb.Jetways);
        Assert.Equal(200, gateInDb.MaxPassengersPerHour);
    }

    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}
