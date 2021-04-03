using System;
using System.Text.Json.Serialization;

namespace PartnerCenter.Console.Models
{
    public class PartnerCenterToken
    {
        [JsonPropertyName("token_type")]
        public string TokenType { get; set; } = null!;

        [JsonPropertyName("expires_in")]
        public long ExpiresIn { get; set; }

        [JsonPropertyName("expires_on")]
        public long ExpiresOn { get; set; }

        [JsonPropertyName("not_before")]
        public long NotBefore { get; set; }

        [JsonPropertyName("resource")]
        public string Resource { get; set; } = null!;

        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = null!;

        public DateTimeOffset ExpiresAt
        {
            get
            {
                DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                dtDateTime = dtDateTime.AddSeconds(ExpiresOn);
                return dtDateTime;
            }
        }
    }
}