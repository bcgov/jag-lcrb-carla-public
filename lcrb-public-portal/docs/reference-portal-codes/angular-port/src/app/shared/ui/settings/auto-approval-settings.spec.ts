import { TestBed } from '@angular/core/testing';
import { afterEach, beforeEach, describe, expect, it } from 'vitest';
import { SessionProvider } from '@core/session/session-provider';
import { getAutoApprovalPolicy } from '@data-access/repositories/special-events/sep-auto-approval';
import { AutoApprovalSettings } from './auto-approval-settings';

const POLICE = { role: 'police', firstName: 'Pat', lastName: 'Lee', organization: 'Vancouver Police Department', email: 'pat@vpd.ca' } as const;

async function settle(fixture: { detectChanges(): void; whenStable(): Promise<unknown> }) {
  fixture.detectChanges();
  await fixture.whenStable();
  await new Promise((r) => setTimeout(r, 0));
  fixture.detectChanges();
}

async function mount(session: Record<string, unknown> | null = POLICE) {
  if (session) TestBed.inject(SessionProvider).login(session as never);
  const fixture = TestBed.createComponent(AutoApprovalSettings);
  document.body.appendChild(fixture.nativeElement);
  await settle(fixture);
  return { fixture, el: fixture.nativeElement as HTMLElement };
}
const dialog = () => document.querySelector('ui-dialog-content') as HTMLElement | null;
const sw = (el: HTMLElement) => el.querySelector('#auto-approval') as HTMLButtonElement;
const button = (root: ParentNode, text: string) =>
  [...root.querySelectorAll('button')].find((b) => b.textContent?.trim() === text) as HTMLButtonElement;

describe('AutoApprovalSettings', () => {
  beforeEach(() => {
    localStorage.clear();
    sessionStorage.clear();
  });
  afterEach(() => document.querySelectorAll('.cdk-overlay-container').forEach((c) => (c.innerHTML = '')));

  it('renders the off state with the prototype classes and copy', async () => {
    const { el } = await mount();
    expect(el.textContent).toContain('Special Event Permit auto-approval');
    expect(el.textContent).toContain("At least 14 days' notice");
    expect(el.textContent).toContain('Enable auto-approval for Vancouver Police Department');
    expect(el.textContent).toContain('Only the account owner can change this.');
    expect(el.querySelector('section')!.className).toBe('rounded-lg border border-border bg-card p-6');
        expect(el.querySelector('.rounded-lg.p-2')!.getAttribute('class')).toBe('rounded-lg p-2 bg-muted text-muted-foreground');
    const badge = el.querySelector('[data-slot="badge"]')!;
    expect(badge.textContent!.trim()).toBe('Off');
    for (const c of ['border-zinc-200', 'bg-zinc-100', 'text-zinc-700']) expect(badge.classList.contains(c)).toBe(true);
    expect(sw(el).getAttribute('aria-checked')).toBe('false');
    expect(sw(el).disabled).toBe(false);
    expect(sw(el).getAttribute('aria-describedby')).toBe('auto-approval-rules');
    expect(el.querySelector('#auto-approval-rules')!.className).toBe('mt-6 grid gap-3 sm:grid-cols-2');
    expect(el.textContent).not.toContain('Last changed by');
  });

  it('turning on asks for confirmation, keeps the switch off until confirmed, then persists', async () => {
    const { fixture, el } = await mount();
    sw(el).click();
    await settle(fixture);
    expect(dialog()!.textContent).toContain('Turn auto-approval on?');
    expect(dialog()!.textContent).toContain('Permits for Vancouver Police Department that meet the standard criteria');
    expect(sw(el).getAttribute('aria-checked')).toBe('false');
    expect(getAutoApprovalPolicy().enabled).toBe(false);

    button(dialog()!, 'Turn on').click();
    await settle(fixture);
    const policy = getAutoApprovalPolicy();
    expect(policy.enabled).toBe(true);
    expect(policy.updatedBy).toBe('Cst. Pat Lee');
    expect(sw(el).getAttribute('aria-checked')).toBe('true');
    expect(el.querySelector('[data-slot="badge"]')!.textContent!.trim()).toBe('On');
    expect(el.querySelector('.rounded-lg.p-2')!.getAttribute('class')).toBe('rounded-lg p-2 bg-green-100 text-green-800');
    expect(el.textContent).toContain('Last changed by Cst. Pat Lee on');
    expect(dialog()?.getAttribute('data-state') ?? 'closed').not.toBe('open');
  });

  it('keep current setting closes without changing the policy, and the switch stays put', async () => {
    const { fixture, el } = await mount();
    sw(el).click();
    await settle(fixture);
    button(dialog()!, 'Keep current setting').click();
    await settle(fixture);
    expect(getAutoApprovalPolicy().enabled).toBe(false);
    expect(sw(el).getAttribute('aria-checked')).toBe('false');
    // And it can be turned off again after being on.
    sw(el).click();
    await settle(fixture);
    button(dialog()!, 'Turn on').click();
    await settle(fixture);
    sw(el).click();
    await settle(fixture);
    expect(dialog()!.textContent).toContain('Turn auto-approval off?');
    button(dialog()!, 'Turn off').click();
    await settle(fixture);
    expect(getAutoApprovalPolicy().enabled).toBe(false);
  });

  it('is read-only for a delegate (not the account owner)', async () => {
    const { el } = await mount({ ...POLICE, isAccountOwner: false });
    expect(sw(el).disabled).toBe(true);
    expect(el.textContent).toContain('Read-only — only the account owner can change this.');
    expect(el.querySelector('svg.lucide-lock')).not.toBeNull();
  });

  it('falls back to "your agency" and owner=false with no session', async () => {
    const { el } = await mount(null);
    expect(el.textContent).toContain('Enable auto-approval for your agency');
    expect(sw(el).disabled).toBe(true);
  });
});
