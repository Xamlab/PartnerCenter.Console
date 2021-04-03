namespace PartnerCenter.Console.Models
{
    public class StatusDetails
    {
        public StatusDetail[]? Errors { get; set; }
        public StatusDetail[]? Warnings { get; set; }
        public CertificationReport[]? CertificationReports { get; set; }
    }
}