import React, { useState } from 'react';
import courseService from '../../services/courseService';
// import { useNavigate } from 'react-router-dom'; // If you want to redirect after creation

const CreateCourseForm = ({ onCourseCreated }) => {
    const [title, setTitle] = useState('');
    const [description, setDescription] = useState('');
    const [mediaUrl, setMediaUrl] = useState(''); // Optional
    const [error, setError] = useState('');
    const [success, setSuccess] = useState('');
    const [loading, setLoading] = useState(false);
    // const navigate = useNavigate();

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError('');
        setSuccess('');
        setLoading(true);

        const courseData = {
            title,
            description,
            mediaUrl: mediaUrl || null, // Send null if empty, or backend handles empty string
        };

        try {
            const newCourse = await courseService.createCourse(courseData);
            setSuccess(`Course "${newCourse.title}" created successfully!`);
            setTitle('');
            setDescription('');
            setMediaUrl('');
            if (onCourseCreated) {
                onCourseCreated(newCourse); // Callback to update parent component's list
            }
            // navigate('/instructor/dashboard'); // Or to the new course details page
        } catch (err) {
            setError(err.message || 'Failed to create course. Please try again.');
        }
        setLoading(false);
    };

    return (
        <div className="card">
            <div className="card-body">
                <h3 className="card-title">Create New Course</h3>
                <form onSubmit={handleSubmit}>
                    {error && <div className="alert alert-danger">{error}</div>}
                    {success && <div className="alert alert-success">{success}</div>}

                    <div className="mb-3">
                        <label htmlFor="courseTitle" className="form-label">Title</label>
                        <input
                            type="text"
                            className="form-control"
                            id="courseTitle"
                            value={title}
                            onChange={(e) => setTitle(e.target.value)}
                            required
                            minLength="3"
                        />
                    </div>

                    <div className="mb-3">
                        <label htmlFor="courseDescription" className="form-label">Description</label>
                        <textarea
                            className="form-control"
                            id="courseDescription"
                            rows="3"
                            value={description}
                            onChange={(e) => setDescription(e.target.value)}
                        ></textarea>
                    </div>

                    <div className="mb-3">
                        <label htmlFor="courseMediaUrl" className="form-label">Media URL (Optional)</label>
                        <input
                            type="url"
                            className="form-control"
                            id="courseMediaUrl"
                            value={mediaUrl}
                            onChange={(e) => setMediaUrl(e.target.value)}
                            placeholder="https://example.com/video_or_document.mp4"
                        />
                    </div>

                    <button type="submit" className="btn btn-primary" disabled={loading}>
                        {loading ? (
                            <>
                                <span className="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Creating...
                            </>
                        ) : 'Create Course'}
                    </button>
                </form>
            </div>
        </div>
    );
};

export default CreateCourseForm; 