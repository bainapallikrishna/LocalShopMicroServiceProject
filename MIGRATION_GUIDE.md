# Migration Guide: Monolith to Microservices

This guide explains how to migrate from the existing monolithic LocalShop application to the new microservice architecture.

## Migration Overview

The migration involves breaking down the monolithic application into focused microservices while maintaining functionality and improving scalability.

## Pre-Migration Checklist

### 1. Database Preparation
- [ ] Backup existing database
- [ ] Document current schema
- [ ] Plan data migration strategy
- [ ] Test database connectivity

### 2. Environment Setup
- [ ] Install .NET 8.0 SDK
- [ ] Install Docker Desktop
- [ ] Configure development environment
- [ ] Set up SQL Server (local or containerized)

### 3. Code Analysis
- [ ] Identify business domains
- [ ] Map existing controllers to services
- [ ] Document shared dependencies
- [ ] Plan API versioning strategy

## Migration Steps

### Step 1: Set Up New Solution Structure

1. **Create the new solution**:
   ```bash
   dotnet new sln -n LocalShop.Microservices
   ```

2. **Add projects to solution**:
   ```bash
   dotnet sln add LocalShop.Shared/LocalShop.Shared.csproj
   dotnet sln add LocalShop.Shared.Infrastructure/LocalShop.Shared.Infrastructure.csproj
   dotnet sln add LocalShop.AuthService/LocalShop.AuthService.csproj
   dotnet sln add LocalShop.ProductService/LocalShop.ProductService.csproj
   dotnet sln add LocalShop.Gateway/LocalShop.Gateway.csproj
   ```

### Step 2: Data Migration

1. **Create new databases**:
   ```sql
   CREATE DATABASE LocalShopAuthDb;
   CREATE DATABASE LocalShopProductDb;
   ```

2. **Migrate user data** (if needed):
   ```sql
   -- Example migration script
   INSERT INTO LocalShopAuthDb.dbo.Users (Username, PasswordHash, Email, CreatedAt, IsActive)
   SELECT Username, PasswordHash, Email, GETUTCDATE(), 1
   FROM LocalShop.dbo.Users;
   ```

3. **Migrate product data** (if needed):
   ```sql
   -- Example migration script
   INSERT INTO LocalShopProductDb.dbo.Products (Name, Description, Image, Price, CreatedAt, UpdatedAt, IsActive, StockQuantity, Category)
   SELECT Name, Description, Image, Price, GETUTCDATE(), GETUTCDATE(), 1, 0, ''
   FROM LocalShop.dbo.Products;
   ```

### Step 3: Service Deployment

1. **Deploy Auth Service**:
   ```bash
   cd LocalShop.AuthService
   dotnet publish -c Release
   ```

2. **Deploy Product Service**:
   ```bash
   cd LocalShop.ProductService
   dotnet publish -c Release
   ```

3. **Deploy API Gateway**:
   ```bash
   cd LocalShop.Gateway
   dotnet publish -c Release
   ```

### Step 4: Configuration Updates

1. **Update connection strings** in all services
2. **Configure JWT settings** consistently across services
3. **Set up environment variables** for production
4. **Configure logging** and monitoring

### Step 5: Testing

1. **Unit Tests**: Test individual service functionality
2. **Integration Tests**: Test service communication
3. **End-to-End Tests**: Test complete user workflows
4. **Performance Tests**: Verify performance characteristics

## API Changes

### Authentication Endpoints

**Before (Monolith)**:
```
POST /api/auth/login
POST /api/auth/register
```

**After (Microservices)**:
```
POST /api/auth/login
POST /api/auth/register
POST /api/auth/register-admin
GET /api/auth/profile
GET /api/auth/users/{userId}
```

### Product Endpoints

**Before (Monolith)**:
```
GET /api/product/GetProduct
POST /api/product/CreateProduct
```

**After (Microservices)**:
```
GET /api/product
GET /api/product/{id}
POST /api/product
PUT /api/product/{id}
DELETE /api/product/{id}
GET /api/product/category/{category}
GET /api/product/search?q={term}
```

## Breaking Changes

### 1. Authentication
- JWT token format remains the same
- Role-based authorization is enhanced
- New admin-specific endpoints added

### 2. Product Management
- Enhanced CRUD operations
- Added search and category filtering
- Improved error handling and validation

### 3. Response Format
- Consistent API response format across all services
- Enhanced error messages and validation feedback

## Rollback Strategy

### If Migration Fails

