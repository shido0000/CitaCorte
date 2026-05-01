using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CitaCorte.API.Service.Interfaces;
using System.Security.Claims;

namespace CitaCorte.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpGet]
    public async Task<IActionResult> GetNotifications([FromQuery] bool unreadOnly = false)
    {
        var userId = GetCurrentUserId();
        try
        {
            var notifications = await _notificationService.GetUserNotificationsAsync(userId, unreadOnly);
            return Ok(notifications);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{notificationId}/read")]
    public async Task<IActionResult> MarkAsRead(int notificationId)
    {
        try
        {
            await _notificationService.MarkNotificationAsReadAsync(notificationId);
            return Ok(new { message = "Notificación marcada como leída" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("read-all")]
    public async Task<IActionResult> MarkAllAsRead()
    {
        var userId = GetCurrentUserId();
        try
        {
            await _notificationService.MarkAllNotificationsAsReadAsync(userId);
            return Ok(new { message = "Todas las notificaciones marcadas como leídas" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{notificationId}")]
    public async Task<IActionResult> DeleteNotification(int notificationId)
    {
        try
        {
            await _notificationService.DeleteNotificationAsync(notificationId);
            return Ok(new { message = "Notificación eliminada" });
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
