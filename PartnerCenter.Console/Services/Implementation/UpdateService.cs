using System;
using System.Collections.Generic;
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
            PackageFlight packageFlight = await GetPackageFlightAsync(flightName, cancellationToken);

            System.Console.WriteLine($"Creating submission for flight '{flightName}'");
            FlightSubmission flightSubmission = await GetOrCreatePackageFlightSubmissionAsync(packageFlight, cancellationToken);

            if(string.IsNullOrWhiteSpace(flightSubmission.FileUploadUrl))
            {
                throw new InvalidOperationException($"Bundle upload URL for flight submission {flightSubmission.FlightId} is missing. The flight submission is in invalid state. Delete the submission and start over.");
            }

            await _partnerCenterService.UploadPackageBundleAsync(bundlePath, flightSubmission.FileUploadUrl, cancellationToken);

            //Delete the current package
            flightSubmission.FlightPackages![0].FileStatus = FileStatus.PendingDelete;
            var packages = new List<FlightPackage>
                           {
                               flightSubmission.FlightPackages[0],
                               new FlightPackage
                               {
                                   FileStatus = FileStatus.PendingUpload,
                                   FileName = $"{Path.GetFileNameWithoutExtension(bundlePath)}.msixbundle",
                                   MinimumDirectXVersion = "None",
                                   MinimumSystemRam = "None"
                               }
                           };
            flightSubmission.FlightPackages = packages.ToArray();

            System.Console.WriteLine($"Updating submission {flightSubmission.Id} in flight '{flightName}'");
            await _partnerCenterService.UpdateFlightSubmissionAsync(flightSubmission, cancellationToken);

            System.Console.WriteLine($"Commiting submission {flightSubmission.Id} in flight '{flightName}'");
            await _partnerCenterService.CommitFlightSubmissionAsync(flightSubmission.FlightId, flightSubmission.Id, cancellationToken);
        }

        private async Task<PackageFlight> GetPackageFlightAsync(string flightName, CancellationToken cancellationToken)
        {
            PackageFlight[]? packageFlights = await _partnerCenterService.GetPackageFlightsAsync(cancellationToken);
            PackageFlight? packageFlight = packageFlights?.FirstOrDefault(pf => pf.FriendlyName == flightName);
            if(packageFlight == null)
            {
                throw new InvalidOperationException($"Flight {flightName} was not found. Before publishing package through API please publish at least one package manually through partner portal");
            }
            return packageFlight;
        }

        private async Task<FlightSubmission> GetOrCreatePackageFlightSubmissionAsync(PackageFlight packageFlight, CancellationToken cancellationToken)
        {
            if(packageFlight.LastPublishedFlightSubmission == null)
            {
                throw new InvalidOperationException($"Before publishing to flight {packageFlight.FriendlyName} through API please publish at least one package manually through partner portal");
            }

            if(packageFlight.PendingFlightSubmission != null)
            {
                await _partnerCenterService.DeleteFlightSubmissionAsync(packageFlight.FlightId, packageFlight.PendingFlightSubmission.Id, cancellationToken);
                packageFlight.PendingFlightSubmission = null;
            }

            FlightSubmission? flightSubmission = await _partnerCenterService.CreateFlightSubmissionAsync(packageFlight.FlightId, cancellationToken);

            if(flightSubmission == null)
            {
                throw new InvalidOperationException($"Something when wrong while retrieving the current submission for flight {packageFlight.FriendlyName}");
            }
            return flightSubmission!;
        }
    }
}