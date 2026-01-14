using Airport.Api.DTOs;
using Airport.Api.Exceptions;
using Airport.Api.Models;
using Airport.Api.Repositories;

namespace Airport.Api.Services;

public class GateService(
    GateRepository gateRepository,
    TerminalRepository terminalRepository
)
{
    private readonly GateRepository _gateRepository = gateRepository;
    private readonly TerminalRepository _terminalRepository = terminalRepository;

    public async Task<int> InsertGate(GateInsertDto data)
    {
        Terminal _ = await _terminalRepository.GetByTerminalId(data.TerminalId)
            ?? throw new EntityNotFoundException("Terminal not found!");

        Gate gate = new(data);

        return await _gateRepository.InsertGate(gate);
    }

    public async Task<Gate?> GetGateById(int gateId)
    {
        return await _gateRepository.GetGateById(gateId);
    }

}