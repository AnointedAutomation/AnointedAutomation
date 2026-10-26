// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️

#if canImport(SwiftUI)
import SwiftUI

/// The standard "Sign in with Anointed Automation" button. The logo is the live
/// https://www.anointedautomation.net/favicon.svg sibling PNG, so it always matches the website.
public struct AnointedSignInButton: View {
    public static let logoURL = URL(string: "https://www.anointedautomation.net/favicon.png")!

    private let title: String
    private let action: () -> Void

    public init(_ title: String = "Sign in with Anointed Automation", action: @escaping () -> Void) {
        self.title = title
        self.action = action
    }

    public var body: some View {
        Button(action: action) {
            HStack(spacing: 10) {
                AsyncImage(url: Self.logoURL) { image in
                    image.resizable().scaledToFit()
                } placeholder: {
                    Color.clear
                }
                .frame(width: 20, height: 20)
                Text(title).fontWeight(.semibold)
            }
            .frame(maxWidth: .infinity, minHeight: 44)
            .foregroundColor(.white)
            .background(Color.black)
            .clipShape(RoundedRectangle(cornerRadius: 8))
        }
        .buttonStyle(.plain)
        .accessibilityLabel(Text(title))
    }
}
#endif
