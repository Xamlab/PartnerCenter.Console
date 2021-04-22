namespace PartnerCenter.Console.Configuration
{
    public class PartnerCenterConfiguration
    {
        public string Tenant { get; set; } = null!;
        public string ApplicationId { get; set; } = null!;
        public string FlightId { get; set; } = null!;
        public string TokenBaseUrl { get; set; } = null!;
        public string ManagementBaseUrl { get; set; } = null!;
        public string ClientId { get; set; } = null!;
        public string ClientSecret { get; set; } = null!;
    }
}