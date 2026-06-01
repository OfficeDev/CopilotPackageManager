using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PackageManagementAPI_MVC.Models;
using PackageManagementAPI_MVC.Services;

namespace PackageManagementAPI_MVC.Controllers;

public class PackagesController : Controller
{
    private readonly PackageService _packageService;

    public PackagesController(PackageService packageService)
    {
        _packageService = packageService;
    }

    public IActionResult Index()
    {
        return RedirectToAction("ListView");
    }

    public IActionResult ListView()
    {
        return View();
    }

    public IActionResult Unblock()
    {
        return View();
    }

    // AJAX: Fetch all packages (optionally filtered)
    [HttpPost]
    public async Task<IActionResult> FetchPackages(string? hostFilter, string? elementFilter, string? dateFilter)
    {
        try
        {
            string? filter = null;

            if (!string.IsNullOrWhiteSpace(dateFilter))
            {
                var (dateValid, dateError, date) = ValidationHelper.ValidateDate(dateFilter);
                if (!dateValid)
                    return Json(new { success = false, error = dateError });
                if (date.HasValue)
                    filter = FilterHelper.ByLastModifiedAfter(new DateTimeOffset(date.Value, TimeSpan.Zero));
            }
            else if (!string.IsNullOrWhiteSpace(elementFilter) && elementFilter != "All")
                filter = FilterHelper.ByElementType(elementFilter);
            else if (!string.IsNullOrWhiteSpace(hostFilter) && hostFilter != "All")
                filter = FilterHelper.BySupportedHost(hostFilter);

            var packages = await _packageService.ListPackagesAsync(filter);
            return Json(new { success = true, count = packages.Count, packages });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, error = ex.Message });
        }
    }

    // AJAX: Get package details
    [HttpPost]
    public async Task<IActionResult> GetDetails(string packageId)
    {
        var (isValid, error) = ValidationHelper.ValidatePackageId(packageId);
        if (!isValid)
            return Json(new { success = false, error });

        try
        {
            var detail = await _packageService.GetPackageDetailAsync(packageId);
            if (detail is null)
                return Json(new { success = false, error = "Package not found." });
            return Json(new { success = true, package = detail });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, error = ex.Message });
        }
    }

    // AJAX: Block package
    [HttpPost]
    public async Task<IActionResult> BlockPackage(string packageId)
    {
        var (isValid, error) = ValidationHelper.ValidatePackageId(packageId);
        if (!isValid)
            return Json(new { success = false, error });

        try
        {
            await _packageService.BlockPackageAsync(packageId);
            return Json(new { success = true, message = $"Package '{packageId}' blocked." });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, error = ex.Message });
        }
    }

    // AJAX: Unblock package
    [HttpPost]
    public async Task<IActionResult> UnblockPackage(string packageId)
    {
        var (isValid, error) = ValidationHelper.ValidatePackageId(packageId);
        if (!isValid)
            return Json(new { success = false, error });

        try
        {
            await _packageService.UnblockPackageAsync(packageId);
            return Json(new { success = true, message = $"Package '{packageId}' unblocked." });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, error = ex.Message });
        }
    }

    // AJAX: Reassign package
    [HttpPost]
    public async Task<IActionResult> ReassignPackage(string packageId, string userId)
    {
        var (pkgValid, pkgError) = ValidationHelper.ValidatePackageId(packageId);
        if (!pkgValid)
            return Json(new { success = false, error = pkgError });

        var (userValid, userError) = ValidationHelper.ValidateUserId(userId);
        if (!userValid)
            return Json(new { success = false, error = userError });

        try
        {
            await _packageService.ReassignPackageAsync(packageId, userId);
            return Json(new { success = true, message = $"Package '{packageId}' reassigned to '{userId}'." });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, error = ex.Message });
        }
    }

    // AJAX: Update access control
    [HttpPost]
    public async Task<IActionResult> UpdateAccess(string packageId, string? allowedJson, string? acquiredJson)
    {
        var (pkgValid, pkgError) = ValidationHelper.ValidatePackageId(packageId);
        if (!pkgValid)
            return Json(new { success = false, error = pkgError });

        var (allowedValid, allowedError, allowed) = ValidationHelper.ValidateAccessJson(allowedJson);
        if (!allowedValid)
            return Json(new { success = false, error = $"Allowed Users & Groups: {allowedError}" });

        var (acquiredValid, acquiredError, acquired) = ValidationHelper.ValidateAccessJson(acquiredJson);
        if (!acquiredValid)
            return Json(new { success = false, error = $"Acquired Users & Groups: {acquiredError}" });

        try
        {
            await _packageService.UpdatePackageAsync(packageId, allowed, acquired);
            return Json(new { success = true, message = $"Package '{packageId}' access updated." });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, error = ex.Message });
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
