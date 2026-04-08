import React from 'react';
import { BrowserRouter, Routes, Route, Link } from 'react-router-dom';
import Dashboard from './pages/Dashboard';
import OrdersTable from './pages/OrdersTable';
import OrderDetail from './pages/OrderDetail';
import FailedOrders from './pages/FailedOrders';

const navStyle = {
  width: '200px',
  minHeight: '100vh',
  background: '#1e1e2e',
  padding: '20px',
  boxSizing: 'border-box'
};

const linkStyle = {
  display: 'block',
  color: 'white',
  textDecoration: 'none',
  marginBottom: '12px',
  padding: '8px',
  borderRadius: '4px'
};

export default function App() {
  return (
    <BrowserRouter>
      <div style={{ display: 'flex' }}>
        <nav style={navStyle}>
          <h3 style={{ color: 'white', marginBottom: '20px' }}>Admin</h3>
          <Link to="/" style={linkStyle}>Dashboard</Link>
          <Link to="/orders" style={linkStyle}>All Orders</Link>
          <Link to="/failed" style={linkStyle}>Failed Orders</Link>
        </nav>
        <main style={{ flex: 1, padding: '30px' }}>
          <Routes>
            <Route path="/" element={<Dashboard />} />
            <Route path="/orders" element={<OrdersTable />} />
            <Route path="/orders/:id" element={<OrderDetail />} />
            <Route path="/failed" element={<FailedOrders />} />
          </Routes>
        </main>
      </div>
    </BrowserRouter>
  );
}
