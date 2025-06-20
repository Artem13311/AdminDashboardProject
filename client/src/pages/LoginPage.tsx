import React, { useState } from 'react';
import axios from 'axios';
import { useNavigate } from 'react-router-dom';

const LoginPage: React.FC = () => {
  const [email, setEmail] = useState('admin@mirra.dev');
  const [password, setPassword] = useState('admin123');
  const [error, setError] = useState('');
  const navigate = useNavigate();

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    try {
      const res = await axios.post('http://localhost:5000/auth/login', { email, password });
      localStorage.setItem('token', res.data.token);
      localStorage.setItem('refreshToken', res.data.refreshToken || '');
      navigate('/dashboard');
    } catch {
      setError('Неверный логин или пароль');
    }
  };

  return (
    <div style={{ display: 'flex', flexDirection: 'column', alignItems: 'center', justifyContent: 'center', height: '100vh', background: 'linear-gradient(120deg, #f6f8fa 0%, #e9eefa 100%)' }}>
      <form onSubmit={handleSubmit} className="card" style={{ display: 'flex', flexDirection: 'column', gap: 18, minWidth: 320, maxWidth: 380, alignItems: 'center' }}>
        <h2 style={{ marginBottom: 8 }}>Вход в админку</h2>
        <input type="email" placeholder="Email" value={email} onChange={e => setEmail(e.target.value)} required style={{ width: '100%' }} />
        <input type="password" placeholder="Пароль" value={password} onChange={e => setPassword(e.target.value)} required style={{ width: '100%' }} />
        <button type="submit" style={{ width: '100%', fontSize: 18, marginTop: 8 }}>Войти</button>
        {error && <div style={{ color: '#d32f2f', marginTop: 4, fontWeight: 500 }}>{error}</div>}
      </form>
    </div>
  );
};

export default LoginPage; 