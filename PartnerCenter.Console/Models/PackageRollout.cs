namespace PartnerCenter.Console.Models
{
    public class PackageRollout
    {
        public bool IsPackageRollout { get; set; }
        public float PackageRolloutPercentage { get; set; }
        public PackageRolloutStatus PackageRolloutStatus { get; set; }
        public string? FallbackSubmissionId { get; set; }
    }
}