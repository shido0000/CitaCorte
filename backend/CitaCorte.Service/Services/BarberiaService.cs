using Microsoft.EntityFrameworkCore;
using CitaCorte.Data.Context;
using CitaCorte.Data.Entities;
using CitaCorte.Service.Interfaces;

namespace CitaCorte.Service.Services;

public class BarberiaService : IBarberiaService
{
    private readonly CitaCorteDbContext _context;
    private readonly INotificationService _notificationService;

    public BarberiaService(CitaCorteDbContext context, INotificationService notificationService)
    {
        _context = context;
        _notificationService = notificationService;
    }

    public async Task<Barberia> CreateBarberiaAsync(User user, string name, string description, string address, decimal latitude, decimal longitude, string phone, SubscriptionType subscriptionType)
    {
        var barberia = new Barberia
        {
            UserId = user.Id,
            Name = name,
            Description = description,
            Address = address,
            Latitude = latitude,
            Longitude = longitude,
            Phone = phone,
            CurrentSubscription = subscriptionType,
            SubscriptionStartDate = DateTime.UtcNow,
            SubscriptionEndDate = DateTime.UtcNow.AddDays(30), // Temporal hasta aprobación
            SubscriptionStatus = SubscriptionStatus.Pending,
            MaxAffiliatedBarbers = 5, // Default until plan is approved
            IsActive = false // Will be activated after subscription approval
        };

        _context.Barberias.Add(barberia);
        await _context.SaveChangesAsync();

        // Notify admin and comercial about new barberia registration
        var admins = await _context.Admins.Include(a => a.User).ToListAsync();
        var comerciales = await _context.Comerciales.Include(c => c.User).ToListAsync();

        foreach (var admin in admins)
        {
            await _notificationService.CreateNotificationAsync(
                admin.UserId,
                "Nueva Barbería Registrada",
                $"La barbería {name} ha solicitado una suscripción {subscriptionType}",
                NotificationType.SubscriptionApproval
            );
        }

        foreach (var comercial in comerciales)
        {
            await _notificationService.CreateNotificationAsync(
                comercial.UserId,
                "Nueva Barbería Registrada",
                $"La barbería {name} ha solicitado una suscripción {subscriptionType}",
                NotificationType.SubscriptionApproval
            );
        }

        return barberia;
    }

    public async Task<Barberia?> GetBarberiaByIdAsync(int barberiaId)
    {
        return await _context.Barberias
            .Include(b => b.User)
            .Include(b => b.AffiliatedBarbers)
            .ThenInclude(ab => ab.User)
            .Include(b => b.Services)
            .FirstOrDefaultAsync(b => b.Id == barberiaId);
    }

    public async Task<Barberia?> GetBarberiaByUserIdAsync(int userId)
    {
        return await _context.Barberias
            .Include(b => b.User)
            .Include(b => b.AffiliatedBarbers)
            .Include(b => b.Services)
            .FirstOrDefaultAsync(b => b.UserId == userId);
    }

    public async Task<Barberia> UpdateBarberiaAsync(int barberiaId, string? description, string? profileImageUrl)
    {
        var barberia = await _context.Barberias.FindAsync(barberiaId);
        if (barberia == null) throw new Exception("Barbería no encontrada");

        barberia.Description = description;
        barberia.ProfileImageUrl = profileImageUrl;

        await _context.SaveChangesAsync();
        return barberia;
    }

    public async Task<IEnumerable<Barbero>> GetAffiliatedBarbersAsync(int barberiaId)
    {
        return await _context.Barberos
            .Include(b => b.User)
            .Where(b => b.BarberiaId == barberiaId && b.AffiliationStatus == AffiliationStatus.Approved)
            .ToListAsync();
    }

    public async Task<bool> ApproveAffiliationRequestAsync(int barberiaId, int barberoId)
    {
        var barberia = await _context.Barberias.FindAsync(barberiaId);
        if (barberia == null || !barberia.IsActive)
            throw new Exception("Barbería no encontrada o no activa");

        // Check if barberia has reached max affiliated barbers
        var currentAffiliatedCount = await _context.Barberos
            .CountAsync(b => b.BarberiaId == barberiaId && b.AffiliationStatus == AffiliationStatus.Approved);

        if (currentAffiliatedCount >= barberia.MaxAffiliatedBarbers)
            throw new Exception("La barbería ha alcanzado el límite máximo de barberos afiliados");

        var barbero = await _context.Barberos
            .Include(b => b.User)
            .FirstOrDefaultAsync(b => b.Id == barberoId);

        if (barbero == null) throw new Exception("Barbero no encontrado");

        barbero.AffiliationStatus = AffiliationStatus.Approved;
        barbero.AffiliationApprovedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        // Notify barbero
        await _notificationService.CreateNotificationAsync(
            barbero.UserId,
            "Afiliación Aprobada",
            $"Tu solicitud de afiliación a {barberia.Name} ha sido aprobada. Las reservas ahora serán redirigidas a la barbería.",
            NotificationType.AffiliationResponse
        );

        return true;
    }

