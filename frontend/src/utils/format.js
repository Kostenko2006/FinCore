const CURRENCIES = ['UAH', 'USD', 'EUR']
const DIRECTIONS = ['Income', 'Expense']
const STATUSES = ['Completed', 'Pending', 'Rejected']
const ACCOUNT_TYPES = ['Checking', 'Savings', 'Credit']
const ACCOUNT_STATUSES = ['Active', 'Frozen', 'Closed']
const CARD_TYPES = ['Debit', 'Credit']
const CARD_STATUSES = ['Active', 'Blocked', 'Expired']

function enumValue(value, values, fallback = '') {
  if (typeof value === 'number') {
    return values[value] || fallback
  }

  if (typeof value === 'string') {
    return value
  }

  return fallback
}

export function currencyCode(value) {
  return enumValue(value, CURRENCIES, 'UAH')
}

export function directionValue(value) {
  return enumValue(value, DIRECTIONS, 'Expense')
}

export function statusValue(value) {
  return enumValue(value, STATUSES, 'Completed')
}

export function accountTypeValue(value) {
  return enumValue(value, ACCOUNT_TYPES, 'Checking')
}

export function accountStatusValue(value) {
  return enumValue(value, ACCOUNT_STATUSES, 'Active')
}

export function cardTypeValue(value) {
  return enumValue(value, CARD_TYPES, 'Debit')
}

export function cardStatusValue(value) {
  return enumValue(value, CARD_STATUSES, 'Active')
}

export function isIncome(value) {
  return directionValue(value) === 'Income'
}

export function money(value, currency = 'UAH') {
  return new Intl.NumberFormat('uk-UA', {
    style: 'currency',
    currency: currencyCode(currency),
    maximumFractionDigits: 2,
  }).format(Number(value || 0))
}
