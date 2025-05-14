import React, { useState } from 'react';
import CreateCourseForm from '../Courses/CreateCourseForm';
import CourseList from '../Courses/CourseList'; // We can reuse this, it filters by instructor implicitly for edit/delete
import { useAuth } from '../../contexts/AuthContext';

const InstructorDashboard = () => {
    const { user } = useAuth();
    const [refreshKey, setRefreshKey] = useState(0); // Used to trigger a refresh in CourseList

    const handleCourseCreated = (newCourse) => {
        console.log("New course created in dashboard:", newCourse);
        setRefreshKey(prevKey => prevKey + 1); // Increment key to trigger re-fetch in CourseList
    };

    return (
        <div className="container mt-4">
            <div className="row mb-4">
                <div className="col">
                    <h2>Instructor Dashboard</h2>
                    <p>Welcome, {user?.Name}! Manage your courses and content here.</p>
                </div>
            </div>

            <div className="row">
                <div className="col-md-4 mb-4">
                    <CreateCourseForm onCourseCreated={handleCourseCreated} />
                </div>
                <div className="col-md-8">
                    <h4>Your Courses</h4>
                    <CourseList refreshTrigger={refreshKey} /> 
                    {/* CourseList will show all courses, but edit/delete buttons only appear for courses owned by the logged-in instructor */}
                </div>
            </div>
            {/* Future: Add sections for managing assessments, viewing student progress etc. */}
        </div>
    );
};

export default InstructorDashboard; 