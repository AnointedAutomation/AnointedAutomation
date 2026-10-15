# AnointedAutomation.Objects

Jesus is King ✝️

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
| `User` | Account record: `UserId`, `Email` (mirrors the primary `Emails` entry), `Emails`, `Username`, `Password`, `Role`, `Profile`, `Token`, `tokenExpiration`, `emailConfirmed`, `IPAddresses`, `Friends`, `FriendId`, `BlockedUsers`, `isBanned`, `banned`, `BannedReason`, `createdDate`, `lastActiveDate`, `timeOnline`, `SSO`, `Meta`, and account deletion fields (`deletionDate`, `DeletionConfirmationCode`, `deletionConfirmationExpiration`). Constructors: parameterless; a full constructor taking every settable property in declaration (BSON) order and assigning each exactly as passed (no defaults); and the legacy 14-parameter constructor, which defaults `banned` to 1900-01-01, `createdDate`/`lastActiveDate` to `DateTime.UtcNow`, a null `IPAddresses` to empty and a null `Profile` to new. A null password is accepted by both (SSO-only accounts). |
| `UserEmail` | One address on a user: `Address` (lower-cased, trimmed), `Verified`, `VerifiedAt`, `IsPrimary`, `Source`, `AddedAt`. |
| `UserEmailSource` | Enum: `Unknown` (0), `Manual`, `Signup`, `Google`, `Microsoft`, `Apple`, `Facebook`, `Shopify`, `Merge`, `Admin`, `Discord`, `GitHub`, `LinkedIn`, `Twitch`, `Yahoo`, `Amazon`, `ShopifyCustomerAccount`. |
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

### Discord (`AnointedAutomation.Objects.Discord`)

| Type | Description |
|---|---|
| `DiscordLogin` | Holds the Discord `User`; stored on `User.SSO.Discord`. |
| `DiscordUser` | Discord's full documented User object as `/users/@me` returns it (`id`, `username`, `discriminator`, `global_name`, `avatar`, `bot`, `system`, `mfa_enabled`, `banner`, `accent_color`, `locale`, `verified`, `email`, `flags`, `premium_type`, `public_flags`, `avatar_decoration_data`, `collectibles`, `primary_guild`), mapped with `[JsonProperty]` to Discord's snake_case names. |
| `DiscordAvatarDecorationData`, `DiscordCollectibles`, `DiscordNameplate`, `DiscordPrimaryGuild` | The nested Discord user structures. |

### GitHub (`AnointedAutomation.Objects.GitHub`)

| Type | Description |
|---|---|
| `GitHubLogin` | Holds `User`, `VerifiedPrimaryEmail` (the primary AND verified address) and `Emails`; stored on `User.SSO.GitHub`. |
| `GitHubUser`, `GitHubPlan` | GitHub's "Private User" object as `GET /user` returns it (every documented field, snake_case `[JsonProperty]` names). |
| `GitHubEmail` | One `GET /user/emails` entry (`email`, `primary`, `verified`, `visibility`). |

### LinkedIn (`AnointedAutomation.Objects.LinkedIn`)

| Type | Description |
|---|---|
| `LinkedInLogin` | Holds `UserInfo`; stored on `User.SSO.LinkedIn`. |
| `LinkedInUserInfo` | OIDC userinfo (`sub`, `name`, `given_name`, `family_name`, `picture`, `locale`, `email`, `email_verified`). |
| `LinkedInLocale`, `LinkedInLocaleConverter` | `locale` as `{country, language}`; reads either the object or a `"en-US"` string. |

### Twitch (`AnointedAutomation.Objects.Twitch`)

| Type | Description |
|---|---|
| `TwitchLogin` | Holds `User` and `IdToken`; stored on `User.SSO.Twitch`. |
| `TwitchUser` | One Get Users entry (`id`, `login`, `display_name`, `type`, `broadcaster_type`, `description`, `profile_image_url`, `offline_image_url`, `view_count`, `email`, `created_at`). |
| `TwitchIdTokenClaims` | OIDC claims (`aud`, `azp`, `exp`, `iat`, `iss`, `sub`, `nonce`, `email`, `email_verified`, `picture`, `preferred_username`, `updated_at`). |

### Yahoo, Amazon, Reddit, Shopify Customer Account

| Type | Description |
|---|---|
| `YahooLogin`, `YahooUserInfo` | Yahoo OIDC claims (`sub`, `name`, `given_name`, `family_name`, `nickname`, `email`, `email_verified`, `birthdate`, `locale`, `picture`, plus token claims); stored on `User.SSO.Yahoo`. |
| `AmazonLogin`, `AmazonProfile` | Login with Amazon profile (`user_id`, `name`, `email`, `postal_code`); stored on `User.SSO.Amazon`. |
| `RedditLogin`, `RedditUser` | Reddit `GET /api/v1/me` identity (no email; `has_verified_email` only); stored on `User.SSO.Reddit`. |
| `ShopifyCustomerAccountLogin`, `ShopifyCustomerAccountClaims` | Shopify Customer Account API id_token claims (`iss`, `sub`, `aud`, `exp`, `iat`, `nonce`, `sid`, `email`, `email_verified`, `given_name`, `family_name`); stored on `User.SSO.ShopifyCustomerAccount`, separate from the `User.SSO.Shopify` store customer link. |

No slot ever holds an access or refresh token.

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
- **Pets:** [https://www.animaldander.com](https://www.animaldander.com)
- **Everything else:** [https://www.mart.club](https://www.mart.club)

Found a bug or have a request? Open an issue at [https://github.com/AnointedAutomation/AnointedAutomation/issues](https://github.com/AnointedAutomation/AnointedAutomation/issues).
