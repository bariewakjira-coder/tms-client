using Microsoft.EntityFrameworkCore;
namespace TmsApi.Data;
using TmsApi.Entities;

public static class DataSeeder
{
    private static readonly (string Code, string Title, int MaxCapacity)[] Courses =
    [
        ("CSE-101", "Web Development Fundamentals",       30),
        ("CSE-102", "TypeScript Essentials",              30),
        ("CSE-103", "Git and Collaborative Workflows",    25),
        ("CSE-201", "ASP.NET Core Fundamentals",          28),
        ("CSE-202", "Entity Framework Core and PostgreSQL", 28),
        ("CSE-203", "Building RESTful Web APIs",          28),
        ("CSE-301", "Advanced Web API Patterns",          24),
        ("CSE-302", "Angular Fundamentals",               26),
        ("CSE-303", "Angular Advanced",                   24),
        ("CSE-304", "Full-Stack Integration",             22),
        ("CSE-305", "Testing and Quality Assurance",      22),
        ("CSE-306", "Security and Authentication",        20),
        ("DAT-101", "Database Design Foundations",        30),
        ("DAT-201", "Advanced SQL and Indexing",          26),
        ("DAT-202", "Data Modelling for the Web",         26),
        ("ARC-101", "Software Architecture Patterns",     22),
        ("ARC-201", "Cloud-Native Architecture",          22),
        ("DEV-101", "DevOps Foundations",                 24),
        ("DEV-201", "Continuous Delivery Pipelines",      22),
        ("MOB-101", "Mobile App Foundations",             24),
        ("MOB-201", "Cross-Platform Mobile",              22),
        ("AI-101",  "Applied Machine Learning",           20),
        ("AI-201",  "Generative AI for Developers",       18),
        ("UX-101",  "UX Research and Wireframing",        24),
        ("UX-201",  "Design Systems and Tokens",          22),
    ];

    public static async Task SeedAsync(
        TmsDbContext context,
        CancellationToken ct = default)
    {
        
        await context.Database.MigrateAsync(ct);

        
        if (await context.Courses.AnyAsync(ct))
            return;

        foreach (var (code, title, maxCapacity) in Courses)
        {
            context.Courses.Add(new Course
            {
                Code        = code,
                Title       = title,
                MaxCapacity = maxCapacity
            });
        }

        await context.SaveChangesAsync(ct);
    }
}