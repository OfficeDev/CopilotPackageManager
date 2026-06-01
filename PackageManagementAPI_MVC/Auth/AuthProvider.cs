using Azure.Core;
using Azure.Identity;
using Microsoft.Graph.Beta;
using Microsoft.Kiota.Authentication.Azure;

namespace PackageManagementAPI_MVC.Auth;

/// <summary>
/// Provides MSAL-based authentication using delegated (interactive browser) flow.
/// </summary>
public static class AuthProvider
{
    private static readonly string[] Scopes = ["CopilotPackages.Read.All"];
    private static readonly string[] TokenScopes = ["https://graph.microsoft.com/CopilotPackages.Read.All"];

    /// <summary>
    /// Creates a GraphServiceClient configured with interactive browser authentication
    /// and forces an immediate token acquisition so the user signs in upfront.
    /// </summary>
    /// <param name="clientId">The Entra AD application (client) ID.</param>
    /// <param name="tenantId">The Entra AD tenant ID.</param>
    /// <returns>An authenticated GraphServiceClient for the Beta endpoint.</returns>
    public static async Task<GraphServiceClient> CreateGraphClientAsync(string clientId, string tenantId)
    {
        var credential = new InteractiveBrowserCredential(new InteractiveBrowserCredentialOptions
        {
            ClientId = clientId,
            TenantId = tenantId,
            RedirectUri = new Uri("http://localhost")
        });

        // Force interactive sign-in immediately
        await credential.GetTokenAsync(new TokenRequestContext(TokenScopes));

        var authProvider = new AzureIdentityAuthenticationProvider(credential, isCaeEnabled: false, scopes: Scopes);

        return new GraphServiceClient(authProvider);
    }
}
