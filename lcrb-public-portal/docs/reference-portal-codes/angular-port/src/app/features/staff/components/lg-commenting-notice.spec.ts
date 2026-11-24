import { TestBed } from '@angular/core/testing';
import { beforeEach, describe, expect, it } from 'vitest';
import { LgCommentingProvider } from '@data-access/repositories/lg-review/lg-commenting-provider';
import { COMMENT_HOLD_DAYS } from '@domain/lg-review/lg-commenting';
import { LgCommentingNotice } from './lg-commenting-notice';

async function mount() {
  const fixture = TestBed.createComponent(LgCommentingNotice);
  for (let i = 0; i < 3; i++) {
    fixture.detectChanges();
    await fixture.whenStable();
    await new Promise((r) => setTimeout(r, 0));
  }
  fixture.detectChanges();
  return { fixture, el: fixture.nativeElement as HTMLElement };
}

describe('LgCommentingNotice', () => {
  beforeEach(() => {
    localStorage.clear();
    sessionStorage.clear();
  });

  it('shows the all-commenting state with the hold period', async () => {
    const { el } = await mount();
    expect(el.querySelector('h2')?.textContent).toContain('Local government commenting');
    expect(el.textContent).toContain('All commenting');
    expect(el.textContent).toContain(`held for up to ${COMMENT_HOLD_DAYS} days`);
    for (const c of 'rounded-lg border border-border bg-card p-5'.split(' ')) expect(el.querySelector('section')!.classList.contains(c)).toBe(true);
  });

  it('shows the opted-out box with the jurisdiction and effective date once a jurisdiction opts out', async () => {
    TestBed.inject(LgCommentingProvider).setOptedOut(true, 'City of Vancouver', 'City of Vancouver');
    const { el } = await mount();
    expect(el.textContent).toContain('1 opted out');
    expect(el.textContent).toContain("City of Vancouver's jurisdiction has");
    expect(el.textContent).toContain('opted out of commenting');
    expect(el.textContent).toContain(`${COMMENT_HOLD_DAYS}-day comment period`);
    expect(el.textContent).toContain('In effect for applications referred on or after');
    expect(el.querySelector('section strong')?.textContent).toBe('not held');
  });
});
