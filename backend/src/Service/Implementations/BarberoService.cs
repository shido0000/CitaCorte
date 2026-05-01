using Microsoft.EntityFrameworkCore;
using CitaCorte.API.Data.Context;
using CitaCorte.API.Data.Entities;
using CitaCorte.API.Service.Interfaces;

namespace CitaCorte.API.Service.Implementations;

public class BarberoService : IBarberoService
{
    private readonly ApplicationDbContext _context;
    private readonly ISubscriptionService _subscriptionService;
    private readonly INotificationService _notificationService;

    public BarberoService(ApplicationDbContext context, ISubscriptionService subscriptionService, INotificationService notificationService)
    {
        _context = context;
        _subscriptionService = subscriptionService;
        _notificationService = notificationService;
    }

    public async Task<Barbero> GetBarberoByUserIdAsync(int userId)
    {
        var barbero = await _context.Barberos
            .Include(b => b.User)
            .Include(b => b.Barberia)
            .FirstOrDefaultAsync(b => b.UserId == userId);

        if (barbero == null) throw new Exception("Perfil de barbero no encontrado");
        return barbero;
    }

    public async Task<Barbero> UpdateBarberoProfileAsync(int userId, string? bio, string? specialties, string? profileImageUrl)
    {
        var barbero = await GetBarberoByUserIdAsync(userId);

        barbero.Bio = bio;
        barbero.Specialties = specialties;
        barbero.ProfileImageUrl = profileImageUrl;

        await _context.SaveChangesAsync();
        return barbero;
    }

    public async Task RequestAffiliationAsync(int barberoUserId, int barberiaId, string? message)
    {
        var barbero = await GetBarberoByUserIdAsync(barberoUserId);
        
        // Check if barbero can receive reservations (needs at least Popular plan)
        if (!await _subscriptionService.CanBarberoReceiveReservationsAsync(barbero.Id))
        {
            throw new Exception("El barbero necesita una suscripción Popular o Premium para afiliarse");
        }

        var barberia = await _context.Barberias.FindAsync(barberiaId);
        if (barberia == null) throw new Exception("Barbería no encontrada");

        // Check if barberia subscription is active
        if (!await _subscriptionService.CanBarberiaAffiliateBarberosAsync(barberiaId))
        {
            throw new Exception("La barbería no tiene una suscripción activa para afiliar barberos");
        }

        // Check if already has pending request
        var existingRequest = await _context.AffiliationRequests
            .FirstOrDefaultAsync(ar => ar.BarberoId == barbero.Id && ar.BarberiaId == barberiaId && ar.Status == AffiliationStatus.Pending);

        if (existingRequest != null)
        {
            throw new Exception("Ya existe una solicitud de afiliación pendiente");
        }

        var request = new AffiliationRequest
        {
            BarberoId = barbero.Id,
            BarberiaId = barberiaId,
            Message = message,
            Status = AffiliationStatus.Pending,
            RequestedAt = DateTime.UtcNow
        };

        await _context.AffiliationRequests.AddAsync(request);
        await _context.SaveChangesAsync();

        // Notify barberia
        await _notificationService.CreateNotificationAsync(
            barberia.UserId,
            NotificationType.AffiliationRequest,
            "Nueva solicitud de afiliación",
            $"El barbero {barbero.User.Name} solicita unirse a tu barbería",
            affiliationRequestId: request.Id
        );
    }

