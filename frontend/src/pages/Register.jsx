import { useState } from 'react'
import { Link, Navigate, useNavigate } from 'react-router-dom'
import { registerUser } from '../api/auth'
import { getErrorMessage } from '../api/client'
import { useAuth } from '../context/AuthContext'

export default function Register() {
  const { login, isAuthenticated } = useAuth()
  const navigate = useNavigate()
  const [form, setForm] = useState({ fullName: '', email: '', password: '' })
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(false)

  if (isAuthenticated) {
    return <Navigate to="/dashboard" replace />
  }

  async function handleSubmit(event) {
    event.preventDefault()
    setLoading(true)
    setError('')
    try {
      const auth = await registerUser(form)
      login(auth)
      navigate('/dashboard')
    } catch (err) {
      setError(getErrorMessage(err))
    } finally {
      setLoading(false)
    }
  }

  return (
    <section className="auth-layout">
      <div className="auth-panel">
        <div>
          <p className="eyebrow">Новий клієнт</p>
          <h1>Створіть банківський профіль</h1>
        </div>

        <form className="auth-form" onSubmit={handleSubmit}>
          <label>
            Ім'я
            <input
              value={form.fullName}
              onChange={(event) => setForm({ ...form, fullName: event.target.value })}
              required
            />
          </label>
          <label>
            Email
            <input
              value={form.email}
              onChange={(event) => setForm({ ...form, email: event.target.value })}
              type="email"
              required
            />
          </label>
          <label>
            Пароль
            <input
              value={form.password}
              onChange={(event) => setForm({ ...form, password: event.target.value })}
              type="password"
              minLength={6}
              required
            />
          </label>
          {error && <div className="form-error">{error}</div>}
          <button className="primary-button" disabled={loading} type="submit">
            {loading ? 'Створення...' : 'Зареєструватися'}
          </button>
        </form>

        <p className="auth-switch">
          Вже є профіль? <Link to="/login">Увійти</Link>
        </p>
      </div>

      <div className="auth-art register-art" aria-hidden="true">
        <div className="note-stack">
          <span>IBAN</span>
          <span>UAH</span>
          <span>SAFE</span>
        </div>
      </div>
    </section>
  )
}
