using System;

namespace PartnerCenter.Console.Models
{
    public class PackageDeliveryOptions
    {
        public PackageRollout? PackageRollout { get; set; }
        public bool IsMandatoryUpdate { get; set; }
        public DateTimeOffset MandatoryUpdateEffectiveDate { get; set; }
    }
}