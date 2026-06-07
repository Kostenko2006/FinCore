import { useEffect, useState } from 'react'
import { Link, useParams } from 'react-router-dom'
import { fetchAccount } from '../api/accounts'
import { getErrorMessage } from '../api/client'
import { fetchTransactions } from '../api/transactions'
import { TransactionList } from './Dashboard'
import { accountStatusValue, accountTypeValue, currencyCode, money } from '../utils/format'

export default function AccountDetails() {
  const { id } = useParams()
  const [account, setAccount] = useState(null)
  const [transactions, setTransactions] = useState([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')

  useEffect(() => {
    async function loadData() {
      setLoading(true)
      setError('')
      try {
        const [accountData, transactionData] = await Promise.all([
          fetchAccount(id),
          fetchTransactions({ accountId: id }),
        ])
        setAccount(accountData)
        setTransactions(transactionData)
      } catch (err) {
        setError(getErrorMessage(err))
      } finally {
        setLoading(false)
      }
    }

    loadData()
  }, [id])

  if (loading) {
    return <div className="page-loader">Завантаження рахунку...</div>
  }

  if (error) {
    return <div className="form-error wide">{error}</div>
  }

  if (!account) {
    return null
  }

  return (
    <section className="page-stack">
      <Link className="back-link" to="/accounts">Назад до рахунків</Link>

      <article className="details-hero bank-details">
        <div>
          <p className="eyebrow">{accountTypeValue(account.type)} · {accountStatusValue(account.status)}</p>
          <h1>{account.name}</h1>
          <strong>{money(account.balance, account.currency)}</strong>
        </div>
        <div className="requisites">
          <span>IBAN</span>
          <p>{account.iban}</p>
          <span>Номер рахунку</span>
          <p>{account.accountNumber}</p>
        </div>
      </article>

      <section className="content-grid">
        <article className="panel">
          <div className="panel-head">
            <div>
              <span>Реквізити</span>
              <strong>Для платежів</strong>
            </div>
          </div>
          <div className="info-grid">
            <span>Валюта</span><strong>{currencyCode(account.currency)}</strong>
            <span>Статус</span><strong>{accountStatusValue(account.status)}</strong>
            <span>Відкрито</span><strong>{new Date(account.createdAt).toLocaleDateString('uk-UA')}</strong>
          </div>
        </article>

        <article className="panel">
          <div className="panel-head">
            <div>
              <span>Операції</span>
              <strong>По рахунку</strong>
            </div>
            <Link to="/transfer">Переказ</Link>
          </div>
          <TransactionList transactions={transactions} />
        </article>
      </section>
    </section>
  )
}
