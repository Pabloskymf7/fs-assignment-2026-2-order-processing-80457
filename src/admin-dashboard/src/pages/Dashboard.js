import React, { useEffect, useState } from 'react';
import { getDashboardSummary, getOrders } from '../services/orderService';
import { useNavigate } from 'react-router-dom';

export function StatusBadge({ status }) {
  const colors = {
    Completed: '#4caf50',
    PaymentFailed: '#f44336',
    InventoryFailed: '#f44336',
    Failed: '#f44336',
    ShippingCreated: '#2196f3',
    PaymentApproved: '#8bc34a',
  };
  const color = colors[status] || '#ff9800';
  return (
    <span style={{
      background: color, color: 'white',
      padding: '4px 8px', borderRadius: '4px', fontSize: '12px'
    }}>
      {status}
    </span>
  );
}

export default function Dashboard() {
  const [summary, setSummary] = useState(null);
  const [recentOrders, setRecentOrders] = useState([]);
  const [loading, setLoading] = useState(true);
  const navigate = useNavigate();

  useEffect(() => {
    Promise.all([getDashboardSummary(), getOrders()])
      .then(([summaryRes, ordersRes]) => {
        setSummary(summaryRes.data);
        setRecentOrders(ordersRes.data.slice(0, 5));
      })
      .catch(err => console.error(err))
      .finally(() => setLoading(false));
  }, []);

  if (loading) return <p>Loading dashboard...</p>;

  return (
    <div>
      <h2>Admin Dashboard</h2>
      <div style={{ display: 'flex', gap: '20px', marginBottom: '30px', flexWrap: 'wrap' }}>
        <StatCard title="Total Orders" value={summary?.totalOrders ?? 0} color="#2196f3" />
        <StatCard title="Completed" value={summary?.completedOrders ?? 0} color="#4caf50" />
        <StatCard title="Failed" value={summary?.failedOrders ?? 0} color="#f44336" />
        <StatCard title="Pending" value={summary?.pendingOrders ?? 0} color="#ff9800" />
        <StatCard title="Revenue" value={'EUR ' + (summary?.totalRevenue ?? 0).toFixed(2)} color="#9c27b0" />
      </div>

      <h3>Recent Orders</h3>
      <table style={{ width: '100%', borderCollapse: 'collapse' }}>
        <thead>
          <tr style={{ background: '#f0f0f0' }}>
            <th style={{ padding: '8px', textAlign: 'left' }}>Order ID</th>
            <th style={{ padding: '8px' }}>Date</th>
            <th style={{ padding: '8px' }}>Total</th>
            <th style={{ padding: '8px' }}>Status</th>
            <th style={{ padding: '8px' }}>Actions</th>
          </tr>
        </thead>
        <tbody>
          {recentOrders.map(order => (
            <tr key={order.id} style={{ borderBottom: '1px solid #eee' }}>
              <td style={{ padding: '8px', fontSize: '12px' }}>{order.id.substring(0, 8)}...</td>
              <td style={{ padding: '8px', textAlign: 'center' }}>{new Date(order.createdAt).toLocaleDateString()}</td>
              <td style={{ padding: '8px', textAlign: 'center' }}>EUR {order.totalAmount}</td>
              <td style={{ padding: '8px', textAlign: 'center' }}><StatusBadge status={order.status} /></td>
              <td style={{ padding: '8px', textAlign: 'center' }}>
                <button onClick={() => navigate('/orders/' + order.id)}
                  style={{ padding: '4px 8px', cursor: 'pointer' }}>View</button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}

function StatCard({ title, value, color }) {
  return (
    <div style={{
      background: color, color: 'white', padding: '20px',
      borderRadius: '8px', minWidth: '150px', textAlign: 'center'
    }}>
      <h3 style={{ margin: 0 }}>{value}</h3>
      <p style={{ margin: '5px 0 0' }}>{title}</p>
    </div>
  );
}
