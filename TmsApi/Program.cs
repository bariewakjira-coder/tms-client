using Microsoft.AspNetCore.Authentication;
using Scalar.AspNetCore;
using Microsoft.EntityFrameworkCore;
using TmsApi.Data;
using TmsApi.Entities;
using TmsApi.Services;
using TmsApi.Filters;

var builder = WebApplication.CreateBuilder(args);

// ═══════════════════════════════════════════════
// AUTHENTICATION & AUTHORIZATION
// ═══════════════════════════════════════════════
builder.Services.AddAuthentication("Training")
    .AddScheme<AuthenticationSchemeOptions, TrainingAuthHandler>("Training", null);

  

builder.Services.AddAuthorization();

// ═══════════════════════════════════════════════
// DATABASE
// ═══════════════════════════════════════════════
builder.Services.AddDbContext<TmsDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("TmsDatabase"))
           .LogTo(Console.WriteLine, LogLevel.Information)
           .EnableSensitiveDataLogging());

// ═══════════════════════════════════════════════
// CORE SERVICES (M4/M5 baseline — required by M6)
// ═══════════════════════════════════════════════
builder.Services.AddControllers();
builder.Services.AddControllers(options =>
{
    options.Filters.Add<AuditLogFilter>();
});
builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();

// ═══════════════════════════════════════════════
// APPLICATION SERVICES
// ✅ FIXED: EnrollmentService must be Scoped
// because it depends on DbContext (which is Scoped)
// Singleton holding a Scoped service crashes with
// ValidateScopes = true
// ═══════════════════════════════════════════════
builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();
builder.Services.AddSingleton<EnrollmentWorker>();

// ═══════════════════════════════════════════════
// STRICT LIFETIME VALIDATION
// Catches scope mismatches at startup not at runtime
// ═══════════════════════════════════════════════
builder.Host.UseDefaultServiceProvider(options =>
{
    options.ValidateScopes = true;
    options.ValidateOnBuild = true;
});

// ═══════════════════════════════════════════════
// BUILD
// ═══════════════════════════════════════════════
var app = builder.Build();

// ═══════════════════════════════════════════════
// MIDDLEWARE PIPELINE
// Order matters — ExceptionHandler must be FIRST
// so it can catch errors from everything below it
// ═══════════════════════════════════════════════

// Catches unhandled exceptions → clean 500 ProblemDetails
// (not raw HTML stack trace)
app.UseExceptionHandler();

// Fills empty error responses with ProblemDetails body
// e.g. 404 with no body becomes proper JSON
app.UseStatusCodePages();

if (app.Environment.IsDevelopment())
{
    // API documentation — dev only
    app.MapOpenApi();
    app.MapScalarApiReference();
}

// Custom middleware — logs every request
app.UseMiddleware<RequestLoggingMiddleware>();

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Terminal middleware — registers all controller routes
app.MapControllers();

