using CoursePlatform.Application.DTOs.Enrollments;
using CoursePlatform.Domain.Enums;

namespace CoursePlatform.Application.Interfaces.Services
{
    public interface IEnrollmentService
    {
        Task<(EnrollmentActionResult result, EnrollmentResponse? data)>
            EnrollAsync(
                int courseId,
                string studentId);
        Task<IReadOnlyList<EnrollmentResponse>> GetMyEnrollmentsAsync(string studentId);
    }
}
