# Core Configuration
variable "subscription_id" {
  description = "Azure subscription ID"
  type        = string
  default     = "95642268-5116-484d-9b88-7dfce8c20ce4"
}

variable "tenant_id" {
  description = "Azure AD tenant ID"
  type        = string
  default     = "0e478cd4-3e52-496d-ac3a-419ca58ba7ac"
}

variable "location" {
  description = "Azure region for resources"
  type        = string
  default     = "eastus"
}

variable "environment" {
  description = "Environment name (dev, staging, prod)"
  type        = string
  default     = "dev"
}

variable "project_name" {
  description = "Project name used for resource naming"
  type        = string
  default     = "eshop"
}

# App Service Configuration
variable "app_service_sku" {
  description = "App Service Plan SKU"
  type        = object({
    tier = string
    size = string
  })
  default = {
    tier = "Basic"
    size = "B1"
  }
}

# Database Configuration
variable "sql_admin_username" {
  description = "SQL Server administrator username"
  type        = string
  default     = "sqladmin"
  sensitive   = true
}

variable "sql_sku_name" {
  description = "SQL Database SKU"
  type        = string
  default     = "Basic"
}

variable "sql_max_size_gb" {
  description = "Maximum database size in GB"
  type        = number
  default     = 2
}

# Entra ID Configuration
variable "entra_app_client_id" {
  description = "Microsoft Entra ID application client ID"
  type        = string
  default     = "a826ab16-8069-42e5-b1d7-91cdeba3770e"
}

# Networking Configuration
variable "enable_private_endpoints" {
  description = "Enable private endpoints for resources"
  type        = bool
  default     = false
}

# Tags
variable "tags" {
  description = "Tags to apply to all resources"
  type        = map(string)
  default     = {
    Project     = "eShopLegacyMigration"
    Environment = "Development"
    ManagedBy   = "Terraform"
  }
}
