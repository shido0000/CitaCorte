import jwt from 'jsonwebtoken';
import Usuario from '../Data/Models/Usuario.js';

// Middleware para verificar token JWT
export const verifyToken = async (req, res, next) => {
  try {
    // Obtener token del header
    const authHeader = req.headers.authorization;
    
    if (!authHeader || !authHeader.startsWith('Bearer ')) {
      return res.status(401).json({
        success: false,
        message: 'No se proporcionó token de autenticación'
      });
    }

    const token = authHeader.split(' ')[1];
    
    // Verificar token
    const decoded = jwt.verify(token, process.env.JWT_SECRET);
    
    // Buscar usuario
    const usuario = await Usuario.findById(decoded.id).select('-password');
    
    if (!usuario || !usuario.activo) {
      return res.status(401).json({
        success: false,
        message: 'Usuario no encontrado o inactivo'
      });
    }

    // Agregar usuario al request
    req.usuario = usuario;
    req.usuarioId = usuario._id.toString();
    
    next();
  } catch (error) {
    if (error.name === 'JsonWebTokenError') {
      return res.status(401).json({
        success: false,
        message: 'Token inválido'
      });
    }
    if (error.name === 'TokenExpiredError') {
      return res.status(401).json({
        success: false,
        message: 'Token expirado'
      });
    }
    
    console.error('Error en verifyToken:', error);
    return res.status(500).json({
      success: false,
      message: 'Error al verificar autenticación'
    });
  }
};

// Middleware para autenticación opcional
export const optionalAuth = async (req, res, next) => {
  try {
    const authHeader = req.headers.authorization;
    
    if (!authHeader || !authHeader.startsWith('Bearer ')) {
      return next(); // Continuar sin usuario
    }

    const token = authHeader.split(' ')[1];
    const decoded = jwt.verify(token, process.env.JWT_SECRET);
    const usuario = await Usuario.findById(decoded.id).select('-password');
    
    if (usuario && usuario.activo) {
      req.usuario = usuario;
      req.usuarioId = usuario._id.toString();
    }
    
    next();
  } catch (error) {
    // Si hay error con el token, continuar sin usuario
    next();
  }
};

export default { verifyToken, optionalAuth };
