using System;

namespace WebAppServiceBus.MSI
{
    public class SecurityToken
    {
        /// <summary>
        /// Token literal
        /// </summary>
        string token;

        /// <summary>
        /// Expiry date-time
        /// </summary>
        DateTime expiresAtUtc;

        /// <summary>
        /// Token audience
        /// </summary>
        string audience;

        /// <summary>
        /// Token type
        /// </summary>
        string tokenType;

        /// <summary>
        /// Creates a new instance of the <see cref="SecurityToken"/> class.
        /// </summary>
        /// <param name="tokenString">The token</param>
        /// <param name="expiresAtUtc">The expiration time</param>
        /// <param name="audience">The audience</param>
        /// <param name="tokenType">The type of the token</param>
        public SecurityToken(string tokenString, DateTime expiresAtUtc, string audience, string tokenType)
        {
            if (string.IsNullOrEmpty(tokenString))
            {
                //exception
            }

            if (string.IsNullOrEmpty(audience))
            {
                //exception
            }

            this.token = tokenString;
            this.expiresAtUtc = expiresAtUtc;
            this.audience = audience;
            this.tokenType = tokenType;
        }

        /// <summary>
        /// Gets the audience of this token.
        /// </summary>
        public string Audience => this.audience;

        /// <summary>
        /// Gets the expiration time of this token.
        /// </summary>
        public DateTime ExpiresAtUtc => this.expiresAtUtc;

        /// <summary>
        /// Gets the actual token.
        /// </summary>
        public virtual string TokenValue => this.token;

        /// <summary>
        /// Gets the token type.
        /// </summary>
        public virtual string TokenType => this.tokenType;
    }
}
