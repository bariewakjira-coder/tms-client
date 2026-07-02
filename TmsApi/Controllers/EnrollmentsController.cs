// using Microsoft.AspNetCore.Mvc;
// using Microsoft.EntityFrameworkCore;
// using TmsApi.Data;


// [ApiController]
// [Route("api/enrollments")]
// public class EnrollmentsController(IEnrollmentService enrollmentService, TmsDbContext context) 
//     : ControllerBase
// {

//     // GET /api/enrollments
//     [HttpGet]
//     public async Task<IActionResult> GetAll()
//     {
//         var enrollments = await enrollmentService.GetAllAsync();

//         return Ok(enrollments);
//     }



//     // GET /api/enrollments/{id}
//     [HttpGet("{id}")]
//     public async Task<IActionResult> GetById(string id)
//     {
//         var record = await enrollmentService.GetByIdAsync(id);

//         return record is not null 
//             ? Ok(record) 
//             : NotFound();
//     }



//     // POST /api/enrollments
//     [HttpPost]
//     public async Task<IActionResult> Create(
//         [FromBody] CreateEnrollmentRequest request)
//     {
//         var record = await enrollmentService.EnrollAsync(
//             request.StudentId, 
//             request.CourseCode
//         );

//         return CreatedAtAction(
//             nameof(GetById),
//             new { id = record.Id },
//             record
//         );
//     }



//     public record CreateEnrollmentRequest(
//         string StudentId, 
//         string CourseCode
//     );



//     // DELETE /api/enrollments/{id}
//     [HttpDelete("{id}")]
//     public async Task<IActionResult> Delete(string id)
//     {
//         var deleted = await enrollmentService.DeleteAsync(id);

//         return deleted 
            // ? NoContent() 
//             : NotFound();
//     }



//     // ==============================
//     // Exercise 9
//     // Bulk archive old enrollments
//     // ==============================

//     [HttpPut("archive-old")]
//     public async Task<IActionResult> ArchiveOldEnrollments()
//     {

//         var cutoff = DateTime.UtcNow.AddYears(-1);


//         var affectedRows = await context.Enrollments
//             .Where(e => e.EnrolledAt < cutoff)
//             .ExecuteUpdateAsync(
//                 setters => setters
//                     .SetProperty(
//                         e => e.IsArchived,
//                         true
//                     )
//             );


//         return Ok(new
//         {
//             message = "Old enrollments archived",
//             archivedCount = affectedRows
//         });
//     }
// }


using Microsoft.AspNetCore.Mvc;
using TmsApi.Dtos;
using TmsApi.Services;

namespace TmsApi.Controllers;

[ApiController]
[Route("api/courses/{courseId:int}/enrollments")]
public class EnrollmentsController(
    ICourseService courseService,
    IEnrollmentService enrollmentService) : ControllerBase
{
    // GET /api/courses/5/enrollments/10
    [HttpGet("{id:int}", Name = nameof(GetEnrollment))]
    public async Task<IActionResult> GetEnrollment(
        int courseId,
        int id,
        CancellationToken ct)
    {
        var enrollment = await enrollmentService.GetByIdAsync(courseId, id, ct);

        return enrollment is not null
            ? Ok(enrollment)
            : NotFound();
    }

    // POST /api/courses/5/enrollments
    [HttpPost]
    public async Task<IActionResult> EnrollStudent(
        int courseId,
        EnrollStudentRequest request,
        CancellationToken ct)
    {
        // Gate 1: Does the course exist?
        // 404 before 409 — no point checking capacity
        // if the course was never there
        var course = await courseService.GetByIdAsync(courseId, ct);

        if (course == null)
            return NotFound();

        // Gate 2: Is the course full?
        if (course.EnrollmentCount >= course.MaxCapacity)
            return Conflict(new ProblemDetails
            {
                Title  = "Course is full",
                Detail = $"Course '{course.Title}' has reached its maximum capacity of {course.MaxCapacity}.",
                Status = StatusCodes.Status409Conflict
            });

        // All gates passed — create enrollment
        var enrollment = await enrollmentService.CreateAsync(courseId, request, ct);

        return CreatedAtAction(
            nameof(GetEnrollment),
            new { courseId, id = enrollment.Id },
            enrollment);
    }
}