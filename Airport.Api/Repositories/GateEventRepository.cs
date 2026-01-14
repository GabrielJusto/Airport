using Airport.Api.Database;
using Airport.Api.Models;

using Microsoft.EntityFrameworkCore;

namespace Airport.Api.Repositories;

public class GateEventRepository(AirportDbContext context)
{
    private readonly AirportDbContext _context = context;

    public async Task<int> InsertGateEvent(GateEvent gateEvent)
    {
        await _context.GateEvents.AddAsync(gateEvent);
        await _context.SaveChangesAsync();

        return gateEvent.GateEventId;
    }

    public bool HasOverlappingEvents(int gateId, DateTimeOffset startDate, DateTimeOffset endDate)
    {
        return _context.GateEvents
            .Any(ge => ge.GateId == gateId &&
                           ((ge.StartDate <= startDate && ge.EndDate > startDate) ||
                            (ge.StartDate < endDate && ge.EndDate >= endDate) ||
                            (ge.StartDate >= startDate && ge.EndDate <= endDate)));
    }
}