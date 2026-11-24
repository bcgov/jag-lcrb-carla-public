/** Accessibility prefs: text scale and high contrast, applied to <html> and saved under `lcrb.a11y.prefs`; loaded after the first render. */
import { afterNextRender, effect, inject, Injectable, signal } from '@angular/core';

type TextScale = 'normal' | 'large' | 'x-large';

const STORAGE_KEY = 'lcrb.a11y.prefs';
const SCALE_ORDER: TextScale[] = ['normal', 'large', 'x-large'];
const SCALE_VALUE: Record<TextScale, string> = {
  normal: '100%',
  large: '112.5%',
  'x-large': '125%',
};

@Injectable({ providedIn: 'root' })
export class A11yProvider {
  private readonly scaleState = signal<TextScale>('normal');
  private readonly contrastState = signal(false);
  readonly textScale = this.scaleState.asReadonly();
  readonly highContrast = this.contrastState.asReadonly();
  /** Storage as it was before the first apply-and-persist run (undefined until then). */
  private storedPrefs: string | null | undefined;

  constructor() {
    // Load persisted prefs
    afterNextRender(() => {
      try {
        const raw = this.storedPrefs !== undefined ? this.storedPrefs : localStorage.getItem(STORAGE_KEY);
        if (raw) {
          const parsed = JSON.parse(raw) as { textScale?: TextScale; highContrast?: boolean };
          if (parsed.textScale) this.scaleState.set(parsed.textScale);
          if (parsed.highContrast) this.contrastState.set(parsed.highContrast);
        }
      } catch {
        // ignore
      }
    });
    // Apply + persist
    effect(() => {
      const textScale = this.scaleState();
      const highContrast = this.contrastState();
      // Angular runs this effect first (React ran the load effect first), so read the stored value before the first write overwrites it.
      if (this.storedPrefs === undefined) {
        try {
          this.storedPrefs = localStorage.getItem(STORAGE_KEY);
        } catch {
          this.storedPrefs = null;
        }
      }
      document.documentElement.style.fontSize = SCALE_VALUE[textScale];
      document.documentElement.classList.toggle('high-contrast', highContrast);
      try {
        localStorage.setItem(STORAGE_KEY, JSON.stringify({ textScale, highContrast }));
      } catch {
        // ignore
      }
    });
  }

  readonly increaseText = (): void =>
    this.scaleState.update((cur) => SCALE_ORDER[Math.min(SCALE_ORDER.indexOf(cur) + 1, SCALE_ORDER.length - 1)]);
  readonly decreaseText = (): void => this.scaleState.update((cur) => SCALE_ORDER[Math.max(SCALE_ORDER.indexOf(cur) - 1, 0)]);
  readonly resetText = (): void => this.scaleState.set('normal');
  readonly toggleContrast = (): void => this.contrastState.update((v) => !v);
}

/** The prototype's useA11y(). */
export function injectA11y(): A11yProvider {
  return inject(A11yProvider);
}
