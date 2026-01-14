
using Airport.Api.DTOs;
using Airport.Api.Enums;
using Airport.Api.Exceptions;
using Airport.Api.Models;
using Airport.Api.Repositories;
using Airport.Api.Validations;

namespace Airport.Api.Services;

public class GateEventService(
    GateEventRepository gateEventRepository,
    GateService gateService
    )

{
    private readonly GateEventRepository _gateEventRepository = gateEventRepository;
    private readonly GateService _gateService = gateService;

    public async Task<int> InsertGateEvent(GateEventInsertDto data)
    {

        Validator validator = new([
            new GateEventTypeValidation(data),
            new GateEventSchedulerValidation(_gateEventRepository, data)
        ]);
        validator.ValidateAndTrhow();

        _ = await _gateService.GetGateById(data.GateId) ?? throw new EntityNotFoundException("Gate not found.");

        int eventTypeId = (int)Enum.Parse<GateEventTypeEnum>(data.EventType, ignoreCase: true);

        GateEvent gateEvent = new()
        {
            StartDate = data.StartDate,
            EndDate = data.EndDate,
            GateId = data.GateId,
            GateEventTypeId = eventTypeId
        };

        return await _gateEventRepository.InsertGateEvent(gateEvent);
    }

}