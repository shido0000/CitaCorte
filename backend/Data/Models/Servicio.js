import mongoose from 'mongoose';

const servicioSchema = new mongoose.Schema({
  nombre: {
    type: String,
    required: [true, 'El nombre del servicio es requerido'],
    trim: true,
    maxlength: [100, 'El nombre no puede exceder 100 caracteres']
  },
  descripcion: {
    type: String,
    trim: true,
    maxlength: [500, 'La descripción no puede exceder 500 caracteres']
  },
  precio: {
    type: Number,
    required: [true, 'El precio es requerido'],
    min: [0, 'El precio no puede ser negativo']
  },
  duracionMinutos: {
    type: Number,
    required: [true, 'La duración es requerida'],
    min: [5, 'La duración mínima es 5 minutos']
  },
  proveedor: {
    tipo: {
      type: String,
      enum: ['barbero', 'barberia'],
      required: true
    },
    id: {
      type: mongoose.Schema.Types.ObjectId,
      required: true,
      refPath: 'proveedor.tipo'
    }
  },
  categoria: {
    type: String,
    trim: true,
    enum: ['corte', 'barba', 'color', 'tratamiento', 'peinado', 'otro']
  },
  activo: {
    type: Boolean,
    default: true
  },
  fotos: [{
    url: String,
    descripcion: String
  }]
}, {
  timestamps: true
});

// Índice para búsquedas
servicioSchema.index({ nombre: 'text', descripcion: 'text' });
servicioSchema.index({ 'proveedor.id': 1, activo: 1 });
servicioSchema.index({ categoria: 1 });

export default mongoose.model('Servicio', servicioSchema);
