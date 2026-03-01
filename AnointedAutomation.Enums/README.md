# AnointedAutomation.Enums

Shared enumerations for AnointedAutomation packages.

## Overview

This package contains all shared enum definitions used across the AnointedAutomation ecosystem. By centralizing enums in a single package, we ensure consistency and avoid duplication across dependent packages.

## Enums

### PaymentType
Defines supported payment methods.
- `None` (0)
- `PayPalToken` (1)
- `MasterCard` (2)
- `Visa` (3)
- `ACH` (4)

### TransactionStatus
Represents the status of a payment transaction across various payment providers.
- `None` (0) - No status specified or unknown
- `Pending` (1) - Awaiting processing
- `Processing` (2) - Currently being processed
- `RequiresAction` (3) - Requires additional action (e.g., 3D Secure)
- `RequiresPaymentMethod` (4) - Requires payment method to be attached
- `RequiresConfirmation` (5) - Requires confirmation before processing
- `RequiresCapture` (6) - Requires capture after authorization
- `Succeeded` (7) - Completed successfully
- `Failed` (8) - Failed to process
- `Canceled` (9) - Canceled before completion
- `Refunded` (10) - Fully refunded
- `PartiallyRefunded` (11) - Partially refunded
- `Disputed` (12) - Disputed by customer
- `Expired` (13) - Expired before completion
- `Authorized` (14) - Authorized but not yet captured
- `Voided` (15) - Voided

### PaymentProvider
Defines supported payment providers.
- `None` (0)
- `Stripe` (1)
- `PayPal` (2)
- `Braintree` (3)
- `CheckoutDotCom` (4)

### SubscriptionStatus
Represents the status of a subscription.
- `None` (0)
- `Active` (1)
- `PastDue` (2)
- `Canceled` (3)
- `Unpaid` (4)
- `Trialing` (5)
- `Paused` (6)
- `Incomplete` (7)
- `IncompleteExpired` (8)

### PaymentOperation
Defines types of payment operations for audit logging.
- `None` (0)
- `Charge` (1)
- `Authorize` (2)
- `Capture` (3)
- `Refund` (4)
- `Void` (5)
- `Dispute` (6)
- `DisputeResponse` (7)
- `Chargeback` (8)
- `SubscriptionCreate` (9)
- `SubscriptionUpdate` (10)
- `SubscriptionCancel` (11)
- `SubscriptionRenew` (12)
- `PaymentMethodAdd` (13)
- `PaymentMethodRemove` (14)
- `PaymentMethodUpdate` (15)

### CardType
Defines credit/debit card types.
- `Unknown` (0)
- `Visa` (1)
- `MasterCard` (2)
- `AmericanExpress` (3)
- `Discover` (4)
- `DinersClub` (5)
- `JCB` (6)
- `UnionPay` (7)

### WebhookEventType
Defines webhook event types from payment providers.
- `Unknown` (0)
- `PaymentSucceeded` (1)
- `PaymentFailed` (2)
- `PaymentRefunded` (3)
- `SubscriptionCreated` (4)
- `SubscriptionUpdated` (5)
- `SubscriptionCanceled` (6)
- `SubscriptionRenewed` (7)
- `DisputeCreated` (8)
- `DisputeUpdated` (9)
- `DisputeClosed` (10)
- `CustomerCreated` (11)
- `CustomerUpdated` (12)
- `InvoiceCreated` (13)
- `InvoicePaid` (14)
- `InvoicePaymentFailed` (15)

## Usage

```csharp
using AnointedAutomation.Enums;

PaymentType paymentMethod = PaymentType.Visa;
TransactionStatus status = TransactionStatus.Succeeded;
PaymentProvider provider = PaymentProvider.Stripe;
```

## License

MIT License - Copyright © 2023 Anointed Automation, LLC

Created by Alexander Fields - https://www.alexanderfields.me
