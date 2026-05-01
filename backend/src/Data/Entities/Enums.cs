namespace CitaCorte.Data.Entities;

public enum RolEnum
{
    Admin = 1,
    Barbero = 2,
    Barberia = 3,
    Comercial = 4,
    Cliente = 5
}

public enum EstadoSuscripcionEnum
{
    Pendiente = 1,
    Activa = 2,
    Rechazada = 3,
    Vencida = 4
}

public enum EstadoReservaEnum
{
    Pendiente = 1,
    Confirmada = 2,
    Rechazada = 3,
    Cancelada = 4,
    Completada = 5
}

public enum EstadoSolicitudAfiliacionEnum
{
    Pendiente = 1,
    Aceptada = 2,
    Rechazada = 3
}

public enum TipoPlanEnum
{
    Free = 1,
    Popular = 2,
    Premium = 3
}
