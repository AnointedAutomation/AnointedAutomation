# AnointedAutomation.Algorithms

Reusable, dependency-free algorithms for the AnointedAutomation suite. Today it ships a pure C# QR code encoder (ISO/IEC 18004) with Reed-Solomon error correction, for developers who need QR codes without pulling in a native or third-party imaging library.

[![NuGet](https://img.shields.io/nuget/v/AnointedAutomation.Algorithms.svg)](https://www.nuget.org/packages/AnointedAutomation.Algorithms) [![Downloads](https://img.shields.io/nuget/dt/AnointedAutomation.Algorithms.svg)](https://www.nuget.org/packages/AnointedAutomation.Algorithms)

## Installation

```bash
dotnet add package AnointedAutomation.Algorithms
```

- Target framework: `net10.0`
- Dependencies: none

## Quick start

```csharp
using System;
using AnointedAutomation.Algorithms.QrCode;

QrMatrix matrix = QrEncoder.Encode("https://store.anointed.company", QrErrorCorrectionLevel.M);

for (int y = 0; y < matrix.Size; y++)
{
    for (int x = 0; x < matrix.Size; x++)
    {
        Console.Write(matrix.IsDark(x, y) ? "##" : "  ");
    }
    Console.WriteLine();
}
```

To turn the matrix into an SVG or PNG, add [AnointedAutomation.Imaging](https://www.nuget.org/packages/AnointedAutomation.Imaging).

## API overview

Namespace: `AnointedAutomation.Algorithms.QrCode`

| Type | Kind | Purpose |
|---|---|---|
| `QrEncoder` | static class | `QrMatrix Encode(string text, QrErrorCorrectionLevel ecc = QrErrorCorrectionLevel.M)` encodes UTF-8 text into a QR symbol. |
| `QrMatrix` | sealed class | The encoded symbol. `int Size` is the module count per side; `bool IsDark(int x, int y)` reads one module. |
| `QrErrorCorrectionLevel` | enum | `L` (~7% recovery), `M` (~15%), `Q` (~25%), `H` (~30%). |

What `Encode` does for you:

- Byte mode (the input is encoded as UTF-8).
- Automatic version selection: the smallest version from 1 to 10 that fits the data.
- Reed-Solomon error correction with block interleaving.
- Finder, timing and alignment patterns, plus format and version information (BCH coded).
- Automatic mask selection using the four ISO/IEC 18004 penalty rules.

Galois-field arithmetic, the Reed-Solomon encoder, mask scoring and the version tables are internal implementation details, not public API.

## Capacity and errors

Versions 1 to 10 are supported (up to 57 x 57 modules). The maximum payload, in UTF-8 bytes, is:

| Level | Max bytes |
|---|---|
| `L` | 271 |
| `M` | 213 |
| `Q` | 151 |
| `H` | 119 |

`Encode` throws `ArgumentNullException` when `text` is null and `ArgumentException` when the text does not fit any supported version. That comfortably covers URLs and short text.

## Related packages

- [AnointedAutomation.Imaging](https://www.nuget.org/packages/AnointedAutomation.Imaging): renders a `QrMatrix` to SVG or PNG.

## License

MIT. See [LICENSE](https://github.com/AnointedAutomation/AnointedAutomation/blob/master/LICENSE).
Copyright © Anointed Automation, LLC. Stewarded by Alexander Fields.

## Support This Project

This library is free and open source. The best way to support the work is to shop with us:

- **Christian items:** [https://store.anointed.company](https://store.anointed.company)
- **Everything else:** [https://www.mart.club](https://www.mart.club)

Found a bug or have a request? Open an issue: [https://github.com/AnointedAutomation/AnointedAutomation/issues](https://github.com/AnointedAutomation/AnointedAutomation/issues)
