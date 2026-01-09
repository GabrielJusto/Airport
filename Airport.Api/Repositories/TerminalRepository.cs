
using Airport.Api.Database;
using Airport.Api.Models;

using Microsoft.EntityFrameworkCore;

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

    public async Task<Terminal?> GetByTerminalId(int terminalId)
    {
        return await _context.Terminals.FirstOrDefaultAsync(t => t.TerminalId == terminalId);
    }

}