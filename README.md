# LocalShop Microservice Architecture

This project demonstrates a microservice architecture for a local shop application, breaking down the monolithic application into smaller, focused services.

## Architecture Overview

The application is divided into the following microservices:

### 1. Auth Service (`LocalShop.AuthService`)
- **Port**: 7001
- **Responsibility**: User authentication, authorization, and user management
- **Database**: LocalShopAuthDb
- **Endpoints**:
  - `POST /api/auth/login` - User login
  - `POST /api/auth/register` - User registration
  - `POST /api/auth/register-admin` - Admin registration (Admin role required)
  - `GET /api/auth/profile` - Get user profile (Authenticated)
  - `GET /api/auth/users/{userId}` - Get user by ID (Admin role required)

### 2. Product Service (`LocalShop.ProductService`)
- **Port**: 7002
- **Responsibility**: Product catalog management
- **Database**: LocalShopProductDb
- **Endpoints**:
  - `GET /api/product` - Get all products
  - `GET /api/product/{id}` - Get product by ID
  - `POST /api/product` - Create product (Admin/Manager role required)
  - `PUT /api/product/{id}` - Update product (Admin/Manager role required)
  - `DELETE /api/product/{id}` - Delete product (Admin role required)
  - `GET /api/product/category/{category}` - Get products by category
  - `GET /api/product/search?q={term}` - Search products

### 3. API Gateway (`LocalShop.Gateway`)
- **Port**: 5000
- **Responsibility**: Route requests to appropriate services, handle authentication
- **Technology**: YARP (Yet Another Reverse Proxy)
- **Features**:
  - Request routing
  - Load balancing
  - Authentication middleware
  - CORS handling

### 4. Shared Libraries
- **LocalShop.Shared**: Common DTOs and models
- **LocalShop.Shared.Infrastructure**: Common middleware, extensions, and utilities

## Technology Stack

- **.NET 8.0**: Core framework
- **Entity Framework Core**: Data access
- **SQL Server**: Database
- **JWT**: Authentication
- **YARP**: Reverse proxy for API Gateway
- **Docker**: Containerization
- **Docker Compose**: Orchestration

## Prerequisites

- .NET 8.0 SDK
- Docker Desktop
- SQL Server (or use Docker container)

## Getting Started

### Option 1: Using Docker Compose (Recommended)

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd LocalShopMicroserviceProject
   ```

2. **Run with Docker Compose**
   ```bash
   docker-compose up -d
   ```

3. **Access the services**
   - API Gateway: http://localhost:5000
   - Auth Service: http://localhost:7001
   - Product Service: http://localhost:7002
   - SQL Server: localhost:1433

### Option 2: Local Development

1. **Update connection strings** in `appsettings.json` files:
   ```json
   {
     "ConnectionStrings": {
       "AuthDb": "Server=(localdb)\\mssqllocaldb;Database=LocalShopAuthDb;Trusted_Connection=true;MultipleActiveResultSets=true",
       "ProductDb": "Server=(localdb)\\mssqllocaldb;Database=LocalShopProductDb;Trusted_Connection=true;MultipleActiveResultSets=true"
     }
   }
   ```

2. **Run the services individually**:
   ```bash
   # Auth Service
   cd LocalShop.AuthService
   dotnet run

   # Product Service (in another terminal)
   cd LocalShop.ProductService
   dotnet run

   # API Gateway (in another terminal)
   cd LocalShop.Gateway
   dotnet run
   ```

## API Usage

### Authentication Flow

1. **Register a user**:
   ```bash
   POST http://localhost:5000/api/auth/register
   Content-Type: application/json

   {
     "username": "testuser",
     "password": "password123",
     "email": "test@example.com"
   }
   ```

2. **Login**:
   ```bash
   POST http://localhost:5000/api/auth/login
   Content-Type: application/json

   {
     "username": "testuser",
     "password": "password123"
   }
   ```

3. **Use the returned JWT token** in subsequent requests:
   ```bash
   Authorization: Bearer <your-jwt-token>
   ```

### Product Management

1. **Get all products**:
   ```bash
   GET http://localhost:5000/api/product
   ```

2. **Create a product** (requires Admin/Manager role):
   ```bash
   POST http://localhost:5000/api/product
   Authorization: Bearer <your-jwt-token>
   Content-Type: application/json

   {
     "name": "Sample Product",
     "description": "A sample product description",
     "price": 29.99,
     "image": "product-image.jpg"
   }
   ```

## Database Setup

The databases will be created automatically when the services start. If you need to create them manually:

```sql
-- For Auth Service
CREATE DATABASE LocalShopAuthDb;

-- For Product Service
CREATE DATABASE LocalShopProductDb;
```

## Configuration

### JWT Settings
Update the JWT settings in all `appsettings.json` files:
```json
{
  "JwtSettings": {
    "SecretKey": "YourSuperSecretKeyHereThatShouldBeAtLeast32CharactersLong",
    "Issuer": "LocalShop.AuthService",
    "Audience": "LocalShop.API",
    "ExpirationHours": 24
  }
}
```

### Service Ports
- Auth Service: 7001
- Product Service: 7002
- API Gateway: 5000
- SQL Server: 1433

## Development

### Adding New Services

1. Create a new project in the solution
2. Add necessary dependencies
3. Create models, services, and controllers
4. Update the API Gateway configuration
5. Add Docker configuration

### Adding New Endpoints

1. Create the endpoint in the appropriate service
2. Update the API Gateway routing if needed
3. Test the endpoint through the gateway

## Security Considerations

- JWT tokens are used for authentication
- Role-based authorization is implemented
- Passwords are hashed using SHA256
- CORS is configured for cross-origin requests
- Input validation is implemented using data annotations

## Monitoring and Logging

- Structured logging is implemented using ILogger
- Global exception middleware handles errors consistently
- Swagger documentation is available for all services

## Troubleshooting

### Common Issues

1. **Database connection errors**: Ensure SQL Server is running and connection strings are correct
2. **JWT token issues**: Verify JWT settings are consistent across all services
3. **CORS errors**: Check CORS configuration in the gateway
4. **Service communication**: Verify service URLs in the gateway configuration

### Logs

Check the logs for each service:
```bash
# Docker logs
docker-compose logs auth-service
docker-compose logs product-service
docker-compose logs gateway

# Local development logs
# Check the console output for each service
```

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests if applicable
5. Submit a pull request

## License

This project is licensed under the MIT License.
