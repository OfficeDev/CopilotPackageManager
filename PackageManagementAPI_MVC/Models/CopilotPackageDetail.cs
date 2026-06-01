using System.Text.Json.Serialization;

namespace PackageManagementAPI_MVC.Models;

/// <summary>
/// Inherits from CopilotPackage and adds detailed metadata — returned by the Get Detail endpoint.
/// </summary>
public class CopilotPackageDetail : CopilotPackage
{
    [JsonPropertyName("longDescription")]
    public string? LongDescription { get; set; }

    [JsonPropertyName("categories")]
    public List<string>? Categories { get; set; }

    [JsonPropertyName("sensitivity")]
    public string? Sensitivity { get; set; }

    [JsonPropertyName("allowedUsersAndGroups")]
    public List<PackageAccessEntity>? AllowedUsersAndGroups { get; set; }

    [JsonPropertyName("acquireUsersAndGroups")]
    public List<PackageAccessEntity>? AcquireUsersAndGroups { get; set; }

    [JsonPropertyName("elementDetails")]
    public List<PackageElementDetail>? ElementDetails { get; set; }
}
