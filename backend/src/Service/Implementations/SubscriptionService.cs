using Microsoft.EntityFrameworkCore;
using CitaCorte.API.Data.Context;
using CitaCorte.API.Data.Entities;
using CitaCorte.API.Service.Interfaces;

namespace CitaCorte.API.Service.Implementations;

public class SubscriptionService : ISubscriptionService
{
    private readonly ApplicationDbContext _context;
    private readonly INotificationService _notificationService;

    public SubscriptionService(ApplicationDbContext context, INotificationService notificationService)
    {
        _context = context;
        _notificationService = notificationService;
    }

    public async Task<SubscriptionPlan> CreatePlanAsync(SubscriptionPlan plan)
    {
        plan.CreatedAt = DateTime.UtcNow;
        await _context.SubscriptionPlans.AddAsync(plan);
        await _context.SaveChangesAsync();
        return plan;
    }

    public async Task<IEnumerable<SubscriptionPlan>> GetAllPlansAsync()
    {
        return await _context.SubscriptionPlans.Where(p => p.IsActive).ToListAsync();
    }

    public async Task<SubscriptionPlan?> GetPlanByTypeAsync(SubscriptionType type)
    {
        return await _context.SubscriptionPlans.FirstOrDefaultAsync(p => p.Type == type && p.IsActive);
    }

    public async Task UpdatePlanAsync(SubscriptionPlan plan)
    {
        plan.UpdatedAt = DateTime.UtcNow;
        _context.SubscriptionPlans.Update(plan);
        await _context.SaveChangesAsync();
    }

    // Barbero subscription management
    public async Task RequestSubscriptionChangeAsync(int barberoId, SubscriptionType newType)
    {
        var barbero = await _context.Barberos
            .Include(b => b.User)
            .FirstOrDefaultAsync(b => b.Id == barberoId);

        if (barbero == null) throw new Exception("Barbero no encontrado");

        // Free plan doesn't need approval
        if (newType == SubscriptionType.Free)
        {
            barbero.CurrentSubscription = SubscriptionType.Free;
            barbero.SubscriptionStatus = SubscriptionStatus.Active;
            await _context.SaveChangesAsync();
            return;
        }

        // For Popular and Premium, request approval
        barbero.SubscriptionStatus = SubscriptionStatus.Pending;
        await _context.SaveChangesAsync();

        // Notify admin and comercial
        var admins = await _context.Users.Where(u => u.Role == UserRole.Admin).ToListAsync();
        var comerciales = await _context.Users.Where(u => u.Role == UserRole.Comercial).ToListAsync();

        foreach (var admin in admins)
        {
            await _notificationService.CreateNotificationAsync(
                admin.Id,
                NotificationType.SubscriptionChange,
                "Solicitud de cambio de suscripción",
                $"El barbero {barbero.User.Name} solicita cambiar a plan {newType}",
                affiliationRequestId: barberoId
            );
        }

        foreach (var comercial in comerciales)
        {
            await _notificationService.CreateNotificationAsync(
                comercial.Id,
                NotificationType.SubscriptionChange,
                "Solicitud de cambio de suscripción",
                $"El barbero {barbero.User.Name} solicita cambiar a plan {newType}",
                affiliationRequestId: barberoId
            );
        }
    }

    public async Task ApproveSubscriptionChangeAsync(int barberoId, bool approved, int? approvedByUserId = null)
    {
        var barbero = await _context.Barberos
            .Include(b => b.User)
            .FirstOrDefaultAsync(b => b.Id == barberoId);

        if (barbero == null) throw new Exception("Barbero no encontrado");

        if (approved)
        {
            var plan = await GetPlanByTypeAsync(barbero.CurrentSubscription);
            barbero.SubscriptionStatus = SubscriptionStatus.Active;
            barbero.SubscriptionStartDate = DateTime.UtcNow;
            
            if (plan != null && plan.DurationDays > 0)
            {
                barbero.SubscriptionEndDate = DateTime.UtcNow.AddDays(plan.DurationDays);
            }

            await _notificationService.CreateNotificationAsync(
                barbero.UserId,
                NotificationType.SubscriptionApproval,
                "Suscripción aprobada",
                $"Tu suscripción {barbero.CurrentSubscription} ha sido aprobada. ¡Disfruta de las nuevas funcionalidades!"
            );
        }
        else
        {
            barbero.SubscriptionStatus = SubscriptionStatus.Rejected;
            
            await _notificationService.CreateNotificationAsync(
                barbero.UserId,
                NotificationType.Error,
                "Suscripción rechazada",
                "Tu solicitud de cambio de suscripción ha sido rechazada. Contacta con soporte para más información."
            );
        }

        await _context.SaveChangesAsync();
    }

    public async Task<bool> CanBarberoReceiveReservationsAsync(int barberoId)
    {
        var barbero = await _context.Barberos.FindAsync(barberoId);
        if (barbero == null) return false;

        return barbero.CurrentSubscription != SubscriptionType.Free 
               && barbero.SubscriptionStatus == SubscriptionStatus.Active
               && (!barbero.SubscriptionEndDate.HasValue || barbero.SubscriptionEndDate > DateTime.UtcNow);
    }

