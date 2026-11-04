// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AnointedAutomation.Shopify.GraphQL;
using Xunit;

namespace AnointedAutomation.Shopify.Tests.GraphQL
{
    public class ShopifyGraphQlBasicsTests
    {
        private static JsonElement Parse(string json)
        {
            using JsonDocument d = JsonDocument.Parse(json);
            return d.RootElement.Clone();
        }

        [Fact]
        public void Endpoints()
        {
            Assert.Equal("https://s.myshopify.com/admin/api/2026-04", ShopifyEndpoints.AdminBase(" s.myshopify.com ", "2026-04"));
            Assert.Equal("https://s.myshopify.com/admin/api/2026-04/graphql.json", ShopifyEndpoints.Graphql("s.myshopify.com", "2026-04"));
            Assert.Equal("https://s.myshopify.com/admin/api/2026-04/orders.json?status=any", ShopifyEndpoints.Rest("s.myshopify.com", "2026-04", "/orders.json?status=any"));
            Assert.Equal("https://s.myshopify.com/api/2026-04/graphql.json", ShopifyEndpoints.StorefrontGraphql("s.myshopify.com", "2026-04"));
            Assert.Throws<ArgumentException>(() => ShopifyEndpoints.AdminBase(null, "v"));
            Assert.Throws<ArgumentException>(() => ShopifyEndpoints.AdminBase("d", ""));
            Assert.Throws<ArgumentException>(() => ShopifyEndpoints.Rest("d", "v", " "));
            Assert.Throws<ArgumentException>(() => ShopifyEndpoints.StorefrontGraphql("", "v"));
            Assert.Throws<ArgumentException>(() => ShopifyEndpoints.StorefrontGraphql("d", null));
        }

        [Fact]
        public void Request_Classification()
        {
            ShopifyGraphQlRequest q = new ShopifyGraphQlRequest(Bodies.Query, null);
            Assert.False(q.IsMutation);
            Assert.True(q.SafeToResendAfterUnknownOutcome);
            Assert.Equal("ShopName", q.OperationName);
            Assert.DoesNotContain("variables", q.ToJson());

            ShopifyGraphQlRequest m = new ShopifyGraphQlRequest(Bodies.Mutation, null);
            Assert.True(m.IsMutation);
            Assert.False(m.IsIdempotent);
            Assert.False(m.SafeToResendAfterUnknownOutcome);

            Assert.False(new ShopifyGraphQlRequest(Bodies.KeyedMutation, null).IsIdempotent);
            Assert.False(new ShopifyGraphQlRequest(Bodies.KeyedMutation, new Dictionary<string, object> { [ShopifyIdempotency.KeyVariable] = " " }).IsIdempotent);
            Assert.False(new ShopifyGraphQlRequest(Bodies.KeyedMutation, new Dictionary<string, object> { [ShopifyIdempotency.KeyVariable] = null }).IsIdempotent);
            ShopifyGraphQlRequest k = new ShopifyGraphQlRequest(Bodies.KeyedMutation, new Dictionary<string, object> { [ShopifyIdempotency.KeyVariable] = "k" });
            Assert.True(k.IsIdempotent);
            Assert.True(k.SafeToResendAfterUnknownOutcome);
            Assert.Throws<ArgumentException>(() => new ShopifyGraphQlRequest(" ", null));
        }

