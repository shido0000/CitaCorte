using CitaCorte.Data.Entities;

namespace CitaCorte.Service.Interfaces;

public interface IAuthService
{
    Task<User?> RegisterAsync(string email, string password, string firstName, string? lastName, string phone, Role role);
    Task<string?> LoginAsync(string email, string password);
    Task<User?> GetUserByIdAsync(int userId);
    Task<User?> GetUserByEmailAsync(string email);
}

public interface IAdminService
{
    Task<SubscriptionPlan> CreateSubscriptionPlanAsync(SubscriptionPlan plan);
    Task<IEnumerable<SubscriptionPlan>> GetAllSubscriptionPlansAsync();
    Task<SubscriptionPlan?> UpdateSubscriptionPlanAsync(int planId, SubscriptionPlan plan);
    Task<bool> DeleteSubscriptionPlanAsync(int planId);
    Task<IEnumerable<Barbero>> GetAllBarberosAsync();
    Task<IEnumerable<Barberia>> GetAllBarberiasAsync();
    Task<Dictionary<string, object>> GetSystemStatisticsAsync();
    Task<bool> ApproveBarberoSubscriptionChangeAsync(int changeId, int adminId);
    Task<bool> RejectBarberoSubscriptionChangeAsync(int changeId, int adminId, string reason);
    Task<bool> ApproveBarberiaSubscriptionChangeAsync(int changeId, int adminId);
    Task<bool> RejectBarberiaSubscriptionChangeAsync(int changeId, int adminId, string reason);
}

public interface IBarberoService
{
    Task<Barbero> CreateBarberoAsync(User user, string? bio, string? address, decimal? latitude, decimal? longitude);
    Task<Barbero?> GetBarberoByIdAsync(int barberoId);
    Task<Barbero?> GetBarberoByUserIdAsync(int userId);
    Task<Barbero> UpdateBarberoAsync(int barberoId, string? bio, string? profileImageUrl, string? address, decimal? latitude, decimal? longitude);
    Task<IEnumerable<Service>> GetBarberoServicesAsync(int barberoId);
    Task<Service> AddServiceAsync(int barberoId, Service service);
    Task<bool> UpdateServiceAsync(int serviceId, Service service);
    Task<bool> DeleteServiceAsync(int serviceId);
    Task<IEnumerable<Product>> GetBarberoProductsAsync(int barberoId);
    Task<Product> AddProductAsync(int barberoId, Product product);
    Task<bool> UpdateProductAsync(int productId, Product product);
    Task<bool> DeleteProductAsync(int productId);
    Task<bool> RequestAffiliationToBarberiaAsync(int barberoId, int barberiaId);
    Task<bool> RequestSubscriptionChangeAsync(int barberoId, SubscriptionType newSubscription);
    Task<Dictionary<string, object>> GetBarberoStatisticsAsync(int barberoId);
    Task<bool> HasActiveSubscriptionAsync(int barberoId);
    Task<bool> CanReceiveReservationsAsync(int barberoId);
    Task<bool> CanSellProductsAsync(int barberoId);
}

public interface IBarberiaService
{
    Task<Barberia> CreateBarberiaAsync(User user, string name, string description, string address, decimal latitude, decimal longitude, string phone, SubscriptionType subscriptionType);
    Task<Barberia?> GetBarberiaByIdAsync(int barberiaId);
    Task<Barberia?> GetBarberiaByUserIdAsync(int userId);
    Task<Barberia> UpdateBarberiaAsync(int barberiaId, string? description, string? profileImageUrl);
    Task<IEnumerable<Barbero>> GetAffiliatedBarbersAsync(int barberiaId);
    Task<bool> ApproveAffiliationRequestAsync(int barberiaId, int barberoId);
    Task<bool> RejectAffiliationRequestAsync(int barberiaId, int barberoId, string? reason = null);
    Task<IEnumerable<Service>> GetBarberiaServicesAsync(int barberiaId);
    Task<Service> AddServiceAsync(int barberiaId, Service service);
    Task<bool> UpdateServiceAsync(int serviceId, Service service);
    Task<bool> DeleteServiceAsync(int serviceId);
    Task<bool> RequestSubscriptionChangeAsync(int barberiaId, SubscriptionType newSubscription);
    Task<Dictionary<string, object>> GetBarberiaStatisticsAsync(int barberiaId);
    Task<bool> IsActiveAsync(int barberiaId);
    Task<int> GetAffiliatedBarbersCountAsync(int barberiaId);
}

public interface IComercialService
{
    Task<IEnumerable<BarberoSubscriptionChange>> GetPendingBarberoSubscriptionChangesAsync();
    Task<IEnumerable<BarberiaSubscriptionChange>> GetPendingBarberiaSubscriptionChangesAsync();
    Task<bool> ApproveBarberoSubscriptionChangeAsync(int changeId, int comercialId);
    Task<bool> RejectBarberoSubscriptionChangeAsync(int changeId, int comercialId, string reason);
    Task<bool> ApproveBarberiaSubscriptionChangeAsync(int changeId, int comercialId);
    Task<bool> RejectBarberiaSubscriptionChangeAsync(int changeId, int comercialId, string reason);
    Task<Dictionary<string, object>> GetSystemStatisticsAsync();
    Task<IEnumerable<Barbero>> GetAllBarberosAsync();
    Task<IEnumerable<Barberia>> GetAllBarberiasAsync();
}

public interface IClienteService
{
    Task<Cliente> CreateClienteAsync(User user);
    Task<Cliente?> GetClienteByUserIdAsync(int userId);
    Task<IEnumerable<Barbero>> SearchBarberosAsync(decimal? latitude, decimal? longitude, string? searchTerm);
    Task<IEnumerable<Barberia>> SearchBarberiasAsync(decimal? latitude, decimal? longitude, string? searchTerm);
    Task<Reservation> CreateReservationAsync(int clienteId, int serviceId, DateTime startDateTime, int? barberoId, int? barberiaId, string? notes);
    Task<bool> ConfirmReservationAsync(int reservationId);
    Task<bool> CancelReservationAsync(int reservationId, int clienteId);
    Task<bool> RejectReservationAsync(int reservationId, string reason);
    Task<IEnumerable<Reservation>> GetClienteReservationsAsync(int clienteId);
    Task<IEnumerable<Service>> GetAvailableServicesAsync(int? barberoId, int? barberiaId);
    Task<bool> IsTimeSlotAvailableAsync(int? barberoId, int? barberiaId, DateTime startDateTime, DateTime endDateTime);
}

public interface INotificationService
{
    Task<Notification> CreateNotificationAsync(int userId, string title, string message, NotificationType type, int? reservationId = null, int? barberoSubscriptionChangeId = null, int? barberiaSubscriptionChangeId = null);
    Task<IEnumerable<Notification>> GetUserNotificationsAsync(int userId);
    Task<bool> MarkNotificationAsReadAsync(int notificationId);
    Task<bool> MarkAllNotificationsAsReadAsync(int userId);
    Task<int> GetUnreadNotificationsCountAsync(int userId);
}

public interface IReservationService
{
    Task<Reservation?> GetReservationByIdAsync(int reservationId);
    Task<IEnumerable<Reservation>> GetReservationsByBarberoAsync(int barberoId, ReservationStatus? status = null);
    Task<IEnumerable<Reservation>> GetReservationsByBarberiaAsync(int barberiaId, ReservationStatus? status = null);
    Task<bool> HasOverlappingReservationAsync(int? barberoId, int? barberiaId, DateTime startDateTime, DateTime endDateTime, int? excludeReservationId = null);
}
