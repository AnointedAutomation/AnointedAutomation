[← Back to Dictionary](./PROJECT_STRUCTURE_DICTIONARY.md)

# PROJECT STRUCTURE LIBRARIES

## Solution Overview
The AnointedAutomation solution contains 10 core libraries targeting .NET 8.0/net10.0, each focusing on specific functionality areas. All libraries are designed as NuGet packages with MIT licensing. All code is fully documented with XML documentation.

## Library Details

### 1. AnointedAutomation.Enums v0.0.1

**Purpose & Functionality:**
- Shared enumeration types used across multiple AnointedAutomation libraries
- Provides standardized payment and transaction status values
- Defines common webhook event types for payment provider integration

**Key Enums:**
- `PaymentType` - Defines supported payment methods
  - None (0) - No payment type specified
  - PayPalToken (1) - PayPal payment token
  - MasterCard (2) - MasterCard credit card
  - Visa (3) - Visa credit card
  - ACH (4) - Automated Clearing House bank transfer

- `PaymentProvider` - Defines supported payment gateway providers
  - None (0), Stripe (1), PayPal (2), Braintree (3), Checkout (4), Square (5), Adyen (6), AuthorizeNet (7)

- `TransactionStatus` - Payment transaction status across providers
  - None, Pending, Processing, RequiresAction, RequiresPaymentMethod, RequiresConfirmation
  - RequiresCapture, Succeeded, Failed, Canceled, Refunded, PartiallyRefunded
  - Disputed, Expired, Authorized, Voided

- `WebhookEventType` - Common webhook event types across payment providers
  - Payment events: PaymentCreated, PaymentSucceeded, PaymentFailed, PaymentCanceled
  - Refund events: RefundCreated, RefundSucceeded, RefundFailed
  - Dispute events: DisputeCreated, DisputeUpdated, DisputeClosed
  - Customer events: CustomerCreated, CustomerUpdated, CustomerDeleted
  - Subscription events: SubscriptionCreated, SubscriptionUpdated, SubscriptionCanceled, SubscriptionTrialEnding
  - Invoice events: InvoiceCreated, InvoicePaid, InvoicePaymentFailed
  - Payment method events: PaymentMethodAttached, PaymentMethodDetached
  - Payout events: PayoutCreated, PayoutPaid, PayoutFailed

**Dependencies:**
- **Internal:** None (foundational library)
- **External:** .NET 8.0 framework only

**Public API Surface:**
- All enums are public and accessible via `AnointedAutomation.Enums` namespace

### 2. AnointedAutomation.Logging v0.0.8

**Purpose & Functionality:**
- Provides standardized logging framework for .NET applications
- Event-driven logging system with predefined message types
- Advanced async method name resolution for better debugging

**Key Classes:**
- `LogMessage` - Core logging object with automatic source tracking
  - Supports various message types (Error, Warning, Success, etc.)
  - Automatic method name resolution for async methods
  - Event-based logging system with LogAdded events
  - Customizable message source for application identification
  - Provides factory methods for each message type
  - Supports message ID tracking and timestamps
  - Complete XML documentation for all methods and properties

- `MessageType` enum - Defines log levels
  - Error (0) - Indicates an error condition that should be investigated
  - Warning (1) - Indicates a potential issue that may require attention
  - Success (2) - Indicates a successful operation or event
  - Informational (3) - Provides general information about system operation
  - Message (4) - A general message without specific severity level
  - Critical (5) - Indicates a severe error that requires immediate attention
  - Celebrate (6) - Indicates a significant achievement or milestone

- `LogMessageEventArgs` - Event arguments for log events
  - Contains the log message associated with the event
  - Used for event-based log handling

**Dependencies:**
- **Internal:** None (foundational library)
- **External:** .NET 8.0 framework only

**Public API Surface:**
- Static factory methods: `LogMessage.Error()`, `LogMessage.Warning()`, etc.
- Event system: `LogMessage.LogAdded` event
- Properties: `id`, `localOperationName`, `message`, `messageSource`, `messageType`, `timeStamp`
- Static property: `MessageSourceSetter` for application identification

### 3. AnointedAutomation.Memory v0.0.5

**Purpose & Functionality:**
- Memory management utilities for .NET applications
- Controlled garbage collection with OS memory return
- Performance optimization through manual GC triggers

**Key Classes:**
- `GarbageCollection` - Main class for memory management operations
  - Provides controlled garbage collection triggers
  - Attempts to return unused memory to the OS
  - Uses no-GC regions for optimal memory management
  - Includes finalizer and disposal pattern implementation
  - Full XML documentation for memory management operations

