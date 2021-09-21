using System;
using System.Collections.Generic;
using System.Configuration;

using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.Owin;
using Microsoft.Owin.Extensions;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Notifications;
using Microsoft.Owin.Security.OpenIdConnect;
using Moldtrax.App_Start;
using Moldtrax.Common;
using Moldtrax.Models;
using Owin;

[assembly: OwinStartup(typeof(Moldtrax.Startup))]

namespace Moldtrax
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var _clientID = ConfigurationManager.AppSettings["ClientID"];
            var _authority = ConfigurationManager.AppSettings["Authority"];
            var _scopes = ConfigurationManager.AppSettings["Scopes"];

            // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=316888
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

            app.UseCookieAuthentication(new CookieAuthenticationOptions());

            app.UseOpenIdConnectAuthentication(
            new OpenIdConnectAuthenticationOptions
            {

                ClientId = _clientID,
                Authority = _authority,
                Scope = _scopes,
                ResponseType = OpenIdConnectResponseType.CodeIdToken,
                Notifications = new OpenIdConnectAuthenticationNotifications()
                {
                    AuthenticationFailed = OnAuthenticationFailed,

                    SecurityTokenValidated = (context) =>
                    {
                        var claims = context.AuthenticationTicket.Identity.Claims;

                        
                        HttpContext.Current.Session["accessToken"] = context.ProtocolMessage.AccessToken;
                        var groups = from c in claims
                                     where c.Type == "groups"
                                     select c;
                        foreach (var group in groups)
                        {
                            context.AuthenticationTicket.Identity.AddClaim(new Claim(ClaimTypes.Role, group.Value));
                        }
                        return Task.FromResult(0);
                    },
                    SecurityTokenReceived = (context) =>
                    {
                        var token = context.ProtocolMessage.AccessToken;
                        
                       
;                        //Logger.Log($"Token  : {token}");
                        return Task.FromResult(0);
                    },
                    TokenResponseReceived = (context) =>
                    {
                        return Task.FromResult(0);
                    }
                }

            });

           // var user = new ClaimsPrincipal();
            //var accessToken = user.FindFirst("access_token").Value;

            // This makes any middleware defined above this line run before the Authorization rule is applied in web.config
            app.UseStageMarker(PipelineStage.Authenticate);
        }

        private Task OnAuthenticationFailed(AuthenticationFailedNotification<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions> context)
        {
            context.HandleResponse();
            context.Response.Redirect("/?errormessage=" + context.Exception.Message);
            return Task.FromResult(0);
        }
    }
}
