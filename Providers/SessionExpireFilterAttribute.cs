using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Moldtrax.Providers
{
    public class SessionExpireFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpContext ctx = HttpContext.Current;
            // check  sessions here
            ////if (HttpContext.Current.Session["CompanyID"] == null || HttpContext.Current.Session["User"] == null || HttpContext.Current.Session["RoleID"] == null || HttpContext.Current.Session["Permission"] == null)
            ////{
            ////    filterContext.Result = new RedirectResult("~/Account/Login");
            ////    return;
            ////}

            base.OnActionExecuting(filterContext);
        }
    }
}