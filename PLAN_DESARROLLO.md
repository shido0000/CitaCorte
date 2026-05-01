/**
 * PLAN DE DESARROLLO - SISTEMA MULTIBARBERO CITACORTE
 * 
 * Este documento describe el plan por objetivos para implementar el sistema.
 */

# OBJETIVOS GENERALES

## 1. ESTRUCTURA DEL PROYECTO
- [x] Crear carpeta backend con arquitectura de 3 capas (Data, Service, Controllers)
- [x] Crear carpeta frontend con Vue 3 Composition API

## 2. MODELOS DE DATOS (Backend - Data/Models)

### 2.1 Modelo de Usuario (Base para todos los roles)
- [ ] Campos: email, password, nombre, apellido, telefono, foto, rol, activo, createdAt, updatedAt

### 2.2 Modelo de Admin
- [ ] Extiende de Usuario
- [ ] Permisos completos del sistema

### 2.3 Modelo de Barbero
- [ ] Extiende de Usuario
- [ ] Campos adicionales: bio, especialidades, experiencia, calificacionPromedio
- [ ] Relación con PlanSuscripcion (default: FREE)
- [ ] Relación con Barberia (solicitud de afiliación)
- [ ] Estados de afiliación: pendiente, aceptado, rechazado, independiente

### 2.4 Modelo de Barberia
- [ ] Extiende de Usuario (o entidad separada)
- [ ] Campos: nombreComercial, direccion, descripcion, fotos, telefono, horarios
- [ ] Relación con PlanSuscripcion (SIN plan FREE)
- [ ] Relación con BarberosAfiliados[]
- [ ] Limite de barberos según plan

### 2.5 Modelo de Comercial
- [ ] Extiende de Usuario
- [ ] Permisos: ver estadísticas, aprobar/rechazar suscripciones

### 2.6 Modelo de Cliente
- [ ] Extiende de Usuario
- [ ] Sin plan de suscripción
- [ ] Historial de reservas

### 2.7 Modelo de PlanSuscripcion
- [ ] Tipos: barberia, barbero
- [ ] Niveles: free, popular, premium (barbero); basico, estandar, premium (barberia)
- [ ] Campos: nombre, tipo, precio, duracionDias, caracteristicas{}, limiteBarberos, activo
- [ ] Fechas: inicio, vencimiento

### 2.8 Modelo de Servicio
- [ ] Relación con Barbero o Barberia
- [ ] Campos: nombre, descripcion, precio, duracionMinutos, activo

### 2.9 Modelo de Reserva
- [ ] Relación con Cliente, Barbero, Barberia (opcional), Servicio
- [ ] Campos: fechaInicio, fechaFin, estado (pendiente, confirmada, cancelada, completada, rechazada)
- [ ] Validación: NO SOLAPAMIENTO de fechas
- [ ] Campos: notas, total, createdAt

### 2.10 Modelo de SolicitudSuscripcion
- [ ] Relación con Usuario (Barbero/Barberia)
- [ ] Plan solicitado
- [ ] Estado: pendiente, aprobada, rechazada, enEspera
- [ ] Relación con Admin y Comercial que aprobaron
- [ ] Fechas: solicitud, respuesta, vencimiento

### 2.11 Modelo de SolicitudAfiliacion
- [ ] Barbero solicitante
- [ ] Barberia destino
- [ ] Estado: pendiente, aceptada, rechazada
- [ ] Fechas: solicitud, respuesta

### 2.12 Modelo de Notificacion
- [ ] Relación con Usuario destinatario
- [ ] Tipo: suscripcion, afiliacion, reserva, sistema
- [ ] Campos: titulo, mensaje, leido, createdAt, datosAdicionales{}

### 2.13 Modelo de Producto (Premium Barbero)
- [ ] Relación con Barbero
- [ ] Campos: nombre, descripcion, precio, fotos[], stock, activo

## 3. SERVICIOS (Backend - Service/)

### 3.1 AuthService
- [ ] register(usuario, rol)
- [ ] login(email, password)
- [ ] logout()
- [ ] refreshToken()
- [ ] validateToken()