    public async Task<bool> CanBarberoSellProductsAsync(int barberoId)
    {
        var barbero = await _context.Barberos.FindAsync(barberoId);
        if (barbero == null) return false;

        return barbero.CurrentSubscription == SubscriptionType.Premium
               && barbero.SubscriptionStatus == SubscriptionStatus.Active
               && (!barbero.SubscriptionEndDate.HasValue || barbero.SubscriptionEndDate > DateTime.UtcNow);
    }

    public async Task<bool> CanBarberoViewStatisticsAsync(int barberoId)
    {
        var barbero = await _context.Barberos.FindAsync(barberoId);
        if (barbero == null) return false;

        return (barbero.CurrentSubscription == SubscriptionType.Popular || 
                barbero.CurrentSubscription == SubscriptionType.Premium)
               && barbero.SubscriptionStatus == SubscriptionStatus.Active
               && (!barbero.SubscriptionEndDate.HasValue || barbero.SubscriptionEndDate > DateTime.UtcNow);
    }

    // Barberia subscription management
    public async Task RequestBarberiaSubscriptionAsync(int barberiaId, SubscriptionType type)
    {
        var barberia = await _context.Barberias
            .Include(b => b.User)
            .FirstOrDefaultAsync(b => b.Id == barberiaId);

        if (barberia == null) throw new Exception("Barbería no encontrada");

        // No free plan for barberias
        if (type == SubscriptionType.Free)
        {
            throw new Exception("Las barberías no pueden tener plan Free");
        }

        barberia.CurrentSubscription = type;
        barberia.SubscriptionStatus = SubscriptionStatus.Pending;
        await _context.SaveChangesAsync();

        // Notify admin and comercial
        var admins = await _context.Users.Where(u => u.Role == UserRole.Admin).ToListAsync();
        var comerciales = await _context.Users.Where(u => u.Role == UserRole.Comercial).ToListAsync();

        foreach (var admin in admins)
        {
            await _notificationService.CreateNotificationAsync(
                admin.Id,
                NotificationType.SubscriptionChange,
                "Nueva suscripción de barbería",
                $"La barbería {barberia.Name} solicita suscripción {type}",
                affiliationRequestId: barberiaId
            );
        }

        foreach (var comercial in comerciales)
        {
            await _notificationService.CreateNotificationAsync(
                comercial.Id,
                NotificationType.SubscriptionChange,
                "Nueva suscripción de barbería",
                $"La barbería {barberia.Name} solicita suscripción {type}",
                affiliationRequestId: barberiaId
            );
        }
    }

    public async Task ApproveBarberiaSubscriptionAsync(int barberiaId, bool approved, int? approvedByUserId = null)
    {
        var barberia = await _context.Barberias
            .Include(b => b.User)
            .FirstOrDefaultAsync(b => b.Id == barberiaId);

        if (barberia == null) throw new Exception("Barbería no encontrada");

        if (approved)
        {
            var plan = await GetPlanByTypeAsync(barberia.CurrentSubscription);
            barberia.SubscriptionStatus = SubscriptionStatus.Active;
            barberia.SubscriptionStartDate = DateTime.UtcNow;
            
            if (plan != null && plan.DurationDays > 0)
            {
                barberia.SubscriptionEndDate = DateTime.UtcNow.AddDays(plan.DurationDays);
            }
            
            // Set max barberos limit based on plan
            if (plan?.MaxBarberosLimit.HasValue == true)
            {
                barberia.MaxBarberos = plan.MaxBarberosLimit.Value;
            }

            await _notificationService.CreateNotificationAsync(
                barberia.UserId,
                NotificationType.SubscriptionApproval,
                "Suscripción aprobada",
                $"La suscripción de tu barbería ha sido aprobada. ¡Ahora puedes afiliar barberos!"
            );
        }
        else
        {
            barberia.SubscriptionStatus = SubscriptionStatus.Rejected;
            
            await _notificationService.CreateNotificationAsync(
                barberia.UserId,
                NotificationType.Error,
                "Suscripción rechazada",
                "La solicitud de suscripción de tu barbería ha sido rechazada. Contacta con soporte."
            );
        }

        await _context.SaveChangesAsync();
    }

    public async Task<bool> CanBarberiaAffiliateBarberosAsync(int barberiaId)
    {
        var barberia = await _context.Barberias.FindAsync(barberiaId);
        if (barberia == null) return false;

        return barberia.SubscriptionStatus == SubscriptionStatus.Active
               && (!barberia.SubscriptionEndDate.HasValue || barberia.SubscriptionEndDate > DateTime.UtcNow);
    }

    public async Task<int> GetBarberiaMaxBarberosLimitAsync(int barberiaId)
    {
        var barberia = await _context.Barberias.FindAsync(barberiaId);
        if (barberia == null) return 0;

        return barberia.MaxBarberos;
    }

    public async Task<bool> IsBarberiaSubscriptionActiveAsync(int barberiaId)
    {
        var barberia = await _context.Barberias.FindAsync(barberiaId);
        if (barberia == null) return false;

        return barberia.SubscriptionStatus == SubscriptionStatus.Active
               && (!barberia.SubscriptionEndDate.HasValue || barberia.SubscriptionEndDate > DateTime.UtcNow);
    }
}
