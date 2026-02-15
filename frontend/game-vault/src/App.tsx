import { useState, useEffect } from 'react'
import './App.css'

function App() {
  const [message, setMessage] = useState<string>('Loading...')

  useEffect(() => {
    fetch('/api/hello')
      .then((res) => res.json())
      .then((data) => setMessage(data.message))
      .catch(() => setMessage('Failed to connect to backend'))
  }, [])

  return (
    <>
      <h1>Game Vault</h1>
      <p>{message}</p>
    </>
  )
}

export default App
