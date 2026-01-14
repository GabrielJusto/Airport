namespace Airport.Api.DTOs;

public record GateEventInsertDto(
    DateTimeOffset StartDate,
    DateTimeOffset EndDate,
    int GateId,
    string EventType
)
{

}