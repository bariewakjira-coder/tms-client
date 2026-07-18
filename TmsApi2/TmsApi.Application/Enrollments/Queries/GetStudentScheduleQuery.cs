using MediatR;

namespace TmsApi.Enrollments.Queries;

public record GetStudentScheduleQuery(int StudentId) : IRequest<ScheduleDto>;

public record ScheduleDto(int StudentId, List<ScheduleItemDto> Courses);
public record ScheduleItemDto(string CourseCode, string Title, string Schedule);