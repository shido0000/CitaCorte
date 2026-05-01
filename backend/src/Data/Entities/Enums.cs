namespace CitaCorte.API.Data.Entities;

public enum UserRole
{
    Admin,
    Barbero,
    Barberia,
    Comercial,
    Cliente
}

public enum SubscriptionType
{
    Free,
    Popular,
    Premium
}

public enum SubscriptionStatus
{
    Pending,
    Active,
    Expired,
    Rejected
}

public enum AffiliationStatus
{
    Pending,
    Accepted,
    Rejected
}

public enum ReservationStatus
{
    Pending,
    Confirmed,
    Cancelled,
    Completed
}

public enum NotificationType
{
    Info,
    Warning,
    Success,
    Error,
    AffiliationRequest,
    AffiliationResponse,
    SubscriptionChange,
    SubscriptionApproval,
    ReservationRequest,
    ReservationResponse
}
