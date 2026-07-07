// using Microsoft.AspNetCore.Mvc;
// using TmsApi.Entities;
// using TmsApi.Services;


// namespace TmsApi.Controllers;


// [ApiController]
// [Route("api/courses")]
// public class CoursesController(
//     ICourseService courseService
// ) : ControllerBase
// {


//     [HttpGet("{id:int}", Name = nameof(GetCourseById))]
//     public async Task<IActionResult> GetCourseById(
//         int id,
//         CancellationToken ct)
//     {

//         var course = await courseService.GetByIdAsync(
//             id,
//             ct
//         );


//         if(course == null)
//         {
//             return NotFound();
//         }


//         return Ok(course);
//     }




//     [HttpPost]
//     public async Task<IActionResult> CreateCourse(
//         Course course,
//         CancellationToken ct)
//     {

//         var result = await courseService.CreateAsync(
//             course,
//             ct
//         );


//         return CreatedAtAction(
//             nameof(GetCourseById),
//             new
//             {
//                 id = result.Id
//             },
//             result
//         );
//     }

// }

using Microsoft.AspNetCore.Mvc;
using TmsApi.Dtos;
using TmsApi.Services;
namespace TmsApi.Controllers;
[ApiController]
[Route("api/courses")]
public class CoursesController(ICourseService courseService) : ControllerBase
{
[HttpGet("{id:int}", Name = nameof(GetCourseById))]
public async Task<IActionResult> GetCourseById(int id, CancellationToken ct)
{
var course = await courseService.GetByIdAsync(id, ct);
return course is not null ? Ok(course) : NotFound();
}

[HttpGet]
public async Task<IActionResult> GetCourses(
    [FromQuery] PagedRequest request,
    CancellationToken ct)
{
    var result = await courseService.GetCoursesAsync(request, ct);

    return Ok(result);
}
// [HttpPost]
// public async Task<IActionResult> CreateCourse(CreateCourseRequest request, CancellationToken ct)
// {
// var result = await courseService.CreateAsync(request, ct);
// return CreatedAtAction(nameof(GetCourseById), new { id = result.Id }, result);
// }



[HttpPost]
    public async Task<IActionResult> CreateCourse(
        CreateCourseRequest request,
        CancellationToken ct)
    {
        // Check BEFORE hitting the database
        // Prevents 500 crash from unique index violation
        if (await courseService.CodeExistsAsync(request.Code, ct))
            return Conflict(new ProblemDetails
            {
                Title  = "Course code already exists",
                Detail = $"A course with code '{request.Code}' is already registered.",
                Status = StatusCodes.Status409Conflict
            });

        var result = await courseService.CreateAsync(request, ct);

        return CreatedAtAction(
            nameof(GetCourseById),
            new { id = result.Id },
            result);
    }

    
}