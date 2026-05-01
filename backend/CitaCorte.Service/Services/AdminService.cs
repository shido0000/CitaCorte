using Microsoft.EntityFrameworkCore;
using CitaCorte.Data.Context;
using CitaCorte.Data.Entities;
using CitaCorte.Service.Interfaces;

namespace CitaCorte.Service.Services;

public class AdminService : IAdminService
{
    private readonly CitaCorteDbContext _context;
    private readonly INotificationService _notificationService;

    public AdminService(CitaCorteDbContext context, INotificationService notificationService)
    {
        _context = context;
        _notificationService = notificationService;
    }

    public async Task<SubscriptionPlan> CreateSubscriptionPlanAsync(SubscriptionPlan plan)
    {
        plan.CreatedAt = DateTime.UtcNow;
        plan.IsActive = true;
        
        _context.SubscriptionPlans.Add(plan);
        await _context.SaveChangesAsync();
        
        return plan;
    }

    public async Task<IEnumerable<SubscriptionPlan>> GetAllSubscriptionPlansAsync()
    {
        return await _context.SubscriptionPlans
            .Where(sp => sp.IsActive)
            .ToListAsync();
    }

    public async Task<SubscriptionPlan?> UpdateSubscriptionPlanAsync(int planId, SubscriptionPlan plan)
    {
        var existingPlan = await _context.SubscriptionPlans.FindAsync(planId);
        if (existingPlan == null) return null;

        existingPlan.Name = plan.Name;
        existingPlan.Description = plan.Description;
        existingPlan.Price = plan.Price;
        existingPlan.DurationDays = plan.DurationDays;
        existingPlan.MaxAffiliatedBarbers = plan.MaxAffiliatedBarbers;
        existingPlan.CanReceiveReservations = plan.CanReceiveReservations;
        existingPlan.CanSellProducts = plan.CanSellProducts;
        existingPlan.HasStatistics = plan.HasStatistics;

        await _context.SaveChangesAsync();
        return existingPlan;
    }

