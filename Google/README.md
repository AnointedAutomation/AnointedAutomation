# Google Integration Objects

This directory contains data models and objects for Google OAuth2 and user profile integration.

## Overview

The Google integration objects provide structured data models for handling Google authentication tokens and user profile information retrieved from Google APIs.

## Classes

### GoogleObjects

Main container class that encapsulates Google authentication and profile data.

**Namespace**: `AnointedAutomation.Objects.Google`

**Properties**:
- `GoogleTokenInfo` - Contains OAuth2 token information
- `UserProfile` - Contains user profile data from Google

### GoogleTokenInfo

Represents OAuth2 token information returned by Google authentication.

**Namespace**: `Google.Integrations`

**Properties**:
- `accessToken` - OAuth2 access token
- `tokenType` - Type of token (typically "Bearer")
- `expiresIn` - Token expiration time in seconds
- `idToken` - ID token for user identification
- `azp` - Authorized party (client ID)
- `aud` - Audience (who the token is intended for)
- `sub` - Subject identifier (unique user ID)
- `scope` - Authorized scopes
- `exp` - Expiration timestamp
- `email` - User's email address
- `emailVerified` - Email verification status
- `accessType` - Type of access granted

### UserProfile

Represents a Google user's profile information.

**Namespace**: `Google.Integrations`

**Properties**:
- `id` - Unique Google user ID
- `email` - User's email address
- `verifiedEmail` - Whether email is verified
- `name` - Full name
- `givenName` - First name
- `familyName` - Last name
- `link` - Link to Google+ profile (deprecated)
- `picture` - URL to profile picture
- `gender` - User's gender
- `locale` - User's locale/language preference
- `hd` - Hosted domain (for G Suite accounts)

## Usage Example

```csharp
using AnointedAutomation.Objects.Google;
using Google.Integrations;

// Create token info
var tokenInfo = new GoogleTokenInfo
{
    accessToken = "ya29.a0AfH6...",
    email = "user@example.com",
    emailVerified = "true",
    expiresIn = 3599
};

// Create user profile
var userProfile = new UserProfile
{
    id = "1234567890",
    email = "user@example.com",
    name = "John Doe",
    givenName = "John",
    familyName = "Doe",
    verifiedEmail = true
};

// Create Google objects container
var googleData = new GoogleObjects(tokenInfo, userProfile);
```

## Dependencies

- Newtonsoft.Json - For JSON serialization/deserialization

## Notes

- All classes include both parameterless constructors and full property constructors
- Properties are decorated with `JsonProperty` attributes for proper JSON mapping
- These objects are typically populated from Google OAuth2 API responses

## License

Copyright © 2023 Anointed Automation, LLC. All rights reserved.

## Author

Created by Alexander Fields
GitHub: [https://github.com/AnointedAutomation](https://github.com/AnointedAutomation)