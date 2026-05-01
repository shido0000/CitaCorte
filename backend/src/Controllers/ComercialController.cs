using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CitaCorte.API.Service.Interfaces;
using System.Security.Claims;

namespace CitaCorte.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Comercial,Admin")]
public class ComercialController : ControllerBase
{
    private readonly IComercialService _comercialService;

    public ComercialController(IComercialService comercialService)
    {
        _comercialService = comercialService;
    }

    [HttpGet("barberos/pending")]
    public async Task<IActionResult> GetBarberosPendingApproval()
    {
        var barberos = await _comercialService.GetBarberosPendingSubscriptionApprovalAsync();
        return Ok(barberos);
    }

    [HttpGet("barberias/pending")]
    public async Task<IActionResult> GetBarberiasPendingApproval()
    {
        var barberias = await _comercialService.GetBarberiasPendingSubscriptionApprovalAsync();
        return Ok(barberias);
    }

    [HttpPost("barberos/{barberoId}/approve")]
    public async Task<IActionResult> ApproveBarberoSubscription(int barberoId, [FromQuery] bool approved = true)
    {
        try
        {
            await _comercialService.ApproveBarberoSubscriptionAsync(barberoId, approved);
            return Ok(new { message = approved ? "Suscripción aprobada" : "Suscripción rechazada" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("barberias/{barberiaId}/approve")]
    public async Task<IActionResult> ApproveBarberiaSubscription(int barberiaId, [FromQuery] bool approved = true)
    {
        try
        {
            await _comercialService.ApproveBarberiaSubscriptionAsync(barberiaId, approved);
            return Ok(new { message = approved ? "Suscripción aprobada" : "Suscripción rechazada" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("statistics")]
    public async Task<IActionResult> GetGeneralStatistics([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        try
        {
            var statistics = await _comercialService.GetGeneralStatisticsAsync(startDate, endDate);
            return Ok(statistics);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
