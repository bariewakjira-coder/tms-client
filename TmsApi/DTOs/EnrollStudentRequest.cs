using System.ComponentModel.DataAnnotations;
namespace TmsApi.Dtos;
public record EnrollStudentRequest
{
[Range(1, int.MaxValue, ErrorMessage = "StudentId must be a positive integer.")]
public required int StudentId { get; init; }
}