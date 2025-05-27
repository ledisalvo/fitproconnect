import { useEffect, useState } from 'react';

export default function UserTable() {
  const [users, setUsers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    fetch('http://localhost:5002/api/user/all')
      .then((res) => {
        if (!res.ok) throw new Error('Failed to fetch users');
        return res.json();
      })
      .then((data) => setUsers(data))
      .catch((err) => setError(err.message))
      .finally(() => setLoading(false));
  }, []);

  if (loading)
    return <p className='text-center text-gray-500'>Loading users...</p>;
  if (error) return <p className='text-center text-red-600'>{error}</p>;

  return (
    <div className='p-6'>
      <h2 className='text-2xl font-semibold mb-4'>Registered Users</h2>
      <div className='overflow-x-auto'>
        <table className='min-w-full bg-white border border-gray-200 shadow'>
          <thead className='bg-gray-100'>
            <tr>
              <th className='text-left p-3 border-b'>Email</th>
              <th className='text-left p-3 border-b'>Role</th>
              <th className='text-left p-3 border-b'>Registered At</th>
            </tr>
          </thead>
          <tbody>
            {users.length === 0 && (
              <tr>
                <td colSpan='3' className='text-center p-4 text-gray-500'>
                  No users found
                </td>
              </tr>
            )}
            {users.map((user) => (
              <tr key={user.id} className='hover:bg-gray-50'>
                <td className='p-3 border-b'>{user.email}</td>
                <td className='p-3 border-b'>{user.role}</td>
                <td className='p-3 border-b'>
                  {new Date(user.registeredAt).toLocaleString()}
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
}
