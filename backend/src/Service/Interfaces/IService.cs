using CitaCorte.API.Data.Entities;

namespace CitaCorte.API.Service.Interfaces;

public interface IAuthService
{
    Task<(User user, string token)?> RegisterAsync(string name, string email, string password, UserRole role, string? phone = null);
    Task<(User user, string token)?> LoginAsync(string email, string password);
    Task<User?> GetUserByIdAsync(int userId);
    Task<User?> GetUserByEmailAsync(string email);
}

public interface ISubscriptionService
{
    Task<SubscriptionPlan> CreatePlanAsync(SubscriptionPlan plan);
    Task<IEnumerable<SubscriptionPlan>> GetAllPlansAsync();
    Task<SubscriptionPlan?> GetPlanByTypeAsync(SubscriptionType type);
    Task UpdatePlanAsync(SubscriptionPlan plan);
    
    // Barbero subscription management
    Task RequestSubscriptionChangeAsync(int barberoId, SubscriptionType newType);
    Task ApproveSubscriptionChangeAsync(int barberoId, bool approved, int? approvedByUserId = null);
    Task<bool> CanBarberoReceiveReservationsAsync(int barberoId);
    Task<bool> CanBarberoSellProductsAsync(int barberoId);
    Task<bool> CanBarberoViewStatisticsAsync(int barberoId);
    
    // Barberia subscription management
    Task RequestBarberiaSubscriptionAsync(int barberiaId, SubscriptionType type);
    Task ApproveBarberiaSubscriptionAsync(int barberiaId, bool approved, int? approvedByUserId = null);
    Task<bool> CanBarberiaAffiliateBarberosAsync(int barberiaId);
    Task<int> GetBarberiaMaxBarberosLimitAsync(int barberiaId);
    Task<bool> IsBarberiaSubscriptionActiveAsync(int barberiaId);
}

public interface IBarberoService
{
    Task<Barbero> GetBarberoByUserIdAsync(int userId);
    Task<Barbero> UpdateBarberoProfileAsync(int userId, string? bio, string? specialties, string? profileImageUrl);
    Task RequestAffiliationAsync(int barberoUserId, int barberiaId, string? message);
    Task CancelAffiliationRequestAsync(int barberoUserId);
    Task<IEnumerable<Barbero>> SearchBarberosAsync(string? searchTerm, int? cityId = null);
    Task<IEnumerable<Service>> GetBarberoServicesAsync(int barberoId);
    Task<Service> AddServiceAsync(int barberoId, Service service);
    Task UpdateServiceAsync(Service service);
    Task DeleteServiceAsync(int serviceId);
    
    // Product management (Premium only)
    Task<Product> AddProductAsync(int barberoId, Product product);
    Task UpdateProductAsync(Product product);
    Task DeleteProductAsync(int productId);
    Task<IEnumerable<Product>> GetBarberoProductsAsync(int barberoId);
    
    // Statistics
    Task<BarberoStatistic?> GetBarberoStatisticsAsync(int barberoId, DateTime? startDate = null, DateTime? endDate = null);
}

public interface IBarberiaService
{
    Task<Barberia> GetBarberiaByUserIdAsync(int userId);
    Task<Barberia> CreateBarberiaAsync(int userId, string name, string? description, string? address, string? city, string? state, string? phone, string? logoUrl);
    Task<Barberia> UpdateBarberiaProfileAsync(int userId, string? name, string? description, string? address, string? city, string? state, string? phone, string? logoUrl);
    
    // Affiliation management
    Task<IEnumerable<AffiliationRequest>> GetPendingAffiliationRequestsAsync(int barberiaUserId);
    Task RespondToAffiliationRequestAsync(int barberiaUserId, int affiliationRequestId, bool accepted, string? rejectionReason);
    Task<IEnumerable<Barbero>> GetAffiliatedBarberosAsync(int barberiaId);
    Task RemoveBarberoAsync(int barberiaId, int barberoId);
    
    // Service management
    Task<Service> AddServiceAsync(int barberiaId, Service service);
    Task UpdateServiceAsync(Service service);
    Task DeleteServiceAsync(int serviceId);
    Task<IEnumerable<Service>> GetBarberiaServicesAsync(int barberiaId);
    
    // Statistics
    Task<BarberiaStatistic?> GetBarberiaStatisticsAsync(int barberiaId, DateTime? startDate = null, DateTime? endDate = null);
}

public interface IReservationService
{
    Task<Reservation> CreateReservationAsync(int clientUserId, int serviceId, int? barberoId, int? barberiaId, DateTime startDateTime, DateTime endDateTime, string? notes);
    Task<Reservation> ConfirmReservationAsync(int reservationId, int responderUserId);
    Task<Reservation> CancelReservationAsync(int reservationId, int userId, string? reason);
    Task<Reservation?> GetReservationByIdAsync(int reservationId);
    Task<IEnumerable<Reservation>> GetClientReservationsAsync(int clientUserId);
    Task<IEnumerable<Reservation>> GetBarberoReservationsAsync(int barberoId, ReservationStatus? status = null);
    Task<IEnumerable<Reservation>> GetBarberiaReservationsAsync(int barberiaId, ReservationStatus? status = null);
    Task<bool> HasTimeSlotConflictAsync(int? barberoId, int? barberiaId, DateTime startDateTime, DateTime endDateTime, int? excludeReservationId = null);
}

public interface INotificationService
{
    Task<Notification> CreateNotificationAsync(int userId, NotificationType type, string title, string message, int? reservationId = null, int? affiliationRequestId = null);
    Task<IEnumerable<Notification>> GetUserNotificationsAsync(int userId, bool unreadOnly = false);
    Task MarkNotificationAsReadAsync(int notificationId);
    Task MarkAllNotificationsAsReadAsync(int userId);
    Task DeleteNotificationAsync(int notificationId);
}

public interface IAdminService
{
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<IEnumerable<Barbero>> GetAllBarberosAsync();
    Task<IEnumerable<Barberia>> GetAllBarberiasAsync();
    Task DeactivateUserAsync(int userId);
    Task ActivateUserAsync(int userId);
    Task<IEnumerable<SubscriptionPlan>> GetAllSubscriptionPlansAsync();
    Task<SubscriptionPlan> CreateSubscriptionPlanAsync(SubscriptionPlan plan);
    Task UpdateSubscriptionPlanAsync(SubscriptionPlan plan);
    Task DeleteSubscriptionPlanAsync(int planId);
}

public interface IComercialService
{
    Task<IEnumerable<Barbero>> GetBarberosPendingSubscriptionApprovalAsync();
    Task<IEnumerable<Barberia>> GetBarberiasPendingSubscriptionApprovalAsync();
    Task ApproveBarberoSubscriptionAsync(int barberoId, bool approved);
    Task ApproveBarberiaSubscriptionAsync(int barberiaId, bool approved);
    Task<object> GetGeneralStatisticsAsync(DateTime? startDate = null, DateTime? endDate = null);
}

public interface IClientService
{
    Task<IEnumerable<Barbero>> SearchAvailableBarberosAsync(DateTime? date = null, string? specialty = null);
    Task<IEnumerable<Barberia>> SearchAvailableBarberiasAsync(DateTime? date = null);
    Task<IEnumerable<Service>> GetBarberoServicesAsync(int barberoId);
    Task<IEnumerable<Service>> GetBarberiaServicesAsync(int barberiaId);
}
