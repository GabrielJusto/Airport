using Airport.Api.DTOs;

namespace Airport.Api.Validations;

public class HubCodeValidation(HubInsertDto data) : IValidation
{
    private readonly HubInsertDto _data = data;

    public List<string> Validate()
    {
        List<string> errors = new List<string>();
        if(_data.Code.Length != 3)
        {
            errors.Add("Hub code must has 3 characters.");
        }

        if(!_data.Code.All(char.IsUpper))
        {
            errors.Add("Hub code must contain only uppercase letters.");
        }

        return errors;
    }

}