using System.Text.Json.Serialization;

namespace PackageManagementAPI_MVC.Models;

/// <summary>
/// Represents a user or group with permissions to access a package.
/// </summary>
public class PackageAccessEntity
{
    [JsonPropertyName("resourceId")]
    public string? ResourceId { get; set; }

    [JsonPropertyName("resourceType")]
    public AccessEntityType? ResourceType { get; set; }
}
