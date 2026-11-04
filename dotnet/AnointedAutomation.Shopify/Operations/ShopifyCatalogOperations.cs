// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
//
// Typed builders for the most repeated NON-money Admin GraphQL operations: tags, metafields, product and variant
// updates, productSet, customerUpdate, inventory activate / set quantities, and product / order / customer reads.
// Each builder pins its exact query text (tests assert it byte for byte) and returns a ShopifyOperation<T> that
// ExecuteAsync runs through ShopifyGraphQlClient and deserializes into the Models types.
// Schema target: 2026-10. Version caveats (stores pinned to an older Admin API version):
//   - productUpdate(product: ProductUpdateInput) needs 2024-10 or newer (older versions only take input: ProductInput).
//   - @idempotent is accepted from 2026-01 and REQUIRED for inventoryActivate / inventorySetQuantities from 2026-04.
//   - InventoryQuantityInput.changeFromQuantity (compare-and-set) replaces ignoreCompareQuantity from 2026-04; it is
//     always sent (null = no compare).
// Out of scope on purpose: orderCancel, refundCreate, orderCapture, orderEdit*, draftOrderCreate (money paths).

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using AnointedAutomation.Shopify.GraphQL;
using AnointedAutomation.Shopify.GraphQL.Models;

namespace AnointedAutomation.Shopify.Operations
{
    /// <summary>A request plus the data field its typed result is read from.</summary>
    public sealed class ShopifyOperation<T>
    {
        /// <summary>Create an operation.</summary>
        public ShopifyOperation(ShopifyGraphQlRequest request, string field)
        {
            Request = request ?? throw new ArgumentNullException(nameof(request));
            if (string.IsNullOrWhiteSpace(field))
            {
                throw new ArgumentException("A result field is required.", nameof(field));
            }
            Field = field;
        }

        /// <summary>The request.</summary>
        public ShopifyGraphQlRequest Request { get; }

        /// <summary>data.&lt;Field&gt; holds the result.</summary>
        public string Field { get; }
    }

