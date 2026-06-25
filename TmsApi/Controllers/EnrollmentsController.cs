using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TmsApi.Data;


[ApiController]
[Route("api/enrollments")]
public class EnrollmentsController(IEnrollmentService enrollmentService, TmsDbContext context) 
    : ControllerBase
{

    // GET /api/enrollments
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var enrollments = await enrollmentService.GetAllAsync();

        return Ok(enrollments);
    }



    // GET /api/enrollments/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var record = await enrollmentService.GetByIdAsync(id);

        return record is not null 
            ? Ok(record) 
            : NotFound();
    }



    // POST /api/enrollments
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateEnrollmentRequest request)
    {
        var record = await enrollmentService.EnrollAsync(
            request.StudentId, 
            request.CourseCode
        );

        return CreatedAtAction(
            nameof(GetById),
            new { id = record.Id },
            record
        );
    }



    public record CreateEnrollmentRequest(
        string StudentId, 
        string CourseCode
    );



    // DELETE /api/enrollments/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var deleted = await enrollmentService.DeleteAsync(id);

        return deleted 
            ? NoContent() 
            : NotFound();
    }



    // ==============================
    // Exercise 9
    // Bulk archive old enrollments
    // ==============================

    [HttpPut("archive-old")]
    public async Task<IActionResult> ArchiveOldEnrollments()
    {

        var cutoff = DateTime.UtcNow.AddYears(-1);


        var affectedRows = await context.Enrollments
            .Where(e => e.EnrolledAt < cutoff)
            .ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(
                        e => e.IsArchived,
                        true
                    )
            );


        return Ok(new
        {
            message = "Old enrollments archived",
            archivedCount = affectedRows
        });
    }
}