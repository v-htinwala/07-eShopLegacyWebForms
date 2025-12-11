# Phase 3: Code Migration - Entra ID Configuration

**Migration Date**: December 10, 2025
**App Name**: DemoEShopLegacyWebForm

---

## Microsoft Entra ID App Registration Details

### Application Registration
- **Display Name**: DemoEShopLegacyWebForm
- **Application (Client) ID**: `a826ab16-8069-42e5-b1d7-91cdeba3770e`
- **Object ID**: `2dc6ce15-04be-4d1f-bf4e-510916ea4226`
- **Directory (Tenant) ID**: `0e478cd4-3e52-496d-ac3a-419ca58ba7ac`
- **Tenant Name**: Microsoft Azure Sponsorship-Factory
- **Publisher Domain**: MngEnvMCAP400868.onmicrosoft.com
- **Sign-in Audience**: AzureADMyOrg (Single tenant)

### Client Secret
- **Secret Value**: `[REDACTED - Store in User Secrets or Azure Key Vault]`
- **Display Name**: DemoEShopClientSecret
- **Created**: December 10, 2025
- ⚠️ **Important**: Store this secret securely in User Secrets (development) or Azure Key Vault (production).

### Redirect URIs (Configured)
- `https://localhost:7001/signin-oidc` (HTTPS - Primary)
- `https://localhost:5001/signin-oidc` (HTTPS - Secondary)

### Token Configuration
- **ID Token Issuance**: Enabled
- **Access Token Issuance**: Disabled
- **Implicit Grant**: ID tokens enabled

### Required Configuration for appsettings.json

```json
{
  "AzureAd": {
    "Instance": "https://login.microsoftonline.com/",
    "Domain": "MngEnvMCAP400868.onmicrosoft.com",
    "TenantId": "0e478cd4-3e52-496d-ac3a-419ca58ba7ac",
    "ClientId": "a826ab16-8069-42e5-b1d7-91cdeba3770e",
    "ClientSecret": "YOUR_CLIENT_SECRET_HERE",
    "CallbackPath": "/signin-oidc",
    "SignedOutCallbackPath": "/signout-callback-oidc"
  }
}
```

### NuGet Packages Required
```xml
<PackageReference Include="Microsoft.Identity.Web" Version="2.15.0" />
<PackageReference Include="Microsoft.Identity.Web.UI" Version="2.15.0" />
```

### Program.cs Configuration
```csharp
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add Microsoft Entra ID authentication
builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"));

// Add authorization
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = options.DefaultPolicy;
});

// Add controllers with views
builder.Services.AddControllersWithViews(options =>
{
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
});

// Add Microsoft Identity UI pages (for sign-in/out)
builder.Services.AddRazorPages()
    .AddMicrosoftIdentityUI();

var app = builder.Build();

// Configure middleware
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
```

---

## Security Best Practices

1. **Never commit secrets to source control**
   - Use Azure Key Vault for production
   - Use User Secrets for development: `dotnet user-secrets init`
   - Store secret: `dotnet user-secrets set "AzureAd:ClientSecret" "YOUR_ACTUAL_SECRET"`

2. **Update Redirect URIs for Production**
   - Add production App Service URL when deployed
   - Format: `https://your-app-name.azurewebsites.net/signin-oidc`

3. **API Permissions** (if needed later)
   - Microsoft Graph: User.Read (for user profile)
   - Add permissions in Azure Portal > App registrations

4. **App Roles** (for authorization)
   - Define roles in app manifest
   - Assign users/groups to roles in Enterprise Applications

---

## Testing Authentication

1. Run the application: `dotnet run`
2. Navigate to `https://localhost:7001`
3. You will be redirected to Microsoft login
4. Sign in with organizational account
5. Consent to permissions (first time only)
6. You will be redirected back to application

---

## Troubleshooting

### Common Issues:
1. **AADSTS50011**: Reply URL mismatch
   - Solution: Verify redirect URI in app registration matches your app

2. **AADSTS700016**: Application not found
   - Solution: Check Client ID and Tenant ID are correct

3. **AADSTS7000215**: Invalid client secret
   - Solution: Verify client secret hasn't expired

### Useful Links:
- [Microsoft Identity Web documentation](https://learn.microsoft.com/entra/identity-platform/microsoft-identity-web)
- [Configure your app in Azure Portal](https://portal.azure.com/#view/Microsoft_AAD_RegisteredApps/ApplicationMenuBlade/~/Overview/appId/a826ab16-8069-42e5-b1d7-91cdeba3770e)

---

**Status**: Entra ID app registration complete ✅
**Next**: Begin project structure creation

