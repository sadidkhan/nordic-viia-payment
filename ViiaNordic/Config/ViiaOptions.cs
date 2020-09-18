using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ViiaNordic.Config
{
    public class ViiaOptions
    {
        public string BaseApiUrl { get; set; }
        public string BaseAppUrl { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string LoginCallbackUrl { get; set; }
        public string WebHookSecret { get; set; }
    }
}
