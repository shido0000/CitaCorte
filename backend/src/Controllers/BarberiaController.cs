using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CitaCorte.API.Service.Interfaces;
using CitaCorte.API.Data.Entities;
using System.Security.Claims;

namespace CitaCorte.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BarberiaController : ControllerBase
{
    private readonly IBarberiaService _barberiaService;
    private readonly ISubscriptionService _subscriptionService;

    public BarberiaController(IBarberiaService barberiaService, ISubscriptionService subscriptionService)
    {
        _barberiaService = barberiaService;
        _subscriptionService = subscriptionService;
    }

    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var userId = GetCurrentUserId();
        try
        {
            var barberia = await _barberiaService.GetBarberiaByUserIdAsync(userId);
            return Ok(barberia);
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateBarberia([FromBody] CreateBarberiaDto dto)
    {
        var userId = GetCurrentUserId();
        try
        {
            var barberia = await _barberiaService.CreateBarberiaAsync(
                userId, dto.Name, dto.Description, dto.Address, dto.City, dto.State, dto.Phone, dto.LogoUrl);
            return Ok(barberia);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateBarberiaProfileDto dto)
    {
        var userId = GetCurrentUserId();
        try
        {
            var barberia = await _barberiaService.UpdateBarberiaProfileAsync(
                userId, dto.Name, dto.Description, dto.Address, dto.City, dto.State, dto.Phone, dto.LogoUrl);
            return Ok(barberia);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("affiliation/requests/pending")]
    public async Task<IActionResult> GetPendingAffiliationRequests()
    {
        var userId = GetCurrentUserId();
        try
        {
            var requests = await _barberiaService.GetPendingAffiliationRequestsAsync(userId);
            return Ok(requests);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("affiliation/respond/{requestId}")]
    public async Task<IActionResult> RespondToAffiliationRequest(int requestId, [FromBody] RespondToAffiliationDto dto)
    {
        var userId = GetCurrentUserId();
        try
        {
            await _barberiaService.RespondToAffiliationRequestAsync(userId, requestId, dto.Accepted, dto.RejectionReason);
            return Ok(new { message = dto.Accepted ? "Solicitud aceptada" : "Solicitud rechazada" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("affiliated/barberos")]
    public async Task<IActionResult> GetAffiliatedBarberos()
    {
        var userId = GetCurrentUserId();
        try
        {
            var barberia = await _barberiaService.GetBarberiaByUserIdAsync(userId);
            var barberos = await _barberiaService.GetAffiliatedBarberosAsync(barberia.Id);
            return Ok(barberos);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("barberos/{barberoId}")]
    public async Task<IActionResult> RemoveBarbero(int barberoId)
    {
        var userId = GetCurrentUserId();
        try
        {
            var barberia = await _barberiaService.GetBarberiaByUserIdAsync(userId);
            await _barberiaService.RemoveBarberoAsync(barberia.Id, barberoId);
            return Ok(new { message = "Barbero removido" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("services")]
    public async Task<IActionResult> GetServices()
    {
        var userId = GetCurrentUserId();
        try
        {
            var barberia = await _barberiaService.GetBarberiaByUserIdAsync(userId);
            var services = await _barberiaService.GetBarberiaServicesAsync(barberia.Id);
            return Ok(services);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("services")]
    public async Task<IActionResult> AddService([FromBody] ServiceDto dto)
    {
        var userId = GetCurrentUserId();
        try
        {
            var barberia = await _barberiaService.GetBarberiaByUserIdAsync(userId);
            var service = new Service
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                DurationMinutes = dto.DurationMinutes
            };
            var result = await _barberiaService.AddServiceAsync(barberia.Id, service);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("services/{serviceId}")]
    public async Task<IActionResult> UpdateService(int serviceId, [FromBody] ServiceDto dto)
    {
        try
        {
            var service = new Service
            {
                Id = serviceId,
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                DurationMinutes = dto.DurationMinutes
            };
            await _barberiaService.UpdateServiceAsync(service);
            return Ok(new { message = "Servicio actualizado" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("services/{serviceId}")]
    public async Task<IActionResult> DeleteService(int serviceId)
    {
        try
        {
            await _barberiaService.DeleteServiceAsync(serviceId);
            return Ok(new { message = "Servicio eliminado" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("statistics")]
    public async Task<IActionResult> GetStatistics([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        var userId = GetCurrentUserId();
        try
        {
            var barberia = await _barberiaService.GetBarberiaByUserIdAsync(userId);
            var statistics = await _barberiaService.GetBarberiaStatisticsAsync(barberia.Id, startDate, endDate);
            return Ok(statistics);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("subscription/request/{subscriptionType}")]
    public async Task<IActionResult> RequestSubscription(SubscriptionType subscriptionType)
    {
        var userId = GetCurrentUserId();
        try
        {
            var barberia = await _barberiaService.GetBarberiaByUserIdAsync(userId);
            await _subscriptionService.RequestBarberiaSubscriptionAsync(barberia.Id, subscriptionType);
            return Ok(new { message = "Solicitud de suscripción enviada. Espera la aprobación del admin o comercial." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
        {
            throw new UnauthorizedAccessException();
        }
        return int.Parse(userIdClaim);
    }
}

public class CreateBarberiaDto
{
    [Required]
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Phone { get; set; }
    public string? LogoUrl { get; set; }
}

public class UpdateBarberiaProfileDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Phone { get; set; }
    public string? LogoUrl { get; set; }
}

public class RespondToAffiliationDto
{
    public bool Accepted { get; set; }
    public string? RejectionReason { get; set; }
}
