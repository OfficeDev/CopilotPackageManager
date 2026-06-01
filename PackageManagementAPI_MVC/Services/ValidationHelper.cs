using System.Text.Json;
using System.Text.RegularExpressions;
using PackageManagementAPI_MVC.Models;

namespace PackageManagementAPI_MVC.Services;

/// <summary>
/// Provides input validation methods for user-supplied values before calling the API.
/// </summary>
public static partial class ValidationHelper
{
    /// <summary>
    /// Package ID format: one or more uppercase letters, an underscore, then a GUID.
    /// Examples: P_81ac6fae-722c-4ce8-672a-298a9ec2435a, T_15c69db7-37ef-dda3-e4bf-8ccbd834b741
    /// </summary>
    [GeneratedRegex(@"^[A-Z]+_[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$")]
    private static partial Regex PackageIdRegex();

    /// <summary>
    /// Standard GUID format: 8-4-4-4-12 hex characters.
    /// </summary>
    [GeneratedRegex(@"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$")]
    private static partial Regex GuidRegex();

    public static readonly string PackageIdHint =
        "Package ID must be in the format '<Prefix>_<GUID>' (e.g., P_81ac6fae-722c-4ce8-672a-298a9ec2435a or T_15c69db7-37ef-dda3-e4bf-8ccbd834b741).";

    public static readonly string UserIdHint =
        "User ID must be a valid GUID (e.g., a1b2c3d4-e5f6-7890-abcd-ef1234567890). Use the Entra Object ID, not an email address.";

    public static readonly string AccessJsonHint =
        "Access JSON must be a valid JSON array. Example: [{\"resourceType\":\"user\",\"resourceId\":\"<GUID>\"}, {\"resourceType\":\"group\",\"resourceId\":\"<GUID>\"}]";

    /// <summary>
    /// Validates a package ID string.
    /// </summary>
    public static (bool IsValid, string? Error) ValidatePackageId(string? packageId)
    {
        if (string.IsNullOrWhiteSpace(packageId))
            return (false, "Package ID is required.");

        if (!PackageIdRegex().IsMatch(packageId))
            return (false, $"Invalid Package ID format. {PackageIdHint}");

        return (true, null);
    }

    /// <summary>
    /// Validates a user ID (Entra Object ID) string.
    /// </summary>
    public static (bool IsValid, string? Error) ValidateUserId(string? userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return (false, "User ID is required.");

        if (!GuidRegex().IsMatch(userId.Trim()))
            return (false, $"Invalid User ID format. {UserIdHint}");

        return (true, null);
    }

    /// <summary>
    /// Validates an access control JSON string (allowedUsersAndGroups or acquireUsersAndGroups).
    /// Returns the deserialized list if valid.
    /// </summary>
    public static (bool IsValid, string? Error, List<PackageAccessEntity>? Entities) ValidateAccessJson(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return (true, null, null);

        List<PackageAccessEntity>? entities;
        try
        {
            entities = JsonSerializer.Deserialize<List<PackageAccessEntity>>(json);
        }
        catch (JsonException)
        {
            return (false, (string?)$"Invalid JSON format. {AccessJsonHint}", null);
        }

        if (entities is null || entities.Count == 0)
            return (false, (string?)$"Access JSON must contain at least one entry. {AccessJsonHint}", null);

        for (int i = 0; i < entities.Count; i++)
        {
            var entity = entities[i];

            if (entity.ResourceType is null)
                return (false, (string?)$"Entry {i + 1}: 'resourceType' is required and must be 'user' or 'group'.", null);

            if (string.IsNullOrWhiteSpace(entity.ResourceId))
                return (false, (string?)$"Entry {i + 1}: 'resourceId' is required.", null);

            if (!GuidRegex().IsMatch(entity.ResourceId))
                return (false, (string?)$"Entry {i + 1}: 'resourceId' must be a valid GUID (e.g., a1b2c3d4-e5f6-7890-abcd-ef1234567890).", null);
        }

        return (true, null, entities);
    }

    /// <summary>
    /// Validates a date filter string. Must be a valid date that is today or in the past.
    /// </summary>
    public static (bool IsValid, string? Error, DateTime? Date) ValidateDate(string? dateFilter)
    {
        if (string.IsNullOrWhiteSpace(dateFilter))
            return (true, null, null);

        if (!DateTime.TryParse(dateFilter, out var date))
            return (false, (string?)"Invalid date format. Please select a valid date.", null);

        if (date.Date > DateTime.Today)
            return (false, (string?)$"Date cannot be in the future. Please select today ({DateTime.Today:yyyy-MM-dd}) or an earlier date.", null);

        return (true, null, date);
    }
}
