# Database Setup Instructions for eShop Application

## Connection String Configured ✅
The App Service has been configured with the following connection string:
- Server: sql-contosouniversity-htinwala-demo.database.windows.net
- Database: eShopCatalogDb
- Authentication: Azure AD Managed Identity

## Required: Grant Managed Identity Permissions

### Execute via Azure Portal Query Editor (Recommended)

1. Open this URL in your browser:
   https://portal.azure.com/#@microsoft.onmicrosoft.com/resource/subscriptions/95642268-5116-484d-9b88-7dfce8c20ce4/resourceGroups/rg-contosoUniversity-v-htinwala-demo/providers/Microsoft.Sql/servers/sql-contosouniversity-htinwala-demo/databases/eShopCatalogDb/overview

2. Click 'Query editor' in the left menu

3. Login with 'Microsoft Entra authentication' using: v-htinwala@microsoft.com

4. Copy and paste this SQL script:

CREATE USER [id-eshop-dev-71vo3l] FROM EXTERNAL PROVIDER;
ALTER ROLE db_datareader ADD MEMBER [id-eshop-dev-71vo3l];
ALTER ROLE db_datawriter ADD MEMBER [id-eshop-dev-71vo3l];
ALTER ROLE db_ddladmin ADD MEMBER [id-eshop-dev-71vo3l];

5. Click 'Run' and verify 'Commands completed successfully'

## After Granting Permissions

Once the permissions are granted, restart the App Service to apply changes:
