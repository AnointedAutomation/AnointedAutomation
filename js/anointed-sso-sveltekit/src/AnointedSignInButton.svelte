<!-- Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️ -->
<script>
  // "Sign in with Anointed Automation". A real link to YOUR server's start route, so it works without JavaScript,
  // is keyboard and screen reader friendly, and carries no secrets and makes no network calls of its own.
  import { buildStartUrl } from "./startUrl.js";

  const LOGO_URL = "https://www.anointedautomation.net/favicon.svg";

  let { startUrl, returnTo, returnToParam, label = "Sign in with Anointed Automation", class: className, disabled = false } = $props();

  const href = $derived(buildStartUrl(startUrl, returnTo, returnToParam));
</script>

<a
  href={disabled ? undefined : href}
  class={["anointed-sign-in", className]}
  role={disabled ? "link" : undefined}
  aria-disabled={disabled ? "true" : undefined}
  data-anointed-sign-in=""
  data-sveltekit-reload
>
  <img src={LOGO_URL} alt="" aria-hidden="true" width="20" height="20" />
  <span>{label}</span>
</a>

<style>
  .anointed-sign-in {
    display: inline-flex;
    align-items: center;
    justify-content: center;
    gap: 10px;
    padding: 10px 18px;
    border: 1px solid #1f2a44;
    border-radius: 8px;
    background: #1f2a44;
    color: #ffffff;
    font: 600 15px/1.2 system-ui, -apple-system, "Segoe UI", Roboto, sans-serif;
    text-decoration: none;
    cursor: pointer;
  }
  .anointed-sign-in:hover {
    background: #2b3a5e;
  }
  .anointed-sign-in:focus-visible {
    outline: 3px solid #f2b705;
    outline-offset: 2px;
  }
  .anointed-sign-in[aria-disabled="true"] {
    opacity: 0.55;
    cursor: not-allowed;
    pointer-events: none;
  }
  .anointed-sign-in img {
    display: block;
    width: 20px;
    height: 20px;
  }
</style>
<!-- Jesus is King ✝️ -->
