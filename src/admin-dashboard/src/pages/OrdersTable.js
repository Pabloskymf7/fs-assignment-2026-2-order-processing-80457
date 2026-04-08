import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { getOrders, getOrdersByStatus, cancelOrder } from '../services/orderService';
import { StatusBadge } from './Dashboard';

export default function OrdersTable() {
  const [orders, setOrders] = useState([]);
  const [filtered, setFiltered] = useState([]);
  const [statusFilter, setStatusFilter] = useState('All');
  const [loading, setLoading] = useState(true);
  const navigate = useNavigate();

  useEffect(() => {
    getOrders()
      .then(res => {
        setOrders(res.data);
        setFiltered(res.data);
      })
      .finally(() => setLoading(false));
  }, []);

  useEffect(() => {
    if (statusFilter === 'All') {
      setFiltered(orders);
    } else {
      getOrdersByStatus(statusFilter)
        .then(res => setFiltered(res.data))
        .catch(() => setFiltered(orders.filter(o => o.status === statusFilter)));
    }
  }, [statusFilter, orders]);

  const handleCancel = async (id) => {
    if (window.confirm('Cancel this order?')) {
      await cancelOrder(id);
      setOrders(orders.filter(o => o.id !== id));
    }
  };

  const statuses = ['All', 'Submitted', 'Completed', 'PaymentFailed',
    'InventoryFailed', 'ShippingCreated', 'PaymentApproved', 'Failed'];

  if (loading) return <p>Loading orders...</p>;

  return (
    <div>
      <h2>All Orders</h2>
      <div style={{ marginBottom: '16px' }}>
        <label>Filter by status: </label>
        <select value={statusFilter} onChange={e => setStatusFilter(e.target.value)}
          style={{ padding: '6px', marginLeft: '8px' }}>
          {statuses.map(s => <option key={s} value={s}>{s}</option>)}
        </select>
        <span style={{ marginLeft: '16px', color: '#666' }}>
          Showing {filtered.length} of {orders.length} orders
        </span>
      </div>

      <table style={{ width: '100%', borderCollapse: 'collapse' }}>
        <thead>
          <tr style={{ background: '#f0f0f0' }}>
            <th style={{ padding: '8px', textAlign: 'left' }}>Order ID</th>
            <th style={{ padding: '8px' }}>Customer</th>
            <th style={{ padding: '8px' }}>Date</th>
            <th style={{ padding: '8px' }}>Total</th>
            <th style={{ padding: '8px' }}>Status</th>
            <th style={{ padding: '8px' }}>Actions</th>
          </tr>
        </thead>
        <tbody>
          {filtered.map(order => (
            <tr key={order.id} style={{ borderBottom: '1px solid #eee' }}>
              <td style={{ padding: '8px', fontSize: '12px' }}>{order.id.substring(0, 8)}...</td>
              <td style={{ padding: '8px', textAlign: 'center', fontSize: '12px' }}>
                {order.customerId.substring(0, 8)}...
              </td>
              <td style={{ padding: '8px', textAlign: 'center' }}>
                {new Date(order.createdAt).toLocaleDateString()}
              </td>
              <td style={{ padding: '8px', textAlign: 'center' }}>EUR {order.totalAmount}</td>
              <td style={{ padding: '8px', textAlign: 'center' }}>
                <StatusBadge status={order.status} />
              </td>
              <td style={{ padding: '8px', textAlign: 'center' }}>
                <button onClick={() => navigate('/orders/' + order.id)}
                  style={{ padding: '4px 8px', cursor: 'pointer', marginRight: '4px' }}>
                  View
                </button>
                <button onClick={() => handleCancel(order.id)}
                  style={{ padding: '4px 8px', cursor: 'pointer', background: '#f44336', color: 'white', border: 'none' }}>
                  Cancel
                </button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
