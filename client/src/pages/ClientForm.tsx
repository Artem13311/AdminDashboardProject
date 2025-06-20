import React, { useState, useEffect } from 'react';

type Props = {
  initial?: {
    name: string;
    email: string;
    balanceT: number;
    labels: string[];
  };
  onSubmit: (data: { name: string; email: string; balanceT: number; labels: string[] }) => void;
  onCancel: () => void;
};

const ClientForm: React.FC<Props> = ({ initial, onSubmit, onCancel }) => {
  const [name, setName] = useState(initial?.name || '');
  const [email, setEmail] = useState(initial?.email || '');
  const [balanceT, setBalanceT] = useState(initial?.balanceT || 0);
  const [labels, setLabels] = useState<string[]>(initial?.labels || []);
  const [newLabel, setNewLabel] = useState('');

  useEffect(() => {
    setName(initial?.name || '');
    setEmail(initial?.email || '');
    setBalanceT(initial?.balanceT || 0);
    setLabels(initial?.labels || []);
    setNewLabel('');
  }, [initial]);

  const handleAddLabel = () => {
    if (newLabel && !labels.includes(newLabel)) {
      setLabels([...labels, newLabel]);
      setNewLabel('');
    }
  };
  const handleRemoveLabel = (label: string) => {
    setLabels(labels.filter(l => l !== label));
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    onSubmit({ name, email, balanceT, labels });
  };

  return (
    <form onSubmit={handleSubmit} className="card" style={{ display: 'flex', flexDirection: 'column', gap: 14, minWidth: 320, maxWidth: 400 }}>
      <input value={name} onChange={e => setName(e.target.value)} placeholder="Имя" required />
      <input value={email} onChange={e => setEmail(e.target.value)} placeholder="Email" type="email" required />
      <input value={balanceT} onChange={e => setBalanceT(Number(e.target.value))} placeholder="Баланс T" type="number" min={0} required />
      <div>
        <div style={{ marginBottom: 4, fontWeight: 500 }}>Метки:</div>
        <div style={{ display: 'flex', gap: 6, flexWrap: 'wrap', marginBottom: 4 }}>
          {labels.map(label => (
            <span key={label} style={{ background: '#e0e7ef', color: '#535bf2', borderRadius: 8, padding: '2px 10px', fontSize: 13, display: 'inline-flex', alignItems: 'center' }}>
              {label} <button type="button" onClick={() => handleRemoveLabel(label)} style={{ marginLeft: 4, background: 'none', color: '#d32f2f', border: 'none', fontWeight: 700, cursor: 'pointer', fontSize: 15 }}>×</button>
            </span>
          ))}
        </div>
        <div style={{ display: 'flex', gap: 8 }}>
          <input value={newLabel} onChange={e => setNewLabel(e.target.value)} placeholder="Новая метка" style={{ flex: 1 }} />
          <button type="button" onClick={handleAddLabel} style={{ padding: '0.3em 1em', fontSize: 14 }}>Добавить</button>
        </div>
      </div>
      <div style={{ display: 'flex', gap: 10, marginTop: 8, justifyContent: 'flex-end' }}>
        <button type="submit" style={{ padding: '0.4em 1.2em' }}>Сохранить</button>
        <button type="button" onClick={onCancel} style={{ background: '#fff', color: '#535bf2', border: '1px solid #535bf2', padding: '0.4em 1.2em' }}>Отмена</button>
      </div>
    </form>
  );
};

export default ClientForm; 