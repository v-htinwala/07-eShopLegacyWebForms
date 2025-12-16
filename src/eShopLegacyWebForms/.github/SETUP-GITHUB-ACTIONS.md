# GitHub Actions Setup Guide

This guide will help you configure GitHub Secrets for the CI/CD pipeline.

## Prerequisites
- Azure CLI installed and authenticated
- Owner or Contributor access to the Azure subscription
- Admin access to the GitHub repository

## Step 1: Create Azure Service Principal

Run the following command to create a service principal with Contributor role:

```powershell
# Set variables
$subscriptionId = "95642268-5116-484d-9b88-7dfce8c20ce4"
$resourceGroup = "rg-eshop-dev-71vo3l"
$appName = "gh-actions-eshop"

# Create service principal
az ad sp create-for-rbac `
  --name $appName `
  --role Contributor `
  --scopes "/subscriptions/$subscriptionId/resourceGroups/$resourceGroup" `
  --sdk-auth

# This will output JSON credentials - save this output for the next step
```

## Step 2: Configure GitHub Secrets

Go to your GitHub repository: https://github.com/v-htinwala/07-eShopLegacyWebForms

Navigate to: **Settings** → **Secrets and variables** → **Actions** → **New repository secret**

Create the following secrets:

### AZURE_CREDENTIALS
Paste the entire JSON output from Step 1. It should look like:
```json
{
  "clientId": "xxx",
  "clientSecret": "xxx",
  "subscriptionId": "95642268-5116-484d-9b88-7dfce8c20ce4",
  "tenantId": "0e478cd4-3e52-496d-ac3a-419ca58ba7ac",
  "activeDirectoryEndpointUrl": "https://login.microsoftonline.com",
  "resourceManagerEndpointUrl": "https://management.azure.com/",
  "activeDirectoryGraphResourceId": "https://graph.windows.net/",
  "sqlManagementEndpointUrl": "https://management.core.windows.net:8443/",
  "galleryEndpointUrl": "https://gallery.azure.com/",
  "managementEndpointUrl": "https://management.core.windows.net/"
}
```

### Additional Secrets for Terraform (Optional)
If using Terraform workflows, also create:

- **ARM_CLIENT_ID**: The clientId from the JSON above
- **ARM_CLIENT_SECRET**: The clientSecret from the JSON above
- **ARM_SUBSCRIPTION_ID**: `95642268-5116-484d-9b88-7dfce8c20ce4`
- **ARM_TENANT_ID**: `0e478cd4-3e52-496d-ac3a-419ca58ba7ac`

## Step 3: Configure GitHub Environments

1. Go to **Settings** → **Environments**
2. Create the following environments:

### staging
- No protection rules needed for initial setup
- Add environment secret if needed for staging-specific configs

### production
- Enable "Required reviewers" and add yourself
- Enable "Wait timer" (optional): 5 minutes
- Add environment secret if needed for production-specific configs

### infrastructure-production
- Enable "Required reviewers" and add yourself
- This protects infrastructure changes

## Step 4: Verify Pipeline Configuration

The following workflows have been created:

1. **CI/CD Pipeline** (`.github/workflows/ci-cd.yml`)
   - Builds and tests the application
   - Deploys to staging automatically on push to `code-remediation`
   - Deploys to production with approval

2. **Infrastructure Deployment** (`.github/workflows/infrastructure.yml`)
   - Validates Terraform configurations
   - Runs security scans (tfsec, Checkov)
   - Deploys infrastructure changes with approval

3. **Pull Request Validation** (`.github/workflows/pr-validation.yml`)
   - Validates code quality on PRs
   - Runs tests and dependency reviews

4. **Dependency Updates** (`.github/workflows/dependency-updates.yml`)
   - Automatically checks for outdated packages weekly
   - Creates PRs for dependency updates

## Step 5: Test the Pipeline

After setting up secrets:

1. Make a small change to your application code
2. Commit and push to the `code-remediation` branch
3. Check the "Actions" tab to see the pipeline running

## Troubleshooting

### Pipeline fails with authentication error
- Verify AZURE_CREDENTIALS secret is set correctly
- Ensure service principal has Contributor role

### Terraform workflow fails
- Verify all ARM_* secrets are set
- Check Terraform backend configuration

### Deployment fails
- Verify App Service name matches in workflow
- Check Azure portal for any resource locks

## Security Best Practices

✅ Service principal uses least-privilege access (scoped to resource group)
✅ Secrets stored in GitHub Secrets (encrypted at rest)
✅ Production deployments require manual approval
✅ Infrastructure changes require approval
✅ Security scanning enabled in pipelines

## Next Steps

1. Set up branch protection rules for `main` and `code-remediation`
2. Configure status checks to require successful builds
3. Set up Slack/Teams notifications for deployment events
4. Configure Application Insights alerts
5. Set up Azure Monitor dashboards

## Resources

- [GitHub Actions Documentation](https://docs.github.com/en/actions)
- [Azure Deploy Action](https://github.com/Azure/webapps-deploy)
- [Azure Login Action](https://github.com/Azure/login)
- [Terraform GitHub Actions](https://learn.hashicorp.com/tutorials/terraform/github-actions)
