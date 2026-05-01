import jwt from 'jsonwebtoken';

const configJwt = {
  // Generar token
  generarToken: (payload) => {
    const secret = process.env.JWT_SECRET || 'default_secret_change_in_production';
    const expire = process.env.JWT_EXPIRE || '7d';
    
    return jwt.sign(payload, secret, { expiresIn: expire });
  },

  // Verificar token
  verificarToken: (token) => {
    const secret = process.env.JWT_SECRET || 'default_secret_change_in_production';
    
    try {
      return jwt.verify(token, secret);
    } catch (error) {
      if (error.name === 'TokenExpiredError') {
        throw new Error('Token expirado');
      } else if (error.name === 'JsonWebTokenError') {
        throw new Error('Token inválido');
      }
      throw error;
    }
  },

  // Decodificar token sin verificar (solo para obtener payload)
  decodificarToken: (token) => {
    return jwt.decode(token);
  }
};

export default configJwt;
