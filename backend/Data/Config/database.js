import mongoose from 'mongoose';

const dbConfig = {
  connect: async () => {
    try {
      const mongoURI = process.env.MONGODB_URI || 'mongodb://localhost:27017/citacorte';
      
      const conn = await mongoose.connect(mongoURI, {
        useNewUrlParser: true,
        useUnifiedTopology: true,
      });

      console.log(`✅ MongoDB conectado: ${conn.connection.host}`);
      
      // Manejo de eventos de conexión
      mongoose.connection.on('error', (err) => {
        console.error(`❌ Error de MongoDB: ${err.message}`);
      });

      mongoose.connection.on('disconnected', () => {
        console.warn('⚠️  MongoDB desconectado');
      });

      // Cierre graceful
      process.on('SIGINT', async () => {
        await mongoose.connection.close();
        console.log('🔒 Conexión a MongoDB cerrada por terminación de la aplicación');
        process.exit(0);
      });

      return conn;
    } catch (error) {
      console.error(`❌ Error al conectar a MongoDB: ${error.message}`);
      process.exit(1);
    }
  },

  disconnect: async () => {
    try {
      await mongoose.connection.close();
      console.log('🔒 MongoDB desconectado');
    } catch (error) {
      console.error(`Error al desconectar MongoDB: ${error.message}`);
    }
  }
};

export default dbConfig;
