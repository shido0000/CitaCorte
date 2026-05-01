using Microsoft.EntityFrameworkCore;
using CitaCorte.API.Data.Context;
using CitaCorte.API.Data.Entities;
using CitaCorte.API.Service.Interfaces;

namespace CitaCorte.API.Service.Implementations;

public class BarberiaService : IBarberiaService
{
    private readonly ApplicationDbContext _context;
    private readonly ISubscriptionService _subscriptionService;
    private readonly INotificationService _notificationService;

    public BarberiaService(ApplicationDbContext context, ISubscriptionService subscriptionService, INotificationService notificationService)
    {
        _context = context;
        _subscriptionService = subscriptionService;
        _notificationService = notificationService;
    }

    public async Task<Barberia> GetBarberiaByUserIdAsync(int userId)
    {
        var barberia = await _context.Barberias
            .Include(b => b.User)
            .FirstOrDefaultAsync(b => b.UserId == userId);

        if (barberia == null) throw new Exception("Perfil de barbería no encontrado");
        return barberia;
    }

    public async Task<Barberia> CreateBarberiaAsync(int userId, string name, string? description, string? address, string? city, string? state, string? phone, string? logoUrl)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null || user.Role != UserRole.Barberia)
        {
            throw new Exception("El usuario no tiene rol de barbería");
        }

        var existingBarberia = await _context.Barberias.FirstOrDefaultAsync(b => b.UserId == userId);
        if (existingBarberia != null)
        {
            throw new Exception("El usuario ya tiene una barbería registrada");
        }

        var barberia = new Barberia
        {
            UserId = userId,
            Name = name,
            Description = description,
            Address = address,
            City = city,
            State = state,
            Phone = phone,
            LogoUrl = logoUrl,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        await _context.Barberias.AddAsync(barberia);
        await _context.SaveChangesAsync();

        return barberia;
    }

    public async Task<Barberia> UpdateBarberiaProfileAsync(int userId, string? name, string? description, string? address, string? city, string? state, string? phone, string? logoUrl)
    {
        var barberia = await GetBarberiaByUserIdAsync(userId);

        if (!string.IsNullOrEmpty(name)) barberia.Name = name;
        if (description != null) barberia.Description = description;
        if (address != null) barberia.Address = address;
        if (city != null) barberia.City = city;
        if (state != null) barberia.State = state;
        if (phone != null) barberia.Phone = phone;
        if (logoUrl != null) barberia.LogoUrl = logoUrl;

        await _context.SaveChangesAsync();
        return barberia;
    }

    // Affiliation management
    public async Task<IEnumerable<AffiliationRequest>> GetPendingAffiliationRequestsAsync(int barberiaUserId)
    {
        var barberia = await GetBarberiaByUserIdAsync(barberiaUserId);

        return await _context.AffiliationRequests
            .Include(ar => ar.Barbero)
                .ThenInclude(b => b.User)
            .Where(ar => ar.BarberiaId == barberia.Id && ar.Status == AffiliationStatus.Pending)
            .ToListAsync();
    }

    public async Task RespondToAffiliationRequestAsync(int barberiaUserId, int affiliationRequestId, bool accepted, string? rejectionReason)
    {
        var barberia = await GetBarberiaByUserIdAsync(barberiaUserId);

        var request = await _context.AffiliationRequests
            .Include(ar => ar.Barbero)
                .ThenInclude(b => b.User)
            .FirstOrDefaultAsync(ar => ar.Id == affiliationRequestId && ar.BarberiaId == barberia.Id);

        if (request == null) throw new Exception("Solicitud de afiliación no encontrada");

        if (accepted)
        {
            // Check max barberos limit
            var maxBarberos = await _subscriptionService.GetBarberiaMaxBarberosLimitAsync(barberia.Id);
            if (barberia.CurrentBarberosCount >= maxBarberos)
            {
                throw new Exception($"Se ha alcanzado el límite máximo de {maxBarberos} barberos afiliados");
            }

            request.Status = AffiliationStatus.Accepted;
            
            // Update barbero affiliation
            var barbero = await _context.Barberos.FindAsync(request.BarberoId);
            if (barbero != null)
            {
                // If barbero was affiliated to another barberia, decrement that count
                if (barbero.BarberiaId.HasValue && barbero.BarberiaId != barberia.Id)
                {
                    var oldBarberia = await _context.Barberias.FindAsync(barbero.BarberiaId.Value);
                    if (oldBarberia != null)
                    {
                        oldBarberia.CurrentBarberosCount--;
                    }
                }

                barbero.BarberiaId = barberia.Id;
                barbero.AffiliationStatus = AffiliationStatus.Accepted;
                barberia.CurrentBarberosCount++;
            }

            // Notify barbero
            await _notificationService.CreateNotificationAsync(
                request.Barbero.UserId,
                NotificationType.AffiliationResponse,
                "Solicitud de afiliación aceptada",
                $"Tu solicitud para unirte a {barberia.Name} ha sido aceptada"
            );
        }
        else
        {
            request.Status = AffiliationStatus.Rejected;
            request.RejectionReason = rejectionReason;

            // Notify barbero
            await _notificationService.CreateNotificationAsync(
                request.Barbero.UserId,
                NotificationType.AffiliationResponse,
                "Solicitud de afiliación rechazada",
                rejectionReason ?? "Tu solicitud de afiliación ha sido rechazada"
            );
        }

        request.RespondedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Barbero>> GetAffiliatedBarberosAsync(int barberiaId)
    {
        return await _context.Barberos
            .Include(b => b.User)
            .Where(b => b.BarberiaId == barberiaId && b.AffiliationStatus == AffiliationStatus.Accepted)
            .ToListAsync();
    }

    public async Task RemoveBarberoAsync(int barberiaId, int barberoId)
    {
        var barberia = await _context.Barberias.FindAsync(barberiaId);
        if (barberia == null) throw new Exception("Barbería no encontrada");

        var barbero = await _context.Barberos.FindAsync(barberoId);
        if (barbero == null || barbero.BarberiaId != barberiaId) 
            throw new Exception("El barbero no está afiliado a esta barbería");

        barbero.BarberiaId = null;
        barbero.AffiliationStatus = AffiliationStatus.Rejected;
        barberia.CurrentBarberosCount--;

        await _context.SaveChangesAsync();
    }

    // Service management
    public async Task<Service> AddServiceAsync(int barberiaId, Service service)
    {
        var barberia = await _context.Barberias.FindAsync(barberiaId);
        if (barberia == null) throw new Exception("Barbería no encontrada");

        service.BarberiaId = barberiaId;
        service.CreatedAt = DateTime.UtcNow;

        await _context.Services.AddAsync(service);
        await _context.SaveChangesAsync();
        return service;
    }

    public async Task UpdateServiceAsync(Service service)
    {
        var existingService = await _context.Services.FindAsync(service.Id);
        if (existingService == null || existingService.BarberiaId == null) 
            throw new Exception("Servicio no encontrado");

        existingService.Name = service.Name;
        existingService.Description = service.Description;
        existingService.Price = service.Price;
        existingService.DurationMinutes = service.DurationMinutes;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteServiceAsync(int serviceId)
    {
        var service = await _context.Services.FindAsync(serviceId);
        if (service != null)
        {
            service.IsActive = false;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Service>> GetBarberiaServicesAsync(int barberiaId)
    {
        return await _context.Services
            .Where(s => s.BarberiaId == barberiaId && s.IsActive)
            .ToListAsync();
    }

    // Statistics
    public async Task<BarberiaStatistic?> GetBarberiaStatisticsAsync(int barberiaId, DateTime? startDate = null, DateTime? endDate = null)
    {
        startDate ??= DateTime.UtcNow.AddDays(-30);
        endDate ??= DateTime.UtcNow;

        var reservations = await _context.Reservations
            .Where(r => r.BarberiaId == barberiaId &&
                        r.CreatedAt >= startDate &&
                        r.CreatedAt <= endDate)
            .ToListAsync();

        var affiliatedBarberos = await _context.Barberos
            .Where(b => b.BarberiaId == barberiaId && b.AffiliationStatus == AffiliationStatus.Accepted)
            .ToListAsync();

        var statistic = new BarberiaStatistic
        {
            BarberiaId = barberiaId,
            Date = DateTime.UtcNow.Date,
            TotalReservations = reservations.Count,
            ConfirmedReservations = reservations.Count(r => r.Status == ReservationStatus.Confirmed),
            CancelledReservations = reservations.Count(r => r.Status == ReservationStatus.Cancelled),
            CompletedReservations = reservations.Count(r => r.Status == ReservationStatus.Completed),
            TotalRevenue = reservations.Where(r => r.Status == ReservationStatus.Completed).Sum(r => r.Service.Price),
            TotalBarberos = affiliatedBarberos.Count,
            ActiveBarberos = affiliatedBarberos.Count(b => b.SubscriptionStatus == SubscriptionStatus.Active),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        return statistic;
    }
}
