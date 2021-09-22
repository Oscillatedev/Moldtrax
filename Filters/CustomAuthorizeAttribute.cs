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
            return false;
            

            //return base.AuthorizeCore(httpContext);
        }
    }
}