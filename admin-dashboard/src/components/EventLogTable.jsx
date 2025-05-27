import { useEffect, useState } from 'react';

export default function EventLogTable() {
  const [logs, setLogs] = useState([]);
  const [filters, setFilters] = useState({ email: '', status: '', source: '' });
  const [loading, setLoading] = useState(true);

  const fetchLogs = () => {
    setLoading(true);
    const params = new URLSearchParams();

    if (filters.email) params.append('email', filters.email);
    if (filters.status) params.append('status', filters.status);
    if (filters.source) params.append('source', filters.source);

    fetch(`http://localhost:5002/api/events/logs?${params.toString()}`)
      .then((res) => res.json())
      .then((data) => setLogs(data))
      .finally(() => setLoading(false));
  };

  useEffect(() => {
    fetchLogs();
  }, []);

  const handleFilterChange = (e) => {
    setFilters((prev) => ({ ...prev, [e.target.name]: e.target.value }));
  };

  const handleSearch = () => {
    fetchLogs();
  };

  return (
    <div className='p-6 mt-8'>
      <h2 className='text-2xl font-semibold mb-4'>Event Log</h2>

      <div className='flex gap-4 mb-4 flex-wrap'>
        <input
          type='text'
          name='email'
          placeholder='Filter by email'
          value={filters.email}
          onChange={handleFilterChange}
          className='border p-2 rounded w-full md:w-1/4'
        />
        <select
          name='status'
          value={filters.status}
          onChange={handleFilterChange}
          className='border p-2 rounded w-full md:w-1/4'
        >
          <option value=''>All statuses</option>
          <option value='consumido'>Consumido</option>
          <option value='reprocesado'>Reprocesado</option>
        </select>
        <select
          name='source'
          value={filters.source}
          onChange={handleFilterChange}
          className='border p-2 rounded w-full md:w-1/4'
        >
          <option value=''>All sources</option>
          <option value='evento'>Evento</option>
          <option value='sync'>Sync</option>
        </select>
        <button
          onClick={handleSearch}
          className='bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700 w-full md:w-auto'
        >
          Apply Filters
        </button>
      </div>

      {loading ? (
        <p className='text-gray-500'>Loading logs...</p>
      ) : (
        <div className='overflow-x-auto'>
          <table className='min-w-full bg-white border shadow'>
            <thead className='bg-gray-100'>
              <tr>
                <th className='p-3 text-left border-b'>Email</th>
                <th className='p-3 text-left border-b'>Status</th>
                <th className='p-3 text-left border-b'>Source</th>
                <th className='p-3 text-left border-b'>Timestamp</th>
              </tr>
            </thead>
            <tbody>
              {logs.length === 0 && (
                <tr>
                  <td colSpan='4' className='p-4 text-center text-gray-500'>
                    No event logs found
                  </td>
                </tr>
              )}
              {logs.map((log) => (
                <tr key={log.id} className='hover:bg-gray-50'>
                  <td className='p-3 border-b'>{log.email}</td>
                  <td className='p-3 border-b'>{log.status}</td>
                  <td className='p-3 border-b'>{log.source}</td>
                  <td className='p-3 border-b'>
                    {new Date(log.timestamp).toLocaleString()}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
}
