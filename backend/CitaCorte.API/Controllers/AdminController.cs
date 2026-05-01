using Microsoft.AspNetCore.Mvc;
using CitaCorte.Data.Entities;
using CitaCorte.Service.Interfaces;

namespace CitaCorte.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    [HttpPost("plans")]
    public async Task<IActionResult> CreateSubscriptionPlan([FromBody] SubscriptionPlan plan)
    {
        var createdPlan = await _adminService.CreateSubscriptionPlanAsync(plan);
        return CreatedAtAction(nameof(GetAllPlans), new { id = createdPlan.Id }, createdPlan);
    }

    [HttpGet("plans")]
    public async Task<IActionResult> GetAllPlans()
    {
        var plans = await _adminService.GetAllSubscriptionPlansAsync();
        return Ok(plans);
    }

    [HttpPut("plans/{id}")]
    public async Task<IActionResult> UpdatePlan(int id, [FromBody] SubscriptionPlan plan)
    {
        var updatedPlan = await _adminService.UpdateSubscriptionPlanAsync(id, plan);
        if (updatedPlan == null) return NotFound();
        return Ok(updatedPlan);
    }

    [HttpDelete("plans/{id}")]
    public async Task<IActionResult> DeletePlan(int id)
    {
        var result = await _adminService.DeleteSubscriptionPlanAsync(id);
        if (!result) return NotFound();
        return NoContent();
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

    [HttpGet("statistics")]
    public async Task<IActionResult> GetSystemStatistics()
    {
        var stats = await _adminService.GetSystemStatisticsAsync();
        return Ok(stats);
    }

    [HttpPost("barbero-subscription/{changeId}/approve")]
    public async Task<IActionResult> ApproveBarberoSubscriptionChange(int changeId)
    {
        var adminId = 1; // Extract from JWT in production
        var result = await _adminService.ApproveBarberoSubscriptionChangeAsync(changeId, adminId);
        if (!result) return BadRequest();
        return Ok(new { message = "Suscripción aprobada" });
    }

    [HttpPost("barbero-subscription/{changeId}/reject")]
    public async Task<IActionResult> RejectBarberoSubscriptionChange(int changeId, [FromBody] RejectRequest request)
    {
        var adminId = 1;
        var result = await _adminService.RejectBarberoSubscriptionChangeAsync(changeId, adminId, request.Reason);
        if (!result) return BadRequest();
        return Ok(new { message = "Suscripción rechazada" });
    }

    [HttpPost("barberia-subscription/{changeId}/approve")]
    public async Task<IActionResult> ApproveBarberiaSubscriptionChange(int changeId)
    {
        var adminId = 1;
        var result = await _adminService.ApproveBarberiaSubscriptionChangeAsync(changeId, adminId);
        if (!result) return BadRequest();
        return Ok(new { message = "Suscripción aprobada" });
    }

    [HttpPost("barberia-subscription/{changeId}/reject")]
    public async Task<IActionResult> RejectBarberiaSubscriptionChange(int changeId, [FromBody] RejectRequest request)
    {
        var adminId = 1;
        var result = await _adminService.RejectBarberiaSubscriptionChangeAsync(changeId, adminId, request.Reason);
        if (!result) return BadRequest();
        return Ok(new { message = "Suscripción rechazada" });
    }
}

public class RejectRequest
{
    public string Reason { get; set; } = string.Empty;
}
