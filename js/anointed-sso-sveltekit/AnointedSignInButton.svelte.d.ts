// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Type definitions for the sign-in button component (Svelte 5).

import type { Component } from "svelte";

export interface AnointedSignInButtonProps {
  /** Path or URL of YOUR start route, for example "/auth/anointed/start". Required. */
  startUrl: string;
  /** Where to land after sign-in. Omit for none. */
  returnTo?: string;
  /** Query parameter your server reads. Must match the server's `returnToParam`. Default "returnTo". */
  returnToParam?: string;
  /** Button text. Default "Sign in with Anointed Automation". */
  label?: string;
  class?: string;
  disabled?: boolean;
}

declare const AnointedSignInButton: Component<AnointedSignInButtonProps>;
export default AnointedSignInButton;
