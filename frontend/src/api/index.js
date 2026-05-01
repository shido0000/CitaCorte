import apiClient from './client'

export const authApi = {
  login: (email, password) => 
    apiClient.post('/auth/login', { email, password }),
  
  register: (name, email, password, role, phone) => 
    apiClient.post('/auth/register', { name, email, password, role, phone }),
  
  getCurrentUser: () => 
    apiClient.get('/auth/me')
}

export const barberoApi = {
  getProfile: () => 
    apiClient.get('/barbero/profile'),
  
  updateProfile: (data) => 
    apiClient.put('/barbero/profile', data),
  
  requestAffiliation: (barberiaId, message) => 
    apiClient.post(`/barbero/affiliation/request/${barberiaId}`, { message }),
  
  cancelAffiliationRequest: () => 
    apiClient.post('/barbero/affiliation/cancel'),
  
  getServices: () => 
    apiClient.get('/barbero/services'),
  
  addService: (service) => 
    apiClient.post('/barbero/services', service),
  
  updateService: (serviceId, service) => 
    apiClient.put(`/barbero/services/${serviceId}`, service),
  
  deleteService: (serviceId) => 
    apiClient.delete(`/barbero/services/${serviceId}`),
  
  getProducts: () => 
    apiClient.get('/barbero/products'),
  
  addProduct: (product) => 
    apiClient.post('/barbero/products', product),
  
  getStatistics: (startDate, endDate) => 
    apiClient.get('/barbero/statistics', { params: { startDate, endDate } }),
  
  requestSubscriptionChange: (subscriptionType) => 
    apiClient.post(`/barbero/subscription/change/${subscriptionType}`)
}

export const barberiaApi = {
  getProfile: () => 
    apiClient.get('/barberia/profile'),
  
  createBarberia: (data) => 
    apiClient.post('/barberia/create', data),
  
  updateProfile: (data) => 
    apiClient.put('/barberia/profile', data),
  
  getPendingAffiliationRequests: () => 
    apiClient.get('/barberia/affiliation/requests/pending'),
  
  respondToAffiliationRequest: (requestId, accepted, rejectionReason) => 
    apiClient.post(`/barberia/affiliation/respond/${requestId}`, { accepted, rejectionReason }),
  
  getAffiliatedBarberos: () => 
    apiClient.get('/barberia/affiliated/barberos'),
  
  removeBarbero: (barberoId) => 
    apiClient.delete(`/barberia/barberos/${barberoId}`),
  
  getServices: () => 
    apiClient.get('/barberia/services'),
  
  addService: (service) => 
    apiClient.post('/barberia/services', service),
  
  updateService: (serviceId, service) => 
    apiClient.put(`/barberia/services/${serviceId}`, service),
  
  deleteService: (serviceId) => 
    apiClient.delete(`/barberia/services/${serviceId}`),
  
  getStatistics: (startDate, endDate) => 
    apiClient.get('/barberia/statistics', { params: { startDate, endDate } }),
  
  requestSubscription: (subscriptionType) => 
    apiClient.post(`/barberia/subscription/request/${subscriptionType}`)
}

export const adminApi = {
  getAllUsers: () => 
    apiClient.get('/admin/users'),
  
  getAllBarberos: () => 
    apiClient.get('/admin/barberos'),
  
  getAllBarberias: () => 
    apiClient.get('/admin/barberias'),
  
  deactivateUser: (userId) => 
    apiClient.post(`/admin/users/${userId}/deactivate`),
  
  activateUser: (userId) => 
    apiClient.post(`/admin/users/${userId}/activate`),
  
  getSubscriptionPlans: () => 
    apiClient.get('/admin/subscription-plans'),
  
  createSubscriptionPlan: (plan) => 
    apiClient.post('/admin/subscription-plans', plan),
  
  updateSubscriptionPlan: (planId, plan) => 
    apiClient.put(`/admin/subscription-plans/${planId}`, plan),
  
  deleteSubscriptionPlan: (planId) => 
    apiClient.delete(`/admin/subscription-plans/${planId}`)
}

export const comercialApi = {
  getBarberosPending: () => 
    apiClient.get('/comercial/barberos/pending'),
  
  getBarberiasPending: () => 
    apiClient.get('/comercial/barberias/pending'),
  
  approveBarberoSubscription: (barberoId, approved) => 
    apiClient.post(`/comercial/barberos/${barberoId}/approve?approved=${approved}`),
  
  approveBarberiaSubscription: (barberiaId, approved) => 
    apiClient.post(`/comercial/barberias/${barberiaId}/approve?approved=${approved}`),
  
  getGeneralStatistics: (startDate, endDate) => 
    apiClient.get('/comercial/statistics', { params: { startDate, endDate } })
}

export const reservationApi = {
  createReservation: (data) => 
    apiClient.post('/reservation', data),
  
  confirmReservation: (reservationId) => 
    apiClient.post(`/reservation/${reservationId}/confirm`),
  
  cancelReservation: (reservationId, reason) => 
    apiClient.post(`/reservation/${reservationId}/cancel`, { reason }),
  
  getReservation: (reservationId) => 
    apiClient.get(`/reservation/${reservationId}`),
  
  getMyReservations: () => 
    apiClient.get('/reservation/my-reservations'),
  
  checkAvailability: (barberoId, barberiaId, startDateTime, durationMinutes) => 
    apiClient.get('/reservation/check-availability', { 
      params: { barberoId, barberiaId, startDateTime, durationMinutes } 
    })
}

export const notificationApi = {
  getNotifications: (unreadOnly = false) => 
    apiClient.get('/notification', { params: { unreadOnly } }),
  
  markAsRead: (notificationId) => 
    apiClient.post(`/notification/${notificationId}/read`),
  
  markAllAsRead: () => 
    apiClient.post('/notification/read-all'),
  
  deleteNotification: (notificationId) => 
    apiClient.delete(`/notification/${notificationId}`)
}

export const clientApi = {
  searchBarberos: (date, specialty) => 
    apiClient.get('/client/barberos', { params: { date, specialty } }),
  
  searchBarberias: (date) => 
    apiClient.get('/client/barberias', { params: { date } }),
  
  getBarberoServices: (barberoId) => 
    apiClient.get(`/client/barberos/${barberoId}/services`),
  
  getBarberiaServices: (barberiaId) => 
    apiClient.get(`/client/barberias/${barberiaId}/services`)
}
