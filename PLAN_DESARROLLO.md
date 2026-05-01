# Plan de Desarrollo - Sistema Multibarbero CitaCorte

## Objetivos Generales

Desarrollar un sistema multibarbero con 5 roles (Admin, Barbero, Barbería, Comercial, Cliente) que gestione suscripciones, reservas, afiliaciones y estadísticas.

---

## Objetivos por Módulo

### Objetivo 1: Estructura del Proyecto
- [x] Crear carpetas backend y frontend
- [ ] Configurar solución .NET con Entity Framework Core
- [ ] Configurar proyecto Vue 3 con Composition API

### Objetivo 2: Modelo de Datos (Backend - Data Layer)
- [ ] Crear entidades base: Usuario, Rol
- [ ] Crear entidad Suscripcion (planes para barberos y barberías)
- [ ] Crear entidad Barbero con relación a Suscripcion
- [ ] Crear entidad Barberia con relación a Suscripcion
- [ ] Crear entidad Servicio (servicios ofrecidos por barberos/barberías)
- [ ] Crear entidad Producto (solo plan Premium barbero)
- [ ] Crear entidad Reserva con validación de solapamiento
- [ ] Crear entidad SolicitudAfiliacion (barbero -> barbería)
- [ ] Crear entidad Notificacion
- [ ] Crear entidad Estadistica/Contabilidad
- [ ] Configurar DbContext con todas las relaciones
- [ ] Crear migraciones iniciales

### Objetivo 3: Capa de Servicio (Backend - Service Layer)
- [ ] Implementar servicio de Autenticación y Autorización
- [ ] Implementar servicio de Suscripciones (CRUD + aprobación)
- [ ] Implementar servicio de Barberos (registro, perfil, cambio suscripción)
- [ ] Implementar servicio de Barberías (registro, perfil, aprobación suscripción)
- [ ] Implementar servicio de Reservas (crear, confirmar, rechazar, validar solapamiento)
- [ ] Implementar servicio de Afiliaciones (solicitar, aceptar, rechazar)
- [ ] Implementar servicio de Notificaciones
- [ ] Implementar servicio de Estadísticas (barberos y barberías)
- [ ] Implementar servicio de Productos (solo premium)
- [ ] Implementar servicio de Comerciales (aprobar suscripciones)
- [ ] Implementar servicio de Admin (gestión total)

### Objetivo 4: Capa de Controladores (Backend - Controllers)
- [ ] AuthController (login, registro, logout)
- [ ] AdminController (gestión suscripciones, usuarios, estadísticas globales)
- [ ] BarberoController (perfil, servicios, reservas, afiliación, productos)
- [ ] BarberiaController (perfil, servicios, reservas, afiliados)
- [ ] ComercialController (aprobar suscripciones, ver estadísticas)
- [ ] ClienteController (registro, reservas, historial)
- [ ] ReservaController (gestión de reservas)
- [ ] NotificacionController (listar, marcar como leída)

### Objetivo 5: Frontend - Estructura Base
- [ ] Configurar Vue 3 con Vite
- [ ] Instalar dependencias (Vue Router, Pinia, Axios, librería de estilos)
- [ ] Configurar router con rutas protegidas por rol
- [ ] Crear stores para autenticación y gestión de estado
- [ ] Crear servicios API para comunicación con backend

### Objetivo 6: Frontend - Vistas por Rol

#### Admin
- [ ] Dashboard con estadísticas globales
- [ ] Gestión de planes de suscripción (crear, editar, eliminar)
- [ ] Listado de barberos con detalles
- [ ] Listado de barberías con detalles
- [ ] Aprobación de cambios de suscripción
- [ ] Aprobación de nuevas barberías

#### Barbero
- [ ] Registro (plan Free por defecto)
- [ ] Dashboard con estadísticas básicas
- [ ] Gestión de perfil y datos personales
- [ ] Gestión de servicios
- [ ] Solicitud de afiliación a barbería
- [ ] Cambio de suscripción (Free -> Popular -> Premium)
- [ ] Gestión de reservas (confirmar/rechazar)
- [ ] Contabilidad de servicios y clientes (Popular+)
- [ ] Gestión de productos (solo Premium)

#### Barbería
- [ ] Registro con selección de plan (sin Free)
- [ ] Dashboard con estadísticas
- [ ] Gestión de perfil y datos
- [ ] Gestión de servicios
- [ ] Gestión de barberos afiliados
- [ ] Aceptación/Rechazo de solicitudes de afiliación
- [ ] Gestión de reservas (confirmar/rechazar)
- [ ] Validación de límite de barberos según plan