### 3.2 AdminService
- [ ] crearPlanSuscripcion(datosPlan)
- [ ] editarPlanSuscripcion(id, datosPlan)
- [ ] eliminarPlanSuscripcion(id)
- [ ] listarPlanesSuscripcion(filtros)
- [ ] verEstadisticasGenerales()
- [ ] verTodosBarberos(filtros)
- [ ] verTodasBarberias(filtros)
- [ ] aprobarSuscripcion(solicitudId, decision)
- [ ] rechazarSuscripcion(solicitudId, motivo)

### 3.3 BarberoService
- [ ] registrarBarbero(datos)
- [ ] actualizarPerfilBarbero(id, datos)
- [ ] obtenerPerfilBarbero(id)
- [ ] crearServicio(barberoId, servicioData)
- [ ] editarServicio(servicioId, servicioData)
- [ ] eliminarServicio(servicioId)
- [ ] listarServicios(barberoId)
- [ ] solicitarAfiliacionBarberia(barberoId, barberiaId)
- [ ] cancelarSolicitudAfiliacion(solicitudId)
- [ ] cambiarSuscripcion(barberoId, planId)
- [ ] obtenerEstadisticas(barberoId, rangoFechas)
- [ ] crearProducto(barberoId, productoData) - solo Premium
- [ ] editarProducto(productoId, productoData) - solo Premium
- [ ] eliminarProducto(productoId) - solo Premium
- [ ] listarProductos(barberoId) - solo Premium

### 3.4 BarberiaService
- [ ] registrarBarberia(datos)
- [ ] actualizarPerfilBarberia(id, datos)
- [ ] obtenerPerfilBarberia(id)
- [ ] listarBarberosAfiliados(barberiaId)
- [ ] responderSolicitudAfiliacion(solicitudId, aceptar, motivo)
- [ ] removerBarberoAfiliado(barberiaId, barberoId)
- [ ] crearServicio(barberiaId, servicioData)
- [ ] editarServicio(servicioId, servicioData)
- [ ] eliminarServicio(servicioId)
- [ ] listarServicios(barberiaId)
- [ ] cambiarSuscripcion(barberiaId, planId)
- [ ] obtenerEstadisticas(barberiaId, rangoFechas)
- [ ] verificarLimiteBarberos(barberiaId)

### 3.5 ComercialService
- [ ] verEstadisticasBarberos(filtros)
- [ ] verEstadisticasBarberias(filtros)
- [ ] aprobarSuscripcion(solicitudId, decision)
- [ ] rechazarSuscripcion(solicitudId, motivo)
- [ ] listarSolicitudesPendientes(tipo)

### 3.6 ClienteService
- [ ] registrarCliente(datos)
- [ ] actualizarPerfilCliente(id, datos)
- [ ] buscarBarberos(filtros)
- [ ] buscarBarberias(filtros)
- [ ] crearReserva(reservaData)
- [ ] cancelarReserva(reservaId)
- [ ] listarMisReservas(clienteId, filtros)
- [ ] confirmarReserva(reservaId) - cuando barbero/barberia confirma
- [ ] rechazarReserva(reservaId) - cliente puede rechazar antes de confirmación

### 3.7 ReservaService
- [ ] crearReserva(data)
- [ ] validarDisponibilidad(barberoId/barberiaId, fechaInicio, fechaFin)
- [ ] confirmarReserva(reservaId)
- [ ] cancelarReserva(reservaId, motivo)
- [ ] rechazarReserva(reservaId, motivo)
- [ ] completarReserva(reservaId)
- [ ] verificarSolapamiento(barberoId/barberiaId, fechaInicio, fechaFin, excludeId)

### 3.8 NotificacionService
- [ ] crearNotificacion(userId, tipo, titulo, mensaje, datosAdicionales)
- [ ] listarNotificaciones(userId, filtros)
- [ ] marcarComoLeida(notificacionId)
- [ ] marcarTodasComoLeidas(userId)
- [ ] enviarNotificacionSuscripcion(usuarioId, tipo, decision)
- [ ] enviarNotificacionAfiliacion(usuarioId, tipo, decision)
- [ ] enviarNotificacionReserva(usuarioId, tipo, datosReserva)

### 3.9 SuscripcionService
- [ ] verificarEstadoSuscripcion(usuarioId)
- [ ] verificarVencimientoSuscripcion(usuarioId)
- [ ] obtenerCaracteristicasPlan(planId)
- [ ] calcularFechaVencimiento(fechaInicio, duracionDias)
- [ ] validarPermisosPorPlan(usuarioId, accion)

