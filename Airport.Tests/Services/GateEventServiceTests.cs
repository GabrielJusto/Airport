using Airport.Api.Database;
using Airport.Api.DTOs;
using Airport.Api.Enums;
using Airport.Api.Exceptions;
using Airport.Api.Models;
using Airport.Api.Repositories;
using Airport.Api.Services;
using Microsoft.EntityFrameworkCore;

namespace Airport.Tests.Services;

public class GateEventServiceTests : IDisposable
{
    private readonly AirportDbContext _context;
    private readonly GateEventRepository _gateEventRepository;
    private readonly GateRepository _gateRepository;
    private readonly TerminalRepository _terminalRepository;
    private readonly GateService _gateService;
    private readonly GateEventService _gateEventService;

    public GateEventServiceTests()
    {
        DbContextOptions<AirportDbContext> options = new DbContextOptionsBuilder<AirportDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AirportDbContext(options);
        _gateEventRepository = new GateEventRepository(_context);
        _gateRepository = new GateRepository(_context);
        _terminalRepository = new TerminalRepository(_context);
        _gateService = new GateService(_gateRepository, _terminalRepository);
        _gateEventService = new GateEventService(_gateEventRepository, _gateService);
    }

    [Fact]
    public async Task InsertGateEvent_WithValidData_ShouldReturnGateEventId()
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

        Gate gate = new()
        {
            Jetways = 2,
            MaxPassengersPerHour = 500,
            TerminalId = terminal.TerminalId
        };
        _context.Gates.Add(gate);
        await _context.SaveChangesAsync();

        GateEventInsertDto gateEventDto = new(
            StartDate: DateTimeOffset.UtcNow,
            EndDate: DateTimeOffset.UtcNow.AddHours(2),
            GateId: gate.GateId,
            EventType: "Departure"
        );

        // Act
        int result = await _gateEventService.InsertGateEvent(gateEventDto);

        // Assert
        Assert.True(result > 0);

