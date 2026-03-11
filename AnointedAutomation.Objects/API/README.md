# AnointedAutomation.Objects - Core Models

## Overview

This directory contains the core data models for the AnointedAutomation.Objects library. These models provide standardized structures for account management, billing operations, and data serialization without API-specific dependencies.

## Core Components

### ResponseData

A generic response wrapper for data exchange.

**Properties:**
- `Data` (object) - The response payload
- `Error` (object) - Error information if applicable
- `message` (string) - Response message

**Usage:**
```csharp
var response = new ResponseData
{
    message = "Operation successful",
    Data = new { id = 123, name = "John" },
    Error = null
};
```

### JObjectSerializer

Custom JSON serializer for MongoDB integration, handling BSON/JSON conversions.

## Account Models

Located in the `Account` namespace:

### User
Core user account information with authentication details.

**Key Properties:**
- `_id` - MongoDB ObjectId
- `Email` - User's email address
- `Username` - Unique username
- `Password` - Hashed password
- `CreatedDate` - Account creation timestamp
- `LastLoginDate` - Last login timestamp
- `IsActive` - Account status

### Profile
Extended user profile information.

**Key Properties:**
- `FirstName`, `LastName` - User's name
- `DisplayName` - Public display name
- `Bio` - User biography
- `ProfilePictureUrl` - Avatar URL
- `DateOfBirth` - User's birthdate
- `PhoneNumber` - Contact number

### Credentials
Authentication credentials for access control.

**Key Properties:**
- `ApiKey` - API authentication key
- `SecretKey` - Secret key for signing
- `ExpiresAt` - Credential expiration
- `Scopes` - Authorized access scopes

### IPInfo
IP address tracking and geolocation data.

**Key Properties:**
- `IPAddress` - IP address string
- `Country`, `City`, `Region` - Location data
- `Latitude`, `Longitude` - Coordinates
- `ISP` - Internet service provider
- `LastSeen` - Last activity timestamp

## Billing Models

Located in the `Billing` namespace:

### Product
Product catalog entries.

**Key Properties:**
- `Name` - Product name
- `Description` - Product details
- `Price` - Unit price
- `SKU` - Stock keeping unit
- `Category` - Product category
- `IsActive` - Availability status

### Purchase
Individual purchase transactions.

**Key Properties:**
- `PurchaseId` - Unique transaction ID
- `UserId` - Buyer's user ID
- `ProductId` - Purchased product
- `Amount` - Transaction amount
- `PurchaseDate` - Transaction timestamp
- `Status` - Transaction status

### Subscription
Recurring billing subscriptions.

**Key Properties:**
- `SubscriptionId` - Unique subscription ID
- `PlanName` - Subscription plan
- `Price` - Recurring price
- `BillingCycle` - Payment frequency
- `StartDate`, `EndDate` - Subscription period
- `IsActive` - Subscription status
- `AutoRenew` - Auto-renewal flag

### Bill
Invoice/billing records.

**Key Properties:**
- `BillId` - Unique bill ID
- `UserId` - Customer ID
- `Amount` - Total amount
- `DueDate` - Payment due date
- `Status` - Payment status
- `Items` - Line items

### CreditCard
Payment card information (PCI compliance required).

**Key Properties:**
- `CardNumber` - Masked card number
- `CardholderName` - Name on card
- `ExpiryMonth`, `ExpiryYear` - Expiration
- `Last4Digits` - Last 4 digits
- `CardType` - Card brand

### Address
Billing/shipping addresses.

**Key Properties:**
- `Street1`, `Street2` - Address lines
- `City`, `State`, `PostalCode` - Location
- `Country` - Country code
- `Type` - Address type (billing/shipping)

### Other Billing Models
- `PaymentCredentials` - Payment gateway credentials
- `PaymentType` - Payment method types
- `PayeeInfo` - Payee/merchant information
- `Contact` - Contact information
- `Inventory` - Stock tracking
- `Sale` - Sales records

## Usage Examples

### Working with User Objects
```csharp
using AnointedAutomation.Objects.API.Account;

var user = new User
{
    Email = "user@example.com",
    Username = "johndoe",
    CreatedDate = DateTime.UtcNow,
    IsActive = true
};

var profile = new Profile
{
    FirstName = "John",
    LastName = "Doe",
    DisplayName = "JohnD",
    Bio = "Software developer"
};
```

### Creating Billing Records
```csharp
using AnointedAutomation.Objects.API.Billing;

var product = new Product
{
    Name = "Premium License",
    Description = "Annual premium license",
    Price = 99.99m,
    SKU = "PREM-001",
    IsActive = true
};

var purchase = new Purchase
{
    UserId = user.Id,
    ProductId = product.Id,
    Amount = product.Price,
    PurchaseDate = DateTime.UtcNow,
    Status = "completed"
};
```

### Using ResponseData
```csharp
// Success response
var successResponse = new ResponseData
{
    message = "User created successfully",
    Data = user,
    Error = null
};

// Error response
var errorResponse = new ResponseData
{
    message = "Failed to create user",
    Data = null,
    Error = new { code = "USER_EXISTS", details = "Username already taken" }
};
```

## Dependencies

- MongoDB.Driver
- Newtonsoft.Json

## Notes

- All models use MongoDB BSON attributes for proper serialization
- DateTime fields should use UTC
- Sensitive data (passwords, credit cards) must be properly encrypted
- These are pure data models without API-specific dependencies
- Suitable for console applications, services, and non-web applications

## License

Copyright © 2023 Anointed Automation, LLC. All rights reserved.

## Author

Created by Alexander Fields
GitHub: [https://github.com/AnointedAutomation](https://github.com/AnointedAutomation)