        [Theory]
        [InlineData("query Q { shop { name } }", false)]
        [InlineData("{ shop { name } }", false)]
        [InlineData("  # mutation in a comment\n{ shop { name } }", false)]
        [InlineData("query Q($s: String = \"mutation\") { shop(q: \"mutation {\") { name } }", false)]
        [InlineData("query Q { shop { description(text: \"\"\"mutation }\"\"\") } }", false)]
        [InlineData("fragment F on Shop { name }\nquery Q { shop { ...F } }", false)]
        [InlineData("subscription S { x }", false)]
        [InlineData("# leading comment\nmutation M { tagsAdd(id: 1) { userErrors { message } } }", true)]
        [InlineData("fragment F on Shop { name }\nmutation M { a { ...F } }", true)]
        [InlineData("query A { shop { name } }\nmutation B { a { b } }", true)]
        [InlineData("{ shop { name } }\nmutation B { a { b } }", true)]
        [InlineData("mutation{a{b}}", true)]
        [InlineData("extend type X { y: Int }", true)]
        [InlineData("fragment F on Shop { name }", true)]
        [InlineData("query Q { shop { name }", true)]
        [InlineData("query Q { shop } }", true)]
        [InlineData("query Q(\"unterminated", true)]
        [InlineData("query Q { s(a: \"open) }", true)]
        [InlineData("query Q { s(a: \"\"\"open) }", true)]
        [InlineData("query Q", true)]
        [InlineData(" ", true)]
        public void MayBeMutation_FailsClosed(string query, bool expected)
        {
            Assert.Equal(expected, ShopifyGraphQlRequest.MayBeMutation(query));
        }

        [Fact]
        public void Request_LeadingCommentMutation_IsNotSafeToResend()
        {
            ShopifyGraphQlRequest r = new ShopifyGraphQlRequest("# note\nmutation M { a { b } }", null);
            Assert.True(r.IsMutation);
            Assert.False(r.SafeToResendAfterUnknownOutcome);
        }

        [Fact]
        public void OperationName()
        {
            Assert.Equal("(empty query)", ShopifyGraphQlErrors.OperationName(null));
            Assert.Equal("orders", ShopifyGraphQlErrors.OperationName("{ orders(first: 1) { id } }"));
            Assert.Equal("(unnamed)", ShopifyGraphQlErrors.OperationName("query"));
        }

        [Fact]
        public void Describe_Variants()
        {
            Assert.Null(ShopifyGraphQlErrors.Describe(Parse("{\"data\":{}}")));
            Assert.Null(ShopifyGraphQlErrors.Describe(Parse("[]")));
            Assert.Null(ShopifyGraphQlErrors.Describe(Parse("{\"errors\":[]}")));
            Assert.Null(ShopifyGraphQlErrors.Describe(Parse("{\"errors\":\" \"}")));
            Assert.Null(ShopifyGraphQlErrors.Describe(Parse("{\"errors\":5}")));
            Assert.Equal("Invalid", ShopifyGraphQlErrors.Describe(Parse("{\"errors\":\"Invalid\"}")));
            Assert.Equal("errors", ShopifyGraphQlErrors.Describe(Parse("{\"errors\":[1]}")));
            Assert.Equal("error; X: y", ShopifyGraphQlErrors.Describe(Parse("{\"errors\":[{},{\"message\":\"y\",\"extensions\":{\"code\":\"X\"}}]}")));
            string longMsg = new string('m', 400);
            Assert.Equal(ShopifyGraphQlErrors.MaxLength, ShopifyGraphQlErrors.Describe(Parse("{\"errors\":\"" + longMsg + "\"}")).Length);
        }

        [Fact]
        public void Codes_AndThrottled()
        {
            Assert.True(ShopifyGraphQlErrors.IsThrottled(Parse(Bodies.Throttled)));
            Assert.False(ShopifyGraphQlErrors.IsThrottled(Parse(Bodies.AccessDenied)));
            Assert.False(ShopifyGraphQlErrors.IsThrottled(Parse("{\"data\":{\"note\":\"THROTTLED\"}}")));
            Assert.False(ShopifyGraphQlErrors.HasErrorCode(Parse("{\"errors\":\"x\"}"), "X"));
        }

