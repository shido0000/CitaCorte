using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CitaCorte.API.Service.Interfaces;
using CitaCorte.API.Data.Entities;

namespace CitaCorte.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _adminService.GetAllUsersAsync();
        return Ok(users);
    }

    [HttpGet("barberos")]
    public async Task<IActionResult> GetAllBarberos()
    {
        var barberos = await _adminService.GetAllBarberosAsync();
        return Ok(barberos);
    }

    [HttpGet("barberias")]
    public async Task<IActionResult> GetAllBarberias()
    {
        var barberias = await _adminService.GetAllBarberiasAsync();
        return Ok(barberias);
    }

    [HttpPost("users/{userId}/deactivate")]
    public async Task<IActionResult> DeactivateUser(int userId)
    {
        try
        {
            await _adminService.DeactivateUserAsync(userId);
            return Ok(new { message = "Usuario desactivado" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("users/{userId}/activate")]
    public async Task<IActionResult> ActivateUser(int userId)
    {
        try
        {
            await _adminService.ActivateUserAsync(userId);
            return Ok(new { message = "Usuario activado" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("subscription-plans")]
    public async Task<IActionResult> GetAllSubscriptionPlans()
    {
        var plans = await _adminService.GetAllSubscriptionPlansAsync();
        return Ok(plans);
    }

    [HttpPost("subscription-plans")]
    public async Task<IActionResult> CreateSubscriptionPlan([FromBody] SubscriptionPlanDto dto)
    {
        try
        {
            var plan = new SubscriptionPlan
            {
                Type = dto.Type,
                Name = dto.Name,
                Description = dto.Description,
                IsForBarbero = dto.IsForBarbero,
                IsForBarberia = dto.IsForBarberia,
                Price = dto.Price,
                Currency = dto.Currency,
                DurationDays = dto.DurationDays,
                CanReceiveReservations = dto.CanReceiveReservations,
                CanViewStatistics = dto.CanViewStatistics,
                CanSellProducts = dto.CanSellProducts,
                CanEditProfile = dto.CanEditProfile,
                CanViewServices = dto.CanViewServices,
                MaxBarberosLimit = dto.MaxBarberosLimit
            };
            var result = await _adminService.CreateSubscriptionPlanAsync(plan);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("subscription-plans/{planId}")]
    public async Task<IActionResult> UpdateSubscriptionPlan(int planId, [FromBody] SubscriptionPlanDto dto)
    {
        try
        {
            var plan = new SubscriptionPlan
            {
                Id = planId,
                Type = dto.Type,
                Name = dto.Name,
                Description = dto.Description,
                IsForBarbero = dto.IsForBarbero,
                IsForBarberia = dto.IsForBarberia,
                Price = dto.Price,
                Currency = dto.Currency,
                DurationDays = dto.DurationDays,
                CanReceiveReservations = dto.CanReceiveReservations,
                CanViewStatistics = dto.CanViewStatistics,
                CanSellProducts = dto.CanSellProducts,
                CanEditProfile = dto.CanEditProfile,
                CanViewServices = dto.CanViewServices,
                MaxBarberosLimit = dto.MaxBarberosLimit
            };
            await _adminService.UpdateSubscriptionPlanAsync(plan);
            return Ok(new { message = "Plan actualizado" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("subscription-plans/{planId}")]
    public async Task<IActionResult> DeleteSubscriptionPlan(int planId)
    {
        try
        {
            await _adminService.DeleteSubscriptionPlanAsync(planId);
            return Ok(new { message = "Plan eliminado" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}

public class SubscriptionPlanDto
{
    [Required]
    public SubscriptionType Type { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsForBarbero { get; set; }
    public bool IsForBarberia { get; set; }
    public decimal Price { get; set; }
    public string Currency { get; set; } = "USD";
    public int DurationDays { get; set; }
    public bool CanReceiveReservations { get; set; }
    public bool CanViewStatistics { get; set; }
    public bool CanSellProducts { get; set; }
    public bool CanEditProfile { get; set; }
    public bool CanViewServices { get; set; }
    public int? MaxBarberosLimit { get; set; }
}
