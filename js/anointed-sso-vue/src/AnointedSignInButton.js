// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// The "Sign in with Anointed Automation" button. A render-function component (no SFC build step needed).
// A real <button> so keyboard and screen readers work, no secrets and no network calls of its own.

import { defineComponent, h } from "vue";
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
 * A brand-styled sign-in button that sends the browser to your server's start route.
 * Props: `startUrl` (required), `returnTo`, `returnToParam`, `label`, `disabled`, `navigate`.
 * Pass `class` to restyle: when a class is given the built-in inline look is dropped so your CSS owns the button.
 * Emits `click` before navigating.
 */
export const AnointedSignInButton = defineComponent({
  name: "AnointedSignInButton",
  inheritAttrs: false,
  props: {
    startUrl: { type: String, required: true },
    returnTo: { type: String, required: false },
    returnToParam: { type: String, required: false },
    label: { type: String, required: false },
    disabled: { type: Boolean, default: false },
    navigate: { type: Function, required: false },
  },
  emits: ["click"],
  setup(props, { attrs, emit }) {
    const hookOptions = { startUrl: props.startUrl };
    if (props.returnToParam !== undefined) hookOptions.returnToParam = props.returnToParam;
    if (props.navigate !== undefined) hookOptions.navigate = props.navigate;
    const { signIn } = useAnointedSignIn(hookOptions);

    return () => {
      const hasClass = attrs.class !== undefined && attrs.class !== "";
      const text = props.label === undefined ? DEFAULT_LABEL : props.label;
      return h(
        "button",
        {
          ...attrs,
          type: "button",
          disabled: props.disabled,
          style: hasClass ? attrs.style : [buttonStyle, attrs.style],
          "data-anointed-sign-in": "",
          onClick: (event) => {
            emit("click", event);
            signIn(props.returnTo);
          },
        },
        [h("img", { src: LOGO_URL, alt: "", "aria-hidden": "true", width: 20, height: 20, style: logoStyle }), h("span", null, text)],
      );
    };
  },
});