**Dependencies:**
- **Internal:** None
- **External:** .NET 8.0 framework only

**Public API Surface:**
- `PerformGarbageCollection(object state)` - Triggers GC and memory cleanup
- `ReturnUnusedMemoryToOS()` - Private method for OS memory return
- Uses .NET's no-GC region feature for optimization

### 4. AnointedAutomation.APIMiddlewares v0.0.16

**Purpose & Functionality:**
- ASP.NET Core middleware collection for API security and monitoring
- IP blacklisting, endpoint tracking, and API key validation
- Memory management integration for idle endpoints

**Key Classes:**
- `IPBlacklistMiddleware` - Blocks malicious IPs
  - Handles IP blocking and access control
  - Maintains blocked IP list
  - Provides event-based logging
  - Real-time IP validation and blocking
  - Complete XML documentation for security features

- `InvalidEndpointTrackerMiddleware` - Tracks failed endpoint attempts
  - Monitors invalid endpoint access
  - Records attempt patterns
  - Provides automated IP blocking
  - Logs security events
  - Fully documented security handling

- `EndpointAccessMiddleware` - Monitors endpoint usage and manages memory
  - Tracks endpoint access times
  - Manages idle endpoint cleanup
  - Integrates with garbage collection
  - Memory optimization for inactive endpoints
  - XML documentation for configuration and usage

- `APIKeyAttribute` - API key validation filter
  - Request validation through attribute
  - Environment-based configuration
  - Secure key validation
  - Complete documentation of security implications

- `BannedIP` - Data model for blocked IPs
  - Stores IP and ban information
  - Tracks ban reasons and timestamps
  - Supports ban management

- `IPBlacklist` - Static IP management class
  - Global IP block list
  - Ban/unban functionality
  - IP validation methods

- `APIUtility` - IP address extraction utilities
  - Request IP parsing
  - Support for forwarded headers
  - IP validation helpers

**Dependencies:**
- **Internal:**
  - AnointedAutomation.Logging (logging functionality)
  - AnointedAutomation.Memory (garbage collection)
- **External:**
  - Microsoft.AspNetCore (2.3.0)
  - Microsoft.AspNetCore.Mvc.Core (2.3.0)

**Public API Surface:**
- Middleware registration methods
- Static methods: `IPBlacklist.AddBannedIP()`, `IPBlacklist.IsIPBlocked()`
- Event system for logging and IP banning
- Environment variable configuration: `API_KEY`, `API_KEY_NAME`

### 5. AnointedAutomation.Repository.Mongo v0.0.11

**Purpose & Functionality:**
- MongoDB database operations wrapper
- Generic CRUD operations with strongly-typed interfaces
- Connection management and logging integration
- Base document classes for MongoDB entities

**Key Classes:**
- `MongoDocument` - Abstract base class for MongoDB documents
  - Provides `Id` property with proper `[BsonId]` and `[BsonRepresentation(BsonType.ObjectId)]` attributes
  - Includes `[BsonIgnoreExtraElements]` for forward compatibility
  - Inherit from this for any MongoDB-persisted entity

- `AuditableMongoDocument` - Base class with audit timestamps
  - Extends `MongoDocument` with `createdAt` and `updatedAt` properties (DateTime = value type = camelCase)
  - Use for entities requiring creation/modification tracking

- `IMongoHelper` - Interface defining MongoDB operations
  - Defines standard CRUD operations
  - Connection management methods
  - Database interaction patterns
  - Complete XML documentation for interface contract

- `MongoHelper` - Implementation of MongoDB operations
  - Implements CRUD operations
  - Handles database connections
  - Provides logging integration
  - Manages MongoDB collections
  - Supports async operations
  - Includes connection string building
  - Document ID extraction utilities
  - Full documentation of all database operations

- `MongoHelperFactory` - Factory for creating configured instances
  - Creates pre-configured MongoHelper instances
  - Manages database connections
  - Handles configuration
  - XML documentation for factory pattern usage

**Dependencies:**
- **Internal:**
  - AnointedAutomation.Logging (for operation logging)
- **External:**
  - MongoDB.Bson (3.4.0)
  - MongoDB.Driver (3.4.0)
  - MongoDB.Driver.Core (2.30.0)
  - MongoDB.Driver.GridFS (2.30.0)
  - MongoDB.Libmongocrypt (1.12.0)

