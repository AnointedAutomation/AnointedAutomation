// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Test stub for react-native: plain components that render DOM so react-dom/server can serialize them.
import { createElement } from "react";

export function Pressable(props) {
  const style = typeof props.style === "function" ? props.style({ pressed: false }) : props.style;
  return createElement("button", { type: "button", disabled: props.disabled, "data-role": props.accessibilityRole, "aria-label": props.accessibilityLabel, "data-busy": String(props.accessibilityState.busy), "data-style": JSON.stringify(style), "data-has-press": String(typeof props.onPress === "function") }, props.children);
}
export function Image(props) {
  return createElement("img", { src: props.source.uri, alt: "", "data-accessible": String(props.accessible) });
}
export function Text(props) {
  return createElement("span", null, props.children);
}
