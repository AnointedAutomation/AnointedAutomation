// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// The "Sign in with Anointed Automation" button: a real <button> so keyboard and screen readers work,
// no secrets and no network calls of its own. Style it with the CSS custom properties below or with `class`.

import { ChangeDetectionStrategy, Component, EventEmitter, Input, Output, inject } from "@angular/core";
import { AnointedSignInService } from "./sign-in.service";

export const DEFAULT_LABEL = "Sign in with Anointed Automation";
export const LOGO_URL = "https://www.anointedautomation.net/favicon.svg";

@Component({
  selector: "anointed-sign-in-button",
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `<button type="button" class="anointed-sign-in" data-anointed-sign-in [disabled]="disabled" (click)="onClick()">
    <img [src]="logoUrl" alt="" aria-hidden="true" width="20" height="20" />
    <span>{{ label }}</span>
  </button>`,
  styles: [
    `:host { display: inline-block; }
    .anointed-sign-in {
      display: inline-flex; align-items: center; justify-content: center; gap: 10px; padding: 10px 18px;
      border: 1px solid var(--anointed-sign-in-bg, #1f2a44); border-radius: var(--anointed-sign-in-radius, 8px);
      background: var(--anointed-sign-in-bg, #1f2a44); color: var(--anointed-sign-in-color, #ffffff);
      font: 600 15px/1.2 system-ui, -apple-system, 'Segoe UI', Roboto, sans-serif; cursor: pointer;
    }
    .anointed-sign-in:hover:not(:disabled) { filter: brightness(1.15); }
    .anointed-sign-in:focus-visible { outline: 3px solid var(--anointed-sign-in-focus, #7aa2ff); outline-offset: 2px; }
    .anointed-sign-in:disabled { opacity: 0.6; cursor: not-allowed; }
    .anointed-sign-in img { width: 20px; height: 20px; display: block; }`,
  ],
})
export class AnointedSignInButtonComponent {
  private readonly service = inject(AnointedSignInService);

  /** Where to land after sign-in. Omit for none. */
  @Input() returnTo?: string;
  /** Query parameter your server reads. Default is the one from provideAnointedSignIn (or "returnTo"). */
  @Input() returnToParam?: string;
  /** Button text. */
  @Input() label: string = DEFAULT_LABEL;
  @Input() disabled: boolean = false;
  @Input() logoUrl: string = LOGO_URL;
  /** Emits the click before navigation starts. */
  @Output() readonly signInStarted = new EventEmitter<void>();

  onClick(): void {
    this.signInStarted.emit();
    this.service.signIn(this.returnTo, this.returnToParam);
  }
}
