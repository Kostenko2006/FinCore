import { useState } from 'react'
import { Link, Navigate, useNavigate } from 'react-router-dom'
import { loginUser } from '../api/auth'
import { getErrorMessage } from '../api/client'
import { useAuth } from '../context/AuthContext'

export default function Login() {
  const { login, isAuthenticated } = useAuth()
  const navigate = useNavigate()
  const [form, setForm] = useState({ email: 'demo@fincore.local', password: 'password123' })
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
      const auth = await loginUser(form)
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
          <p className="eyebrow">Онлайн-банкінг</p>
          <h1>Увійдіть до FinCore Bank</h1>
        </div>

        <form className="auth-form" onSubmit={handleSubmit}>
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
              required
            />
          </label>
          {error && <div className="form-error">{error}</div>}
          <button className="primary-button" disabled={loading} type="submit">
            {loading ? 'Вхід...' : 'Увійти'}
          </button>
        </form>

        <p className="auth-switch">
          Немає акаунта? <Link to="/register">Створити профіль</Link>
        </p>
      </div>

      <div className="auth-art" aria-hidden="true">
        <div className="bank-card-preview">
          <span>FinCore</span>
          <strong>5168 **** **** 4829</strong>
          <small>DEMO CLIENT</small>
        </div>
        <div className="finance-card">
          <span>Баланс</span>
          <strong>84 250 UAH</strong>
          <small>демо-кабінет</small>
        </div>
      </div>
    </section>
  )
}
