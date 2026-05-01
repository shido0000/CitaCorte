using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CitaCorte.API.Service.Interfaces;
using CitaCorte.API.Data.Entities;
using System.Security.Claims;

namespace CitaCorte.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReservationController : ControllerBase
{
    private readonly IReservationService _reservationService;

    public ReservationController(IReservationService reservationService)
    {
        _reservationService = reservationService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateReservation([FromBody] CreateReservationDto dto)
    {
        var userId = GetCurrentUserId();
        try
        {
            var endDateTime = dto.StartDateTime.AddMinutes(dto.DurationMinutes);
            var reservation = await _reservationService.CreateReservationAsync(
                userId, dto.ServiceId, dto.BarberoId, dto.BarberiaId, 
                dto.StartDateTime, endDateTime, dto.Notes);
            return Ok(reservation);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{reservationId}/confirm")]
    public async Task<IActionResult> ConfirmReservation(int reservationId)
    {
        var userId = GetCurrentUserId();
        try
        {
            var reservation = await _reservationService.ConfirmReservationAsync(reservationId, userId);
            return Ok(reservation);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{reservationId}/cancel")]
    public async Task<IActionResult> CancelReservation(int reservationId, [FromBody] CancelReservationDto? dto)
    {
        var userId = GetCurrentUserId();
        try
        {
            var reservation = await _reservationService.CancelReservationAsync(reservationId, userId, dto?.Reason);
            return Ok(reservation);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{reservationId}")]
    public async Task<IActionResult> GetReservation(int reservationId)
    {
        try
        {
            var reservation = await _reservationService.GetReservationByIdAsync(reservationId);
            if (reservation == null)
            {
                return NotFound();
            }
            return Ok(reservation);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("my-reservations")]
    public async Task<IActionResult> GetMyReservations()
    {
        var userId = GetCurrentUserId();
        try
        {
            var reservations = await _reservationService.GetClientReservationsAsync(userId);
            return Ok(reservations);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("barbero")]
    public async Task<IActionResult> GetBarberoReservations([FromQuery] ReservationStatus? status)
    {
        var userId = GetCurrentUserId();
        // In a real app, you would get the barbero ID from the user's profile
        // For now, this is a simplified version
        return BadRequest(new { message = "Use el endpoint específico de barbero" });
    }

    [HttpGet("barberia")]
    public async Task<IActionResult> GetBarberiaReservations([FromQuery] ReservationStatus? status)
    {
        var userId = GetCurrentUserId();
        // In a real app, you would get the barberia ID from the user's profile
        // For now, this is a simplified version
        return BadRequest(new { message = "Use el endpoint específico de barbería" });
    }

    [HttpGet("check-availability")]
    public async Task<IActionResult> CheckAvailability(
        [FromQuery] int? barberoId, 
        [FromQuery] int? barberiaId, 
        [FromQuery] DateTime startDateTime, 
        [FromQuery] int durationMinutes)
    {
        try
        {
            var endDateTime = startDateTime.AddMinutes(durationMinutes);
            var hasConflict = await _reservationService.HasTimeSlotConflictAsync(
                barberoId, barberiaId, startDateTime, endDateTime);
            
            return Ok(new { Available = !hasConflict });
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

public class CreateReservationDto
{
    [Required]
    public int ServiceId { get; set; }
    public int? BarberoId { get; set; }
    public int? BarberiaId { get; set; }
    [Required]
    public DateTime StartDateTime { get; set; }
    [Required]
    public int DurationMinutes { get; set; }
    public string? Notes { get; set; }
}

public class CancelReservationDto
{
    public string? Reason { get; set; }
}
