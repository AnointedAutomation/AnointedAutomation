# AnointedAutomation.Concepts.Anthropic

Jesus is King ✝️

An optional adapter for [AnointedAutomation.Concepts](https://github.com/AnointedAutomation/AnointedAutomation/tree/master/dotnet/AnointedAutomation.Concepts):
it implements the core `ILanguageModel` port with Claude through the official
[Anthropic .NET SDK](https://www.nuget.org/packages/Anthropic) (13.8.0). The core package has no AI SDK and never
references this one; install this package only when the language engine should talk to Claude.

## Install

```bash
dotnet add package AnointedAutomation.Concepts.Anthropic
```

## Configure

Every setting is required. There are no defaults and no fallback model: a missing value throws
`AnthropicConfigurationException` at startup. Keep the API key in user secrets, environment variables or a secret
store, never in source control. The key is never logged.

```json
{
  "Anthropic": {
    "ApiKey": "<from your secret store>",
    "Model": "claude-opus-5-5",
    "MaxTokens": 16000,
    "TimeoutSeconds": 120,
    "MaxAttempts": 3,
    "RetryBaseDelayMilliseconds": 1000
  }
}
```

```csharp
using AnointedAutomation.Concepts.Anthropic;

builder.Services.AddAnthropicLanguageModel(builder.Configuration.GetSection("Anthropic"));
// ILanguageModel now resolves to AnthropicLanguageModel (a typed HttpClient).
```

## Build a HybridReasoner

The deterministic `CanonReasoner` answers first. Only when it returns Unknown does the `LanguageReasoner` ask Claude,
once per owner, query and model; the answer is kept in the owner's private `InterpretationLedger` (which also caches
by prompt and model, so this adapter does not cache).

```csharp
using AnointedAutomation.Concepts.Reasoning;
using AnointedAutomation.Concepts.Reasoning.Hybrid;
using AnointedAutomation.Concepts.Reasoning.Language;

ILanguageModel claude = services.GetRequiredService<ILanguageModel>();
LanguageReasoner language = new LanguageReasoner(claude, new InterpretationParser(parserLogger), TimeProvider.System);
InterpretationLedger ledger = new InterpretationLedger(new InMemoryInterpretationStore(), ownerOnlyPolicy, TimeProvider.System);
HybridReasoner reasoner = new HybridReasoner(new CanonReasoner(TimeProvider.System), language, ledger, new OwnerScope("chat-123"));

Answer answer = await reasoner.Ask(query, cancellationToken);
```

Language answers are always Unverified and private to the owner's chat. **Only the owner can promote an AI
interpretation into a permanent fact**: `InterpretationLedger.Promote` asks the injected `IPromotionPolicy`, and your
policy must allow only the owner's `Reviewer` (there is no default policy).

## How a call works

- One Messages API request per question. The core prompt asks for every field at once, and the reply is constrained
  with structured outputs (`output_config.format`, a JSON schema built from `PromptComposer.ReplySchema`). The core
  `InterpretationParser` still validates every reply and throws `InterpretationRejected` for anything malformed.
- 429, 408, 5xx, overload, network errors and timeouts are retried with exponential backoff (starting at
  `RetryBaseDelayMilliseconds`, capped at 30 seconds) for at most `MaxAttempts` calls in total, then raised as
  `AnthropicTransientException`. The SDK's own retries are off so there is one bounded policy.
- 401 and 403 raise `AnthropicConfigurationException` immediately and are logged at Critical.
- Other API errors (for example 400) are logged at Error with the request id and rethrown as the SDK exception.
- Cancellation is rethrown quietly and never retried.

## License

MIT. Copyright © Anointed Automation Corporation. Stewarded by Alexander Fields.
