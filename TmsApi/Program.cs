using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);
// Register authentication and authorization services
builder.Services.AddAuthentication("Training")
    .AddScheme<AuthenticationSchemeOptions, TrainingAuthHandler>("Training", null);

builder.Services.AddAuthorization();

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddProblemDetails();

var app = builder.Build();
// Registration Order
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseExceptionHandler(); // Placeholder for future modules

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();
app.MapGet("/api/assessments/results", () => Results.Ok(new
{
    courseCode = "CS-101",
    studentId = "S-001",
    letterGrade = "A"
}))
.RequireAuthorization(); 

app.Run();