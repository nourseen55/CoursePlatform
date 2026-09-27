using CoursePlatform.Application.DTOs.Enrollments;
using CoursePlatform.Application.Interfaces.Services;
using CoursePlatform.Application.Interfaces.UnitOfWork;
using CoursePlatform.Domain.Entities;
using CoursePlatform.Domain.Enums;

namespace CoursePlatform.Application.Services
{

    public class EnrollmentService(
    IUnitOfWork unitOfWork) : IEnrollmentService
    {
        public async Task<(
            EnrollmentActionResult result,
            EnrollmentResponse? data)>
            EnrollAsync(
                int courseId,
                string studentId)
        {
            var course = await unitOfWork
                .Repository<Course>()
                .GetByIdAsync(courseId);

            if (course is null)
                return (
                    EnrollmentActionResult.CourseNotFound,
                    null);

            if (course.Status != CourseStatus.Published)
                return (
                    EnrollmentActionResult.NotPublished,
                    null);

            var existingEnrollment =
                 unitOfWork.Repository<Enrollment>()
                    .GetAllQuerable().FirstOrDefault(e => e.CourseId == courseId && e.StudentId == studentId);

            if (existingEnrollment is not null)
                return (
                    EnrollmentActionResult.AlreadyEnrolled,
                    null);

            var enrollment = new Enrollment
            {
                StudentId = studentId,
                CourseId = courseId,
                Status = EnrollmentStatus.Active,
                EnrolledAt = DateTime.UtcNow
            };

            await unitOfWork.Repository<Enrollment>().AddAsync(enrollment);

            await unitOfWork.SaveChangesAsync();

            var response = new EnrollmentResponse
            {
                Id = enrollment.Id,
                CourseId = course.Id,
                CourseTitle = course.Title,
                Status = enrollment.Status.ToString(),
                EnrolledAt = enrollment.EnrolledAt
            };

            return (
                EnrollmentActionResult.Success,
                response);
        }
        public async Task<IReadOnlyList<EnrollmentResponse>>  GetMyEnrollmentsAsync(string studentId)
        {
            var enrollments = await unitOfWork
                .Repository<Enrollment>()
                .GetAllAsync(
                    e => e.StudentId == studentId,
                    e => e.Course);

            return enrollments
                .OrderByDescending(e => e.EnrolledAt)
                .Select(e => new EnrollmentResponse
                {
                    Id = e.Id,
                    CourseId = e.CourseId,
                    CourseTitle = e.Course.Title,
                    Status = e.Status.ToString(),
                    EnrolledAt = e.EnrolledAt
                })
                .ToList();
        }
    }
}