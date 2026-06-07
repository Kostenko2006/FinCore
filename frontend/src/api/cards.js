import api from './client'

export async function fetchCards() {
  const { data } = await api.get('/cards')
  return data
}

export async function createCard(payload) {
  const { data } = await api.post('/cards', payload)
  return data
}
