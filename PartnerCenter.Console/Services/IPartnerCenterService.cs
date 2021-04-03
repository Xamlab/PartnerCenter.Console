using System;
using System.Threading;
using System.Threading.Tasks;
using PartnerCenter.Console.Models;

namespace PartnerCenter.Console.Services
{
    public interface IPartnerCenterService
    {
        Task<PackageFlight[]?> GetPackageFlightsAsync(CancellationToken cancellationToken = default);
        Task<PackageFlight> CreatePackageFlightAsync(string flight, CancellationToken cancellationToken = default);
        Task<FlightSubmission> CreateFlightSubmissionAsync(Guid flightId, CancellationToken cancellationToken = default);
        Task<FlightSubmission> GetFlightSubmissionAsync(Guid flightId, string submissionId, CancellationToken cancellationToken = default);
        Task DeleteFlightSubmissionAsync(Guid flightId, string submissionId, CancellationToken cancellationToken = default);
        Task UploadPackageBundleAsync(string bundlePath, string sasUrl, CancellationToken cancellationToken = default);
        Task UpdateFlightSubmissionAsync(FlightSubmission flightSubmission, CancellationToken cancellationToken = default);
        Task CommitFlightSubmissionAsync(Guid flightId, string submissionId, CancellationToken cancellationToken = default);
    }
}