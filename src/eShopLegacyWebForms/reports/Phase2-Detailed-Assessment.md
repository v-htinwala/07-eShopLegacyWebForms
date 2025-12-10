# Phase 2: Detailed Code Assessment Report
**eShopLegacyWebForms Migration to ASP.NET Core MVC**

---

**Assessment Date**: December 10, 2025 16:54:35  
**Framework**: ASP.NET Web Forms 4.7.2 → ASP.NET Core 8.0 MVC  
**Hosting**: Azure App Service  
**Database**: SQL Server LocalDB → Azure SQL Database  
**Authentication**: Forms Auth → Microsoft Entra ID  
**IaC**: Terraform

---

## Executive Summary

**Migration Complexity**: HIGH  
**Estimated Duration**: 6-8 weeks  
**Code Impact**: ~3,500-4,500 lines  
**Risk Level**: Medium-High

The application is a product catalog manager built with ASP.NET Web Forms. Migration to ASP.NET Core MVC requires complete rewrite of UI layer while preserving business logic.

---

## 1. Application Architecture Analysis

### Current Architecture
```
┌─────────────────────────────────────────┐
│  ASP.NET Web Forms (.NET Framework)     │
├─────────────────────────────────────────┤
│  Presentation Layer                      │
│  - 7 .aspx pages + code-behind          │
│  - Site.Master layout                    │
│  - Server controls (ListView, TextBox)  │
│  - ViewState management                  │
├─────────────────────────────────────────┤
│  Business Layer                          │
│  - ICatalogService interface            │
│  - CatalogService implementation         │
├─────────────────────────────────────────┤
│  Data Access Layer                       │
│  - Entity Framework 6.2.0               │
│  - CatalogDBContext                      │
│  - LINQ queries                          │
├─────────────────────────────────────────┤
│  Dependency Injection                    │
│  - Autofac 4.9.1                        │
│  - Web Forms integration                 │
└─────────────────────────────────────────┘
```

### Target Architecture
```
┌─────────────────────────────────────────┐
│  ASP.NET Core 8.0 MVC                   │
├─────────────────────────────────────────┤
│  Presentation Layer                      │
│  - Controllers (HomeController, etc)    │
│  - Views (.cshtml Razor views)          │
│  - Tag Helpers                           │
│  - Model binding                         │
├─────────────────────────────────────────┤
│  Business Layer (Reusable)              │
│  - ICatalogService interface ✓          │
│  - CatalogService implementation ✓      │
├─────────────────────────────────────────┤
│  Data Access Layer                       │
│  - Entity Framework Core 8.0            │
│  - CatalogDbContext (updated)           │
│  - Async/await patterns                  │
├─────────────────────────────────────────┤
│  Dependency Injection                    │
│  - Microsoft.Extensions.DI (built-in)   │
│  - Program.cs configuration              │
└─────────────────────────────────────────┘
```

---

## 2. Page Inventory & Migration Strategy

### Pages to Migrate (Web Forms → MVC)

| Current Page | Type | Target MVC | Controller | Action | Complexity |
|--------------|------|-----------|------------|--------|------------|
| Default.aspx | List | Index.cshtml | HomeController | Index | HIGH |
| About.aspx | Static | About.cshtml | HomeController | About | LOW |
| Contact.aspx | Static | Contact.cshtml | HomeController | Contact | LOW |
| Catalog/Create.aspx | Form | Create.cshtml | CatalogController | Create (GET/POST) | MEDIUM |
| Catalog/Edit.aspx | Form | Edit.cshtml | CatalogController | Edit (GET/POST) | MEDIUM |
| Catalog/Details.aspx | View | Details.cshtml | CatalogController | Details | LOW |
| Catalog/Delete.aspx | Form | Delete.cshtml | CatalogController | Delete (GET/POST) | MEDIUM |
| Site.Master | Layout | _Layout.cshtml | Shared | N/A | MEDIUM |

**Total**: 7 pages + 1 master → 2 controllers + 8 views

---

## 3. Breaking Changes & Migration Requirements

### 3.1 Web Forms → MVC Conversion

#### Page Model vs Controller/View
**Current (Web Forms)**:
- Page_Load event handlers
- ViewState for state management
- Server-side control events
- Postback model

**Target (MVC)**:
- Controller actions (GET/POST)
- Model binding
- Client-side state management
- RESTful URLs

#### Server Controls → HTML Helpers/Tag Helpers
- `<asp:ListView>` → `@foreach` loop with HTML
- `<asp:TextBox>` → `<input asp-for="Model.Property">`
- `<asp:Button>` → `<button type="submit">`
- `<asp:RequiredFieldValidator>` → Data Annotations + Client validation

