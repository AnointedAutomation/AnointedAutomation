// Copyright © Anointed Automation Corporation., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️

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
        Admin = 9,
        /// <summary>From a Discord sign-in.</summary>
        Discord = 10,
        /// <summary>From a GitHub sign-in.</summary>
        GitHub = 11,
        /// <summary>From a LinkedIn sign-in.</summary>
        LinkedIn = 12,
        /// <summary>From a Twitch sign-in.</summary>
        Twitch = 13,
        /// <summary>From a Yahoo sign-in.</summary>
        Yahoo = 14,
        /// <summary>From a Login with Amazon sign-in.</summary>
        Amazon = 15,
        /// <summary>From a Shopify Customer Account API sign-in.</summary>
        ShopifyCustomerAccount = 16
    }
}
