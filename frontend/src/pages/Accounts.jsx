import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { createAccount, fetchAccounts } from '../api/accounts'
import { getErrorMessage } from '../api/client'
import EmptyState from '../components/EmptyState'
import { accountStatusValue, accountTypeValue, currencyCode, money } from '../utils/format'

const EMPTY_FORM = { name: '', type: 'Checking', currency: 'UAH' }

export default function Accounts() {
  const [accounts, setAccounts] = useState([])
  const [form, setForm] = useState(EMPTY_FORM)
  const [showForm, setShowForm] = useState(false)
  const [loading, setLoading] = useState(true)
  const [saving, setSaving] = useState(false)
  const [error, setError] = useState('')

  useEffect(() => {
    loadAccounts()
  }, [])

  async function loadAccounts() {
    setLoading(true)
    setError('')
    try {
      setAccounts(await fetchAccounts())
    } catch (err) {
      setError(getErrorMessage(err))
    } finally {
      setLoading(false)
    }
  }

  async function handleCreate(event) {
    event.preventDefault()
    setSaving(true)
    setError('')
    try {
      await createAccount(form)
      setForm(EMPTY_FORM)
      setShowForm(false)
      await loadAccounts()
    } catch (err) {
      setError(getErrorMessage(err))
    } finally {
      setSaving(false)
    }
  }

  return (
    <section className="page-stack">
      <div className="dashboard-hero">
        <div>
          <p className="eyebrow">Рахунки</p>
          <h1>Керуйте балансами та реквізитами</h1>
        </div>
        <button className="primary-button compact" onClick={() => setShowForm(true)} type="button">
          Новий рахунок
        </button>
      </div>

      {error && <div className="form-error wide">{error}</div>}

      {loading ? (
        <div className="page-loader">Завантаження рахунків...</div>
      ) : accounts.length ? (
        <section className="account-grid">
          {accounts.map((account) => (
            <Link className="account-card" key={account.id} to={`/accounts/${account.id}`}>
              <span>{accountTypeValue(account.type)} · {accountStatusValue(account.status)}</span>
              <h2>{account.name}</h2>
              <strong>{money(account.balance, account.currency)}</strong>
              <p>{currencyCode(account.currency)} · {account.iban}</p>
            </Link>
          ))}
        </section>
      ) : (
        <EmptyState title="Рахунків немає" text="Створіть перший рахунок для операцій у FinCore Bank." />
      )}

      {showForm && (
        <div className="modal-backdrop">
          <form className="modal-form" onSubmit={handleCreate}>
            <div className="form-head">
              <h2>Новий рахунок</h2>
              <button aria-label="Закрити" onClick={() => setShowForm(false)} type="button">X</button>
            </div>
            <label>
              Назва
              <input value={form.name} onChange={(event) => setForm({ ...form, name: event.target.value })} required />
            </label>
            <label>
              Тип
              <select value={form.type} onChange={(event) => setForm({ ...form, type: event.target.value })}>
                <option value="Checking">Поточний</option>
                <option value="Savings">Накопичувальний</option>
                <option value="Credit">Кредитний</option>
              </select>
            </label>
            <label>
              Валюта
              <select value={form.currency} onChange={(event) => setForm({ ...form, currency: event.target.value })}>
                <option value="UAH">UAH</option>
                <option value="USD">USD</option>
                <option value="EUR">EUR</option>
              </select>
            </label>
            <div className="form-actions">
              <button className="ghost-button" onClick={() => setShowForm(false)} type="button">Скасувати</button>
              <button className="primary-button compact" disabled={saving} type="submit">
                {saving ? 'Створення...' : 'Створити'}
              </button>
            </div>
          </form>
        </div>
      )}
    </section>
  )
}
