// Copyright © Anointed Automation Corporation., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// signIn(returnTo?) sends the browser to YOUR server's start route. The server (using @anointedautomation/sso)
// holds the client secret, builds the PKCE request and redirects to Anointed Automation.

import { Injectable, Injector, inject } from "@angular/core";
import { DOCUMENT } from "@angular/common";
import { ANOINTED_SIGN_IN_CONFIG } from "./config";
import { buildStartUrl } from "./start-url";

@Injectable({ providedIn: "root" })
export class AnointedSignInService {
  private readonly config = inject(ANOINTED_SIGN_IN_CONFIG);
  private readonly injector = inject(Injector);

  /**
   * Navigate the browser to the start route, carrying returnTo when given.
   * @param returnTo Where to land after sign-in. The server validates it (same-site paths by default).
   * @param returnToParam Optional per-call override of the configured query parameter name.
   */
  signIn(returnTo?: string, returnToParam?: string): void {
    const param = returnToParam === undefined ? this.config.returnToParam : returnToParam;
    const url = param === undefined ? buildStartUrl(this.config.startUrl, returnTo) : buildStartUrl(this.config.startUrl, returnTo, param);
    if (this.config.navigate !== undefined) {
      this.config.navigate(url);
      return;
    }
    // A full page load: the start route answers with a redirect to another site.
    const view = this.injector.get(DOCUMENT).defaultView;
    if (view === null) throw new Error("@anointedautomation/sso-angular: signIn needs a browser window (call it from a click handler, not during server rendering)");
    view.location.assign(url);
  }
}
