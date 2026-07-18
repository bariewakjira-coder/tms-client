
// using Microsoft.AspNetCore.Mvc;
// using TmsApi.Dtos;
// using TmsApi.Services;

// namespace TmsApi.Controllers;

// [ApiController]
// [Route("api/courses/{courseId:int}/enrollments")]
// public class EnrollmentsController(
//     ICourseService courseService,
//     IEnrollmentService enrollmentService) : ControllerBase
// {

//     // GET /api/courses/5/enrollments/10
//     [HttpGet("{id:int}", Name = nameof(GetEnrollment))]
//     public async Task<IActionResult> GetEnrollment(
//         int courseId,
//         int id,
//         CancellationToken ct)
//     {
//         var enrollment = await enrollmentService.GetByIdAsync(courseId, id, ct);

//         return enrollment is not null
//             ? Ok(enrollment)
//             : NotFound();
//     }
// // GET /api/courses/5/enrollments
// [HttpGet]
// public async Task<IActionResult> GetEnrollments(
//     int courseId,
//     CancellationToken ct)
// {
//     var enrollments = await enrollmentService.GetAllAsync(courseId, ct);

//     return Ok(enrollments);
// }
//     // POST /api/courses/5/enrollments
//     [HttpPost]
//     public async Task<IActionResult> EnrollStudent(
//         int courseId,
//         EnrollStudentRequest request,
//         CancellationToken ct)
//     {
//         // Gate 1: Does the course exist?
//         // 404 before 409 — no point checking capacity
//         // if the course was never there
//         var course = await courseService.GetByIdAsync(courseId, ct);

//         if (course == null)
//             return NotFound();

//         // Gate 2: Is the course full?
//         if (course.EnrollmentCount >= course.MaxCapacity)
//             return Conflict(new ProblemDetails
//             {
//                 Title  = "Course is full",
//                 Detail = $"Course '{course.Title}' has reached its maximum capacity of {course.MaxCapacity}.",
//                 Status = StatusCodes.Status409Conflict
//             });

//         // All gates passed — create enrollment
//         var enrollment = await enrollmentService.CreateAsync(courseId, request, ct);

//         return CreatedAtAction(
//             nameof(GetEnrollment),
//             new { courseId, id = enrollment.Id },
//             enrollment);
//     }
// }
using Microsoft.AspNetCore.Mvc;
using TmsApi.Dtos;
using TmsApi.Services;

namespace TmsApi.Controllers;

[ApiController]
[Route("api/courses/{courseId:int}/enrollments")]
[Tags("Enrollments")]
[Produces("application/json")]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
public class EnrollmentsController(
    ICourseService courseService,
    IEnrollmentService enrollmentService) : ControllerBase
{

    // GET /api/courses/5/enrollments
    [HttpGet(Name = "ListCourseEnrollments")]
    [ProducesResponseType(typeof(IReadOnlyList<EnrollmentResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [EndpointSummary("List enrolments for a course")]
    public async Task<IActionResult> GetEnrollments(
        int courseId,
        CancellationToken ct)
    {
        var enrollments = await enrollmentService.GetAllAsync(courseId, ct);

        return Ok(enrollments);
    }


    // GET /api/courses/5/enrollments/10
    [HttpGet("{id:int}", Name = nameof(GetEnrollment))]
    [ProducesResponseType(typeof(EnrollmentResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [EndpointSummary("Get one enrolment for a course")]
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
    [ProducesResponseType(typeof(EnrollmentResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [EndpointSummary("Enrol a student in a course")]
    [EndpointDescription("Returns 404 if the course does not exist, 409 if the course has reached MaxCapacity.")]
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
        {
            return Conflict(new ProblemDetails
            {
                Title = "Course is full",
                Detail = $"Course '{course.Title}' has reached its maximum capacity of {course.MaxCapacity}.",
                Status = StatusCodes.Status409Conflict
            });
        }


        // All gates passed — create enrollment
        var enrollment = await enrollmentService.CreateAsync(courseId, request, ct);

        return CreatedAtAction(
            nameof(GetEnrollment),
            new { courseId, id = enrollment.Id },
            enrollment);
    }
}