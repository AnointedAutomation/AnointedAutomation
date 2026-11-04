// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
//
// Canonical builders for Shopify Admin and Storefront API URLs, so the URL shape is identical everywhere.

using System;

namespace AnointedAutomation.Shopify.GraphQL
{
    /// <summary>Static builders for Shopify API URLs. Every argument is required (no defaults).</summary>
    public static class ShopifyEndpoints
    {
        /// <summary>Admin API base, no trailing slash: https://{domain}/admin/api/{version}</summary>
        public static string AdminBase(string domain, string version)
        {
            Require(domain, nameof(domain));
            Require(version, nameof(version));
            return "https://" + domain.Trim() + "/admin/api/" + version.Trim();
        }

        /// <summary>Admin GraphQL endpoint: https://{domain}/admin/api/{version}/graphql.json</summary>
        public static string Graphql(string domain, string version) => AdminBase(domain, version) + "/graphql.json";

        /// <summary>Admin REST endpoint for a relative path, e.g. Rest(d, v, "orders.json?status=any").</summary>
        public static string Rest(string domain, string version, string path)
        {
            Require(path, nameof(path));
            return AdminBase(domain, version) + "/" + path.TrimStart('/');
        }

        /// <summary>Storefront GraphQL endpoint: https://{domain}/api/{version}/graphql.json</summary>
        public static string StorefrontGraphql(string domain, string version)
        {
            Require(domain, nameof(domain));
            Require(version, nameof(version));
            return "https://" + domain.Trim() + "/api/" + version.Trim() + "/graphql.json";
        }

        private static void Require(string value, string name)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException(name + " is required.", name);
            }
        }
    }
}
