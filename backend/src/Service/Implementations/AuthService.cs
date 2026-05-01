using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using CitaCorte.API.Data.Context;
using CitaCorte.API.Data.Entities;
using CitaCorte.API.Service.Interfaces;

namespace CitaCorte.API.Service.Implementations;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly INotificationService _notificationService;

    public AuthService(ApplicationDbContext context, IConfiguration configuration, INotificationService notificationService)
    {
        _context = context;
        _configuration = configuration;
        _notificationService = notificationService;
    }

    public async Task<(User user, string token)?> RegisterAsync(string name, string email, string password, UserRole role, string? phone = null)
    {
        // Check if email already exists
        if (await _context.Users.AnyAsync(u => u.Email == email))
        {
            return null;
        }

        var user = new User
        {
            Name = name,
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            Role = role,
            Phone = phone,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        // Create profile based on role
        if (role == UserRole.Barbero)
        {
            var barbero = new Barbero
            {
                UserId = user.Id,
                CurrentSubscription = SubscriptionType.Free,
                SubscriptionStatus = SubscriptionStatus.Active, // Free plan is auto-approved
                SubscriptionStartDate = DateTime.UtcNow,
                AffiliationStatus = AffiliationStatus.Rejected
            };
            await _context.Barberos.AddAsync(barbero);
            await _context.SaveChangesAsync();
        }
        else if (role == UserRole.Barberia)
        {
            // Barberia needs subscription approval - handled separately
            // Just create the basic record, subscription must be selected during registration
        }
        else if (role == UserRole.Cliente)
        {
            // Welcome notification for new clients
            await _notificationService.CreateNotificationAsync(
                user.Id, 
                NotificationType.Info, 
                "¡Bienvenido a CitaCorte!", 
                "Gracias por registrarte. Ahora puedes buscar barberos y barberías para reservar tus citas."
            );
        }

        var token = GenerateJwtToken(user);
        return (user, token);
    }

    public async Task<(User user, string token)?> LoginAsync(string email, string password)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email);

        if (user == null || !user.IsActive || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            return null;
        }

        var token = GenerateJwtToken(user);
        return (user, token);
    }

    public async Task<User?> GetUserByIdAsync(int userId)
    {
        return await _context.Users.FindAsync(userId);
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    private string GenerateJwtToken(User user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"] ?? "default-secret-key-change-in-production";
        var issuer = jwtSettings["Issuer"] ?? "CitaCorte";
        var audience = jwtSettings["Audience"] ?? "CitaCorteUsers";

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