## 4. CONTROLADORES (Backend - Controllers/)

### 4.1 AuthController
- [ ] POST /api/auth/register
- [ ] POST /api/auth/login
- [ ] POST /api/auth/logout
- [ ] GET /api/auth/me
- [ ] PUT /api/auth/update-profile

### 4.2 AdminController
- [ ] POST /api/admin/planes
- [ ] PUT /api/admin/planes/:id
- [ ] DELETE /api/admin/planes/:id
- [ ] GET /api/admin/planes
- [ ] GET /api/admin/estadisticas
- [ ] GET /api/admin/barberos
- [ ] GET /api/admin/barberias
- [ ] POST /api/admin/suscripciones/:id/aprobar
- [ ] POST /api/admin/suscripciones/:id/rechazar

### 4.3 BarberoController
- [ ] GET /api/barbero/perfil
- [ ] PUT /api/barbero/perfil
- [ ] POST /api/barbero/servicios
- [ ] PUT /api/barbero/servicios/:id
- [ ] DELETE /api/barbero/servicios/:id
- [ ] GET /api/barbero/servicios
- [ ] POST /api/barbero/afiliacion/solicitar
- [ ] POST /api/barbero/suscripcion/cambiar
- [ ] GET /api/barbero/estadisticas
- [ ] POST /api/barbero/productos (solo Premium)
- [ ] PUT /api/barbero/productos/:id (solo Premium)
- [ ] DELETE /api/barbero/productos/:id (solo Premium)
- [ ] GET /api/barbero/productos (solo Premium)

### 4.4 BarberiaController
- [ ] GET /api/barberia/perfil
- [ ] PUT /api/barberia/perfil
- [ ] GET /api/barberia/barberos
- [ ] POST /api/barberia/afiliacion/:solicitudId/responder
- [ ] DELETE /api/barberia/barberos/:barberoId
- [ ] POST /api/barberia/servicios
- [ ] PUT /api/barberia/servicios/:id
- [ ] DELETE /api/barberia/servicios/:id
- [ ] GET /api/barberia/servicios
- [ ] POST /api/barberia/suscripcion/cambiar
- [ ] GET /api/barberia/estadisticas

### 4.5 ComercialController
- [ ] GET /api/comercial/estadisticas/barberos
- [ ] GET /api/comercial/estadisticas/barberias
- [ ] GET /api/comercial/solicitudes/pendientes
- [ ] POST /api/comercial/suscripciones/:id/aprobar
- [ ] POST /api/comercial/suscripciones/:id/rechazar

### 4.6 ClienteController
- [ ] GET /api/cliente/perfil
- [ ] PUT /api/cliente/perfil
- [ ] GET /api/cliente/barberos/buscar
- [ ] GET /api/cliente/barberias/buscar
- [ ] POST /api/cliente/reservas
- [ ] GET /api/cliente/reservas
- [ ] PUT /api/cliente/reservas/:id/cancelar
- [ ] PUT /api/cliente/reservas/:id/rechazar

### 4.7 ReservaController
- [ ] POST /api/reservas
- [ ] GET /api/reservas/:id
- [ ] PUT /api/reservas/:id/confirmar
- [ ] PUT /api/reservas/:id/cancelar
- [ ] PUT /api/reservas/:id/rechazar
- [ ] PUT /api/reservas/:id/completar

### 4.8 NotificacionController
- [ ] GET /api/notificaciones
- [ ] PUT /api/notificaciones/:id/leer
- [ ] PUT /api/notificaciones/leer-todas
- [ ] GET /api/notificaciones/no-leidas/count

## 5. MIDDLEWARES (Backend - Middlewares/)

### 5.1 AuthMiddleware
- [ ] verifyToken
- [ ] optionalAuth

### 5.2 RoleMiddleware
- [ ] checkRole(roles[])
- [ ] checkPermission(permision)

### 5.3 SubscriptionMiddleware
- [ ] checkSubscriptionActive
- [ ] checkSubscriptionLevel(nivelRequerido)
- [ ] checkAffiliationStatus

### 5.4 ValidationMiddleware
- [ ] validateRequest(schema)

## 6. FRONTEND (Vue 3 Composition API)

### 6.1 Estructura de Vistas

