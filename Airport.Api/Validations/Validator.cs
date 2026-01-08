
namespace Airport.Api.Validations;

public class Validator(List<IValidation> validations)
{
    private readonly List<IValidation> _validations = validations;
    private List<string> Errors { get; } = new List<string>();

    public List<string> GetErrors() => Errors;

    public bool Validate()
    {
        _validations.ForEach(v => Errors.AddRange(v.Validate()));
        return Errors.Count == 0;
    }




}