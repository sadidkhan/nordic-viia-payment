namespace ViiaNordic.Models.View
{
    public class CreatePaymentResultViewModel
    {
        public string ErrorDescription { get; set; }
        public string PaymentId { get; set; }
        public string AuthorizationUrl { get; set; }
    }
}