        GateEvent? gateEventInDb = await _context.GateEvents.FindAsync(result);
        Assert.NotNull(gateEventInDb);
        Assert.Equal(gate.GateId, gateEventInDb.GateId);
        Assert.Equal((int)GateEventTypeEnum.Departure, gateEventInDb.GateEventTypeId);
        Assert.Equal(gateEventDto.StartDate, gateEventInDb.StartDate);
        Assert.Equal(gateEventDto.EndDate, gateEventInDb.EndDate);
    }

    [Fact]
    public async Task InsertGateEvent_WithArrivalEventType_ShouldReturnGateEventId()
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

        Gate gate = new()
        {
            Jetways = 1,
            MaxPassengersPerHour = 300,
            TerminalId = terminal.TerminalId
        };
        _context.Gates.Add(gate);
        await _context.SaveChangesAsync();

        GateEventInsertDto gateEventDto = new(
            StartDate: DateTimeOffset.UtcNow,
            EndDate: DateTimeOffset.UtcNow.AddHours(1),
            GateId: gate.GateId,
            EventType: "Arrival"
        );

        // Act
        int result = await _gateEventService.InsertGateEvent(gateEventDto);

        // Assert
        Assert.True(result > 0);

        GateEvent? gateEventInDb = await _context.GateEvents.FindAsync(result);
        Assert.NotNull(gateEventInDb);
        Assert.Equal((int)GateEventTypeEnum.Arrival, gateEventInDb.GateEventTypeId);
    }

    [Fact]
    public async Task InsertGateEvent_WithCaseInsensitiveEventType_ShouldReturnGateEventId()
    {
        // Arrange
        Hub hub = new()
        {
            Name = "Aeroporto de Brasília",
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

        Gate gate = new()
        {
            Jetways = 3,
            MaxPassengersPerHour = 600,
            TerminalId = terminal.TerminalId
        };
        _context.Gates.Add(gate);
        await _context.SaveChangesAsync();

        GateEventInsertDto gateEventDto = new(
            StartDate: DateTimeOffset.UtcNow,
            EndDate: DateTimeOffset.UtcNow.AddHours(3),
            GateId: gate.GateId,
            EventType: "departure"
        );

        // Act
        int result = await _gateEventService.InsertGateEvent(gateEventDto);

        // Assert
        Assert.True(result > 0);

        GateEvent? gateEventInDb = await _context.GateEvents.FindAsync(result);
        Assert.NotNull(gateEventInDb);
        Assert.Equal((int)GateEventTypeEnum.Departure, gateEventInDb.GateEventTypeId);
    }

    [Fact]
    public async Task InsertGateEvent_WithNonExistentGateId_ShouldThrowEntityNotFoundException()
    {
        // Arrange
        GateEventInsertDto gateEventDto = new(
            StartDate: DateTimeOffset.UtcNow,
            EndDate: DateTimeOffset.UtcNow.AddHours(2),
            GateId: 999,
            EventType: "Departure"
        );

        // Act & Assert
        EntityNotFoundException exception = await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _gateEventService.InsertGateEvent(gateEventDto)
        );

        Assert.Equal("Gate not found.", exception.Message);
    }

    [Fact]
    public async Task InsertGateEvent_WithInvalidEventType_ShouldThrowValidationException()
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

        Gate gate = new()
        {
            Jetways = 2,
            MaxPassengersPerHour = 450,
            TerminalId = terminal.TerminalId
        };
        _context.Gates.Add(gate);
        await _context.SaveChangesAsync();

        GateEventInsertDto gateEventDto = new(
            StartDate: DateTimeOffset.UtcNow,
            EndDate: DateTimeOffset.UtcNow.AddHours(2),
            GateId: gate.GateId,
            EventType: "InvalidEventType"
        );

        // Act & Assert
        ValidationException exception = await Assert.ThrowsAsync<ValidationException>(
            () => _gateEventService.InsertGateEvent(gateEventDto)
        );

        Assert.Contains("Invalid EventType", exception.Errors);
    }

    [Fact]
    public async Task InsertGateEvent_WithValidData_ShouldPersistToDatabase()
    {
        // Arrange
        Hub hub = new()
        {
            Name = "Aeroporto de Confins",
            Code = "CNF",
            Location = new NetTopologySuite.Geometries.Point(-43.972, -19.624) { SRID = 4326 }
        };
        _context.Hubs.Add(hub);
        await _context.SaveChangesAsync();

        Terminal terminal = new()
        {
            HubId = hub.HubId
        };
        _context.Terminals.Add(terminal);
        await _context.SaveChangesAsync();

        Gate gate = new()
        {
            Jetways = 2,
            MaxPassengersPerHour = 500,
            TerminalId = terminal.TerminalId
        };
        _context.Gates.Add(gate);
        await _context.SaveChangesAsync();

        GateEventInsertDto gateEventDto = new(
            StartDate: DateTimeOffset.UtcNow,
            EndDate: DateTimeOffset.UtcNow.AddHours(2),
            GateId: gate.GateId,
            EventType: "Departure"
        );

        // Act
        int gateEventId = await _gateEventService.InsertGateEvent(gateEventDto);

        // Assert
        GateEvent? savedGateEvent = await _context.GateEvents.FirstOrDefaultAsync(ge => ge.GateEventId == gateEventId);
        Assert.NotNull(savedGateEvent);
        Assert.Equal(gate.GateId, savedGateEvent.GateId);
        Assert.Equal((int)GateEventTypeEnum.Departure, savedGateEvent.GateEventTypeId);
    }

    [Fact]
    public async Task InsertGateEvent_MultipleEvents_ShouldGenerateDifferentIds()
    {
        // Arrange
        Hub hub = new()
        {
            Name = "Aeroporto de Guarulhos",
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

        Gate gate = new()
        {
            Jetways = 2,
            MaxPassengersPerHour = 500,
            TerminalId = terminal.TerminalId
        };
        _context.Gates.Add(gate);
        await _context.SaveChangesAsync();

        GateEventInsertDto gateEvent1Dto = new(
            StartDate: DateTimeOffset.UtcNow,
            EndDate: DateTimeOffset.UtcNow.AddHours(2),
            GateId: gate.GateId,
            EventType: "Departure"
        );

        GateEventInsertDto gateEvent2Dto = new(
            StartDate: DateTimeOffset.UtcNow.AddHours(3),
            EndDate: DateTimeOffset.UtcNow.AddHours(4),
            GateId: gate.GateId,
            EventType: "Arrival"
        );

        // Act
        int gateEventId1 = await _gateEventService.InsertGateEvent(gateEvent1Dto);
        int gateEventId2 = await _gateEventService.InsertGateEvent(gateEvent2Dto);

        // Assert
        Assert.NotEqual(gateEventId1, gateEventId2);
        Assert.Equal(2, await _context.GateEvents.CountAsync());
    }

    [Fact]
    public async Task InsertGateEvent_WithOverlappingDates_ShouldThrowValidationException()
    {
        // Arrange
        Hub hub = new()
        {
            Name = "Aeroporto de Salvador",
            Code = "SSA",
            Location = new NetTopologySuite.Geometries.Point(-38.331, -12.911) { SRID = 4326 }
        };
        _context.Hubs.Add(hub);
        await _context.SaveChangesAsync();

        Terminal terminal = new()
        {
            HubId = hub.HubId
        };
        _context.Terminals.Add(terminal);
        await _context.SaveChangesAsync();

        Gate gate = new()
        {
            Jetways = 2,
            MaxPassengersPerHour = 400,
            TerminalId = terminal.TerminalId
        };
        _context.Gates.Add(gate);
        await _context.SaveChangesAsync();

        DateTimeOffset baseDate = DateTimeOffset.UtcNow;

        GateEventInsertDto firstEventDto = new(
            StartDate: baseDate,
            EndDate: baseDate.AddHours(3),
            GateId: gate.GateId,
            EventType: "Departure"
        );

        await _gateEventService.InsertGateEvent(firstEventDto);

        GateEventInsertDto overlappingEventDto = new(
            StartDate: baseDate.AddHours(1),
            EndDate: baseDate.AddHours(4),
            GateId: gate.GateId,
            EventType: "Arrival"
        );

        // Act & Assert
        ValidationException exception = await Assert.ThrowsAsync<ValidationException>(
            () => _gateEventService.InsertGateEvent(overlappingEventDto)
        );

        Assert.Contains("Gate already has an event scheduled during this time period", exception.Errors);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}
