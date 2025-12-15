# Generate random suffix for unique resource names
resource "random_string" "suffix" {
  length  = 6
  special = false
  upper   = false
}

locals {
  resource_suffix = random_string.suffix.result
  common_tags     = merge(var.tags, {
    Environment = var.environment
  })
}

# Resource Group
resource "azurerm_resource_group" "main" {
  name     = "rg-${var.project_name}-${var.environment}-${local.resource_suffix}"
  location = var.location
  tags     = local.common_tags
}

# Log Analytics Workspace
resource "azurerm_log_analytics_workspace" "main" {
  name                = "log-${var.project_name}-${var.environment}-${local.resource_suffix}"
  location            = azurerm_resource_group.main.location
  resource_group_name = azurerm_resource_group.main.name
  sku                 = "PerGB2018"
  retention_in_days   = 30
  tags                = local.common_tags
}

# Application Insights
resource "azurerm_application_insights" "main" {
  name                = "appi-${var.project_name}-${var.environment}-${local.resource_suffix}"
  location            = azurerm_resource_group.main.location
  resource_group_name = azurerm_resource_group.main.name
  workspace_id        = azurerm_log_analytics_workspace.main.id
  application_type    = "web"
  tags                = local.common_tags
}

# User-Assigned Managed Identity
resource "azurerm_user_assigned_identity" "app" {
  name                = "id-${var.project_name}-${var.environment}-${local.resource_suffix}"
  location            = azurerm_resource_group.main.location
  resource_group_name = azurerm_resource_group.main.name
  tags                = local.common_tags
}

# Key Vault
resource "azurerm_key_vault" "main" {
  name                       = "kv-${var.project_name}-${var.environment}-${local.resource_suffix}"
  location                   = azurerm_resource_group.main.location
  resource_group_name        = azurerm_resource_group.main.name
  tenant_id                  = var.tenant_id
  sku_name                   = "standard"
  soft_delete_retention_days = 7
  purge_protection_enabled   = false
  rbac_authorization_enabled = true
  tags                       = local.common_tags
}

# SQL Server with Entra ID Authentication
# TEMPORARILY DISABLED - Blocked by subscription policy
# resource "azurerm_mssql_server" "main" {
#   name                         = "sql-${var.project_name}-${var.environment}-${local.resource_suffix}"
#   location                     = azurerm_resource_group.main.location
#   resource_group_name          = azurerm_resource_group.main.name
#   version                      = "12.0"
#   minimum_tls_version          = "1.2"
#   public_network_access_enabled = true
#   administrator_login          = "sqladmin"
#   administrator_login_password = "P@ssw0rd123!TempOnly"
#   
#   azuread_administrator {
#     login_username              = azurerm_user_assigned_identity.app.name
#     object_id                   = azurerm_user_assigned_identity.app.principal_id
#     azuread_authentication_only = false
#   }
#
#   identity {
#     type         = "UserAssigned"
#     identity_ids = [azurerm_user_assigned_identity.app.id]
#   }
#
#   tags = local.common_tags
# }

# SQL Database
# TEMPORARILY DISABLED - Depends on SQL Server
# resource "azurerm_mssql_database" "main" {
#   name                        = "sqldb-${var.project_name}-${var.environment}"
#   server_id                   = azurerm_mssql_server.main.id
#   collation                   = "SQL_Latin1_General_CP1_CI_AS"
#   sku_name                    = var.sql_sku_name
#   max_size_gb                 = var.sql_max_size_gb
#   zone_redundant              = false
#   
#   tags = local.common_tags
# }

# SQL Firewall Rule - Allow Azure Services
# TEMPORARILY DISABLED - Depends on SQL Server
# resource "azurerm_mssql_firewall_rule" "allow_azure" {
#   name             = "AllowAzureServices"
#   server_id        = azurerm_mssql_server.main.id
#   start_ip_address = "0.0.0.0"
#   end_ip_address   = "0.0.0.0"
# }

# App Service Plan
resource "azurerm_service_plan" "main" {
  name                = "asp-${var.project_name}-${var.environment}-${local.resource_suffix}"
  location            = azurerm_resource_group.main.location
  resource_group_name = azurerm_resource_group.main.name
  os_type             = "Linux"
  sku_name            = var.app_service_sku.size
  tags                = local.common_tags
}

# App Service
resource "azurerm_linux_web_app" "main" {
  name                = "app-${var.project_name}-${var.environment}-${local.resource_suffix}"
  location            = azurerm_resource_group.main.location
  resource_group_name = azurerm_resource_group.main.name
  service_plan_id     = azurerm_service_plan.main.id
  https_only          = true

  identity {
    type         = "UserAssigned"
    identity_ids = [azurerm_user_assigned_identity.app.id]
  }

  site_config {
    always_on                         = true
    ftps_state                        = "Disabled"
    http2_enabled                     = true
    minimum_tls_version               = "1.2"
    use_32_bit_worker                 = false
    
    application_stack {
      dotnet_version = "8.0"
    }
  }

  app_settings = {
    "APPLICATIONINSIGHTS_CONNECTION_STRING" = azurerm_application_insights.main.connection_string
    "ApplicationInsightsAgent_EXTENSION_VERSION" = "~3"
    "AzureAd__Instance"                    = "https://login.microsoftonline.com/"
    "AzureAd__TenantId"                    = var.tenant_id
    "AzureAd__ClientId"                    = var.entra_app_client_id
    "AzureAd__CallbackPath"                = "/signin-oidc"
    "AZURE_CLIENT_ID"                      = azurerm_user_assigned_identity.app.client_id
  }

  # Connection string temporarily disabled - SQL Server blocked by policy
  # connection_string {
  #   name  = "CatalogConnection"
  #   type  = "SQLAzure"
  #   value = "Server=tcp:${azurerm_mssql_server.main.fully_qualified_domain_name},1433;Initial Catalog=${azurerm_mssql_database.main.name};Authentication=Active Directory Default;"
  # }

  logs {
    application_logs {
      file_system_level = "Information"
    }
    http_logs {
      file_system {
        retention_in_days = 7
        retention_in_mb   = 35
      }
    }
  }

  tags = merge(local.common_tags, {
    "azd-service-name" = "web"
  })
}

# RBAC: Key Vault Secrets User
resource "azurerm_role_assignment" "keyvault_secrets_user" {
  scope                = azurerm_key_vault.main.id
  role_definition_name = "Key Vault Secrets User"
  principal_id         = azurerm_user_assigned_identity.app.principal_id
}

# RBAC: SQL DB Contributor
# TEMPORARILY DISABLED - Depends on SQL Server
# resource "azurerm_role_assignment" "sql_db_contributor" {
#   scope                = azurerm_mssql_server.main.id
#   role_definition_name = "SQL DB Contributor"
#   principal_id         = azurerm_user_assigned_identity.app.principal_id
# }

# Store Entra ID Client Secret in Key Vault (to be set manually)
# TEMPORARILY DISABLED - RBAC permissions need time to propagate
# resource "azurerm_key_vault_secret" "client_secret" {
#   name         = "AzureAd--ClientSecret"
#   value        = "placeholder-update-after-deployment"
#   key_vault_id = azurerm_key_vault.main.id
#   
#   depends_on = [azurerm_role_assignment.keyvault_secrets_user]
#   
#   tags = local.common_tags
# }
