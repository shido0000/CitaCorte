import mongoose from 'mongoose';

const barberiaSchema = new mongoose.Schema({
  usuario: {
    type: mongoose.Schema.Types.ObjectId,
    ref: 'Usuario',
    required: true
  },
  nombreComercial: {
    type: String,
    required: [true, 'El nombre comercial es requerido'],
    trim: true,
    maxlength: [100, 'El nombre comercial no puede exceder 100 caracteres']
  },
  direccion: {
    calle: String,
    numero: String,
    colonia: String,
    ciudad: String,
    estado: String,
    codigoPostal: String,
    pais: {
      type: String,
      default: 'México'
    },
    coordenadas: {
      latitud: Number,
      longitud: Number
    }
  },
  descripcion: {
    type: String,
    trim: true,
    maxlength: [1000, 'La descripción no puede exceder 1000 caracteres']
  },
  fotos: [{
    url: String,
    descripcion: String
  }],
  telefono: {
    type: String,
    trim: true,
    maxlength: [20, 'El teléfono no puede exceder 20 caracteres']
  },
  horarios: [{
    diaSemana: {
      type: String,
      enum: ['lunes', 'martes', 'miercoles', 'jueves', 'viernes', 'sabado', 'domingo']
    },
    apertura: String, // Formato HH:mm
    cierre: String,   // Formato HH:mm
    activo: {
      type: Boolean,
      default: true
    }
  }],
  planSuscripcionActual: {
    plan: {
      type: mongoose.Schema.Types.ObjectId,
      ref: 'PlanSuscripcion'
    },
    fechaInicio: Date,
    fechaVencimiento: Date,
    estado: {
      type: String,
      enum: ['activo', 'vencido', 'pendiente_aprobacion'],
      default: 'pendiente_aprobacion' // Requiere aprobación
    }
  },
  barberosAfiliados: [{
    barbero: {
      type: mongoose.Schema.Types.ObjectId,
      ref: 'Barbero'
    },
    fechaAfiliacion: Date,
    activo: {
      type: Boolean,
      default: true
    }
  }],
  serviciosOfrecidos: [{
    tipo: mongoose.Schema.Types.ObjectId,
    ref: 'Servicio'
  }],
  calificacionPromedio: {
    type: Number,
    default: 0,
    min: [0, 'La calificación no puede ser menor a 0'],
    max: [5, 'La calificación no puede ser mayor a 5']
  },
  redesSociales: {
    facebook: String,
    instagram: String,
    twitter: String,
    whatsapp: String
  }
}, {
  timestamps: true
});

// Índice para búsquedas
barberiaSchema.index({ usuario: 1 });
barberiaSchema.index({ 'direccion.ciudad': 1 });
barberiaSchema.index({ nombreComercial: 'text' });
barberiaSchema.index({ descripcion: 'text' });

// Método para verificar si puede afiliar más barberos
barberiaSchema.methods.puedeAfiliarBarbero = function() {
  if (this.planSuscripcionActual?.estado !== 'activo') return false;
  
  const limite = this.planSuscripcionActual?.plan?.limiteBarberos || 0;
  const actuales = this.barberosAfiliados.filter(b => b.activo).length;
  
  return actuales < limite;
};

// Método para obtener número de barberos afiliados activos
barberiaSchema.virtual('numeroBarberosActivos').get(function() {
  return this.barberosAfiliados.filter(b => b.activo).length;
});

export default mongoose.model('Barberia', barberiaSchema);
