using System.Text.Json.Serialization;

namespace PackageManagementAPI_MVC.Models;

/// <summary>
/// Represents a single element within a package.
/// </summary>
public class PackageElement
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("definition")]
    public string? Definition { get; set; }
}
