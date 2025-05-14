//const API_URL = 'your_backend_api_url/auth'; // Replace with your actual backend API URL

// In-memory store for mock users
let mockUsers = [
  // You can pre-populate with some users if needed, or keep it empty
  // Example:
  // { UserId: 'predefined-student-guid', Name: 'Predefined Student', Email: 'student@example.com', PasswordHash: 'password', Role: 'Student' },
  // { UserId: 'predefined-instructor-guid', Name: 'Predefined Instructor', Email: 'instructor@example.com', PasswordHash: 'password', Role: 'Instructor' }
];

const register = async (name, email, password, role) => {
  console.log("Attempting to register user in mock service:", { name, email, role });
  
  // Check if user already exists
  const existingUser = mockUsers.find(user => user.Email === email);
  if (existingUser) {
    console.log("Mock registration failed: User already exists with email:", email);
    return Promise.reject(new Error('User with this email already exists.'));
  }

  // Simulate password hashing (in a real app, backend does this securely)
  const passwordHash = password; // Plain text for mock, DO NOT DO THIS IN PRODUCTION
  const newUser = {
    UserId: `guid-${Math.random().toString(36).substr(2, 9)}${Date.now()}`,
    Name: name,
    Email: email,
    PasswordHash: passwordHash, // Store the "hashed" password
    Role: role,
  };

  mockUsers.push(newUser);
  console.log("Mock user registered successfully:", newUser);
  console.log("Current mockUsers list:", mockUsers);
  // Return a subset of user data, similar to what a real API might do (excluding password)
  return Promise.resolve({ UserId: newUser.UserId, Name: newUser.Name, Email: newUser.Email, Role: newUser.Role });
};

const login = async (email, password) => {
  console.log("Attempting to login user in mock service:", { email });
  const user = mockUsers.find(u => u.Email === email);

  if (user && user.PasswordHash === password) { // In a real app, backend compares hashed passwords
    console.log("Mock login successful for:", email);
    // Return user data (excluding password hash)
    const { PasswordHash, ...userDataToReturn } = user;
    
    // Generate a mock token
    const mockToken = `mock-jwt-token-for-${user.Email}-${Date.now()}`;
    console.log("Generated mock token:", mockToken);

    return Promise.resolve({ ...userDataToReturn, token: mockToken }); // Return user data and token
  } else {
    console.log("Mock login failed for:", email, "User found:", !!user, "Password match:", user ? user.PasswordHash === password : false);
    console.log("Current mockUsers list:", mockUsers);
    return Promise.reject(new Error('Invalid email or password.'));
  }
};

const logout = () => {
  // localStorage.removeItem("user"); // This is handled by AuthContext
  console.log("User logged out from mock service");
  // No specific action needed for mock logout in this service beyond what AuthContext does
};

const authService = {
  register,
  login,
  logout,
  // For debugging or testing, you might want to expose this, but not for production:
  // getMockUsers: () => mockUsers 
};

export default authService; 