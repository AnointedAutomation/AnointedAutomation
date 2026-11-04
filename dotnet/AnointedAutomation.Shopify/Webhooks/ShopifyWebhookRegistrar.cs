// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
//
// Idempotent webhook subscription sync over the Admin GraphQL API. Lists every subscription, keeps the ones that
// already match a wanted (topic, uri), creates the missing ones, and deletes stale ones ONLY under the caller's own
// managed URI prefix (another app's or a hand-made subscription elsewhere is never touched). Re-running is safe:
// webhookSubscriptionCreate is unkeyed, so an unknown outcome is reported as a failure (never resent) and the next
// run's list shows whether it landed. Targets the 2026-10 schema (WebhookSubscription.uri).

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AnointedAutomation.Shopify.GraphQL;
using Microsoft.Extensions.Logging;

namespace AnointedAutomation.Shopify.Webhooks
{
    /// <summary>An existing subscription.</summary>
    public sealed record ShopifyWebhookSubscription(string Id, string Topic, string Uri);

    /// <summary>A wanted subscription: GraphQL topic enum (e.g. ORDERS_CREATE) and delivery URI.</summary>
    public sealed record ShopifyWebhookTarget(string Topic, string Uri);

    /// <summary>What a sync did for one subscription.</summary>
    public enum ShopifyWebhookChange
    {
        /// <summary>Not set.</summary>
        Unknown = 0,
        /// <summary>Already present, left alone.</summary>
        Kept = 1,
        /// <summary>Created.</summary>
        Created = 2,
        /// <summary>Stale (under the managed prefix, not wanted), deleted.</summary>
        Deleted = 3,
        /// <summary>The create or delete failed; see Error.</summary>
        Failed = 4,
    }

    /// <summary>One sync outcome.</summary>
    public sealed record ShopifyWebhookOutcome(string Topic, string Uri, ShopifyWebhookChange Change, string SubscriptionId, string Error);

    /// <summary>The diff between wanted and existing subscriptions.</summary>
    public sealed record ShopifyWebhookPlan(IReadOnlyList<ShopifyWebhookSubscription> Keep, IReadOnlyList<ShopifyWebhookTarget> Create,
        IReadOnlyList<ShopifyWebhookSubscription> Delete);

    /// <summary>Lists, creates and deletes webhook subscriptions, and syncs them to a wanted set.</summary>
    public sealed class ShopifyWebhookRegistrar
    {
        /// <summary>Lists subscriptions (paged by <see cref="ShopifyPager"/>).</summary>
        public const string ListQuery =
            "query WebhookSubscriptions($first: Int!, $after: String) { webhookSubscriptions(first: $first, after: $after) { nodes { id topic uri } pageInfo { hasNextPage endCursor } } }";

        /// <summary>Creates one subscription.</summary>
        public const string CreateMutation =
            "mutation WebhookSubscriptionCreate($topic: WebhookSubscriptionTopic!, $webhookSubscription: WebhookSubscriptionInput!) { webhookSubscriptionCreate(topic: $topic, webhookSubscription: $webhookSubscription) { webhookSubscription { id topic uri } userErrors { field message } } }";

        /// <summary>Deletes one subscription.</summary>
        public const string DeleteMutation =
            "mutation WebhookSubscriptionDelete($id: ID!) { webhookSubscriptionDelete(id: $id) { deletedWebhookSubscriptionId userErrors { field message } } }";

        private readonly ShopifyGraphQlClient _client;
        private readonly ILogger _logger;

        /// <summary>Create a registrar.</summary>
        public ShopifyWebhookRegistrar(ShopifyGraphQlClient client, ILogger logger)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>Every subscription on the shop.</summary>
        public async Task<IReadOnlyList<ShopifyWebhookSubscription>> ListAsync(CancellationToken cancellationToken)
        {
            List<ShopifyWebhookSubscription> list = new List<ShopifyWebhookSubscription>();
            await foreach (JsonElement node in ShopifyPager.EnumerateNodesAsync(_client, ListQuery, null,
                data => data.GetProperty("webhookSubscriptions"), 100, cancellationToken).ConfigureAwait(false))
            {
                list.Add(new ShopifyWebhookSubscription(Str(node, "id"), Str(node, "topic"), Str(node, "uri")));
            }
            return list;
        }

        /// <summary>Create one subscription (JSON format). Returns its id; throws <see cref="ShopifyGraphQlException"/>
        /// on refusal or an unknown outcome.</summary>
        public async Task<string> CreateAsync(ShopifyWebhookTarget target, CancellationToken cancellationToken)
        {
            Validate(target);
            JsonElement payload = await _client.MutateAsync(new ShopifyGraphQlRequest(CreateMutation, new Dictionary<string, object>
            {
                ["topic"] = target.Topic,
                ["webhookSubscription"] = new Dictionary<string, object> { ["uri"] = target.Uri, ["format"] = "JSON" },
            }), "webhookSubscriptionCreate", cancellationToken).ConfigureAwait(false);
            return payload.TryGetProperty("webhookSubscription", out JsonElement sub) ? Str(sub, "id") : null;
        }

        /// <summary>Delete one subscription by id.</summary>
        public Task DeleteAsync(string id, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("A subscription id is required.", nameof(id));
            }
            return _client.MutateAsync(new ShopifyGraphQlRequest(DeleteMutation, new Dictionary<string, object> { ["id"] = id }),
                "webhookSubscriptionDelete", cancellationToken);
        }

