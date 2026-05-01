import mongoose from 'mongoose';

const barberoSchema = new mongoose.Schema({
  usuario: {
    type: mongoose.Schema.Types.ObjectId,
    ref: 'Usuario',
    required: true
  },
  bio: {
    type: String,
    trim: true,
    maxlength: [500, 'La bio no puede exceder 500 caracteres']
  },
  especialidades: [{
    type: String,
    trim: true
  }],
  experiencia: {
    type: Number,
    min: [0, 'La experiencia no puede ser negativa'],
    default: 0 // años de experiencia
  },
  calificacionPromedio: {
    type: Number,
    default: 0,
    min: [0, 'La calificación no puede ser menor a 0'],
    max: [5, 'La calificación no puede ser mayor a 5']
  },
  planSuscripcionActual: {
    plan: {
      type: mongoose.Schema.Types.ObjectId,
      ref: 'PlanSuscripcion',
      default: null // Se asignará el plan FREE por defecto
    },
    fechaInicio: Date,
    fechaVencimiento: Date,
    estado: {
      type: String,
      enum: ['activo', 'vencido', 'pendiente_aprobacion'],
      default: 'pendiente_aprobacion' // FREE se activa automáticamente
    }
  },
  afiliacionBarberia: {
    barberia: {
      type: mongoose.Schema.Types.ObjectId,
      ref: 'Barberia',
      default: null
    },
    estado: {
      type: String,
      enum: ['independiente', 'pendiente', 'afiliado', 'rechazado'],
      default: 'independiente'
    },
    fechaSolicitud: Date,
    fechaRespuesta: Date,
    motivoRechazo: String
  },
  serviciosOfrecidos: [{
    tipo: mongoose.Schema.Types.ObjectId,
    ref: 'Servicio'
  }],
  productosVenta: [{
    tipo: mongoose.Schema.Types.ObjectId,
    ref: 'Producto'
  }]
}, {
  timestamps: true
});

// Índice para búsquedas
barberoSchema.index({ usuario: 1 });
barberoSchema.index({ 'afiliacionBarberia.estado': 1 });
barberoSchema.index({ especialidades: 1 });

// Virtual para obtener si puede recibir reservas
barberoSchema.virtual('puedeRecibirReservas').get(function() {
  if (this.planSuscripcionActual?.estado !== 'activo') return false;
  
  // Si está afiliado a una barberia, depende del plan de la barberia
  if (this.afiliacionBarberia.estado === 'afiliado') return true;
  
  // Si es independiente, depende de su plan
  return this.planSuscripcionActual?.plan?.caracteristicas?.puedeRecibirReservas || false;
});

// Método para verificar si tiene plan premium
barberoSchema.methods.esPremium = function() {
  return this.planSuscripcionActual?.plan?.nivel === 'premium';
};

export default mongoose.model('Barbero', barberoSchema);
