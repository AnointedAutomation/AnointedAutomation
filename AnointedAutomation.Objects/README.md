# AnointedAutomation.Objects

A .NET library providing standardized data models and objects for general application development, focusing on account management, billing systems, and data serialization.

## Overview

AnointedAutomation.Objects provides a core set of reusable data models that can be used across various types of applications. Unlike the API-specific variant, this library focuses on general-purpose object models without API-specific dependencies.

## Installation

Install via NuGet:

```bash
dotnet add package AnointedAutomation.Objects
```

## Features

- **Account Management Models**: User accounts, profiles, credentials, and IP tracking
- **Billing System Models**: Complete billing workflow models
- **MongoDB Integration**: Native MongoDB support with BSON serialization
- **JSON Serialization**: Newtonsoft.Json integration
- **No API Dependencies**: Pure data models without ASP.NET Core dependencies

## Key Differences from AnointedAutomation.Objects.API

- No ASP.NET Core MVC dependencies
- No API-specific utilities (CustomFormFile, etc.)
- Focused on pure data models
- Suitable for console apps, services, and non-web applications

## Core Models

### Account Namespace
- `User` - User account information
- `Profile` - User profile details  
- `Credentials` - Authentication credentials
- `IPInfo` - IP address and location tracking

### Billing Namespace
- `Product` - Product catalog entries
- `Purchase` - Purchase transactions
- `Subscription` - Recurring subscriptions
- `Bill` - Invoices and bills
- `CreditCard` - Payment card details
- `PaymentCredentials` - Payment authentication
- `PaymentType` - Payment method types
- `Address` - Billing/shipping addresses
- `Contact` - Contact information
- `Inventory` - Inventory tracking
- `Sale` - Sales transactions

### Utilities
- `ResponseData<T>` - Generic response wrapper
- `JObjectSerializer` - Custom JSON/BSON serialization

## Usage Example

```csharp
using AnointedAutomation.Objects.API.Account;
using AnointedAutomation.Objects.API.Billing;

// Create a user
var user = new User
{
    Email = "user@example.com",
    Username = "johndoe",
    CreatedDate = DateTime.UtcNow
};

// Create a product
var product = new Product
{
    Name = "Premium Subscription",
    Price = 9.99m,
    Description = "Monthly premium access"
};

// Process a purchase
var purchase = new Purchase
{
    UserId = user.Id,
    ProductId = product.Id,
    Amount = product.Price,
    PurchaseDate = DateTime.UtcNow
};
```

## Dependencies

- .NET 8.0
- MongoDB.Driver (3.4.0)
- Newtonsoft.Json (13.0.3)

## Documentation

For detailed documentation on individual models and their properties, see the [API subfolder README](API/README.md).

## License

Copyright © 2023 Anointed Automation, LLC. All rights reserved.

## Author

Created by Alexander Fields
GitHub: [https://github.com/AnointedAutomation](https://github.com/AnointedAutomation)