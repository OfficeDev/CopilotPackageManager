using System.Text.Json.Serialization;

namespace PackageManagementAPI_MVC.Models;

/// <summary>
/// Represents a Copilot package in the tenant — returned by the List endpoint.
/// </summary>
public class CopilotPackage
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("displayName")]
    public string? DisplayName { get; set; }

    [JsonPropertyName("type")]
    public PackageType? Type { get; set; }

    [JsonPropertyName("shortDescription")]
    public string? ShortDescription { get; set; }

    [JsonPropertyName("isBlocked")]
    public bool? IsBlocked { get; set; }

    [JsonPropertyName("availableTo")]
    public PackageStatus? AvailableTo { get; set; }

    [JsonPropertyName("deployedTo")]
    public PackageStatus? DeployedTo { get; set; }

    [JsonPropertyName("lastModifiedDateTime")]
    public DateTimeOffset? LastModifiedDateTime { get; set; }

    [JsonPropertyName("supportedHosts")]
    public List<string>? SupportedHosts { get; set; }

    [JsonPropertyName("elementTypes")]
    public List<string>? ElementTypes { get; set; }

    [JsonPropertyName("publisher")]
    public string? Publisher { get; set; }

    [JsonPropertyName("platform")]
    public string? Platform { get; set; }

    [JsonPropertyName("version")]
    public string? Version { get; set; }

    [JsonPropertyName("manifestVersion")]
    public string? ManifestVersion { get; set; }

    [JsonPropertyName("manifestId")]
    public string? ManifestId { get; set; }

    [JsonPropertyName("appId")]
    public string? AppId { get; set; }

    [JsonPropertyName("assetId")]
    public string? AssetId { get; set; }
}
