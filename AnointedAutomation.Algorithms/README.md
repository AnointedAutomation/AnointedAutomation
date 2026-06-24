# AnointedAutomation.Algorithms

Reusable, dependency-free algorithms for the AnointedAutomation suite.

## QR code encoding (`AnointedAutomation.Algorithms.QrCode`)

An ISO/IEC 18004 QR encoder (byte mode) with Reed–Solomon error correction, automatic
version selection, and mask optimisation. Produces a `QrMatrix` of modules. Pair it with
`AnointedAutomation.Imaging` to render to SVG / PNG.

```csharp
using AnointedAutomation.Algorithms.QrCode;

QrMatrix matrix = QrEncoder.Encode("https://shop.app/@anointedattire", QrErrorCorrectionLevel.M);
// matrix.Size, matrix.IsDark(x, y)
```

- Byte mode (UTF-8), EC levels L / M / Q / H.
- Versions 1–10 (auto-selected); capacity tables extend cleanly to 40.
- Internals: `GaloisField256`, `ReedSolomonEncoder`, mask scoring, format/version BCH.

## License

MIT © Anointed Automation, LLC.
