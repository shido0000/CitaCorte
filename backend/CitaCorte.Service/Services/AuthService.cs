using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using CitaCorte.Data.Context;
using CitaCorte.Data.Entities;
using CitaCorte.Service.Interfaces;

namespace CitaCorte.Service.Services;

public class AuthService : IAuthService
{
    private readonly CitaCorteDbContext _context;
    private readonly INotificationService _notificationService;

    public AuthService(CitaCorteDbContext context, INotificationService notificationService)
    {
        _context = context;
        _notificationService = notificationService;
    }

    public async Task<User?> RegisterAsync(string email, string password, string firstName, string? lastName, string phone, Role role)
    {
        // Check if email already exists
        if (await _context.Users.AnyAsync(u => u.Email == email))
            return null;

        var user = new User
        {
            Email = email,
            PasswordHash = HashPassword(password),
            FirstName = firstName,
            LastName = lastName,
            Phone = phone,
            Role = role,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Create role-specific entity
        switch (role)
        {
            case Role.Admin:
                _context.Admins.Add(new Admin { UserId = user.Id });
                break;
            case Role.Barbero:
                var barbero = new Barbero
                {
                    UserId = user.Id,
                    CurrentSubscription = SubscriptionType.Free,
                    SubscriptionStartDate = DateTime.UtcNow,
                    SubscriptionEndDate = DateTime.UtcNow.AddDays(30), // Default 30 days trial
                    AffiliationStatus = AffiliationStatus.Pending
                };
                _context.Barberos.Add(barbero);
                break;
            case Role.Barberia:
                // Barberia must select a subscription during registration - handled separately
                break;
            case Role.Comercial:
                _context.Comerciales.Add(new Comercial { UserId = user.Id });
                break;
            case Role.Cliente:
                _context.Clientes.Add(new Cliente { UserId = user.Id });
                break;
        }

        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<string?> LoginAsync(string email, string password)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null || !VerifyPassword(password, user.PasswordHash))
            return null;

        // Simple JWT token generation (in production, use proper JWT library)
        var token = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{user.Id}:{user.Email}:{DateTime.UtcNow.AddDays(7)}"));
        return token;
    }

    public async Task<User?> GetUserByIdAsync(int userId)
    {
        return await _context.Users
            .Include(u => u.Barbero)
            .Include(u => u.Barberia)
            .Include(u => u.Comercial)
            .Include(u => u.Cliente)
            .Include(u => u.Admin)
            .FirstOrDefaultAsync(u => u.Id == userId);
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _context.Users
            .Include(u => u.Barbero)
            .Include(u => u.Barberia)
            .Include(u => u.Comercial)
            .Include(u => u.Cliente)
            .Include(u => u.Admin)
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }

    private static bool VerifyPassword(string password, string hashedPassword)
    {
        var hashedBytes = HashPassword(password);
        return CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(hashedPassword),
            Encoding.UTF8.GetBytes(hashedBytes));
    }
}