// ═══════════════════════════════════════════════
// DATABASE SEED
// Runs once on startup if tables are empty
// ═══════════════════════════════════════════════
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider
        .GetRequiredService<TmsDbContext>();

    context.Database.Migrate();

    if (!context.Students.Any())
    {
        // ─────────────────────────────
        // STUDENTS
        // ─────────────────────────────
        var students = new List<Student>
        {
            new() { RegistrationNumber = "TMS-2026-0001",
                    Name = "Alice Smith",       GPA = 3.8m,  IsActive = true  },
            new() { RegistrationNumber = "TMS-2026-0002",
                    Name = "Bob Jones",         GPA = 2.9m,  IsActive = true  },
            new() { RegistrationNumber = "TMS-2026-0003",
                    Name = "Charlie Brown",     GPA = 3.4m,  IsActive = false },
            new() { RegistrationNumber = "TMS-2026-0004",
                    Name = "Diana Prince",      GPA = 3.9m,  IsActive = true  },
            new() { RegistrationNumber = "TMS-2026-0005",
                    Name = "Evan Wright",       GPA = 2.5m,  IsActive = true  },
            new() { RegistrationNumber = "TMS-2026-0006",
                    Name = "Joe Doe",           GPA = 4.00m, IsActive = true  },
            new() { RegistrationNumber = "TMS-2026-0007",
                    Name = "John Stones",       GPA = 3.5m,  IsActive = false },
            new() { RegistrationNumber = "TMS-2026-0008",
                    Name = "Kayl Walker",       GPA = 1.5m,  IsActive = true  },
            new() { RegistrationNumber = "TMS-2026-0009",
                    Name = "Tewodros Abiyu",    GPA = 3.5m,  IsActive = true  },
            new() { RegistrationNumber = "TMS-2026-0010",
                    Name = "Mesfin Abeje",      GPA = 3.3m,  IsActive = true  },
            new() { RegistrationNumber = "TMS-2026-0011",
                    Name = "Eden Mogos",        GPA = 3.4m,  IsActive = false },
            new() { RegistrationNumber = "TMS-2026-0012",
                    Name = "Yasin Tahir",       GPA = 3.6m,  IsActive = true  },
            new() { RegistrationNumber = "TMS-2026-0013",
                    Name = "Muluken Showa",     GPA = 3.7m,  IsActive = false },
            new() { RegistrationNumber = "TMS-2026-0014",
                    Name = "Azmeraw Tefera",    GPA = 3.7m,  IsActive = true  },
            new() { RegistrationNumber = "TMS-2026-0015",
                    Name = "Tesema Melaku",     GPA = 2.9m,  IsActive = false },
            new() { RegistrationNumber = "TMS-2026-0016",
                    Name = "Demelash Ayele",    GPA = 3.1m,  IsActive = true  },
            new() { RegistrationNumber = "TMS-2026-0017",
                    Name = "Leyikun Mekonin",   GPA = 2.5m,  IsActive = false },
            new() { RegistrationNumber = "TMS-2026-0018",
                    Name = "John Smith",        GPA = 3.4m,  IsActive = true  },
            new() { RegistrationNumber = "TMS-2026-0019",
                    Name = "John Stones",       GPA = 3.3m,  IsActive = false },
            new() { RegistrationNumber = "TMS-2026-0020",
                    Name = "Look Shaw",         GPA = 2.2m,  IsActive = true  },
            new() { RegistrationNumber = "TMS-2026-0021",
                    Name = "Dagim Sisay",       GPA = 3.63m, IsActive = false },
            new() { RegistrationNumber = "TMS-2026-0022",
                    Name = "Elsa Berihun",      GPA = 3.73m, IsActive = true  },
            new() { RegistrationNumber = "TMS-2026-0023",
                    Name = "Eliyas Tamiru",     GPA = 3.83m, IsActive = false },
            new() { RegistrationNumber = "TMS-2026-0024",
                    Name = "Yonatan Ejigu",     GPA = 3.8m,  IsActive = true  },
            new() { RegistrationNumber = "TMS-2026-0025",
                    Name = "Estifanos Sisay",   GPA = 2.4m,  IsActive = false },
        };

        context.Students.AddRange(students);

        // ─────────────────────────────
        // COURSES
        // ✅ ALL codes fixed to 10 chars or less
        // to satisfy HasMaxLength(10) in CourseConfiguration
        // ─────────────────────────────
        var courses = new List<Course>
        {
            new() { Code = "CS-101",   Title = "Introduction to Computer Science",  MaxCapacity = 30 },
            new() { Code = "CS-201",   Title = "Data Structures and Algorithms",    MaxCapacity = 25 },
            new() { Code = "MAT-101",  Title = "Calculus I",                        MaxCapacity = 40 },
            new() { Code = "MAT-102",  Title = "Calculus II",                       MaxCapacity = 45 },
            new() { Code = "MAT-103",  Title = "Calculus III",                      MaxCapacity = 30 },
            new() { Code = "THR-104",  Title = "Thermodynamics",                    MaxCapacity = 23 },
            new() { Code = "EMB-105",  Title = "Embedded Systems",                  MaxCapacity = 25 },
            new() { Code = "MAT-106",  Title = "Calculus IV",                       MaxCapacity = 40 },
            new() { Code = "NET-107",  Title = "Computer Network Security",         MaxCapacity = 45 },
            new() { Code = "CSH-108",  Title = "C# Programming",                   MaxCapacity = 34 },
            new() { Code = "JAV-109",  Title = "Java Programming",                  MaxCapacity = 42 },
            new() { Code = "TS-110",   Title = "Typescript Programming",            MaxCapacity = 28 },
            new() { Code = "DS-111",   Title = "Data Structure and Algorithms",     MaxCapacity = 40 },
            new() { Code = "SQL-112",  Title = "Structured Query Languages",        MaxCapacity = 58 },
        };

        context.Courses.AddRange(courses);
        context.SaveChanges();

        // ─────────────────────────────
        // ENROLLMENTS
        // Must come after SaveChanges above
        // so student and course IDs are generated
        // ─────────────────────────────
        var enrollments = new List<Enrollment>
        {
            new() { StudentId = students[0].Id, CourseId = courses[0].Id, Grade = 4.0m },
            new() { StudentId = students[0].Id, CourseId = courses[1].Id, Grade = 3.6m },
            new() { StudentId = students[1].Id, CourseId = courses[0].Id, Grade = 2.8m },
            new() { StudentId = students[3].Id, CourseId = courses[1].Id, Grade = 3.9m },
        };

        context.Enrollments.AddRange(enrollments);
        context.SaveChanges();
    }
}



if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider
        .GetRequiredService<TmsDbContext>();
    await DataSeeder.SeedAsync(context);
}


app.Run();