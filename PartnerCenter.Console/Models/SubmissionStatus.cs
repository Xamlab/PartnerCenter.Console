namespace PartnerCenter.Console.Models
{
    public enum SubmissionStatus
    {
        None,
        Canceled,
        PendingCommit,
        CommitStarted,
        CommitFailed,
        PendingPublication,
        Publishing,
        Published,
        PublishFailed,
        PreProcessing,
        PreProcessingFailed,
        Certification,
        CertificationFailed,
        Release,
        ReleaseFailed
    }
}