#### 6.1.1 Vistas Públicas
- [ ] Home (/)
- [ ] Login (/login)
- [ ] Registro (/registro/:rol)
- [ ] BuscarBarberos (/buscar/barberos)
- [ ] BuscarBarberias (/buscar/barberias)
- [ ] PerfilBarbero (/barbero/:id)
- [ ] PerfilBarberia (/barberia/:id)
- [ ] DetalleServicio (/servicio/:id)

#### 6.1.2 Dashboard Admin (/admin/*)
- [ ] DashboardAdmin (/admin/dashboard)
- [ ] GestionPlanes (/admin/planes)
- [ ] VerBarberos (/admin/barberos)
- [ ] VerBarberias (/admin/barberias)
- [ ] SolicitudesSuscripcion (/admin/solicitudes)
- [ ] Estadisticas (/admin/estadisticas)

#### 6.1.3 Dashboard Barbero (/barbero/*)
- [ ] DashboardBarbero (/barbero/dashboard)
- [ ] MiPerfil (/barbero/perfil)
- [ ] MisServicios (/barbero/servicios)
- [ ] SolicitarAfiliacion (/barbero/afiliacion)
- [ ] CambiarSuscripcion (/barbero/suscripcion)
- [ ] Estadisticas (/barbero/estadisticas)
- [ ] Productos (/barbero/productos) - solo Premium
- [ ] Reservas (/barbero/reservas)

#### 6.1.4 Dashboard Barberia (/barberia/*)
- [ ] DashboardBarberia (/barberia/dashboard)
- [ ] MiPerfil (/barberia/perfil)
- [ ] BarberosAfiliados (/barberia/barberos)
- [ ] SolicitudesAfiliacion (/barberia/solicitudes)
- [ ] MisServicios (/barberia/servicios)
- [ ] CambiarSuscripcion (/barberia/suscripcion)
- [ ] Estadisticas (/barberia/estadisticas)
- [ ] Reservas (/barberia/reservas)

#### 6.1.5 Dashboard Comercial (/comercial/*)
- [ ] DashboardComercial (/comercial/dashboard)
- [ ] EstadisticasBarberos (/comercial/estadisticas/barberos)
- [ ] EstadisticasBarberias (/comercial/estadisticas/barberias)
- [ ] SolicitudesSuscripcion (/comercial/solicitudes)

#### 6.1.6 Dashboard Cliente (/cliente/*)
- [ ] DashboardCliente (/cliente/dashboard)
- [ ] MiPerfil (/cliente/perfil)
- [ ] MisReservas (/cliente/reservas)
- [ ] HistorialReservas (/cliente/historial)

### 6.2 Componentes Reutilizables
- [ ] Navbar
- [ ] Sidebar (por rol)
- [ ] Footer
- [ ] CardServicio
- [ ] CardBarbero
- [ ] CardBarberia
- [ ] CardReserva
- [ ] CalendarPicker
- [ ] TimeSlotSelector
- [ ] ModalConfirmacion
- [ ] NotificacionToast
- [ ] TablaDatos
- [ ] EstadisticaCard
- [ ] PlanCard
- [ ] FileUploader

### 6.3 Stores (Pinia)
- [ ] authStore (usuario actual, token, login, logout)
- [ ] barberoStore
- [ ] barberiaStore
- [ ] clienteStore
- [ ] adminStore
- [ ] comercialStore
- [ ] reservasStore
- [ ] notificacionesStore
- [ ] suscripcionesStore

### 6.4 Servicios API (src/services/)
- [ ] authService.js
- [ ] barberoService.js
- [ ] barberiaService.js
- [ ] clienteService.js
- [ ] adminService.js
- [ ] comercialService.js
- [ ] reservaService.js
- [ ] notificacionService.js
- [ ] suscripcionService.js

### 6.5 Composables
- [ ] useAuth.js
- [ ] usePermissions.js
- [ ] useSubscription.js
- [ ] useReservas.js
- [ ] useNotificaciones.js
- [ ] useFormValidation.js
- [ ] useDateFormat.js

## 7. VALIDACIONES Y REGLAS DE NEGOCIO

### 7.1 Validación de Solapamiento de Reservas
```javascript
// Al crear reserva, verificar:
function verificarSolapamiento(barberoId, barberiaId, fechaInicio, fechaFin, reservaIdExcluir = null) {
  // 1. Si es barbero independiente, verificar sus reservas
  // 2. Si es barbero afiliado, verificar reservas de la barberia
  // 3. Si es barberia, verificar todas las reservas de esa barberia
  // 4. Excluir la reserva actual si es una actualización
  // 5. Retornar true si hay conflicto, false si está disponible
}
```

