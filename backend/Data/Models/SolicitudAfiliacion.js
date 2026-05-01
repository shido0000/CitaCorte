import mongoose from 'mongoose';

const solicitudAfiliacionSchema = new mongoose.Schema({
  barbero: {
    type: mongoose.Schema.Types.ObjectId,
    ref: 'Barbero',
    required: true
  },
  barberia: {
    type: mongoose.Schema.Types.ObjectId,
    ref: 'Barberia',
    required: true
  },
  estado: {
    type: String,
    enum: ['pendiente', 'aceptada', 'rechazada'],
    default: 'pendiente'
  },
  mensajeSolicitud: {
    type: String,
    trim: true,
    maxlength: [500, 'El mensaje no puede exceder 500 caracteres']
  },
  motivoRechazo: {
    type: String,
    trim: true,
    maxlength: [500, 'El motivo no puede exceder 500 caracteres']
  },
  fechas: {
    solicitud: {
      type: Date,
      default: Date.now
    },
    respuesta: Date
  }
}, {
  timestamps: true
});

// Índices para búsquedas
solicitudAfiliacionSchema.index({ barbero: 1, estado: 1 });
solicitudAfiliacionSchema.index({ barberia: 1, estado: 1 });

// Validar que solo pueda haber una solicitud pendiente entre barbero y barberia
solicitudAfiliacionSchema.statics.verificarSolicitudPendiente = async function(barberoId, barberiaId) {
  const solicitudPendiente = await this.findOne({
    barbero: barberoId,
    barberia: barberiaId,
    estado: 'pendiente'
  });
  return solicitudPendiente !== null;
};

export default mongoose.model('SolicitudAfiliacion', solicitudAfiliacionSchema);
