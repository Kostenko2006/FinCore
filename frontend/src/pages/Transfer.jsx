import { useEffect, useMemo, useState } from 'react'
import { fetchAccounts } from '../api/accounts'
import { getErrorMessage } from '../api/client'
import { createTransfer } from '../api/transfers'
import { currencyCode, money } from '../utils/format'

export default function Transfer() {
  const [accounts, setAccounts] = useState([])
  const [form, setForm] = useState({
    fromAccountId: '',
    toAccountId: '',
    externalIban: '',
    amount: '',
    currency: 'UAH',
    description: '',
  })
  const [mode, setMode] = useState('own')
  const [notice, setNotice] = useState('')
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(true)
  const [saving, setSaving] = useState(false)

  useEffect(() => {
    async function loadAccounts() {
      setLoading(true)
      try {
        const data = await fetchAccounts()
        setAccounts(data)
        const first = data[0]
        setForm((current) => ({
          ...current,
          fromAccountId: first?.id || '',
          currency: currencyCode(first?.currency),
        }))
      } catch (err) {
        setError(getErrorMessage(err))
      } finally {
        setLoading(false)
      }
    }

    loadAccounts()
  }, [])

  const source = useMemo(
    () => accounts.find((account) => String(account.id) === String(form.fromAccountId)),
    [accounts, form.fromAccountId],
  )

  function updateSource(value) {
    const account = accounts.find((item) => String(item.id) === String(value))
    setForm({ ...form, fromAccountId: value, currency: currencyCode(account?.currency || form.currency), toAccountId: '' })
  }

  async function handleSubmit(event) {
    event.preventDefault()
    setSaving(true)
    setError('')
    setNotice('')

    const payload = {
      fromAccountId: Number(form.fromAccountId),
      toAccountId: mode === 'own' ? Number(form.toAccountId) : null,
      externalIban: mode === 'external' ? form.externalIban : null,
      amount: Number(form.amount),
      currency: form.currency,
      description: form.description || 'Transfer',
    }

    try {
      await createTransfer(payload)
      setNotice('Переказ виконано. Баланс та історію операцій оновлено.')
      setForm({ ...form, amount: '', description: '', externalIban: '', toAccountId: '' })
    } catch (err) {
      setError(getErrorMessage(err))
    } finally {
      setSaving(false)
    }
  }

  const targetAccounts = accounts.filter(
    (account) => String(account.id) !== String(form.fromAccountId) && currencyCode(account.currency) === currencyCode(form.currency),
  )

  return (
    <section className="page-stack transfer-page">
      <div className="dashboard-hero">
        <div>
          <p className="eyebrow">Переказ</p>
          <h1>Створіть безпечний платіж</h1>
        </div>
      </div>

      {loading ? (
        <div className="page-loader">Завантаження рахунків...</div>
      ) : (
        <form className="transfer-shell" onSubmit={handleSubmit}>
          <div className="segmented-control">
            <button className={mode === 'own' ? 'active' : ''} onClick={() => setMode('own')} type="button">Між своїми</button>
            <button className={mode === 'external' ? 'active' : ''} onClick={() => setMode('external')} type="button">На IBAN</button>
          </div>

          <label>
            З рахунку
            <select value={form.fromAccountId} onChange={(event) => updateSource(event.target.value)} required>
              {accounts.map((account) => (
                <option key={account.id} value={account.id}>
                  {account.name} · {money(account.balance, account.currency)}
                </option>
              ))}
            </select>
          </label>

          {source && <div className="hint-box">Доступно: {money(source.balance, source.currency)}</div>}

          {mode === 'own' ? (
            <label>
              На рахунок
              <select value={form.toAccountId} onChange={(event) => setForm({ ...form, toAccountId: event.target.value })} required>
                <option value="">Оберіть рахунок</option>
                {targetAccounts.map((account) => (
                  <option key={account.id} value={account.id}>{account.name} · {currencyCode(account.currency)}</option>
                ))}
              </select>
            </label>
          ) : (
            <label>
              IBAN отримувача
              <input value={form.externalIban} onChange={(event) => setForm({ ...form, externalIban: event.target.value })} required />
            </label>
          )}

          <div className="form-grid">
            <label>
              Сума
              <input type="number" min="0.01" step="0.01" value={form.amount} onChange={(event) => setForm({ ...form, amount: event.target.value })} required />
            </label>
            <label>
              Валюта
              <input value={form.currency} disabled />
            </label>
          </div>

          <label>
            Призначення
            <input value={form.description} onChange={(event) => setForm({ ...form, description: event.target.value })} placeholder="Наприклад: поповнення заощаджень" />
          </label>

          {error && <div className="form-error">{error}</div>}
          {notice && <div className="notice-box">{notice}</div>}

          <button className="primary-button" disabled={saving} type="submit">
            {saving ? 'Виконання...' : 'Виконати переказ'}
          </button>
        </form>
      )}
    </section>
  )
}