### 7.2 Flujo de Suscripciones

#### Barbero:
1. Registro automático → Plan FREE
2. FREE: Puede editar perfil, mostrar servicios, NO puede recibir reservas
3. Solicita cambio a Popular/Premium → Notificación a Admin y Comercial
4. Admin/Comercial aprueba → Se activa nuevo plan
5. Popular: Puede recibir reservas, ver estadísticas básicas
6. Premium: Todo lo de Popular + vender productos, estadísticas avanzadas

#### Barberia:
1. Registro → SELECCIONA plan (NO existe FREE)
2. Completa registro → Notificación a Admin y Comercial
3. Pendiente de aprobación → NO puede afiliar barberos
4. Admin/Comercial aprueba → Plan activo, puede afiliar barberos
5. Límite de barberos según plan contratado

### 7.3 Flujo de Afiliación Barbero-Barberia
1. Barbero solicita afiliación a Barberia
2. Barberia recibe notificación
3. Barberia acepta o rechaza
4. Si acepta:
   - Barbero queda afiliado
   - Búsquedas del barbero redirigen a barberia
   - Reservas del barbero se gestionan por barberia
5. Si rechaza:
   - Barbero recibe notificación
   - Barbero puede solicitar a otra barberia
6. Notificaciones en ambos casos

### 7.4 Flujo de Reservas
1. Cliente busca barbero/barberia
2. Selecciona servicio y horario disponible
3. Crea reserva (estado: PENDIENTE)
4. Barbero/Barberia recibe notificación
5. Barbero/Barberia confirma o rechaza
6. Si confirma → estado: CONFIRMADA
7. Si rechaza → estado: RECHAZADA, cliente notificado
8. Cliente puede cancelar reservas pendientes o confirmadas
9. Después del servicio → estado: COMPLETADA

## 8. CONSIDERACIONES TÉCNICAS

### 8.1 Seguridad
- [ ] Hash de contraseñas con bcryptjs
- [ ] JWT para autenticación
- [ ] Middleware de autorización por roles
- [ ] Validación de datos de entrada
- [ ] Rate limiting (opcional)
- [ ] CORS configurado

### 8.2 Base de Datos (MongoDB)
- [ ] Índices en campos de búsqueda frecuente
- [ ] Validaciones a nivel de schema
- [ ] Transacciones para operaciones críticas
- [ ] Soft deletes donde aplique

### 8.3 Notificaciones
- [ ] Sistema de notificaciones internas
- [ ] Email opcional (nodemailer)
- [ ] WebSockets para tiempo real (opcional fase 2)

### 8.4 Manejo de Errores
- [ ] Global error handler
- [ ] Logs de errores
- [ ] Respuestas estandarizadas

## 9. ORDEN DE IMPLEMENTACIÓN SUGERIDO

### Fase 1: Fundación (Semana 1-2)
1. Configurar proyecto backend y frontend
2. Implementar modelos de datos básicos
3. Sistema de autenticación y autorización
4. CRUD de usuarios por rol

### Fase 2: Núcleo del Negocio (Semana 3-4)
1. Sistema de planes de suscripción
2. Gestión de servicios
3. Sistema de reservas con validación de solapamiento
4. Búsqueda de barberos y barberias

### Fase 3: Flujos Complejos (Semana 5-6)
1. Sistema de afiliación barbero-barberia
2. Aprobación de suscripciones (Admin/Comercial)
3. Notificaciones
4. Estadísticas básicas

### Fase 4: Características Avanzadas (Semana 7-8)
1. Productos para barberos Premium
2. Estadísticas avanzadas
3. Dashboard completos por rol
4. Pruebas y ajustes finales

## 10. PRUEBAS

### 10.1 Backend
- [ ] Tests unitarios de servicios
- [ ] Tests de integración de controladores
- [ ] Tests de flujos completos (reservas, suscripciones, afiliaciones)

### 10.2 Frontend
- [ ] Tests de componentes críticos
- [ ] Tests de flujos de usuario
- [ ] Pruebas E2E de escenarios principales

---

**NOTA**: Este plan debe seguirse de manera iterativa, validando cada objetivo antes de pasar al siguiente.