        [Fact]
        public void UserErrors_AndPayload()
        {
            JsonElement root = Parse("{\"data\":{\"m\":{\"userErrors\":[{\"message\":\"a\"},{\"field\":[\"x\"]},5]}}}");
            Assert.Equal(new[] { "a" }, ShopifyGraphQlErrors.ReadUserErrors(root, "m"));
            Assert.Empty(ShopifyGraphQlErrors.ReadUserErrors(Parse("{\"data\":{\"m\":{}}}"), "m"));
            Assert.Empty(ShopifyGraphQlErrors.ReadUserErrors(Parse("{}"), "m"));
            Assert.Throws<ArgumentException>(() => ShopifyGraphQlErrors.TryGetPayload(root, " ", out _));
            Assert.Equal("a", ShopifyGraphQlErrors.FirstMutationError(root, "m"));
            Assert.Null(ShopifyGraphQlErrors.FirstMutationError(Parse("{\"data\":{\"m\":{\"userErrors\":[]}}}"), "m"));
            Assert.Equal("no m result returned", ShopifyGraphQlErrors.FirstMutationError(Parse("{\"data\":{\"m\":null}}"), "m"));
            Assert.Equal("ACCESS_DENIED: Access denied", ShopifyGraphQlErrors.FirstMutationError(Parse(Bodies.AccessDenied), "m"));
        }

        [Fact]
        public void Response_AndCost()
        {
            ShopifyGraphQlResponse r = new ShopifyGraphQlResponse(200, Bodies.Ok);
            Assert.True(r.HasData);
            Assert.Equal(10, r.Cost.RequestedQueryCost);
            Assert.True(r.Cost.HasThrottleStatus);
            Assert.Equal(1000, r.Cost.MaximumAvailable);
            Assert.Null(new ShopifyGraphQlResponse(200, "{\"data\":null}").Cost);
            Assert.False(new ShopifyGraphQlResponse(200, "[1]").HasData);
            ShopifyQueryCost noStatus = ShopifyQueryCost.Read(Parse("{\"extensions\":{\"cost\":{\"requestedQueryCost\":3,\"throttleStatus\":{\"restoreRate\":0}}}}"));
            Assert.Equal(3, noStatus.RequestedQueryCost);
            Assert.False(noStatus.HasThrottleStatus);
            Assert.ThrowsAny<JsonException>(() => new ShopifyGraphQlResponse(200, "nope"));
        }

        [Fact]
        public void Idempotency_Keys()
        {
            Assert.Equal(ShopifyIdempotency.KeyFor("refundCreate", "o1|5.00"), ShopifyIdempotency.KeyFor("refundCreate", "o1|5.00"));
            Assert.NotEqual(ShopifyIdempotency.KeyFor("refundCreate", "o1|5.00"), ShopifyIdempotency.KeyFor("refundCreate", "o1|5.01"));
            Assert.NotEqual(ShopifyIdempotency.KeyFor("a", "x"), ShopifyIdempotency.KeyFor("b", "x"));
            Assert.True(Guid.TryParse(ShopifyIdempotency.KeyFor("a", ""), out _));
            Assert.NotEqual(ShopifyIdempotency.NewKey(), ShopifyIdempotency.NewKey());
            Assert.Throws<ArgumentException>(() => ShopifyIdempotency.KeyFor(" ", "x"));
            Assert.Throws<ArgumentNullException>(() => ShopifyIdempotency.KeyFor("a", null));
            Assert.True(ShopifyIdempotency.UsesDirective(Bodies.KeyedMutation));
            Assert.False(ShopifyIdempotency.UsesDirective(null));
            Assert.Contains(ShopifyIdempotency.KeyVariable, ShopifyIdempotency.Directive);
            Assert.Contains(ShopifyIdempotency.KeyVariable, ShopifyIdempotency.KeyDeclaration);
        }

        [Fact]
        public async Task ImmediateScheduler_RunsNow()
        {
            Assert.Equal(5, await ImmediateShopifyRequestScheduler.Instance.ScheduleAsync("op", _ => Task.FromResult(5), CancellationToken.None));
            await Assert.ThrowsAsync<ArgumentNullException>(() => ImmediateShopifyRequestScheduler.Instance.ScheduleAsync<int>("op", null, CancellationToken.None));
        }

        [Fact]
        public void Exception_CarriesFields()
        {
            ShopifyGraphQlException ex = new ShopifyGraphQlException("Op", 502, ShopifyCallOutcome.Indeterminate, "why", null);
            Assert.Equal("Op", ex.Operation);
            Assert.Equal(502, ex.StatusCode);
            Assert.Equal("why", ex.Reason);
            Assert.Contains("Indeterminate", ex.Message);
        }
    }
}
