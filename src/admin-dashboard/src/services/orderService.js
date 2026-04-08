import axios from 'axios';

const API = axios.create({
  baseURL: process.env.REACT_APP_API_URL || 'http://localhost:5250'
});

export const getOrders = () => API.get('/api/Orders');
export const getOrderById = (id) => API.get('/api/Orders/' + id);
export const getDashboardSummary = () => API.get('/api/Orders/dashboard/summary');
export const getOrdersByStatus = (status) => API.get('/api/Orders/filter/' + status);
export const cancelOrder = (id) => API.delete('/api/Orders/' + id);
