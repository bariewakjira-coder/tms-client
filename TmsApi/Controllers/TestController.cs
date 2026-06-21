using Microsoft.AspNetCore.Mvc;
using TmsApi.Data;

namespace TmsApi.Controllers;

[ApiController]
[Route("api/test")]
public class TestController(TmsDbContext context) : ControllerBase
{
    private static bool IsHonorRoll(decimal gpa)
    {
        return gpa >= 3.5m;
    }

    [HttpGet("deferred")]
    public IActionResult TestDeferred()
    {
        Console.WriteLine(
        "\n>>> STEP 1: Building query object");


        var query = context.Students
            .Where(s => s.GPA >= 3.0m);


        Console.WriteLine(
        ">>> STEP 2: Adding sorting");


        var orderedQuery = query
            .OrderBy(s => s.Name);


        Console.WriteLine(
        ">>> STEP 3: Convert to List");


        var results = orderedQuery.ToList();


        Console.WriteLine(
        ">>> STEP 4: Finished");


        return Ok(results);
    }
    [HttpGet("translation-fail")]
public IActionResult TestTranslationFail()
{
Console.WriteLine("\n>>> STEP 1: Running non-translatable query...");
try
{
var students = context.Students
.Where(s => IsHonorRoll(s.GPA)) // EF Core does not know how to map this method to SQL
.ToList();
return Ok(students);
}
catch (Exception ex)
{
Console.WriteLine($">>> EXCEPTION CAUGHT: {ex.Message}\n");return BadRequest(new { Message = ex.Message });
}
}
}