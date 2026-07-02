namespace TmsApi.Dtos;
public record EnrollmentResponseDto(
int Id,
int CourseId,
int StudentId,
DateTime EnrolledAt);