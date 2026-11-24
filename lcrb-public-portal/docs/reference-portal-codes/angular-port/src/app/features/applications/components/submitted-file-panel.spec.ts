import { Component, signal } from '@angular/core';
import { TestBed } from '@angular/core/testing';
import { afterEach, beforeEach, describe, expect, it } from 'vitest';
import { SessionProvider } from '@core/session/session-provider';
import { cancelApplication, getCancellation } from '@data-access/repositories/applications/cancellation-service';
import { SubmittedFilePanel, SubmittedStatusBadge } from './submitted-file-panel';

@Component({
  imports: [SubmittedStatusBadge, SubmittedFilePanel],
  template: `
    <submitted-status-badge [id]="id()" [status]="status()" />
    <submitted-file-panel [id]="id()" [type]="type" [status]="status()" applicant="Cedar Room Ltd." />
  `,
})
class Host {
  id = signal('LP-125998');
  status = signal('under_review');
  type = 'Liquor Primary Licence';
}

const settle = async (f: { detectChanges(): void; whenStable(): Promise<unknown> }) => {
  for (let i = 0; i < 3; i++) {
    f.detectChanges();
    await f.whenStable();
    await new Promise((r) => setTimeout(r, 0));
  }
  f.detectChanges();
};
async function mount(setup?: (h: Host) => void) {
  TestBed.inject(SessionProvider).login({ role: 'licensee', firstName: 'Dana', lastName: 'Lee', organization: 'Cedar' } as never);
  const fixture = TestBed.createComponent(Host);
  setup?.(fixture.componentInstance);
  document.body.appendChild(fixture.nativeElement);
  await settle(fixture);
  return { fixture, el: fixture.nativeElement as HTMLElement };
}
const badge = (el: HTMLElement) => el.querySelector('submitted-status-badge span')!;

describe('SubmittedStatusBadge', () => {
  beforeEach(() => {
    localStorage.clear();
    sessionStorage.clear();
  });
  afterEach(() => document.querySelectorAll('.cdk-overlay-container').forEach((c) => (c.innerHTML = '')));

  it.each([
    ['under_review', 'Under Review', 'bg-blue-100 text-blue-800 border-blue-200'],
    ['pending_documents', 'Pending Documents', 'bg-yellow-100 text-yellow-800 border-yellow-200'],
    ['approved', 'Approved', 'bg-green-100 text-green-800 border-green-200'],
    ['cancelled', 'Cancelled', 'bg-zinc-100 text-zinc-600 border-zinc-200'],
  ])('renders %s', async (status, label, color) => {
    const { el } = await mount((h) => h.status.set(status));
    expect(badge(el).textContent!.trim()).toBe(label);
    for (const c of color.split(' ')) expect(badge(el).classList.contains(c)).toBe(true);
    expect(badge(el).classList.contains('flex')).toBe(true);
    expect(badge(el).querySelector('svg')!.getAttribute('class')).toContain('h-3 w-3');
  });

  it('falls back to Under Review for an unknown status and shows Cancelled once withdrawn', async () => {
    const unknown = await mount((h) => h.status.set('mystery'));
    expect(badge(unknown.el).textContent!.trim()).toBe('Under Review');
    cancelApplication({ jobNumber: 'LP-125998', applicationType: 't', applicant: 'a', actor: 'x', reason: 'r', priorStatus: 'under_review', referredTo: [] });
    await settle(unknown.fixture);
    expect(badge(unknown.el).textContent!.trim()).toBe('Cancelled');
  });
});

describe('SubmittedFilePanel', () => {
  beforeEach(() => {
    localStorage.clear();
    sessionStorage.clear();
  });
  afterEach(() => document.querySelectorAll('.cdk-overlay-container').forEach((c) => (c.innerHTML = '')));

  it('shows the withdraw card, the cancel trigger and the LG-aware conversation for an in-process file', async () => {
    const { el } = await mount();
    const panel = el.querySelector('submitted-file-panel')!;
    expect(panel.className).toContain('space-y-6');
    expect(panel.textContent).toContain('No longer proceeding?');
    expect(panel.textContent).toContain('You can withdraw this application while it is under review.');
    const card = panel.querySelector('[data-slot="card"]')!;
    expect(card.querySelector('[data-slot="card-content"]')!.className).toBe('flex flex-col gap-3 p-5 sm:flex-row sm:items-center sm:justify-between');
    expect([...panel.querySelectorAll('button')].some((b) => b.textContent!.trim() === 'Cancel application')).toBe(true);
    expect(panel.querySelector('case-thread h2')!.textContent).toBe('Conversation with LCRB and City of Victoria');
    expect(panel.querySelector('case-thread h2 + span')!.textContent!.trim()).toBe('Live');
  });

  it('uses the plain conversation title when no local government is involved', async () => {
    const { el } = await mount((h) => h.id.set('CM-125876'));
    expect(el.querySelector('case-thread h2')!.textContent).toBe('Conversation with LCRB');
  });

  it('withdraws through the dialog: records the cancellation, then shows the cancelled card and a closed thread', async () => {
    const { fixture, el } = await mount();
    const trigger = [...el.querySelectorAll('submitted-file-panel button')].find((b) => b.textContent!.trim() === 'Cancel application') as HTMLButtonElement;
    trigger.click();
    await settle(fixture);
    const radio = document.querySelector('ui-dialog-content input[type="radio"]') as HTMLInputElement;
    radio.checked = true;
    radio.dispatchEvent(new Event('change'));
    await settle(fixture);
    (document.querySelector('ui-dialog-content [role="checkbox"]') as HTMLButtonElement).click();
    await settle(fixture);
    const confirm = [...document.querySelectorAll('ui-dialog-content button')].find((b) => b.textContent!.trim() === 'Cancel application') as HTMLButtonElement;
    confirm.click();
    await settle(fixture);
    const c = getCancellation('LP-125998')!;
    expect(c).toMatchObject({ applicant: 'Cedar Room Ltd.', cancelledBy: 'Dana Lee', priorStatus: 'under_review', reason: 'Plans have changed — no longer proceeding' });
    const card = el.querySelector('submitted-file-panel [data-slot="card"]')!;
    expect(card.className).toContain('border-zinc-300 bg-zinc-50');
    expect(card.textContent).toContain('Cancelled on');
    expect(card.textContent).toContain('You withdrew this application — Plans have changed — no longer proceeding. LCRB and the local government have been notified.');
    expect(el.querySelector('case-thread h2 + span')!.textContent!.trim()).toBe('Closed');
    expect(badge(el).textContent!.trim()).toBe('Cancelled');
  });

  it('shows neither card for an approved file and a closed thread', async () => {
    const { el } = await mount((h) => {
      h.id.set('CAT-125654');
      h.status.set('approved');
    });
    expect(el.querySelector('submitted-file-panel [data-slot="card"]')).toBeNull();
    expect(el.querySelector('case-thread h2 + span')!.textContent!.trim()).toBe('Closed');
  });
});
