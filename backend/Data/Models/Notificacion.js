import mongoose from 'mongoose';

const notificacionSchema = new mongoose.Schema({
  destinatario: {
    type: mongoose.Schema.Types.ObjectId,
    ref: 'Usuario',
    required: true
  },
  tipo: {
    type: String,
    enum: ['suscripcion', 'afiliacion', 'reserva', 'sistema', 'producto'],
    required: true
  },
  titulo: {
    type: String,
    required: [true, 'El título es requerido'],
    trim: true,
    maxlength: [100, 'El título no puede exceder 100 caracteres']
  },
  mensaje: {
    type: String,
    required: [true, 'El mensaje es requerido'],
    trim: true,
    maxlength: [1000, 'El mensaje no puede exceder 1000 caracteres']
  },
  leido: {
    type: Boolean,
    default: false
  },
  datosAdicionales: {
    tipoDato: String, // 'solicitudSuscripcion', 'solicitudAfiliacion', 'reserva', etc.
    idReferencia: mongoose.Schema.Types.ObjectId,
    metadata: Object // Datos extra en formato JSON
  },
  acciones: [{
    texto: String,
    accion: String, // URL o acción a realizar
    tipo: {
      type: String,
      enum: ['aceptar', 'rechazar', 'ver', 'cancelar'],
      default: 'ver'
    }
  }],
  prioridad: {
    type: String,
    enum: ['baja', 'media', 'alta', 'urgente'],
    default: 'media'
  }
}, {
  timestamps: true
});

// Índices para búsquedas eficientes
notificacionSchema.index({ destinatario: 1, leido: 1 });
notificacionSchema.index({ destinatario: 1, createdAt: -1 });
notificacionSchema.index({ tipo: 1, leido: 1 });

// Método estático para crear notificación masiva
notificacionSchema.statics.crearNotificacionMasiva = async function(destinatarios, datosNotificacion) {
  const notificaciones = destinatarios.map(destinatarioId => ({
    destinatario: destinatarioId,
    ...datosNotificacion
  }));
  
  return await this.insertMany(notificaciones);
};

export default mongoose.model('Notificacion', notificacionSchema);
