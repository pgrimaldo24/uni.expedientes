using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Unir.Expedientes.Application.Common.Models.Security
{
    public class AccountsModel
    {
        [JsonPropertyName("meta")] public Meta Meta { get; set; }
        [JsonPropertyName("data")] public List<Data> Data;
    }

    public class Meta
    {
        [JsonPropertyName("totalCount")] public int TotalCount;
        [JsonPropertyName("offset")] public int OffSet;
        [JsonPropertyName("limit")] public int Limit;
    }

    public class Data
    {
        [JsonPropertyName("accountId")] public string AccountId;
        [JsonPropertyName("loginNames")] public string[] LoginNames;
        [JsonPropertyName("firstName")] public string FirstName;
        [JsonPropertyName("surname")] public string Surname;
        [JsonPropertyName("mobileNumber")] public string MobileNumber;
        [JsonPropertyName("trabajadorIntegrationId")] public string TrabajadorIntegrationId;
        [JsonPropertyName("email")] public string Email;
        [JsonPropertyName("hidden")] public bool Hidden;
        [JsonPropertyName("active")] public bool Active;
        [JsonPropertyName("lastLogin")] public string LastLogin;
        [JsonPropertyName("loginCount")] public int LoginCount;
    }
}