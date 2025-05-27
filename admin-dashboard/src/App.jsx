import { useState } from 'react';

function App() {
  const [count, setCount] = useState(0);

  return (
    <>
      {/* <div className='min-h-screen bg-gray-50 p-4'>
        <UserTable />
      </div>
      <div className='p-4 bg-gray-100 min-h-screen'>
        <EventLogTable />
      </div>
      <div className='p-4 bg-gray-100 min-h-screen'>
        <MissingUsers />
      </div> */}
      <div className='min-h-screen bg-gray-100 p-8'>
        <h1 className='text-4xl font-bold text-blue-600 underline'>
          Tailwind está funcionando 🎉
        </h1>
      </div>
    </>
  );
}

export default App;
