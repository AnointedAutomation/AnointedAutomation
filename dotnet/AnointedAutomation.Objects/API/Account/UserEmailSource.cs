// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️

namespace AnointedAutomation.Objects.Account
{
    /// <summary>
    /// How a <see cref="UserEmail"/> came to be on an account. 0 is reserved for Unknown.
    /// </summary>
    public enum UserEmailSource
    {
        /// <summary>Not known (default).</summary>
        Unknown = 0,
        /// <summary>Added by the user by hand.</summary>
        Manual = 1,
        /// <summary>The address the account signed up with.</summary>
        Signup = 2,
        /// <summary>From a Google sign-in.</summary>
        Google = 3,
        /// <summary>From a Microsoft sign-in.</summary>
        Microsoft = 4,
        /// <summary>From a Sign in with Apple.</summary>
        Apple = 5,
        /// <summary>From a Facebook login.</summary>
        Facebook = 6,
        /// <summary>From a linked Shopify customer.</summary>
        Shopify = 7,
        /// <summary>Brought over by an account merge.</summary>
        Merge = 8,
        /// <summary>Added by an administrator.</summary>
        Admin = 9
    }
}
