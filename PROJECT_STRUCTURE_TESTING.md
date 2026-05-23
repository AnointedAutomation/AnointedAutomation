[← Back to Dictionary](./PROJECT_STRUCTURE_DICTIONARY.md)

# PROJECT STRUCTURE TESTING

## Test Organization
All test projects are organized under a "Tests" solution folder in the main .sln file.

## Test Projects Structure

### 1. AnointedAutomation.Logging.Tests
**Test Files:**
- `LogMessageTests.cs` - Tests for LogMessage class (49 tests - ENHANCED)
  - Comprehensive edge case coverage added (null, empty, long strings, special chars, Unicode)
  - Event handling tests (LogAdded event)
  - Boundary value tests (id = 0, -1, int.MaxValue)
  - All factory method edge cases covered
- `MessageTypeTests.cs` - Tests for MessageType enum

**Testing Framework:** Xunit (.NET 8.0)
**Test Coverage:** ✅ EXCELLENT - Meets CLAUDE_TESTING.md standards (Success, Failure, Edge scenarios)

### 2. AnointedAutomation.Memory.Tests
**Test Files:**
- `GarbageCollectionTests.cs` - Tests for GarbageCollection class (5 tests)

**Testing Framework:** Xunit (.NET 8.0)
**Test Coverage:** ⚠️ Basic - Could benefit from additional edge case testing

### 3. AnointedAutomation.APIMiddlewares.Tests
**Test Files:**
- `EndpointAccessMiddlewareTests.cs` - Tests for endpoint access middleware (27 tests - ENHANCED)
  - Constructor tests (timeout, cleanMem, null timeout, zero, negative, very large)
  - Invoke path tracking (empty, root, long paths, special characters, numbers)
  - HasBeenHitRecently edge cases (null throws ArgumentNullException, empty path)
  - Exception propagation and path tracking when next throws
  - Multiple request tracking, query string handling
- `IPBlacklistMiddlewareTests.cs` - Tests for IP blacklist middleware
- `InvalidEndpointTrackerMiddlewareTests.cs` - Tests for endpoint tracking (28 tests - ENHANCED)
  - Constructor and basic invoke tests
  - ClearFailedAttempts edge cases (multiple times, does not throw)
  - Multiple failed attempts leading to ban
  - Same path multiple times counting
  - Valid endpoint (200), server error (500), .env paths, case sensitivity
  - Very long paths, special characters, response body restoration
- `AttemptInfoTests.cs` - Tests for attempt tracking objects

**Subdirectories:**
- `Filters/APIKeyAttributeTests.cs` - Tests for API key validation filter
- `Objects/BannedIPTests.cs` - Tests for BannedIP objects
- `Objects/IPBlacklistTests.cs` - Tests for IPBlacklist functionality (58 total tests - ENHANCED)
  - Comprehensive edge case coverage added (null ip/reason, empty strings, whitespace)
  - ArgumentNullException tests for null parameters
  - Special character and very long string tests
  - IsIPBlocked logging verification test
- `Utility/APIUtilityTests.cs` - Tests for API utility methods

**Testing Framework:** Xunit (.NET 10.0)
**Test Coverage:** EXCELLENT - Meets CLAUDE_TESTING.md standards

### 4. AnointedAutomation.Repository.Mongo.Tests
**Test Files:**
- `MongoHelperTests.cs` - Tests for MongoDB operations (69 tests - COMPREHENSIVE)
  - MongoHelper constructor and property tests
  - ConnectionStringBuilder tests (URL encoding, special characters)
  - GetIdFromObj and GetIdFromObjAsString reflection tests
  - Async CRUD operations (CreateDocumentAsync, DeleteDocumentAsync, GetAllDocumentsAsync, etc.)
  - GetAllDocumentIdsAsync projection tests
  - Failure scenarios (database errors, document not found)
  - Edge cases (empty collections, no matches)
- `MongoHelperFactoryTests.cs` - Tests for factory caching behavior
- `BsonClassMapRegistrarTests.cs` - Tests for BSON class map registration
- `JObjectSerializerTests.cs` - Tests for JObject serialization
- `MongoDocumentTests.cs` - Tests for MongoDocument and AuditableMongoDocument classes

**Testing Framework:** Xunit (.NET 10.0)
**Test Coverage:** EXCELLENT - 100% line coverage on MongoHelper async CRUD operations, 90% branch coverage

### 5. AnointedAutomation.Repository.MySql.Tests
**Test Files:**
- `MySqlHelperTests.cs` - Tests for MySQL repository operations (97 tests - ENHANCED)
  - GenericRepository CRUD operations
  - MySqlHelper async CRUD operations
  - MySqlHelperFactory caching behavior
  - DynamicDbContext initialization
  - ConnectionStringBuilder formatting (12 edge case tests added)
    - Empty strings for server, database, username, password
    - Special characters and unicode in password
    - Very long strings, whitespace values
    - Port boundary values (0, negative, max int)
  - Logging edge cases (7 tests added)
    - GetLogs returns array not original collection
    - AddLog maintains order
    - ClearLogs edge cases (empty, multiple times)
    - LogAdded and LogCleared event verification
  - Property edge cases (6 tests added)
    - DbName, ConnectionString, Database can be set to null/empty
    - Multiple set operations
  - MySqlHelperFactory caching tests (5 new tests)
    - Cache hit path testing via reflection
    - Different instances for different connection strings
    - Remove returns true when cached instance exists
    - ClearCache removes all cached instances
    - Multiple calls with same connection string return same instance
  - Failure scenarios (null database, null entities, null predicates)

