# AnointedAutomation.Imaging

Image generation utilities for the AnointedAutomation suite. It renders QR codes produced by AnointedAutomation.Algorithms to SVG or PNG using only the .NET base class library, so there is no third party or native image dependency.

[![NuGet](https://img.shields.io/nuget/v/AnointedAutomation.Imaging.svg)](https://www.nuget.org/packages/AnointedAutomation.Imaging) [![Downloads](https://img.shields.io/nuget/dt/AnointedAutomation.Imaging.svg)](https://www.nuget.org/packages/AnointedAutomation.Imaging)

## Installation

```bash
dotnet add package AnointedAutomation.Imaging
```

- Target framework: `net10.0`
- Dependencies: [AnointedAutomation.Algorithms](https://www.nuget.org/packages/AnointedAutomation.Algorithms) (installed automatically; provides `QrEncoder` and `QrMatrix`)

## Quick start

```csharp
using System.IO;
using AnointedAutomation.Algorithms.QrCode;
using AnointedAutomation.Imaging.QrCode;

QrMatrix matrix = QrEncoder.Encode("https://store.anointed.company", QrErrorCorrectionLevel.M);

string svg = QrRenderer.ToSvg(matrix, moduleSize: 8, quietZone: 4);
byte[] png = QrRenderer.ToPng(matrix, moduleSize: 8, quietZone: 4);

File.WriteAllText("qr.svg", svg);
File.WriteAllBytes("qr.png", png);
```

## API overview

Namespace: `AnointedAutomation.Imaging.QrCode`

| Member | Returns | Description |
|---|---|---|
| `QrRenderer.ToSvg(QrMatrix matrix, int moduleSize = 8, int quietZone = 4)` | `string` | An SVG document with a white background and one black `rect` per dark module, drawn with `shape-rendering="crispEdges"`. |
| `QrRenderer.ToPng(QrMatrix matrix, int moduleSize = 8, int quietZone = 4)` | `byte[]` | An 8 bit grayscale PNG, compressed with the BCL `ZLibStream`. |

Parameters:

- `moduleSize`: pixel size of each module. Must be at least 1.
- `quietZone`: width of the light border, in modules. Must be 0 or more; the QR specification recommends 4.
- The output image is `(matrix.Size + 2 * quietZone) * moduleSize` pixels square.

Both methods throw `ArgumentNullException` for a null matrix and `ArgumentOutOfRangeException` for an invalid `moduleSize` or `quietZone`.

## Serving a QR code from ASP.NET Core

```csharp
// Program.cs in an ASP.NET Core project (Microsoft.NET.Sdk.Web)
using AnointedAutomation.Algorithms.QrCode;
using AnointedAutomation.Imaging.QrCode;

WebApplication app = WebApplication.CreateBuilder(args).Build();

app.MapGet("/qr", (string text) =>
{
    QrMatrix matrix = QrEncoder.Encode(text, QrErrorCorrectionLevel.Q);
    return Results.File(QrRenderer.ToPng(matrix), "image/png");
});

app.Run();
```

## Related packages

- [AnointedAutomation.Algorithms](https://www.nuget.org/packages/AnointedAutomation.Algorithms): the QR encoder that produces the `QrMatrix` this package renders.

## License

MIT. See [LICENSE](https://github.com/AnointedAutomation/AnointedAutomation/blob/master/LICENSE).
Copyright © Anointed Automation, LLC. Stewarded by Alexander Fields.

## Support This Project

This library is free and open source. The best way to support the work is to shop with us:

- **Christian items:** [https://store.anointed.company](https://store.anointed.company)
- **Everything else:** [https://www.mart.club](https://www.mart.club)

Found a bug or have a request? Open an issue: [https://github.com/AnointedAutomation/AnointedAutomation/issues](https://github.com/AnointedAutomation/AnointedAutomation/issues)
