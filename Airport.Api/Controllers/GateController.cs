
using Airport.Api.DTOs;
using Airport.Api.Exceptions;
using Airport.Api.Services;

using Microsoft.AspNetCore.Mvc;

namespace Airport.Api.Controllers;

[ApiController]
[Route("gate")]
public class GateController(GateInsertService gateInsertService) : ControllerBase
{
    private readonly GateInsertService _gateInsertService = gateInsertService;


    [HttpPost("create")]
    public async Task<IResult> CreateGate(GateInsertDto data)
    {
        try
        {
            await _gateInsertService.InsertGate(data);

            return Results.Created();
        }
        catch(EntityNotFoundException e)
        {
            return Results.NotFound(e.Message);
        }
        catch(Exception)
        {
            return Results.InternalServerError();
        }
    }
}