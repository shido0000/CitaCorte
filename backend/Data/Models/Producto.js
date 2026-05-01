import mongoose from 'mongoose';

const productoSchema = new mongoose.Schema({
  barbero: {
    type: mongoose.Schema.Types.ObjectId,
    ref: 'Barbero',
    required: true
  },
  nombre: {
    type: String,
    required: [true, 'El nombre del producto es requerido'],
    trim: true,
    maxlength: [100, 'El nombre no puede exceder 100 caracteres']
  },
  descripcion: {
    type: String,
    trim: true,
    maxlength: [1000, 'La descripción no puede exceder 1000 caracteres']
  },
  precio: {
    type: Number,
    required: [true, 'El precio es requerido'],
    min: [0, 'El precio no puede ser negativo']
  },
  stock: {
    type: Number,
    required: [true, 'El stock es requerido'],
    min: [0, 'El stock no puede ser negativo'],
    default: 0
  },
  categoria: {
    type: String,
    trim: true,
    enum: ['productos_cabello', 'productos_barba', 'accesorios', 'cuidado_personal', 'otros']
  },
  fotos: [{
    url: String,
    descripcion: String,
    principal: {
      type: Boolean,
      default: false
    }
  }],
  activo: {
    type: Boolean,
    default: true
  },
  caracteristicas: {
    marca: String,
    tamanio: String,
    ingredientes: [String],
    instruccionesUso: String
  }
}, {
  timestamps: true
});

// Índices para búsquedas
productoSchema.index({ barbero: 1, activo: 1 });
productoSchema.index({ nombre: 'text', descripcion: 'text' });
productoSchema.index({ categoria: 1 });

export default mongoose.model('Producto', productoSchema);
