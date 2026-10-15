// swift-tools-version:5.9
// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Swift Package Manager reads this file from the repository root, so it lives here; the code is under swift/AnointedSSO.

import PackageDescription

let package = Package(
    name: "AnointedSSO",
    platforms: [.iOS(.v15), .macOS(.v12)],
    products: [
        .library(name: "AnointedSSO", targets: ["AnointedSSO"]),
    ],
    targets: [
        .target(name: "AnointedSSO", path: "swift/AnointedSSO/Sources/AnointedSSO"),
        .testTarget(name: "AnointedSSOTests", dependencies: ["AnointedSSO"], path: "swift/AnointedSSO/Tests/AnointedSSOTests"),
    ]
)
