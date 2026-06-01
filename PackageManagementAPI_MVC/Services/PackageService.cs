using Microsoft.Graph.Beta;
using Microsoft.Kiota.Abstractions;
using PackageManagementAPI_MVC.Models;
using System.Text.Json;

namespace PackageManagementAPI_MVC.Services;

/// <summary>
/// Service for listing and retrieving Copilot package information via Microsoft Graph Beta API.
/// </summary>
public class PackageService
{
    private readonly string _baseUrl;
    private readonly GraphServiceClient _graphClient;

    public PackageService(GraphServiceClient graphClient, string baseUrl)
    {
        _graphClient = graphClient;
        _baseUrl = baseUrl;
    }

    /// <summary>
    /// Lists all packages, optionally applying an OData $filter.
    /// Handles pagination via @odata.nextLink.
    /// </summary>
    /// <param name="filter">Optional OData $filter query string.</param>
    /// <returns>A list of CopilotPackage objects.</returns>
    public async Task<List<CopilotPackage>> ListPackagesAsync(string? filter = null)
    {
        var allPackages = new List<CopilotPackage>();
        var url = _baseUrl;
        if (!string.IsNullOrWhiteSpace(filter))
        {
            url += $"?$filter={filter}";
        }

        while (!string.IsNullOrEmpty(url))
        {
            using var response = await SendRequestAsync(url);
            if (response is null) break;

            using var doc = await JsonDocument.ParseAsync(response);
            var root = doc.RootElement;

            if (root.TryGetProperty("value", out var valueArray))
            {
                var packages = JsonSerializer.Deserialize<List<CopilotPackage>>(valueArray.GetRawText());
                if (packages is not null)
                {
                    allPackages.AddRange(packages);
                }
            }

            url = root.TryGetProperty("@odata.nextLink", out var nextLink)
                ? nextLink.GetString()
                : null;
        }

        return allPackages;
    }

    /// <summary>
    /// Gets detailed metadata for a specific package by ID.
    /// </summary>
    /// <param name="packageId">The unique package identifier.</param>
    /// <returns>A CopilotPackageDetail object, or null if not found.</returns>
    public async Task<CopilotPackageDetail?> GetPackageDetailAsync(string packageId)
    {
        var url = $"{_baseUrl}/{Uri.EscapeDataString(packageId)}";

        using var response = await SendRequestAsync(url);
        if (response is null) return null;

        return await JsonSerializer.DeserializeAsync<CopilotPackageDetail>(response);
    }

    /// <summary>
    /// Updates the access control properties of a package.
    /// </summary>
    /// <param name="packageId">The unique package identifier.</param>
    /// <param name="allowedUsersAndGroups">Users/groups permitted to access the package.</param>
    /// <param name="acquireUsersAndGroups">Users/groups for whom the package is deployed.</param>
    public async Task UpdatePackageAsync(string packageId, List<PackageAccessEntity>? allowedUsersAndGroups, List<PackageAccessEntity>? acquireUsersAndGroups)
    {
        var url = $"{_baseUrl}/{Uri.EscapeDataString(packageId)}";
        var body = new Dictionary<string, object?>();
        if (allowedUsersAndGroups is not null)
            body["allowedUsersAndGroups"] = allowedUsersAndGroups;
        if (acquireUsersAndGroups is not null)
            body["acquireUsersAndGroups"] = acquireUsersAndGroups;

        var jsonBody = JsonSerializer.Serialize(body);
        await SendRequestWithBodyAsync(url, Method.PATCH, jsonBody);
    }

    /// <summary>
    /// Blocks a package to prevent its usage.
    /// </summary>
    /// <param name="packageId">The unique package identifier.</param>
    public async Task BlockPackageAsync(string packageId)
    {
        var url = $"{_baseUrl}/{Uri.EscapeDataString(packageId)}/block";
        await SendRequestWithBodyAsync(url, Method.POST, null);
    }

    /// <summary>
    /// Unblocks a package to allow its usage.
    /// </summary>
    /// <param name="packageId">The unique package identifier.</param>
    public async Task UnblockPackageAsync(string packageId)
    {
        var url = $"{_baseUrl}/{Uri.EscapeDataString(packageId)}/unblock";
        await SendRequestWithBodyAsync(url, Method.POST, null);
    }

    /// <summary>
    /// Reassigns ownership of a package to a different user.
    /// </summary>
    /// <param name="packageId">The unique package identifier.</param>
    /// <param name="userId">The user ID of the new owner.</param>
    public async Task ReassignPackageAsync(string packageId, string userId)
    {
        var url = $"{_baseUrl}/{Uri.EscapeDataString(packageId)}/reassign";
        var jsonBody = JsonSerializer.Serialize(new { userId });
        await SendRequestWithBodyAsync(url, Method.POST, jsonBody);
    }

    private async Task SendRequestWithBodyAsync(string url, Method method, string? jsonBody)
    {
        var requestInfo = new RequestInformation
        {
            HttpMethod = method,
            URI = new Uri(url)
        };

        if (jsonBody is not null)
        {
            requestInfo.SetStreamContent(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(jsonBody)), "application/json");
        }

        var nativeResponseHandler = new NativeResponseHandler();
        requestInfo.SetResponseHandler(nativeResponseHandler);

        await _graphClient.RequestAdapter.SendPrimitiveAsync<Stream>(requestInfo);

        var httpResponse = nativeResponseHandler.Value as HttpResponseMessage;
        if (httpResponse is null)
            throw new InvalidOperationException("Failed to get HTTP response.");

        if (httpResponse.IsSuccessStatusCode)
            return;

        var errorBody = await httpResponse.Content.ReadAsStringAsync();
        var statusCode = (int)httpResponse.StatusCode;

        throw new InvalidOperationException(
            $"HTTP {statusCode} ({httpResponse.StatusCode}). Response: {errorBody}");
    }

    private async Task<Stream?> SendRequestAsync(string url)
    {
        var requestInfo = new RequestInformation
        {
            HttpMethod = Method.GET,
            URI = new Uri(url)
        };

        var nativeResponseHandler = new NativeResponseHandler();
        requestInfo.SetResponseHandler(nativeResponseHandler);

        await _graphClient.RequestAdapter.SendPrimitiveAsync<Stream>(requestInfo);

        var httpResponse = nativeResponseHandler.Value as HttpResponseMessage;
        if (httpResponse is null)
            throw new InvalidOperationException("Failed to get HTTP response.");

        if (httpResponse.IsSuccessStatusCode)
        {
            return await httpResponse.Content.ReadAsStreamAsync();
        }

        var errorBody = await httpResponse.Content.ReadAsStringAsync();
        var statusCode = (int)httpResponse.StatusCode;

        throw new InvalidOperationException(
            $"HTTP {statusCode} ({httpResponse.StatusCode}). Response: {errorBody}");
    }
}
