using MediatR;
using TmsApi.Common;
using TmsApi.Services; // Adjust to wherever your M6 IEnrollmentRepository/ICourseRepository live
using TmsApi.Entities; // Adjust to where your Domain Entities (Enrollment, Course) live
using TmsApi.Application.Interfaces; // <-- Ensure this is present
namespace TmsApi.Enrollments.Commands;

public class EnrollStudentHandler(
    IEnrollmentRepository enrollmentRepo,
    ICourseRepository courseRepo)
    : IRequestHandler<EnrollStudentCommand, Result<EnrollmentCreated, EnrollmentError>>
{
    public async Task<Result<EnrollmentCreated, EnrollmentError>> Handle(
        EnrollStudentCommand command, CancellationToken ct)
    {
        var course = await courseRepo.GetByCodeAsync(command.CourseCode, ct);
        if (course is null)
            return Result<EnrollmentCreated, EnrollmentError>.Failure(EnrollmentError.CourseNotFound(command.CourseCode));

        var alreadyEnrolled = await enrollmentRepo.ExistsAsync(command.StudentId, command.CourseCode, ct);
        if (alreadyEnrolled)
            return Result<EnrollmentCreated, EnrollmentError>.Failure(EnrollmentError.AlreadyEnrolled(command.StudentId, command.CourseCode));

        if (course.Enrollments.Count >= course.MaxCapacity)
            return Result<EnrollmentCreated, EnrollmentError>.Failure(EnrollmentError.CourseFull(course.Title, course.MaxCapacity));

        var enrollment = new Enrollment
        {
            StudentId = command.StudentId,
            CourseId = course.Id
        };

        await enrollmentRepo.AddAsync(enrollment, ct);

        return Result<EnrollmentCreated, EnrollmentError>.Success(
            new EnrollmentCreated(enrollment.Id, enrollment.StudentId, course.Code));
    }
}