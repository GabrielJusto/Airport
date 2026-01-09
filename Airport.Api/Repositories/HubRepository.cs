
using Airport.Api.Database;
using Airport.Api.Models;

using Microsoft.EntityFrameworkCore;

namespace Airport.Api.Repositories;

public class HubRepository(AirportDbContext context)
{

    private readonly AirportDbContext _context = context;

    public async Task<int> InsertHub(Hub hub)
    {
        await _context.Hubs.AddAsync(hub);
        await _context.SaveChangesAsync();

        return hub.HubId;
    }

    public async Task<Hub?> GetHubByIdAsync(int id)
    {
        return await _context.Hubs.FirstOrDefaultAsync(h => h.HubId == id);
    }
}