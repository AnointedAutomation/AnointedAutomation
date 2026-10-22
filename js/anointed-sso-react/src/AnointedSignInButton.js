// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
"use client";
// The "Sign in with Anointed Automation" button. Plain CSS (inline defaults, override with `className`),
// a real <button> so keyboard and screen readers work, no secrets and no network calls of its own.

import { createElement } from "react";
import { useAnointedSignIn } from "./useAnointedSignIn.js";

export const DEFAULT_LABEL = "Sign in with Anointed Automation";
export const LOGO_URL = "https://www.anointedautomation.net/favicon.svg";

const buttonStyle = {
  display: "inline-flex",
  alignItems: "center",
  justifyContent: "center",
  gap: "10px",
  padding: "10px 18px",
  border: "1px solid #1f2a44",
  borderRadius: "8px",
  background: "#1f2a44",
  color: "#ffffff",
  font: "600 15px/1.2 system-ui, -apple-system, 'Segoe UI', Roboto, sans-serif",
  cursor: "pointer",
};

const logoStyle = { width: "20px", height: "20px", display: "block" };

/**
 * @summary    A brand-styled sign-in button that sends the browser to your server's start route.
 * @param {object} props
 * @param {string} props.startUrl          Required. Path or URL of your server's start route.
 * @param {string} [props.returnTo]        Where to land after sign-in. Omit for none.
 * @param {string} [props.returnToParam]   Query parameter your server reads. Default `returnTo`.
 * @param {string} [props.label]           Button text. Default "Sign in with Anointed Automation".
 * @param {string} [props.className]       When set, the built-in inline look is dropped so your CSS owns the button.
 * @param {boolean} [props.disabled]
 * @param {(url: string) => void} [props.navigate]  Override navigation (tests, custom routers).
 * @returns {import("react").ReactElement}
 */
export function AnointedSignInButton(props) {
  const { startUrl, returnTo, returnToParam, label, className, disabled, navigate } = props;
  const hookOptions = { startUrl };
  if (returnToParam !== undefined) hookOptions.returnToParam = returnToParam;
  if (navigate !== undefined) hookOptions.navigate = navigate;
  const { signIn } = useAnointedSignIn(hookOptions);
  const text = label === undefined ? DEFAULT_LABEL : label;
  const onClick = () => signIn(returnTo);

  return createElement(
    "button",
    {
      type: "button",
      onClick,
      disabled: disabled === true,
      className,
      style: className === undefined ? buttonStyle : undefined,
      "data-anointed-sign-in": "",
    },
    createElement("img", { src: LOGO_URL, alt: "", "aria-hidden": "true", width: 20, height: 20, style: logoStyle }),
    createElement("span", null, text),
  );
}
