using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Moldtrax.Models
{
    public class HttpError
    {
        public string HttpCode { get; set; }
        public string HttpMessage { get; set; }
        public string Message { get; set; }
        public string ErrorClass { get; set; }
    }
}