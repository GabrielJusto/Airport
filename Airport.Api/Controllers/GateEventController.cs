
using Airport.Api.DTOs;
using Airport.Api.Exceptions;
using Airport.Api.Services;

using Microsoft.AspNetCore.Mvc;

namespace Airport.Api.Controllers;

[ApiController]
[Route("gate-event")]
public class GateEventController(
    GateEventService gateEventService
) : ControllerBase
{
    private readonly GateEventService _gateEventService = gateEventService;

    [HttpPost("create")]
    public async Task<IResult> CreateGateEvent(GateEventInsertDto data)
    {
        try
        {
            int gateEventId = await _gateEventService.InsertGateEvent(data);

            return Results.Created();
        }
        catch(EntityNotFoundException e)
        {
            return Results.NotFound(e.Message);
        }
        catch(ValidationException e)
        {
            return Results.BadRequest(e.Errors);
        }
        catch(Exception)
        {
            return Results.InternalServerError();
        }
    }
}