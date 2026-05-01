using Microsoft.EntityFrameworkCore;
using CitaCorte.Data.Context;
using CitaCorte.Data.Entities;
using CitaCorte.Service.Interfaces;

namespace CitaCorte.Service.Services;

public class BarberoService : IBarberoService
{
    private readonly CitaCorteDbContext _context;
    private readonly INotificationService _notificationService;
    private readonly IReservationService _reservationService;

    public BarberoService(CitaCorteDbContext context, INotificationService notificationService, IReservationService reservationService)
    {
        _context = context;
        _notificationService = notificationService;
        _reservationService = reservationService;
    }

    public async Task<Barbero> CreateBarberoAsync(User user, string? bio, string? address, decimal? latitude, decimal? longitude)
    {
        var barbero = new Barbero
        {
            UserId = user.Id,
            Bio = bio,
            Address = address,
            Latitude = latitude,
            Longitude = longitude,
            CurrentSubscription = SubscriptionType.Free,
            SubscriptionStartDate = DateTime.UtcNow,
            SubscriptionEndDate = DateTime.UtcNow.AddDays(30),
            AffiliationStatus = AffiliationStatus.Pending
        };

        _context.Barberos.Add(barbero);
        await _context.SaveChangesAsync();

        return barbero;
    }

    public async Task<Barbero?> GetBarberoByIdAsync(int barberoId)
    {
        return await _context.Barberos
            .Include(b => b.User)
            .Include(b => b.Barberia)
            .Include(b => b.Services)
            .FirstOrDefaultAsync(b => b.Id == barberoId);
    }

    public async Task<Barbero?> GetBarberoByUserIdAsync(int userId)
    {
        return await _context.Barberos
            .Include(b => b.User)
            .Include(b => b.Barberia)
            .Include(b => b.Services)
            .FirstOrDefaultAsync(b => b.UserId == userId);
    }

    public async Task<Barbero> UpdateBarberoAsync(int barberoId, string? bio, string? profileImageUrl, string? address, decimal? latitude, decimal? longitude)
    {
        var barbero = await _context.Barberos.FindAsync(barberoId);
        if (barbero == null) throw new Exception("Barbero no encontrado");

        barbero.Bio = bio;
        barbero.ProfileImageUrl = profileImageUrl;
        barbero.Address = address;
        barbero.Latitude = latitude;
        barbero.Longitude = longitude;

        await _context.SaveChangesAsync();
        return barbero;
    }

    public async Task<IEnumerable<Service>> GetBarberoServicesAsync(int barberoId)
    {
        return await _context.Services
            .Where(s => s.BarberoId == barberoId && s.IsActive)
            .ToListAsync();
    }

