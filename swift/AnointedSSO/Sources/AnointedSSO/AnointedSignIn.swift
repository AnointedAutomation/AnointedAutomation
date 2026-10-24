// Copyright © Anointed Automation Corporation., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️

#if canImport(AuthenticationServices)
import AuthenticationServices
import Foundation
#if os(iOS)
import UIKit
#else
import AppKit
#endif

/// "Sign in with Anointed Automation" for iOS and macOS apps.
///
/// The client secret NEVER ships in the app. The sheet opens your server's start route (the
/// @anointedautomation/sso kit), your server finishes the OpenID Connect flow, then redirects to the
/// callback with whatever your app needs (for example a one-time code your app trades for its session).
@MainActor
public final class AnointedSignIn: NSObject, ASWebAuthenticationPresentationContextProviding {
    private let start: URL
    private let callback: AnointedCallback
    private let ephemeral: Bool
    private var session: ASWebAuthenticationSession?

    /// - Parameters:
    ///   - start: your server's https start route.
    ///   - callback: how the sheet returns to the app.
    ///   - ephemeral: true = no shared browser cookies (the person signs in every time).
    public init(start: URL, callback: AnointedCallback, ephemeral: Bool = false) {
        self.start = start
        self.callback = callback
        self.ephemeral = ephemeral
    }

    /// Runs the sign-in sheet and returns the query items your server put on the callback.
    public func signIn() async throws -> [String: String] {
        let request = try AnointedSignInRequest(start: start, callback: callback)
        let url: URL = try await withCheckedThrowingContinuation { continuation in
            let handler: (URL?, Error?) -> Void = { url, error in
                if let error = error as? ASWebAuthenticationSessionError, error.code == .canceledLogin {
                    continuation.resume(throwing: AnointedSignInError.cancelled)
                } else if let error {
                    continuation.resume(throwing: AnointedSignInError.presentation(error.localizedDescription))
                } else if let url {
                    continuation.resume(returning: url)
                } else {
                    continuation.resume(throwing: AnointedSignInError.presentation("no callback URL"))
                }
            }
            let session: ASWebAuthenticationSession
            switch callback {
            case let .https(host, path):
                if #available(iOS 17.4, macOS 14.4, *) {
                    session = ASWebAuthenticationSession(url: request.startURL, callback: .https(host: host, path: path.hasPrefix("/") ? path : "/" + path), completionHandler: handler)
                } else {
                    continuation.resume(throwing: AnointedSignInError.config("an https callback needs iOS 17.4 / macOS 14.4; use .scheme on older systems"))
                    return
                }
            case let .scheme(scheme, _):
                session = ASWebAuthenticationSession(url: request.startURL, callbackURLScheme: scheme, completionHandler: handler)
            }
            session.presentationContextProvider = self
            session.prefersEphemeralWebBrowserSession = ephemeral
            self.session = session
            if !session.start() {
                continuation.resume(throwing: AnointedSignInError.presentation("the sign-in sheet could not start"))
            }
        }
        session = nil
        return try request.parseCallback(url)
    }

    public nonisolated func presentationAnchor(for session: ASWebAuthenticationSession) -> ASPresentationAnchor {
        MainActor.assumeIsolated {
            #if os(iOS)
            let scenes = UIApplication.shared.connectedScenes.compactMap { $0 as? UIWindowScene }
            return scenes.flatMap(\.windows).first { $0.isKeyWindow } ?? ASPresentationAnchor()
            #else
            return NSApplication.shared.keyWindow ?? ASPresentationAnchor()
            #endif
        }
    }
}
#endif
