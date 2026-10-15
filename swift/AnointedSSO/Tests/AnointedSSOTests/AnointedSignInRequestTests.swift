// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️

import XCTest
@testable import AnointedSSO

final class AnointedSignInRequestTests: XCTestCase {
    private let start = URL(string: "https://api.example.com/auth/anointed/start")!
    private let via = URL(string: "https://api.example.com/auth/anointed/app-done")!

    func testStartUrlCarriesReturnToWithState() throws {
        let r = try AnointedSignInRequest(start: start, callback: .https(host: "app.example.com", path: "/signed-in"), state: "abc")
        let items = URLComponents(url: r.startURL, resolvingAgainstBaseURL: false)!.queryItems!
        XCTAssertEqual(items.first { $0.name == "returnTo" }?.value, "https://app.example.com/signed-in?aa_state=abc")
    }

    func testCustomSchemeCallback() throws {
        let r = try AnointedSignInRequest(start: start, callback: .scheme("myapp", via: via), state: "abc")
        XCTAssertEqual(r.callbackURL.absoluteString, "myapp://anointed-sign-in")
        XCTAssertEqual(r.returnTo.absoluteString, "https://api.example.com/auth/anointed/app-done?aa_state=abc")
    }

    func testRefusesHttpVia() {
        XCTAssertThrowsError(try AnointedSignInRequest(start: start, callback: .scheme("myapp", via: URL(string: "http://api.example.com/x")!)))
    }

    func testReplacesAnExistingReturnTo() throws {
        let s = URL(string: "https://api.example.com/start?returnTo=/evil&x=1")!
        let r = try AnointedSignInRequest(start: s, callback: .scheme("myapp", via: via), state: "abc")
        let items = URLComponents(url: r.startURL, resolvingAgainstBaseURL: false)!.queryItems!
        XCTAssertEqual(items.filter { $0.name == "returnTo" }.count, 1)
        XCTAssertEqual(items.first { $0.name == "x" }?.value, "1")
    }

    func testRefusesHttpStart() {
        XCTAssertThrowsError(try AnointedSignInRequest(start: URL(string: "http://api.example.com/start")!, callback: .scheme("myapp", via: via)))
    }

    func testRefusesBadScheme() {
        XCTAssertThrowsError(try AnointedSignInRequest(start: start, callback: .scheme("https", via: via)))
        XCTAssertThrowsError(try AnointedSignInRequest(start: start, callback: .scheme("1bad", via: via)))
        XCTAssertThrowsError(try AnointedSignInRequest(start: start, callback: .scheme("", via: via)))
    }

    func testParseCallbackReturnsItemsWithoutState() throws {
        let r = try AnointedSignInRequest(start: start, callback: .scheme("myapp", via: via), state: "abc")
        let v = try r.parseCallback(URL(string: "myapp://anointed-sign-in?aa_state=abc&code=xyz")!)
        XCTAssertEqual(v, ["code": "xyz"])
    }

    func testParseCallbackRejectsWrongState() throws {
        let r = try AnointedSignInRequest(start: start, callback: .scheme("myapp", via: via), state: "abc")
        XCTAssertThrowsError(try r.parseCallback(URL(string: "myapp://anointed-sign-in?aa_state=nope&code=xyz")!)) {
            XCTAssertEqual($0 as? AnointedSignInError, .stateMismatch)
        }
    }

    func testParseCallbackSurfacesServerError() throws {
        let r = try AnointedSignInRequest(start: start, callback: .scheme("myapp", via: via), state: "abc")
        XCTAssertThrowsError(try r.parseCallback(URL(string: "myapp://anointed-sign-in?aa_state=abc&error=access_denied")!)) {
            XCTAssertEqual($0 as? AnointedSignInError, .serverError("access_denied"))
        }
    }

    func testRandomStateIsUrlSafeAndUnique() {
        let a = AnointedSignInRequest.randomState(), b = AnointedSignInRequest.randomState()
        XCTAssertNotEqual(a, b)
        XCTAssertEqual(a.count, 43)
        XCTAssertFalse(a.contains("+") || a.contains("/") || a.contains("="))
    }
}
