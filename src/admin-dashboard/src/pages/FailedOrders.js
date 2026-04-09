import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { getOrders } from '../services/orderService';
import { StatusBadge } from './Dashboard';

export default function FailedOrders() {
  const [orders, setOrders] = useState([]);
  const [loading, setLoading] = useState(true);
  const navigate = useNavigate();

  useEffect(() => {
    getOrders()
      .then(res => {
        const failed = res.data.filter(o =>
          ['Failed', 'PaymentFailed', 'InventoryFailed'].includes(o.status)
        );
        setOrders(failed);
      })
      .finally(() => setLoading(false));
  }, []);

  if (loading) return <p>Loading...</p>;

  return (
    <div>
      <h2>Failed Orders ({orders.length})</h2>
      {orders.length === 0 ? (
        <p style={{ color: 'green' }}>No failed orders.</p>
      ) : (
        <table style={{ width: '100%', borderCollapse: 'collapse' }}>
          <thead>
            <tr style={{ background: '#ffebee' }}>
              <th style={{ padding: '8px', textAlign: 'left' }}>Order ID</th>
              <th style={{ padding: '8px' }}>Date</th>
              <th style={{ padding: '8px' }}>Total</th>
              <th style={{ padding: '8px' }}>Status</th>
              <th style={{ padding: '8px' }}>Reason</th>
              <th style={{ padding: '8px' }}>Actions</th>
            </tr>
          </thead>
          <tbody>
            {orders.map(order => (
              <tr key={order.id} style={{ borderBottom: '1px solid #eee' }}>
                <td style={{ padding: '8px', fontSize: '12px' }}>{order.id.substring(0, 8)}...</td>
                <td style={{ padding: '8px', textAlign: 'center' }}>
                  {new Date(order.createdAt).toLocaleDateString()}
                </td>
                <td style={{ padding: '8px', textAlign: 'center' }}>EUR {order.totalAmount}</td>
                <td style={{ padding: '8px', textAlign: 'center' }}>
                  <StatusBadge status={order.status} />
                </td>
                <td style={{ padding: '8px', color: 'red', fontSize: '12px' }}>
                  {order.failureReason ?? '-'}
                </td>
                <td style={{ padding: '8px', textAlign: 'center' }}>
                  <button onClick={() => navigate('/orders/' + order.id)}
                    style={{ padding: '4px 8px', cursor: 'pointer' }}>
                    View
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  );
}
