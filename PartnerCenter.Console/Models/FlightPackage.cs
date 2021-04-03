namespace PartnerCenter.Console.Models
{
    public class FlightPackage
    {
        public string? FileName { get; set; }
        public FileStatus FileStatus { get; set; }
        public string? Id { get; set; }
        public string? Version { get; set; }
        public string[]? Languages { get; set; }
        public string[]? Capabilities { get; set; }
        public string? MinimumDirectXVersion { get; set; }
        public string? MinimumSystemRam { get; set; }
        public string[]? TargetDeviceFamilies { get; set; }
    }
}