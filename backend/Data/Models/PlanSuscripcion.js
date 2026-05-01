import mongoose from 'mongoose';

const planSuscripcionSchema = new mongoose.Schema({
  nombre: {
    type: String,
    required: [true, 'El nombre del plan es requerido'],
    trim: true,
    maxlength: [50, 'El nombre no puede exceder 50 caracteres']
  },
  tipo: {
    type: String,
    enum: ['barbero', 'barberia'],
    required: [true, 'El tipo de plan es requerido']
  },
  nivel: {
    type: String,
    enum: ['free', 'popular', 'premium'], // Para barberos
    // Para barberias: 'basico', 'estandar', 'premium' se maneja en validación
    required: [true, 'El nivel del plan es requerido']
  },
  precio: {
    type: Number,
    required: [true, 'El precio es requerido'],
    min: [0, 'El precio no puede ser negativo']
  },
  duracionDias: {
    type: Number,
    required: [true, 'La duración del plan es requerida'],
    min: [1, 'La duración debe ser al menos 1 día']
  },
  caracteristicas: {
    puedeRecibirReservas: {
      type: Boolean,
      default: false
    },
    verEstadisticas: {
      type: Boolean,
      default: false
    },
    estadisticasAvanzadas: {
      type: Boolean,
      default: false
    },
    venderProductos: {
      type: Boolean,
      default: false
    },
    soportePrioritario: {
      type: Boolean,
      default: false
    }
  },
  limiteBarberos: {
    type: Number,
    default: 0, // Solo aplica para barberias
    min: [0, 'El límite no puede ser negativo']
  },
  descripcion: {
    type: String,
    trim: true,
    maxlength: [500, 'La descripción no puede exceder 500 caracteres']
  },
  activo: {
    type: Boolean,
    default: true
  }
}, {
  timestamps: true
});

// Validar que barberias no tengan nivel 'free'
planSuscripcionSchema.pre('validate', function(next) {
  if (this.tipo === 'barberia' && this.nivel === 'free') {
    const error = new Error('Las barberías no pueden tener un plan FREE');
    error.nombreError = 'ValidationError';
    return next(error);
  }
  next();
});

// Índice compuesto para búsquedas
planSuscripcionSchema.index({ tipo: 1, nivel: 1, activo: 1 });

export default mongoose.model('PlanSuscripcion', planSuscripcionSchema);