    public async Task<bool> RejectAffiliationRequestAsync(int barberiaId, int barberoId, string? reason = null)
    {
        var barbero = await _context.Barberos
            .Include(b => b.User)
            .FirstOrDefaultAsync(b => b.Id == barberoId);

        if (barbero == null) throw new Exception("Barbero no encontrado");

        barbero.AffiliationStatus = AffiliationStatus.Rejected;
        barbero.BarberiaId = null;

        await _context.SaveChangesAsync();

        // Notify barbero
        await _notificationService.CreateNotificationAsync(
            barbero.UserId,
            "Afiliación Rechazada",
            $"Tu solicitud de afiliación ha sido rechazada.{(reason != null ? " Razón: " + reason : "")}",
            NotificationType.AffiliationResponse
        );

        return true;
    }

    public async Task<IEnumerable<Service>> GetBarberiaServicesAsync(int barberiaId)
    {
        return await _context.Services
            .Where(s => s.BarberiaId == barberiaId && s.IsActive)
            .ToListAsync();
    }

    public async Task<Service> AddServiceAsync(int barberiaId, Service service)
    {
        var barberia = await _context.Barberias.FindAsync(barberiaId);
        if (barberia == null) throw new Exception("Barbería no encontrada");

        service.BarberiaId = barberiaId;
        _context.Services.Add(service);
        await _context.SaveChangesAsync();

        return service;
    }

    public async Task<bool> UpdateServiceAsync(int serviceId, Service service)
    {
        var existingService = await _context.Services.FindAsync(serviceId);
        if (existingService == null) return false;

        existingService.Name = service.Name;
        existingService.Description = service.Description;
        existingService.Price = service.Price;
        existingService.DurationMinutes = service.DurationMinutes;
        existingService.IsActive = service.IsActive;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteServiceAsync(int serviceId)
    {
        var service = await _context.Services.FindAsync(serviceId);
        if (service == null) return false;

        service.IsActive = false;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RequestSubscriptionChangeAsync(int barberiaId, SubscriptionType newSubscription)
    {
        var barberia = await _context.Barberias
            .Include(b => b.User)
            .FirstOrDefaultAsync(b => b.Id == barberiaId);

        if (barberia == null) throw new Exception("Barbería no encontrada");

        if (newSubscription == barberia.CurrentSubscription)
            throw new Exception("Ya tienes esta suscripción");

        var changeRequest = new BarberiaSubscriptionChange
        {
            BarberiaId = barberiaId,
            RequestedSubscription = newSubscription,
            Status = SubscriptionStatus.Pending,
            RequestedAt = DateTime.UtcNow
        };

        _context.BarberiaSubscriptionChanges.Add(changeRequest);
        await _context.SaveChangesAsync();

        // Notify admin and comercial
        var admins = await _context.Admins.Include(a => a.User).ToListAsync();
        var comerciales = await _context.Comerciales.Include(c => c.User).ToListAsync();

        foreach (var admin in admins)
        {
            await _notificationService.CreateNotificationAsync(
                admin.UserId,
                "Cambio de Suscripción Pendiente",
                $"La barbería {barberia.Name} ha solicitado cambiar a {newSubscription}",
                NotificationType.SubscriptionChange,
                barberiaSubscriptionChangeId: changeRequest.Id
            );
        }

        foreach (var comercial in comerciales)
        {
            await _notificationService.CreateNotificationAsync(
                comercial.UserId,
                "Cambio de Suscripción Pendiente",
                $"La barbería {barberia.Name} ha solicitado cambiar a {newSubscription}",
                NotificationType.SubscriptionChange,
                barberiaSubscriptionChangeId: changeRequest.Id
            );
        }

        return true;
    }

    public async Task<Dictionary<string, object>> GetBarberiaStatisticsAsync(int barberiaId)
    {
        var barberia = await _context.Barberias.FindAsync(barberiaId);
        if (barberia == null) throw new Exception("Barbería no encontrada");

        var reservations = await _context.Reservations
            .Where(r => r.BarberiaId == barberiaId)
            .ToListAsync();

        var totalReservations = reservations.Count;
        var completedReservations = reservations.Count(r => r.Status == ReservationStatus.Completed);
        var cancelledReservations = reservations.Count(r => r.Status == ReservationStatus.Cancelled);
        var totalRevenue = reservations.Where(r => r.Status == ReservationStatus.Completed).Sum(r => r.Service.Price);

        var affiliatedBarbersCount = await _context.Barberos
            .CountAsync(b => b.BarberiaId == barberiaId && b.AffiliationStatus == AffiliationStatus.Approved);

        return new Dictionary<string, object>
        {
            { "TotalReservations", totalReservations },
            { "CompletedReservations", completedReservations },
            { "CancelledReservations", cancelledReservations },
            { "TotalRevenue", totalRevenue },
            { "AffiliatedBarbersCount", affiliatedBarbersCount }
        };
    }

    public async Task<bool> IsActiveAsync(int barberiaId)
    {
        var barberia = await _context.Barberias.FindAsync(barberiaId);
        return barberia != null && barberia.IsActive && barberia.SubscriptionEndDate > DateTime.UtcNow;
    }

    public async Task<int> GetAffiliatedBarbersCountAsync(int barberiaId)
    {
        return await _context.Barberos
            .CountAsync(b => b.BarberiaId == barberiaId && b.AffiliationStatus == AffiliationStatus.Approved);
    }
}
