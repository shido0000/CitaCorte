import express from 'express';
import cors from 'cors';
import dotenv from 'dotenv';
import dbConfig from './Data/Config/database.js';
import { errorHandler, notFoundHandler } from './Middlewares/errorMiddleware.js';

// Importar rutas (se crearán en los controllers)
// import authRoutes from './Controllers/auth.routes.js';
// import adminRoutes from './Controllers/admin.routes.js';
// ... etc

// Cargar variables de entorno
dotenv.config();

const app = express();
const PORT = process.env.PORT || 5000;

// Middlewares globales
app.use(cors({
  origin: process.env.FRONTEND_URL || 'http://localhost:3000',
  credentials: true
}));
app.use(express.json());
app.use(express.urlencoded({ extended: true }));

// Middleware de logging para desarrollo
if (process.env.NODE_ENV === 'development') {
  app.use((req, res, next) => {
    console.log(`${new Date().toISOString()} - ${req.method} ${req.path}`);
    next();
  });
}

// Ruta de health check
app.get('/api/health', (req, res) => {
  res.json({
    success: true,
    message: 'API CitaCorte funcionando correctamente',
    timestamp: new Date().toISOString()
  });
});

// Rutas de la API
// app.use('/api/auth', authRoutes);
// app.use('/api/admin', adminRoutes);
// ... etc

// Manejador de rutas no encontradas
app.use(notFoundHandler);

// Manejador global de errores
app.use(errorHandler);

// Iniciar servidor y conexión a BD
const startServer = async () => {
  try {
    // Conectar a MongoDB
    await dbConfig.connect();
    
    // Iniciar servidor
    app.listen(PORT, () => {
      console.log(`
╔════════════════════════════════════════════════╗
║          🚀 SERVIDOR CITACORTE INICIADO        ║
╠════════════════════════════════════════════════╣
║  Puerto: ${PORT}                                
║  Entorno: ${process.env.NODE_ENV || 'development'}                          
║  URL: http://localhost:${PORT}                  
║  API Health: http://localhost:${PORT}/api/health
╚════════════════════════════════════════════════╝
      `);
    });
  } catch (error) {
    console.error('Error al iniciar el servidor:', error);
    process.exit(1);
  }
};

// Manejo de cierre graceful
process.on('SIGTERM', async () => {
  console.log('\n🛑 Señal SIGTERM recibida, cerrando servidor...');
  await dbConfig.disconnect();
  process.exit(0);
});

export default app;
startServer();
