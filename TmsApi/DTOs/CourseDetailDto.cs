using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components.Web;

namespace TmsApi.Dtos;
public record CourseDetailDto
{
    public required int Id {get;init;}
    public required string Code {get;init;}
    public required string Title {get;init;}
    public required int MaxCapacity {get;init;}
    public required int EnrollmentCount {get;init;}
    public required IReadOnlyList<LinkDto> Links {get;init;}
}