    /// <summary>One metafieldsSet entry.</summary>
    public sealed record ShopifyMetafieldsSetInput(
        [property: JsonPropertyName("ownerId")] string OwnerId,
        [property: JsonPropertyName("namespace")] string Namespace,
        [property: JsonPropertyName("key")] string Key,
        [property: JsonPropertyName("type"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] string Type,
        [property: JsonPropertyName("value")] string Value);

    /// <summary>ProductUpdateInput (common fields; null fields are not sent).</summary>
    public sealed class ShopifyProductUpdateInput
    {
        /// <summary>Product gid (required).</summary>
        [JsonPropertyName("id")] public string Id { get; init; }
        /// <summary>Title.</summary>
        [JsonPropertyName("title"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] public string Title { get; init; }
        /// <summary>Handle.</summary>
        [JsonPropertyName("handle"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] public string Handle { get; init; }
        /// <summary>Description HTML.</summary>
        [JsonPropertyName("descriptionHtml"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] public string DescriptionHtml { get; init; }
        /// <summary>Vendor.</summary>
        [JsonPropertyName("vendor"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] public string Vendor { get; init; }
        /// <summary>Product type.</summary>
        [JsonPropertyName("productType"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] public string ProductType { get; init; }
        /// <summary>ACTIVE, ARCHIVED, DRAFT.</summary>
        [JsonPropertyName("status"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] public string Status { get; init; }
        /// <summary>Replaces ALL tags (use TagsAdd / TagsRemove to change some).</summary>
        [JsonPropertyName("tags"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] public IReadOnlyList<string> Tags { get; init; }
    }

    /// <summary>ProductVariantsBulkInput (common fields; null fields are not sent).</summary>
    public sealed class ShopifyVariantBulkInput
    {
        /// <summary>Variant gid (required).</summary>
        [JsonPropertyName("id")] public string Id { get; init; }
        /// <summary>Price.</summary>
        [JsonPropertyName("price"), JsonNumberHandling(JsonNumberHandling.WriteAsString), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public decimal? Price { get; init; }
        /// <summary>Compare-at price.</summary>
        [JsonPropertyName("compareAtPrice"), JsonNumberHandling(JsonNumberHandling.WriteAsString), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public decimal? CompareAtPrice { get; init; }
        /// <summary>Barcode.</summary>
        [JsonPropertyName("barcode"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] public string Barcode { get; init; }
        /// <summary>Taxable.</summary>
        [JsonPropertyName("taxable"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] public bool? Taxable { get; init; }
    }

    /// <summary>CustomerInput for customerUpdate (null fields are not sent).</summary>
    public sealed class ShopifyCustomerUpdateInput
    {
        /// <summary>Customer gid (required).</summary>
        [JsonPropertyName("id")] public string Id { get; init; }
        /// <summary>First name.</summary>
        [JsonPropertyName("firstName"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] public string FirstName { get; init; }
        /// <summary>Last name.</summary>
        [JsonPropertyName("lastName"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] public string LastName { get; init; }
        /// <summary>Email.</summary>
        [JsonPropertyName("email"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] public string Email { get; init; }
        /// <summary>Phone.</summary>
        [JsonPropertyName("phone"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] public string Phone { get; init; }
        /// <summary>Note.</summary>
        [JsonPropertyName("note"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] public string Note { get; init; }
        /// <summary>Replaces ALL tags.</summary>
        [JsonPropertyName("tags"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] public IReadOnlyList<string> Tags { get; init; }
    }

    /// <summary>One InventoryQuantityInput. ChangeFromQuantity is always sent (null = no compare-and-set).</summary>
    public sealed record ShopifyInventoryQuantityInput(
        [property: JsonPropertyName("inventoryItemId")] string InventoryItemId,
        [property: JsonPropertyName("locationId")] string LocationId,
        [property: JsonPropertyName("quantity")] int Quantity,
        [property: JsonPropertyName("changeFromQuantity")] int? ChangeFromQuantity);

    /// <summary>InventorySetQuantitiesInput.</summary>
    public sealed record ShopifyInventorySetQuantitiesInput(
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("reason")] string Reason,
        [property: JsonPropertyName("referenceDocumentUri"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] string ReferenceDocumentUri,
        [property: JsonPropertyName("quantities")] IReadOnlyList<ShopifyInventoryQuantityInput> Quantities);

    /// <summary>The builders and their pinned query texts.</summary>
    public static class ShopifyCatalogOperations
    {
        /// <summary>tagsAdd on any taggable gid.</summary>
        public const string TagsAddMutation =
            "mutation($id: ID!, $tags: [String!]!) { tagsAdd(id: $id, tags: $tags) { userErrors { field message } } }";

        /// <summary>tagsRemove on any taggable gid.</summary>
        public const string TagsRemoveMutation =
            "mutation($id: ID!, $tags: [String!]!) { tagsRemove(id: $id, tags: $tags) { userErrors { field message } } }";

        /// <summary>metafieldsSet (up to 25 entries per call).</summary>
        public const string MetafieldsSetMutation =
            "mutation bulkMetafieldsSet($metafields: [MetafieldsSetInput!]!) { metafieldsSet(metafields: $metafields) { metafields { id } userErrors { field message } } }";

        /// <summary>productUpdate (2024-10+ ProductUpdateInput shape).</summary>
        public const string ProductUpdateMutation =
            "mutation($input: ProductUpdateInput!){ productUpdate(product:$input){ product{ id title } userErrors{ message } } }";

        /// <summary>productSet (create or replace a product with its options and variants).</summary>
        public const string ProductSetMutation =
            "mutation($input: ProductSetInput!, $sync: Boolean!) { productSet(synchronous: $sync, input: $input) { product { id } userErrors { field message code } } }";

        /// <summary>productVariantsBulkUpdate.</summary>
        public const string VariantsBulkUpdateMutation =
            "mutation($productId: ID!, $variants: [ProductVariantsBulkInput!]!) { productVariantsBulkUpdate(productId: $productId, variants: $variants) { userErrors { field message } } }";

        /// <summary>customerUpdate.</summary>
        public const string CustomerUpdateMutation =
            "mutation($input: CustomerInput!) { customerUpdate(input: $input) { customer { id } userErrors { field message } } }";

        /// <summary>inventoryActivate (keyed with @idempotent).</summary>
        public const string InventoryActivateMutation =
            "mutation($inventoryItemId: ID!, $locationId: ID!, " + ShopifyIdempotency.KeyDeclaration + ") { " +
            "inventoryActivate(inventoryItemId: $inventoryItemId, locationId: $locationId) " + ShopifyIdempotency.Directive + " " +
            "{ inventoryLevel { id } userErrors { field message } } }";

        /// <summary>inventorySetQuantities (keyed with @idempotent).</summary>
        public const string InventorySetQuantitiesMutation =
            "mutation($input: InventorySetQuantitiesInput!, " + ShopifyIdempotency.KeyDeclaration + ") { " +
            "inventorySetQuantities(input: $input) " + ShopifyIdempotency.Directive + " " +
            "{ userErrors { field message } } }";

        /// <summary>One product with its first 100 variants.</summary>
        public const string ProductByIdQuery =
            "query ProductById($id: ID!) { product(id: $id) { id title handle status vendor productType tags descriptionHtml createdAt updatedAt variants(first: 100) { nodes { id title sku price compareAtPrice inventoryQuantity inventoryItem { id tracked } } pageInfo { hasNextPage endCursor } } } }";

        /// <summary>One order with its first 100 line items.</summary>
        public const string OrderByIdQuery =
            "query OrderById($id: ID!) { order(id: $id) { id name email createdAt cancelledAt displayFinancialStatus displayFulfillmentStatus tags note totalPriceSet { shopMoney { amount currencyCode } presentmentMoney { amount currencyCode } } customer { id } lineItems(first: 100) { nodes { id title quantity sku variant { id } } pageInfo { hasNextPage endCursor } } } }";

        /// <summary>One customer.</summary>
        public const string CustomerByIdQuery =
            "query CustomerById($id: ID!) { customer(id: $id) { id firstName lastName email phone note tags numberOfOrders amountSpent { amount currencyCode } createdAt updatedAt } }";

        /// <summary>A products page ($query is Shopify search syntax, may be null).</summary>
        public const string ProductsPageQuery =
            "query ProductsPage($first: Int!, $after: String, $query: String) { products(first: $first, after: $after, query: $query) { nodes { id title handle status vendor productType tags createdAt updatedAt } pageInfo { hasNextPage endCursor } } }";

        /// <summary>An orders page.</summary>
        public const string OrdersPageQuery =
            "query OrdersPage($first: Int!, $after: String, $query: String) { orders(first: $first, after: $after, query: $query) { nodes { id name email createdAt cancelledAt displayFinancialStatus displayFulfillmentStatus tags totalPriceSet { shopMoney { amount currencyCode } } customer { id } } pageInfo { hasNextPage endCursor } } }";

        /// <summary>A customers page.</summary>
        public const string CustomersPageQuery =
            "query CustomersPage($first: Int!, $after: String, $query: String) { customers(first: $first, after: $after, query: $query) { nodes { id firstName lastName email phone tags numberOfOrders createdAt updatedAt } pageInfo { hasNextPage endCursor } } }";

        /// <summary>Add tags to <paramref name="id"/>.</summary>
        public static ShopifyOperation<ShopifyUserErrorsPayload> TagsAdd(string id, IEnumerable<string> tags) =>
            Tags(TagsAddMutation, "tagsAdd", id, tags);

        /// <summary>Remove tags from <paramref name="id"/>.</summary>
        public static ShopifyOperation<ShopifyUserErrorsPayload> TagsRemove(string id, IEnumerable<string> tags) =>
            Tags(TagsRemoveMutation, "tagsRemove", id, tags);

        /// <summary>Set 1 to 25 metafields.</summary>
        public static ShopifyOperation<ShopifyMetafieldsSetPayload> MetafieldsSet(IReadOnlyList<ShopifyMetafieldsSetInput> metafields)
        {
            if (metafields == null || metafields.Count == 0 || metafields.Count > 25)
            {
                throw new ArgumentException("metafieldsSet takes 1 to 25 entries.", nameof(metafields));
            }
            foreach (ShopifyMetafieldsSetInput m in metafields)
            {
                if (m == null || string.IsNullOrWhiteSpace(m.OwnerId) || string.IsNullOrWhiteSpace(m.Namespace)
                    || string.IsNullOrWhiteSpace(m.Key) || m.Value == null)
                {
                    throw new ArgumentException("Every metafield needs ownerId, namespace, key and value.", nameof(metafields));
                }
            }
            return Op<ShopifyMetafieldsSetPayload>(MetafieldsSetMutation, "metafieldsSet", new Dictionary<string, object> { ["metafields"] = metafields });
        }

        /// <summary>Update a product.</summary>
        public static ShopifyOperation<ShopifyProductPayload> ProductUpdate(ShopifyProductUpdateInput input)
        {
            if (input == null || string.IsNullOrWhiteSpace(input.Id))
            {
                throw new ArgumentException("A product id is required.", nameof(input));
            }
            return Op<ShopifyProductPayload>(ProductUpdateMutation, "productUpdate", new Dictionary<string, object> { ["input"] = input });
        }

        /// <summary>productSet. <paramref name="input"/> is any JSON-serializable ProductSetInput shape (a dictionary or
        /// your own record with camelCase names).</summary>
        public static ShopifyOperation<ShopifyProductPayload> ProductSet(object input, bool synchronous)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }
            return Op<ShopifyProductPayload>(ProductSetMutation, "productSet", new Dictionary<string, object>
            {
                ["input"] = input,
                ["sync"] = synchronous,
            });
        }

        /// <summary>Update variants of one product.</summary>
        public static ShopifyOperation<ShopifyUserErrorsPayload> ProductVariantsBulkUpdate(string productId, IReadOnlyList<ShopifyVariantBulkInput> variants)
        {
            Require(productId, nameof(productId));
            if (variants == null || variants.Count == 0 || variants.Any(v => v == null || string.IsNullOrWhiteSpace(v.Id)))
            {
                throw new ArgumentException("At least one variant, each with an id, is required.", nameof(variants));
            }
            return Op<ShopifyUserErrorsPayload>(VariantsBulkUpdateMutation, "productVariantsBulkUpdate", new Dictionary<string, object>
            {
                ["productId"] = productId,
                ["variants"] = variants,
            });
        }

        /// <summary>Update a customer.</summary>
        public static ShopifyOperation<ShopifyCustomerPayload> CustomerUpdate(ShopifyCustomerUpdateInput input)
        {
            if (input == null || string.IsNullOrWhiteSpace(input.Id))
            {
                throw new ArgumentException("A customer id is required.", nameof(input));
            }
            return Op<ShopifyCustomerPayload>(CustomerUpdateMutation, "customerUpdate", new Dictionary<string, object> { ["input"] = input });
        }

        /// <summary>Stock an inventory item at a location. Use a key from <see cref="ShopifyIdempotency"/> (the same key
        /// for a retry of the same logical operation).</summary>
        public static ShopifyOperation<ShopifyInventoryActivatePayload> InventoryActivate(string inventoryItemId, string locationId, string idempotencyKey)
        {
            Require(inventoryItemId, nameof(inventoryItemId));
            Require(locationId, nameof(locationId));
            Require(idempotencyKey, nameof(idempotencyKey));
            return Op<ShopifyInventoryActivatePayload>(InventoryActivateMutation, "inventoryActivate", new Dictionary<string, object>
            {
                ["inventoryItemId"] = inventoryItemId,
                ["locationId"] = locationId,
                [ShopifyIdempotency.KeyVariable] = idempotencyKey,
            });
        }

        /// <summary>Set absolute quantities. Keyed with @idempotent.</summary>
        public static ShopifyOperation<ShopifyUserErrorsPayload> InventorySetQuantities(ShopifyInventorySetQuantitiesInput input, string idempotencyKey)
        {
            if (input == null || string.IsNullOrWhiteSpace(input.Name) || string.IsNullOrWhiteSpace(input.Reason)
                || input.Quantities == null || input.Quantities.Count == 0)
            {
                throw new ArgumentException("name, reason and at least one quantity are required.", nameof(input));
            }
            Require(idempotencyKey, nameof(idempotencyKey));
            return Op<ShopifyUserErrorsPayload>(InventorySetQuantitiesMutation, "inventorySetQuantities", new Dictionary<string, object>
            {
                ["input"] = input,
                [ShopifyIdempotency.KeyVariable] = idempotencyKey,
            });
        }

        /// <summary>Read a product (null when not found).</summary>
        public static ShopifyOperation<ShopifyProduct> ProductById(string id) => ById<ShopifyProduct>(ProductByIdQuery, "product", id);

        /// <summary>Read an order (null when not found).</summary>
        public static ShopifyOperation<ShopifyOrder> OrderById(string id) => ById<ShopifyOrder>(OrderByIdQuery, "order", id);

        /// <summary>Read a customer (null when not found).</summary>
        public static ShopifyOperation<ShopifyCustomer> CustomerById(string id) => ById<ShopifyCustomer>(CustomerByIdQuery, "customer", id);

        /// <summary>
        /// Run an operation. A mutation goes through <see cref="ShopifyGraphQlClient.MutateAsync"/> (throws on userErrors;
        /// an unkeyed mutation is never resent after an unknown outcome). A query goes through
        /// <see cref="ShopifyGraphQlClient.QueryAsync"/>; a null field (not found) returns default.
        /// </summary>
        public static async Task<T> ExecuteAsync<T>(this ShopifyGraphQlClient client, ShopifyOperation<T> operation, CancellationToken cancellationToken)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }
            if (operation == null)
            {
                throw new ArgumentNullException(nameof(operation));
            }
            if (operation.Request.IsMutation)
            {
                JsonElement payload = await client.MutateAsync(operation.Request, operation.Field, cancellationToken).ConfigureAwait(false);
                return ShopifyGraphQlJson.Read<T>(payload);
            }
            ShopifyGraphQlResponse response = await client.QueryAsync(operation.Request, cancellationToken).ConfigureAwait(false);
            if (!response.TryGetData(out JsonElement data) || !data.TryGetProperty(operation.Field, out JsonElement value))
            {
                throw new InvalidOperationException("The answer has no data." + operation.Field + ".");
            }
            return ShopifyGraphQlJson.Read<T>(value);
        }

        /// <summary>Every product matching <paramref name="search"/> (null = all), page by page.</summary>
        public static IAsyncEnumerable<ShopifyProduct> EnumerateProductsAsync(ShopifyGraphQlClient client, string search, int pageSize, CancellationToken cancellationToken) =>
            EnumerateAsync<ShopifyProduct>(client, ProductsPageQuery, "products", search, pageSize, cancellationToken);

        /// <summary>Every order matching <paramref name="search"/> (null = all), page by page.</summary>
        public static IAsyncEnumerable<ShopifyOrder> EnumerateOrdersAsync(ShopifyGraphQlClient client, string search, int pageSize, CancellationToken cancellationToken) =>
            EnumerateAsync<ShopifyOrder>(client, OrdersPageQuery, "orders", search, pageSize, cancellationToken);

        /// <summary>Every customer matching <paramref name="search"/> (null = all), page by page.</summary>
        public static IAsyncEnumerable<ShopifyCustomer> EnumerateCustomersAsync(ShopifyGraphQlClient client, string search, int pageSize, CancellationToken cancellationToken) =>
            EnumerateAsync<ShopifyCustomer>(client, CustomersPageQuery, "customers", search, pageSize, cancellationToken);

        private static async IAsyncEnumerable<T> EnumerateAsync<T>(ShopifyGraphQlClient client, string query, string field, string search,
            int pageSize, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            Dictionary<string, object> vars = new Dictionary<string, object>(StringComparer.Ordinal) { ["query"] = search };
            await foreach (JsonElement node in ShopifyPager.EnumerateNodesAsync(client, query, vars, data => data.GetProperty(field),
                pageSize, cancellationToken).ConfigureAwait(false))
            {
                yield return ShopifyGraphQlJson.Read<T>(node);
            }
        }

        private static ShopifyOperation<ShopifyUserErrorsPayload> Tags(string mutation, string field, string id, IEnumerable<string> tags)
        {
            Require(id, nameof(id));
            List<string> list = tags == null ? null : tags.ToList();
            if (list == null || list.Count == 0 || list.Any(string.IsNullOrWhiteSpace))
            {
                throw new ArgumentException("At least one non-blank tag is required.", nameof(tags));
            }
            return Op<ShopifyUserErrorsPayload>(mutation, field, new Dictionary<string, object> { ["id"] = id, ["tags"] = list });
        }

        private static ShopifyOperation<T> ById<T>(string query, string field, string id)
        {
            Require(id, nameof(id));
            return Op<T>(query, field, new Dictionary<string, object> { ["id"] = id });
        }

        private static ShopifyOperation<T> Op<T>(string query, string field, Dictionary<string, object> variables) =>
            new ShopifyOperation<T>(new ShopifyGraphQlRequest(query, variables), field);

        private static void Require(string value, string name)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException(name + " is required.", name);
            }
        }
    }
}
