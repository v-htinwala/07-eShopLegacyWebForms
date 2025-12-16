# Application Assessment Report
## eShopLegacyWebForms Migration to Azure

---

## Executive Summary

This document provides a comprehensive assessment of the eShopLegacyWebForms application for migration from .NET Framework 4.7.2 to .NET 8.0 LTS and deployment to Azure App Service with Microsoft Entra ID authentication.

**Assessment Date**: December 10, 2025  
**Project**: eShopLegacyWebForms  
**Migration Complexity**: **High**  
**Estimated Effort**: 6-8 weeks (depending on team size and experience)

---

## 1. Application Overview

### 1.1 Current State
- **Application Type**: ASP.NET Web Forms
- **Framework Version**: .NET Framework 4.7.2
- **Target Framework**: NET 4.6.1 (configured in Web.config)
- **Database**: SQL Server LocalDB (Entity Framework 6.2.0)
- **Authentication**: Forms Authentication (legacy)
- **Hosting**: IIS Express (local development)

### 1.2 Application Features
- Product catalog management (CRUD operations)
- Product browsing and viewing
- Session-based user tracking
- Application Insights telemetry
- Responsive design with Bootstrap

### 1.3 Key Technologies in Use

#### Backend
- ASP.NET Web Forms 4.7.2
- Entity Framework 6.2.0
- Autofac 4.9.1 (Dependency Injection)
- log4net 2.0.10 (Logging)
- Microsoft.ApplicationInsights 2.9.1

#### Frontend
- Bootstrap 4.3.1
- jQuery 3.3.1
- ASP.NET AJAX
- WebForms ScriptManager

#### Data Access
- Entity Framework 6 Code First
- SQL Server LocalDB
- System.Data.SqlClient provider

---

## 2. Target Architecture

### 2.1 Azure Services

| Component | Current | Target | Rationale |
|-----------|---------|--------|-----------|
| **Compute** | IIS Express | Azure App Service (Windows, .NET 8) | PaaS solution, managed infrastructure, auto-scaling |
| **Database** | SQL Server LocalDB | Azure SQL Database | Fully managed, compatible with SQL Server, built-in HA/DR |
| **Authentication** | Forms Auth | Microsoft Entra ID | Enterprise SSO, MFA, conditional access |
| **Monitoring** | Application Insights | Azure Application Insights | Already in use, enhanced Azure integration |
| **IaC** | None | Terraform | Infrastructure as Code, version control, repeatability |
| **CI/CD** | Manual | Azure DevOps / GitHub Actions | Automated deployments, quality gates |

### 2.2 Application Architecture Changes

#### Current: Monolithic Web Forms
```
┌─────────────────────────────────────┐
│   ASP.NET Web Forms Application     │
│  (.NET Framework 4.7.2)              │
│                                      │
│  ┌────────────┐  ┌────────────┐     │
│  │  .aspx     │  │  .aspx.cs  │     │
│  │  Pages     │  │  Code-     │     │
│  │            │  │  Behind     │     │
│  └────────────┘  └────────────┘     │
│                                      │
│  ┌─────────────────────────┐        │
│  │   Entity Framework 6    │        │
│  │   (System.Data.Entity)  │        │
│  └─────────────────────────┘        │
│                                      │
│  ┌─────────────────────────┐        │
│  │   SQL Server LocalDB    │        │
│  └─────────────────────────┘        │
└─────────────────────────────────────┘
```

#### Target: Modern .NET 8 on Azure
```
┌───────────────────────────────────────────┐
│   Azure App Service                       │
│   (.NET 8 - Razor Pages/Blazor)           │
│                                            │
│  ┌──────────────┐  ┌──────────────┐       │
│  │  Razor Pages │  │  Blazor      │       │
│  │  / MVC       │  │  Components  │       │
│  └──────────────┘  └──────────────┘       │
│                                            │
│  ┌─────────────────────────────────┐      │
│  │  Entity Framework Core 8        │      │
│  │  (Microsoft.EntityFrameworkCore)│      │
│  └─────────────────────────────────┘      │
│           ↓                                │
└───────────┼────────────────────────────────┘
            ↓
  ┌─────────────────────┐
  │  Azure SQL Database │
  └─────────────────────┘

┌──────────────────────┐
│  Microsoft Entra ID  │
│  (Authentication)    │
└──────────────────────┘
```

---

## 3. Migration Assessment

### 3.1 Web Forms Migration Strategy

