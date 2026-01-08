using Airport.Api.DTOs;
using Airport.Api.Models;
using Airport.Api.Services;

using Microsoft.AspNetCore.Mvc;

namespace Airport.Api.Controllers;

[ApiController]
[Route("hub")]
public class HubController(HubInsertService hubInsertService) : ControllerBase
{
    private readonly HubInsertService _hubInsertService = hubInsertService;

    [HttpPost("create")]
    public async Task<IResult> CreateHub(HubInsertDto data)
    {
        await _hubInsertService.InsertHub(data);
        return Results.Created();
    }
}