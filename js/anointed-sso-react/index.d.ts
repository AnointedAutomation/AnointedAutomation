// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Type definitions for @anointedautomation/sso-react. Needs @types/react (peer of your React setup).

import type { ReactElement } from "react";

export interface UseAnointedSignInOptions {
  /** Path or URL of YOUR server's start route, for example "/api/auth/anointed/start". Required. */
  startUrl: string;
  /** Query parameter the server reads the return target from. Must match the server's `returnToParam`. Default "returnTo". */
  returnToParam?: string;
  /** Override how the browser navigates (tests, custom routers). Default `window.location.assign`. */
  navigate?: (url: string) => void;
}

export interface UseAnointedSignIn {
  /** Navigate the browser to the start route, carrying `returnTo` when given. */
  signIn: (returnTo?: string) => void;
}

export interface AnointedSignInButtonProps {
  /** Path or URL of YOUR server's start route. Required. */
  startUrl: string;
  /** Where to land after sign-in. The server validates it (same-site paths by default). */
  returnTo?: string;
  /** Query parameter the server reads the return target from. Default "returnTo". */
  returnToParam?: string;
  /** Button text. Default "Sign in with Anointed Automation". */
  label?: string;
  /** When set, the built-in inline look is dropped so your CSS owns the button. */
  className?: string;
  disabled?: boolean;
  /** Override how the browser navigates (tests, custom routers). */
  navigate?: (url: string) => void;
}

export declare const DEFAULT_LABEL: string;
export declare const LOGO_URL: string;

export declare function useAnointedSignIn(options: UseAnointedSignInOptions): UseAnointedSignIn;
export declare function AnointedSignInButton(props: AnointedSignInButtonProps): ReactElement;
export declare function buildStartUrl(startUrl: string, returnTo?: string | null, returnToParam?: string): string;
