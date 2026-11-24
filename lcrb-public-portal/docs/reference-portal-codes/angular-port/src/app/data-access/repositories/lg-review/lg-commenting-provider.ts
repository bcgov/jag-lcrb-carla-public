/** Local government commenting opt-out. */
import { afterNextRender, inject, Injectable, signal } from '@angular/core';
import { DEFAULT_LG_COMMENTING, commentingStatusLabel } from '@domain/lg-review/lg-commenting';
import { type LgCommentingPreference } from '@domain/models/lg-review';
import { recordAudit } from '../activity/audit-service';
import { notify } from '../notifications/notification-service';

const STORAGE_KEY = 'lcrb.lg.commenting';

@Injectable({ providedIn: 'root' })
export class LgCommentingProvider {
  private readonly preferenceState = signal<LgCommentingPreference>(DEFAULT_LG_COMMENTING);
  private readonly loadedState = signal(false);
  readonly preference = this.preferenceState.asReadonly();
  /** Hydrated from storage yet - avoids a flash of the wrong state. */
  readonly loaded = this.loadedState.asReadonly();

  constructor() {
    afterNextRender(() => {
      try {
        const raw = localStorage.getItem(STORAGE_KEY);
        if (raw) this.preferenceState.set({ ...DEFAULT_LG_COMMENTING, ...JSON.parse(raw) });
      } catch {
        // ignore malformed storage
      }
      this.loadedState.set(true);
    });
  }

  /** Change the setting. `actor` and `jurisdiction` are recorded for the audit trail. */
  readonly setOptedOut = (optedOut: boolean, actor: string, jurisdiction: string): void => {
    const now = new Date();
    const next: LgCommentingPreference = {
      optedOut,
      updatedAt: now.toISOString(),
      updatedBy: actor,
      // Opting out takes effect today; opting back in clears the date so all future referrals require comment again.
      effectiveFrom: optedOut ? now.toISOString().slice(0, 10) : null,
    };
    this.preferenceState.set(next);
    try {
      localStorage.setItem(STORAGE_KEY, JSON.stringify(next));
    } catch {
      // ignore storage write failures
    }
    // Audit the change - this is a consequential, account-level setting.
    recordAudit({
      category: 'system',
      action: optedOut ? 'lg.commenting.opted-out' : 'lg.commenting.opted-in',
      summary: `${commentingStatusLabel(next)} for ${jurisdiction}`,
      actor,
      actorRole: 'local-government',
      subjectId: jurisdiction,
      subjectType: 'lg-commenting-preference',
      meta: { optedOut: String(optedOut), effectiveFrom: next.effectiveFrom ?? '—' },
    });
    // Let the branch know the hold period no longer applies to new referrals.
    notify({
      recipient: 'lcrb',
      category: 'system',
      title: optedOut ? 'Local government opted out of commenting' : 'Local government resumed commenting',
      body: optedOut
        ? `${jurisdiction} has turned off commenting. New referrals will flow through without a comment hold.`
        : `${jurisdiction} has turned commenting back on. New referrals will be held for comment again.`,
      href: '/lcrb',
      subjectId: jurisdiction,
      channels: ['in-portal'],
    });
  };
}

/** The prototype's useLgCommenting(). */
export function injectLgCommenting(): LgCommentingProvider {
  return inject(LgCommentingProvider);
}
