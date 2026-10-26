// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Openers: how the start route is shown to the person and how the callback URL comes back.
// An opener is `(startUrl, callbackUrl, options) => Promise<{type: "success", url} | {type: "cancel"} | {type: "dismiss"}>`,
// which is exactly what expo-web-browser's openAuthSessionAsync and react-native-inappbrowser-reborn's openAuth return.

import { AnointedSignInError, ERROR_CODES } from "./request.js";

/**
 * @summary    Opener backed by expo-web-browser (optional peer dependency, loaded on first use).
 * @param {object} [webBrowser]   An already imported expo-web-browser module. Default: dynamic import.
 * @returns {(startUrl: string, callbackUrl: string, options?: {ephemeral?: boolean}) => Promise<{type: string, url?: string}>}
 */
export function createExpoOpener(webBrowser) {
  return async (startUrl, callbackUrl, options = {}) => {
    let module = webBrowser;
    if (module === undefined) {
      try {
        module = await import("expo-web-browser");
      } catch {
        throw new AnointedSignInError(ERROR_CODES.CONFIG, "@anointedautomation/sso-react-native: install expo-web-browser or pass your own opener");
      }
    }
    return module.openAuthSessionAsync(startUrl, callbackUrl, { preferEphemeralSession: options.ephemeral === true });
  };
}

/**
 * @summary    Opener backed by react-native's Linking: opens the start route in the system browser and waits for the callback deep link.
 * @description Use it in bare React Native when you have no in-app auth session. The callback must be a custom scheme
 *              (or a universal link your app handles) that Linking delivers as a `url` event.
 * @param {{openURL: (url: string) => Promise<unknown>, addEventListener: Function}} linking   react-native's Linking.
 * @returns {(startUrl: string, callbackUrl: string) => Promise<{type: string, url?: string}>}
 */
export function createLinkingOpener(linking) {
  if (typeof linking !== "object" || linking === null || typeof linking.openURL !== "function" || typeof linking.addEventListener !== "function") {
    throw new AnointedSignInError(ERROR_CODES.CONFIG, "@anointedautomation/sso-react-native: createLinkingOpener needs react-native's Linking");
  }
  return (startUrl, callbackUrl) =>
    new Promise((resolve, reject) => {
      const subscription = linking.addEventListener("url", (event) => {
        if (typeof event.url === "string" && event.url.startsWith(callbackUrl)) {
          subscription.remove();
          resolve({ type: "success", url: event.url });
        }
      });
      Promise.resolve(linking.openURL(startUrl)).catch((error) => {
        subscription.remove();
        reject(error);
      });
    });
}
