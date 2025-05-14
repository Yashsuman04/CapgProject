import React, { useState, useEffect } from 'react';
import courseService from '../../services/courseService';
import { useAuth } from '../../contexts/AuthContext';
import { Link } from 'react-router-dom'; // For navigation

const CourseList = ({ refreshTrigger }) => { // refreshTrigger to re-fetch when a course is created/updated
    const [courses, setCourses] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');
    const { user, isAuthenticated, hasRole } = useAuth();

    useEffect(() => {
        const fetchCourses = async () => {
            setLoading(true);
            setError('');
            try {
                const data = await courseService.getAllCourses();
                setCourses(data);
            } catch (err) {
                setError(err.message || 'Failed to fetch courses.');
            }
            setLoading(false);
        };
        fetchCourses();
    }, [refreshTrigger]);

    const handleDelete = async (courseId, courseTitle) => {
        if (window.confirm(`Are you sure you want to delete the course "${courseTitle}"? This action cannot be undone.`)) {
            try {
                await courseService.deleteCourse(courseId);
                setCourses(prevCourses => prevCourses.filter(course => course.courseId !== courseId));
                alert('Course deleted successfully.');
            } catch (err) {
                alert(`Failed to delete course: ${err.message || 'Unknown error'}`);
            }
        }
    };

    if (loading) return <p><span className="spinner-border spinner-border-sm"></span> Loading courses...</p>;
    if (error) return <div className="alert alert-danger">Error: {error}</div>;
    if (courses.length === 0) return <p>No courses available at the moment.</p>;

    return (
        <div>
            <h2>Available Courses</h2>
            <div className="row row-cols-1 row-cols-md-2 row-cols-lg-3 g-4">
                {courses.map(course => (
                    <div key={course.courseId} className="col">
                        <div className="card h-100">
                            <div className="card-body">
                                <h5 className="card-title">{course.title}</h5>
                                <h6 className="card-subtitle mb-2 text-muted">By: {course.instructorName || 'N/A'}</h6>
                                <p className="card-text">{course.description?.substring(0, 100)}{course.description && course.description.length > 100 ? '...' : ''}</p>
                                {/* Students might see an enroll button, instructors see manage buttons */}
                                {/* Example link to a detailed course view (to be created) */}
                                <Link to={`/courses/${course.courseId}`} className="btn btn-sm btn-outline-primary me-2">View Details</Link>
                                
                                {isAuthenticated && hasRole('Instructor') && user?.userId === course.instructorId && (
                                    <>
                                        {/* Link to an edit course page (to be created) */}
                                        <Link to={`/instructor/courses/edit/${course.courseId}`} className="btn btn-sm btn-outline-warning me-2">Edit</Link>
                                        <button 
                                            onClick={() => handleDelete(course.courseId, course.title)}
                                            className="btn btn-sm btn-outline-danger"
                                        >
                                            Delete
                                        </button>
                                    </>
                                )}
                            </div>
                            {course.mediaUrl && (
                                <div className="card-footer">
                                    <small className="text-muted"><a href={course.mediaUrl} target="_blank" rel="noopener noreferrer">View Media</a></small>
                                </div>
                            )}
                        </div>
                    </div>
                ))}
            </div>
        </div>
    );
};

export default CourseList; 