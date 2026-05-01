import mongoose from 'mongoose';

const clienteSchema = new mongoose.Schema({
  usuario: {
    type: mongoose.Schema.Types.ObjectId,
    ref: 'Usuario',
    required: true
  },
  fechaNacimiento: Date,
  genero: {
    type: String,
    enum: ['masculino', 'femenino', 'otro', 'prefiero_no_decirlo']
  },
  preferenciasNotificacion: {
    email: {
      type: Boolean,
      default: true
    },
    sms: {
      type: Boolean,
      default: false
    },
    push: {
      type: Boolean,
      default: true
    }
  },
  historialReservas: [{
    tipo: mongoose.Schema.Types.ObjectId,
    ref: 'Reserva'
  }],
  barberosFavoritos: [{
    tipo: mongoose.Schema.Types.ObjectId,
    ref: 'Barbero'
  }],
  barberiasFavoritas: [{
    tipo: mongoose.Schema.Types.ObjectId,
    ref: 'Barberia'
  }]
}, {
  timestamps: true
});

// Índice para búsquedas
clienteSchema.index({ usuario: 1 });

export default mongoose.model('Cliente', clienteSchema);
