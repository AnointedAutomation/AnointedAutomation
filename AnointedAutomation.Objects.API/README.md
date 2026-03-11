# AnointedAutomation.Objects.API

A .NET library providing standardized data models and objects for API development, including account management, billing, and common API response structures.

## Overview

AnointedAutomation.Objects.API offers a comprehensive set of data models designed for building robust APIs. It includes models for user accounts, billing operations, and standardized API responses with built-in JSON serialization support.

## Installation

Install via NuGet:

```bash
dotnet add package AnointedAutomation.Objects.API
```

## Features

- **Account Management Models**: User accounts, profiles, credentials, and IP information
- **Billing System Models**: Complete billing workflow including products, purchases, subscriptions, and payment processing
- **API Response Standardization**: Consistent response format with generic type support
- **File Upload Support**: Custom form file handling for multipart uploads
- **MongoDB Integration**: Built-in MongoDB support for all data models
- **JSON Serialization**: Newtonsoft.Json integration with custom serializers

## Key Components

### Account Models
- `User` - Core user account information
- `Profile` - User profile details
- `Credentials` - Authentication credentials
- `IPInfo` - IP address tracking and geolocation

### Billing Models
- `Product` - Product catalog items
- `Purchase` - Purchase transactions
- `Subscription` - Recurring billing subscriptions
- `Bill` - Invoice/bill generation
- `CreditCard` - Payment card information
- `PaymentCredentials` - Payment authentication
- `Address` - Billing/shipping addresses

### API Utilities
- `ResponseData<T>` - Generic API response wrapper
- `CustomFormFile` - Multipart file upload handling
- `JObjectSerializer` - Custom JSON serialization for MongoDB

## Usage Example

```csharp
using AnointedAutomation.Objects.API;

// Create a standardized API response
var response = new ResponseData<User>
{
    Success = true,
    Data = new User 
    {
        Email = "user@example.com",
        Username = "johndoe"
    },
    Message = "User retrieved successfully"
};

// Handle file uploads
public async Task<IActionResult> Upload(CustomFormFile file)
{
    if (file != null && file.Length > 0)
    {
        // Process file
    }
}
```

## Dependencies

- .NET 8.0
- MongoDB.Driver (3.4.0)
- Newtonsoft.Json (13.0.3)
- Microsoft.AspNetCore.Mvc.NewtonsoftJson (8.0.15)

## Documentation

For detailed documentation on individual models and their properties, see the [API subfolder README](API/README.md).

## License

Copyright © 2023 Anointed Automation, LLC. All rights reserved.

## Author

Created by Alexander Fields
GitHub: [https://github.com/AnointedAutomation](https://github.com/AnointedAutomation)