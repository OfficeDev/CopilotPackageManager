using PackageManagementAPI_MVC.Models;
using PackageManagementAPI_MVC.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<PackageService>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    ConfigurationSettings.LoadSettings(config);
    return new PackageService(ConfigurationSettings.GraphClient, ConfigurationSettings.GraphBaseURL);
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Packages/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Packages}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
