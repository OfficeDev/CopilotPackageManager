using Microsoft.Graph.Beta;
using PackageManagementAPI_MVC.Auth;

namespace PackageManagementAPI_MVC.Models;

/// <summary>
/// Holds application-wide configuration and the authenticated Graph client.
/// </summary>
public static class ConfigurationSettings
{
    public static void LoadSettings(IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        ClientID = configuration.GetValue<string>("CLIENT_ID") ?? "";
        TenantID = configuration.GetValue<string>("TENANT_ID") ?? "";
        GraphBaseURL = configuration.GetValue<string>("GRAPH_BASE_URL") ?? "";
        GraphClient = AuthProvider.CreateGraphClientAsync(ClientID, TenantID).GetAwaiter().GetResult();
    }

    public static string ClientID { get; set; } = "";

    public static string TenantID { get; set; } = "";

    public static string GraphBaseURL { get; set; } = "";

    public static GraphServiceClient GraphClient { get; set; } = null!;
}