### 3.2 Entity Framework 6 → EF Core 8

**Breaking Changes**:
1. **DbContext Configuration**: No more constructors with connection string names
2. **Lazy Loading**: Requires explicit configuration or .Include()
3. **Database Initializers**: Removed (use Migrations)
4. **Provider**: `System.Data.SqlClient` → `Microsoft.Data.SqlClient`

**Migration Steps**:
```csharp
// OLD (EF6)
public CatalogDBContext() : base("name=CatalogDBContext") { }

// NEW (EF Core 8)
public CatalogDbContext(DbContextOptions<CatalogDbContext> options) : base(options) { }
```

### 3.3 Dependency Injection

**Autofac → Microsoft.Extensions.DependencyInjection**

**Current**:
```csharp
var builder = new ContainerBuilder();
builder.RegisterType<CatalogService>().As<ICatalogService>();
container = builder.Build();
```

**Target**:
```csharp
builder.Services.AddScoped<ICatalogService, CatalogService>();
builder.Services.AddDbContext<CatalogDbContext>(options =>
    options.UseSqlServer(connectionString));
```

### 3.4 Configuration

**Web.config → appsettings.json**

**Current**:
```xml
<connectionStrings>
  <add name="CatalogDBContext" connectionString="..." />
</connectionStrings>
<appSettings>
  <add key="UseMockData" value="true" />
</appSettings>
```

**Target**:
```json
{
  "ConnectionStrings": {
    "CatalogDbContext": "..."
  },
  "AppSettings": {
    "UseMockData": true
  }
}
```

---

## 4. Detailed File Migration Plan

### Phase 1: Infrastructure (Week 1)
- [ ] Create new ASP.NET Core 8.0 MVC project
- [ ] Set up Program.cs with DI configuration
- [ ] Migrate appsettings.json
- [ ] Install NuGet packages (EF Core, Identity, etc.)
- [ ] Create folder structure (Controllers, Views, Models, Data, Services)

### Phase 2: Data Layer (Week 1-2)
- [ ] Migrate CatalogItem, CatalogBrand, CatalogType models (minimal changes)
- [ ] Convert CatalogDBContext to CatalogDbContext (EF Core)
- [ ] Update LINQ queries to async patterns
- [ ] Create EF Core migrations
- [ ] Test database connectivity

### Phase 3: Business Layer (Week 2)
- [ ] Migrate ICatalogService interface (add async methods)
- [ ] Update CatalogService implementation
- [ ] Add async/await patterns
- [ ] Migrate CatalogServiceMock
- [ ] Update PaginatedItemsViewModel

### Phase 4: Controllers (Week 3)
- [ ] Create HomeController (Index, About, Contact)
- [ ] Create CatalogController (Create, Edit, Delete, Details)
- [ ] Implement GET/POST action methods
- [ ] Add model validation
- [ ] Configure routing

### Phase 5: Views (Week 3-4)
- [ ] Create _Layout.cshtml
- [ ] Migrate Index view (product list with pagination)
- [ ] Migrate Create view (form with validation)
- [ ] Migrate Edit view
- [ ] Migrate Details view
- [ ] Migrate Delete view (confirmation)
- [ ] Create About and Contact views

### Phase 6: Authentication (Week 5)
- [ ] Install Microsoft.Identity.Web packages
- [ ] Configure Entra ID in appsettings.json
- [ ] Update Program.cs with authentication
- [ ] Add [Authorize] attributes
- [ ] Create login/logout views
- [ ] Test authentication flow

### Phase 7: Styling & Assets (Week 5)
- [ ] Migrate CSS files to wwwroot/css
- [ ] Migrate images to wwwroot/images
- [ ] Update Bootstrap to version 5.3
- [ ] Update jQuery references
- [ ] Remove Web Forms-specific scripts

### Phase 8: Testing (Week 6)
- [ ] Unit tests for services
- [ ] Integration tests for controllers
- [ ] End-to-end testing
- [ ] Performance testing
- [ ] Security testing

---

## 5. Code Examples - Key Migrations

### Example 1: Default.aspx → HomeController/Index

**Before (Default.aspx.cs)**:
```csharp
public partial class _Default : Page
{
    public ICatalogService CatalogService { get; set; }
    protected PaginatedItemsViewModel<CatalogItem> Model { get; set; }
    
    protected void Page_Load(object sender, EventArgs e)
    {
        var size = Convert.ToInt32(Page.RouteData.Values["size"]);
        var index = Convert.ToInt32(Page.RouteData.Values["index"]);
        Model = CatalogService.GetCatalogItemsPaginated(size, index);
        productList.DataSource = Model.Data;
        productList.DataBind();
    }
}
```

