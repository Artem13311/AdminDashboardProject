import ClientsTable from './ClientsTable';
import RateBlock from './RateBlock';
import PaymentsHistory from './PaymentsHistory';
import { useNavigate } from 'react-router-dom';

const DashboardPage = () => {
  const navigate = useNavigate();
  const handleLogout = () => {
    localStorage.removeItem('token');
    localStorage.removeItem('refreshToken');
    navigate('/login');
  };
  return (
    <div style={{ padding: 32, maxWidth: 1000, margin: '0 auto' }}>
      <div className="card" style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 24, padding: '18px 32px' }}>
        <h2 style={{ margin: 0 }}>Панель администратора</h2>
        <button onClick={handleLogout} style={{ background: '#fff', color: '#d32f2f', border: '1px solid #d32f2f', fontWeight: 600 }}>Выйти</button>
      </div>
      <div style={{ display: 'flex', gap: 24, flexWrap: 'wrap', alignItems: 'flex-start' }}>
        <RateBlock />
      </div>
      <ClientsTable />
      <PaymentsHistory />
    </div>
  );
};

export default DashboardPage; 