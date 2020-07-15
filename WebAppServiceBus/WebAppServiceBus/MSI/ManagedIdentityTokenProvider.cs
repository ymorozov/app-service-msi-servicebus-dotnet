using Microsoft.Azure.Services.AppAuthentication;

namespace WebAppServiceBus.MSI
{
    public class ManagedIdentityTokenProvider
    {
        private readonly AzureServiceTokenProvider azureServiceTokenProvider;

        /// <summary>Initializes new instance of <see cref="ManagedIdentityTokenProvider"/> class with default <see cref="AzureServiceTokenProvider"/> configuration.
        public ManagedIdentityTokenProvider() : this(new AzureServiceTokenProvider()) { }

        /// <summary>Initializes new instance of <see cref="ManagedIdentityTokenProvider"/> class with <see cref="AzureServiceTokenProvider"/>.
        /// <remarks>Call that constructore to set <see cref="AzureServiceTokenProvider"/> with required Managed Identity connection string.</remarks>
        public ManagedIdentityTokenProvider(AzureServiceTokenProvider azureServiceTokenProvider)
        {
            this.azureServiceTokenProvider = azureServiceTokenProvider;
        }

        /// <summary>
        /// Gets a <see cref="SecurityToken"/> for the given audience and duration.
        /// </summary>
        /// <param name="appliesTo">The URI which the access token applies to</param>
        /// <returns><see cref="SecurityToken"/></returns>
        public SecurityToken GetTokenAsync(string appliesTo)
        {
            string accessToken = azureServiceTokenProvider.GetAccessTokenAsync("https://servicebus.azure.net/").Result;
            return new JsonSecurityToken(accessToken, appliesTo, "servicebus.windows.net:sastoken");
        }
    }
}
