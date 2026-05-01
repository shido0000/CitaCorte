using Microsoft.EntityFrameworkCore;
using CitaCorte.Data.Context;
using CitaCorte.Data.Entities;
using CitaCorte.Service.Interfaces;

namespace CitaCorte.Service.Services;

public class NotificationService : INotificationService
{
    private readonly CitaCorteDbContext _context;

    public NotificationService(CitaCorteDbContext context)
    {
        _context = context;
    }

    public async Task<Notification> CreateNotificationAsync(int userId, string title, string message, NotificationType type, int? reservationId = null, int? barberoSubscriptionChangeId = null, int? barberiaSubscriptionChangeId = null)
    {
        var notification = new Notification
        {
            UserId = userId,
            Title = title,
            Message = message,
            Type = type,
            ReservationId = reservationId,
            BarberoSubscriptionChangeId = barberoSubscriptionChangeId,
            BarberiaSubscriptionChangeId = barberiaSubscriptionChangeId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();

        return notification;
    }

    public async Task<IEnumerable<Notification>> GetUserNotificationsAsync(int userId)
    {
        return await _context.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }

    public async Task<bool> MarkNotificationAsReadAsync(int notificationId)
    {
        var notification = await _context.Notifications.FindAsync(notificationId);
        if (notification == null) return false;

        notification.IsRead = true;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> MarkAllNotificationsAsReadAsync(int userId)
    {
        var notifications = await _context.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .ToListAsync();

        foreach (var notification in notifications)
        {
            notification.IsRead = true;
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<int> GetUnreadNotificationsCountAsync(int userId)
    {
        return await _context.Notifications
            .CountAsync(n => n.UserId == userId && !n.IsRead);
    }
}

public class ReservationService : IReservationService
{
    private readonly CitaCorteDbContext _context;

    public ReservationService(CitaCorteDbContext context)
    {
        _context = context;
    }

    public async Task<Reservation?> GetReservationByIdAsync(int reservationId)
    {
        return await _context.Reservations
            .Include(r => r.Cliente).ThenInclude(c => c.User)
            .Include(r => r.Service)
            .Include(r => r.Barbero)
            .Include(r => r.Barberia)
            .FirstOrDefaultAsync(r => r.Id == reservationId);
    }

    public async Task<IEnumerable<Reservation>> GetReservationsByBarberoAsync(int barberoId, ReservationStatus? status = null)
    {
        var query = _context.Reservations
            .Include(r => r.Cliente).ThenInclude(c => c.User)
            .Include(r => r.Service)
            .Where(r => r.BarberoId == barberoId);

        if (status.HasValue)
        {
            query = query.Where(r => r.Status == status.Value);
        }

        return await query.OrderByDescending(r => r.StartDateTime).ToListAsync();
    }

    public async Task<IEnumerable<Reservation>> GetReservationsByBarberiaAsync(int barberiaId, ReservationStatus? status = null)
    {
        var query = _context.Reservations
            .Include(r => r.Cliente).ThenInclude(c => c.User)
            .Include(r => r.Service)
            .Include(r => r.Barbero)
            .Where(r => r.BarberiaId == barberiaId);

        if (status.HasValue)
        {
            query = query.Where(r => r.Status == status.Value);
        }

        return await query.OrderByDescending(r => r.StartDateTime).ToListAsync();
    }

    public async Task<bool> HasOverlappingReservationAsync(int? barberoId, int? barberiaId, DateTime startDateTime, DateTime endDateTime, int? excludeReservationId = null)
    {
        var query = _context.Reservations
            .Where(r => r.Status != ReservationStatus.Cancelled && r.Status != ReservationStatus.Rejected);

        if (barberoId.HasValue)
        {
            query = query.Where(r => r.BarberoId == barberoId.Value);
        }
        else if (barberiaId.HasValue)
        {
            query = query.Where(r => r.BarberiaId == barberiaId.Value);
        }

        if (excludeReservationId.HasValue)
        {
            query = query.Where(r => r.Id != excludeReservationId.Value);
        }

        // Check for overlapping reservations
        return await query.AnyAsync(r =>
            (r.StartDateTime < endDateTime && r.EndDateTime > startDateTime));
    }
}

public class ClienteService : IClienteService
{
    private readonly CitaCorteDbContext _context;
    private readonly INotificationService _notificationService;
    private readonly IReservationService _reservationService;

    public ClienteService(CitaCorteDbContext context, INotificationService notificationService, IReservationService reservationService)
    {
        _context = context;
        _notificationService = notificationService;
        _reservationService = reservationService;
    }

    public async Task<Cliente> CreateClienteAsync(User user)
    {
        var cliente = new Cliente { UserId = user.Id };
        _context.Clientes.Add(cliente);
        await _context.SaveChangesAsync();
        return cliente;
    }

    public async Task<Cliente?> GetClienteByUserIdAsync(int userId)
    {
        return await _context.Clientes
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.UserId == userId);
    }

    public async Task<IEnumerable<Barbero>> SearchBarberosAsync(decimal? latitude, decimal? longitude, string? searchTerm)
    {
        var query = _context.Barberos
            .Include(b => b.User)
            .Include(b => b.Services)
            .Include(b => b.Barberia)
            .Where(b => b.CanReceiveReservations && b.SubscriptionEndDate > DateTime.UtcNow);

        if (!string.IsNullOrEmpty(searchTerm))
        {
            query = query.Where(b => 
                b.User.FirstName.Contains(searchTerm) ||
                (b.User.LastName != null && b.User.LastName.Contains(searchTerm)) ||
                (b.Bio != null && b.Bio.Contains(searchTerm)));
        }

        // If barbero is affiliated to a barberia, the reservation will be redirected
        return await query.ToListAsync();
    }

    public async Task<IEnumerable<Barberia>> SearchBarberiasAsync(decimal? latitude, decimal? longitude, string? searchTerm)
    {
        var query = _context.Barberias
            .Include(b => b.User)
            .Include(b => b.Services)
            .Where(b => b.IsActive && b.SubscriptionEndDate > DateTime.UtcNow);

        if (!string.IsNullOrEmpty(searchTerm))
        {
            query = query.Where(b => 
                b.Name.Contains(searchTerm) ||
                (b.Description != null && b.Description.Contains(searchTerm)));
        }

        return await query.ToListAsync();
    }

    public async Task<Reservation> CreateReservationAsync(int clienteId, int serviceId, DateTime startDateTime, int? barberoId, int? barberiaId, string? notes)
    {
        var service = await _context.Services.FindAsync(serviceId);
        if (service == null) throw new Exception("Servicio no encontrado");

        var endDateTime = startDateTime.AddMinutes(service.DurationMinutes);

        // Check if time slot is available
        bool hasOverlap;
        if (barberoId.HasValue)
        {
            // Check if barbero can receive reservations
            var barbero = await _context.Barberos.FindAsync(barberoId.Value);
            if (barbero == null || !barbero.CanReceiveReservations)
                throw new Exception("El barbero no puede recibir reservas");

            // If barbero is affiliated to a barberia, redirect to barberia
            if (barbero.AffiliationStatus == AffiliationStatus.Approved && barbero.BarberiaId.HasValue)
            {
                barberiaId = barbero.BarberiaId;
                barberoId = null;
            }

            hasOverlap = await _reservationService.HasOverlappingReservationAsync(barberoId, barberiaId, startDateTime, endDateTime);
        }
        else if (barberiaId.HasValue)
        {
            hasOverlap = await _reservationService.HasOverlappingReservationAsync(barberoId, barberiaId, startDateTime, endDateTime);
        }
        else
        {
            throw new Exception("Debe especificar barbero o barbería");
        }

        if (hasOverlap)
            throw new Exception("El horario seleccionado no está disponible");

        var reservation = new Reservation
        {
            ClienteId = clienteId,
            ServiceId = serviceId,
            BarberoId = barberoId,
            BarberiaId = barberiaId,
            StartDateTime = startDateTime,
            EndDateTime = endDateTime,
            Status = ReservationStatus.Pending,
            Notes = notes,
            CreatedAt = DateTime.UtcNow
        };

        _context.Reservations.Add(reservation);
        await _context.SaveChangesAsync();

        // Notify barbero or barberia
        if (barberoId.HasValue)
        {
            var barbero = await _context.Barberos.Include(b => b.User).FirstOrDefaultAsync(b => b.Id == barberoId.Value);
            if (barbero != null)
            {
                await _notificationService.CreateNotificationAsync(
                    barbero.UserId,
                    "Nueva Reserva",
                    $"Tienes una nueva solicitud de reserva para el {startDateTime:dd/MM/yyyy HH:mm}",
                    NotificationType.ReservationRequest,
                    reservationId: reservation.Id
                );
            }
        }
        else if (barberiaId.HasValue)
        {
            var barberia = await _context.Barberias.Include(b => b.User).FirstOrDefaultAsync(b => b.Id == barberiaId.Value);
            if (barberia != null)
            {
                await _notificationService.CreateNotificationAsync(
                    barberia.UserId,
                    "Nueva Reserva",
                    $"Tienes una nueva solicitud de reserva para el {startDateTime:dd/MM/yyyy HH:mm}",
                    NotificationType.ReservationRequest,
                    reservationId: reservation.Id
                );
            }
        }

        return reservation;
    }

    public async Task<bool> ConfirmReservationAsync(int reservationId)
    {
        var reservation = await _context.Reservations
            .Include(r => r.Cliente)
            .ThenInclude(c => c.User)
            .FirstOrDefaultAsync(r => r.Id == reservationId);

        if (reservation == null) return false;

        reservation.Status = ReservationStatus.Confirmed;
        reservation.ConfirmedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        // Notify client
        await _notificationService.CreateNotificationAsync(
            reservation.Cliente.UserId,
            "Reserva Confirmada",
            $"Tu reserva para el {reservation.StartDateTime:dd/MM/yyyy HH:mm} ha sido confirmada",
            NotificationType.ReservationResponse,
            reservationId: reservationId
        );

        return true;
    }

    public async Task<bool> CancelReservationAsync(int reservationId, int clienteId)
    {
        var reservation = await _context.Reservations
            .Include(r => r.Barbero)
            .Include(r => r.Barberia)
            .FirstOrDefaultAsync(r => r.Id == reservationId && r.ClienteId == clienteId);

        if (reservation == null) return false;

        reservation.Status = ReservationStatus.Cancelled;
        reservation.CancelledAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        // Notify barbero or barberia
        if (reservation.BarberoId.HasValue)
        {
            var barbero = await _context.Barberos.Include(b => b.User).FirstOrDefaultAsync(b => b.Id == reservation.BarberoId.Value);
            if (barbero != null)
            {
                await _notificationService.CreateNotificationAsync(
                    barbero.UserId,
                    "Reserva Cancelada",
                    $"El cliente ha cancelado la reserva del {reservation.StartDateTime:dd/MM/yyyy HH:mm}",
                    NotificationType.ReservationResponse,
                    reservationId: reservationId
                );
            }
        }
        else if (reservation.BarberiaId.HasValue)
        {
            var barberia = await _context.Barberias.Include(b => b.User).FirstOrDefaultAsync(b => b.Id == reservation.BarberiaId.Value);
            if (barberia != null)
            {
                await _notificationService.CreateNotificationAsync(
                    barberia.UserId,
                    "Reserva Cancelada",
                    $"El cliente ha cancelado la reserva del {reservation.StartDateTime:dd/MM/yyyy HH:mm}",
                    NotificationType.ReservationResponse,
                    reservationId: reservationId
                );
            }
        }

        return true;
    }

    public async Task<bool> RejectReservationAsync(int reservationId, string reason)
    {
        var reservation = await _context.Reservations
            .Include(r => r.Cliente)
            .ThenInclude(c => c.User)
            .FirstOrDefaultAsync(r => r.Id == reservationId);

        if (reservation == null) return false;

        reservation.Status = ReservationStatus.Rejected;
        reservation.CancelledAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        // Notify client with reason
        await _notificationService.CreateNotificationAsync(
            reservation.Cliente.UserId,
            "Reserva Rechazada",
            $"Tu reserva ha sido rechazada. Razón: {reason}",
            NotificationType.ReservationResponse,
            reservationId: reservationId
        );

        return true;
    }

    public async Task<IEnumerable<Reservation>> GetClienteReservationsAsync(int clienteId)
    {
        return await _context.Reservations
            .Include(r => r.Service)
            .Include(r => r.Barbero)
            .Include(r => r.Barberia)
            .Where(r => r.ClienteId == clienteId)
            .OrderByDescending(r => r.StartDateTime)
            .ToListAsync();
    }

    public async Task<IEnumerable<Service>> GetAvailableServicesAsync(int? barberoId, int? barberiaId)
    {
        var query = _context.Services.Where(s => s.IsActive);

        if (barberoId.HasValue)
        {
            query = query.Where(s => s.BarberoId == barberoId.Value);
        }
        else if (barberiaId.HasValue)
        {
            query = query.Where(s => s.BarberiaId == barberiaId.Value);
        }

        return await query.ToListAsync();
    }

    public async Task<bool> IsTimeSlotAvailableAsync(int? barberoId, int? barberiaId, DateTime startDateTime, DateTime endDateTime)
    {
        return !await _reservationService.HasOverlappingReservationAsync(barberoId, barberiaId, startDateTime, endDateTime);
    }
}

public class ComercialService : IComercialService
{
    private readonly CitaCorteDbContext _context;
    private readonly INotificationService _notificationService;

    public ComercialService(CitaCorteDbContext context, INotificationService notificationService)
    {
        _context = context;
        _notificationService = notificationService;
    }

    public async Task<IEnumerable<BarberoSubscriptionChange>> GetPendingBarberoSubscriptionChangesAsync()
    {
        return await _context.BarberoSubscriptionChanges
            .Include(sc => sc.Barbero)
            .ThenInclude(b => b.User)
            .Where(sc => sc.Status == SubscriptionStatus.Pending)
            .ToListAsync();
    }

    public async Task<IEnumerable<BarberiaSubscriptionChange>> GetPendingBarberiaSubscriptionChangesAsync()
    {
        return await _context.BarberiaSubscriptionChanges
            .Include(sc => sc.Barberia)
            .ThenInclude(b => b.User)
            .Where(sc => sc.Status == SubscriptionStatus.Pending)
            .ToListAsync();
    }

    public async Task<bool> ApproveBarberoSubscriptionChangeAsync(int changeId, int comercialId)
    {
        var change = await _context.BarberoSubscriptionChanges
            .Include(c => c.Barbero)
            .Include(c => c.Barbero.User)
            .FirstOrDefaultAsync(c => c.Id == changeId);

        if (change == null) return false;

        change.Status = SubscriptionStatus.Approved;
        change.ReviewedAt = DateTime.UtcNow;
        change.ReviewedByComercialId = comercialId;

        var plan = await _context.SubscriptionPlans
            .FirstOrDefaultAsync(p => p.Type == change.RequestedSubscription && !p.IsForBarberia);

        if (plan != null)
        {
            change.Barbero.CurrentSubscription = change.RequestedSubscription;
            change.Barbero.SubscriptionEndDate = DateTime.UtcNow.AddDays(plan.DurationDays);
        }

        await _context.SaveChangesAsync();

        await _notificationService.CreateNotificationAsync(
            change.Barbero.UserId,
            "Suscripción Aprobada",
            $"Tu cambio de suscripción a {change.RequestedSubscription} ha sido aprobado por el equipo comercial.",
            NotificationType.SubscriptionApproval,
            barberoSubscriptionChangeId: changeId
        );

        return true;
    }

    public async Task<bool> RejectBarberoSubscriptionChangeAsync(int changeId, int comercialId, string reason)
    {
        var change = await _context.BarberoSubscriptionChanges
            .Include(c => c.Barbero)
            .Include(c => c.Barbero.User)
            .FirstOrDefaultAsync(c => c.Id == changeId);

        if (change == null) return false;

        change.Status = SubscriptionStatus.Rejected;
        change.ReviewedAt = DateTime.UtcNow;
        change.ReviewedByComercialId = comercialId;
        change.RejectionReason = reason;

        await _context.SaveChangesAsync();

        await _notificationService.CreateNotificationAsync(
            change.Barbero.UserId,
            "Suscripción Rechazada",
            $"Tu cambio de suscripción ha sido rechazado. Razón: {reason}",
            NotificationType.SubscriptionApproval,
            barberoSubscriptionChangeId: changeId
        );

        return true;
    }

    public async Task<bool> ApproveBarberiaSubscriptionChangeAsync(int changeId, int comercialId)
    {
        var change = await _context.BarberiaSubscriptionChanges
            .Include(c => c.Barberia)
            .Include(c => c.Barberia.User)
            .FirstOrDefaultAsync(c => c.Id == changeId);

        if (change == null) return false;

        change.Status = SubscriptionStatus.Approved;
        change.ReviewedAt = DateTime.UtcNow;
        change.ReviewedByComercialId = comercialId;

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

        await _notificationService.CreateNotificationAsync(
            change.Barberia.UserId,
            "Suscripción Aprobada",
            $"Tu cambio de suscripción a {change.RequestedSubscription} ha sido aprobado por el equipo comercial.",
            NotificationType.SubscriptionApproval,
            barberiaSubscriptionChangeId: changeId
        );

        return true;
    }

    public async Task<bool> RejectBarberiaSubscriptionChangeAsync(int changeId, int comercialId, string reason)
    {
        var change = await _context.BarberiaSubscriptionChanges
            .Include(c => c.Barberia)
            .Include(c => c.Barberia.User)
            .FirstOrDefaultAsync(c => c.Id == changeId);

        if (change == null) return false;

        change.Status = SubscriptionStatus.Rejected;
        change.ReviewedAt = DateTime.UtcNow;
        change.ReviewedByComercialId = comercialId;
        change.RejectionReason = reason;

        await _context.SaveChangesAsync();

        await _notificationService.CreateNotificationAsync(
            change.Barberia.UserId,
            "Suscripción Rechazada",
            $"Tu cambio de suscripción ha sido rechazado. Razón: {reason}",
            NotificationType.SubscriptionApproval,
            barberiaSubscriptionChangeId: changeId
        );

        return true;
    }

    public async Task<Dictionary<string, object>> GetSystemStatisticsAsync()
    {
        var totalBarberos = await _context.Barberos.CountAsync();
        var totalBarberias = await _context.Barberias.CountAsync();
        var totalReservations = await _context.Reservations.CountAsync();
        var pendingBarberoChanges = await _context.BarberoSubscriptionChanges.CountAsync(sc => sc.Status == SubscriptionStatus.Pending);
        var pendingBarberiaChanges = await _context.BarberiaSubscriptionChanges.CountAsync(sc => sc.Status == SubscriptionStatus.Pending);

        var revenueBySubscription = await _context.Barberos
            .GroupBy(b => b.CurrentSubscription)
            .Select(g => new { Subscription = g.Key, Count = g.Count() })
            .ToListAsync();

        return new Dictionary<string, object>
        {
            { "TotalBarberos", totalBarberos },
            { "TotalBarberias", totalBarberias },
            { "TotalReservations", totalReservations },
            { "PendingBarberoChanges", pendingBarberoChanges },
            { "PendingBarberiaChanges", pendingBarberiaChanges },
            { "RevenueBySubscription", revenueBySubscription }
        };
    }

    public async Task<IEnumerable<Barbero>> GetAllBarberosAsync()
    {
        return await _context.Barberos
            .Include(b => b.User)
            .Include(b => b.Barberia)
            .ToListAsync();
    }

    public async Task<IEnumerable<Barberia>> GetAllBarberiasAsync()
    {
        return await _context.Barberias
            .Include(b => b.User)
            .Include(b => b.AffiliatedBarbers)
            .ToListAsync();
    }
}
