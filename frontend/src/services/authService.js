import api from './api'

export const authService = {
  // Registro de usuario
  async register(userData) {
    const response = await api.post('/auth/register', userData)
    return response.data
  },

  // Login
  async login(credentials) {
    const response = await api.post('/auth/login', credentials)
    if (response.data.success && response.data.token) {
      localStorage.setItem('token', response.data.token)
      localStorage.setItem('usuario', JSON.stringify(response.data.usuario))
    }
    return response.data
  },

  // Logout
  logout() {
    localStorage.removeItem('token')
    localStorage.removeItem('usuario')
  },

  // Obtener perfil actual
  async getMe() {
    const response = await api.get('/auth/me')
    return response.data
  },

  // Actualizar perfil
  async updateProfile(profileData) {
    const response = await api.put('/auth/update-profile', profileData)
    return response.data
  },

  // Verificar si está autenticado
  isAuthenticated() {
    return !!localStorage.getItem('token')
  },

  // Obtener usuario actual
  getCurrentUser() {
    const usuario = localStorage.getItem('usuario')
    return usuario ? JSON.parse(usuario) : null
  },

  // Obtener rol del usuario actual
  getCurrentRole() {
    const usuario = this.getCurrentUser()
    return usuario?.rol || null
  }
}

export default authService
