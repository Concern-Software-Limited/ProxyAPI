# Quick Start Guide - ProxyAPI

## Prerequisites
- .NET 9.0 SDK installed
- MySQL or SQL Server database
- Database created and schema applied (see `DATABASE_SCHEMA.sql`)

## Step 1: Update Database

### For MySQL:
```bash
mysql -u root -p proxydb < DATABASE_UPDATE_MYSQL.sql
```

### For SQL Server:
```bash
sqlcmd -S localhost -d proxydb -i DATABASE_UPDATE_SCRIPT.sql
```

### Or manually:
```sql
-- MySQL
ALTER TABLE Products ADD COLUMN Image VARCHAR(500) NULL;
ALTER TABLE Products ADD COLUMN IsActive TINYINT(1) NOT NULL DEFAULT 1;

-- SQL Server
ALTER TABLE Products ADD Image VARCHAR(500) NULL;
ALTER TABLE Products ADD IsActive BIT NOT NULL DEFAULT 1;
```

## Step 2: Update Connection String

Edit `appsettings.json`:

### For MySQL:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=proxydb;User=root;Password=yourpassword;"
  }
}
```

### For SQL Server:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=proxydb;Trusted_Connection=true;"
  }
}
```

## Step 3: Build and Run

```bash
cd ProxyAPI
dotnet build
dotnet run
```

The API will be available at:
- HTTP: `https://api.realproxy.store`
- HTTPS: `https://localhost:7142`
- Swagger UI: `https://api.realproxy.store/swagger`

## Step 4: Test the API

### 1. Register a User
```http
POST https://api.realproxy.store/api/auth/send-otp
Content-Type: application/json

{
  "email": "admin@example.com"
}
```

### 2. Verify OTP
```http
POST https://api.realproxy.store/api/auth/verify-otp
Content-Type: application/json

{
  "email": "admin@example.com",
  "otpCode": "123456"
}
```

### 3. Create a Product (Admin Only)
```http
POST https://api.realproxy.store/api/product
Authorization: Bearer {your-token}
Content-Type: application/json

{
  "name": "ABC GB",
  "description": "ABC Proxy Service",
  "image": "/images/abc.png",
  "isActive": true
}
```

### 4. Create a Variant
```http
POST https://api.realproxy.store/api/variant
Authorization: Bearer {your-token}
Content-Type: application/json

{
  "productId": 1,
  "name": "1GB"
}
```

### 5. Bulk Add CD Keys
```http
POST https://api.realproxy.store/api/cdkey
Authorization: Bearer {your-token}
Content-Type: application/json

{
  "productId": 1,
  "variantId": 1,
  "keys": [
    "xyz-abc-123",
    "xyz-abc-124",
    "xyz-abc-125"
  ],
  "status": "Available"
}
```

### 6. Get All Products with Statistics
```http
GET https://api.realproxy.store/api/product
Authorization: Bearer {your-token}
```

### 7. Get Product Details with Variants
```http
GET https://api.realproxy.store/api/product/1
Authorization: Bearer {your-token}
```

### 8. Get Sold Keys
```http
GET https://api.realproxy.store/api/cdkey/sold?Search=abc&StartDate=2024-01-01&EndDate=2024-12-31
Authorization: Bearer {your-token}
```

## Common Operations

### Get Keys by Product and Variant
```http
GET https://api.realproxy.store/api/cdkey/product/1/variant/1?status=Available
Authorization: Bearer {your-token}
```

### Get Keys with Filters
```http
GET https://api.realproxy.store/api/cdkey?ProductId=1&Status=Available&SortBy=Newest
Authorization: Bearer {your-token}
```

### Search Products
```http
GET https://api.realproxy.store/api/product/search?name=ABC
Authorization: Bearer {your-token}
```

## API Response Format

All endpoints return responses in this format:

### Success Response:
```json
{
  "success": true,
  "message": "Operation successful",
  "data": { ... }
}
```

### Error Response:
```json
{
  "success": false,
  "message": "Error message",
  "data": null
}
```

## Authentication

All endpoints (except auth endpoints) require a JWT token in the Authorization header:
```
Authorization: Bearer {your-jwt-token}
```

Admin-only endpoints (POST, PUT, DELETE for products, variants, keys) require Admin role.

## Postman Collection

Import `ProxyAPI_Postman_Collection.json` into Postman for pre-configured requests.

## Troubleshooting

### Port Already in Use
Edit `launchSettings.json` and change the ports.

### Database Connection Failed
Check your connection string in `appsettings.json`.

### Unauthorized Error
Ensure you're sending the JWT token in the Authorization header.

### Foreign Key Constraint
Ensure parent entities (Product, Variant) exist before creating child entities.

## Next Steps

1. ✅ API is ready and tested
2. 🎨 Connect your frontend application
3. 📊 Test all CRUD operations
4. 🔐 Configure SMTP for email OTPs
5. 🚀 Deploy to production

## Support

See complete documentation:
- `PROXY_API_IMPLEMENTATION_SUMMARY.md` - Complete implementation details
- `API_ENDPOINTS.md` - All available endpoints
- `DATABASE_SCHEMA.sql` - Database structure
- `README.md` - General information
