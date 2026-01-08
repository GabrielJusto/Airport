using Airport.Api.DTOs;
using Airport.Api.Exceptions;
using Airport.Api.Repositories;
using Airport.Api.Services;

using Microsoft.AspNetCore.Mvc;

namespace Airport.Api.Controllers;

[ApiController]
[Route("terminal")]
public class TerminalController(TerminalInsertService terminalInsertService) : ControllerBase
{

    private readonly TerminalInsertService _terminalInsertService = terminalInsertService;
    [HttpPost("create")]
    public async Task<IResult> CreateTerminal(TerminalInsertDto data)
    {
        try
        {
            await _terminalInsertService.InsertTerminalAsync(data);

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