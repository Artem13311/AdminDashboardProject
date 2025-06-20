import React, { useEffect, useState } from 'react';
import api from '../api';
import ClientForm from './ClientForm';

type Client = {
  id: number;
  name: string;
  email: string;
  balanceT: number;
  labels: string[];
};

const ClientsTable: React.FC = () => {
  const [clients, setClients] = useState<Client[]>([]);
  const [loading, setLoading] = useState(true);
  const [editing, setEditing] = useState<Client | null>(null);
  const [adding, setAdding] = useState(false);

  const fetchClients = async () => {
    setLoading(true);
    const res = await api.get('/clients');
    setClients(res.data);
    setLoading(false);
  };

  useEffect(() => { fetchClients(); }, []);

  const handleAdd = async (data: Omit<Client, 'id'>) => {
    await api.post('/clients', data);
    setAdding(false);
    fetchClients();
  };
  const handleEdit = async (data: Omit<Client, 'id'>) => {
    if (!editing) return;
    await api.put(`/clients/${editing.id}`, data);
    setEditing(null);
    fetchClients();
  };
  const handleDelete = async (id: number) => {
    if (window.confirm('Удалить клиента?')) {
      await api.delete(`/clients/${id}`);
      fetchClients();
    }
  };

  if (loading) return <div className="block">Загрузка клиентов...</div>;

  return (
    <div className="card" style={{ marginTop: 24 }}>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 16 }}>
        <h3 style={{ margin: 0 }}>Клиенты</h3>
        <button onClick={() => setAdding(true)}>Добавить клиента</button>
      </div>
      {adding && (
        <div style={{ marginBottom: 16 }}>
          <ClientForm onSubmit={handleAdd} onCancel={() => setAdding(false)} />
        </div>
      )}
      {editing && (
        <div style={{ marginBottom: 16 }}>
          <ClientForm
            initial={editing}
            onSubmit={handleEdit}
            onCancel={() => setEditing(null)}
          />
        </div>
      )}
      <div style={{ overflowX: 'auto' }}>
        <table>
          <thead>
            <tr>
              <th>Имя</th>
              <th>Email</th>
              <th>Баланс T</th>
              <th>Метки</th>
              <th></th>
            </tr>
          </thead>
          <tbody>
            {clients.length === 0 ? (
              <tr><td colSpan={5} style={{ textAlign: 'center', color: '#888' }}>Нет клиентов</td></tr>
            ) : clients.map(c => (
              <tr key={c.id}>
                <td>{c.name}</td>
                <td>{c.email}</td>
                <td>{c.balanceT}</td>
                <td>{c.labels.map(l => <span key={l} style={{ background: '#e0e7ef', color: '#535bf2', borderRadius: 6, padding: '2px 8px', marginRight: 4, fontSize: 13 }}>{l}</span>)}</td>
                <td>
                  <button style={{ padding: '0.3em 0.8em', fontSize: 14 }} onClick={() => setEditing(c)}>✏️</button>
                  <button style={{ marginLeft: 8, padding: '0.3em 0.8em', fontSize: 14, background: '#fff', color: '#d32f2f', border: '1px solid #d32f2f' }} onClick={() => handleDelete(c.id)}>🗑️</button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
};

export default ClientsTable; 