1. **Immediate Rollback**:
   - Stop new microservices
   - Restart monolithic application
   - Restore original database if needed

2. **Gradual Rollback**:
   - Route traffic back to monolith
   - Keep microservices running for testing
   - Plan next migration attempt

### Rollback Checklist

- [ ] Stop all microservices
- [ ] Restart monolithic application
- [ ] Verify database integrity
- [ ] Test critical functionality
- [ ] Update DNS/routing if needed

## Post-Migration Tasks

### 1. Monitoring Setup
- Configure application insights
- Set up health checks
- Implement logging aggregation
- Monitor service performance

### 2. Documentation Updates
- Update API documentation
- Create service-specific guides
- Document deployment procedures
- Update troubleshooting guides

### 3. Team Training
- Train developers on microservice patterns
- Update deployment procedures
- Review monitoring and alerting
- Establish incident response procedures

## Performance Considerations

### Before Migration
- Single application instance
- Shared database connections
- Monolithic scaling

### After Migration
- Independent service scaling
- Separate database connections
- Load balancing through gateway
- Potential network latency

### Optimization Strategies
- Implement caching where appropriate
- Use connection pooling
- Optimize database queries
- Consider service mesh for advanced routing

## Security Considerations

### Authentication
- JWT tokens remain the same
- Enhanced role-based access control
- Service-to-service authentication

### Data Protection
- Encrypt sensitive data
- Implement proper input validation
- Use HTTPS for all communications
- Regular security audits

## Troubleshooting Common Issues

### 1. Service Communication
**Issue**: Services can't communicate
**Solution**: Check network configuration and service URLs

### 2. Database Connectivity
**Issue**: Database connection errors
**Solution**: Verify connection strings and database availability

### 3. JWT Token Issues
**Issue**: Authentication failures
**Solution**: Ensure JWT settings are consistent across services

### 4. CORS Errors
**Issue**: Cross-origin request failures
**Solution**: Check CORS configuration in gateway

## Support and Maintenance

### Ongoing Maintenance
- Regular security updates
- Performance monitoring
- Database maintenance
- Service health checks

### Support Channels
- Technical documentation
- Team training materials
- Incident response procedures
- Escalation paths

## Conclusion

The migration to microservices provides:
- Better scalability and maintainability
- Independent service deployment
- Enhanced security and monitoring
- Improved development velocity

Follow this guide carefully and test thoroughly at each step to ensure a successful migration.

 Microservice Architecture Overview
Services Created:
🔐 Auth Service (LocalShop.AuthService)
Handles user authentication, registration, and authorization
JWT token generation and validation
User management with role-based access control
Port: 7001
📦 Product Service (LocalShop.ProductService)
Manages product catalog and inventory
CRUD operations for products
Search and category filtering
Port: 7002
🌐 API Gateway (LocalShop.Gateway)
Routes requests to appropriate services
Handles authentication and CORS
Load balancing and request aggregation
Port: 5000
📚 Shared Libraries
LocalShop.Shared: Common DTOs and models
LocalShop.Shared.Infrastructure: Middleware, extensions, and utilities
Key Features:
✅ JWT Authentication - Secure token-based authentication across all services
✅ Role-Based Authorization - Admin, Manager, and User roles
✅ Database Separation - Each service has its own database
✅ Docker Support - Complete containerization with Docker Compose
✅ API Gateway - YARP-based reverse proxy for service routing
✅ Global Exception Handling - Consistent error handling across services
✅ Swagger Documentation - API documentation for all services
✅ CORS Support - Cross-origin request handling
Technology Stack:
.NET 8.0 - Modern .NET framework
Entity Framework Core - Data access
SQL Server - Database
JWT - Authentication
YARP - Reverse proxy
Docker - Containerization
Docker Compose - Orchestration
🚀 Getting Started
Option 1: Docker Compose (Recommended)
Option 2: Local Development
📖 Documentation
I've created comprehensive documentation:
README.md - Complete setup and usage guide
MIGRATION_GUIDE.md - Step-by-step migration from monolith to microservices
�� Next Steps
Test the services using the provided endpoints
Configure your database connection strings
Update JWT settings for production use
Deploy to your preferred hosting platform
The microservice architecture provides:
Better scalability - Each service can scale independently
Improved maintainability - Focused, single-responsibility services
Enhanced security - Isolated services with proper authentication
Faster development - Teams can work on services independently
Would you like me to help you with any specific aspect of the microservice setup or explain any part of the architecture in more detail?
