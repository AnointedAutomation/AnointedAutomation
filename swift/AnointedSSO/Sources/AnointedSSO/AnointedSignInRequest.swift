// Copyright © Anointed Automation Corporation., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️

import Foundation

/// Errors raised by the kit. Every case is final; none is retried for you.
public enum AnointedSignInError: Error, Equatable {
    /// A configuration value is unusable (the message says which).
    case config(String)
    /// The person closed the sign-in sheet.
    case cancelled
    /// The callback did not carry the state this request sent, so it is not ours.
    case stateMismatch
    /// Your server sent the person back with an `error` query item.
    case serverError(String)
    /// The system sign-in sheet failed (the message is the system's).
    case presentation(String)
}

/// How the sign-in sheet hands control back to the app.
public enum AnointedCallback: Equatable {
    /// A universal link on your own domain (iOS 17.4+, macOS 14.4+). Needs the domain in Associated Domains
    /// with the `webcredentials` service. Preferred: nothing else on the device can claim it.
    case https(host: String, path: String)
    /// A custom URL scheme (iOS 15+). The kit only returns to https URLs, so `via` is an https route on
    /// your server that forwards every query item to `<scheme>://anointed-sign-in` (see the README).
    case scheme(String, via: URL)

    /// The URL your server's sign-in finishes on (without the state query item).
    public var returnURL: URL? {
        switch self {
        case .https: return url
        case let .scheme(_, via): return via.scheme == "https" && via.host != nil ? via : nil
        }
    }

    /// The URL the sign-in sheet is finally handed (without the state query item).
    public var url: URL? {
        switch self {
        case let .https(host, path):
            var c = URLComponents()
            c.scheme = "https"
            c.host = host
            c.path = path.hasPrefix("/") ? path : "/" + path
            return c.url
        case let .scheme(scheme, _):
            return URL(string: scheme + "://anointed-sign-in")
        }
    }
}

/// One sign-in attempt: the URL to open and the state that must come back. Pure, so it is testable anywhere.
public struct AnointedSignInRequest: Equatable {
    /// Query item carrying the app's state through your server and back.
    public static let stateItem = "aa_state"

    public let startURL: URL
    public let callbackURL: URL
    public let state: String

    /// - Parameters:
    ///   - start: your server's start route, e.g. https://api.yourapp.com/auth/anointed/start (the route the
    ///     @anointedautomation/sso kit mounts). Must be https.
    ///   - callback: where your server sends the person when done.
    ///   - state: random per attempt; leave the default.
    public init(start: URL, callback: AnointedCallback, state: String = AnointedSignInRequest.randomState()) throws {
        guard start.scheme == "https", start.host != nil else {
            throw AnointedSignInError.config("start URL must be an absolute https URL")
        }
        if case let .scheme(s, _) = callback {
            let valid = !s.isEmpty && s.allSatisfy { $0.isLetter || $0.isNumber || "+-.".contains($0) } && s.first!.isLetter
            guard valid, s.lowercased() != "http", s.lowercased() != "https" else {
                throw AnointedSignInError.config("callback scheme '\(s)' is not a valid custom scheme")
            }
        }
        guard let base = callback.url else { throw AnointedSignInError.config("callback is not a valid URL") }
        guard let returnURL = callback.returnURL else { throw AnointedSignInError.config("the scheme callback's via URL must be an absolute https URL") }
        guard !state.isEmpty else { throw AnointedSignInError.config("state must not be empty") }

        var cb = URLComponents(url: returnURL, resolvingAgainstBaseURL: false)!
        cb.queryItems = [URLQueryItem(name: Self.stateItem, value: state)]
        var st = URLComponents(url: start, resolvingAgainstBaseURL: false)!
        st.queryItems = (st.queryItems ?? []).filter { $0.name != "returnTo" } + [URLQueryItem(name: "returnTo", value: cb.url!.absoluteString)]

        self.startURL = st.url!
        self.callbackURL = base
        self.state = state
    }

    /// Your server's route that finishes sign-in, with the state; this is the `returnTo` sent to the start route.
    public var returnTo: URL {
        URLComponents(url: startURL, resolvingAgainstBaseURL: false)!.queryItems!.first { $0.name == "returnTo" }!.value.flatMap(URL.init(string:))!
    }

    /// Checks a callback URL and returns its query items (minus the state). Throws when the state does not
    /// match or your server reported an error.
    public func parseCallback(_ url: URL) throws -> [String: String] {
        let items = URLComponents(url: url, resolvingAgainstBaseURL: false)?.queryItems ?? []
        var values: [String: String] = [:]
        for item in items { values[item.name] = item.value ?? "" }
        guard values[Self.stateItem] == state else { throw AnointedSignInError.stateMismatch }
        if let error = values["error"] { throw AnointedSignInError.serverError(error) }
        values.removeValue(forKey: Self.stateItem)
        return values
    }

    /// 32 random bytes, base64url.
    public static func randomState() -> String {
        var g = SystemRandomNumberGenerator()
        let bytes = (0..<32).map { _ in UInt8.random(in: 0...255, using: &g) }
        return Data(bytes).base64EncodedString()
            .replacingOccurrences(of: "+", with: "-")
            .replacingOccurrences(of: "/", with: "_")
            .replacingOccurrences(of: "=", with: "")
    }
}
