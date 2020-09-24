using Newtonsoft.Json;

namespace ViiaNordic.Models.Viia
{
    public class ValidateAccountFeature
    {
        [JsonProperty("paymentFrom")]
        public bool PaymentFrom { get; set; }

        [JsonProperty("paymentTo")]
        public bool PaymentTo { get; set; }

        [JsonProperty("psdPaymentAccount")]
        public bool PsdPaymentAccount { get; set; }

        [JsonProperty("queryable")]
        public bool Queryable { get; set; }

    }
}
