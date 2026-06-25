using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TmsApi.Data;
using TmsApi.DTOs;
using TmsApi.Entities;


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

    [HttpGet("{id}")]
public async Task<IActionResult> GetStudent(int id)
{
    var student = await context.Students
        .FirstOrDefaultAsync(s => s.Id == id);

    if (student == null)
    {
        return NotFound();
    }

    return Ok(student);
}
// [HttpPut("{id}")]
public async Task<IActionResult> UpdateStudent(
    int id,
    UpdateStudentDto dto)
{
    var student = await context.Students
        .FirstOrDefaultAsync(s=>s.Id==id);


    if(student == null)
        return NotFound();


    student.Name = dto.Name;
    student.GPA = dto.GPA;


    try
    {
        await context.SaveChangesAsync();
    }
    catch(DbUpdateConcurrencyException)
    {
        return Conflict(
            "Student was modified by another user"
        );
    }


    return Ok(student);
}

[HttpPost("validation")]
public async Task<IActionResult> CreateStudent(CreateStudentDto dto)
{
    var student = new Student
    {
        RegistrationNumber = dto.RegistrationNumber,
        Name = dto.Name,
        GPA = dto.GPA,
        IsActive = dto.IsActive
    };


    context.Students.Add(student);


    try
    {
        await context.SaveChangesAsync();
    }
    catch(DbUpdateException ex)
    {
        return BadRequest(new
        {
            message = "RegistrationNumber must be maximum 10 characters",
            error = ex.InnerException?.Message
        });
    }


    return CreatedAtAction(
        nameof(GetStudent),
        new { id = student.Id },
        student
    );
}
[HttpGet("admin/all")]
public async Task<IActionResult> GetAllStudentsAdmin()
{
    var students = await context.Students
        .IgnoreQueryFilters()
        .ToListAsync();

    return Ok(students);
}

[HttpDelete("{id}")]
public async Task<IActionResult> DeleteStudent(int id)
{
    var student = await context.Students
        .IgnoreQueryFilters()
        .FirstOrDefaultAsync(s => s.Id == id);


    if(student == null)
    {
        return NotFound();
    }


    student.IsDeleted = true;


    await context.SaveChangesAsync();


    return NoContent();
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
