using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using PhongTro.Domain.Entities;
using PhongTro.Domain.Infracstucture;
using PhongTro.Model.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace PhongTro.WebApi.Providers
{
    public class CustomOAuthProvider : OAuthAuthorizationServerProvider
    {
        /// <summary>
        /// Method validates client.
        /// </summary>
        /// <param name="context">Context contains client credentials</param>
        /// <returns>
        /// A null object
        /// </returns>
        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            // request is always valid here
            context.Validated();
            return Task.FromResult<object>(null);
        }

        /// <summary>
        /// Method takes the username and password from the request, then validates them.
        /// </summary>
        /// <param name="context">Context contains username and password</param>
        /// <returns>
        /// Success: OkResult with ClaimsIdentity object.
        /// Fail: BadRequestResult comes with Error content.
        /// </returns>
        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {

            var allowedOrigin = "*";

            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { allowedOrigin });

            var userManager = context.OwinContext.GetUserManager<PhongTroUserManager>();

            PhongTroUser user = await userManager.FindAsync(context.UserName, context.Password);

            if (null == user)
            {
                context.SetError("invalid_grant", "The user name or password is incorrect.");
                return;
            }
            
            ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(userManager, "JWT");

            var ticket = new AuthenticationTicket(oAuthIdentity, null);

            context.Validated(ticket);
        }
    }
}