
namespace Airport.Api.DTOs;

public record HubInsertDto(
    string Name,
    string Code,
    double Latitude,
    double Longitude
)
{

}