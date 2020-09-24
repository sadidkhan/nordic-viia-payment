using Newtonsoft.Json;

namespace ViiaNordic.Models.Viia
{
    public class ValidateAccountResponse
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("features")]
        public ValidateAccountFeature Features { get; set; }

        [JsonProperty("isOrphaned")]
        public bool IsOrphaned { get; set; }
    }
}
