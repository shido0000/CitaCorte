namespace CitaCorte.Data.Entities;

public enum Role
{
    Admin = 1,
    Barbero = 2,
    Barberia = 3,
    Comercial = 4,
    Cliente = 5
}

public enum SubscriptionType
{
    Free = 1,
    Popular = 2,
    Premium = 3
}

public enum SubscriptionStatus
{
    Pending = 1,
    Approved = 2,
    Rejected = 3,
    Expired = 4
}

public enum AffiliationStatus
{
    Pending = 1,
    Approved = 2,
    Rejected = 3
}

public enum ReservationStatus
{
    Pending = 1,
    Confirmed = 2,
    Cancelled = 3,
    Completed = 4,
    Rejected = 5
}

public enum NotificationType
{
    SubscriptionChange = 1,
    AffiliationRequest = 2,
    AffiliationResponse = 3,
    ReservationRequest = 4,
    ReservationResponse = 5,
    SubscriptionApproval = 6,
    General = 7
}
