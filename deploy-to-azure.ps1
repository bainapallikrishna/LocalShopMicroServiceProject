# Azure App Service Deployment Script for LocalShop
# Make sure you have Azure CLI installed and are logged in

Write-Host "🚀 Starting Azure App Service Deployment for LocalShop..." -ForegroundColor Green

# Check if Azure CLI is installed
try {
    $azVersion = az version --output json | ConvertFrom-Json
    Write-Host "✅ Azure CLI version: $($azVersion.'azure-cli')" -ForegroundColor Green
} catch {
    Write-Host "❌ Azure CLI not found. Please install it from: https://docs.microsoft.com/en-us/cli/azure/install-azure-cli" -ForegroundColor Red
    exit 1
}

# Check if logged in to Azure
try {
    $account = az account show --output json | ConvertFrom-Json
    Write-Host "✅ Logged in as: $($account.user.name)" -ForegroundColor Green
} catch {
    Write-Host "❌ Not logged in to Azure. Please run: az login" -ForegroundColor Red
    exit 1
}

# Configuration variables - UPDATE THESE VALUES
$resourceGroupName = "LocalShop-RG"
$appServicePlanName = "LocalShop-Plan"
$appServiceName = "localshop-app"
$location = "East US"  # Change to your preferred region
$sku = "B1"  # Basic tier, change to S1 for Standard

Write-Host "📋 Configuration:" -ForegroundColor Yellow
Write-Host "   Resource Group: $resourceGroupName" -ForegroundColor White
Write-Host "   App Service Plan: $appServicePlanName" -ForegroundColor White
Write-Host "   App Service: $appServiceName" -ForegroundColor White
Write-Host "   Location: $location" -ForegroundColor White
Write-Host "   SKU: $sku" -ForegroundColor White

# Create Resource Group
Write-Host "🔧 Creating Resource Group..." -ForegroundColor Yellow
az group create --name $resourceGroupName --location $location

# Create App Service Plan
Write-Host "🔧 Creating App Service Plan..." -ForegroundColor Yellow
az appservice plan create --name $appServicePlanName --resource-group $resourceGroupName --sku $sku --is-linux

# Create Web App
Write-Host "🔧 Creating Web App..." -ForegroundColor Yellow
az webapp create --name $appServiceName --resource-group $resourceGroupName --plan $appServicePlanName --runtime "DOTNETCORE:8.0"

# Configure app settings for production
Write-Host "🔧 Configuring app settings..." -ForegroundColor Yellow
az webapp config appsettings set --name $appServiceName --resource-group $resourceGroupName --settings ASPNETCORE_ENVIRONMENT=Production

# Deploy the published application
Write-Host "🚀 Deploying application..." -ForegroundColor Yellow
az webapp deployment source config-zip --resource-group $resourceGroupName --name $appServiceName --src "./publish.zip"

Write-Host "✅ Deployment completed successfully!" -ForegroundColor Green
Write-Host "🌐 Your app is available at: https://$appServiceName.azurewebsites.net" -ForegroundColor Cyan

# Get the app URL
$appUrl = "https://$appServiceName.azurewebsites.net"
Write-Host "🔗 App URL: $appUrl" -ForegroundColor Cyan

# Open the app in browser
Start-Process $appUrl

