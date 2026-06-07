import { BrowserRouter, Navigate, Route, Routes } from 'react-router-dom'
import AccountDetails from './pages/AccountDetails'
import Accounts from './pages/Accounts'
import Cards from './pages/Cards'
import Dashboard from './pages/Dashboard'
import Login from './pages/Login'
import Register from './pages/Register'
import Transactions from './pages/Transactions'
import Transfer from './pages/Transfer'

import { AuthProvider } from './context/AuthContext'
import PrivateRoute from './components/PrivateRoute'
import Navbar from './components/Navbar'

export default function App() {
  return (
    <AuthProvider>
      <BrowserRouter future={{ v7_startTransition: true, v7_relativeSplatPath: true }}>
        <Navbar />
        <main className="app-shell">
          <Routes>
            <Route path="/" element={<Navigate to="/dashboard" replace />} />
            <Route path="/login" element={<Login />} />
            <Route path="/register" element={<Register />} />
            <Route
              path="/dashboard"
              element={
                <PrivateRoute>
                  <Dashboard />
                </PrivateRoute>
              }
            />
            <Route
              path="/accounts"
              element={
                <PrivateRoute>
                  <Accounts />
                </PrivateRoute>
              }
            />
            <Route
              path="/accounts/:id"
              element={
                <PrivateRoute>
                  <AccountDetails />
                </PrivateRoute>
              }
            />
            <Route
              path="/cards"
              element={
                <PrivateRoute>
                  <Cards />
                </PrivateRoute>
              }
            />
            <Route
              path="/transfer"
              element={
                <PrivateRoute>
                  <Transfer />
                </PrivateRoute>
              }
            />
            <Route
              path="/transactions"
              element={
                <PrivateRoute>
                  <Transactions />
                </PrivateRoute>
              }
            />
          </Routes>
        </main>
      </BrowserRouter>
    </AuthProvider>
  )
}
