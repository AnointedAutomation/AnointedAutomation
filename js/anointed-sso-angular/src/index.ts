// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Public entry point of @anointedautomation/sso-angular. Browser safe: no secrets, no OIDC logic.

export { AnointedSignInButtonComponent, DEFAULT_LABEL, LOGO_URL } from "./sign-in-button.component";
export { AnointedSignInService } from "./sign-in.service";
export { ANOINTED_SIGN_IN_CONFIG, provideAnointedSignIn, type AnointedSignInConfig } from "./config";
export { buildStartUrl } from "./start-url";
