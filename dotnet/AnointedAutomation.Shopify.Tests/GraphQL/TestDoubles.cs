// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AnointedAutomation.Shopify.GraphQL;
using Microsoft.Extensions.Logging;

namespace AnointedAutomation.Shopify.Tests.GraphQL
{
    internal sealed class FakeHandler : HttpMessageHandler
    {
        private readonly Queue<Func<HttpRequestMessage, HttpResponseMessage>> _answers = new Queue<Func<HttpRequestMessage, HttpResponseMessage>>();
        public List<(HttpRequestMessage Request, string Body)> Requests { get; } = new List<(HttpRequestMessage, string)>();

        public FakeHandler Json(HttpStatusCode status, string body)
        {
            _answers.Enqueue(_ => new HttpResponseMessage(status) { Content = new StringContent(body, Encoding.UTF8, "application/json") });
            return this;
        }

        public FakeHandler Answer(Func<HttpRequestMessage, HttpResponseMessage> answer)
        {
            _answers.Enqueue(answer);
            return this;
        }

        public FakeHandler Throw(Exception ex)
        {
            _answers.Enqueue(_ => throw ex);
            return this;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            string body = request.Content == null ? null : await request.Content.ReadAsStringAsync(cancellationToken);
            Requests.Add((request, body));
            if (_answers.Count == 0)
            {
                throw new InvalidOperationException("No more fake answers.");
            }
            return _answers.Dequeue()(request);
        }
    }

    internal sealed class FakeStore : IShopifyStoreContext
    {
        public string ShopDomain { get; set; } = "example.myshopify.com";
        public string ApiVersion { get; set; } = "2026-04";
        public string Token { get; set; } = "shpat_secret_token";
        public Task<string> GetAccessTokenAsync(CancellationToken cancellationToken) => Task.FromResult(Token);
    }

    internal sealed class ListLogger : ILogger
    {
        public List<(LogLevel Level, string Message, Exception Exception)> Entries { get; } = new List<(LogLevel, string, Exception)>();
        public IDisposable BeginScope<TState>(TState state) => null;
        public bool IsEnabled(LogLevel logLevel) => true;
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter) =>
            Entries.Add((logLevel, formatter(state, exception), exception));
    }

    internal sealed class FixedClock : TimeProvider
    {
        public DateTimeOffset Now { get; set; } = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);
        public override DateTimeOffset GetUtcNow() => Now;
    }

    internal sealed class RecordingScheduler : IShopifyRequestScheduler
    {
        public List<string> Operations { get; } = new List<string>();
        public Task<T> ScheduleAsync<T>(string operation, Func<CancellationToken, Task<T>> send, CancellationToken cancellationToken)
        {
            Operations.Add(operation);
            return send(cancellationToken);
        }
    }

    internal sealed class Harness
    {
        public FakeHandler Handler { get; } = new FakeHandler();
        public FakeStore Store { get; } = new FakeStore();
        public ListLogger Logger { get; } = new ListLogger();
        public FixedClock Clock { get; } = new FixedClock();
        public List<TimeSpan> Delays { get; } = new List<TimeSpan>();
        public RecordingScheduler Scheduler { get; } = new RecordingScheduler();
        public ShopifyCostBucket Bucket { get; } = new ShopifyCostBucket();
        public ShopifyRetryPolicy Policy { get; set; } = new ShopifyRetryPolicy(3, 2, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(30));

        public ShopifyGraphQlClient Client() => new ShopifyGraphQlClient(new HttpClient(Handler), Store, Logger, new ShopifyGraphQlClientOptions
        {
            RetryPolicy = Policy,
            CostBucket = Bucket,
            Scheduler = Scheduler,
            Delay = (d, t) => { t.ThrowIfCancellationRequested(); Delays.Add(d); return Task.CompletedTask; },
            Clock = Clock,
            MaxPreThrottleWait = TimeSpan.FromSeconds(30),
        });
    }

    internal static class Bodies
    {
        public const string Ok = "{\"data\":{\"shop\":{\"name\":\"x\"}},\"extensions\":{\"cost\":{\"requestedQueryCost\":10,\"throttleStatus\":{\"maximumAvailable\":1000,\"currentlyAvailable\":990,\"restoreRate\":50}}}}";
        public const string Throttled = "{\"errors\":[{\"message\":\"Throttled\",\"extensions\":{\"code\":\"THROTTLED\"}}],\"extensions\":{\"cost\":{\"requestedQueryCost\":100,\"throttleStatus\":{\"maximumAvailable\":1000,\"currentlyAvailable\":0,\"restoreRate\":50}}}}";
        public const string AccessDenied = "{\"errors\":[{\"message\":\"Access denied\",\"extensions\":{\"code\":\"ACCESS_DENIED\"}}]}";
        public const string Query = "query ShopName { shop { name } }";
        public const string Mutation = "mutation TagsAdd($id: ID!) { tagsAdd(id: $id, tags: [\"a\"]) { userErrors { message } } }";
        public const string KeyedMutation = "mutation RefundCreate($input: RefundInput!, $idempotencyKey: String!) { refundCreate(input: $input) @idempotent(key: $idempotencyKey) { refund { id } userErrors { message } } }";
    }
}
