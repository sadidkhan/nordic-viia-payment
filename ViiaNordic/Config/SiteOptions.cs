using ViiaNordic.Config;

namespace ViiaNordic
{
    public class SiteOptions
    {
        // public HumioOptions Humio { get; set; }
        public bool LogToConsole { get; set; } = false;
        // public SendGridOptions SendGrid { get; set; }
        public string FakeUserEmail { get; set; }
        public ViiaOptions Viia { get; set; }
    }

    //public class SendGridOptions
    //{
    //    public string ApiKey { get; set; }
    //    public string EmailFrom { get; set; }
    //    public string NameFrom { get; set; }
    //}

    //public class HumioOptions
    //{
    //    public string IngestToken { get; set; }
    //    public string IngestUrl { get; set; }
    //}
}
