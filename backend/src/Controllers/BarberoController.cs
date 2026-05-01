using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CitaCorte.API.Service.Interfaces;
using CitaCorte.API.Data.Entities;
using System.Security.Claims;

namespace CitaCorte.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BarberoController : ControllerBase
{
    private readonly IBarberoService _barberoService;
    private readonly ISubscriptionService _subscriptionService;

    public BarberoController(IBarberoService barberoService, ISubscriptionService subscriptionService)
    {
        _barberoService = barberoService;
        _subscriptionService = subscriptionService;
    }

    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var userId = GetCurrentUserId();
        try
        {
            var barbero = await _barberoService.GetBarberoByUserIdAsync(userId);
            return Ok(barbero);
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateBarberoProfileDto dto)
    {
        var userId = GetCurrentUserId();
        try
        {
            var barbero = await _barberoService.UpdateBarberoProfileAsync(userId, dto.Bio, dto.Specialties, dto.ProfileImageUrl);
            return Ok(barbero);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("affiliation/request/{barberiaId}")]
    public async Task<IActionResult> RequestAffiliation(int barberiaId, [FromBody] AffiliationRequestDto? dto)
    {
        var userId = GetCurrentUserId();
        try
        {
            await _barberoService.RequestAffiliationAsync(userId, barberiaId, dto?.Message);
            return Ok(new { message = "Solicitud de afiliación enviada" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("affiliation/cancel")]
    public async Task<IActionResult> CancelAffiliationRequest()
    {
        var userId = GetCurrentUserId();
        try
        {
            await _barberoService.CancelAffiliationRequestAsync(userId);
            return Ok(new { message = "Solicitud de afiliación cancelada" });
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
            var barbero = await _barberoService.GetBarberoByUserIdAsync(userId);
            var services = await _barberoService.GetBarberoServicesAsync(barbero.Id);
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
            var barbero = await _barberoService.GetBarberoByUserIdAsync(userId);
            var service = new Service
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                DurationMinutes = dto.DurationMinutes
            };
            var result = await _barberoService.AddServiceAsync(barbero.Id, service);
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
            await _barberoService.UpdateServiceAsync(service);
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
            await _barberoService.DeleteServiceAsync(serviceId);
            return Ok(new { message = "Servicio eliminado" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("products")]
    public async Task<IActionResult> AddProduct([FromBody] ProductDto dto)
    {
        var userId = GetCurrentUserId();
        try
        {
            var barbero = await _barberoService.GetBarberoByUserIdAsync(userId);
            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                StockQuantity = dto.StockQuantity,
                ImageUrl = dto.ImageUrl
            };
            var result = await _barberoService.AddProductAsync(barbero.Id, product);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("products")]
    public async Task<IActionResult> GetProducts()
    {
        var userId = GetCurrentUserId();
        try
        {
            var barbero = await _barberoService.GetBarberoByUserIdAsync(userId);
            var products = await _barberoService.GetBarberoProductsAsync(barbero.Id);
            return Ok(products);
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
            var barbero = await _barberoService.GetBarberoByUserIdAsync(userId);
            var statistics = await _barberoService.GetBarberoStatisticsAsync(barbero.Id, startDate, endDate);
            return Ok(statistics);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("subscription/change/{subscriptionType}")]
    public async Task<IActionResult> RequestSubscriptionChange(SubscriptionType subscriptionType)
    {
        var userId = GetCurrentUserId();
        try
        {
            var barbero = await _barberoService.GetBarberoByUserIdAsync(userId);
            await _subscriptionService.RequestSubscriptionChangeAsync(barbero.Id, subscriptionType);
            return Ok(new { message = "Solicitud de cambio de suscripción enviada. Espera la aprobación del admin o comercial." });
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

public class UpdateBarberoProfileDto
{
    public string? Bio { get; set; }
    public string? Specialties { get; set; }
    public string? ProfileImageUrl { get; set; }
}

public class AffiliationRequestDto
{
    public string? Message { get; set; }
}

public class ServiceDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int DurationMinutes { get; set; }
}

public class ProductDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public string? ImageUrl { get; set; }
}
