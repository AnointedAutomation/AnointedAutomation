// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Configuration token and provider function.

import { InjectionToken, makeEnvironmentProviders, type EnvironmentProviders } from "@angular/core";
import { requireStartUrl } from "./start-url";

export interface AnointedSignInConfig {
  /** Path or URL of YOUR server's start route, for example "/api/auth/anointed/start". Required. */
  startUrl: string;
  /** Query parameter the server reads the return target from. Must match the server's returnToParam. Default "returnTo". */
  returnToParam?: string;
  /** Override how the browser navigates (tests, custom routers). Default window.location.assign. */
  navigate?: (url: string) => void;
}

export const ANOINTED_SIGN_IN_CONFIG = new InjectionToken<AnointedSignInConfig>("ANOINTED_SIGN_IN_CONFIG");

/**
 * Register the sign-in configuration. Add it to appConfig.providers (or bootstrapApplication providers).
 * @param config The configuration. startUrl is required, there is no default.
 * @returns Environment providers.
 */
export function provideAnointedSignIn(config: AnointedSignInConfig): EnvironmentProviders {
  if (typeof config !== "object" || config === null) {
    throw new Error("@anointedautomation/sso-angular: provideAnointedSignIn requires a config object with startUrl");
  }
  requireStartUrl(config.startUrl);
  if (config.navigate !== undefined && typeof config.navigate !== "function") {
    throw new Error("@anointedautomation/sso-angular: navigate must be a function");
  }
  return makeEnvironmentProviders([{ provide: ANOINTED_SIGN_IN_CONFIG, useValue: config }]);
}