**After (HomeController.cs)**:
```csharp
public class HomeController : Controller
{
    private readonly ICatalogService _catalogService;
    
    public HomeController(ICatalogService catalogService)
    {
        _catalogService = catalogService;
    }
    
    public async Task<IActionResult> Index(int? pageSize, int? pageIndex)
    {
        int size = pageSize ?? 10;
        int index = pageIndex ?? 0;
        var model = await _catalogService.GetCatalogItemsPaginatedAsync(size, index);
        return View(model);
    }
}
```

### Example 2: Create.aspx → CatalogController/Create

**Before (Create.aspx.cs)**:
```csharp
protected void Create_Click(object sender, EventArgs e)
{
    if (this.ModelState.IsValid)
    {
        var catalogItem = new CatalogItem
        {
            Name = Name.Text,
            Price = decimal.Parse(Price.Text),
            // ...
        };
        CatalogService.CreateCatalogItem(catalogItem);
        Response.Redirect("~");
    }
}
```

**After (CatalogController.cs)**:
```csharp
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create(CatalogItem catalogItem)
{
    if (ModelState.IsValid)
    {
        await _catalogService.CreateCatalogItemAsync(catalogItem);
        return RedirectToAction(nameof(Index), "Home");
    }
    await LoadDropdownsAsync();
    return View(catalogItem);
}
```

---

## 6. Dependency Compatibility Matrix

| Package | Current | Target | Status | Notes |
|---------|---------|--------|--------|-------|
| **Framework** | .NET Framework 4.7.2 | .NET 8.0 | ⚠️ Incompatible | Complete rewrite required |
| **Entity Framework** | 6.2.0 | EF Core 8.0 | ⚠️ Breaking | API changes required |
| **Autofac** | 4.9.1 | Microsoft.Extensions.DI | ⚠️ Replace | Use built-in DI |
| **log4net** | 2.0.10 | Microsoft.Extensions.Logging | ⚠️ Replace | Use ILogger<T> |
| **Application Insights** | 2.9.1 | 3.x (AspNetCore) | ✅ Compatible | Update package |
| **Bootstrap** | 4.3.1 | 5.3.x | ✅ Compatible | Minor CSS changes |
| **jQuery** | 3.5.0 | 3.7.x | ✅ Compatible | No changes |
| **Newtonsoft.Json** | 12.0.0 | System.Text.Json | ⚠️ Optional | Prefer built-in |

---

## 7. Authentication Migration Plan

### Current: Forms Authentication (Basic)
- No authentication currently implemented
- Session tracking via Session["MachineName"]

### Target: Microsoft Entra ID

**Step 1**: Register application in Azure Portal
- Create App Registration
- Configure Redirect URIs
- Generate client secret
- Set API permissions

**Step 2**: Update appsettings.json
```json
{
  "AzureAd": {
    "Instance": "https://login.microsoftonline.com/",
    "Domain": "yourdomain.onmicrosoft.com",
    "TenantId": "your-tenant-id",
    "ClientId": "your-client-id",
    "CallbackPath": "/signin-oidc"
  }
}
```

**Step 3**: Configure authentication in Program.cs
```csharp
builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"));

builder.Services.AddAuthorization();
app.UseAuthentication();
app.UseAuthorization();
```

**Step 4**: Protect controllers
```csharp
[Authorize]
public class CatalogController : Controller { }
```

---

## 8. Risk Assessment

### HIGH RISKS
| Risk | Impact | Mitigation |
|------|--------|------------|
| Feature parity loss | High | Detailed testing matrix |
| Data migration issues | Critical | Database backup + rollback plan |
| Authentication bugs | High | Staged rollout with fallback |
| Performance regression | Medium | Load testing + optimization |

### MEDIUM RISKS
| Risk | Impact | Mitigation |
|------|--------|------------|
| Third-party dependency issues | Medium | Early compatibility testing |
| Learning curve (MVC) | Medium | Training + documentation |
| Session state management | Medium | Distributed cache (Redis) |

### LOW RISKS
| Risk | Impact | Mitigation |
|------|--------|------------|
| CSS/JS compatibility | Low | Browser testing |
| Database schema changes | Low | EF Core migrations handle this |

---

## 9. Testing Strategy

### Unit Tests
- Service layer (CatalogService methods)
- Model validation
- Business logic

### Integration Tests
- Controller actions
- Database operations
- Authentication flows

### End-to-End Tests
- User workflows (Create → Edit → Delete)
- Pagination
- Form validation
- Authentication

