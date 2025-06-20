import React, { useEffect, useState } from 'react';
import axios from 'axios';

type Payment = {
  id: number;
  clientId: number;
  clientName: string;
  amount: number;
  date: string;
};

const PaymentsHistory: React.FC = () => {
  const [payments, setPayments] = useState<Payment[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchPayments = async () => {
      const token = localStorage.getItem('token');
      const res = await axios.get('http://localhost:5000/payments?take=5', {
        headers: { Authorization: `Bearer ${token}` }
      });
      setPayments(res.data);
      setLoading(false);
    };
    fetchPayments();
  }, []);

  if (loading) return <div className="block">Загрузка платежей...</div>;

  return (
    <div className="card" style={{ marginTop: 24 }}>
      <h3 style={{ marginBottom: 16 }}>Последние платежи</h3>
      <div style={{ overflowX: 'auto' }}>
        <table>
          <thead>
            <tr>
              <th>Клиент</th>
              <th>Сумма</th>
              <th>Дата</th>
            </tr>
          </thead>
          <tbody>
            {payments.length === 0 ? (
              <tr><td colSpan={3} style={{ textAlign: 'center', color: '#888' }}>Нет платежей</td></tr>
            ) : payments.map(p => (
              <tr key={p.id}>
                <td>{p.clientName}</td>
                <td>{p.amount}</td>
                <td>{new Date(p.date).toLocaleString()}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
};

export default PaymentsHistory; 