**Challenge**: ASP.NET Web Forms is not supported in .NET Core/.NET 5+

**Options**:

#### Option A: Migrate to Razor Pages (RECOMMENDED)
- **Effort**: Medium to High
- **Benefits**: 
  - Similar page-based model to Web Forms
  - Native .NET 8 support
  - Modern development patterns
  - Better testability
- **Migration Path**: Convert .aspx pages to .cshtml Razor Pages

#### Option B: Migrate to Blazor Server
- **Effort**: High
- **Benefits**:
  - Rich interactive UI
  - Component-based architecture
  - Real-time updates via SignalR
- **Considerations**: Requires more significant architectural changes

#### Option C: Migrate to MVC
- **Effort**: High
- **Benefits**:
  - Full control over HTML
  - Mature pattern
- **Considerations**: Different paradigm from page-based approach

**Recommendation**: **Razor Pages** for quickest migration path while modernizing the codebase.

### 3.2 Page Migration Matrix

| Current Page | Type | Target | Complexity | Notes |
|--------------|------|--------|------------|-------|
| Default.aspx | List View | Index.cshtml | Medium | Convert GridView to HTML table/list |
| About.aspx | Static | About.cshtml | Low | Simple content page |
| Contact.aspx | Static | Contact.cshtml | Low | Simple content page |
| Catalog/Create.aspx | Form | Catalog/Create.cshtml | Medium | Convert server controls to Tag Helpers |
| Catalog/Edit.aspx | Form | Catalog/Edit.cshtml | Medium | Convert server controls to Tag Helpers |
| Catalog/Delete.aspx | Form | Catalog/Delete.cshtml | Medium | Confirmation page |
| Catalog/Details.aspx | Detail View | Catalog/Details.cshtml | Low | Read-only display |
| Site.Master | Master Page | _Layout.cshtml | Medium | Convert to Razor layout |

### 3.3 Entity Framework Migration

**Current**: Entity Framework 6.2.0  
**Target**: Entity Framework Core 8.0

#### Breaking Changes to Address:
1. **DbContext Configuration**: Move from App.config to code-based configuration
2. **Connection Strings**: Migrate to appsettings.json
3. **Lazy Loading**: Different implementation in EF Core (requires proxies package)
4. **Database Initializers**: Not supported in EF Core (use migrations instead)
5. **ObjectContext APIs**: Replace with DbContext APIs
6. **SQL Server Provider**: Change from `System.Data.SqlClient` to `Microsoft.Data.SqlClient`

#### Migration Steps:
1. Install EF Core packages:
   - `Microsoft.EntityFrameworkCore`
   - `Microsoft.EntityFrameworkCore.SqlServer`
   - `Microsoft.EntityFrameworkCore.Tools`
2. Update DbContext inheritance and configuration
3. Update LINQ queries (some syntax differences)
4. Create initial migration from existing database
5. Test data access layer thoroughly

### 3.4 Dependency Analysis

#### Dependencies Requiring Replacement

| Current Package | Version | Target Package | Notes |
|-----------------|---------|----------------|-------|
| Autofac | 4.9.1 | Microsoft.Extensions.DependencyInjection | Built-in DI in .NET 8 |
| Autofac.Integration.Web | 4.0.0 | (Remove) | Web Forms specific |
| log4net | 2.0.10 | Microsoft.Extensions.Logging | Built-in logging framework |
| EntityFramework | 6.2.0 | Microsoft.EntityFrameworkCore | Version 8.0 |
| System.Data.SqlClient | - | Microsoft.Data.SqlClient | Modern SQL client |
| Microsoft.AspNet.FriendlyUrls | 1.0.2 | (Remove) | Use routing in Razor Pages |
| WebGrease | 1.6.0 | (Remove) | Modern bundling not needed |
| Antlr3.Runtime | 3.5.0.2 | (Remove) | Dependency of WebGrease |

#### Dependencies to Update

| Current Package | Current Version | Target Version | Compatibility |
|-----------------|-----------------|----------------|---------------|
| Microsoft.ApplicationInsights | 2.9.1 | Latest (3.x) | ✅ Compatible |
| Microsoft.ApplicationInsights.Web | 2.9.1 | .AspNetCore variant | ✅ Compatible |
| Newtonsoft.Json | 12.0.0 | 13.x or System.Text.Json | ✅ Compatible |
| Bootstrap | 4.3.1 | 5.3.x | ✅ Compatible (minor changes) |
| jQuery | 3.3.1 | 3.7.x | ✅ Compatible |

