using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler.Encoder;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Web;
using Thinktecture.IdentityModel.Tokens;

namespace PhongTro.WebApi.Providers
{
    /// <summary>
    /// Class is reponsible for custom JWT format.
    /// </summary>
    public class CustomJwtFormat : ISecureDataFormat<AuthenticationTicket>
    {
        #region Constant
        // Key references to values in Web.config
        const string KeyAudienceId = "as:AudienceId";
        const string KeyAudienceSecret = "as:AudienceSecret";
        #endregion

        private readonly string _issuer = string.Empty;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="issuer">String represents the issuer</param>
        public CustomJwtFormat(string issuer)
        {
            _issuer = issuer;
        }

        /// <summary>
        /// Method where the JWT generation takes place.
        /// </summary>
        /// <param name="data">Contains user identity information</param>
        /// <returns></returns>
        public string Protect(AuthenticationTicket data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            string audienceId = ConfigurationManager.AppSettings[KeyAudienceId];

            string symmetricKeyAsBase64 = ConfigurationManager.AppSettings[KeyAudienceSecret];

            var keyByteArray = TextEncodings.Base64Url.Decode(symmetricKeyAsBase64);

            var signingKey = new HmacSigningCredentials(keyByteArray);

            var issued = data.Properties.IssuedUtc;

            var expires = data.Properties.ExpiresUtc;

            var token = new JwtSecurityToken(_issuer, audienceId, data.Identity.Claims, issued.Value.UtcDateTime, expires.Value.UtcDateTime, signingKey);

            var handler = new JwtSecurityTokenHandler();

            var jwt = handler.WriteToken(token);

            return jwt;
        }

        public AuthenticationTicket Unprotect(string protectedText)
        {
            throw new NotImplementedException();
        }
    }
}