# Azure Infrastructure - Terraform Configuration

This directory contains Terraform infrastructure as code (IaC) for deploying the eShop Modernized application to Azure.

## Architecture

The infrastructure provisions the following Azure resources:

- **Resource Group**: Container for all Azure resources
- **App Service Plan**: Linux-based B1 tier for cost optimization
- **App Service**: .NET 8.0 web application with managed identity
- **SQL Server**: Azure SQL with Entra ID authentication
- **SQL Database**: Basic tier (2GB) for catalog data
- **Key Vault**: Secure storage for secrets with RBAC
- **Application Insights**: Application performance monitoring
- **Log Analytics**: Centralized logging workspace
- **Managed Identity**: User-assigned identity for secure Azure service authentication

## Security Features

- **Managed Identity**: App Service uses managed identity for passwordless authentication to SQL and Key Vault
- **RBAC**: Least privilege access with specific role assignments
- **TLS 1.2**: Enforced minimum TLS version on all services
- **HTTPS Only**: App Service redirects all HTTP traffic to HTTPS
- **Entra ID Authentication**: SQL Server configured for Microsoft Entra ID authentication
- **Key Vault Integration**: Secrets stored in Key Vault with RBAC authorization

## Prerequisites

1. **Azure CLI**: Installed and authenticated (`az login`)
2. **Terraform**: Version >= 1.6.0 installed
3. **Azure Subscription**: Active subscription with appropriate permissions
4. **Entra ID App Registration**: Client ID configured in variables

## Configuration

Key configuration values are in `variables.tf`:

- `subscription_id`: Azure subscription ID (default: 95642268-5116-484d-9b88-7dfce8c20ce4)
- `tenant_id`: Microsoft Entra ID tenant ID (default: 0e478cd4-3e52-496d-ac3a-419ca58ba7ac)
- `location`: Azure region (default: eastus)
- `environment`: Environment name (default: dev)
- `entra_app_client_id`: Entra ID application client ID (default: a826ab16-8069-42e5-b1d7-91cdeba3770e)

## Deployment Steps

### 1. Initialize Terraform

```bash
cd infra
terraform init
```

### 2. Validate Configuration

```bash
terraform validate
```

### 3. Preview Changes

```bash
terraform plan
```

### 4. Deploy Infrastructure

```bash
terraform apply
```

### 5. Update Key Vault Secret

After deployment, update the Entra ID client secret in Key Vault:

```bash
# Get Key Vault name from outputs
$kvName = terraform output -raw key_vault_name

# Set the client secret
az keyvault secret set --vault-name $kvName --name "AzureAd--ClientSecret" --value "YOUR_ACTUAL_CLIENT_SECRET"
```

### 6. Deploy Application Code

Use Azure Developer CLI:

```bash
cd ..
azd deploy
```

Or manually:

```bash
# Build and publish
dotnet publish -c Release -o ./publish

# Deploy to App Service
$appName = terraform output -raw app_service_name
az webapp deployment source config-zip --resource-group $(terraform output -raw resource_group_name) --name $appName --src publish.zip
```

## Outputs

After deployment, Terraform provides these outputs:

- `app_service_url`: Public URL of the deployed application
- `sql_server_fqdn`: SQL Server hostname (sensitive)
- `key_vault_name`: Key Vault name for secret management
- `managed_identity_client_id`: Managed identity client ID

View outputs:

```bash
terraform output
```

## Database Migration

Run EF Core migrations after infrastructure deployment:

```bash
# Update connection string with outputs
$sqlServer = terraform output -raw sql_server_fqdn
$sqlDb = terraform output -raw sql_database_name

# Run migrations (requires Azure CLI authentication)
dotnet ef database update --connection "Server=tcp:$sqlServer,1433;Initial Catalog=$sqlDb;Authentication=Active Directory Default;"
```

## Cost Optimization

Current configuration uses:

- **App Service Plan**: B1 (Basic) - ~$13/month
- **SQL Database**: Basic tier (2GB) - ~$5/month
- **Key Vault**: Standard - Pay per operation
- **Application Insights**: Pay per GB ingested

Estimated monthly cost: ~$20-30 USD

## Cleanup

To destroy all resources:

```bash
terraform destroy
```

## Troubleshooting

### Authentication Issues

Ensure you're logged in with Azure CLI:

```bash
az login --tenant 0e478cd4-3e52-496d-ac3a-419ca58ba7ac
az account set --subscription 95642268-5116-484d-9b88-7dfce8c20ce4
```

### SQL Connection Issues

Verify managed identity has SQL permissions:

```bash
# Check role assignments
az role assignment list --assignee $(terraform output -raw managed_identity_principal_id)
```

### Key Vault Access Issues

Verify RBAC permissions:

```bash
az role assignment list --scope $(terraform output -raw key_vault_id)
```

## Best Practices Implemented

- Infrastructure as Code with Terraform
- Managed identities for passwordless authentication
- RBAC authorization model
- Minimum TLS 1.2 enforcement
- Centralized logging with Log Analytics
- Application performance monitoring
- Resource naming conventions with environment prefix
- Resource tagging for management
- Soft delete on Key Vault (7-day retention)

## Next Steps

1. Deploy infrastructure with `terraform apply`
2. Update Key Vault secret with Entra ID client secret
3. Run database migrations
4. Deploy application code with `azd deploy`
5. Verify application functionality
6. Configure custom domain (optional)
7. Set up CI/CD pipeline (optional)
