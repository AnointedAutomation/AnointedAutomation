# AnointedAutomation.Objects.API - API Models

## Overview

This directory contains the data models and utilities specifically designed for ASP.NET Core API applications. These models provide standardized structures for account management, billing operations, and API interactions.

## Core Components

### ResponseData

A generic response wrapper for API endpoints.

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

### CustomFormFile

Implementation of `IFormFile` for handling file uploads with MongoDB integration.

**Properties:**
- `FileName` - Name of the file (MongoDB BsonId)
- `Content` - Byte array containing file data
- `ContentType` - MIME type (defaults to "application/octet-stream")
- `Length` - File size in bytes

**Usage:**
```csharp
var file = new CustomFormFile("document.pdf", fileBytes);
await file.CopyToAsync(stream);
```

### JObjectSerializer

Custom JSON serializer for MongoDB integration, handling BSON/JSON conversions.

## Account Models

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
Authentication credentials for API access.

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

### Creating an API Response
```csharp
public IActionResult GetUser(int id)
{
    var user = _userService.GetById(id);
    
    return Ok(new ResponseData
    {
        message = "User retrieved successfully",
        Data = user,
        Error = null
    });
}
```

### Handling File Uploads
```csharp
[HttpPost("upload")]
public async Task<IActionResult> Upload(CustomFormFile file)
{
    if (file?.Length > 0)
    {
        // Process file
        var fileName = file.FileName;
        var content = file.Content;
        
        // Save to storage
        await _fileService.SaveAsync(fileName, content);
        
        return Ok(new ResponseData
        {
            message = "File uploaded successfully",
            Data = new { fileName, size = file.Length }
        });
    }
    
    return BadRequest(new ResponseData
    {
        message = "No file provided",
        Error = "File is required"
    });
}
```

### Working with Subscriptions
```csharp
var subscription = new Subscription
{
    UserId = user.Id,
    PlanName = "Premium",
    Price = 9.99m,
    BillingCycle = "monthly",
    StartDate = DateTime.UtcNow,
    IsActive = true,
    AutoRenew = true
};
```

## Dependencies

- ASP.NET Core HTTP abstractions
- MongoDB.Driver
- Newtonsoft.Json

## Notes

- All models use MongoDB BSON attributes for proper serialization
- DateTime fields should use UTC
- Sensitive data (passwords, credit cards) must be properly encrypted
- Follow PCI compliance guidelines for payment data

## License

Copyright © 2023 Anointed Automation, LLC. All rights reserved.

## Author

Created by Alexander Fields
GitHub: [https://github.com/AnointedAutomation](https://github.com/AnointedAutomation)