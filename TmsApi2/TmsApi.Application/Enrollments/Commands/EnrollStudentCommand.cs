using MediatR;
using TmsApi.Common;

namespace TmsApi.Enrollments.Commands;

public record EnrollStudentCommand(int StudentId, string CourseCode) 
    : IRequest<Result<EnrollmentCreated, EnrollmentError>>;

public record EnrollmentCreated(int EnrollmentId, int StudentId, string CourseCode);