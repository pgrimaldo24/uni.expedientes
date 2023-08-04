using System;
using System.Text.Json.Serialization;

namespace Unir.Expedientes.Application.Common.Models.Security
{
    public class AuthTokenModel
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }
        [JsonPropertyName("expires_in")]
        public int? ExpiresIn { get; set; }

        public DateTime ExpirationDateTime { get; set; }
    }
}