    public async Task<Service> AddServiceAsync(int barberoId, Service service)
    {
        var barbero = await _context.Barberos.FindAsync(barberoId);
        if (barbero == null) throw new Exception("Barbero no encontrado");

        service.BarberoId = barberoId;
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

    public async Task<IEnumerable<Product>> GetBarberoProductsAsync(int barberoId)
    {
        var barbero = await _context.Barberos.FindAsync(barberoId);
        if (barbero == null || !barbero.CanSellProducts)
            throw new Exception("El barbero no tiene permiso para vender productos");

        return await _context.Products
            .Where(p => p.BarberoId == barberoId && p.IsActive)
            .ToListAsync();
    }

    public async Task<Product> AddProductAsync(int barberoId, Product product)
    {
        var barbero = await _context.Barberos.FindAsync(barberoId);
        if (barbero == null || !barbero.CanSellProducts)
            throw new Exception("El barbero no tiene permiso para vender productos");

        product.BarberoId = barberoId;
        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return product;
    }

    public async Task<bool> UpdateProductAsync(int productId, Product product)
    {
        var existingProduct = await _context.Products.FindAsync(productId);
        if (existingProduct == null) return false;

        existingProduct.Name = product.Name;
        existingProduct.Description = product.Description;
        existingProduct.Price = product.Price;
        existingProduct.Stock = product.Stock;
        existingProduct.ImageUrl = product.ImageUrl;
        existingProduct.IsActive = product.IsActive;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteProductAsync(int productId)
    {
        var product = await _context.Products.FindAsync(productId);
        if (product == null) return false;

        product.IsActive = false;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RequestAffiliationToBarberiaAsync(int barberoId, int barberiaId)
    {
        var barbero = await _context.Barberos
            .Include(b => b.User)
            .FirstOrDefaultAsync(b => b.Id == barberoId);
            
        if (barbero == null) throw new Exception("Barbero no encontrado");

        var barberia = await _context.Barberias
            .Include(b => b.User)
            .FirstOrDefaultAsync(b => b.Id == barberiaId);
            
        if (barberia == null) throw new Exception("Barbería no encontrada");

        // Check if already affiliated or pending
        if (barbero.AffiliationStatus == AffiliationStatus.Approved && barbero.BarberiaId == barberiaId)
            throw new Exception("El barbero ya está afiliado a esta barbería");

        barbero.BarberiaId = barberiaId;
        barbero.AffiliationStatus = AffiliationStatus.Pending;
        barbero.AffiliationRequestedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        // Notify barberia
        await _notificationService.CreateNotificationAsync(
            barberia.UserId,
            "Solicitud de Afiliación",
            $"El barbero {barbero.User.FirstName} {barbero.User.LastName} ha solicitado unirse a tu barbería.",
            NotificationType.AffiliationRequest
        );

        return true;
    }

    public async Task<bool> RequestSubscriptionChangeAsync(int barberoId, SubscriptionType newSubscription)
    {
        var barbero = await _context.Barberos
            .Include(b => b.User)
            .FirstOrDefaultAsync(b => b.Id == barberoId);
            
        if (barbero == null) throw new Exception("Barbero no encontrado");

        if (newSubscription == barbero.CurrentSubscription)
            throw new Exception("Ya tienes esta suscripción");

        var changeRequest = new BarberoSubscriptionChange
        {
            BarberoId = barberoId,
            RequestedSubscription = newSubscription,
            Status = SubscriptionStatus.Pending,
            RequestedAt = DateTime.UtcNow
        };

        _context.BarberoSubscriptionChanges.Add(changeRequest);
        await _context.SaveChangesAsync();

        // Notify admin and comercial
        var admins = await _context.Admins.Include(a => a.User).ToListAsync();
        var comerciales = await _context.Comerciales.Include(c => c.User).ToListAsync();

        foreach (var admin in admins)
        {
            await _notificationService.CreateNotificationAsync(
                admin.UserId,
                "Cambio de Suscripción Pendiente",
                $"El barbero {barbero.User.FirstName} {barbero.User.LastName} ha solicitado cambiar a {newSubscription}",
                NotificationType.SubscriptionChange,
                barberoSubscriptionChangeId: changeRequest.Id
            );
        }

        foreach (var comercial in comerciales)
        {
            await _notificationService.CreateNotificationAsync(
                comercial.UserId,
                "Cambio de Suscripción Pendiente",
                $"El barbero {barbero.User.FirstName} {barbero.User.LastName} ha solicitado cambiar a {newSubscription}",
                NotificationType.SubscriptionChange,
                barberoSubscriptionChangeId: changeRequest.Id
            );
        }

        return true;
    }

    public async Task<Dictionary<string, object>> GetBarberoStatisticsAsync(int barberoId)
    {
        var barbero = await _context.Barberos.FindAsync(barberoId);
        if (barbero == null) throw new Exception("Barbero no encontrado");

        var reservations = await _context.Reservations
            .Where(r => r.BarberoId == barberoId)
            .ToListAsync();

        var totalReservations = reservations.Count;
        var completedReservations = reservations.Count(r => r.Status == ReservationStatus.Completed);
        var cancelledReservations = reservations.Count(r => r.Status == ReservationStatus.Cancelled);
        var totalRevenue = reservations.Where(r => r.Status == ReservationStatus.Completed).Sum(r => r.Service.Price);

        var productsSold = 0;
        var productsRevenue = 0m;

        if (barbero.CanSellProducts)
        {
            var sales = await _context.ProductSales
                .Where(ps => ps.Product.BarberoId == barberoId)
                .ToListAsync();
            productsSold = sales.Sum(ps => ps.Quantity);
            productsRevenue = sales.Sum(ps => ps.TotalPrice);
        }

        return new Dictionary<string, object>
        {
            { "TotalReservations", totalReservations },
            { "CompletedReservations", completedReservations },
            { "CancelledReservations", cancelledReservations },
            { "TotalRevenue", totalRevenue },
            { "ProductsSold", productsSold },
            { "ProductsRevenue", productsRevenue }
        };
    }

    public async Task<bool> HasActiveSubscriptionAsync(int barberoId)
    {
        var barbero = await _context.Barberos.FindAsync(barberoId);
        return barbero != null && barbero.SubscriptionEndDate > DateTime.UtcNow;
    }

    public async Task<bool> CanReceiveReservationsAsync(int barberoId)
    {
        var barbero = await _context.Barberos.FindAsync(barberoId);
        return barbero != null && barbero.CanReceiveReservations && barbero.SubscriptionEndDate > DateTime.UtcNow;
    }

    public async Task<bool> CanSellProductsAsync(int barberoId)
    {
        var barbero = await _context.Barberos.FindAsync(barberoId);
        return barbero != null && barbero.CanSellProducts && barbero.SubscriptionEndDate > DateTime.UtcNow;
    }
}
