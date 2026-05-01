using Microsoft.AspNetCore.Mvc;
using CitaCorte.Data.Entities;
using CitaCorte.Service.Interfaces;

namespace CitaCorte.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BarberoController : ControllerBase
{
    private readonly IBarberoService _barberoService;

    public BarberoController(IBarberoService barberoService)
    {
        _barberoService = barberoService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetBarbero(int id)
    {
        var barbero = await _barberoService.GetBarberoByIdAsync(id);
        if (barbero == null) return NotFound();
        return Ok(barbero);
    }

    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateBarberoRequest request)
    {
        var barberoId = 1; // Extract from JWT
        try
        {
            var updated = await _barberoService.UpdateBarberoAsync(
                barberoId, request.Bio, request.ProfileImageUrl, request.Address, request.Latitude, request.Longitude);
            return Ok(updated);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id}/services")]
    public async Task<IActionResult> GetServices(int id)
    {
        var services = await _barberoService.GetBarberoServicesAsync(id);
        return Ok(services);
    }

    [HttpPost("{id}/services")]
    public async Task<IActionResult> AddService(int id, [FromBody] Service service)
    {
        try
        {
            var created = await _barberoService.AddServiceAsync(id, service);
            return CreatedAtAction(nameof(GetServices), new { id }, created);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("services/{serviceId}")]
    public async Task<IActionResult> UpdateService(int serviceId, [FromBody] Service service)
    {
        var result = await _barberoService.UpdateServiceAsync(serviceId, service);
        if (!result) return NotFound();
        return NoContent();
    }

    [HttpDelete("services/{serviceId}")]
    public async Task<IActionResult> DeleteService(int serviceId)
    {
        var result = await _barberoService.DeleteServiceAsync(serviceId);
        if (!result) return NotFound();
        return NoContent();
    }

    [HttpGet("{id}/products")]
    public async Task<IActionResult> GetProducts(int id)
    {
        try
        {
            var products = await _barberoService.GetBarberoProductsAsync(id);
            return Ok(products);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{id}/products")]
    public async Task<IActionResult> AddProduct(int id, [FromBody] Product product)
    {
        try
        {
            var created = await _barberoService.AddProductAsync(id, product);
            return CreatedAtAction(nameof(GetProducts), new { id }, created);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("affiliate/{barberiaId}")]
    public async Task<IActionResult> RequestAffiliation(int barberiaId)
    {
        var barberoId = 1;
        try
        {
            var result = await _barberoService.RequestAffiliationToBarberiaAsync(barberoId, barberiaId);
            return Ok(new { message = "Solicitud de afiliación enviada" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("subscription/change")]
    public async Task<IActionResult> RequestSubscriptionChange([FromBody] SubscriptionChangeRequest request)
    {
        var barberoId = 1;
        try
        {
            var result = await _barberoService.RequestSubscriptionChangeAsync(barberoId, request.NewSubscription);
            return Ok(new { message = "Solicitud de cambio de suscripción enviada" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("statistics")]
    public async Task<IActionResult> GetStatistics()
    {
        var barberoId = 1;
        try
        {
            var stats = await _barberoService.GetBarberoStatisticsAsync(barberoId);
            return Ok(stats);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}

public class UpdateBarberoRequest
{
    public string? Bio { get; set; }
    public string? ProfileImageUrl { get; set; }
    public string? Address { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
}

public class SubscriptionChangeRequest
{
    public SubscriptionType NewSubscription { get; set; }
}
