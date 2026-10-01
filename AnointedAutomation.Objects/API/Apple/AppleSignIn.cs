// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me
//Stewarded by Alexander Fields

namespace AnointedAutomation.Objects.Apple
{
    /// <summary>
    /// What Sign in with Apple actually gives us: the verified identity token claims, plus the one-time
    /// first-consent <c>user</c> object (null on every later sign-in). Stored at <c>User.SSO.Apple</c>.
    /// </summary>
    public class AppleSignIn
    {
        /// <summary>The verified identity token claims.</summary>
        public AppleIdTokenClaims IdToken { get; set; }

        /// <summary>The first-consent user object, null when Apple did not send it.</summary>
        public AppleUser User { get; set; }

        /// <summary>
        /// Whether Apple is forwarding mail from the private relay address, as last reported by an
        /// <c>email-enabled</c> / <c>email-disabled</c> server notification. Null until one arrives.
        /// </summary>
        public bool? RelayEmailEnabled { get; set; }

        /// <summary>The relay address named by the last email notification.</summary>
        public string RelayEmail { get; set; }

        /// <summary>When Apple says the last email notification happened, epoch milliseconds.</summary>
        public long? RelayEmailEventTime { get; set; }
    }
}
