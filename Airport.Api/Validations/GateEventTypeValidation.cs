

using Airport.Api.DTOs;
using Airport.Api.Enums;

namespace Airport.Api.Validations;

public class GateEventTypeValidation(GateEventInsertDto data) : IValidation
{
    private readonly GateEventInsertDto _data = data;
    public List<string> Validate()
    {
        List<string> errors = [];
        if(!Enum.TryParse<GateEventTypeEnum>(_data.EventType, ignoreCase: true, out var _))
        {
            errors.Add("Invalid EventType");
        }
        return errors;
    }
}