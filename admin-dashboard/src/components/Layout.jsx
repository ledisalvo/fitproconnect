import { Link, Outlet, useLocation } from 'react-router-dom';

export default function Layout() {
  const location = useLocation();

  const navItems = [
    { path: '/users', label: 'Users' },
    { path: '/events', label: 'Events' },
    { path: '/sync', label: 'Sync' },
  ];

  return (
    <div className='min-h-screen bg-gray-50'>
      <nav className='bg-white shadow px-6 py-4 flex gap-4'>
        {navItems.map((item) => (
          <Link
            key={item.path}
            to={item.path}
            className={`font-medium ${
              location.pathname === item.path
                ? 'text-blue-600'
                : 'text-gray-600 hover:text-blue-500'
            }`}
          >
            {item.label}
          </Link>
        ))}
      </nav>
      <main className='p-6'>
        <Outlet />
      </main>
    </div>
  );
}
