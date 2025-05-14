import React from 'react';
import './App.css';
import { BrowserRouter as Router, Routes, Route, Link, Navigate } from 'react-router-dom';
import { AuthProvider, useAuth } from './contexts/AuthContext';
import Login from './components/Auth/Login';
import Register from './components/Auth/Register';
import InstructorDashboard from './components/Dashboard/InstructorDashboard';
import CourseList from './components/Courses/CourseList';

// Placeholder for Student Dashboard
const StudentDashboard = () => <div className="container mt-3"><h2>Student Dashboard</h2><p>Welcome, Student! View your enrolled courses here.</p><CourseList /></div>;

const HomePage = () => (
    <div className="container mt-3">
        <h2>Welcome to EduPlatform!</h2>
        <p>Your one-stop destination for learning and teaching. Browse our available courses below.</p>
        <hr />
        <CourseList />
    </div>
);


function AppContent() {
  const { user, logout, isAuthenticated, hasRole } = useAuth();

  return (
    <Router>
      <nav className="navbar navbar-expand-lg navbar-dark bg-dark sticky-top">
        <div className="container-fluid">
          <Link className="navbar-brand" to="/">EduPlatform</Link>
          <button className="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarNav" aria-controls="navbarNav" aria-expanded="false" aria-label="Toggle navigation">
            <span className="navbar-toggler-icon"></span>
          </button>
          <div className="collapse navbar-collapse" id="navbarNav">
            <ul className="navbar-nav ms-auto mb-2 mb-lg-0">
              <li className="nav-item">
                <Link className="nav-link" to="/">Home</Link>
              </li>
               <li className="nav-item">
                <Link className="nav-link" to="/courses">Courses</Link>
              </li>
              {!isAuthenticated && (
                <>
                  <li className="nav-item">
                    <Link className="nav-link" to="/login">Login</Link>
                  </li>
                  <li className="nav-item">
                    <Link className="nav-link" to="/register">Register</Link>
                  </li>
                </>
              )}
              {isAuthenticated && (
                <>
                  <li className="nav-item">
                    <Link className="nav-link" to="/dashboard">Dashboard</Link>
                  </li>
                  <li className="nav-item">
                    <span className="nav-link text-light">Hi, {user.Name} ({user.Role})</span>
                  </li>
                  <li className="nav-item">
                    <button className="btn btn-outline-light ms-2" onClick={logout}>Logout</button>
                  </li>
                </>
              )}
            </ul>
          </div>
        </div>
      </nav>

      <main className="container mt-4 flex-grow-1">
        <Routes>
          <Route path="/" element={<HomePage />} />
          <Route path="/courses" element={<CourseList />} />
          <Route path="/login" element={!isAuthenticated ? <Login /> : <Navigate to="/dashboard" />} />
          <Route path="/register" element={!isAuthenticated ? <Register /> : <Navigate to="/dashboard" />} />
          
          <Route 
            path="/dashboard"
            element={
              !isAuthenticated ? <Navigate to="/login" /> :
              hasRole('Instructor') ? <InstructorDashboard /> :
              hasRole('Student') ? <StudentDashboard /> :
              <Navigate to="/" />
            }
          />

        </Routes>
      </main>

      <footer className="bg-dark text-white text-center p-3 mt-auto">
        <p>&copy; {new Date().getFullYear()} EduPlatform. All Rights Reserved.</p>
      </footer>
    </Router>
  );
}

function App() {
  return (
    <AuthProvider>
      <div className="d-flex flex-column min-vh-100">
        <AppContent />
      </div>
    </AuthProvider>
  );
}

export default App;
