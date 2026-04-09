import React, { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { getOrderById } from '../services/orderService';
import { StatusBadge } from './Dashboard';

export default function OrderDetail() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [order, setOrder] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    getOrderById(id)
      .then(res => setOrder(res.data))
      .finally(() => setLoading(false));
  }, [id]);

  if (loading) return <p>Loading order...</p>;
  if (!order) return <p>Order not found.</p>;

  return (
    <div style={{ maxWidth: '700px' }}>
      <button onClick={() => navigate('/orders')}
        style={{ marginBottom: '16px', padding: '6px 12px', cursor: 'pointer' }}>
        Back to Orders
      </button>

      <h2>Order Detail</h2>

      <div style={{ background: '#f9f9f9', padding: '16px', borderRadius: '8px', marginBottom: '16px' }}>
        <p><strong>Order ID:</strong> {order.id}</p>
        <p><strong>Customer ID:</strong> {order.customerId}</p>
        <p><strong>Date:</strong> {new Date(order.createdAt).toLocaleString()}</p>
        <p><strong>Total:</strong> EUR {order.totalAmount}</p>
        <p><strong>Status:</strong> <StatusBadge status={order.status} /></p>
        {order.failureReason && (
          <p style={{ color: 'red' }}><strong>Failure Reason:</strong> {order.failureReason}</p>
        )}
      </div>

      {order.payment && (
        <div style={{ background: '#f9f9f9', padding: '16px', borderRadius: '8px', marginBottom: '16px' }}>
          <h4>Payment Info</h4>
          <p><strong>Transaction ID:</strong> {order.payment.transactionId ?? 'N/A'}</p>
          <p><strong>Approved:</strong> {order.payment.approved ? 'Yes' : 'No'}</p>
          {order.payment.reason && <p><strong>Reason:</strong> {order.payment.reason}</p>}
        </div>
      )}

      {order.shipment && (
        <div style={{ background: '#f9f9f9', padding: '16px', borderRadius: '8px', marginBottom: '16px' }}>
          <h4>Shipment Info</h4>
          <p><strong>Tracking Ref:</strong> {order.shipment.trackingReference}</p>
          <p><strong>Estimated Dispatch:</strong> {new Date(order.shipment.estimatedDispatch).toLocaleDateString()}</p>
        </div>
      )}

      <h4>Items</h4>
      <table style={{ width: '100%', borderCollapse: 'collapse' }}>
        <thead>
          <tr style={{ background: '#f0f0f0' }}>
            <th style={{ padding: '8px', textAlign: 'left' }}>Product</th>
            <th style={{ padding: '8px' }}>Qty</th>
            <th style={{ padding: '8px' }}>Unit Price</th>
            <th style={{ padding: '8px' }}>Subtotal</th>
          </tr>
        </thead>
        <tbody>
          {order.items.map((item, i) => (
            <tr key={i} style={{ borderBottom: '1px solid #eee' }}>
              <td style={{ padding: '8px' }}>{item.productName}</td>
              <td style={{ padding: '8px', textAlign: 'center' }}>{item.quantity}</td>
              <td style={{ padding: '8px', textAlign: 'center' }}>EUR {item.unitPrice}</td>
              <td style={{ padding: '8px', textAlign: 'center' }}>EUR {(item.quantity * item.unitPrice).toFixed(2)}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
