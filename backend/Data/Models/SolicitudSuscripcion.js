import mongoose from 'mongoose';

const solicitudSuscripcionSchema = new mongoose.Schema({
  usuario: {
    type: mongoose.Schema.Types.ObjectId,
    ref: 'Usuario',
    required: true
  },
  tipoUsuario: {
    type: String,
    enum: ['barbero', 'barberia'],
    required: true
  },
  planSolicitado: {
    type: mongoose.Schema.Types.ObjectId,
    ref: 'PlanSuscripcion',
    required: true
  },
  estado: {
    type: String,
    enum: ['pendiente', 'aprobada', 'rechazada', 'enEspera'],
    default: 'pendiente'
  },
  motivoSolicitud: {
    type: String,
    trim: true,
    maxlength: [500, 'El motivo no puede exceder 500 caracteres']
  },
  motivoRechazo: {
    type: String,
    trim: true,
    maxlength: [500, 'El motivo no puede exceder 500 caracteres']
  },
  aprobacion: {
    admin: {
      aprobo: {
        type: mongoose.Schema.Types.ObjectId,
        ref: 'Usuario'
      },
      fecha: Date,
      comentarios: String
    },
    comercial: {
      aprobo: {
        type: mongoose.Schema.Types.ObjectId,
        ref: 'Usuario'
      },
      fecha: Date,
      comentarios: String
    }
  },
  fechas: {
    solicitud: {
      type: Date,
      default: Date.now
    },
    respuesta: Date,
    inicioVigencia: Date,
    finVigencia: Date
  }
}, {
  timestamps: true
});

// Índices para búsquedas
solicitudSuscripcionSchema.index({ usuario: 1, estado: 1 });
solicitudSuscripcionSchema.index({ tipoUsuario: 1, estado: 1 });
solicitudSuscripcionSchema.index({ 'aprobacion.admin.aprobo': 1 });
solicitudSuscripcionSchema.index({ 'aprobacion.comercial.aprobo': 1 });

// Validar que solo pueda haber una solicitud pendiente por usuario
solicitudSuscripcionSchema.statics.verificarSolicitudPendiente = async function(usuarioId) {
  const solicitudPendiente = await this.findOne({
    usuario: usuarioId,
    estado: { $in: ['pendiente', 'enEspera'] }
  });
  return solicitudPendiente !== null;
};

export default mongoose.model('SolicitudSuscripcion', solicitudSuscripcionSchema);
