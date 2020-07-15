using System;
using System.IdentityModel.Tokens.Jwt;

namespace WebAppServiceBus.MSI
{
    public class JsonSecurityToken : SecurityToken
    {
        /// <summary>
        /// Creates a new instance of the <see cref="JsonSecurityToken"/> class.
        /// </summary>
        /// <param name="rawToken">Raw JSON Web Token string</param>
        /// <param name="audience">The audience</param>
        public JsonSecurityToken(string rawToken, string audience, string tokenType)
            : base(rawToken, GetExpirationDateTimeUtcFromToken(rawToken), audience, tokenType)
        {
        }

        static DateTime GetExpirationDateTimeUtcFromToken(string token)
        {
            var jwtSecurityToken = new JwtSecurityToken(token);
            return jwtSecurityToken.ValidTo;
        }
    }
}