**Public API Surface:**
- Base classes: `MongoDocument`, `AuditableMongoDocument`
- CRUD operations: `CreateDocumentAsync<T>()`, `GetAllDocumentsAsync<T>()`, `UpdateDocumentAsync<T>()`, `DeleteDocumentAsync<T>()`
- Connection management: `CreateMongoDbInstance()`, `TestConnection()`
- Utility methods: `ConnectionStringBuilder()`, `GetIdFromObj<T>()`
- Event system: `LogAdded`, `LogCleared` events

### 6. AnointedAutomation.Objects v2.0.0

**Purpose & Functionality:**
- Core data models for general application development
- Account management and billing system models
- MongoDB and JSON serialization support

**Breaking Change (2.0.0):** The Concepts namespace (Love, Agape, SacrificialLove, SelfSeekingLove,
the behavior tree machinery, and the Reality subtree) was migrated out of this library into the new
AnointedAutomation.Concepts package. Consumers referencing `AnointedAutomation.Objects.Concepts` must
now reference AnointedAutomation.Concepts and use the `AnointedAutomation.Concepts` namespace instead.

**Key Components:**

**Account Namespace:**
- `User` - User account with authentication and profile data
  - Authentication and authorization data
  - Profile information
  - Token management
  - IP tracking
  - Account status and activity tracking
  - Complete XML documentation for security considerations

- `Profile` - User profile information
  - Personal information
  - Account settings
  - Contact details
  - Full property documentation

- `Credentials` - Authentication credentials
  - Login information
  - Token storage
  - OAuth integration
  - Security documentation

- `IPInfo` - IP address and location tracking
  - IP address storage
  - Login history
  - Access tracking
  - Activity monitoring

**Billing Namespace:**
- `Product`, `Purchase`, `Sale` - Product and transaction models
- `Subscription` - Recurring billing
- `Bill` - Invoice generation
- `CreditCard`, `PaymentCredentials` - Payment processing (PaymentType moved to AnointedAutomation.Enums)
- `Address`, `Contact` - Contact information
- `Inventory` - Inventory tracking
- `PaymentIntent` - Standardized payment intent model for Stripe, PayPal, Braintree, Checkout.com
- `PaymentCustomer` - Customer profile stored with payment providers
- `PaymentMethodToken` - Tokenized payment method storage
- `Refund` - Standardized refund object across payment providers
- `Dispute` - Chargeback/dispute handling with DisputeStatus enum
- `WebhookEvent` - Webhook event handling for payment provider notifications
All billing models include complete XML documentation

**Core Classes:**
- `ResponseData` - Generic response wrapper
- `JObjectSerializer` - Custom BSON/JSON serialization

**Dependencies:**
- **Internal:**
  - AnointedAutomation.Enums (PaymentType, TransactionStatus, PaymentProvider, WebhookEventType enums)
- **External:**
  - Newtonsoft.Json (13.0.4)

### 7. AnointedAutomation.Objects.API v0.0.14

**Purpose & Functionality:**
- API-specific data models extending AnointedAutomation.Objects
- ASP.NET Core integration for web APIs
- File upload handling and API response standardization

**Key Classes:**
- All classes from AnointedAutomation.Objects (same structure, different namespace)
- `CustomFormFile` - IFormFile implementation for multipart uploads
  - Complete XML documentation for file handling
- API-specific `ResponseData` class
  - Full documentation of response structure

**Dependencies:**
- **Internal:** None (contains duplicate models from AnointedAutomation.Objects)
- **External:**
  - MongoDB.Driver (3.4.0)
  - MongoDB.Driver.Core (2.30.0)
  - MongoDB.Bson (3.4.0)
  - MongoDB.Driver.GridFS (2.30.0)
  - MongoDB.Libmongocrypt (1.12.0)
  - Microsoft.AspNetCore.Mvc.NewtonsoftJson (8.0.17)
  - Newtonsoft.Json (13.0.3)

### 8. AnointedAutomation.Objects.Mongo v0.0.1

**Purpose & Functionality:**
- MongoDB-specific data models with BSON serialization attributes
- Extends base AnointedAutomation.Objects classes with MongoDB support
- Clean separation of MongoDB dependencies from base models

**Key Classes:**

**Account Namespace:**
- `MongoUser` - MongoDB-specific user extending base `User`
  - Adds `[BsonId]` attribute to UserId property
  - Adds `[BsonIgnoreExtraElements]` for flexible schema evolution
  - Full constructor matching base User class
  - Complete XML documentation

