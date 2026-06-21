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
            },
            new()
            {
                RegistrationNumber = "TMS-2026-0006",
                Name = "Joe Doe",
                GPA = 4.00m,
                IsActive = true
            },
            new()
            {
                RegistrationNumber = "TMS-2026-0007",
                Name = "John Stones",
                GPA = 3.5m,
                IsActive = false
            },
            new()
            {
                RegistrationNumber = "TMS-2026-0008",
                Name = "Kayl walker",
                GPA = 1.5m,
                IsActive = true
            },
            new()
            {
                RegistrationNumber = "TMS-2026-0009",
                Name = "Tewodros Abiyu",
                GPA = 3.5m,
                IsActive = true
            },
            new()
            {
                RegistrationNumber = "TMS-2026-0010",
                Name = "Mesfin Abeje",
                GPA = 3.3m,
                IsActive = true
            },
            new()
            {
                RegistrationNumber = "TMS-2026-0011",
                Name = "Eden Mogos",
                GPA = 3.4m,
                IsActive = false
            },
            new()
            {
                RegistrationNumber = "TMS-2026-0012",
                Name = "Yasin Tahir",
                GPA = 3.6m,
                IsActive = true
            },
            new()
            {
                RegistrationNumber = "TMS-2026-0013",
                Name = "Muluken Showa",
                GPA = 3.7m,
                IsActive = false
            },
            new()
            {
                RegistrationNumber = "TMS-2026-0014",
                Name = "Azmeraw Tefera",
                GPA = 3.7m,
                IsActive = true
            },
            new()
            {
                RegistrationNumber = "TMS-2026-0015",
                Name = "Tesema Melaku",
                GPA = 2.9m,
                IsActive = false
            },
            new()
            {
                RegistrationNumber = "TMS-2026-0016",
                Name = "Demelash Ayele",
                GPA = 3.1m,
                IsActive = true
            },
            new()
            {
                RegistrationNumber = "TMS-2026-0017",
                Name = "Leyikun Mekonin",
                GPA = 2.5m,
                IsActive = false
            },
            new()
            {
                RegistrationNumber = "TMS-2026-0018",
                Name = "John Smith",
                GPA = 3.4m,
                IsActive = true
            },
            new()
            {
                RegistrationNumber = "TMS-2026-0019",
                Name = "John Stones",
                GPA = 3.3m,
                IsActive = false
            },
            new()
            {
                RegistrationNumber = "TMS-2026-0020",
                Name = "Look Shaw",
                GPA = 2.2m,
                IsActive = true
            },
            new()
            {
                RegistrationNumber = "TMS-2026-0021",
                Name = "Dagim Sisay",
                GPA = 3.63m,
                IsActive = false
            },
            new()
            {
                RegistrationNumber = "TMS-2026-0022",
                Name = "Elsa Berihun",
                GPA = 3.73m,
                IsActive = true
            },
            new()
            {
                RegistrationNumber = "TMS-2026-0023",
                Name = "Eliyas Tamiru",
                GPA = 3.83m,
                IsActive = false
            },
            new()
            {
                RegistrationNumber = "TMS-2026-0024",
                Name = "Yonatan Ejigu",
                GPA = 3.8m,
                IsActive = true
            },
            new()
            {
                RegistrationNumber = "TMS-2026-0025",
                Name = "Estifanos Sisay",
                GPA = 2.4m,
                IsActive = false
            },

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
            },
            new()
            {
                Code = "MAT-102",
                Title = "Calculus II",
                Capacity = 45
            },
            new()
            {
                Code = "MAT-103",
                Title = "Calculus III",
                Capacity = 30
            },
            new()
            {
                Code = "THERMO-104",
                Title = "Thermodynamics",
                Capacity = 23
            },
            new()
            {
                Code = "Embeded-105",
                Title = "Embeded Systems",
                Capacity = 25
            },
            new()
            {
                Code = "MAT-106",
                Title = "Calculus I",
                Capacity = 40
            },
            new()
            {
                Code = "Computer-107",
                Title = "Computer Network-security",
                Capacity = 45
            },
            new()
            {
                Code = "C#-108",
                Title = "C# Programming",
                Capacity = 34
            },
            new()
            {
                Code = "Java-109",
                Title = "Java Programing",
                Capacity = 42
            },
            new()
            {
                Code = "typescript-110",
                Title = "Typescript Programming",
                Capacity = 28
            },
            new()
            {
                Code = "Data Structure-111",
                Title = "Data Structure and Algorithms",
                Capacity = 40
            },
            new()
            {
                Code = "SQL-112",
                Title = "Structured Quesry Languages",
                Capacity = 58
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