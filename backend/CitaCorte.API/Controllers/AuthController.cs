using Microsoft.AspNetCore.Mvc;
using CitaCorte.Data.Entities;
using CitaCorte.Service.Interfaces;

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
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var user = await _authService.RegisterAsync(
            request.Email,
            request.Password,
            request.FirstName,
            request.LastName,
            request.Phone,
            request.Role
        );

        if (user == null)
            return BadRequest(new { message = "El correo electrónico ya está registrado" });

        return Ok(new { message = "Usuario registrado exitosamente", userId = user.Id });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var token = await _authService.LoginAsync(request.Email, request.Password);

        if (token == null)
            return Unauthorized(new { message = "Credenciales inválidas" });

        return Ok(new { token });
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        // In production, extract userId from JWT token
        var userId = 1; // Placeholder
        var user = await _authService.GetUserByIdAsync(userId);

        if (user == null)
            return NotFound();

        return Ok(user);
    }
}

public class RegisterRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string? LastName { get; set; }
    public string Phone { get; set; } = string.Empty;
    public Role Role { get; set; }
}

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
