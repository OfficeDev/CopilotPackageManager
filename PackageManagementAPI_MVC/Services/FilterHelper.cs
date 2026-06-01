namespace PackageManagementAPI_MVC.Services;

/// <summary>
/// Utility class for building OData $filter query strings for the Package Management API.
/// </summary>
public static class FilterHelper
{
    /// <summary>
    /// Builds a filter to match packages by supported host (e.g., Copilot, Teams, Outlook).
    /// </summary>
    public static string BySupportedHost(string host) =>
        $"supportedHosts/any(h:h eq '{host}')";

    /// <summary>
    /// Builds a filter to match packages by element type (e.g., DeclarativeAgent, Bot).
    /// </summary>
    public static string ByElementType(string elementType) =>
        $"elementTypes/any(h:h eq '{elementType}')";

    /// <summary>
    /// Builds a filter to match packages modified after a specific date/time.
    /// </summary>
    public static string ByLastModifiedAfter(DateTimeOffset dateTime) =>
        $"lastModifiedDateTime gt {dateTime:yyyy-MM-ddTHH:mm:ssZ}";
}
