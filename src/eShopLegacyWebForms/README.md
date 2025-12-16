# eShop Application - Modernized

This repository contains the modernized version of the eShop application, migrated from ASP.NET Web Forms (.NET Framework 4.7.2) to ASP.NET Core MVC (.NET 8.0).

## 📁 Repository Structure

\\\
07-eShopLegacyWebForms/
├── eShopModernized/              # Main application (ASP.NET Core 8.0)
│   ├── Controllers/              # MVC Controllers
│   ├── Models/                   # Domain models
│   ├── Views/                    # Razor views
│   ├── Data/                     # EF Core DbContext
│   ├── Services/                 # Business logic services
│   ├── wwwroot/                  # Static files
│   ├── infra/                    # Infrastructure as Code (Terraform)
│   ├── appsettings.json          # Application configuration
│   ├── Program.cs                # Application entry point
│   └── eShopModernized.csproj    # Project file
│
├── docs/                         # Documentation
│   ├── reports/                  # Migration reports and status
│   ├── Database-Connection-Verification.md
│   ├── SQL-Setup-Instructions.md
│   └── grant-access.sql
│
├── archive/                      # Legacy code (preserved for reference)
│   └── legacy-webforms/          # Original ASP.NET Web Forms application
│
├── .github/                      # GitHub Actions workflows
│   └── workflows/
│
└── README.md                     # This file
\\\

## 🚀 Quick Start

### Prerequisites
- .NET 8.0 SDK
- Azure CLI
- Azure Developer CLI (azd)
- Azure Subscription

### Local Development

\\\powershell
# Navigate to the application directory
cd eShopModernized

# Restore dependencies
dotnet restore

# Run the application
dotnet run
\\\

The application will be available at:
- HTTPS: https://localhost:7095
- HTTP: http://localhost:5001

### Azure Deployment

\\\powershell
# Navigate to the application directory
cd eShopModernized

# Deploy to Azure
azd deploy
\\\

**Live Application:** https://app-eshop-dev-71vo3l.azurewebsites.net/

## ��️ Infrastructure

The application uses **Terraform** for Infrastructure as Code (IaC):

- **Resource Group:** rg-eshop-dev-71vo3l
- **App Service:** app-eshop-dev-71vo3l (B1 Linux)
- **Database:** Azure SQL Database (eShopCatalogDb)
- **Authentication:** Microsoft Entra ID
- **Monitoring:** Application Insights
- **Security:** Managed Identity, Azure Key Vault

### Infrastructure Deployment

\\\powershell
cd eShopModernized/infra
terraform init
terraform plan
terraform apply
\\\

## 🔐 Authentication

The application uses **Microsoft Entra ID** (Azure AD) for authentication:
- Single Sign-On (SSO) enabled
- OAuth 2.0 / OpenID Connect
- Managed Identity for Azure resources

## 📊 Database

- **Provider:** Azure SQL Database
- **Location:** Central India
- **Server:** sql-contosouniversity-htinwala-demo.database.windows.net
- **Database:** eShopCatalogDb
- **Authentication:** Microsoft Entra ID (Managed Identity)
- **Migrations:** Applied automatically on application startup

## �� CI/CD Pipeline

CI/CD pipelines are configured for:
- ✅ Automated build and test
- ✅ Infrastructure provisioning (Terraform)
- ✅ Application deployment (Azure App Service)
- ✅ Security scanning
- ✅ Quality gates

## 📈 Migration Progress

| Phase | Status | Progress |
|-------|--------|----------|
| Phase 1: Assessment | ✅ Complete | 100% |
| Phase 2: Code Migration | ✅ Complete | 100% |
| Phase 3: Entra ID Setup | ✅ Complete | 100% |
| Phase 4: Infrastructure | ✅ Complete | 100% |
| Phase 5: Deployment | ✅ Complete | 100% |
| Phase 6: CI/CD Setup | 🔄 In Progress | 0% |

**Overall Progress:** 90% Complete

See \docs/reports/Report-Status.md\ for detailed migration status.

## 🛠️ Technology Stack

### Modernized Stack (.NET 8.0)
- **Framework:** ASP.NET Core 8.0 MVC
- **Language:** C# 12
- **Database:** Entity Framework Core 8.0
- **Authentication:** Microsoft.Identity.Web
- **Logging:** ILogger, Application Insights
- **Frontend:** Bootstrap 5, jQuery
- **IaC:** Terraform
- **Cloud:** Azure App Service, Azure SQL Database

### Legacy Stack (.NET Framework 4.7.2)
See \rchive/legacy-webforms/\ for the original implementation.

## 📝 Documentation

- **Migration Reports:** \docs/reports/\
- **Database Setup:** \docs/SQL-Setup-Instructions.md\
- **Deployment Guide:** \docs/Database-Connection-Verification.md\
- **Architecture:** \docs/reports/Application-Assessment-Report.md\

## 🤝 Contributing

This repository follows migration best practices for .NET modernization:
- Maintain clean separation between legacy and modern code
- Document all architectural decisions
- Use Infrastructure as Code for reproducibility
- Implement automated testing and deployment

## 📄 License

This is a demonstration project for .NET modernization and Azure migration patterns.

---

**Last Updated:** December 16, 2025  
**Migration Status:** Phase 6 - CI/CD Setup
