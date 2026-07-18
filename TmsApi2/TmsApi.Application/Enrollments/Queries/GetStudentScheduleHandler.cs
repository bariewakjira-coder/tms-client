using MediatR;
using TmsApi.Services;
using TmsApi.Application.Interfaces; // <-- Add this using directive
namespace TmsApi.Enrollments.Queries;

public class GetStudentScheduleHandler(IEnrollmentRepository enrollmentRepo)
    : IRequestHandler<GetStudentScheduleQuery, ScheduleDto>
{
    public async Task<ScheduleDto> Handle(GetStudentScheduleQuery query, CancellationToken ct)
    {
        var enrollments = await enrollmentRepo.GetByStudentIdAsync(query.StudentId, ct);

        var items = enrollments.Select(e => new ScheduleItemDto(
            e.Course.Code,
            e.Course.Title,
            "TBD"
        )).ToList();

        return new ScheduleDto(query.StudentId, items);
    }
}