// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Type definitions for @anointedautomation/sso-react-native. Needs @types/react and react-native types.

import type { ReactElement } from "react";
import type { StyleProp, TextStyle, ViewStyle } from "react-native";

export type HttpsCallback = { kind: "https"; host: string; path: string };
export type SchemeCallback = { kind: "scheme"; scheme: string; via: string };
export type AnointedCallback = HttpsCallback | SchemeCallback;

export type AuthOpenerResult = { type: "success"; url: string } | { type: "cancel" } | { type: "dismiss" };
/** Opens startUrl in a system auth session and resolves with the deep-link callback. */
export type AuthOpener = (startUrl: string, callbackUrl: string, options: { ephemeral?: boolean }) => Promise<AuthOpenerResult>;

export interface AnointedSignInOptions {
  /** Your server's https start route. Required. */
  start: string;
  /** httpsCallback(...) or schemeCallback(...). Required. */
  callback: AnointedCallback;
  /** Default: expo-web-browser's openAuthSessionAsync. */
  opener?: AuthOpener;
  /** true = no shared browser cookies. */
  ephemeral?: boolean;
  /** Secure random source when crypto.getRandomValues is missing, for example expo-crypto's getRandomBytes. */
  randomBytes?: (length: number) => Uint8Array;
}

export type ErrorCode = "config" | "cancelled" | "state_mismatch" | "missing_state" | "server_error" | "presentation";

export declare class AnointedSignInError extends Error {
  code: ErrorCode;
  constructor(code: ErrorCode, message: string);
}
export declare const ERROR_CODES: Readonly<{
  CONFIG: "config";
  CANCELLED: "cancelled";
  STATE_MISMATCH: "state_mismatch";
  MISSING_STATE: "missing_state";
  SERVER_ERROR: "server_error";
  PRESENTATION: "presentation";
}>;
export declare const STATE_ITEM: string;

export interface SignInRequest {
  startUrl: string;
  callbackUrl: string;
  returnTo: string;
  state: string;
  parseCallback(url: string): Record<string, string>;
}

export interface UseAnointedSignIn {
  /** Resolves with the callback items, or null when it failed (see `error`). */
  signIn: () => Promise<Record<string, string> | null>;
  status: "idle" | "pending" | "success" | "error";
  result: Record<string, string> | null;
  error: AnointedSignInError | null;
  reset: () => void;
}

export interface AnointedSignInButtonProps {
  onPress: () => void;
  label?: string;
  disabled?: boolean;
  loading?: boolean;
  style?: StyleProp<ViewStyle>;
  textStyle?: StyleProp<TextStyle>;
}

export declare const DEFAULT_LABEL: string;
export declare const LOGO_URL: string;

export declare function AnointedSignInButton(props: AnointedSignInButtonProps): ReactElement;
export declare function useAnointedSignIn(options: AnointedSignInOptions): UseAnointedSignIn;
export declare function createAnointedSignIn(options: AnointedSignInOptions): { signIn: () => Promise<Record<string, string>> };
export declare function createExpoOpener(webBrowser?: object): AuthOpener;
export declare function createLinkingOpener(linking: object): AuthOpener;
export declare function httpsCallback(host: string, path: string): HttpsCallback;
export declare function schemeCallback(scheme: string, via: string): SchemeCallback;
export declare function createSignInRequest(options: { start: string; callback: AnointedCallback; state?: string; randomBytes?: (length: number) => Uint8Array }): SignInRequest;
export declare function randomState(randomBytes?: (length: number) => Uint8Array): string;
