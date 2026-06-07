import { useEffect, useMemo, useState } from 'react'
import { fetchAccounts } from '../api/accounts'
import { getErrorMessage } from '../api/client'
import { fetchTransactions } from '../api/transactions'
import EmptyState from '../components/EmptyState'
import { isIncome, money, statusValue } from '../utils/format'

export default function Transactions() {
  const [transactions, setTransactions] = useState([])
  const [accounts, setAccounts] = useState([])
  const [filters, setFilters] = useState({ accountId: '', direction: '', status: '' })
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')

  useEffect(() => {
    async function loadAccounts() {
      try {
        setAccounts(await fetchAccounts())
      } catch (err) {
        setError(getErrorMessage(err))
      }
    }

    loadAccounts()
  }, [])

  useEffect(() => {
    async function loadTransactions() {
      setLoading(true)
      setError('')
      try {
        const params = Object.fromEntries(Object.entries(filters).filter(([, value]) => value))
        setTransactions(await fetchTransactions(params))
      } catch (err) {
        setError(getErrorMessage(err))
      } finally {
        setLoading(false)
      }
    }

    loadTransactions()
  }, [filters])

  const totals = useMemo(() => ({
    income: transactions.filter((item) => isIncome(item.direction)).reduce((sum, item) => sum + Number(item.amount), 0),
    expense: transactions.filter((item) => !isIncome(item.direction)).reduce((sum, item) => sum + Number(item.amount), 0),
  }), [transactions])

  return (
    <section className="page-stack">
      <div className="dashboard-hero">
        <div>
          <p className="eyebrow">Операції</p>
          <h1>Історія руху коштів</h1>
        </div>
      </div>

      {error && <div className="form-error wide">{error}</div>}

      <section className="stats-grid">
        <article className="stat-tile green"><span>Доходи у вибірці</span><strong>{money(totals.income, 'UAH')}</strong></article>
        <article className="stat-tile gold"><span>Витрати у вибірці</span><strong>{money(totals.expense, 'UAH')}</strong></article>
        <article className="stat-tile blue"><span>Кількість</span><strong>{transactions.length}</strong></article>
      </section>

      <section className="filters-row">
        <label>
          Рахунок
          <select value={filters.accountId} onChange={(event) => setFilters({ ...filters, accountId: event.target.value })}>
            <option value="">Всі рахунки</option>
            {accounts.map((account) => (
              <option key={account.id} value={account.id}>{account.name}</option>
            ))}
          </select>
        </label>
        <label>
          Напрям
          <select value={filters.direction} onChange={(event) => setFilters({ ...filters, direction: event.target.value })}>
            <option value="">Всі</option>
            <option value="Income">Доходи</option>
            <option value="Expense">Витрати</option>
          </select>
        </label>
        <label>
          Статус
          <select value={filters.status} onChange={(event) => setFilters({ ...filters, status: event.target.value })}>
            <option value="">Всі</option>
            <option value="Completed">Виконано</option>
            <option value="Pending">В обробці</option>
            <option value="Rejected">Відхилено</option>
          </select>
        </label>
      </section>

      {loading ? (
        <div className="page-loader">Завантаження операцій...</div>
      ) : transactions.length ? (
        <div className="table-shell">
          <table>
            <thead>
              <tr>
                <th>Дата</th>
                <th>Опис</th>
                <th>Рахунок</th>
                <th>Категорія</th>
                <th>Статус</th>
                <th>Сума</th>
              </tr>
            </thead>
            <tbody>
              {transactions.map((item) => (
                <tr key={item.id}>
                  <td>{new Date(item.createdAt).toLocaleDateString('uk-UA')}</td>
                  <td>{item.description}<span>{item.counterparty}</span></td>
                  <td>{item.accountName}</td>
                  <td>{item.category}</td>
                  <td><span className={`status-pill ${statusValue(item.status).toLowerCase()}`}>{statusValue(item.status)}</span></td>
                  <td className={isIncome(item.direction) ? 'amount-positive' : 'amount-negative'}>
                    {isIncome(item.direction) ? '+' : '-'}{money(item.amount, item.currency)}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      ) : (
        <EmptyState title="Операцій не знайдено" text="Змініть фільтри або виконайте перший переказ." />
      )}
    </section>
  )
}
