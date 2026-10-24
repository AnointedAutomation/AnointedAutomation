// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// The "Sign in with Anointed Automation" button for React Native. A Pressable with the button role and a
// label, 48 point minimum height. It owns no sign-in logic: it calls `onPress`, or the hook when given `signIn`.

import { createElement } from "react";
import { Image, Pressable, Text } from "react-native";

export const DEFAULT_LABEL = "Sign in with Anointed Automation";
export const LOGO_URL = "https://www.anointedautomation.net/favicon.png";

const buttonStyle = {
  flexDirection: "row",
  alignItems: "center",
  justifyContent: "center",
  minHeight: 48,
  paddingHorizontal: 18,
  borderRadius: 8,
  backgroundColor: "#1f2a44",
};
const pressedStyle = { opacity: 0.85 };
const disabledStyle = { opacity: 0.5 };
const logoStyle = { width: 20, height: 20, marginRight: 10 };
const textStyle = { color: "#ffffff", fontSize: 15, fontWeight: "600" };

/**
 * @summary    A brand-styled, accessible sign-in button.
 * @param {object} props
 * @param {() => void} props.onPress        Required. Usually `signIn` from useAnointedSignIn.
 * @param {string} [props.label]            Button text. Default "Sign in with Anointed Automation".
 * @param {boolean} [props.disabled]
 * @param {boolean} [props.loading]         Marks the button busy (announced to screen readers) and blocks presses.
 * @param {object} [props.style]            Extra style merged after the built-in look.
 * @param {object} [props.textStyle]        Extra style for the label.
 * @returns {import("react").ReactElement}
 */
export function AnointedSignInButton(props) {
  const { onPress, label, disabled, loading } = props;
  if (typeof onPress !== "function") {
    throw new Error("@anointedautomation/sso-react-native: AnointedSignInButton requires onPress");
  }
  const text = label === undefined ? DEFAULT_LABEL : label;
  const blocked = disabled === true || loading === true;
  return createElement(
    Pressable,
    {
      onPress: blocked ? undefined : onPress,
      disabled: blocked,
      accessibilityRole: "button",
      accessibilityLabel: text,
      accessibilityState: { disabled: blocked, busy: loading === true },
      style: ({ pressed }) => [buttonStyle, pressed ? pressedStyle : null, blocked ? disabledStyle : null, props.style],
    },
    createElement(Image, { source: { uri: LOGO_URL }, style: logoStyle, accessible: false, accessibilityElementsHidden: true, importantForAccessibility: "no-hide-descendants" }),
    createElement(Text, { style: [textStyle, props.textStyle] }, text),
  );
}
