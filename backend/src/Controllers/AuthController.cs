using Microsoft.AspNetCore.Mvc;
using CitaCorte.API.Service.Interfaces;
using CitaCorte.API.Data.Entities;
using System.Security.Claims;

namespace CitaCorte.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var result = await _authService.RegisterAsync(dto.Name, dto.Email, dto.Password, dto.Role, dto.Phone);
        
        if (result == null)
        {
            return BadRequest(new { message = "El correo electrónico ya está registrado" });
        }

        var (user, token) = result.Value;
        return Ok(new 
        { 
            user = new { user.Id, user.Name, user.Email, user.Role },
            token 
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var result = await _authService.LoginAsync(dto.Email, dto.Password);
        
        if (result == null)
        {
            return Unauthorized(new { message = "Correo o contraseña inválidos" });
        }

        var (user, token) = result.Value;
        return Ok(new 
        { 
            user = new { user.Id, user.Name, user.Email, user.Role },
            token 
        });
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
        {
            return Unauthorized();
        }

        int userId = int.Parse(userIdClaim);
        var user = await _authService.GetUserByIdAsync(userId);
        
        if (user == null)
        {
            return NotFound();
        }

        return Ok(new { user.Id, user.Name, user.Email, user.Role, user.Phone });
    }
}

public class RegisterDto
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public string? Phone { get; set; }
}

public class LoginDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
