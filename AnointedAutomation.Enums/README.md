# AnointedAutomation.Enums

Shared enumerations for the AnointedAutomation packages: payment methods, providers, transaction and subscription states, payment operations, card brands, webhook event types, and moderation categories. Centralizing them in one small package keeps the numeric values consistent across every package and service that stores or exchanges them.

[![NuGet](https://img.shields.io/nuget/v/AnointedAutomation.Enums.svg)](https://www.nuget.org/packages/AnointedAutomation.Enums) [![Downloads](https://img.shields.io/nuget/dt/AnointedAutomation.Enums.svg)](https://www.nuget.org/packages/AnointedAutomation.Enums)

## Installation

```bash
dotnet add package AnointedAutomation.Enums
```

- Target framework: `net10.0`
- Dependencies: none

## Quick start

```csharp
using AnointedAutomation.Enums;

PaymentProvider provider = PaymentProvider.Stripe;
TransactionStatus status = TransactionStatus.Succeeded;
CardType card = CardType.Visa;

bool settled = status == TransactionStatus.Succeeded || status == TransactionStatus.PartiallyRefunded;
```

## Enums

All enums live in the `AnointedAutomation.Enums` namespace. Every value is assigned explicitly, so persisted integers stay stable.

| Enum | Underlying type | Purpose |
|---|---|---|
| `PaymentType` | `int` | Supported payment methods. |
| `PaymentProvider` | `int` | Supported payment gateway providers. |
| `TransactionStatus` | `int` | Status of a payment transaction across providers. |
| `SubscriptionStatus` | `int` | Status of a subscription. |
| `PaymentOperation` | `int` | Operations performed against a payment processor, for audit logging. |
| `CardType` | `int` | Credit and debit card brands. |
| `WebhookEventType` | `int` | Common webhook event types across payment providers. |
| `Sin` | `uint` | Moderation categories: prohibited conduct that can get a person flagged or banned on Anointed Automation platforms. |

### PaymentType

`None` (0), `PayPalToken` (1), `MasterCard` (2), `Visa` (3), `ACH` (4)

### PaymentProvider

`None` (0), `Stripe` (1), `PayPal` (2), `Braintree` (3), `Checkout` (4, Checkout.com), `Square` (5), `Adyen` (6), `AuthorizeNet` (7)

### TransactionStatus

`None` (0), `Pending` (1), `Processing` (2), `RequiresAction` (3, e.g. 3D Secure), `RequiresPaymentMethod` (4), `RequiresConfirmation` (5), `RequiresCapture` (6), `Succeeded` (7), `Failed` (8), `Canceled` (9), `Refunded` (10), `PartiallyRefunded` (11), `Disputed` (12), `Expired` (13), `Authorized` (14), `Voided` (15)

### SubscriptionStatus

`None` (0), `Active` (1), `Trialing` (2), `PastDue` (3), `Cancelled` (4), `Suspended` (5), `Paused` (6), `Expired` (7), `Pending` (8), `NotRenewed` (9)

### PaymentOperation

`Unknown` (0), `CreateCharge` (1), `CreateRefund` (2), `CreateCustomer` (3), `GetCustomer` (4), `UpdateCustomer` (5), `DeleteCustomer` (6), `CreateSubscription` (7), `UpdateSubscription` (8), `CancelSubscription` (9), `PauseSubscription` (10), `ResumeSubscription` (11), `GetTransaction` (12), `VerifyWebhook` (13), `HandleWebhookEvent` (14), `CreatePaymentIntent` (15), `ConfirmPaymentIntent` (16), `CancelPaymentIntent` (17), `CapturePayment` (18), `VoidPayment` (19), `AddPaymentMethod` (20), `RemovePaymentMethod` (21), `SetDefaultPaymentMethod` (22), `CreateInvoice` (23), `PayInvoice` (24), `VoidInvoice` (25)

### CardType

`Unknown` (0), `Visa` (1), `MasterCard` (2), `AmericanExpress` (3), `Discover` (4), `DinersClub` (5), `JCB` (6), `UnionPay` (7), `Maestro` (8)

### WebhookEventType

`Unknown` (0), `PaymentCreated` (1), `PaymentSucceeded` (2), `PaymentFailed` (3), `PaymentCanceled` (4), `RefundCreated` (5), `RefundSucceeded` (6), `RefundFailed` (7), `DisputeCreated` (8), `DisputeUpdated` (9), `DisputeClosed` (10), `CustomerCreated` (11), `CustomerUpdated` (12), `CustomerDeleted` (13), `SubscriptionCreated` (14), `SubscriptionUpdated` (15), `SubscriptionCanceled` (16), `SubscriptionTrialEnding` (17), `InvoiceCreated` (18), `InvoicePaid` (19), `InvoicePaymentFailed` (20), `PaymentMethodAttached` (21), `PaymentMethodDetached` (22), `PayoutCreated` (23), `PayoutPaid` (24), `PayoutFailed` (25)

### Sin

Grounded in Galatians 5:19-21 and the classic deadly sins. Serialized as its unsigned integer value.

`Blasphemy` (0), `Idolatry` (1), `Sorcery` (2), `SexualImmorality` (3), `Pride` (4), `Greed` (5), `Lust` (6), `Wrath` (7), `Envy` (8), `Gluttony` (9), `Sloth` (10), `Deceit` (11), `Theft` (12), `Murder` (13)

Note the spellings: `TransactionStatus.Canceled` and `WebhookEventType.SubscriptionCanceled` use one "l", while `SubscriptionStatus.Cancelled` uses two.

## Related packages

- [AnointedAutomation.Objects](https://www.nuget.org/packages/AnointedAutomation.Objects): shared domain objects that use these enums.

## License

MIT. See [LICENSE](https://github.com/AnointedAutomation/AnointedAutomation/blob/master/LICENSE).
Copyright © Anointed Automation, LLC. Stewarded by Alexander Fields.

## Support This Project

This library is free and open source. The best way to support the work is to shop with us:

- **Christian items:** [https://store.anointed.company](https://store.anointed.company)
- **Everything else:** [https://www.mart.club](https://www.mart.club)

Found a bug or have a request? Open an issue: [https://github.com/AnointedAutomation/AnointedAutomation/issues](https://github.com/AnointedAutomation/AnointedAutomation/issues)
