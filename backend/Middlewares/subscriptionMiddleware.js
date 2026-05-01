import Barbero from '../Data/Models/Barbero.js';
import Barberia from '../Data/Models/Barberia.js';

// Middleware para verificar estado de suscripción activa
export const checkSubscriptionActive = async (req, res, next) => {
  try {
    const usuarioId = req.usuarioId;
    const rol = req.usuario.rol;

    if (rol === 'barbero') {
      const barbero = await Barbero.findOne({ usuario: usuarioId })
        .populate('planSuscripcionActual.plan');
      
      if (!barbero) {
        return res.status(404).json({
          success: false,
          message: 'Perfil de barbero no encontrado'
        });
      }

      // Verificar si tiene plan activo
      if (barbero.planSuscripcionActual?.estado !== 'activo') {
        return res.status(403).json({
          success: false,
          message: 'No tienes una suscripción activa',
          codigo: 'SUSCRIPCION_INACTIVA'
        });
      }

      // Verificar si está vencido
      if (barbero.planSuscripcionActual?.fechaVencimiento < new Date()) {
        return res.status(403).json({
          success: false,
          message: 'Tu suscripción ha vencido',
          codigo: 'SUSCRIPCION_VENCIDA'
        });
      }

      req.barbero = barbero;
    } else if (rol === 'barberia') {
      const barberia = await Barberia.findOne({ usuario: usuarioId })
        .populate('planSuscripcionActual.plan');
      
      if (!barberia) {
        return res.status(404).json({
          success: false,
          message: 'Perfil de barbería no encontrado'
        });
      }

      // Verificar si tiene plan activo
      if (barberia.planSuscripcionActual?.estado !== 'activo') {
        return res.status(403).json({
          success: false,
          message: 'No tienes una suscripción activa',
          codigo: 'SUSCRIPCION_INACTIVA'
        });
      }

      // Verificar si está vencido
      if (barberia.planSuscripcionActual?.fechaVencimiento < new Date()) {
        return res.status(403).json({
          success: false,
          message: 'Tu suscripción ha vencido',
          codigo: 'SUSCRIPCION_VENCIDA'
        });
      }

      req.barberia = barberia;
    }

    next();
  } catch (error) {
    console.error('Error en checkSubscriptionActive:', error);
    return res.status(500).json({
      success: false,
      message: 'Error al verificar suscripción'
    });
  }
};

// Middleware para verificar nivel de suscripción requerido
export const checkSubscriptionLevel = (nivelRequerido) => {
  return async (req, res, next) => {
    try {
      const rol = req.usuario.rol;
      let entidad;

      if (rol === 'barbero') {
        entidad = req.barbero || await Barbero.findOne({ usuario: req.usuarioId })
          .populate('planSuscripcionActual.plan');
      } else if (rol === 'barberia') {
        entidad = req.barberia || await Barberia.findOne({ usuario: req.usuarioId })
          .populate('planSuscripcionActual.plan');
      }

      if (!entidad || !entidad.planSuscripcionActual?.plan) {
        return res.status(403).json({
          success: false,
          message: 'No tienes un plan de suscripción'
        });
      }

      const nivelesJerarquia = {
        free: 0,
        popular: 1,
        premium: 2,
        basico: 1,
        estandar: 2
      };

      const nivelActual = entidad.planSuscripcionActual.plan.nivel;
      const nivelActualValor = nivelesJerarquia[nivelActual] || 0;
      const nivelRequeridoValor = nivelesJerarquia[nivelRequerido] || 0;

      if (nivelActualValor < nivelRequeridoValor) {
        return res.status(403).json({
          success: false,
          message: `Esta función requiere un plan ${nivelRequerido} o superior`,
          codigo: 'NIVEL_SUFICIENTE',
          planActual: nivelActual
        });
      }

      next();
    } catch (error) {
      console.error('Error en checkSubscriptionLevel:', error);
      return res.status(500).json({
        success: false,
        message: 'Error al verificar nivel de suscripción'
      });
    }
  };
};

// Middleware para verificar estado de afiliación
export const checkAffiliationStatus = async (req, res, next) => {
  try {
    const rol = req.usuario.rol;

    if (rol === 'barbero') {
      const barbero = req.barbero || await Barbero.findOne({ usuario: req.usuarioId });
      
      if (!barbero) {
        return res.status(404).json({
          success: false,
          message: 'Perfil de barbero no encontrado'
        });
      }

      req.afiliacionEstado = barbero.afiliacionBarberia?.estado || 'independiente';
    }

    next();
  } catch (error) {
    console.error('Error en checkAffiliationStatus:', error);
    return res.status(500).json({
      success: false,
      message: 'Error al verificar estado de afiliación'
    });
  }
};

export default {
  checkSubscriptionActive,
  checkSubscriptionLevel,
  checkAffiliationStatus
};
