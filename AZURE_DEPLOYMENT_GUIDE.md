# 🚀 Azure App Service Deployment Guide for LocalShop

## 📋 Prerequisites

1. **Azure Account**: You need an active Azure subscription
2. **Azure CLI**: Install from [Microsoft Docs](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli)
3. **Published Project**: Your project has been published to the `./publish` folder

## 🔧 Step-by-Step Deployment

### 1. Install and Login to Azure CLI

```bash
# Install Azure CLI (if not already installed)
# Download from: https://docs.microsoft.com/en-us/cli/azure/install-azure-cli

# Login to Azure
az login
```

### 2. Update Configuration Files

#### Update `appsettings.Production.json`:
```json
{
  "ConnectionStrings": {
    "AzureSqlConnection": "Server=tcp:YOUR-AZURE-SQL-SERVER.database.windows.net,1433;Initial Catalog=LocalShop;Persist Security Info=False;User ID=YOUR-USERNAME;Password=YOUR-PASSWORD;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
  },
  "Jwt": {
    "Key": "YOUR-SECURE-JWT-KEY-HERE-MAKE-IT-LONG-AND-SECURE",
    "Issuer": "LocalShop",
    "Audience": "yourAppUsers",
    "ExpiryHours": 24
  }
}
```

#### Update `deploy-to-azure.ps1`:
```powershell
$resourceGroupName = "LocalShop-RG"           # Your resource group name
$appServicePlanName = "LocalShop-Plan"        # Your app service plan name
$appServiceName = "localshop-app"             # Your app service name (must be unique globally)
$location = "East US"                         # Your preferred Azure region
$sku = "B1"                                   # Pricing tier (B1=Basic, S1=Standard)
```

### 3. Run the Deployment Script

```powershell
# Make sure you're in the project directory
cd C:\Krishna\BACKEND\LocalShop

# Run the deployment script
.\deploy-to-azure.ps1
```

### 4. Manual Deployment (Alternative)

If you prefer manual deployment:

```bash
# Create Resource Group
az group create --name LocalShop-RG --location "East US"

# Create App Service Plan
az appservice plan create --name LocalShop-Plan --resource-group LocalShop-RG --sku B1 --is-linux

# Create Web App
az webapp create --name localshop-app --resource-group LocalShop-RG --plan LocalShop-Plan --runtime "DOTNETCORE:8.0"

# Set environment to Production
az webapp config appsettings set --name localshop-app --resource-group LocalShop-RG --settings ASPNETCORE_ENVIRONMENT=Production

# Deploy the application
az webapp deployment source config-zip --resource-group LocalShop-RG --name localshop-app --src "./publish.zip"
```

## 🌐 Post-Deployment Configuration

### 1. Configure Azure SQL Connection

1. Go to Azure Portal → SQL Servers
2. Select your SQL Server
3. Go to "Networking" → "Public access"
4. Change "Deny public network access" to **"No"**
5. Add your IP address to firewall rules

### 2. Update Connection String in Azure

1. Go to Azure Portal → App Services
2. Select your app service
3. Go to "Configuration" → "Application settings"
4. Add/Update these connection strings:
   - `AzureSqlConnection`: Your Azure SQL connection string
   - `ASPNETCORE_ENVIRONMENT`: `Production`

### 3. Configure JWT Settings

1. In the same "Application settings" section
2. Add these JWT settings:
   - `Jwt__Key`: Your secure JWT key
   - `Jwt__Issuer`: `LocalShop`
   - `Jwt__Audience`: `yourAppUsers`
   - `Jwt__ExpiryHours`: `24`

## 🔍 Troubleshooting

### Common Issues:

1. **"Deny Public Network Access" Error**:
   - Enable public access in Azure SQL Server
   - Add your IP to firewall rules

2. **Connection String Issues**:
   - Verify server name, username, and password
   - Check if the database exists
   - Ensure the user has proper permissions

3. **App Service Not Starting**:
   - Check application logs in Azure Portal
   - Verify all required environment variables are set
   - Check if the runtime version matches (.NET 8.0)

### View Logs:

```bash
# View real-time logs
az webapp log tail --name localshop-app --resource-group LocalShop-RG

# Download logs
az webapp log download --name localshop-app --resource-group LocalShop-RG
```

## 📱 Testing Your Deployed App

1. **Swagger UI**: `https://your-app-name.azurewebsites.net/swagger`
2. **Health Check**: `https://your-app-name.azurewebsites.net/health`
3. **API Endpoints**: Test your controllers

## 🔒 Security Best Practices

1. **Use Azure Key Vault** for sensitive connection strings
2. **Enable HTTPS only** in App Service
3. **Configure authentication** if needed
4. **Regular security updates** for dependencies
5. **Monitor application logs** for suspicious activity

## 💰 Cost Optimization

1. **Start with Basic tier (B1)** for development
2. **Scale up** only when needed
3. **Use Azure Reserved Instances** for production
4. **Monitor usage** in Azure Cost Management

## 📞 Support

- **Azure Documentation**: [docs.microsoft.com/azure](https://docs.microsoft.com/azure)
- **Azure Support**: Available in Azure Portal
- **Community**: [Stack Overflow](https://stackoverflow.com/questions/tagged/azure)

---

## 🎯 Quick Commands Reference

```bash
# Check Azure CLI version
az version

# List subscriptions
az account list

# Set subscription
az account set --subscription "Your-Subscription-Name"

# List resource groups
az group list

# List app services
az webapp list --resource-group LocalShop-RG

# Restart app service
az webapp restart --name localshop-app --resource-group LocalShop-RG

# Delete resources (cleanup)
az group delete --name LocalShop-RG --yes
```

**Happy Deploying! 🚀**