#### Comercial
- [ ] Dashboard con estadísticas
- [ ] Aprobación de suscripciones de barberías
- [ ] Aprobación de cambios de suscripción de barberos
- [ ] Listado de barberos y barberías

#### Cliente
- [ ] Registro
- [ ] Búsqueda de barberos
- [ ] Búsqueda de barberías
- [ ] Realizar reservas
- [ ] Ver historial de reservas
- [ ] Cancelar reservas (antes de confirmación)

### Objetivo 7: Validaciones de Flujos
- [ ] Validar que barbero Free no pueda recibir reservas
- [ ] Validar solapamiento de fechas en reservas
- [ ] Validar que barbería sin suscripción aprobada no pueda afiliar
- [ ] Validar límite de barberos por plan de barbería
- [ ] Validar fecha de vencimiento de suscripciones
- [ ] Validar flujo de aprobación de suscripciones (Admin + Comercial)
- [ ] Validar redirección de reservas cuando barbero está afiliado
- [ ] Validar notificaciones en todos los flujos

### Objetivo 8: Pruebas y Documentación
- [ ] Documentar endpoints API
- [ ] Pruebas de integración de flujos críticos
- [ ] Manual de usuario básico

---

## Flujos Críticos a Validar

### Flujo 1: Registro de Barbero
1. Barbero se registra → Plan Free por defecto
2. Puede editar perfil y mostrar servicios
3. NO puede recibir reservas
4. Solicita cambio a Popular → Notificación a Admin y Comercial
5. Admin/Comercial aprueba → Suscripción cambia → Puede recibir reservas

### Flujo 2: Registro de Barbería
1. Barbería se registra → Selecciona plan (NO existe Free)
2. Completa requisitos → Notificación a Admin y Comercial
3. Barbería registrada pero NO puede afiliar hasta aprobación
4. Admin/Comercial aprueba → Barbería puede afiliar barberos

### Flujo 3: Afiliación Barbero-Barbería
1. Barbero solicita afiliación a barbería
2. Barbería recibe notificación
3. Barbería acepta → Barbero afiliado
   - Reservas al barbero se redirigen a barbería
4. Barbería rechaza → Notificación a ambas partes
5. Barbero notificado en ambos casos

### Flujo 4: Reserva de Cliente
1. Cliente busca barbero/barbería
2. Si barbero está afiliado → Redirigir a barbería
3. Cliente selecciona servicio y fecha
4. Sistema valida NO solapamiento
5. Sistema valida plan del barbero (Free = NO reserva)
6. Reserva creada como "Pendiente"
7. Barbero/Barbería confirma o rechaza
8. Notificación al cliente

### Flujo 5: Vencimiento de Suscripción
1. Sistema verifica fecha vencimiento
2. Notificación próxima a vencer
3. Al vencer:
   - Barbero: Vuelve a Free (si era Popular/Premium)
   - Barbería: No puede operar hasta renovar

---

## Reglas de Negocio Importantes

1. **Solapamiento**: Ninguna reserva puede tener fechas superpuestas para el mismo recurso (barbero o silla de barbería)
2. **Plan Free**: Solo exposición, sin reservas
3. **Plan Popular**: Reservas + estadísticas básicas
4. **Plan Premium**: Popular + productos en venta
5. **Aprobación Dual**: Cambios de suscripción requieren aprobación de Admin O Comercial
6. **Redirección**: Barbero afiliado → reservas van a barbería
7. **Límites**: Cada plan de barbería tiene máximo de barberos afiliados
8. **Temporalidad**: Todas las suscripciones tienen fecha de vencimiento

---

## Tecnologías

### Backend
- .NET 8
- Entity Framework Core
- SQL Server
- Arquitectura en 3 capas: Data, Service, Controllers

### Frontend
- Vue 3 (Composition API)
- Vue Router
- Pinia (estado)
- Axios (HTTP)
- Librería de estilos: PrimeVue o TailwindCSS

---

## Estructura de Carpetas Final

```
/workspace
├── backend/
│   ├── src/
│   │   ├── Data/
│   │   │   ├── Entities/
│   │   │   ├── Config/
│   │   │   └── AppDbContext.cs
│   │   ├── Service/
│   │   │   ├── Interfaces/
│   │   │   └── Implementations/
│   │   └── Controllers/
│   └── Program.cs
│
└── frontend/
    ├── src/
    │   ├── components/
    │   ├── views/
    │   ├── router/
    │   ├── stores/
    │   ├── services/
    │   └── composables/
    └── package.json
```
