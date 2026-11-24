import { TestBed } from '@angular/core/testing';
import { afterEach, beforeEach, describe, expect, it } from 'vitest';
import { LgCommentingProvider } from '@data-access/repositories/lg-review/lg-commenting-provider';
import { SessionProvider } from '@core/session/session-provider';
import { CommentingSettings } from './commenting-settings';

const LG = { role: 'local-government', firstName: 'Lee', lastName: 'Chan', organization: 'City of Kelowna', email: 'lee@kelowna.ca' } as const;

async function settle(fixture: { detectChanges(): void; whenStable(): Promise<unknown> }) {
  fixture.detectChanges();
  await fixture.whenStable();
  await new Promise((r) => setTimeout(r, 0));
  fixture.detectChanges();
}

async function mount(session: Record<string, unknown> | null = LG) {
  if (session) TestBed.inject(SessionProvider).login(session as never);
  const fixture = TestBed.createComponent(CommentingSettings);
  document.body.appendChild(fixture.nativeElement);
  await settle(fixture);
  return { fixture, el: fixture.nativeElement as HTMLElement };
}
const sw = (el: HTMLElement) => el.querySelector('#opt-out-commenting') as HTMLButtonElement;
const alert = () => document.querySelector('ui-alert-dialog-content') as HTMLElement | null;
const button = (root: ParentNode, text: string) =>
  [...root.querySelectorAll('button')].find((b) => b.textContent?.trim() === text) as HTMLButtonElement;

describe('CommentingSettings', () => {
  beforeEach(() => {
    localStorage.clear();
    sessionStorage.clear();
  });
  afterEach(() => document.querySelectorAll('.cdk-overlay-container').forEach((c) => (c.innerHTML = '')));

  it('renders the commenting-on state with the prototype classes', async () => {
    const { el } = await mount();
    expect(el.textContent).toContain('Commenting on applications');
    expect(el.textContent).toContain('Choose whether City of Kelowna is asked to comment');
    const badge = el.querySelector('[data-slot="badge"]')!;
    expect(badge.textContent!.trim()).toBe('Commenting on');
    for (const c of ['border-green-300', 'bg-green-50', 'text-green-900']) expect(badge.classList.contains(c)).toBe(true);
    expect([...el.querySelector('[data-slot="card"]')!.classList].sort()).toEqual(
      'bg-card text-card-foreground flex flex-col gap-6 rounded-xl border py-6 shadow-sm'.split(' ').sort(),
    );
    expect(el.querySelector('[data-slot="card-content"]')!.className).toBe('space-y-5 px-6');
    expect(el.querySelector('[data-slot="card-title"] svg')!.getAttribute('class')).toContain('h-5 w-5 text-[#013366]');
    expect(sw(el).getAttribute('aria-checked')).toBe('false');
    expect(sw(el).disabled).toBe(false);
    expect(el.querySelector('#owner-only-note')).toBeNull();
    expect(el.textContent).toContain('What happens when commenting is off');
    expect(el.textContent).toContain('usual 30-day comment period');
    expect(el.textContent).not.toContain('Simulate new referral');
  });

  it('turning commenting off is confirmed, keeps the switch until then, and persists', async () => {
    const { fixture, el } = await mount();
    sw(el).click();
    await settle(fixture);
    expect(alert()!.textContent).toContain('Turn off commenting on applications?');
    expect(alert()!.closest('[role="alertdialog"]')).not.toBeNull();
    expect(sw(el).getAttribute('aria-checked')).toBe('false');
    const confirm = button(alert()!, 'Turn off commenting');
    expect(confirm.className).toContain('bg-amber-700 hover:bg-amber-800');
    confirm.click();
    await settle(fixture);

    const pref = TestBed.inject(LgCommentingProvider).preference();
    expect(pref.optedOut).toBe(true);
    expect(pref.updatedBy).toBe('Lee Chan');
    expect(JSON.parse(localStorage.getItem('lcrb.lg.commenting')!).optedOut).toBe(true);
    expect(sw(el).getAttribute('aria-checked')).toBe('true');
    expect(el.querySelector('[data-slot="badge"]')!.textContent!.trim()).toBe('Commenting off');
    expect(el.textContent).toContain('Last changed by Lee Chan on');
    expect(el.textContent).toContain('In effect for applications referred on or after');
    expect(el.textContent).toContain('Simulate new referral');
  });

  it('cancel leaves commenting on; turning back on needs no confirmation', async () => {
    const { fixture, el } = await mount();
    sw(el).click();
    await settle(fixture);
    button(alert()!, 'Cancel').click();
    await settle(fixture);
    const provider = TestBed.inject(LgCommentingProvider);
    expect(provider.preference().optedOut).toBe(false);
    expect(sw(el).getAttribute('aria-checked')).toBe('false');

    provider.setOptedOut(true, 'x', 'City of Kelowna');
    await settle(fixture);
    expect(sw(el).getAttribute('aria-checked')).toBe('true');
    sw(el).click();
    await settle(fixture);
    expect(alert()).toBeNull();
    expect(provider.preference().optedOut).toBe(false);
    expect(sw(el).getAttribute('aria-checked')).toBe('false');
  });

  it('shows the simulate button while off and it adds a referral', async () => {
    const { fixture, el } = await mount();
    TestBed.inject(LgCommentingProvider).setOptedOut(true, 'x', 'City of Kelowna');
    await settle(fixture);
    const simulate = button(el, 'Simulate new referral');
    expect(simulate).toBeTruthy();
    simulate.click();
    await settle(fixture);
    expect(JSON.parse(localStorage.getItem('lcrb.lg.applications')!).some((a: { jobNumber: string }) => a.jobNumber.startsWith('SIM-'))).toBe(true);
  });

  it('is read-only for a delegate, with the owner-only note', async () => {
    const { el } = await mount({ ...LG, isAccountOwner: false });
    expect(sw(el).disabled).toBe(true);
    expect(sw(el).getAttribute('aria-describedby')).toBe('owner-only-note');
    const note = el.querySelector('#owner-only-note')!;
    expect(note.className).toBe('flex items-start gap-3 rounded-lg border border-border bg-muted/50 p-4 text-sm');
    expect(note.textContent).toContain('Only the account owner can change this setting.');
  });

  it('falls back to "your jurisdiction" with no session', async () => {
    const { el } = await mount(null);
    expect(el.textContent).toContain('Choose whether your jurisdiction is asked');
  });
});
