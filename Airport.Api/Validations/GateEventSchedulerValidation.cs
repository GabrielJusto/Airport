
using Airport.Api.DTOs;
using Airport.Api.Exceptions;
using Airport.Api.Repositories;

namespace Airport.Api.Validations;

public class GateEventSchedulerValidation(
    GateEventRepository gateEventRepository,
    GateEventInsertDto data
) : IValidation
{

    private readonly GateEventRepository _gateEventRepository = gateEventRepository;
    private readonly GateEventInsertDto _data = data;
    public List<string> Validate()
    {
        bool hasOverlap = _gateEventRepository.HasOverlappingEvents(_data.GateId, _data.StartDate, _data.EndDate);
        if(hasOverlap)
        {
            throw new ValidationException(["Gate already has an event scheduled during this time period"]);
        }
        return [];
    }
}