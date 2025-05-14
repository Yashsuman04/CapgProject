import axios from 'axios';

const API_URL = process.env.REACT_APP_API_URL || 'https://localhost:7001/api'; // Adjust if your API runs elsewhere

// Helper to get the auth token from localStorage
const getAuthToken = () => {
    const tokenData = localStorage.getItem('authToken');
    return tokenData; // The token itself is stored directly
};

// Helper to create an Axios instance with Authorization header
const getAuthInstance = () => {
    const token = getAuthToken();
    return axios.create({
        baseURL: API_URL,
        headers: { 'Authorization': `Bearer ${token}` }
    });
};

// Get all courses
const getAllCourses = async () => {
    try {
        // Public endpoint, so no auth needed, or use a general instance
        const response = await axios.get(`${API_URL}/courses`);
        return response.data;
    } catch (error) {
        console.error("Error fetching courses:", error.response?.data || error.message);
        throw error.response?.data || error;
    }
};

// Get a single course by ID
const getCourseById = async (courseId) => {
    try {
        const response = await axios.get(`${API_URL}/courses/${courseId}`);
        return response.data;
    } catch (error) {
        console.error(`Error fetching course ${courseId}:`, error.response?.data || error.message);
        throw error.response?.data || error;
    }
};

// Create a new course (Instructor only)
const createCourse = async (courseData) => {
    try {
        const authAxios = getAuthInstance();
        const response = await authAxios.post('/courses', courseData);
        return response.data;
    } catch (error) {
        console.error("Error creating course:", error.response?.data || error.message);
        throw error.response?.data || error;
    }
};

// Update an existing course (Instructor only, owns the course)
const updateCourse = async (courseId, courseData) => {
    try {
        const authAxios = getAuthInstance();
        const response = await authAxios.put(`/courses/${courseId}`, courseData);
        return response.data; // Or handle NoContent response
    } catch (error) {
        console.error(`Error updating course ${courseId}:`, error.response?.data || error.message);
        throw error.response?.data || error;
    }
};

// Delete a course (Instructor only, owns the course)
const deleteCourse = async (courseId) => {
    try {
        const authAxios = getAuthInstance();
        await authAxios.delete(`/courses/${courseId}`);
        // No content is returned on successful deletion
    } catch (error) {
        console.error(`Error deleting course ${courseId}:`, error.response?.data || error.message);
        throw error.response?.data || error;
    }
};

const courseService = {
    getAllCourses,
    getCourseById,
    createCourse,
    updateCourse,
    deleteCourse,
};

export default courseService; 