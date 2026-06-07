import api from './client'

export async function fetchTransactions(params = {}) {
  const { data } = await api.get('/transactions', { params })
  return data
}

export async function fetchTransactionStats() {
  const { data } = await api.get('/transactions/stats')
  return data
}