**Testing Framework:** Xunit (.NET 10.0)
**Test Coverage:** EXCELLENT - Comprehensive edge case coverage, MySqlHelperFactory at 100% branch coverage

### 6. AnointedAutomation.Objects.API.Tests
**Test Files:**
- `CustomFormFileTests.cs` - Tests for CustomFormFile class (37 tests)
  - Constructor tests (success, failure, null parameters)
  - Property tests (FileName, Content, ContentDisposition, ContentType, Headers, Length, Name)
  - CopyTo synchronous stream operations
  - CopyToAsync asynchronous stream operations
  - OpenReadStream tests
  - IFormFile interface implementation
  - Edge cases (special characters, Unicode, long filenames, binary content)
  - Failure scenarios (null content operations)

**Testing Framework:** Xunit (.NET 10.0)
**Test Coverage:** EXCELLENT - Meets CLAUDE_TESTING.md standards (Success, Failure, Edge scenarios)

### Missing Test Projects
The following libraries do not have corresponding test projects:
- **AnointedAutomation.Objects** - No test project found (pure DTOs/POCOs - no tests needed)
- **AnointedAutomation.Enums** - No test project found (enum definitions - no tests needed)

## Test Patterns and Standards

### Naming Conventions
- Test projects follow pattern: `{LibraryName}.Tests`
- Test files follow pattern: `{ClassUnderTest}Tests.cs`
- Test methods typically use descriptive names

### Framework Consistency
- All existing test projects use **Xunit** framework
- All target **.NET 8.0** framework
- Consistent project structure with `bin/` and `obj/` directories

### Test Coverage

**Overall Test Statistics (Updated 2026-03-28):**
- **Total Tests:** 370 (All PASSING)
  - AnointedAutomation.Memory.Tests: 5 tests
  - AnointedAutomation.Logging.Tests: 49 tests (FULLY ENHANCED)
  - AnointedAutomation.APIMiddlewares.Tests: 113 tests (FULLY ENHANCED - +26 tests)
  - AnointedAutomation.Repository.Mongo.Tests: 69 tests (COMPREHENSIVE - 100% line coverage on async CRUD)
  - AnointedAutomation.Repository.MySql.Tests: 97 tests (ENHANCED - +30 tests including factory caching)
  - AnointedAutomation.Objects.API.Tests: 37 tests (comprehensive coverage)
- **Improvement:** +56 tests from initial baseline (+17.8% increase from 314)

**CLAUDE_TESTING.md Compliance:**
- **AnointedAutomation.Logging.Tests** - EXCEEDS STANDARDS (Success, Failure, Null/Edge scenarios)
- **AnointedAutomation.APIMiddlewares.Tests** - EXCEEDS STANDARDS (Success, Failure, Null/Edge scenarios)
  - EndpointAccessMiddlewareTests: 27 tests (comprehensive - NEW)
  - InvalidEndpointTrackerMiddlewareTests: 28 tests (comprehensive - NEW)
  - IPBlacklistTests: 21 tests (comprehensive)
  - AttemptInfoTests: 18 tests (comprehensive)
  - BannedIPTests: 13 tests (comprehensive)
  - APIUtilityTests: 17 tests (comprehensive)
- **AnointedAutomation.Repository.MySql.Tests** - EXCEEDS STANDARDS (Success, Failure, Null/Edge scenarios)
  - MySqlHelperTests: 97 tests (comprehensive - +30 edge case tests including factory caching)
  - MySqlHelperFactory: 100% branch coverage achieved
- **AnointedAutomation.Repository.Mongo.Tests** - EXCEEDS STANDARDS (Success, Failure, Null/Edge scenarios)
  - MongoHelperTests: 69 tests (comprehensive - 100% line coverage on async CRUD)
  - All async CRUD methods fully tested with mocked MongoDB driver
- **AnointedAutomation.Memory.Tests** - Basic coverage (acceptable for simple GC wrapper)

**Coverage by Library:**
- **Comprehensively Tested:**
  - AnointedAutomation.Logging (2 test files, 49 tests total)
  - AnointedAutomation.APIMiddlewares (7 test files, 113 tests total)
  - AnointedAutomation.Repository.MySql (1 test file, 97 tests total - includes GenericRepository, MySqlHelper, MySqlHelperFactory with 100% branch coverage)
  - AnointedAutomation.Repository.Mongo (5 test files, 69 tests total - 100% line coverage on async CRUD)
  - AnointedAutomation.Objects.API (1 test file, 37 tests total - CustomFormFile with edge cases)

- **Adequately Tested:**
  - AnointedAutomation.Memory (1 test file, 5 tests - appropriate for simple GC wrapper)

