using System;

namespace PartnerCenter.Console.Models
{
    public class FlightSubmission
    {
        public string Id { get; set; } = null!;
        public Guid FlightId { get; set; }
        public SubmissionStatus Status { get; set; }
        public StatusDetails? StatusDetails { get; set; }
        public FlightPackage[]? FlightPackages { get; set; }
        public PackageDeliveryOptions? PackageDeliveryOptions { get; set; }
        public string? FileUploadUrl { get; set; }
        public TargetPublishMode TargetPublishMode { get; set; }
        public DateTimeOffset TargetPublishDate { get; set; }
        public string? NotesForCertification { get; set; }
    }
}