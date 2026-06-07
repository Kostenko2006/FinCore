import api from './client'

export async function fetchAccounts() {
  const { data } = await api.get('/accounts')
  return data
}

export async function fetchAccount(id) {
  const { data } = await api.get(`/accounts/${id}`)
  return data
}

export async function createAccount(payload) {
  const { data } = await api.post('/accounts', payload)
  return data
}