### Performance Tests
- Page load times (< 2s)
- Database query performance
- Concurrent user handling (100+ users)

---

## 10. Terraform Infrastructure

### Required Azure Resources
```hcl
# Resource Group
# App Service Plan (Windows, B1 or S1)
# App Service (Web App)
# Azure SQL Server
# Azure SQL Database (Basic or S0)
# Application Insights
# Key Vault
# Entra ID App Registration (manual)
```

### Estimated Monthly Cost
- App Service (S1): ~$70
- Azure SQL (S0): ~$15
- Application Insights: ~$5-10
- Key Vault: ~$0.03
**Total**: ~$90-95/month (dev/test)

---

## Change Report

### Required Code Changes

#### CHANGE-001: Migrate to ASP.NET Core MVC Project Structure
**Objective**: Create modern .NET 8.0 MVC application structure  
**Refactor**: Create new project with proper folder structure  
**Documentation**: https://learn.microsoft.com/aspnet/core/mvc/overview  
**Constraints**: Must preserve all business logic  
**Status**: Ready to implement

#### CHANGE-002: Convert Web Forms Pages to MVC Controllers/Views
**Objective**: Modernize presentation layer  
**Refactor**: Convert 7 .aspx pages to controller actions + Razor views  
**Documentation**: https://learn.microsoft.com/aspnet/core/tutorials/first-mvc-app/  
**Constraints**: Maintain UI/UX consistency  
**Status**: Ready to implement

#### CHANGE-003: Upgrade Entity Framework 6 to EF Core 8
**Objective**: Use modern ORM with better performance  
**Refactor**: Update DbContext, add async methods, use migrations  
**Documentation**: https://learn.microsoft.com/ef/core/what-is-new/ef-core-8.0/  
**Breaking Changes**: Constructor changes, provider changes, API differences  
**Status**: Ready to implement

#### CHANGE-004: Replace Autofac with Microsoft.Extensions.DependencyInjection
**Objective**: Use built-in DI container  
**Refactor**: Update service registration in Program.cs  
**Documentation**: https://learn.microsoft.com/aspnet/core/fundamentals/dependency-injection  
**Constraints**: Must maintain service lifetimes  
**Status**: Ready to implement

#### CHANGE-005: Migrate Configuration (Web.config → appsettings.json)
**Objective**: Use modern configuration system  
**Refactor**: Convert XML to JSON, update configuration access  
**Documentation**: https://learn.microsoft.com/aspnet/core/fundamentals/configuration/  
**Status**: Ready to implement

#### CHANGE-006: Implement Microsoft Entra ID Authentication
**Objective**: Enterprise-grade authentication with SSO  
**Refactor**: Add Microsoft.Identity.Web, configure authentication  
**Documentation**: https://learn.microsoft.com/entra/identity-platform/  
**Constraints**: Requires Azure AD tenant setup  
**Status**: Requires Azure resource provisioning first

#### CHANGE-007: Replace log4net with Microsoft.Extensions.Logging
**Objective**: Use built-in logging framework  
**Refactor**: Replace LogManager.GetLogger with ILogger<T>  
**Documentation**: https://learn.microsoft.com/aspnet/core/fundamentals/logging/  
**Status**: Ready to implement

#### CHANGE-008: Update Application Insights Integration
**Objective**: Enhanced monitoring for ASP.NET Core  
**Refactor**: Install AspNetCore package, configure in Program.cs  
**Documentation**: https://learn.microsoft.com/azure/azure-monitor/app/asp-net-core  
**Status**: Ready to implement

#### CHANGE-009: Modernize Frontend (Bootstrap 4 → 5)
**Objective**: Use latest Bootstrap version  
**Refactor**: Update CDN links, fix CSS class names  
**Documentation**: https://getbootstrap.com/docs/5.3/migration/  
**Constraints**: Minor breaking changes in class names  
**Status**: Ready to implement

#### CHANGE-010: Add Async/Await Patterns
**Objective**: Improve scalability and performance  
**Refactor**: Convert synchronous methods to async throughout  
**Documentation**: https://learn.microsoft.com/dotnet/csharp/async  
**Constraints**: Must test thoroughly for deadlocks  
**Status**: Ready to implement

---

## Next Steps

**Ready to proceed with Phase 3: Code Migration**

Run the following command to start the migration:
```
/phase3-migratecode
```

This will begin the automated migration process, creating the new ASP.NET Core MVC project structure and migrating code in phases.

---

**Report Generated**: December 10, 2025 16:54:35  
**Status**: Phase 2 Assessment Complete ✅  
**Next Phase**: Code Migration
