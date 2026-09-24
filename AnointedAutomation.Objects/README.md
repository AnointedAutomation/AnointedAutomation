# AnointedAutomation.Objects

Plain C# models shared across AnointedAutomation services: user accounts and profiles, billing and payment records, Google sign in payloads, and standard API response envelopes. The classes carry no database attributes, so the same types work with MongoDB, SQL or plain JSON.

[![NuGet](https://img.shields.io/nuget/v/AnointedAutomation.Objects.svg)](https://www.nuget.org/packages/AnointedAutomation.Objects) [![Downloads](https://img.shields.io/nuget/dt/AnointedAutomation.Objects.svg)](https://www.nuget.org/packages/AnointedAutomation.Objects)

## Installation

```bash
dotnet add package AnointedAutomation.Objects
```

- Target framework: `net10.0`
- Dependencies:
  - [AnointedAutomation.Enums](https://www.nuget.org/packages/AnointedAutomation.Enums) (payment and card enums such as `PaymentProvider`, `TransactionStatus`, `CardType`)
  - [Newtonsoft.Json](https://www.nuget.org/packages/Newtonsoft.Json) 13.0.4

## Quick start

```csharp
using System.Collections.Generic;
using AnointedAutomation.Objects;
using AnointedAutomation.Objects.Account;
using AnointedAutomation.Objects.Billing;

User user = new User
{
    UserId = "u_123",
    Email = "user@example.com",
    Username = "johndoe",
    Role = "member",
    Profile = new Profile { FirstName = "John", LastName = "Doe" }
};

// Typed response envelope with status-code factories.
ResponseData<User> ok = ResponseData<User>.Ok(user, "User loaded");
ResponseData<User> missing = ResponseData<User>.NotFound("No such user");

// Paged results: totalPages, hasNextPage and hasPreviousPage are computed.
PaginatedResponse<User> page = new PaginatedResponse<User>(new List<User> { user }, currentPage: 1, pageSize: 20, totalItems: 41);

// Card helpers.
bool looksValid = CreditCard.ValidateLuhn("4242424242424242");
string masked = CreditCard.MaskCardNumber("4242424242424242");
```

## API overview

### Responses (`AnointedAutomation.Objects`)

| Type | Description |
|---|---|
| `ResponseData` | Untyped envelope: `Message`, `Data`, `Error`, `success`, `Timestamp` (ISO 8601 UTC), plus fixed `Anointed`, `Automation`, `Copyright`, `Links` and `Trinity` fields. The `(string message, object data, object error)` constructor sets `success` from whether `Error` is null. |
| `ResponseData<T>` | Typed envelope with `Data`, `Message`, `statusCode`, `success`, `Timestamp`, and static factories `Ok`, `Created`, `Accepted`, `NoContent`, `Error`, `BadRequest`, `Unauthorized`, `Forbidden`, `NotFound`, `Conflict`, `UnprocessableEntity`, `TooManyRequests`, `InternalServerError`, `ServiceUnavailable`. |
| `PaginatedResponse<T>` | `Data`, `currentPage`, `pageSize`, `totalItems`, `totalPages`, `hasNextPage`, `hasPreviousPage`. |
| `ChristianEmoticons` | String constants such as `LatinCross`, `PrayingHands`, `Dove`, `Bible`, `Fish`. |

### Account (`AnointedAutomation.Objects.Account`)

| Type | Description |
|---|---|
| `User` | Account record: `UserId`, `Email`, `Username`, `Password`, `Role`, `Profile`, `Token`, `tokenExpiration`, `emailConfirmed`, `IPAddresses`, `Friends`, `FriendId`, `BlockedUsers`, `isBanned`, `banned`, `BannedReason`, `createdDate`, `lastActiveDate`, `timeOnline`, `Google`, `Meta`, and account deletion fields (`deletionDate`, `DeletionConfirmationCode`, `deletionConfirmationExpiration`). |
| `Profile` | Extends `Billing.Contact` (`FirstName`, `MiddleName`, `LastName`, `dob`, `number`) with a Newtonsoft `JObject AccountSettings`. |
| `Credentials` | Login payload: `Email`, `Password`, `Token`, `GoogleToken`. |
| `IPInfo` | Login history for one IP: `IpAddress`, `firstLogin`, `lastLogin`, `loginCount`, `timeOnline`. |

### Billing (`AnointedAutomation.Objects.Billing`)

| Type | Description |
|---|---|
| `Purchase` | Order with addresses, `Item` (`Product`), discounts, tax, tip, `total`, `TransactionId`, `OrderStatus` (`TransactionStatus`) and a `StatusHistory`; `UpdateOrderStatus(newStatus, changedBy, reason)` records each change. |
| `Subscription` | Extends `Purchase` with `Status` (`SubscriptionStatus`), billing period, trial, pause/cancel dates, per metric `Usage`, and lifecycle methods `Pause`, `Resume`, `Cancel`, `Renew`, `UpdateStatus`, `IsUsable`, `IsTrialing`, `CalculateNextBillingDate`, `GetDaysRemaining`, `AddUsageMetric`, `UpdateUsage`, `IsOverUsageLimit`. |
| `SubscriptionUsage` | Metered usage: `Limit`, `Used`, `Remaining`, `UsagePercentage`, `IsOverLimit`, `IsNearLimit`, `IncrementUsage`, `DecrementUsage`, `ResetForNewPeriod`. |
| `StatusHistoryEntry` | One status transition: `PreviousStatus`, `NewStatus`, `ChangedBy`, `Reason`, `Timestamp`. |
| `Bill` | A customer's billing record: `AdressBilling`, `Orders`, `PaymentTypesOnFile`, `Purchases`, `Refunds`, `Subscriptions`. |
| `Product`, `Sale`, `Inventory` | Catalog, sales and stock records. `Product.GetTotalQuantitySold(products, sales)` totals sales per product. |
| `Address`, `Contact` | Postal address and person/contact details. |
| `CreditCard` | Card details with `IsValid`, `IsExpired`, `ValidateLuhn`, static `DetectCardType` (`CardType`), static `MaskCardNumber`, `GetDisplayString` and `ToSecureObject` (strips sensitive data). |
| `PaymentCredentials`, `PayeeInfo` | Payment method on file (`CreditCard`, `PayeeInfo`, `paymentType`). |
| `PaymentIntent`, `PaymentCustomer`, `PaymentMethodToken`, `Refund`, `Dispute` | Provider agnostic payment records, each tagged with a `PaymentProvider` and the provider's own id. Amounts are `long` minor units. |
| `WebhookEvent` | A received provider webhook (`Provider`, `EventType`, `RawPayload`, `IsProcessed`, `ProcessingError`, `Signature`). |
| `PaymentAuditLog` | Request/response audit record for a provider call; `SetRequestBody` and `SetResponseBody` pass bodies through the static `MaskSensitiveData`, and `CalculateDuration` fills `DurationMs`. |
| `DisputeStatus` | Enum of dispute states. |

### Google (`AnointedAutomation.Objects.Google`)

| Type | Description |
|---|---|
| `GoogleObjects` | Holds `GoogleTokenInfo` and `UserProfile`; stored on `User.Google`. |
| `GoogleTokenInfo` | Google token response and token info fields, mapped with `[JsonProperty]` to Google's snake_case names. |
| `UserProfile` | Google user info (`id`, `email`, `verifiedEmail`, `name`, `givenName`, `familyName`, `picture`, `locale`, `hd`, ...). |

## Naming convention

Properties follow the AnointedAutomation hybrid casing: value types are camelCase (`success`, `statusCode`, `createdDate`) and reference types are PascalCase (`Message`, `Data`, `Email`). The companion serializers in [AnointedAutomation.Objects.API](https://www.nuget.org/packages/AnointedAutomation.Objects.API) (`JsonCasingConvention`) and [AnointedAutomation.Repository.Mongo](https://www.nuget.org/packages/AnointedAutomation.Repository.Mongo) (`HybridElementNameConvention`) keep the JSON wire and the database on the same rule.

## Storing these models

The models have no MongoDB or EF Core attributes. For MongoDB, call `BsonClassMapRegistrar.RegisterClassMaps()` from [AnointedAutomation.Repository.Mongo](https://www.nuget.org/packages/AnointedAutomation.Repository.Mongo) at startup; it maps `User.UserId` to `_id` and registers a serializer for the `JObject` in `Profile.AccountSettings`.

## Related packages

- [AnointedAutomation.Enums](https://www.nuget.org/packages/AnointedAutomation.Enums): enums used by the billing models.
- [AnointedAutomation.Objects.API](https://www.nuget.org/packages/AnointedAutomation.Objects.API): ASP.NET Core additions (`CustomFormFile`, `JsonCasingConvention`, `GraphQlEnvelope`).
- [AnointedAutomation.Repository.Mongo](https://www.nuget.org/packages/AnointedAutomation.Repository.Mongo): MongoDB helper and BSON class maps for these models.

## License

MIT. See [LICENSE](https://github.com/AnointedAutomation/AnointedAutomation/blob/master/LICENSE).

## Support This Project

This library is free and open source. The best way to support the work is to shop with us:

- **Christian items:** [https://store.anointed.company](https://store.anointed.company)
- **Everything else:** [https://www.mart.club](https://www.mart.club)

Found a bug or have a request? Open an issue at [https://github.com/AnointedAutomation/AnointedAutomation/issues](https://github.com/AnointedAutomation/AnointedAutomation/issues).
