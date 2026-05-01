import mongoose from 'mongoose';
import bcrypt from 'bcryptjs';

const usuarioSchema = new mongoose.Schema({
  email: {
    type: String,
    required: [true, 'El email es requerido'],
    unique: true,
    lowercase: true,
    trim: true,
    match: [/^\S+@\S+\.\S+$/, 'Por favor ingresa un email válido']
  },
  password: {
    type: String,
    required: [true, 'La contraseña es requerida'],
    minlength: [6, 'La contraseña debe tener al menos 6 caracteres'],
    select: false
  },
  nombre: {
    type: String,
    required: [true, 'El nombre es requerido'],
    trim: true,
    maxlength: [50, 'El nombre no puede exceder 50 caracteres']
  },
  apellido: {
    type: String,
    required: [true, 'El apellido es requerido'],
    trim: true,
    maxlength: [50, 'El apellido no puede exceder 50 caracteres']
  },
  telefono: {
    type: String,
    trim: true,
    maxlength: [20, 'El teléfono no puede exceder 20 caracteres']
  },
  foto: {
    type: String,
    default: ''
  },
  rol: {
    type: String,
    enum: ['admin', 'barbero', 'barberia', 'comercial', 'cliente'],
    required: [true, 'El rol es requerido']
  },
  activo: {
    type: Boolean,
    default: true
  }
}, {
  timestamps: true
});

// Hash de contraseña antes de guardar
usuarioSchema.pre('save', async function(next) {
  if (!this.isModified('password')) return next();
  
  const salt = await bcrypt.genSalt(10);
  this.password = await bcrypt.hash(this.password, salt);
  next();
});

// Método para comparar contraseñas
usuarioSchema.methods.compararPassword = async function(passwordCandidate) {
  return await bcrypt.compare(passwordCandidate, this.password);
};

// Método para obtener nombre completo
usuarioSchema.virtual('nombreCompleto').get(function() {
  return `${this.nombre} ${this.apellido}`;
});

export default mongoose.model('Usuario', usuarioSchema);
