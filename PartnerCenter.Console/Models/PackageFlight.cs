using System;

namespace PartnerCenter.Console.Models
{
    public class PackageFlight
    {
        public Guid FlightId { get; set; }
        public string FriendlyName { get; set; } = null!;
        public FlightSubmissionInfo? PendingFlightSubmission { get; set; }
        public FlightSubmissionInfo? LastPublishedFlightSubmission { get; set; }
        public string[] GroupIds { get; set; } = null!;
        public string RankHigherThan { get; set; } = null!;
    }
    /*
     * {
            "flightId": "65410f96-960a-4d97-aed4-c919736fe415",
            "friendlyName": "Staging",
            "lastPublishedFlightSubmission": {
                "id": "1152921505693310969",
                "resourceLocation": "flights/65410f96-960a-4d97-aed4-c919736fe415/submissions/1152921505693310969"
            },
            "pendingFlightSubmission": {
                "id": "1152921505693334651",
                "resourceLocation": "flights/505cac50-4049-479f-b31d-7c18b62e1528/submissions/1152921505693334651"
            },
            "groupIds": [
                "1152921504607280734"
            ],
            "rankHigherThan": "Non-flighted submission"
        },
     */
}