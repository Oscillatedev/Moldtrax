using System.Collections.Generic;
using System.Net.Http;

namespace Moldtrax.Models
{
    public class HttpRequestInput
    {
        public string BaseUrl { get; set; }
        public string Url { get; set; }
        public Dictionary<string, string> Headers { get; set; }

        public HttpContent Content { get; set; }
    }
}