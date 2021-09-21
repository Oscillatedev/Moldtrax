using Moldtrax.App_Start;
using Moldtrax.Common;
using Moldtrax.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Moldtrax.Filters
{
    public class CustomAuthorizeAttribute :AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
           // Logger.Log("came here");
            if (httpContext.Session["accessToken"]!=null)
            {
                var token = httpContext.Session["accessToken"].ToString();
                var headers = new Dictionary<string, string>
                {
                    { "Authorization",token}
                };

                HttpRequestInput input = new HttpRequestInput
                {
                    BaseUrl = "graph.microsoft.com/beta/",
                    Url = "me",
                    Headers = headers
    ,
                };
                var response = HttpService.Instance.Get(input).GetAwaiter().GetResult();
                if (response.IsSuccessStatusCode)
                {
                    var context = response.Content.ReadAsStringAsync().Result;
                    Logger.Log(context);
                }
                return true;
            }
            return false;
            

            //return base.AuthorizeCore(httpContext);
        }
    }
}