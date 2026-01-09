
namespace Airport.Api.DTOs;

public record GateInsertDto(
    int Jetways,
    int MaxPassengersPerHour,
    int TerminalId
)
{

}