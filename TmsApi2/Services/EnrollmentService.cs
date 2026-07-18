using Microsoft.EntityFrameworkCore;
using TmsApi.Data;
using TmsApi.Dtos;
using TmsApi.Entities;

namespace TmsApi.Services;

public class EnrollmentService(
    TmsDbContext context,
    ILogger<EnrollmentService> logger) : IEnrollmentService
{
    public Task<EnrollmentResponseDto?> GetByIdAsync(
        int courseId,
        int id,
        CancellationToken ct) =>
        context.Enrollments
            .AsNoTracking()
            .Where(e => e.Id == id && e.CourseId == courseId)
            .Select(e => new EnrollmentResponseDto(
                e.Id,
                e.CourseId,
                e.StudentId,
                e.EnrolledAt))
            .FirstOrDefaultAsync(ct);

    public async Task<IEnumerable<EnrollmentResponseDto>> GetAllAsync(
        int courseId,
        CancellationToken ct)
    {
        return await context.Enrollments
            .AsNoTracking()
            .Where(e => e.CourseId == courseId)
            .Select(e => new EnrollmentResponseDto(
                e.Id,
                e.CourseId,
                e.StudentId,
                e.EnrolledAt))
            .ToListAsync(ct);
    }

    public async Task<EnrollmentResponseDto> CreateAsync(
        int courseId,
        EnrollStudentRequest request,
        CancellationToken ct)
    {
        var course = await context.Courses
            .FirstOrDefaultAsync(c => c.Id == courseId, ct);

        if (course == null)
            throw new KeyNotFoundException($"Course {courseId} was not found.");

        var alreadyEnrolled = await context.Enrollments
            .AnyAsync(e => e.CourseId == courseId && e.StudentId == request.StudentId, ct);

        if (alreadyEnrolled)
            throw new InvalidOperationException(
                $"Student {request.StudentId} is already enrolled in course {courseId}.");

        var enrolledCount = await context.Enrollments
            .CountAsync(e => e.CourseId == courseId, ct);

        if (enrolledCount >= course.MaxCapacity)
            throw new InvalidOperationException(
                $"Course {courseId} has reached its maximum capacity of {course.MaxCapacity}.");

        var enrollment = new Enrollment
        {
            CourseId = courseId,
            StudentId = request.StudentId,
            EnrolledAt = DateTime.UtcNow
        };

        context.Enrollments.Add(enrollment);
        await context.SaveChangesAsync(ct);

        logger.LogInformation(
            "Enrolled student {StudentId} in course {CourseId} — enrollment {EnrollmentId}",
            enrollment.StudentId,
            enrollment.CourseId,
            enrollment.Id);

        return (await GetByIdAsync(courseId, enrollment.Id, ct))!;
    }
}