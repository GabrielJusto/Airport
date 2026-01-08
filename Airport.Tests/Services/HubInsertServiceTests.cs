using Airport.Api.Database;
using Airport.Api.DTOs;
using Airport.Api.Exceptions;
using Airport.Api.Models;
using Airport.Api.Repositories;
using Airport.Api.Services;
using Microsoft.EntityFrameworkCore;

namespace Airport.Tests.Services;

public class HubInsertServiceTests : IDisposable
{
    private readonly AirportDbContext _context;
    private readonly HubRepository _hubRepository;
    private readonly HubInsertService _hubInsertService;

    public HubInsertServiceTests()
    {
        DbContextOptions<AirportDbContext> options = new DbContextOptionsBuilder<AirportDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AirportDbContext(options);
        _hubRepository = new HubRepository(_context);
        _hubInsertService = new HubInsertService(_hubRepository);
    }

    [Fact]
    public async Task InsertHub_WithValidData_ShouldReturnHubId()
    {
        // Arrange
        HubInsertDto hubDto = new(
            Name: "Aeroporto Internacional de São Paulo",
            Code: "GRU",
            Latitude: -23.432,
            Longitude: -46.469
        );

        // Act
        int result = await _hubInsertService.InsertHub(hubDto);

        // Assert
        Assert.True(result > 0);

        Hub? hubInDb = await _context.Hubs.FindAsync(result);
        Assert.NotNull(hubInDb);
        Assert.Equal("Aeroporto Internacional de São Paulo", hubInDb.Name);
        Assert.Equal("GRU", hubInDb.Code);
        Assert.Equal(-46.469, hubInDb.Location.X);
        Assert.Equal(-23.432, hubInDb.Location.Y);
    }

    [Fact]
    public async Task InsertHub_WithInvalidCodeLength_ShouldThrowValidationException()
    {
        // Arrange
        HubInsertDto hubDto = new(
            Name: "Aeroporto de Congonhas",
            Code: "CG",
            Latitude: -23.626,
            Longitude: -46.656
        );

        // Act & Assert
        ValidationException exception = await Assert.ThrowsAsync<ValidationException>(
            () => _hubInsertService.InsertHub(hubDto)
        );

        Assert.Contains("Hub code must has 3 characters.", exception.Errors);
    }

    [Fact]
    public async Task InsertHub_WithLowercaseCode_ShouldThrowValidationException()
    {
        // Arrange
        HubInsertDto hubDto = new(
            Name: "Aeroporto de Brasília",
            Code: "bsb",
            Latitude: -15.869,
            Longitude: -47.920
        );

        // Act & Assert
        ValidationException exception = await Assert.ThrowsAsync<ValidationException>(
            () => _hubInsertService.InsertHub(hubDto)
        );

        Assert.Contains("Hub code must contain only uppercase letters.", exception.Errors);
    }

    [Fact]
    public async Task InsertHub_WithMultipleValidationErrors_ShouldThrowValidationExceptionWithAllErrors()
    {
        // Arrange
        HubInsertDto hubDto = new(
            Name: "Aeroporto do Galeão",
            Code: "gig1",
            Latitude: -22.809,
            Longitude: -43.250
        );

        // Act & Assert
        ValidationException exception = await Assert.ThrowsAsync<ValidationException>(
            () => _hubInsertService.InsertHub(hubDto)
        );

        Assert.Contains("Hub code must has 3 characters.", exception.Errors);
        Assert.Contains("Hub code must contain only uppercase letters.", exception.Errors);
        Assert.Equal(2, exception.Errors.Count);
    }

    [Fact]
    public async Task InsertHub_WithValidData_ShouldPersistToDatabase()
    {
        // Arrange
        HubInsertDto hubDto = new(
            Name: "Aeroporto de Confins",
            Code: "CNF",
            Latitude: -19.624,
            Longitude: -43.972
        );

        // Act
        int hubId = await _hubInsertService.InsertHub(hubDto);

        // Assert
        Hub? savedHub = await _context.Hubs.FirstOrDefaultAsync(h => h.HubId == hubId);
        Assert.NotNull(savedHub);
        Assert.Equal("Aeroporto de Confins", savedHub.Name);
        Assert.Equal("CNF", savedHub.Code);
    }

    [Fact]
    public async Task InsertHub_MultipleHubs_ShouldGenerateDifferentIds()
    {
        // Arrange
        HubInsertDto hub1Dto = new HubInsertDto("Hub 1", "AAA", -10.0, -20.0);
        HubInsertDto hub2Dto = new HubInsertDto("Hub 2", "BBB", -15.0, -25.0);

        // Act
        int hubId1 = await _hubInsertService.InsertHub(hub1Dto);
        int hubId2 = await _hubInsertService.InsertHub(hub2Dto);

        // Assert
        Assert.NotEqual(hubId1, hubId2);
        Assert.Equal(2, await _context.Hubs.CountAsync());
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}
