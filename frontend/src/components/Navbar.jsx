import { NavLink, useNavigate } from 'react-router-dom'
import { useAuth } from '../context/AuthContext'

export default function Navbar() {
  const { user, isAuthenticated, logout } = useAuth()
  const navigate = useNavigate()

  function handleLogout() {
    logout()
    navigate('/login')
  }

  return (
    <header className="topbar">
      <NavLink to="/" className="brand">
        <span className="brand-mark">F</span>
        <span>FinCore Bank</span>
      </NavLink>

      <nav className="topnav">
        {isAuthenticated ? (
          <>
            <NavLink to="/dashboard">Огляд</NavLink>
            <NavLink to="/accounts">Рахунки</NavLink>
            <NavLink to="/cards">Картки</NavLink>
            <NavLink to="/transfer">Переказ</NavLink>
            <NavLink to="/transactions">Операції</NavLink>
            <span className="user-pill">{user?.fullName || user?.email}</span>
            <button className="ghost-button" onClick={handleLogout} type="button">
              Вийти
            </button>
          </>
        ) : (
          <>
            <NavLink to="/login">Вхід</NavLink>
            <NavLink to="/register" className="nav-cta">
              Реєстрація
            </NavLink>
          </>
        )}
      </nav>
    </header>
  )
}
