import mongoose from 'mongoose';

const reservaSchema = new mongoose.Schema({
  cliente: {
    type: mongoose.Schema.Types.ObjectId,
    ref: 'Cliente',
    required: true
  },
  servicio: {
    tipo: mongoose.Schema.Types.ObjectId,
    ref: 'Servicio',
    required: true
  },
  barbero: {
    type: mongoose.Schema.Types.ObjectId,
    ref: 'Barbero',
    required: function() {
      return !this.barberia; // Requerido si no hay barberia
    }
  },
  barberia: {
    type: mongoose.Schema.Types.ObjectId,
    ref: 'Barberia',
    required: function() {
      return !this.barbero || this.barbero.afiliado; // Requerido si es barberia o barbero afiliado
    }
  },
  fechaInicio: {
    type: Date,
    required: [true, 'La fecha de inicio es requerida'],
    validate: {
      validator: function(value) {
        return value > new Date(Date.now() - 60000); // No permitir fechas pasadas (1 min margen)
      },
      message: 'La fecha de inicio debe ser futura'
    }
  },
  fechaFin: {
    type: Date,
    required: [true, 'La fecha de fin es requerida'],
    validate: {
      validator: function(value) {
        return value > this.fechaInicio;
      },
      message: 'La fecha de fin debe ser posterior a la fecha de inicio'
    }
  },
  estado: {
    type: String,
    enum: ['pendiente', 'confirmada', 'cancelada', 'completada', 'rechazada'],
    default: 'pendiente'
  },
  notas: {
    type: String,
    trim: true,
    maxlength: [500, 'Las notas no pueden exceder 500 caracteres']
  },
  total: {
    type: Number,
    required: true,
    min: [0, 'El total no puede ser negativo']
  },
  metodoPago: {
    type: String,
    enum: ['efectivo', 'tarjeta', 'transferencia', 'pendiente'],
    default: 'pendiente'
  },
  cancelacionMotivo: {
    type: String,
    maxlength: [300, 'El motivo no puede exceder 300 caracteres']
  },
  rechazoMotivo: {
    type: String,
    maxlength: [300, 'El motivo no puede exceder 300 caracteres']
  },
  recordatoriosEnviados: [{
    fecha: Date,
    tipo: {
      type: String,
      enum: ['email', 'sms', 'push']
    }
  }]
}, {
  timestamps: true
});

// Índices para búsquedas eficientes
reservaSchema.index({ cliente: 1, estado: 1 });
reservaSchema.index({ barbero: 1, fechaInicio: 1, estado: 1 });
reservaSchema.index({ barberia: 1, fechaInicio: 1, estado: 1 });
reservaSchema.index({ fechaInicio: 1 });

// Validación CRÍTICA: No solapamiento de fechas
reservaSchema.statics.verificarSolapamiento = async function({ barberoId, barberiaId, fechaInicio, fechaFin, excludeId = null }) {
  const query = {
    $or: [],
    estado: { $in: ['pendiente', 'confirmada'] },
    _id: excludeId ? { $ne: excludeId } : { $exists: true }
  };

  // Construir query según sea barbero o barberia
  if (barberoId) {
    query.$or.push(
      { 
        barbero: barberoId,
        fechaInicio: { $lt: fechaFin },
        fechaFin: { $gt: fechaInicio }
      }
    );
  }
  
  if (barberiaId) {
    query.$or.push(
      {
        barberia: barberiaId,
        fechaInicio: { $lt: fechaFin },
        fechaFin: { $gt: fechaInicio }
      }
    );
  }

  const reservaExistente = await this.findOne(query);
  return reservaExistente !== null; // Retorna true si hay conflicto
};

// Método de instancia para verificar solapamiento
reservaSchema.methods.verificarSolapamiento = async function() {
  const Model = this.constructor;
  return await Model.verificarSolapamiento({
    barberoId: this.barbero,
    barberiaId: this.barberia,
    fechaInicio: this.fechaInicio,
    fechaFin: this.fechaFin,
    excludeId: this._id
  });
};

// Virtual para obtener duración en minutos
reservaSchema.virtual('duracionMinutos').get(function() {
  return Math.round((this.fechaFin - this.fechaInicio) / 60000);
});

// Pre-save validation para solapamiento
reservaSchema.pre('save', async function(next) {
  // Solo validar en creación o cuando cambian las fechas
  if (!this.isNew && !this.isModified('fechaInicio') && !this.isModified('fechaFin')) {
    return next();
  }
  
  // Si la reserva está cancelada o rechazada, no validar solapamiento
  if (['cancelada', 'rechazada'].includes(this.estado)) {
    return next();
  }
  
  const haySolapamiento = await this.verificarSolapamiento();
  if (haySolapamiento) {
    const error = new Error('Ya existe una reserva en ese horario');
    error.nombreError = 'ValidationError';
    error.codigoError = 'SOLAPAMIENTO_RESERVA';
    return next(error);
  }
  
  next();
});

export default mongoose.model('Reserva', reservaSchema);
