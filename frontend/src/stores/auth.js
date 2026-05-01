import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { authApi } from '@/api'

export const useAuthStore = defineStore('auth', () => {
  const user = ref(JSON.parse(localStorage.getItem('user') || 'null'))
  const token = ref(localStorage.getItem('token') || null)
  const loading = ref(false)
  const error = ref(null)

  const isAuthenticated = computed(() => !!token.value)
  const userRole = computed(() => user.value?.role)
  
  const isAdmin = computed(() => userRole.value === 'Admin')
  const isBarbero = computed(() => userRole.value === 'Barbero')
  const isBarberia = computed(() => userRole.value === 'Barberia')
  const isComercial = computed(() => userRole.value === 'Comercial')
  const isCliente = computed(() => userRole.value === 'Cliente')

  async function login(email, password) {
    loading.value = true
    error.value = null
    try {
      const response = await authApi.login(email, password)
      token.value = response.data.token
      user.value = response.data.user
      localStorage.setItem('token', token.value)
      localStorage.setItem('user', JSON.stringify(user.value))
      return response.data
    } catch (err) {
      error.value = err.response?.data?.message || 'Error al iniciar sesión'
      throw err
    } finally {
      loading.value = false
    }
  }

  async function register(name, email, password, role, phone) {
    loading.value = true
    error.value = null
    try {
      const response = await authApi.register(name, email, password, role, phone)
      token.value = response.data.token
      user.value = response.data.user
      localStorage.setItem('token', token.value)
      localStorage.setItem('user', JSON.stringify(user.value))
      return response.data
    } catch (err) {
      error.value = err.response?.data?.message || 'Error al registrarse'
      throw err
    } finally {
      loading.value = false
    }
  }

  async function getCurrentUser() {
    if (!token.value) return null
    try {
      const response = await authApi.getCurrentUser()
      user.value = response.data
      localStorage.setItem('user', JSON.stringify(user.value))
      return response.data
    } catch (err) {
      logout()
      return null
    }
  }

  function logout() {
    token.value = null
    user.value = null
    localStorage.removeItem('token')
    localStorage.removeItem('user')
  }

  function clearError() {
    error.value = null
  }

  return {
    user,
    token,
    loading,
    error,
    isAuthenticated,
    userRole,
    isAdmin,
    isBarbero,
    isBarberia,
    isComercial,
    isCliente,
    login,
    register,
    getCurrentUser,
    logout,
    clearError
  }
})
