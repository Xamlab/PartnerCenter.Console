using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using PartnerCenter.Console.Configuration;
using PartnerCenter.Console.Models;

namespace PartnerCenter.Console.Services.Implementation
{
    public class PartnerCenterService : IPartnerCenterService
    {
        private readonly PartnerCenterConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonSerializationOptions;

        public PartnerCenterService(PartnerCenterConfiguration configuration,
                                    HttpClient httpClient)
        {
            _configuration = configuration;
            _httpClient = httpClient;
            _jsonSerializationOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
            _jsonSerializationOptions.Converters.Add(new JsonStringEnumConverter());
        }

        public async Task<PackageFlight[]?> GetPackageFlightsAsync(CancellationToken cancellationToken = default)
        {
            var result = await _httpClient.GetFromJsonAsync<CollectionResult<PackageFlight>>($"v1.0/my/applications/{_configuration.ApplicationId}/listflights", _jsonSerializationOptions, cancellationToken);
            return result?.Value;
        }

        public async Task<PackageFlight> CreatePackageFlightAsync(string flight, CancellationToken cancellationToken)
        {
            var createPackageFlightRequest = new
                                             {
                                                 friendlyName = flight,
                                                 groupIds = new[] { "1152921504607280734" }
                                             };
            var response = await _httpClient.PostAsJsonAsync($"v1.0/my/applications/{_configuration.ApplicationId}/flights", createPackageFlightRequest, _jsonSerializationOptions, cancellationToken);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<PackageFlight>(cancellationToken: cancellationToken);
            return result!;
        }

        public async Task<FlightSubmission> CreateFlightSubmissionAsync(Guid flightId, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PostAsJsonAsync($"v1.0/my/applications/{_configuration.ApplicationId}/flights/{flightId}/submissions", new {}, cancellationToken);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<FlightSubmission>(_jsonSerializationOptions, cancellationToken);
            return result!;
        }

        public async Task<FlightSubmission> GetFlightSubmissionAsync(Guid flightId, string submissionId, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetAsync($"v1.0/my/applications/{_configuration.ApplicationId}/flights/{flightId}/submissions/{submissionId}", cancellationToken);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<FlightSubmission>(_jsonSerializationOptions, cancellationToken);
            return result!;
        }

        public async Task DeleteFlightSubmissionAsync(Guid flightId, string submissionId, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.DeleteAsync($"v1.0/my/applications/{_configuration.ApplicationId}/flights/{flightId}/submissions/{submissionId}", cancellationToken);
            response.EnsureSuccessStatusCode();
        }

        public async Task UploadPackageBundleAsync(string bundlePath, string sasUrl, CancellationToken cancellationToken = default)
        {
            var blobClient = new BlobClient(new Uri(sasUrl));
            using var consoleStorageProgress = new ConsoleStorageProgress(bundlePath);
            await blobClient.UploadAsync(bundlePath, progressHandler: consoleStorageProgress,
                                         cancellationToken: cancellationToken);
        }

        private static string CreateBundleArchive(string bundlePath)
        {
            var bundleArchivePath = $"package_{DateTime.UtcNow.Ticks}.zip";
            using var bundleStream = new FileStream(bundleArchivePath, FileMode.Create);
            using var bundleArchive = new ZipArchive(bundleStream, ZipArchiveMode.Create);
            bundleArchive.CreateEntryFromFile(bundlePath, Path.GetFileName(bundlePath));
            return bundleArchivePath;
        }

        public async Task UpdateFlightSubmissionAsync(FlightSubmission flightSubmission, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PutAsJsonAsync($"v1.0/my/applications/{_configuration.ApplicationId}/flights/{flightSubmission.FlightId}/submissions/{flightSubmission.Id}", flightSubmission, _jsonSerializationOptions, cancellationToken);
            response.EnsureSuccessStatusCode();
        }

        public async Task CommitFlightSubmissionAsync(Guid flightId, string submissionId, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PostAsJsonAsync($"v1.0/my/applications/{_configuration.ApplicationId}/flights/{flightId}/submissions/{submissionId}/commit", new {}, cancellationToken);
            response.EnsureSuccessStatusCode();
        }
    }
}