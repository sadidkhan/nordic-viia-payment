using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ViiaNordic.Models.Cache
{
    public class ChachedViiaInfo
    {
        public string ViiaAccessToken { get; set; }
        public DateTimeOffset ViiaAccessTokenExpires { get; set; }
        public string ViiaConsentId { get; set; }
        public string ViiaRefreshToken { get; set; }
        public string ViiaTokenType { get; set; }
    }
}
