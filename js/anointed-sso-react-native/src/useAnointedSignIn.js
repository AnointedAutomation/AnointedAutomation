// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// The hook: wraps createAnointedSignIn with status, result and error state for a component.

import { useCallback, useMemo, useRef, useState } from "react";
import { createAnointedSignIn } from "./signIn.js";

/**
 * @summary    Sign in with Anointed Automation from a React Native component.
 * @param {object} options   Same as createAnointedSignIn: start, callback, opener, ephemeral, randomBytes.
 * @returns {{signIn: () => Promise<Record<string, string> | null>, status: "idle"|"pending"|"success"|"error", result: Record<string, string> | null, error: Error | null, reset: () => void}}
 * @throws {AnointedSignInError} config, when start or callback is unusable
 */
export function useAnointedSignIn(options) {
  const runner = useMemo(() => createAnointedSignIn(options), [options.start, JSON.stringify(options.callback), options.opener, options.ephemeral, options.randomBytes]);
  const [state, setState] = useState({ status: "idle", result: null, error: null });
  const running = useRef(false);

  const signIn = useCallback(async () => {
    if (running.current) return null;
    running.current = true;
    setState({ status: "pending", result: null, error: null });
    try {
      const result = await runner.signIn();
      setState({ status: "success", result, error: null });
      return result;
    } catch (error) {
      setState({ status: "error", result: null, error });
      return null;
    } finally {
      running.current = false;
    }
  }, [runner]);

  const reset = useCallback(() => setState({ status: "idle", result: null, error: null }), []);

  return { signIn, status: state.status, result: state.result, error: state.error, reset };
}