- `MongoProfile` - MongoDB-specific profile extending base `Profile`
  - Adds `[BsonIgnoreExtraElements]` attribute
  - Custom BSON serialization for JObject properties
  - Full constructor matching base Profile class

**Utilities:**
- `JObjectSerializer` - Custom serializer for JObject to BSON conversion
  - Enables seamless storage of Newtonsoft.Json JObjects in MongoDB
  - Handles null values correctly
  - Bidirectional serialization/deserialization
  - Complete XML documentation

**Dependencies:**
- **Internal:**
  - AnointedAutomation.Objects (base data models)
- **External:**
  - MongoDB.Bson (3.6.0)
  - Newtonsoft.Json (13.0.4)

**Public API Surface:**
- MongoDB-ready classes: `MongoUser`, `MongoProfile`
- Serialization: `JObjectSerializer` for JObject to BSON conversion
- All classes inherit from base Objects and can be cast to base types

### 9. AnointedAutomation.Concepts v1.0.0

**Purpose & Functionality:**
- Concept modeling classes migrated out of AnointedAutomation.Objects (Love, Agape, SacrificialLove,
  SelfSeekingLove, the behavior tree machinery, Situation/Circumstances, and the Reality subtree)
- A new epistemics engine (Epistemics/) that checks consistency of theological claims against
  foundational claims over a shared proposition vocabulary
- See PROJECT_STRUCTURE_CODE.md sections 4a for full class-level detail and the design spec at
  `docs/superpowers/specs/2026-07-02-theology-engine-design.md`

**Key Namespaces:**
- `AnointedAutomation.Concepts` - Love/Agape/Reality concept model
- `AnointedAutomation.Concepts.Epistemics` - Proposition, Testability, LawDomain, EpistemicStatus,
  FoundationalClaim, TheologicalClaim, Verdict, DerivationStep, Examination, Tension, EpistemicLedger

**Dependencies:**
- **Internal:** None (foundational for Concepts/Mathematics)
- **External:** .NET 10.0 (net10.0) framework only

### 10. AnointedAutomation.Mathematics v1.0.0

**Purpose & Functionality:**
- Curated catalogs of foundational and theological claims drawn from mathematics and physics, for use
  with the AnointedAutomation.Concepts.Epistemics engine
- `UniversalPropositions` - shared proposition vocabulary
- `UniversalLaws` - Law status claims (NonContradiction, Identity, ExcludedMiddle, Causality,
  ConservationOfEnergy, EntropyIncrease)
- `PhysicalTheories` - Theory status claims (MassEnergyEquivalence, InvariantLightSpeed)
- `Conjectures` - Conjecture status claims at zero weight (Collatz, Goldbach, RiemannHypothesis)

**Dependencies:**
- **Internal:**
  - AnointedAutomation.Concepts (Epistemics engine types)
- **External:** .NET 10.0 (net10.0) framework only

## Inter-Library Dependencies

```
AnointedAutomation.Enums
└── (no internal dependencies - foundational library)

AnointedAutomation.APIMiddlewares
├── AnointedAutomation.Logging (logging)
└── AnointedAutomation.Memory (garbage collection)

AnointedAutomation.Repository.Mongo
└── AnointedAutomation.Logging (operation logging)

AnointedAutomation.Memory
└── (no internal dependencies)

AnointedAutomation.Logging
└── (no internal dependencies)

AnointedAutomation.Objects
└── AnointedAutomation.Enums (payment and transaction enums)

AnointedAutomation.Objects.API
└── (no internal dependencies)

AnointedAutomation.Objects.Mongo
└── AnointedAutomation.Objects (base models)

AnointedAutomation.Concepts
└── (no internal dependencies)

AnointedAutomation.Mathematics
└── AnointedAutomation.Concepts (Epistemics engine types)
```

## Key Architectural Notes

1. **Namespace Structure**: Libraries use `AnointedAutomation.{Feature}` pattern with sub-namespaces
2. **Duplicate Models**: AnointedAutomation.Objects and AnointedAutomation.Objects.API contain identical models in different namespaces
3. **MongoDB Integration**: Heavy use of MongoDB across multiple libraries
4. **Event-Driven Architecture**: Logging and middleware components use .NET events
5. **NuGet Package Strategy**: All libraries configured for NuGet distribution
6. **Versioning**: Libraries are at different version stages (0.0.4 to 0.0.13)
7. **Documentation**: All classes, methods, and properties have complete XML documentation
8. **Test Coverage**: Unit tests include XML documentation of test scenarios
---

**[← Back to Project Dictionary](./PROJECT_STRUCTURE_DICTIONARY.md)**
