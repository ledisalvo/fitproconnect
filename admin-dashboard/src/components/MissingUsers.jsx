import { useEffect, useState } from 'react';

export default function MissingUsers() {
  const [missing, setMissing] = useState([]);
  const [loading, setLoading] = useState(true);
  const [syncing, setSyncing] = useState(false);
  const [syncResult, setSyncResult] = useState(null);

  const fetchMissing = () => {
    setLoading(true);
    fetch('http://localhost:5002/api/sync/missing')
      .then((res) => res.json())
      .then((data) => setMissing(data))
      .finally(() => setLoading(false));
  };

  const handleSync = () => {
    setSyncing(true);
    fetch('http://localhost:5002/api/sync/reprocess', { method: 'POST' })
      .then((res) => res.json())
      .then((data) => {
        setSyncResult(data);
        fetchMissing(); // refresh list after sync
      })
      .finally(() => setSyncing(false));
  };

  useEffect(() => {
    fetchMissing();
  }, []);

  return (
    <div className='p-6 mt-8'>
      <h2 className='text-2xl font-semibold mb-4'>
        Missing Users in UserService
      </h2>

      <button
        onClick={handleSync}
        disabled={syncing || missing.length === 0}
        className='mb-4 bg-green-600 text-white px-4 py-2 rounded hover:bg-green-700 disabled:opacity-50'
      >
        {syncing ? 'Syncing...' : 'Reprocess Missing Users'}
      </button>

      {syncResult && (
        <p className='mb-4 text-green-700'>
          ✅ {syncResult.reprocessed} user(s) reprocessed
        </p>
      )}

      {loading ? (
        <p className='text-gray-500'>Loading missing users...</p>
      ) : missing.length === 0 ? (
        <p className='text-gray-500'>No missing users detected 🎉</p>
      ) : (
        <ul className='bg-white border rounded shadow p-4 space-y-2'>
          {missing.map((email) => (
            <li
              key={email}
              className='text-gray-800 border-b pb-2 last:border-none'
            >
              {email}
            </li>
          ))}
        </ul>
      )}
    </div>
  );
}
