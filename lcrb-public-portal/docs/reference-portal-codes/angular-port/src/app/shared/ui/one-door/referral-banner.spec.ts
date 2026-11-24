import { TestBed } from '@angular/core/testing';
import { provideRouter, Router } from '@angular/router';
import { beforeEach, describe, expect, it } from 'vitest';
import { ReferralBanner } from './referral-banner';

async function render(url: string) {
  TestBed.configureTestingModule({ providers: [provideRouter([{ path: '**', component: ReferralBanner }])] });
  await TestBed.inject(Router).navigateByUrl(url);
  const fixture = TestBed.createComponent(ReferralBanner);
  await fixture.whenStable();
  return fixture.nativeElement as HTMLElement;
}

describe('ReferralBanner', () => {
  beforeEach(() => {
    localStorage.clear();
    sessionStorage.clear();
  });

  it('shows the hand-off banner only with ?from=one-door', async () => {
    const el = await render('/login?from=one-door');
    const banner = el.querySelector('[role="status"]')!;
    expect(banner.className).toBe('mb-8 flex items-start gap-3 rounded-lg border border-[#013366]/30 bg-white px-4 py-3 text-sm shadow-sm');
    expect(banner.textContent).toContain('Transferred from the BC Government assistant');
    expect(banner.textContent).toContain("You've been connected to the Liquor and Cannabis Regulation Branch.");
    expect(banner.querySelector('svg.lucide-arrow-left-right')).not.toBeNull();
  });

  it('renders nothing without the param or with another value', async () => {
    expect((await render('/login')).querySelector('[role="status"]')).toBeNull();
    TestBed.resetTestingModule();
    expect((await render('/login?from=other')).querySelector('[role="status"]')).toBeNull();
  });
});
