
namespace Airport.Api.Exceptions;

public class ValidationException(List<string> errors) : Exception
{
    public List<string> Errors = errors;


}