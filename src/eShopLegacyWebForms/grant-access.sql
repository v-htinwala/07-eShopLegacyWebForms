CREATE USER [id-eshop-dev-71vo3l] FROM EXTERNAL PROVIDER;
ALTER ROLE db_datareader ADD MEMBER [id-eshop-dev-71vo3l];
ALTER ROLE db_datawriter ADD MEMBER [id-eshop-dev-71vo3l];
ALTER ROLE db_ddladmin ADD MEMBER [id-eshop-dev-71vo3l];
