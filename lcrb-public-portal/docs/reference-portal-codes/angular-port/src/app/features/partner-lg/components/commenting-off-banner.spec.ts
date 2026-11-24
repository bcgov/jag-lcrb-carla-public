import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { beforeEach, describe, expect, it } from 'vitest';
import { LgCommentingProvider } from '@data-access/repositories/lg-review/lg-commenting-provider';
import { SessionProvider } from '@core/session/session-provider';
import { CommentingOffBanner } from './commenting-off-banner';

const LG = { role: 'local-government', firstName: 'Lee', lastName: 'Chan', organization: 'City of Kelowna' } as const;

async function settle(fixture: { detectChanges(): void; whenStable(): Promise<unknown> }) {
  fixture.detectChanges();
  await fixture.whenStable();
  await new Promise((r) => setTimeout(r, 0));
  fixture.detectChanges();
}

async function mount(session: Record<string, unknown> = LG) {
  TestBed.inject(SessionProvider).login(session as never);
  const fixture = TestBed.createComponent(CommentingOffBanner);
  fixture.componentRef.setInput('className', 'mb-8');
  await settle(fixture);
  return { fixture, el: fixture.nativeElement as HTMLElement };
}

describe('CommentingOffBanner', () => {
  beforeEach(() => {
    localStorage.clear();
    sessionStorage.clear();
    TestBed.configureTestingModule({ providers: [provideRouter([])] });
  });

  it('renders nothing while commenting is on', async () => {
    const { el } = await mount();
    expect(el.querySelector('div')).toBeNull();
  });

  it('shows the banner with the jurisdiction, the grandfathered count and the account link for the owner', async () => {
    const { fixture, el } = await mount();
    TestBed.inject(LgCommentingProvider).setOptedOut(true, 'Lee Chan', 'City of Kelowna');
    await settle(fixture);
    const root = el.firstElementChild!;
    expect(root.getAttribute('class')).toBe('flex items-start gap-3 rounded-lg border border-amber-200 bg-amber-50 p-4 text-amber-900 mb-8');
    expect(el.textContent).toContain('Commenting is turned off for City of Kelowna');
    expect(el.textContent).toContain('they go straight to the LCRB for processing.');
    expect(el.textContent).toMatch(/\d+ applications? referred before this change (is|are) still open for your comment/);
    const link = el.querySelector('a')!;
    expect(link.getAttribute('href')).toBe('/account');
    expect(link.textContent).toContain('Change this in Account Settings');
  });

  it('tells a delegated user that only the owner can change it', async () => {
    const { fixture, el } = await mount({ ...LG, isAccountOwner: false });
    TestBed.inject(LgCommentingProvider).setOptedOut(true, 'Lee Chan', 'City of Kelowna');
    await settle(fixture);
    expect(el.querySelector('a')).toBeNull();
    expect(el.textContent).toContain('Only the account owner can change this setting.');
  });
});