### 3.5 Authentication Migration

**Current**: Forms Authentication (legacy)  
**Target**: Microsoft Entra ID

#### Migration Requirements:
1. **Azure AD App Registration**
   - Create app registration in Entra ID
   - Configure redirect URIs
   - Set up API permissions
   - Generate client secrets

2. **Code Changes**
   - Install `Microsoft.Identity.Web` packages
   - Configure authentication in `Program.cs`
   - Update authorization attributes
   - Implement sign-in/sign-out flows
   - Handle user claims

3. **Session Management**
   - Migrate from InProc sessions to distributed cache
   - Consider Azure Cache for Redis for session state
   - Update session tracking logic

#### Authentication Flow:
```
User → Azure App Service → Microsoft Entra ID → Token → App Service → Application
```

### 3.6 Configuration Migration

**Current**: Web.config (XML-based)  
**Target**: appsettings.json (JSON-based)

#### Items to Migrate:
- ✅ Connection strings
- ✅ Application settings (UseMockData, UseCustomizationData)
- ✅ Session state configuration
- ✅ Compilation settings (move to .csproj)
- ✅ HTTP modules/handlers (migrate to middleware)
- ✅ Assembly bindings (mostly unnecessary in .NET 8)

---

## 4. Infrastructure as Code (Terraform)

### 4.1 Required Azure Resources

```hcl
# Resource Group
# App Service Plan (Windows, .NET 8)
# App Service (Web App)
# Azure SQL Server
# Azure SQL Database
# Application Insights
# Key Vault (for secrets)
# Azure Cache for Redis (for session state)
# Microsoft Entra ID App Registration
# Managed Identity for App Service
```

### 4.2 Terraform Module Structure

```
terraform/
├── main.tf                 # Main configuration
├── variables.tf            # Input variables
├── outputs.tf              # Output values
├── provider.tf             # Azure provider config
├── modules/
│   ├── app-service/       # App Service module
│   ├── sql-database/      # SQL Database module
│   ├── networking/        # VNet, subnets, NSG
│   ├── monitoring/        # App Insights, Log Analytics
│   └── security/          # Key Vault, Managed Identity
└── environments/
    ├── dev.tfvars
    ├── staging.tfvars
    └── prod.tfvars
```

### 4.3 Key Terraform Configurations

#### App Service
- SKU: S1 (Standard) minimum for production
- Always On: Enabled
- .NET version: 8.0
- Platform: Windows
- Managed Identity: System-assigned
- App Settings: Connection strings, Entra ID config

#### Azure SQL Database
- Service Tier: Standard S2 or higher
- Backup retention: 35 days
- Geo-replication: Enabled for production
- Firewall: Allow Azure services
- Private endpoint (optional for enhanced security)

#### Entra ID Integration
- Managed Identity for passwordless connections
- RBAC roles for SQL Database access
- Key Vault access policies

---

## 5. Migration Complexity Assessment

### 5.1 Complexity Matrix

| Component | Complexity | Effort | Risk |
|-----------|-----------|--------|------|
| Web Forms → Razor Pages | **High** | 4-5 weeks | High |
| EF6 → EF Core 8 | **Medium** | 1-2 weeks | Medium |
| Autofac → MS DI | **Low** | 3-5 days | Low |
| log4net → MS Logging | **Low** | 2-3 days | Low |
| Forms Auth → Entra ID | **High** | 1-2 weeks | Medium |
| Configuration Migration | **Low** | 2-3 days | Low |
| Terraform Setup | **Medium** | 1 week | Low |
| Testing & Validation | **Medium** | 2-3 weeks | Medium |

### 5.2 Code Impact Analysis

```
Estimated Lines of Code to Change/Rewrite:
- ASPX Pages: ~500-800 lines → Razor Pages
- Code-Behind: ~1000-1500 lines → Page Models
- Master Pages: ~200-300 lines → Layouts
- Entity Framework: ~500 lines → Minimal changes
- Configuration: ~200 lines → New format
- Authentication: ~300-400 lines → Complete rewrite

Total Estimated Impact: 2,700-3,700 lines of code
```

### 5.3 Risk Assessment

#### High Risks
1. **Functional Parity**: Ensuring all Web Forms features work in Razor Pages
2. **Data Loss**: Database migration must preserve all data
3. **Authentication Issues**: Entra ID integration bugs could block access
4. **Performance Degradation**: Must maintain or improve performance

