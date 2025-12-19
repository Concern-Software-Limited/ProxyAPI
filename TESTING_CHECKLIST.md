# Testing Checklist - ProxyAPI Implementation

## ✅ Pre-Testing Setup

- [ ] Database updated with new columns (Image, IsActive)
- [ ] API running on https://api.realproxy.store
- [ ] Admin user created and authenticated
- [ ] JWT token obtained and stored

## 🧪 API Endpoint Tests

### 1. Product Management

#### Get All Products
- [ ] `GET /api/product`
  - Returns list of products with statistics
  - Shows VariantCount, TotalKeys, AvailableKeys, SoldKeys
  - Only shows active products (IsActive = true)
  
**Expected Response:**
```json
{
  "success": true,
  "message": "Products retrieved successfully",
  "data": [
    {
      "id": 1,
      "name": "ABC GB",
      "description": "ABC Proxy Service",
      "image": "/images/abc.png",
      "isActive": true,
      "variantCount": 2,
      "totalKeys": 24,
      "availableKeys": 12,
      "soldKeys": 12,
      "createdAt": "2024-01-01T00:00:00Z"
    }
  ]
}
```

#### Get Product Details
- [ ] `GET /api/product/{id}`
  - Returns product with all variants
  - Each variant shows TotalKeys, AvailableKeys, SoldKeys
  
**Expected Response:**
```json
{
  "success": true,
  "message": "Product retrieved successfully",
  "data": {
    "id": 1,
    "name": "ABC GB",
    "description": "ABC Proxy Service",
    "image": "/images/abc.png",
    "isActive": true,
    "variants": [
      {
        "id": 1,
        "productId": 1,
        "productName": "ABC GB",
        "name": "1GB",
        "totalKeys": 24,
        "availableKeys": 12,
        "soldKeys": 12
      }
    ],
    "createdAt": "2024-01-01T00:00:00Z"
  }
}
```

#### Create Product
- [ ] `POST /api/product`
  - Admin role required
  - Creates product successfully
  - Returns created product with ID

**Test Request:**
```json
{
  "name": "Test Product",
  "description": "Test Description",
  "image": "/images/test.png",
  "isActive": true
}
```

#### Update Product
- [ ] `PUT /api/product/{id}`
  - Admin role required
  - Updates product successfully
  
#### Delete Product
- [ ] `DELETE /api/product/{id}`
  - Admin role required
  - Fails if product has variants
  - Succeeds if product has no variants

#### Search Products
- [ ] `GET /api/product/search?name=ABC`
  - Returns matching products
  - Case-insensitive search

### 2. Variant Management

#### Get All Variants
- [ ] `GET /api/variant`
  - Returns all variants with product names
  - Shows key statistics

#### Get Variants by Product
- [ ] `GET /api/variant/product/{productId}`
  - Returns only variants for specified product

#### Create Variant
- [ ] `POST /api/variant`
  - Admin role required
  - Creates variant successfully
  - Validates product exists

**Test Request:**
```json
{
  "productId": 1,
  "name": "5GB"
}
```

#### Update Variant
- [ ] `PUT /api/variant/{id}`
  - Admin role required
  - Updates variant name

#### Delete Variant
- [ ] `DELETE /api/variant/{id}`
  - Admin role required
  - Fails if variant has keys
  - Succeeds if variant has no keys

### 3. CD Key Management

#### Bulk Add Keys
- [ ] `POST /api/cdkey`
  - Admin role required
  - Adds multiple keys at once
  - **Duplicate Detection**: Rejects duplicate keys
  - **Statistics Update**: Automatically updates variant TotalKeys and AvailableKeys
  
**Test Request:**
```json
{
  "productId": 1,
  "variantId": 1,
  "keys": [
    "test-key-001",
    "test-key-002",
    "test-key-003"
  ],
  "status": "Available"
}
```

**Test Duplicate Detection:**
```json
{
  "productId": 1,
  "variantId": 1,
  "keys": [
    "test-key-001",  // Already exists
    "test-key-004"   // New
  ],
  "status": "Available"
}
```
Expected: Error message listing "test-key-001" as duplicate

#### Get Keys with Filters
- [ ] `GET /api/cdkey?ProductId=1&VariantId=1&Status=Available&SortBy=Newest`
  - Filters by product ID
  - Filters by variant ID
  - Filters by status
  - Sorts by newest/oldest

#### Get Keys by Product and Variant
- [ ] `GET /api/cdkey/product/1/variant/1?status=Available`
  - Returns keys for specific product/variant combination
  - Optional status filter

#### Get Sold Keys
- [ ] `GET /api/cdkey/sold`
  - Returns only sold keys
  - Shows KeyValue, ProductName, VariantName, UserEmail, SoldDate
  
- [ ] `GET /api/cdkey/sold?Search=abc`
  - Searches key value, product name, and user email
  
- [ ] `GET /api/cdkey/sold?StartDate=2024-01-01&EndDate=2024-12-31`
  - Filters by date range

**Expected Response:**
```json
{
  "success": true,
  "message": "Sold keys retrieved successfully",
  "data": [
    {
      "keyValue": "abc-123",
      "productName": "ABC GB",
      "variantName": "1GB",
      "userEmail": "user1@example.com",
      "soldDate": "2024-01-10T00:00:00Z"
    }
  ]
}
```

#### Delete Key
- [ ] `DELETE /api/cdkey/{id}`
  - Admin role required
  - **Cannot delete sold keys**
  - Updates variant statistics when deleting available keys

