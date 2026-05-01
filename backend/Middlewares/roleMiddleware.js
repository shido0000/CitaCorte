// Middleware para verificar roles de usuario

export const checkRole = (rolesPermitidos) => {
  return (req, res, next) => {
    if (!req.usuario) {
      return res.status(401).json({
        success: false,
        message: 'Usuario no autenticado'
      });
    }

    if (!rolesPermitidos.includes(req.usuario.rol)) {
      return res.status(403).json({
        success: false,
        message: `Acceso denegado. Se requiere uno de los siguientes roles: ${rolesPermitidos.join(', ')}`
      });
    }

    next();
  };
};

// Middlewares predefinidos por rol
export const esAdmin = checkRole(['admin']);
export const esBarbero = checkRole(['barbero']);
export const esBarberia = checkRole(['barberia']);
export const esComercial = checkRole(['comercial']);
export const esCliente = checkRole(['cliente']);

// Middleware para múltiples roles
export const esAdminOComercial = checkRole(['admin', 'comercial']);
export const esBarberoOBarberia = checkRole(['barbero', 'barberia']);

export default {
  checkRole,
  esAdmin,
  esBarbero,
  esBarberia,
  esComercial,
  esCliente,
  esAdminOComercial,
  esBarberoOBarberia
};