#### Medium Risks
1. **Third-party Dependencies**: Some packages may not have .NET 8 versions
2. **Testing Coverage**: Insufficient testing could lead to production issues
3. **Session State**: Different session management approach

#### Low Risks
1. **Frontend Compatibility**: Bootstrap/jQuery are stable
2. **Database Compatibility**: SQL Server → Azure SQL is straightforward
3. **Monitoring**: Application Insights already in use

---

## 6. Migration Phases

### Phase 1: Planning & Assessment ✅ COMPLETE
- Select hosting platform
- Choose IaC tool
- Define target architecture
- Complete this assessment

### Phase 2: Development Environment Setup
**Duration**: 1 week

Tasks:
- Set up .NET 8 SDK
- Install EF Core tools
- Create new .NET 8 project structure
- Set up local SQL Server database
- Configure development Entra ID tenant

### Phase 3: Code Migration
**Duration**: 4-5 weeks

#### Sprint 1: Foundation (1 week)
- Create new .NET 8 Razor Pages project
- Migrate models and data context to EF Core
- Set up dependency injection
- Implement logging infrastructure
- Migrate configuration files

#### Sprint 2: Core Pages (2 weeks)
- Migrate layout and master pages
- Convert Default.aspx, About.aspx, Contact.aspx
- Implement routing
- Migrate shared components
- Update CSS and JavaScript

#### Sprint 3: Catalog Features (1.5 weeks)
- Migrate Catalog/Create
- Migrate Catalog/Edit
- Migrate Catalog/Delete
- Migrate Catalog/Details
- Implement data validation

#### Sprint 4: Authentication (1 week)
- Implement Entra ID authentication
- Configure authorization policies
- Migrate user session logic
- Update UI for authentication

### Phase 4: Infrastructure Setup
**Duration**: 1 week

Tasks:
- Create Terraform configurations
- Provision Azure resources (dev environment)
- Configure networking and security
- Set up Key Vault and secrets
- Configure Application Insights
- Set up SQL Database and firewall rules

### Phase 5: Testing
**Duration**: 2-3 weeks

Test Categories:
- Unit tests for business logic
- Integration tests for data access
- End-to-end functional tests
- Authentication and authorization tests
- Performance testing
- Security testing
- Browser compatibility testing

### Phase 6: Deployment Preparation
**Duration**: 1 week

Tasks:
- Set up CI/CD pipeline
- Configure deployment slots (staging/production)
- Database migration scripts
- Rollback procedures
- Monitoring and alerting
- Documentation

### Phase 7: Deployment & Validation
**Duration**: 1 week

Tasks:
- Deploy to staging
- Staging validation
- Production deployment
- Post-deployment validation
- Performance monitoring
- User acceptance testing

---

## 7. Recommended Approach

### 7.1 Strangler Fig Pattern (Alternative)

For large applications, consider a **strangler fig** approach:
1. Deploy existing app to Azure App Service (Windows, .NET Framework)
2. Incrementally migrate pages to .NET 8
3. Use routing to direct traffic to new/old pages
4. Gradually replace all pages
5. Decommission old application

**Benefits**:
- Lower risk
- Incremental value delivery
- Easier rollback
- Continuous operation

### 7.2 Big Bang Migration (Current Plan)

Complete migration before going live:
- Higher initial effort
- All-or-nothing deployment
- Cleaner final architecture
- Better for smaller applications

**Recommendation**: Given the application size, **Big Bang** is feasible and recommended.

---

## 8. Prerequisites & Requirements

### 8.1 Development Tools
- ✅ Visual Studio 2022 (17.8 or later) or VS Code
- ✅ .NET 8 SDK
- ✅ Azure CLI
- ✅ Terraform CLI
- ✅ SQL Server Management Studio or Azure Data Studio
- ✅ Git

### 8.2 Azure Subscription Requirements
- Active Azure subscription
- Contributor or Owner role
- Sufficient quota for resources:
  - App Service Plan (S1 or higher)
  - Azure SQL Database
  - Application Insights
  - Key Vault

### 8.3 Microsoft Entra ID
- Access to Entra ID tenant
- Permission to create app registrations
- Ability to grant admin consent

### 8.4 Skills & Knowledge
- .NET 8 development
- Razor Pages or Blazor
- Entity Framework Core
- Azure services
- Terraform
- Microsoft Entra ID authentication

---

## 9. Cost Estimation