        /// <summary>
        /// The pure diff: keep existing (topic, uri) matches, create wanted ones that are missing, delete existing ones
        /// whose uri starts with <paramref name="managedUriPrefix"/> and are not wanted (including duplicates of a
        /// wanted pair). Matching is ordinal.
        /// </summary>
        public static ShopifyWebhookPlan Diff(IEnumerable<ShopifyWebhookTarget> wanted, IEnumerable<ShopifyWebhookSubscription> existing,
            string managedUriPrefix)
        {
            if (wanted == null)
            {
                throw new ArgumentNullException(nameof(wanted));
            }
            if (existing == null)
            {
                throw new ArgumentNullException(nameof(existing));
            }
            if (string.IsNullOrWhiteSpace(managedUriPrefix))
            {
                throw new ArgumentException("A managed URI prefix is required (stale deletes are limited to it).", nameof(managedUriPrefix));
            }
            HashSet<string> wantedKeys = new HashSet<string>(StringComparer.Ordinal);
            List<ShopifyWebhookTarget> wantedList = new List<ShopifyWebhookTarget>();
            foreach (ShopifyWebhookTarget t in wanted)
            {
                Validate(t);
                if (wantedKeys.Add(Key(t.Topic, t.Uri)))
                {
                    wantedList.Add(t);
                }
            }
            HashSet<string> present = new HashSet<string>(StringComparer.Ordinal);
            List<ShopifyWebhookSubscription> keep = new List<ShopifyWebhookSubscription>();
            List<ShopifyWebhookSubscription> delete = new List<ShopifyWebhookSubscription>();
            foreach (ShopifyWebhookSubscription s in existing)
            {
                string key = Key(s.Topic, s.Uri);
                if (wantedKeys.Contains(key) && present.Add(key))
                {
                    keep.Add(s);
                }
                else if (s.Uri != null && s.Uri.StartsWith(managedUriPrefix, StringComparison.Ordinal))
                {
                    delete.Add(s);
                }
            }
            List<ShopifyWebhookTarget> create = new List<ShopifyWebhookTarget>();
            foreach (ShopifyWebhookTarget t in wantedList)
            {
                if (!present.Contains(Key(t.Topic, t.Uri)))
                {
                    create.Add(t);
                }
            }
            return new ShopifyWebhookPlan(keep, create, delete);
        }

        /// <summary>
        /// List, diff (<see cref="Diff"/>) and apply. A failed create or delete is logged and reported as
        /// <see cref="ShopifyWebhookChange.Failed"/>; the others still run. A failed list throws (nothing is changed
        /// on a read that did not complete).
        /// </summary>
        public async Task<IReadOnlyList<ShopifyWebhookOutcome>> SyncAsync(IEnumerable<ShopifyWebhookTarget> wanted, string managedUriPrefix,
            CancellationToken cancellationToken)
        {
            IReadOnlyList<ShopifyWebhookSubscription> existing = await ListAsync(cancellationToken).ConfigureAwait(false);
            ShopifyWebhookPlan plan = Diff(wanted, existing, managedUriPrefix);
            List<ShopifyWebhookOutcome> outcomes = new List<ShopifyWebhookOutcome>();
            foreach (ShopifyWebhookSubscription s in plan.Keep)
            {
                outcomes.Add(new ShopifyWebhookOutcome(s.Topic, s.Uri, ShopifyWebhookChange.Kept, s.Id, null));
            }
            foreach (ShopifyWebhookTarget t in plan.Create)
            {
                try
                {
                    string id = await CreateAsync(t, cancellationToken).ConfigureAwait(false);
                    outcomes.Add(new ShopifyWebhookOutcome(t.Topic, t.Uri, ShopifyWebhookChange.Created, id, null));
                }
                catch (ShopifyGraphQlException ex)
                {
                    _logger.LogError(ex, "Shopify webhook create {Topic} -> {Uri} failed ({Outcome}): {Reason}", t.Topic, t.Uri, ex.Outcome, ex.Message);
                    outcomes.Add(new ShopifyWebhookOutcome(t.Topic, t.Uri, ShopifyWebhookChange.Failed, null, ex.Message));
                }
            }
            foreach (ShopifyWebhookSubscription s in plan.Delete)
            {
                try
                {
                    await DeleteAsync(s.Id, cancellationToken).ConfigureAwait(false);
                    outcomes.Add(new ShopifyWebhookOutcome(s.Topic, s.Uri, ShopifyWebhookChange.Deleted, s.Id, null));
                }
                catch (ShopifyGraphQlException ex)
                {
                    _logger.LogError(ex, "Shopify webhook delete {SubscriptionId} ({Topic} -> {Uri}) failed ({Outcome}): {Reason}",
                        s.Id, s.Topic, s.Uri, ex.Outcome, ex.Message);
                    outcomes.Add(new ShopifyWebhookOutcome(s.Topic, s.Uri, ShopifyWebhookChange.Failed, s.Id, ex.Message));
                }
            }
            return outcomes;
        }

        private static void Validate(ShopifyWebhookTarget target)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }
            if (string.IsNullOrWhiteSpace(target.Topic))
            {
                throw new ArgumentException("A webhook topic is required.", nameof(target));
            }
            if (string.IsNullOrWhiteSpace(target.Uri))
            {
                throw new ArgumentException("A webhook uri is required.", nameof(target));
            }
        }

        private static string Key(string topic, string uri) => topic + "|" + uri;

        private static string Str(JsonElement obj, string name) =>
            obj.ValueKind == JsonValueKind.Object && obj.TryGetProperty(name, out JsonElement v) && v.ValueKind == JsonValueKind.String
                ? v.GetString()
                : null;
    }
}
