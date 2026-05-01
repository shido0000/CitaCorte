// Manejador global de errores

export const errorHandler = (err, req, res, next) => {
  console.error('Error:', err);

  // Error por defecto
  let error = { ...err };
  error.message = err.message;

  // Error de validación de Mongoose
  if (err.name === 'ValidationError') {
    const messages = Object.values(err.errors).map(val => val.message);
    return res.status(400).json({
      success: false,
      message: messages.join(', '),
      codigo: 'VALIDATION_ERROR',
      detalles: err.errors
    });
  }

  // Error de solapamiento de reserva
  if (err.codigoError === 'SOLAPAMIENTO_RESERVA') {
    return res.status(409).json({
      success: false,
      message: err.message,
      codigo: 'SOLAPAMIENTO_RESERVA'
    });
  }

  // Error de duplicado (E11000)
  if (err.code === 11000) {
    const campo = Object.keys(err.keyValue)[0];
    return res.status(400).json({
      success: false,
      message: `El ${campo} ya está en uso`,
      codigo: 'DUPLICADO'
    });
  }

  // Error de Cast (ID inválido)
  if (err.name === 'CastError') {
    return res.status(400).json({
      success: false,
      message: 'ID inválido',
      codigo: 'CAST_ERROR'
    });
  }

  // Error de JWT
  if (err.name === 'JsonWebTokenError') {
    return res.status(401).json({
      success: false,
      message: 'Token inválido',
      codigo: 'JWT_ERROR'
    });
  }

  // Error de token expirado
  if (err.name === 'TokenExpiredError') {
    return res.status(401).json({
      success: false,
      message: 'Token expirado',
      codigo: 'TOKEN_EXPIRED'
    });
  }

  // Status code por defecto: 500
  const statusCode = err.statusCode || 500;

  res.status(statusCode).json({
    success: false,
    message: error.message || 'Error interno del servidor',
    codigo: error.codigoError || 'INTERNAL_ERROR',
    ...(process.env.NODE_ENV === 'development' && { stack: err.stack })
  });
};

// Middleware para rutas no encontradas (404)
export const notFoundHandler = (req, res, next) => {
  res.status(404).json({
    success: false,
    message: `Ruta ${req.originalUrl} no encontrada`,
    codigo: 'NOT_FOUND'
  });
};

export default { errorHandler, notFoundHandler };
