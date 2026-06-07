import { useEffect, useState } from 'react'
import { fetchAccounts } from '../api/accounts'
import { createCard, fetchCards } from '../api/cards'
import { getErrorMessage } from '../api/client'
import EmptyState from '../components/EmptyState'
import { cardStatusValue, currencyCode, money } from '../utils/format'

export default function Cards() {
  const [cards, setCards] = useState([])
  const [accounts, setAccounts] = useState([])
  const [form, setForm] = useState({ accountId: '', type: 'Debit', dailyLimit: 10000 })
  const [showForm, setShowForm] = useState(false)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')

  useEffect(() => {
    loadData()
  }, [])

  async function loadData() {
    setLoading(true)
    setError('')
    try {
      const [cardData, accountData] = await Promise.all([fetchCards(), fetchAccounts()])
      setCards(cardData)
      setAccounts(accountData)
      setForm((current) => ({ ...current, accountId: current.accountId || accountData[0]?.id || '' }))
    } catch (err) {
      setError(getErrorMessage(err))
    } finally {
      setLoading(false)
    }
  }

  async function handleCreate(event) {
    event.preventDefault()
    try {
      await createCard({ ...form, accountId: Number(form.accountId), dailyLimit: Number(form.dailyLimit) })
      setShowForm(false)
      await loadData()
    } catch (err) {
      setError(getErrorMessage(err))
    }
  }

  return (
    <section className="page-stack">
      <div className="dashboard-hero">
        <div>
          <p className="eyebrow">Картки</p>
          <h1>Картки та ліміти</h1>
        </div>
        <button className="primary-button compact" onClick={() => setShowForm(true)} type="button">Випустити картку</button>
      </div>

      {error && <div className="form-error wide">{error}</div>}

      {loading ? (
        <div className="page-loader">Завантаження карток...</div>
      ) : cards.length ? (
        <section className="card-grid">
          {cards.map((card) => (
            <article className="plastic-card" key={card.id}>
              <span>FinCore</span>
              <strong>{card.maskedNumber}</strong>
              <div>
                <p>{card.cardHolder}</p>
                <p>{String(card.expiryMonth).padStart(2, '0')}/{card.expiryYear}</p>
              </div>
              <small>{card.accountName} · {cardStatusValue(card.status)} · {money(card.dailyLimit, 'UAH')} / день</small>
            </article>
          ))}
        </section>
      ) : (
        <EmptyState title="Карток немає" text="Випустіть картку до активного рахунку." />
      )}

      {showForm && (
        <div className="modal-backdrop">
          <form className="modal-form" onSubmit={handleCreate}>
            <div className="form-head">
              <h2>Нова картка</h2>
              <button aria-label="Закрити" onClick={() => setShowForm(false)} type="button">X</button>
            </div>
            <label>
              Рахунок
              <select value={form.accountId} onChange={(event) => setForm({ ...form, accountId: event.target.value })} required>
                {accounts.map((account) => (
                  <option key={account.id} value={account.id}>{account.name} · {currencyCode(account.currency)}</option>
                ))}
              </select>
            </label>
            <label>
              Тип
              <select value={form.type} onChange={(event) => setForm({ ...form, type: event.target.value })}>
                <option value="Debit">Дебетова</option>
                <option value="Credit">Кредитна</option>
              </select>
            </label>
            <label>
              Денний ліміт
              <input type="number" min="1" value={form.dailyLimit} onChange={(event) => setForm({ ...form, dailyLimit: event.target.value })} />
            </label>
            <div className="form-actions">
              <button className="ghost-button" onClick={() => setShowForm(false)} type="button">Скасувати</button>
              <button className="primary-button compact" type="submit">Створити</button>
            </div>
          </form>
        </div>
      )}
    </section>
  )
}