### 9.1 Azure Monthly Costs (Estimate)

| Service | SKU | Estimated Cost (USD) |
|---------|-----|----------------------|
| App Service | S1 Standard | ~$70 |
| Azure SQL Database | S2 Standard (50 DTU) | ~$75 |
| Application Insights | Pay-as-you-go | ~$5-20 |
| Key Vault | Standard | ~$0.03 |
| Azure Cache for Redis | Basic C0 | ~$16 |
| **Total** | | **~$166-181/month** |

**Notes**:
- Costs vary by region
- Can be optimized with reserved instances
- Development/staging environments add to costs
- Traffic and storage costs not included

### 9.2 Migration Project Costs

| Category | Estimated Hours | Rate (example) | Cost |
|----------|----------------|----------------|------|
| Development | 320-400 | $100/hr | $32,000-40,000 |
| Testing | 80-120 | $80/hr | $6,400-9,600 |
| DevOps/Infrastructure | 40-60 | $120/hr | $4,800-7,200 |
| Project Management | 40-60 | $100/hr | $4,000-6,000 |
| **Total** | | | **$47,200-62,800** |

---

## 10. Success Metrics

### 10.1 Technical Metrics
- ✅ All pages migrated to Razor Pages
- ✅ Zero data loss during database migration
- ✅ 100% feature parity with legacy application
- ✅ Page load time ≤ legacy application
- ✅ 99.9% uptime SLA
- ✅ All security scans pass (OWASP Top 10)

### 10.2 Business Metrics
- ✅ Migration completed within estimated timeline
- ✅ No critical production issues in first 30 days
- ✅ User satisfaction maintained or improved
- ✅ Reduced maintenance costs (legacy dependencies removed)

---

## 11. Next Steps

### Immediate Actions:
1. ✅ **Review this assessment** with stakeholders
2. ✅ **Approve migration approach** (Big Bang vs. Strangler Fig)
3. ✅ **Allocate resources** (developers, budget, Azure subscription)
4. ✅ **Set up development environment**
5. ✅ **Begin Phase 2: Assessment**

### Command to Continue:
```
/phase2-assessproject
```

This will initiate the detailed code assessment phase, which will:
- Analyze code compatibility with .NET 8
- Identify specific breaking changes
- Generate detailed remediation tasks
- Create migration work items

---

## 12. Appendices

### Appendix A: Useful Resources
- [Migrate ASP.NET Web Forms to ASP.NET Core](https://docs.microsoft.com/aspnet/core/migration/proper-to-2x/)
- [Entity Framework Core vs EF6](https://docs.microsoft.com/ef/efcore-and-ef6/)
- [Microsoft Identity Platform](https://docs.microsoft.com/azure/active-directory/develop/)
- [Azure App Service Documentation](https://docs.microsoft.com/azure/app-service/)
- [Terraform Azure Provider](https://registry.terraform.io/providers/hashicorp/azurerm/latest)

### Appendix B: Project Structure Comparison

**Current (.NET Framework 4.7.2)**
```
eShopLegacyWebForms/
├── *.aspx (pages)
├── *.aspx.cs (code-behind)
├── Site.Master (layout)
├── Web.config (configuration)
├── Global.asax (app events)
├── Models/ (EF6 models)
├── Services/ (business logic)
└── bin/ (compiled assemblies)
```

**Target (.NET 8)**
```
eShopLegacyWebForms/
├── Program.cs (entry point)
├── appsettings.json (configuration)
├── Pages/ (Razor Pages)
│   ├── Index.cshtml
│   ├── About.cshtml
│   ├── Catalog/
│   │   ├── Index.cshtml
│   │   ├── Create.cshtml
│   │   ├── Edit.cshtml
│   │   └── Details.cshtml
│   └── Shared/
│       └── _Layout.cshtml
├── Models/ (EF Core entities)
├── Data/ (DbContext)
├── Services/ (business logic)
└── wwwroot/ (static files)
```

### Appendix C: Database Migration Checklist
- [ ] Export current database schema
- [ ] Create EF Core initial migration
- [ ] Compare schemas for differences
- [ ] Test data migration on copy
- [ ] Create rollback scripts
- [ ] Validate data integrity
- [ ] Update connection strings
- [ ] Configure Azure SQL firewall
- [ ] Test application connectivity
- [ ] Monitor performance

---

**Document Version**: 1.0  
**Last Updated**: December 10, 2025  
**Status**: Ready for Phase 2 Assessment

---
