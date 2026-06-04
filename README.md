# Copilot Package Management API – Web Application

A .NET 9 ASP.NET Core MVC web application that provides an interactive UI for managing Microsoft 365 Copilot packages using the [Microsoft Graph Beta – Package Management APIs](https://learn.microsoft.com/en-us/microsoft-365/copilot/extensibility/api/admin-settings/package/overview).
A .NET 9 ASP.NET Core MVC web application that provides an interactive UI for IT administrators to view and manage agents and apps (packages) across Microsoft 365 using the [Microsoft Graph Beta – Package Management APIs](https://learn.microsoft.com/en-us/microsoft-365/copilot/extensibility/api/admin-settings/package/overview).

> A **package** represents either an agent or a Microsoft 365 app in the organization catalog.

---

## Features

| Feature | Description |
|---------|-------------|
| **List Packages** | Retrieve all Copilot packages with automatic pagination. |
| **Filter** | Filter by supported host, element type, or last modified date (one filter at a time — API limitation). |
| **View Details** | Inspect package metadata: title, description, publisher, supported hosts, and element types. |
| **Block / Unblock** | Block a package to prevent usage, or unblock it to restore availability. A dedicated Unblock page is available for packages that may no longer be visible in the list after blocking. |
| **Reassign** | Reassign ownership of shared Copilot Studio agents to a different user. |
| **Update Access** | Modify allowed and acquired users/groups for a package through a user-friendly form. |

---

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- A **Microsoft Agent 365 license** — required to access the Package Management API
- A **Microsoft Entra ID** app registration with:
  - An **Application (client) ID**
  - A **Directory (tenant) ID**
  - Delegated API permission: **`CopilotPackages.Read.All`** (used for listing and reading packages)
  - For write operations (block, unblock, reassign, update access), additional permissions such as **`CopilotPackages.ReadWrite.All`** may be required — refer to the [API documentation](https://learn.microsoft.com/en-us/microsoft-365/copilot/extensibility/api/admin-settings/package/overview) for the latest permission requirements
  - Redirect URI set to **`http://localhost`** (required for the interactive browser sign-in flow)
  - The signed-in user must have a **Microsoft 365 admin role** with access to Copilot package management
- The signed-in user must be an **IT administrator** with access to manage Copilot packages across the organization

> The app uses **interactive browser authentication** — a browser window will open at startup for you to sign in with your Entra account.

---

## Configuration

Open `PackageManagementAPI_MVC/appsettings.json` and replace the placeholders with your Entra app registration values:

```json
{
  "CLIENT_ID": "<YOUR_CLIENT_ID>",
  "TENANT_ID": "<YOUR_TENANT_ID>",
  "GRAPH_BASE_URL": "https://graph.microsoft.com/beta/copilot/admin/catalog/packages"
}
```

> **Important:** Never commit real credentials. Keep `CLIENT_ID` and `TENANT_ID` as placeholders in source control.

---

## Getting Started

1. **Clone** the repository:
   ```bash
   git clone https://github.com/OfficeDev/CopilotPackageManager.git
   ```

2. **Configure** – Update `appsettings.json` with your `CLIENT_ID` and `TENANT_ID`.

3. **Run** the application:
   ```bash
   dotnet run --project PackageManagementAPI_MVC
   ```

4. **Sign in** – A browser window will open for Entra authentication. After sign-in, the app launches at `https://localhost:<port>`.

---

## Project Structure

```
PackageManagementAPI_MVC/
├── Auth/
│   └── AuthProvider.cs              # Interactive browser authentication and Graph client creation
├── Controllers/
│   └── PackagesController.cs        # MVC controller with AJAX endpoints for all package operations
├── Models/
│   ├── ConfigurationSettings.cs     # Loads app config and initializes the Graph client at startup
│   ├── CopilotPackage.cs            # Model for package list items
│   ├── CopilotPackageDetail.cs      # Model for detailed package info
│   ├── Enumerations.cs              # Enums for package type, status, host, and element types
│   ├── ErrorViewModel.cs            # Standard error view model
│   ├── PackageAccessEntity.cs       # Model for user/group access entries
│   ├── PackageElement.cs            # Model for package element summaries
│   └── PackageElementDetail.cs      # Model for detailed element info
├── Services/
│   ├── FilterHelper.cs              # Builds OData $filter strings for API queries
│   ├── PackageService.cs            # Core service calling Graph Beta package management endpoints
│   └── ValidationHelper.cs          # Input validation for package IDs, user IDs, dates, and access data
├── Views/
│   ├── Packages/
│   │   ├── ListView.cshtml          # Main interactive admin console UI
│   │   └── Unblock.cshtml           # Standalone unblock page for blocked packages
│   └── Shared/
│       ├── _Layout.cshtml           # Shared layout with navigation
│       └── Error.cshtml             # Error page
├── wwwroot/                         # Static assets (CSS, JS, images)
├── appsettings.json                 # App configuration (CLIENT_ID, TENANT_ID, GRAPH_BASE_URL)
└── Program.cs                       # Application startup and dependency injection
```

---

## How It Works

1. At startup, `Program.cs` loads configuration from `appsettings.json` and creates an authenticated `GraphServiceClient` using interactive browser sign-in.
2. The `PackageService` uses the Graph client and the configured base URL to call the Graph Beta package management endpoints via raw HTTP requests.
3. The `PackagesController` exposes AJAX endpoints that the `ListView.cshtml` UI calls via jQuery `$.post` requests.
4. All user inputs (package IDs, user IDs, dates, access entries) are validated using `ValidationHelper` before any API call is made.

### Application Walkthrough



https://github.com/user-attachments/assets/76321c93-d29c-4aa8-88e7-57746071b491

This video demonstrates the end-to-end functionality of the application, including package retrieval, detail view, management actions, and filtering capabilities.

---

## API Limitations
## API Reference

| Operation | HTTP Method | Endpoint |
|-----------|-------------|----------|
| List packages | `GET` | `/copilot/admin/catalog/packages` |
| Get package details | `GET` | `/copilot/admin/catalog/packages/{id}` |
| Update package | `PATCH` | `/copilot/admin/catalog/packages/{id}` |
| Block | `POST` | `/copilot/admin/catalog/packages/{id}/block` |
| Unblock | `POST` | `/copilot/admin/catalog/packages/{id}/unblock` |
| Reassign | `POST` | `/copilot/admin/catalog/packages/{id}/reassign` |

For full details, see the [Package Management API overview](https://learn.microsoft.com/en-us/microsoft-365/copilot/extensibility/api/admin-settings/package/overview).

---

## Known Limitations

- The Graph Beta package management API supports **only one `$filter` per request**. The UI enforces mutual exclusion among filter options.
- **Reassign** is restricted to shared agents created in Copilot Studio.
- **Update Access** sends a success response from the API, but actual propagation may vary depending on the package type.
- These APIs are in **preview** and behavior may change.

---

## License

See [LICENSE](LICENSE) for details.
