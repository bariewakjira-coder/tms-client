using System.Threading;
using System.Threading.Tasks;
using TmsApi.Entities; // Adjust namespace to match your Course entity location

namespace TmsApi.Application.Interfaces
{
    public interface ICourseRepository
    {
        Task<Course?> GetByCodeAsync(string courseCode, CancellationToken ct);
    }
}