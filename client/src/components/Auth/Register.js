import React, { useState } from 'react';
// import { useAuth } from '../../contexts/AuthContext'; // Not typically used on register, or used differently
import authService from '../../services/authService'; // Use the centralized service
import { Link } from 'react-router-dom'; // For linking to Login page
// import { useNavigate } from 'react-router-dom';

const Register = () => {
  const [name, setName] = useState('');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [role, setRole] = useState('Student');
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  const [loading, setLoading] = useState(false);
  // const { login } = useAuth(); // Usually, you don't auto-login on register, but redirect
  // const navigate = useNavigate();

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setSuccess('');

    if (password !== confirmPassword) {
      setError('Passwords do not match.');
      return;
    }
    setLoading(true);

    try {
      // Call the centralized authService for registration
      await authService.register(name, email, password, role);
      
      setSuccess('Registration successful! You can now login.');
      // Clear form after successful registration
      setName('');
      setEmail('');
      setPassword('');
      setConfirmPassword('');
      setRole('Student');
      // navigate('/login'); // Optionally redirect to login page
    } catch (err) {
      setError(err.message || 'Failed to register. Please try again.');
    }
    setLoading(false);
  };

  return (
    <div className="row justify-content-center mt-5">
      <div className="col-md-6 col-lg-5">
        <div className="card shadow-sm">
          <div className="card-body">
            <h2 className="card-title text-center mb-4">Register</h2>
            <form onSubmit={handleSubmit}>
              {error && <div className="alert alert-danger">{error}</div>}
              {success && <div className="alert alert-success">{success}</div>}
              
              <div className="mb-3">
                <label htmlFor="nameInput" className="form-label">Full Name</label>
                <input
                  type="text"
                  className="form-control"
                  id="nameInput"
                  value={name}
                  onChange={(e) => setName(e.target.value)}
                  required
                  placeholder="Enter your full name"
                />
              </div>

              <div className="mb-3">
                <label htmlFor="emailInputReg" className="form-label">Email address</label>
                <input
                  type="email"
                  className="form-control"
                  id="emailInputReg"
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
                  required
                  placeholder="Enter your email"
                />
              </div>

              <div className="mb-3">
                <label htmlFor="passwordInputReg" className="form-label">Password</label>
                <input
                  type="password"
                  className="form-control"
                  id="passwordInputReg"
                  value={password}
                  onChange={(e) => setPassword(e.target.value)}
                  required
                  placeholder="Create a password (min. 6 characters suggested)"
                />
              </div>

              <div className="mb-3">
                <label htmlFor="confirmPasswordInput" className="form-label">Confirm Password</label>
                <input
                  type="password"
                  className="form-control"
                  id="confirmPasswordInput"
                  value={confirmPassword}
                  onChange={(e) => setConfirmPassword(e.target.value)}
                  required
                  placeholder="Confirm your password"
                />
              </div>

              <div className="mb-3">
                <label htmlFor="roleSelect" className="form-label">Register as</label>
                <select 
                  id="roleSelect" 
                  className="form-select" 
                  value={role} 
                  onChange={(e) => setRole(e.target.value)} 
                  required
                >
                  <option value="Student">Student</option>
                  <option value="Instructor">Instructor</option>
                </select>
              </div>

              <div className="d-grid">
                <button type="submit" className="btn btn-primary" disabled={loading}>
                  {loading ? (
                    <>
                      <span className="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span>
                      <span className="visually-hidden">Loading...</span> Registering...
                    </>
                  ) : 'Register'}
                </button>
              </div>
            </form>
            <div className="mt-3 text-center">
              <p>Already have an account? <Link to="/login">Login here</Link></p>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default Register; 