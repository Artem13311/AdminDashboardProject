import React, { useEffect, useState } from 'react';
import axios from 'axios';

type Rate = {
  value: number;
  updatedAt: string;
};

const RateBlock: React.FC = () => {
  const [rate, setRate] = useState<Rate | null>(null);
  const [newRate, setNewRate] = useState('');
  const [loading, setLoading] = useState(true);

  const fetchRate = async () => {
    setLoading(true);
    const token = localStorage.getItem('token');
    const res = await axios.get('http://localhost:5000/rate', {
      headers: { Authorization: `Bearer ${token}` }
    });
    setRate(res.data);
    setLoading(false);
  };

  useEffect(() => { fetchRate(); }, []);

  const handleUpdate = async (e: React.FormEvent) => {
    e.preventDefault();
    const token = localStorage.getItem('token');
    await axios.post('http://localhost:5000/rate', { value: Number(newRate) }, {
      headers: { Authorization: `Bearer ${token}` }
    });
    setNewRate('');
    fetchRate();
  };

  if (loading) return <div className="block">Загрузка курса...</div>;

  return (
    <div className="card" style={{ margin: '24px 0', maxWidth: 340, display: 'inline-block' }}>
      <div style={{ fontSize: 18, marginBottom: 6 }}>Текущий курс: <b>{rate?.value}</b></div>
      <div style={{ fontSize: 12, color: '#888', marginBottom: 10 }}>Обновлено: {rate && new Date(rate.updatedAt).toLocaleString()}</div>
      <form onSubmit={handleUpdate} style={{ marginTop: 8, display: 'flex', gap: 8 }}>
        <input type="number" value={newRate} onChange={e => setNewRate(e.target.value)} placeholder="Новый курс" required min={1} style={{ flex: 1 }} />
        <button type="submit" style={{ padding: '0.3em 1em', fontSize: 15 }}>Изменить</button>
      </form>
    </div>
  );
};

export default RateBlock; 