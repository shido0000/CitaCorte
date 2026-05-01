using Microsoft.EntityFrameworkCore;
using CitaCorte.API.Data.Context;
using CitaCorte.API.Data.Entities;
using CitaCorte.API.Service.Interfaces;

namespace CitaCorte.API.Service.Implementations;

public class AdminService : IAdminService
{
    private readonly ApplicationDbContext _context;
    private readonly ISubscriptionService _subscriptionService;

    public AdminService(ApplicationDbContext context, ISubscriptionService subscriptionService)
    {
        _context = context;
        _subscriptionService = subscriptionService;
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await _context.Users
            .OrderByDescending(u => u.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Barbero>> GetAllBarberosAsync()
    {
        return await _context.Barberos
            .Include(b => b.User)
            .Include(b => b.Barberia)
            .OrderByDescending(b => b.User.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Barberia>> GetAllBarberiasAsync()
    {
        return await _context.Barberias
            .Include(b => b.User)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();
    }

    public async Task DeactivateUserAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) throw new Exception("Usuario no encontrado");

        user.IsActive = false;
        await _context.SaveChangesAsync();
    }

    public async Task ActivateUserAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) throw new Exception("Usuario no encontrado");

        user.IsActive = true;
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<SubscriptionPlan>> GetAllSubscriptionPlansAsync()
    {
        return await _context.SubscriptionPlans.ToListAsync();
    }

    public async Task<SubscriptionPlan> CreateSubscriptionPlanAsync(SubscriptionPlan plan)
    {
        return await _subscriptionService.CreatePlanAsync(plan);
    }

    public async Task UpdateSubscriptionPlanAsync(SubscriptionPlan plan)
    {
        await _subscriptionService.UpdatePlanAsync(plan);
    }

    public async Task DeleteSubscriptionPlanAsync(int planId)
    {
        var plan = await _context.SubscriptionPlans.FindAsync(planId);
        if (plan != null)
        {
            plan.IsActive = false;
            await _context.SaveChangesAsync();
        }
    }
}

public class ComercialService : IComercialService
{
    private readonly ApplicationDbContext _context;
    private readonly ISubscriptionService _subscriptionService;

    public ComercialService(ApplicationDbContext context, ISubscriptionService subscriptionService)
    {
        _context = context;
        _subscriptionService = subscriptionService;
    }

    public async Task<IEnumerable<Barbero>> GetBarberosPendingSubscriptionApprovalAsync()
    {
        return await _context.Barberos
            .Include(b => b.User)
            .Where(b => b.SubscriptionStatus == SubscriptionStatus.Pending && 
                       b.CurrentSubscription != SubscriptionType.Free)
            .ToListAsync();
    }

    public async Task<IEnumerable<Barberia>> GetBarberiasPendingSubscriptionApprovalAsync()
    {
        return await _context.Barberias
            .Include(b => b.User)
            .Where(b => b.SubscriptionStatus == SubscriptionStatus.Pending)
            .ToListAsync();
    }

    public async Task ApproveBarberoSubscriptionAsync(int barberoId, bool approved)
    {
        await _subscriptionService.ApproveSubscriptionChangeAsync(barberoId, approved);
    }

    public async Task ApproveBarberiaSubscriptionAsync(int barberiaId, bool approved)
    {
        await _subscriptionService.ApproveBarberiaSubscriptionAsync(barberiaId, approved);
    }

    public async Task<object> GetGeneralStatisticsAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        startDate ??= DateTime.UtcNow.AddDays(-30);
        endDate ??= DateTime.UtcNow;

        var totalUsers = await _context.Users.CountAsync();
        var totalBarberos = await _context.Barberos.CountAsync();
        var totalBarberias = await _context.Barberias.CountAsync();
        var totalClients = await _context.Users.CountAsync(u => u.Role == UserRole.Cliente);

        var totalReservations = await _context.Reservations
            .CountAsync(r => r.CreatedAt >= startDate && r.CreatedAt <= endDate);

        var confirmedReservations = await _context.Reservations
            .CountAsync(r => r.Status == ReservationStatus.Confirmed && 
                            r.CreatedAt >= startDate && r.CreatedAt <= endDate);

        var pendingBarberos = await _context.Barberos
            .CountAsync(b => b.SubscriptionStatus == SubscriptionStatus.Pending);

        var pendingBarberias = await _context.Barberias
            .CountAsync(b => b.SubscriptionStatus == SubscriptionStatus.Pending);

        return new
        {
            TotalUsers = totalUsers,
            TotalBarberos = totalBarberos,
            TotalBarberias = totalBarberias,
            TotalClients = totalClients,
            TotalReservations = totalReservations,
            ConfirmedReservations = confirmedReservations,
            PendingBarberosSubscription = pendingBarberos,
            PendingBarberiasSubscription = pendingBarberias,
            PeriodStart = startDate,
            PeriodEnd = endDate
        };
    }
}

public class ClientService : IClientService
{
    private readonly ApplicationDbContext _context;
    private readonly ISubscriptionService _subscriptionService;

    public ClientService(ApplicationDbContext context, ISubscriptionService subscriptionService)
    {
        _context = context;
        _subscriptionService = subscriptionService;
    }

    public async Task<IEnumerable<Barbero>> SearchAvailableBarberosAsync(DateTime? date = null, string? specialty = null)
    {
        var query = _context.Barberos
            .Include(b => b.User)
            .Include(b => b.Barberia)
            .Where(b => b.SubscriptionStatus == SubscriptionStatus.Active &&
                       b.AffiliationStatus == AffiliationStatus.Rejected); // Independent barberos

        if (!string.IsNullOrEmpty(specialty))
        {
            query = query.Where(b => b.Specialties != null && b.Specialties.Contains(specialty));
        }

        return await query.ToListAsync();
    }

    public async Task<IEnumerable<Barberia>> SearchAvailableBarberiasAsync(DateTime? date = null)
    {
        return await _context.Barberias
            .Include(b => b.User)
            .Where(b => b.IsActive && 
                       b.SubscriptionStatus == SubscriptionStatus.Active &&
                       (!b.SubscriptionEndDate.HasValue || b.SubscriptionEndDate > DateTime.UtcNow))
            .ToListAsync();
    }

    public async Task<IEnumerable<Service>> GetBarberoServicesAsync(int barberoId)
    {
        return await _context.Services
            .Where(s => s.BarberoId == barberoId && s.IsActive)
            .ToListAsync();
    }

    public async Task<IEnumerable<Service>> GetBarberiaServicesAsync(int barberiaId)
    {
        return await _context.Services
            .Where(s => s.BarberiaId == barberiaId && s.IsActive)
            .ToListAsync();
    }
}
