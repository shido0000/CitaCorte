using Microsoft.EntityFrameworkCore;
using CitaCorte.API.Data.Context;
using CitaCorte.API.Data.Entities;
using CitaCorte.API.Service.Interfaces;

namespace CitaCorte.API.Service.Implementations;

public class ReservationService : IReservationService
{
    private readonly ApplicationDbContext _context;
    private readonly INotificationService _notificationService;
    private readonly ISubscriptionService _subscriptionService;

    public ReservationService(ApplicationDbContext context, INotificationService notificationService, ISubscriptionService subscriptionService)
    {
        _context = context;
        _notificationService = notificationService;
        _subscriptionService = subscriptionService;
    }

    public async Task<Reservation> CreateReservationAsync(int clientUserId, int serviceId, int? barberoId, int? barberiaId, DateTime startDateTime, DateTime endDateTime, string? notes)
    {
        // Validate service exists
        var service = await _context.Services.FindAsync(serviceId);
        if (service == null || !service.IsActive)
        {
            throw new Exception("Servicio no encontrado o no disponible");
        }

        // Check time slot conflicts
        if (await HasTimeSlotConflictAsync(barberoId, barberiaId, startDateTime, endDateTime))
        {
            throw new Exception("El horario seleccionado no está disponible");
        }

        // If reserving with barbero, check if they can receive reservations
        if (barberoId.HasValue)
        {
            if (!await _subscriptionService.CanBarberoReceiveReservationsAsync(barberoId.Value))
            {
                throw new Exception("Este barbero no puede recibir reservas en este momento");
            }

            // If barbero is affiliated to a barberia, redirect reservation to barberia
            var barbero = await _context.Barberos
                .Include(b => b.Barberia)
                .FirstOrDefaultAsync(b => b.Id == barberoId.Value);

            if (barbero?.BarberiaId.HasValue == true && barbero.AffiliationStatus == AffiliationStatus.Accepted)
            {
                // Redirect to barberia
                barberiaId = barbero.BarberiaId.Value;
                barberoId = null;
            }
        }

        var reservation = new Reservation
        {
            ClientUserId = clientUserId,
            ServiceId = serviceId,
            BarberoId = barberoId,
            BarberiaId = barberiaId,
            StartDateTime = startDateTime,
            EndDateTime = endDateTime,
            Status = ReservationStatus.Pending,
            Notes = notes,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Reservations.AddAsync(reservation);
        await _context.SaveChangesAsync();

        // Notify barbero or barberia
        if (barberoId.HasValue)
        {
            var barbero = await _context.Barberos.Include(b => b.User).FirstOrDefaultAsync(b => b.Id == barberoId.Value);
            if (barbero != null)
            {
                await _notificationService.CreateNotificationAsync(
                    barbero.UserId,
                    NotificationType.ReservationRequest,
                    "Nueva solicitud de reserva",
                    $"Tienes una nueva solicitud de reserva para el servicio {service.Name}",
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
                    NotificationType.ReservationRequest,
                    "Nueva solicitud de reserva",
                    $"Tu barbería tiene una nueva solicitud de reserva para el servicio {service.Name}",
                    reservationId: reservation.Id
                );
            }
        }

        return reservation;
    }

    public async Task<Reservation> ConfirmReservationAsync(int reservationId, int responderUserId)
    {
        var reservation = await _context.Reservations
            .Include(r => r.Service)
            .FirstOrDefaultAsync(r => r.Id == reservationId);

        if (reservation == null) throw new Exception("Reserva no encontrada");
        if (reservation.Status != ReservationStatus.Pending)
        {
            throw new Exception("La reserva ya ha sido procesada");
        }

        // Verify responder is the barbero or barberia owner
        bool isAuthorized = false;

        if (reservation.BarberoId.HasValue)
        {
            var barbero = await _context.Barberos.FirstOrDefaultAsync(b => b.Id == reservation.BarberoId.Value);
            if (barbero != null && barbero.UserId == responderUserId)
            {
                isAuthorized = true;
            }
        }

        if (reservation.BarberiaId.HasValue && !isAuthorized)
        {
            var barberia = await _context.Barberias.FirstOrDefaultAsync(b => b.Id == reservation.BarberiaId.Value);
            if (barberia != null && barberia.UserId == responderUserId)
            {
                isAuthorized = true;
            }
        }

        if (!isAuthorized) throw new Exception("No autorizado para confirmar esta reserva");

        reservation.Status = ReservationStatus.Confirmed;
        reservation.ConfirmedAt = DateTime.UtcNow;
        reservation.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        // Notify client
        var client = await _context.Users.FindAsync(reservation.ClientUserId);
        if (client != null)
        {
            await _notificationService.CreateNotificationAsync(
                reservation.ClientUserId,
                NotificationType.ReservationResponse,
                "Reserva confirmada",
                $"Tu reserva para {reservation.Service.Name} ha sido confirmada",
                reservationId: reservation.Id
            );
        }

        return reservation;
    }

    public async Task<Reservation> CancelReservationAsync(int reservationId, int userId, string? reason)
    {
        var reservation = await _context.Reservations
            .Include(r => r.Service)
            .FirstOrDefaultAsync(r => r.Id == reservationId);

        if (reservation == null) throw new Exception("Reserva no encontrada");

        // Only client can cancel their own reservation, or barbero/barberia can cancel theirs
        bool canCancel = reservation.ClientUserId == userId;

        if (!canCancel && reservation.BarberoId.HasValue)
        {
            var barbero = await _context.Barberos.FirstOrDefaultAsync(b => b.Id == reservation.BarberoId.Value);
            if (barbero != null && barbero.UserId == userId)
            {
                canCancel = true;
            }
        }

        if (!canCancel && reservation.BarberiaId.HasValue)
        {
            var barberia = await _context.Barberias.FirstOrDefaultAsync(b => b.Id == reservation.BarberiaId.Value);
            if (barberia != null && barberia.UserId == userId)
            {
                canCancel = true;
            }
        }

        if (!canCancel) throw new Exception("No autorizado para cancelar esta reserva");

        reservation.Status = ReservationStatus.Cancelled;
        reservation.CancellationReason = reason;
        reservation.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        // Notify the other party
        int notifyUserId;
        string notificationMessage;

        if (userId == reservation.ClientUserId)
        {
            // Client cancelled, notify barbero/barberia
            if (reservation.BarberoId.HasValue)
            {
                var barbero = await _context.Barberos.Include(b => b.User).FirstOrDefaultAsync(b => b.Id == reservation.BarberoId.Value);
                if (barbero != null)
                {
                    notifyUserId = barbero.UserId;
                    notificationMessage = $"El cliente ha cancelado la reserva para {reservation.Service.Name}";
                }
                else return reservation;
            }
            else if (reservation.BarberiaId.HasValue)
            {
                var barberia = await _context.Barberias.Include(b => b.User).FirstOrDefaultAsync(b => b.Id == reservation.BarberiaId.Value);
                if (barberia != null)
                {
                    notifyUserId = barberia.UserId;
                    notificationMessage = $"El cliente ha cancelado la reserva para {reservation.Service.Name}";
                }
                else return reservation;
            }
            else return reservation;
        }
        else
        {
            // Barbero/Barberia cancelled, notify client
            notifyUserId = reservation.ClientUserId;
            notificationMessage = $"Tu reserva para {reservation.Service.Name} ha sido cancelada";
        }

        await _notificationService.CreateNotificationAsync(
            notifyUserId,
            NotificationType.ReservationResponse,
            "Reserva cancelada",
            notificationMessage,
            reservationId: reservation.Id
        );

        return reservation;
    }

    public async Task<Reservation?> GetReservationByIdAsync(int reservationId)
    {
        return await _context.Reservations
            .Include(r => r.Service)
            .Include(r => r.ClientUser)
            .Include(r => r.Barbero)
                .ThenInclude(b => b!.User)
            .Include(r => r.Barberia)
                .ThenInclude(b => b!.User)
            .FirstOrDefaultAsync(r => r.Id == reservationId);
    }

    public async Task<IEnumerable<Reservation>> GetClientReservationsAsync(int clientUserId)
    {
        return await _context.Reservations
            .Include(r => r.Service)
            .Include(r => r.Barbero)
                .ThenInclude(b => b!.User)
            .Include(r => r.Barberia)
                .ThenInclude(b => b!.User)
            .Where(r => r.ClientUserId == clientUserId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Reservation>> GetBarberoReservationsAsync(int barberoId, ReservationStatus? status = null)
    {
        var query = _context.Reservations
            .Include(r => r.Service)
            .Include(r => r.ClientUser)
            .Where(r => r.BarberoId == barberoId);

        if (status.HasValue)
        {
            query = query.Where(r => r.Status == status.Value);
        }

        return await query.OrderByDescending(r => r.CreatedAt).ToListAsync();
    }

    public async Task<IEnumerable<Reservation>> GetBarberiaReservationsAsync(int barberiaId, ReservationStatus? status = null)
    {
        var query = _context.Reservations
            .Include(r => r.Service)
            .Include(r => r.ClientUser)
            .Where(r => r.BarberiaId == barberiaId);

        if (status.HasValue)
        {
            query = query.Where(r => r.Status == status.Value);
        }

        return await query.OrderByDescending(r => r.CreatedAt).ToListAsync();
    }

    public async Task<bool> HasTimeSlotConflictAsync(int? barberoId, int? barberiaId, DateTime startDateTime, DateTime endDateTime, int? excludeReservationId = null)
    {
        var query = _context.Reservations
            .Where(r => r.Status != ReservationStatus.Cancelled);

        if (barberoId.HasValue)
        {
            query = query.Where(r => r.BarberoId == barberoId.Value);
        }
        else if (barberiaId.HasValue)
        {
            query = query.Where(r => r.BarberiaId == barberiaId.Value);
        }
        else
        {
            return false; // No provider specified, no conflict possible
        }

        if (excludeReservationId.HasValue)
        {
            query = query.Where(r => r.Id != excludeReservationId.Value);
        }

        // Check for overlapping time slots
        return await query.AnyAsync(r =>
            (r.StartDateTime < endDateTime && r.EndDateTime > startDateTime)
        );
    }
}
