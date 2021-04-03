using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PartnerCenter.Console.Models;

namespace PartnerCenter.Console.Services.Implementation
{
    public class UpdateService : IUpdateService
    {
        private readonly IPartnerCenterService _partnerCenterService;

        public UpdateService(IPartnerCenterService partnerCenterService)
        {
            _partnerCenterService = partnerCenterService;
        }

        public async Task UpdateFlightAsync(string flightName, string bundlePath, CancellationToken cancellationToken = default)
        {
            System.Console.WriteLine($"Retrieving flight '{flightName}'");
            PackageFlight packageFlight = await GetOrCreatePackageFlightAsync(flightName, cancellationToken);
            
            System.Console.WriteLine($"Creating submission for flight '{flightName}'");
            FlightSubmission submission = await GetOrCreatePackageFlightSubmissionAsync(packageFlight, cancellationToken);
            
            if(string.IsNullOrWhiteSpace(submission.FileUploadUrl))
            {
                throw new InvalidOperationException($"Bundle upload URL for flight submission {submission.FlightId} is missing. The flight submission is in invalid state. Delete the submission and start over.");
            }

            await _partnerCenterService.UploadPackageBundleAsync(bundlePath, submission.FileUploadUrl, cancellationToken);
            
            submission.FlightPackages = new[]
                                        {
                                            new FlightPackage
                                            {
                                                FileStatus = FileStatus.PendingUpload,
                                                FileName = $"{Path.GetFileNameWithoutExtension(bundlePath)}.msixbundle",
                                                MinimumDirectXVersion = "None",
                                                MinimumSystemRam = "None"
                                            }
                                        };
            
            System.Console.WriteLine($"Updating submission {submission.Id} in flight '{flightName}'");
            await _partnerCenterService.UpdateFlightSubmissionAsync(submission, cancellationToken);
            
            System.Console.WriteLine($"Commiting submission {submission.Id} in flight '{flightName}'");
            await _partnerCenterService.CommitFlightSubmissionAsync(submission.FlightId, submission.Id, cancellationToken);
        }

        
        private async Task<PackageFlight> GetOrCreatePackageFlightAsync(string flightName, CancellationToken cancellationToken)
        {
            var packageFlights = await _partnerCenterService.GetPackageFlightsAsync(cancellationToken);
            var packageFlight = packageFlights?.FirstOrDefault(pf => pf.FriendlyName == flightName);
            if(packageFlight != null)
            {
                return packageFlight;
            }

            packageFlight = await _partnerCenterService.CreatePackageFlightAsync(flightName, cancellationToken);
            if(packageFlight == null)
            {
                throw new InvalidOperationException($"Something when wrong while retrieving flight {flightName}");
            }
            return packageFlight;
        }
        
        private async Task<FlightSubmission> GetOrCreatePackageFlightSubmissionAsync(PackageFlight packageFlight, CancellationToken cancellationToken)
        {       
            //Delete any pending submissions, unless we are publishing for the first time
            //in which case there won't be any existing published submissions
            if(packageFlight.PendingFlightSubmission != null && packageFlight.LastPublishedFlightSubmission != null)
            {
                await _partnerCenterService.DeleteFlightSubmissionAsync(packageFlight.FlightId, packageFlight.PendingFlightSubmission.Id, cancellationToken);
                packageFlight.PendingFlightSubmission = null;
            }
            
            FlightSubmission? flightSubmission;
            if(!string.IsNullOrWhiteSpace(packageFlight.PendingFlightSubmission?.Id))
            {
                flightSubmission = await _partnerCenterService.GetFlightSubmissionAsync(packageFlight.FlightId, packageFlight.PendingFlightSubmission.Id, cancellationToken);
            }
            else
            {
                flightSubmission = await _partnerCenterService.CreateFlightSubmissionAsync(packageFlight.FlightId, cancellationToken);   
            }
            if(flightSubmission == null)
            {
                throw new InvalidOperationException($"Something when wrong while retrieving the current submission for flight {packageFlight.FriendlyName}");
            }
            return flightSubmission!;
        }
        
    }
}