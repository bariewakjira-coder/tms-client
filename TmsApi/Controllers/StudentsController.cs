using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TmsApi.Data;

namespace TmsApi.Controllers;

[ApiController]
[Route("api/students")]
public class StudentsController : ControllerBase
{
    private readonly TmsDbContext context;

    public StudentsController(TmsDbContext context)
    {
        this.context = context;
    }


    [HttpGet]
    public async Task<IActionResult> GetStudents(
        int page = 1,
        CancellationToken cancellationToken = default)
    {
        int pageSize = 5;


        var students = await context.Students
            .OrderBy(s => s.Name)   // IMPORTANT FIRST
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);


        return Ok(students);
    }

    [HttpGet("top-courses")]
public async Task<IActionResult> GetTopCourses()
{
    var courses = await context.Courses
        .Select(c => new
        {
            c.Title,
            EnrollmentCount = c.Enrollments.Count()
        })
        .OrderByDescending(x => x.EnrollmentCount)
        .Take(5)
        .ToListAsync();


    return Ok(courses);
}
}
