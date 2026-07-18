using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TmsApi.Entities; // Adjust namespace to match your Enrollment entity

namespace TmsApi.Application.Interfaces
{
    public interface IEnrollmentRepository
    {
        Task<bool> ExistsAsync(int studentId, string courseCode, CancellationToken ct);
        Task AddAsync(Enrollment enrollment, CancellationToken ct);
        
        // Add this missing method:
        Task<List<Enrollment>> GetByStudentIdAsync(int studentId, CancellationToken ct);
    }
}