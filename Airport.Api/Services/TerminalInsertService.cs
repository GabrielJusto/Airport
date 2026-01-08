
using Airport.Api.DTOs;
using Airport.Api.Exceptions;
using Airport.Api.Models;
using Airport.Api.Repositories;

namespace Airport.Api.Services;

public class TerminalInsertService(
    TerminalRepository terminalRepository,
    HubRepository hubRepository)
{
    private readonly TerminalRepository _terminalRepository = terminalRepository;
    private readonly HubRepository _hubRepository = hubRepository;


    public async Task<int> InsertTerminalAsync(TerminalInsertDto data)
    {
        Hub? hub = await _hubRepository.GetHubByIdAsync(data.HubId);
        if(hub == null)
        {
            throw new EntityNotFoundException("Hub not found");
        }

        Terminal terminal = new()
        {
            HubId = hub.HubId
        };

        return await _terminalRepository.InsertTerminalAsync(terminal);
    }
}