    public async Task<bool> DeleteSubscriptionPlanAsync(int planId)
    {
        var plan = await _context.SubscriptionPlans.FindAsync(planId);
        if (plan == null) return false;

        plan.IsActive = false;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<Barbero>> GetAllBarberosAsync()
    {
        return await _context.Barberos
            .Include(b => b.User)
            .Include(b => b.Barberia)
            .Include(b => b.Services)
            .ToListAsync();
    }

    public async Task<IEnumerable<Barberia>> GetAllBarberiasAsync()
    {
        return await _context.Barberias
            .Include(b => b.User)
            .Include(b => b.AffiliatedBarbers)
            .Include(b => b.Services)
            .ToListAsync();
    }

    public async Task<Dictionary<string, object>> GetSystemStatisticsAsync()
    {
        var totalUsers = await _context.Users.CountAsync();
        var totalBarberos = await _context.Barberos.CountAsync();
        var totalBarberias = await _context.Barberias.CountAsync();
        var totalClientes = await _context.Clientes.CountAsync();
        var totalReservations = await _context.Reservations.CountAsync();
        var pendingSubscriptionChanges = await _context.BarberoSubscriptionChanges.CountAsync(sc => sc.Status == SubscriptionStatus.Pending) +
                                         await _context.BarberiaSubscriptionChanges.CountAsync(sc => sc.Status == SubscriptionStatus.Pending);

        return new Dictionary<string, object>
        {
            { "TotalUsers", totalUsers },
            { "TotalBarberos", totalBarberos },
            { "TotalBarberias", totalBarberias },
            { "TotalClientes", totalClientes },
            { "TotalReservations", totalReservations },
            { "PendingSubscriptionChanges", pendingSubscriptionChanges }
        };
    }

    public async Task<bool> ApproveBarberoSubscriptionChangeAsync(int changeId, int adminId)
    {
        var change = await _context.BarberoSubscriptionChanges
            .Include(c => c.Barbero)
            .Include(c => c.Barbero.User)
            .FirstOrDefaultAsync(c => c.Id == changeId);
            
        if (change == null) return false;

        change.Status = SubscriptionStatus.Approved;
        change.ReviewedAt = DateTime.UtcNow;
        change.ReviewedByAdminId = adminId;

        // Update barbero subscription
        var plan = await _context.SubscriptionPlans
            .FirstOrDefaultAsync(p => p.Type == change.RequestedSubscription && !p.IsForBarberia);
            
        if (plan != null)
        {
            change.Barbero.CurrentSubscription = change.RequestedSubscription;
            change.Barbero.SubscriptionEndDate = DateTime.UtcNow.AddDays(plan.DurationDays);
        }

        await _context.SaveChangesAsync();

        // Notify barbero
        await _notificationService.CreateNotificationAsync(
            change.Barbero.UserId,
            "Suscripción Aprobada",
            $"Tu cambio de suscripción a {change.RequestedSubscription} ha sido aprobado.",
            NotificationType.SubscriptionApproval,
            barberoSubscriptionChangeId: changeId
        );

        return true;
    }

    public async Task<bool> RejectBarberoSubscriptionChangeAsync(int changeId, int adminId, string reason)
    {
        var change = await _context.BarberoSubscriptionChanges
            .Include(c => c.Barbero)
            .Include(c => c.Barbero.User)
            .FirstOrDefaultAsync(c => c.Id == changeId);
            
        if (change == null) return false;

        change.Status = SubscriptionStatus.Rejected;
        change.ReviewedAt = DateTime.UtcNow;
        change.ReviewedByAdminId = adminId;
        change.RejectionReason = reason;

        await _context.SaveChangesAsync();

        // Notify barbero
        await _notificationService.CreateNotificationAsync(
            change.Barbero.UserId,
            "Suscripción Rechazada",
            $"Tu cambio de suscripción ha sido rechazado. Razón: {reason}",
            NotificationType.SubscriptionApproval,
            barberoSubscriptionChangeId: changeId
        );

        return true;
    }

    public async Task<bool> ApproveBarberiaSubscriptionChangeAsync(int changeId, int adminId)
    {
        var change = await _context.BarberiaSubscriptionChanges
            .Include(c => c.Barberia)
            .Include(c => c.Barberia.User)
            .FirstOrDefaultAsync(c => c.Id == changeId);
            
        if (change == null) return false;

        change.Status = SubscriptionStatus.Approved;
        change.ReviewedAt = DateTime.UtcNow;
        change.ReviewedByAdminId = adminId;

        // Update barberia subscription
        var plan = await _context.SubscriptionPlans
            .FirstOrDefaultAsync(p => p.Type == change.RequestedSubscription && p.IsForBarberia);
            
        if (plan != null)
        {
            change.Barberia.CurrentSubscription = change.RequestedSubscription;
            change.Barberia.SubscriptionEndDate = DateTime.UtcNow.AddDays(plan.DurationDays);
            change.Barberia.MaxAffiliatedBarbers = plan.MaxAffiliatedBarbers;
            change.Barberia.IsActive = true;
        }

        await _context.SaveChangesAsync();

        // Notify barberia
        await _notificationService.CreateNotificationAsync(
            change.Barberia.UserId,
            "Suscripción Aprobada",
            $"Tu cambio de suscripción a {change.RequestedSubscription} ha sido aprobado.",
            NotificationType.SubscriptionApproval,
            barberiaSubscriptionChangeId: changeId
        );

        return true;
    }

    public async Task<bool> RejectBarberiaSubscriptionChangeAsync(int changeId, int adminId, string reason)
    {
        var change = await _context.BarberiaSubscriptionChanges
            .Include(c => c.Barberia)
            .Include(c => c.Barberia.User)
            .FirstOrDefaultAsync(c => c.Id == changeId);
            
        if (change == null) return false;

        change.Status = SubscriptionStatus.Rejected;
        change.ReviewedAt = DateTime.UtcNow;
        change.ReviewedByAdminId = adminId;
        change.RejectionReason = reason;

        await _context.SaveChangesAsync();

        // Notify barberia
        await _notificationService.CreateNotificationAsync(
            change.Barberia.UserId,
            "Suscripción Rechazada",
            $"Tu cambio de suscripción ha sido rechazado. Razón: {reason}",
            NotificationType.SubscriptionApproval,
            barberiaSubscriptionChangeId: changeId
        );

        return true;
    }
}
