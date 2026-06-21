using Microsoft.AspNetCore.Authentication;
using Scalar.AspNetCore;
using Microsoft.EntityFrameworkCore;
using TmsApi.Data;
using TmsApi.Entities;

var builder = WebApplication.CreateBuilder(args);
// Register authentication and authorization services
builder.Services.AddAuthentication("Training")
    .AddScheme<AuthenticationSchemeOptions, TrainingAuthHandler>("Training", null);
builder.Services.AddDbContext<TmsDbContext>(options =>
options.UseNpgsql(builder.Configuration.GetConnectionString("TmsDatabase"))
.LogTo(Console.WriteLine, LogLevel.Information) // Log SQLto output window
.EnableSensitiveDataLogging()); // Show parameters in querylogs (dev only)

builder.Services.AddAuthorization();

// Add services to the container.


builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();
// --- Exercise 2 Registrations ---
// builder.Services.AddSingleton<EnrollmentWorker>();
// builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();
// --- Exercise 2 Registrations ---
builder.Services.AddSingleton<EnrollmentWorker>(); 
builder.Services.AddSingleton<IEnrollmentService, EnrollmentService>();


// --- Strict Host Validation ---
builder.Host.UseDefaultServiceProvider(options =>
{
    options.ValidateScopes = true;
    options.ValidateOnBuild = true;
});
// --- Exercise 3: Options Pattern Validation ---
builder.Services.AddOptions<PaymentOptions>()
    .BindConfiguration("Payments")
    .ValidateDataAnnotations()
    .ValidateOnStart();


var app = builder.Build();
// 1
// builder.Services.AddProblemDetails();

// // Registration Order
// app.UseExceptionHandler(); // Placeholder for future modules
// builder.Services.AddOpenApi();

// //2
// app.UseStatusCodePages(); 

// TODO 1: Check if the app is running in Development mode
if (app.Environment.IsDevelopment())
{
    app.UseExceptionHandler();
    app.UseStatusCodePages();

    // TODO 2: Expose documentation tools in DEV ONLY
    app.MapOpenApi();              
    app.MapScalarApiReference();   
}
else
{
    // TODO 3: In Production, hide docs but mask errors cleanly
    app.UseExceptionHandler();     
    app.UseStatusCodePages();      
}
app.UseMiddleware<RequestLoggingMiddleware>();


// Configure the HTTP request pipeline.

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();
// app.MapGet("/api/assessments/results", () => Results.Ok(new
// {
//     courseCode = "CS-101",
//     studentId = "S-001",
//     letterGrade = "A"
// }))
// .RequireAuthorization(); 
// app.MapGet("/api/enrollments/worker-smoke", (EnrollmentWorker worker) =>
// {
//     worker.ProcessBatch();
//     return Results.Ok("processed");
// });

// app.MapGet("/api/error", () =>
// {
//     throw new TmsDatabaseException("Simulated database failure for ProblemDetails testing");
// });


using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider
        .GetRequiredService<TmsDbContext>();

    context.Database.Migrate();

    if (!context.Students.Any())
    {
        var students = new List<Student>
        {
            new()
            {
                RegistrationNumber = "TMS-2026-0001",
                Name = "Alice Smith",
                GPA = 3.8m,
                IsActive = true
            },

            new()
            {
                RegistrationNumber = "TMS-2026-0002",
                Name = "Bob Jones",
                GPA = 2.9m,
                IsActive = true
            },

            new()
            {
                RegistrationNumber = "TMS-2026-0003",
                Name = "Charlie Brown",
                GPA = 3.4m,
                IsActive = false
            },

            new()
            {
                RegistrationNumber = "TMS-2026-0004",
                Name = "Diana Prince",
                GPA = 3.9m,
                IsActive = true
            },

            new()
            {
                RegistrationNumber = "TMS-2026-0005",
                Name = "Evan Wright",
                GPA = 2.5m,
                IsActive = true
            }
        };


        context.Students.AddRange(students);


        var courses = new List<Course>
        {
            new()
            {
                Code = "CS-101",
                Title = "Introduction to Computer Science",
                Capacity = 30
            },

            new()
            {
                Code = "CS-201",
                Title = "Data Structures and Algorithms",
                Capacity = 25
            },

            new()
            {
                Code = "MAT-101",
                Title = "Calculus I",
                Capacity = 40
            }
        };


        context.Courses.AddRange(courses);

        context.SaveChanges();


        var enrollments = new List<Enrollment>
        {
            new()
            {
                StudentId = students[0].Id,
                CourseId = courses[0].Id,
                Grade = 4.0m
            },

            new()
            {
                StudentId = students[0].Id,
                CourseId = courses[1].Id,
                Grade = 3.6m
            },

            new()
            {
                StudentId = students[1].Id,
                CourseId = courses[0].Id,
                Grade = 2.8m
            },

            new()
            {
                StudentId = students[3].Id,
                CourseId = courses[1].Id,
                Grade = 3.9m
            }
        };


        context.Enrollments.AddRange(enrollments);

        context.SaveChanges();
    }
}

app.Run();