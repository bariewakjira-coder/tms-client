using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TmsApi.Data;

namespace TmsApi.Controllers;

[ApiController]
[Route("api/reports")]
public class ReportsController(TmsDbContext db) : ControllerBase
{

    // 1. Count active students with GPA >= 3.0
    [HttpGet("active-students")]
    public async Task<IActionResult> GetActiveStudents()
    {
        var count = await db.Students
            .Where(s => s.IsActive && s.GPA >= 3.0m)
            .CountAsync();

        return Ok(count);
    }



    // 2. Courses with the most enrollments
    [HttpGet("popular-courses")]
    public async Task<IActionResult> GetPopularCourses()
    {
        var list = await db.Courses
            .Select(c => new
            {
                c.Title,
                EnrollmentCount = c.Enrollments.Count
            })
            .OrderByDescending(x => x.EnrollmentCount)
            .ToListAsync();

        return Ok(list);
    }



    // 3. Average GPA per course
    [HttpGet("average-gpa")]
    public async Task<IActionResult> GetAverageGpa()
    {
        var list = await db.Enrollments
            .GroupBy(e => e.Course.Title)
            .Select(g => new
            {
                Course = g.Key,
                AverageGPA = g.Average(e => e.Student.GPA)
            })
            .ToListAsync();

        return Ok(list);
    }



    // 4A. Students with zero enrollments (NOT EXISTS)
    [HttpGet("no-enrollments")]
    public async Task<IActionResult> GetStudentsWithoutEnrollments()
    {
        var list = await db.Students
            .Where(s => !s.Enrollments.Any())
            .Select(s => s.Name)
            .ToListAsync();

        return Ok(list);
    }


// Exercise 7 Part A: Intentional N+1 query
[HttpGet("n-plus-one")]
public async Task<IActionResult> NPlusOne(
    CancellationToken cancellationToken)
{
   var students = await db.Students
    .AsNoTracking()
    .ToListAsync(cancellationToken);


foreach (var s in students)
{
    // Query enrollment count for this student
    // This creates 1 + N SQL queries

    var count = await db.Enrollments
        .AsNoTracking()
        .CountAsync(
            e => e.StudentId == s.Id,
            cancellationToken
        );


    Console.WriteLine(
        $"{s.Name}: {count} enrollments"
    );
}
    
    return Ok(students);
}



// Exercise 7 Part B: Fixed shaped query
[HttpGet("student-enrollment-report")]
public async Task<IActionResult> StudentEnrollmentReport(
    CancellationToken cancellationToken)
{
   var report = await db.Students
    .AsNoTracking()
    .Select(s => new
    {
        s.Name,

        EnrollmentCount = s.Enrollments.Count
    })
    .ToListAsync(cancellationToken);



foreach (var r in report)
{
    Console.WriteLine(
        $"{r.Name}: {r.EnrollmentCount} enrollments"
    );
}

    return Ok(report);
}
}

