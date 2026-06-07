import { useEffect, useMemo, useState } from 'react'
import { Link } from 'react-router-dom'
import { Area, AreaChart, Cell, Pie, PieChart, ResponsiveContainer, Tooltip, XAxis, YAxis } from 'recharts'
import { fetchDashboardSummary } from '../api/dashboard'
import { getErrorMessage } from '../api/client'
import EmptyState from '../components/EmptyState'
import { useAuth } from '../context/AuthContext'
import { currencyCode, isIncome, money } from '../utils/format'

const COLORS = ['#146c5f', '#2f6f8f', '#d19b2c', '#7b61ff', '#dc5f57']

export default function Dashboard() {
  const { user } = useAuth()
  const [summary, setSummary] = useState(null)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')

  useEffect(() => {
    async function loadSummary() {
      setLoading(true)
      setError('')
      try {
        setSummary(await fetchDashboardSummary())
      } catch (err) {
        setError(getErrorMessage(err))
      } finally {
        setLoading(false)
      }
    }

    loadSummary()
  }, [])

  const firstName = useMemo(() => user?.fullName?.split(' ')[0] || 'клієнт', [user])

  if (loading) {
    return <div className="page-loader">Завантаження FinCore Bank...</div>
  }

  if (error) {
    return <div className="form-error wide">{error}</div>
  }

  if (!summary) {
    return null
  }

  return (
    <section className="dashboard">
      <div className="dashboard-hero">
        <div>
          <p className="eyebrow">FinCore Bank</p>
          <h1>Фінансовий огляд, {firstName}</h1>
        </div>
        <div className="hero-actions">
          <Link className="primary-button compact" to="/transfer">Новий переказ</Link>
          <Link className="ghost-button compact" to="/accounts">Рахунки</Link>
        </div>
      </div>

      <section className="stats-grid">
        <StatTile label="Еквівалент у UAH" value={money(summary.totalUahEquivalent, 'UAH')} tone="ink" />
        <StatTile label="Активні рахунки" value={summary.activeAccounts} tone="green" />
        <StatTile label="Активні картки" value={summary.activeCards} tone="blue" />
        <StatTile label="Витрати місяця" value={money(summary.monthlyExpense, 'UAH')} tone="gold" />
      </section>

      <section className="insights-band">
        <article className="panel">
          <div className="panel-head">
            <div>
              <span>Динаміка</span>
              <strong>Доходи та витрати</strong>
            </div>
          </div>
          <ResponsiveContainer width="100%" height={220}>
            <AreaChart data={summary.monthlyFlow}>
              <XAxis dataKey="month" />
              <YAxis />
              <Tooltip formatter={(value) => money(value, 'UAH')} />
              <Area type="monotone" dataKey="income" stroke="#146c5f" fill="#146c5f22" name="Доходи" />
              <Area type="monotone" dataKey="expense" stroke="#dc5f57" fill="#dc5f5722" name="Витрати" />
            </AreaChart>
          </ResponsiveContainer>
        </article>

        <article className="panel">
          <div className="panel-head">
            <div>
              <span>Категорії</span>
              <strong>Структура витрат</strong>
            </div>
          </div>
          {summary.categoryExpenses.length ? (
            <ResponsiveContainer width="100%" height={220}>
              <PieChart>
                <Pie data={summary.categoryExpenses} dataKey="amount" innerRadius={52} outerRadius={86} paddingAngle={4}>
                  {summary.categoryExpenses.map((item, index) => (
                    <Cell key={item.category} fill={COLORS[index % COLORS.length]} />
                  ))}
                </Pie>
                <Tooltip formatter={(value) => money(value, 'UAH')} />
              </PieChart>
            </ResponsiveContainer>
          ) : (
            <EmptyState title="Витрат ще немає" text="Після перших операцій тут з'явиться аналітика." />
          )}
        </article>
      </section>

      <section className="content-grid">
        <article className="panel">
          <div className="panel-head">
            <div>
              <span>Баланси</span>
              <strong>Ваші валюти</strong>
            </div>
            <Link to="/accounts">Всі рахунки</Link>
          </div>
          <div className="balance-list">
            {summary.balances.map((item) => (
              <div className="balance-row" key={item.currency}>
                <span>{currencyCode(item.currency)}</span>
                <strong>{money(item.balance, item.currency)}</strong>
              </div>
            ))}
          </div>
        </article>

        <article className="panel">
          <div className="panel-head">
            <div>
              <span>Останні</span>
              <strong>Операції</strong>
            </div>
            <Link to="/transactions">Вся історія</Link>
          </div>
          <TransactionList transactions={summary.recentTransactions} />
        </article>
      </section>
    </section>
  )
}

function StatTile({ label, value, tone }) {
  return (
    <article className={`stat-tile ${tone}`}>
      <span>{label}</span>
      <strong>{value}</strong>
    </article>
  )
}

export function TransactionList({ transactions }) {
  if (!transactions.length) {
    return <EmptyState title="Операцій немає" text="Історія з'явиться після першого платежу або переказу." />
  }

  return (
    <div className="transaction-list">
      {transactions.map((item) => (
        <div className="transaction-row" key={item.id}>
          <div>
            <strong>{item.description}</strong>
            <span>{item.accountName} · {item.category}</span>
          </div>
          <p className={isIncome(item.direction) ? 'amount-positive' : 'amount-negative'}>
            {isIncome(item.direction) ? '+' : '-'}{money(item.amount, item.currency)}
          </p>
        </div>
      ))}
    </div>
  )
}