    public async Task CancelAffiliationRequestAsync(int barberoUserId)
    {
        var barbero = await GetBarberoByUserIdAsync(barberoUserId);
        
        var request = await _context.AffiliationRequests
            .FirstOrDefaultAsync(ar => ar.BarberoId == barbero.Id && ar.Status == AffiliationStatus.Pending);

        if (request != null)
        {
            request.Status = AffiliationStatus.Rejected;
            request.RespondedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Barbero>> SearchBarberosAsync(string? searchTerm, int? cityId = null)
    {
        var query = _context.Barberos
            .Include(b => b.User)
            .Include(b => b.Barberia)
            .Where(b => b.SubscriptionStatus == SubscriptionStatus.Active);

        if (!string.IsNullOrEmpty(searchTerm))
        {
            query = query.Where(b => b.User.Name.Contains(searchTerm) || 
                                     (b.Specialties != null && b.Specialties.Contains(searchTerm)));
        }

        return await query.ToListAsync();
    }

    public async Task<IEnumerable<Service>> GetBarberoServicesAsync(int barberoId)
    {
        return await _context.Services
            .Where(s => s.BarberoId == barberoId && s.IsActive)
            .ToListAsync();
    }

    public async Task<Service> AddServiceAsync(int barberoId, Service service)
    {
        var barbero = await GetBarberoByUserIdAsync(barberoId);
        
        service.BarberoId = barbero.Id;
        service.CreatedAt = DateTime.UtcNow;
        
        await _context.Services.AddAsync(service);
        await _context.SaveChangesAsync();
        return service;
    }

    public async Task UpdateServiceAsync(Service service)
    {
        var existingService = await _context.Services.FindAsync(service.Id);
        if (existingService == null) throw new Exception("Servicio no encontrado");

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

    // Product management (Premium only)
    public async Task<Product> AddProductAsync(int barberoId, Product product)
    {
        var barbero = await GetBarberoByUserIdAsync(barberoId);

        if (!await _subscriptionService.CanBarberoSellProductsAsync(barbero.Id))
        {
            throw new Exception("El barbero necesita una suscripción Premium para vender productos");
        }

        product.BarberoId = barbero.Id;
        product.CreatedAt = DateTime.UtcNow;

        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task UpdateProductAsync(Product product)
    {
        var existingProduct = await _context.Products.FindAsync(product.Id);
        if (existingProduct == null) throw new Exception("Producto no encontrado");

        existingProduct.Name = product.Name;
        existingProduct.Description = product.Description;
        existingProduct.Price = product.Price;
        existingProduct.StockQuantity = product.StockQuantity;
        existingProduct.ImageUrl = product.ImageUrl;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteProductAsync(int productId)
    {
        var product = await _context.Products.FindAsync(productId);
        if (product != null)
        {
            product.IsActive = false;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Product>> GetBarberoProductsAsync(int barberoId)
    {
        return await _context.Products
            .Where(p => p.BarberoId == barberoId && p.IsActive)
            .ToListAsync();
    }

    public async Task<BarberoStatistic?> GetBarberoStatisticsAsync(int barberoId, DateTime? startDate = null, DateTime? endDate = null)
    {
        var barbero = await GetBarberoByUserIdAsync(barberoId);

        if (!await _subscriptionService.CanBarberoViewStatisticsAsync(barbero.Id))
        {
            throw new Exception("El barbero necesita una suscripción Popular o Premium para ver estadísticas");
        }

        startDate ??= DateTime.UtcNow.AddDays(-30);
        endDate ??= DateTime.UtcNow;

        var reservations = await _context.Reservations
            .Where(r => r.BarberoId == barberoId && 
                        r.CreatedAt >= startDate && 
                        r.CreatedAt <= endDate)
            .ToListAsync();

        var statistic = new BarberoStatistic
        {
            BarberoId = barberoId,
            Date = DateTime.UtcNow.Date,
            TotalReservations = reservations.Count,
            ConfirmedReservations = reservations.Count(r => r.Status == ReservationStatus.Confirmed),
            CancelledReservations = reservations.Count(r => r.Status == ReservationStatus.Cancelled),
            CompletedReservations = reservations.Count(r => r.Status == ReservationStatus.Completed),
            TotalRevenue = reservations.Where(r => r.Status == ReservationStatus.Completed).Sum(r => r.Service.Price),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        return statistic;
    }
}
