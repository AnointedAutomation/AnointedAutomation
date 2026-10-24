// Copyright © Anointed Automation Corporation., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
//Stewarded by Alexander Fields

using System;
using Newtonsoft.Json;

namespace AnointedAutomation.Objects.GitHub
{
    /// <summary>
    /// GitHub's "Private User" object as <c>GET /user</c> returns it
    /// (<c>https://docs.github.com/en/rest/users/users#get-the-authenticated-user</c>). Field names follow GitHub's
    /// snake_case wire JSON. A token without <c>read:user</c> gets the "Public User" shape, so every private-only
    /// field is nullable and null when GitHub omits it.
    /// </summary>
    public class GitHubUser
    {
        /// <summary>The user's login (username).</summary>
        [JsonProperty("login")]
        public string Login { get; set; }

        /// <summary>The user's numeric id (int64), stable across renames.</summary>
        [JsonProperty("id")]
        public long? Id { get; set; }

        /// <summary>The user view type (<c>public</c> or <c>private</c>).</summary>
        [JsonProperty("user_view_type")]
        public string UserViewType { get; set; }

        /// <summary>The GraphQL node id.</summary>
        [JsonProperty("node_id")]
        public string NodeId { get; set; }

        /// <summary>The avatar URL.</summary>
        [JsonProperty("avatar_url")]
        public string AvatarUrl { get; set; }

        /// <summary>The Gravatar id, or null.</summary>
        [JsonProperty("gravatar_id")]
        public string GravatarId { get; set; }

        /// <summary>The API URL of the user.</summary>
        [JsonProperty("url")]
        public string Url { get; set; }

        /// <summary>The profile page URL.</summary>
        [JsonProperty("html_url")]
        public string HtmlUrl { get; set; }

        /// <summary>The followers API URL.</summary>
        [JsonProperty("followers_url")]
        public string FollowersUrl { get; set; }

        /// <summary>The following API URL template.</summary>
        [JsonProperty("following_url")]
        public string FollowingUrl { get; set; }

        /// <summary>The gists API URL template.</summary>
        [JsonProperty("gists_url")]
        public string GistsUrl { get; set; }

        /// <summary>The starred API URL template.</summary>
        [JsonProperty("starred_url")]
        public string StarredUrl { get; set; }

        /// <summary>The subscriptions API URL.</summary>
        [JsonProperty("subscriptions_url")]
        public string SubscriptionsUrl { get; set; }

        /// <summary>The organizations API URL.</summary>
        [JsonProperty("organizations_url")]
        public string OrganizationsUrl { get; set; }

        /// <summary>The repositories API URL.</summary>
        [JsonProperty("repos_url")]
        public string ReposUrl { get; set; }

        /// <summary>The events API URL template.</summary>
        [JsonProperty("events_url")]
        public string EventsUrl { get; set; }

        /// <summary>The received events API URL.</summary>
        [JsonProperty("received_events_url")]
        public string ReceivedEventsUrl { get; set; }

        /// <summary>The account type (e.g. <c>User</c>).</summary>
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>Whether the user is a GitHub site administrator.</summary>
        [JsonProperty("site_admin")]
        public bool? SiteAdmin { get; set; }

        /// <summary>The display name, or null.</summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>The company, or null.</summary>
        [JsonProperty("company")]
        public string Company { get; set; }

        /// <summary>The blog URL, or null.</summary>
        [JsonProperty("blog")]
        public string Blog { get; set; }

        /// <summary>The location, or null.</summary>
        [JsonProperty("location")]
        public string Location { get; set; }

        /// <summary>The PUBLIC profile email, or null. Not proof of verification; see <see cref="GitHubLogin.VerifiedPrimaryEmail"/>.</summary>
        [JsonProperty("email")]
        public string Email { get; set; }

        /// <summary>The notification email, or null.</summary>
        [JsonProperty("notification_email")]
        public string NotificationEmail { get; set; }

        /// <summary>Whether the user is hireable, or null.</summary>
        [JsonProperty("hireable")]
        public bool? Hireable { get; set; }

        /// <summary>The bio, or null.</summary>
        [JsonProperty("bio")]
        public string Bio { get; set; }

        /// <summary>The Twitter username, or null.</summary>
        [JsonProperty("twitter_username")]
        public string TwitterUsername { get; set; }

        /// <summary>Number of public repositories.</summary>
        [JsonProperty("public_repos")]
        public int? PublicRepos { get; set; }

        /// <summary>Number of public gists.</summary>
        [JsonProperty("public_gists")]
        public int? PublicGists { get; set; }

        /// <summary>Number of followers.</summary>
        [JsonProperty("followers")]
        public int? Followers { get; set; }

        /// <summary>Number of accounts followed.</summary>
        [JsonProperty("following")]
        public int? Following { get; set; }

        /// <summary>When the account was created (UTC).</summary>
        [JsonProperty("created_at")]
        public DateTime? CreatedAt { get; set; }

        /// <summary>When the account was last updated (UTC).</summary>
        [JsonProperty("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        /// <summary>Number of private gists (private user only).</summary>
        [JsonProperty("private_gists")]
        public int? PrivateGists { get; set; }

        /// <summary>Total private repositories (private user only).</summary>
        [JsonProperty("total_private_repos")]
        public int? TotalPrivateRepos { get; set; }

        /// <summary>Owned private repositories (private user only).</summary>
        [JsonProperty("owned_private_repos")]
        public int? OwnedPrivateRepos { get; set; }

        /// <summary>Disk usage in KB (private user only).</summary>
        [JsonProperty("disk_usage")]
        public int? DiskUsage { get; set; }

        /// <summary>Number of collaborators (private user only).</summary>
        [JsonProperty("collaborators")]
        public int? Collaborators { get; set; }

        /// <summary>Whether two-factor authentication is on (private user only).</summary>
        [JsonProperty("two_factor_authentication")]
        public bool? TwoFactorAuthentication { get; set; }

        /// <summary>The user's plan (private user only).</summary>
        [JsonProperty("plan")]
        public GitHubPlan Plan { get; set; }

        /// <summary>Whether the user is on GitHub Enterprise Cloud with data residency.</summary>
        [JsonProperty("business_plus")]
        public bool? BusinessPlus { get; set; }

        /// <summary>The LDAP distinguished name (GitHub Enterprise Server).</summary>
        [JsonProperty("ldap_dn")]
        public string LdapDn { get; set; }
    }

    /// <summary>The <c>plan</c> object of a GitHub private user.</summary>
    public class GitHubPlan
    {
        /// <summary>Number of collaborators the plan allows.</summary>
        [JsonProperty("collaborators")]
        public int? Collaborators { get; set; }

        /// <summary>The plan name.</summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>The plan's storage space.</summary>
        [JsonProperty("space")]
        public long? Space { get; set; }

        /// <summary>Number of private repositories the plan allows.</summary>
        [JsonProperty("private_repos")]
        public int? PrivateRepos { get; set; }
    }
}
