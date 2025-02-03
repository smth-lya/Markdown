import React from 'react';
import { useAuth } from '../auth/AuthContext';

const Dashboard = () => {
  const { logout } = useAuth();

  return (
    <div className="flex flex-col items-center justify-center h-screen">
      <h1 className="text-3xl font-bold mb-4">Welcome to the Dashboard!</h1>
      <button onClick={logout} className="p-2 bg-red-500 text-white rounded">Logout</button>
    </div>
  );
};

export default Dashboard;