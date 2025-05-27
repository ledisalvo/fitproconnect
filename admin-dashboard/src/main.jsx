import React from 'react';
import ReactDOM from 'react-dom/client';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import Layout from './components/Layout';
import UserTable from './components/UserTable';
import EventLogTable from './components/EventLogTable';
import MissingUsers from './components/MissingUsers';
import './index.css';
import App from './App';

ReactDOM.createRoot(document.getElementById('root')).render(
  <React.StrictMode>
    <BrowserRouter>
      <Routes>
        <Route path='/' element={<Layout />}>
          <Route index element={<Navigate to='/users' />} />
          <Route path='users' element={<UserTable />} />
          <Route path='events' element={<EventLogTable />} />
          <Route path='sync' element={<MissingUsers />} />
          <Route path='test' element={<App />} />
        </Route>
      </Routes>
    </BrowserRouter>
  </React.StrictMode>
);
