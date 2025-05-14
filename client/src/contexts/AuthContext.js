import React, { createContext, useState, useContext, useEffect } from 'react';

const AuthContext = createContext(null);

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null); // User object: { UserId, Name, Email, Role }
  const [token, setToken] = useState(localStorage.getItem('authToken')); // Load token from localStorage

  useEffect(() => {
    const storedToken = localStorage.getItem('authToken');
    const storedUser = localStorage.getItem('user');
    if (storedToken && storedUser) {
      // In a real app, you might want to verify the token with the backend here.
      // For mock purposes, if a token and user exist, we assume they are valid.
      setToken(storedToken);
      setUser(JSON.parse(storedUser));
      console.log("AuthContext: Session restored from localStorage");
    } else {
      console.log("AuthContext: No active session found in localStorage");
    }
  }, []); // Empty dependency array ensures this runs once on mount

  const login = (userDataWithToken) => {
    const { token: receivedToken, ...userDetails } = userDataWithToken;
    setUser(userDetails);
    setToken(receivedToken);
    localStorage.setItem('user', JSON.stringify(userDetails));
    localStorage.setItem('authToken', receivedToken);
    console.log("AuthContext: User logged in, token set:", receivedToken);
  };

  const logout = () => {
    setUser(null);
    setToken(null);
    localStorage.removeItem('user');
    localStorage.removeItem('authToken');
    console.log("AuthContext: User logged out, token removed");
    // In a real app, you might also call a backend endpoint to invalidate the session/token on the server.
  };

  const hasRole = (role) => {
    return user && user.Role === role;
  };

  // isAuthenticated can now be more robustly based on the presence of a token
  const isAuthenticated = !!token && !!user; 

  return (
    <AuthContext.Provider value={{ user, token, login, logout, isAuthenticated, hasRole }}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => useContext(AuthContext); 