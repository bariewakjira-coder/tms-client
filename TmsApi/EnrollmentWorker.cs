// using System;

// public class EnrollmentWorker
// {
//     private readonly IEnrollmentService _enrollmentService;

//     // Intentionally injecting the Scoped service directly into a Singleton constructor
//     public EnrollmentWorker(IEnrollmentService enrollmentService)
//     {
//         _enrollmentService = enrollmentService;
//     }

//     public void ProcessBatch()
//     {
//         // Smoke test method placeholder
//         Console.WriteLine("Processing background batch...");
//     }
// }

using System;
using Microsoft.Extensions.DependencyInjection;

public class EnrollmentWorker
{
    private readonly IServiceScopeFactory _scopeFactory;

    // Injecting the Factory (which is a Singleton) instead of the Scoped service directly
    public EnrollmentWorker(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public void ProcessBatch()
    {
        // Dynamically create a short-lived scope
        using (var scope = _scopeFactory.CreateScope())
        {
            // Resolve the Scoped service safely inside this block
            var enrollmentService = scope.ServiceProvider.GetRequiredService<IEnrollmentService>();
            
            Console.WriteLine("Processing background batch using a safe scope...");
        } // The scope ends here, cleaning up resources properly
    }
}

