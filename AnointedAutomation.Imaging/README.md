# AnointedAutomation.Imaging

Image generation utilities for the AnointedAutomation suite. Depends on
`AnointedAutomation.Algorithms` for encoding work.

## QR rendering (`AnointedAutomation.Imaging.QrCode`)

Renders a `QrMatrix` (from `AnointedAutomation.Algorithms`) to SVG or PNG. The PNG writer is
built on the BCL's `ZLibStream` plus a small CRC-32, so there is no third-party image
dependency.

```csharp
using AnointedAutomation.Algorithms.QrCode;
using AnointedAutomation.Imaging.QrCode;

QrMatrix matrix = QrEncoder.Encode("https://shop.app/@anointedattire", QrErrorCorrectionLevel.M);
string svg = QrRenderer.ToSvg(matrix, moduleSize: 8, quietZone: 4);
byte[] png = QrRenderer.ToPng(matrix, moduleSize: 8, quietZone: 4);
```

## License

MIT © Anointed Automation, LLC.

## Support This Project

This library is free and open source. The best way to support the work is to shop with us:

- **Christian items:** [https://store.anointed.company](https://store.anointed.company)
- **Everything else:** [https://www.mart.club](https://www.mart.club)
