using Microsoft.AspNetCore.Authentication;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
// Register authentication and authorization services
builder.Services.AddAuthentication("Training")
    .AddScheme<AuthenticationSchemeOptions, TrainingAuthHandler>("Training", null);

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
app.MapGet("/api/assessments/results", () => Results.Ok(new
{
    courseCode = "CS-101",
    studentId = "S-001",
    letterGrade = "A"
}))
.RequireAuthorization(); 
app.MapGet("/api/enrollments/worker-smoke", (EnrollmentWorker worker) =>
{
    worker.ProcessBatch();
    return Results.Ok("processed");
});

app.MapGet("/api/error", () =>
{
    throw new TmsDatabaseException("Simulated database failure for ProblemDetails testing");
});



app.Run();