### 4. Statistics Verification

After adding keys, verify statistics are correct:

- [ ] Variant TotalKeys increments
- [ ] Variant AvailableKeys increments (for Available status)
- [ ] Product aggregated statistics are correct
- [ ] After deleting key, statistics decrement

## 🎨 Frontend Tests (Sold Keys Page)

### Page Loading
- [ ] Page loads at `/sold-keys`
- [ ] Shows in sidebar menu
- [ ] Displays table with proper columns

### Search Functionality
- [ ] Search input works
- [ ] Searches across key value, product name, and user email
- [ ] Results update in real-time

### Date Filter
- [ ] Start date picker works
- [ ] End date picker works
- [ ] Date range filtering works correctly

### Table Display
- [ ] Shows: Key Value, Product, Variant, User, Sold Date
- [ ] Date formatting is correct (e.g., "Jan 10, 2024")
- [ ] Loading spinner shows while fetching
- [ ] Empty state shows when no keys found

### Dark Mode
- [ ] Switches between light and dark mode correctly
- [ ] All elements are visible in both modes

## 🔒 Security Tests

### Authentication
- [ ] Endpoints reject requests without token (401 Unauthorized)
- [ ] Endpoints reject requests with invalid token (401 Unauthorized)

### Authorization
- [ ] Non-admin users cannot create products (403 Forbidden)
- [ ] Non-admin users cannot create variants (403 Forbidden)
- [ ] Non-admin users cannot add keys (403 Forbidden)
- [ ] Non-admin users cannot delete keys (403 Forbidden)

### Validation
- [ ] Empty product name is rejected
- [ ] Empty keys array is rejected
- [ ] Invalid product ID is rejected
- [ ] Invalid variant ID is rejected

## 🐛 Error Handling Tests

### Product Deletion
- [ ] Cannot delete product with variants
- [ ] Proper error message returned

### Variant Deletion
- [ ] Cannot delete variant with keys
- [ ] Proper error message returned

### Key Deletion
- [ ] Cannot delete sold keys
- [ ] Proper error message returned

### Duplicate Keys
- [ ] Duplicate keys are detected
- [ ] List of duplicate keys returned in error message

## 📊 Database Integrity Tests

After operations, verify database:

### Foreign Keys
- [ ] Variants reference valid products
- [ ] Keys reference valid products and variants
- [ ] Sold keys reference valid users

### Statistics Consistency
```sql
-- Verify variant statistics match actual counts
SELECT 
    v.Id,
    v.Name,
    v.TotalKeys,
    v.AvailableKeys,
    v.SoldKeys,
    (SELECT COUNT(*) FROM CDKeys WHERE VariantId = v.Id) AS ActualTotal,
    (SELECT COUNT(*) FROM CDKeys WHERE VariantId = v.Id AND Status = 'Available') AS ActualAvailable,
    (SELECT COUNT(*) FROM CDKeys WHERE VariantId = v.Id AND Status = 'Sold') AS ActualSold
FROM Variants v;
```

- [ ] TotalKeys = ActualTotal
- [ ] AvailableKeys = ActualAvailable
- [ ] SoldKeys = ActualSold

### Unique Constraints
- [ ] Key values are unique
- [ ] Cannot insert duplicate keys manually

## 📝 Test Scenarios

### Scenario 1: Complete Product Setup
1. [ ] Create product "Test Product"
2. [ ] Create variant "1GB" for product
3. [ ] Add 10 keys to variant
4. [ ] Verify product shows: 1 variant, 10 total keys, 10 available keys
5. [ ] Verify variant shows: 10 total, 10 available, 0 sold

### Scenario 2: Key Sales Flow
1. [ ] Mark 3 keys as sold (manually update database for testing)
2. [ ] Update variant statistics
3. [ ] Verify product shows: 10 total, 7 available, 3 sold
4. [ ] Verify sold keys page shows 3 sold keys
5. [ ] Search for sold keys by product name
6. [ ] Filter sold keys by date range

### Scenario 3: Bulk Operations
1. [ ] Add 100 keys at once
2. [ ] Verify all keys added successfully
3. [ ] Verify statistics updated correctly
4. [ ] Try adding duplicate keys
5. [ ] Verify error message shows duplicates

### Scenario 4: Deletion Protection
1. [ ] Try to delete product with variants
2. [ ] Verify error returned
3. [ ] Try to delete variant with keys
4. [ ] Verify error returned
5. [ ] Try to delete sold key
6. [ ] Verify error returned

## ✅ Acceptance Criteria

All tests passed:
- [ ] All API endpoints working
- [ ] Statistics accurate
- [ ] Duplicate detection working
- [ ] Frontend displays data correctly
- [ ] Search and filters working
- [ ] Security enforced
- [ ] Error handling proper
- [ ] Database integrity maintained

## 🚀 Production Readiness

- [ ] All tests passed
- [ ] Performance acceptable (< 500ms for list endpoints)
- [ ] Error logging configured
- [ ] API documentation complete
- [ ] Frontend integrated
- [ ] Database backed up
- [ ] Connection strings configured for production
- [ ] CORS configured properly
- [ ] Rate limiting considered
- [ ] Monitoring setup

## 📞 Support

If any test fails, check:
1. `PROXY_API_IMPLEMENTATION_SUMMARY.md` - Implementation details
2. `QUICK_START_API.md` - Setup instructions
3. `DATABASE_SCHEMA.sql` - Database structure
4. Console logs for detailed errors
5. Swagger UI at https://api.realproxy.store/swagger
