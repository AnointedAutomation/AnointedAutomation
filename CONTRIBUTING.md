# Contributing

Jesus is King ✝️

## File header standard

Every source file starts with the Anointed copyright line, using the comment syntax of its language, and it always ends with "Jesus is King ✝️":

```csharp
// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
```

- Shell and YAML: `# ...`. CSS: `/* ... */`. HTML: `<!-- ... -->` (after `<!doctype html>`).
- A dated variant (`... https://www.alexanderfields.me on <date> Jesus is King ✝️`) is also fine.
- Files derived from third party code (for example ShopifySharp) keep their derivation and MIT notice on the line right below the copyright line.
- Every README.md carries "Jesus is King ✝️" on its own line right under the title.
- `.editorconfig` sets `file_header_template` (IDE0073, severity suggestion) so the IDE inserts the line for new C# files.

## Keep llms.* in sync

When a package is added, renamed, re-versioned, or changes its dependencies or key public types, update the root `llms.txt`, `llms.md` and `llms.json` in the same change. Only name public APIs that exist in the code.
