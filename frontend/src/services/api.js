import axios from 'axios';

const API_BASE_URL = process.env.REACT_APP_BACKEND_URL || 'http://localhost:8001';

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Tables API
export const tablesAPI = {
  // Get all tables
  getTables: () => api.get('/api/tables'),
  
  // Get table by ID
  getTable: (tableId) => api.get(`/api/tables/${tableId}`),
  
  // Open table
  openTable: (tableId, covers) => api.post('/api/tables/open', { table_id: tableId, covers }),
  
  // Close table
  closeTable: (tableId) => api.post(`/api/tables/${tableId}/close`),
};

// Products API
export const productsAPI = {
  // Get all products or by category
  getProducts: (category) => api.get('/api/products', { params: category ? { category } : {} }),
  
  // Get categories
  getCategories: () => api.get('/api/products/categories'),
  
  // Get dough types
  getDoughTypes: () => api.get('/api/dough-types'),
  
  // Get extras
  getExtras: () => api.get('/api/extras'),
};

// Orders API
export const ordersAPI = {
  // Add item to order
  addItem: (tableId, productId, doughType, extraIds) => 
    api.post('/api/orders/add-item', {
      table_id: tableId,
      product_id: productId,
      dough_type: doughType,
      extra_ids: extraIds || []
    }),
  
  // Remove item from order
  removeItem: (itemId) => api.delete(`/api/orders/items/${itemId}`),
  
  // Get order for table
  getOrderForTable: (tableId) => api.get(`/api/orders/table/${tableId}`),
  
  // Send order
  sendOrder: (orderId) => api.post(`/api/orders/${orderId}/send`),
  
  // Get receipt
  getReceipt: (orderId) => api.get(`/api/orders/${orderId}/receipt`),
};

export default api;