import api from './client'

export async function createTransfer(payload) {
  const { data } = await api.post('/transfers', payload)
  return data
}