- **No Tests Required (Data Models Only):**
  - AnointedAutomation.Objects (pure DTOs/POCOs)
  - AnointedAutomation.Enums (enum definitions)

## Testing Gaps and Recommendations

### Comprehensive Enhancement (2025-10-22)
**Total Enhancement: +29 tests across 5 files (123 → 152 tests)**

1. **Enhanced IPBlacklistTests.cs** (+13 tests → 21 total):
   - Null parameter handling (ArgumentNullException tests)
   - Empty string and whitespace tests
   - Special character tests
   - Very long string tests (10,000 chars)
   - Logging verification tests

2. **Enhanced LogMessageTests.cs** (+25 tests → 49 total):
   - Null message tests for ALL factory methods
   - Empty string tests for all factory methods
   - Very long string tests (10,000 chars)
   - Special character and Unicode character tests
   - Event handling tests (LogAdded event)
   - Boundary value tests (id = 0, -1, int.MaxValue)
   - LogMessageEventArgs null handling

3. **Enhanced AttemptInfoTests.cs** (+13 tests → 18 total):
   - Count boundary values (0, negative, int.MaxValue)
   - Null path additions
   - Empty string and whitespace paths
   - Very long path tests (10,000 chars)
   - HashSet operations (Clear, Remove, reassign, null)

4. **Enhanced BannedIPTests.cs** (+8 tests → 13 total):
   - Null constructor parameters (all combinations)
   - Empty string constructor parameters
   - Very long strings (10,000 chars)
   - Properties set to null/empty
   - Special character handling

5. **Enhanced APIUtilityTests.cs** (+11 tests → 17 total):
   - Null RemoteIpAddress handling
   - Empty/whitespace X-Forwarded-For
   - Multiple IPs in X-Forwarded-For
   - IPv6 address handling
   - Both HttpContext and ActionExecutingContext overloads

### New Test Project Added (2026-03-28)
**AnointedAutomation.Objects.API.Tests** - 37 tests for CustomFormFile class:
- Constructor tests (default, with parameters, null handling)
- Property tests (all IFormFile interface properties)
- Stream operations (CopyTo, CopyToAsync, OpenReadStream)
- Edge cases (special characters, unicode, long strings, binary content)
- Failure scenarios (null content handling)

### Middleware and MySqlHelper Coverage Enhancement (2026-03-28)
**Total Enhancement: +51 tests across 3 files (314 -> 365 tests)**

1. **Enhanced EndpointAccessMiddlewareTests.cs** (+18 tests -> 27 total):
   - Null path handling (throws ArgumentNullException)
   - Empty path, root path, very long path, special characters, numbers
   - Timeout edge cases (zero, negative, very large)
   - Exception propagation and path tracking when next throws
   - Multiple request tracking, query string handling

2. **Enhanced InvalidEndpointTrackerMiddlewareTests.cs** (+16 tests -> 28 total):
   - ClearFailedAttempts edge cases (multiple times, does not throw)
   - Multiple failed attempts leading to ban
   - Same path multiple times counting
   - Valid endpoint (200), server error (500)
   - .env path variants, case sensitivity
   - Very long paths, special characters
   - Response body restoration verification

3. **Enhanced MySqlHelperTests.cs** (+30 tests -> 97 total):
   - ConnectionStringBuilder edge cases (12 tests)
     - Empty strings, special characters, unicode, whitespace
     - Port boundary values (0, negative, int.MaxValue)
   - Logging edge cases (7 tests)
     - GetLogs returns array not original, AddLog order
     - ClearLogs edge cases, event handling
   - Property edge cases (6 tests)
     - Null/empty string assignment, multiple set operations
   - MySqlHelperFactory caching tests (5 tests)
     - Cache hit path tested via reflection (bypasses MySQL connection requirement)
     - Different instances for different cached connection strings
     - Remove returns true when cached instance exists
     - ClearCache removes all cached instances
     - Multiple calls with same connection string return same instance
   - **Coverage Improvement:** MySqlHelperFactory branch coverage improved from 50% to 100%

### Design Decisions
1. **AnointedAutomation.Objects** - No tests needed (pure data models/DTOs with no behavioral logic)
2. **AnointedAutomation.Enums** - No tests needed (enum definitions with no behavioral logic)
3. **Middleware tests** - Existing coverage appropriate for complexity level

### Future Opportunities (Optional)
1. **AnointedAutomation.Memory.Tests** - Could add more edge cases if GC logic becomes more complex
2. **AnointedAutomation.Repository.Mongo.Tests** - Could add integration tests with actual MongoDB (requires test infrastructure)
3. **Middleware integration tests** - Could test full request/response pipelines (complex setup required)

## Build and Test Commands
Based on the .NET nature of the project:
- Build: `~/.dotnet/dotnet build`
- Test: `~/.dotnet/dotnet test`
- Individual project testing: `~/.dotnet/dotnet test AnointedAutomation.{LibraryName}.Tests/`
---

**[← Back to Project Dictionary](./PROJECT_STRUCTURE_DICTIONARY.md)**
