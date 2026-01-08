
using Airport.Api.Database;
using Airport.Api.Models;

namespace Airport.Api.Repositories;

public class TerminalRepository(AirportDbContext context)
{
    private readonly AirportDbContext _context = context;

    public async Task<int> InsertTerminalAsync(Terminal terminal)
    {

        await _context.Terminals.AddAsync(terminal);
        await _context.SaveChangesAsync();

        return terminal.TerminalId;
    }

}