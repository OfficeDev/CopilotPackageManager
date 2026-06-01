using System.Text.Json.Serialization;

namespace PackageManagementAPI_MVC.Models;

/// <summary>
/// Provides details for each element type within a package.
/// </summary>
public class PackageElementDetail
{
    [JsonPropertyName("elementType")]
    public string? ElementType { get; set; }

    [JsonPropertyName("elements")]
    public List<PackageElement>? Elements { get; set; }
}
