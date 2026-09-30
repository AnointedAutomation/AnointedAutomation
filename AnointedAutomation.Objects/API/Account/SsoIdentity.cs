// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me
//Stewarded by Alexander Fields

using System.Runtime.Serialization;

namespace AnointedAutomation.Objects.Account
{
    [System.Serializable]
    /// <summary>
    /// A single external (SSO) identity linked to a <see cref="User"/>. One shared shape for every
    /// provider (Google, Microsoft, Apple, Facebook, Shopify) so external-identity metadata lives in
    /// one place instead of scattered fields. A struct, so under the hybrid casing rule every field
    /// serializes camelCase (subject, email, name, picture, linkedAt).
    /// </summary>
    public struct SsoIdentity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SsoIdentity"/> struct.
        /// </summary>
        /// <param name="subject">The provider's stable, immutable user id.</param>
        /// <param name="email">The email the provider reports (may be null).</param>
        /// <param name="name">The display name the provider reports (may be null).</param>
        /// <param name="picture">The avatar/picture URL the provider reports (may be null).</param>
        /// <param name="linkedAt">When this identity was first linked to the user.</param>
        public SsoIdentity(string subject, string email, string name, string picture, System.DateTime? linkedAt)
        {
            this.Subject = subject;
            this.Email = email;
            this.Name = name;
            this.Picture = picture;
            this.LinkedAt = linkedAt;
        }

        /// <summary>
        /// Gets or sets the provider's stable user id (Google sub, Microsoft oid, Apple sub,
        /// Facebook id, or Shopify customer id). This is the durable link key.
        /// </summary>
        [DataMember]
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the email address reported by the provider (may be null).
        /// </summary>
        [DataMember]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the display name reported by the provider (may be null).
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the avatar/picture URL reported by the provider (may be null).
        /// </summary>
        [DataMember]
        public string Picture { get; set; }

        /// <summary>
        /// Gets or sets the timestamp this identity was first linked to the user (may be null).
        /// </summary>
        [DataMember]
        public System.DateTime? LinkedAt { get; set; }
    }
}
