using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Newtonsoft.Json.Linq;
using PhongTro.Domain.Entities;
using PhongTro.Domain.Infracstucture;
using PhongTro.Model.Core;
using PhongTro.WebApi.Providers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace PhongTro.WebApi.Controllers
{
    /// <summary>
    /// Base class for controllers 
    /// </summary>
    public class BaseApiController : ApiController
    {
        #region Constants
        const string KeyTokenIssuer = "tokenIssuer";
        const string JWTAuthenticationType = "JWT";
        const string KeyResponseToken = "access_token";
        const string KeyResponseType = "token_type";
        const string KeyResponseExpire = "expires_in";
        const string TokenType = "bearer";

        #endregion

        private IRepository _repository;

        protected IRepository _Repository
        {
            get
            {
                return _repository;
            }
            set
            {
                _repository = value;
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="repo">The implement of IRepository interface</param>
        public BaseApiController(IRepository repo)
        {
            _Repository = repo;
        }

        /// <summary>
        /// Generate a valid token for a user from controller
        /// </summary>
        /// <param name="user">The logged in user</param>
        /// <returns></returns>
        protected async Task<JObject> GenerateLocalAccessToken(PhongTroUser user)
        {
            var userManager = HttpContext.Current.GetOwinContext().GetUserManager<PhongTroUserManager>();

            var validTime = TimeSpan.FromDays(1);
            var identity = await userManager.CreateIdentityAsync(user, JWTAuthenticationType);
            var jwtFormat = new CustomJwtFormat(ConfigurationManager.AppSettings[KeyTokenIssuer]);
            var authenticationProperties = new AuthenticationProperties()
            {
                IssuedUtc = DateTimeOffset.UtcNow,
                ExpiresUtc = DateTimeOffset.UtcNow.Add(validTime)
            };
            var authenticationTicket = new AuthenticationTicket(identity, authenticationProperties);
            var token = jwtFormat.Protect(authenticationTicket);

            JObject response = new JObject(
                                        new JProperty(KeyResponseToken, token),
                                        new JProperty(KeyResponseType, TokenType),
                                        new JProperty(KeyResponseExpire, validTime.TotalSeconds.ToString()));

            return response;
        }

        /// <summary>
        /// Determine the type of result
        /// </summary>
        /// <param name="result">Object contains the result</param>
        /// <returns>An IHttpActionResult object</returns>
        protected IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }
    }
}