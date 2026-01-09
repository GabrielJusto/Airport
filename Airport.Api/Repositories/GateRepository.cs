
using Airport.Api.Database;
using Airport.Api.Models;

namespace Airport.Api.Repositories;

public class GateRepository(AirportDbContext context)
{
    private readonly AirportDbContext _context = context;

    public async Task<int> InsertGate(Gate gate)
    {
        await _context.Gates.AddAsync(gate);
        await _context.SaveChangesAsync();

        return gate.GateId;
    }
}