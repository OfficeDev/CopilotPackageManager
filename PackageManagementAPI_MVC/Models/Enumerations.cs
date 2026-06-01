using System.Text.Json.Serialization;

namespace PackageManagementAPI_MVC.Models;

/// <summary>
/// Classification of a Copilot package.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<PackageType>))]
public enum PackageType
{
    microsoft,
    external,
    firstParty,
    thirdParty,
    shared,
    custom,
    lob,
    sideloaded,
    unknownFutureValue
}

/// <summary>
/// Availability or deployment scope of a package.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<PackageStatus>))]
public enum PackageStatus
{
    none,
    some,
    all,
    allowedForAll,
    allowedForSome,
    allowedForNone,
    acquiredForAll,
    acquiredForSome,
    acquiredForNone,
    unknownFutureValue
}

/// <summary>
/// Type of access entity (user or group).
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<AccessEntityType>))]
public enum AccessEntityType
{
    user,
    group,
    unknownFutureValue
}
