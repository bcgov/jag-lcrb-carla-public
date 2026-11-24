/** Visual id SEP-2026-04788 (under review, assigned to another officer), plus 04812 (submitted) and 04791 (auto-approved). PDF save() is stubbed; the AI endpoints use canned fixtures. */
import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { jsPDF } from 'jspdf';
import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { API_FIXTURES } from '../../../../../tests/fixtures/api';
import { stubFetch } from '../../../../../tests/support/fetch-stub';
import { SessionProvider } from '@core/session/session-provider';
import { SepStoreProvider } from '@data-access/repositories/special-events/sep-store-provider';
import { STANDARD_CONDITIONS } from '@domain/special-events/seps';
import { PoliceReviewPage } from './police-review.page';

const settle = async (f: { detectChanges(): void; whenStable(): Promise<unknown> }) => {
  for (let i = 0; i < 3; i++) {
    f.detectChanges();
    await f.whenStable();
    await new Promise((r) => setTimeout(r, 0));
  }
  f.detectChanges();
};

const saved: string[] = [];
const noSave = [
  'initialized',
  function (this: { save: (name: string) => void }) {
    this.save = (name: string) => void saved.push(name);
  },
];
const pdfEvents = () => (jsPDF.API as unknown as { events: unknown[] }).events;

const OFFICER = 'Cst. Dana Okafor';

async function mount(id: string) {
  TestBed.configureTestingModule({ providers: [provideRouter([])] });
  TestBed.inject(SessionProvider).login({ role: 'police', firstName: 'Dana', lastName: 'Okafor', organization: 'Victoria PD' } as never);
  const fixture = TestBed.createComponent(PoliceReviewPage);
  fixture.componentRef.setInput('id', id);
  document.body.appendChild(fixture.nativeElement);
  await settle(fixture);
  return { fixture, el: fixture.nativeElement as HTMLElement, store: TestBed.inject(SepStoreProvider) };
}
const button = (el: ParentNode, text: string) => [...el.querySelectorAll('button')].find((b) => b.textContent!.trim() === text) as HTMLButtonElement | undefined;
const dialog = () => document.querySelector('[role="dialog"]') as HTMLElement | null;
const type = (el: HTMLInputElement | HTMLTextAreaElement, value: string) => {
  el.value = value;
  el.dispatchEvent(new Event('input'));
};

describe('PoliceReviewPage', () => {
  beforeEach(() => {
    localStorage.clear();
    sessionStorage.clear();
    saved.length = 0;
    pdfEvents().push(noSave);
    stubFetch(API_FIXTURES);
  });
  afterEach(() => {
    pdfEvents().splice(pdfEvents().indexOf(noSave), 1);
    vi.unstubAllGlobals();
    document.body.innerHTML = '';
    document.querySelectorAll('.cdk-overlay-container').forEach((c) => (c.innerHTML = ''));
  });

  it('renders the inline not-found screen for an unknown id', async () => {
    const { el } = await mount('SEP-0000-00000');
    expect(el.querySelector('h1')!.textContent).toBe('Permit not found');
    expect(el.textContent).toContain("We couldn't find a Special Event Permit with ID SEP-0000-00000.");
    expect(el.querySelector('main a[href="/police/queue"]')!.textContent).toContain('Back to review queue');
  });

  it('renders the hero, details, applicant, history, thread, related permits and the reviewer brief', async () => {
    const { el } = await mount('SEP-2026-04788');
    expect(el.querySelector('h1')!.textContent).toContain('Westshore Charity Gala');
    expect(el.textContent).toContain('SEP-2026-04788 · ');
    expect(el.textContent).toContain('Event details');
    expect(el.textContent).toContain('Westshore Conference Centre');
    expect(el.textContent).toContain('Marcus Lee');
    expect(el.textContent).toContain('Review history');
    expect(el.querySelector('section[aria-label="Conversation with applicant and LCRB"]')).not.toBeNull();
    expect(el.textContent).toContain('Permit history');
    expect(el.querySelector('section[aria-label="AI reviewer brief"]')).not.toBeNull();
    expect(button(el, 'Full PDF')).toBeTruthy();
    expect(el.textContent).toContain('Officer field copy');
    // Direct children of space-y parents need real boxes (the prototype renders no wrapper).
    expect((el.querySelector('reviewer-brief') as HTMLElement).style.display).toBe('block');
    expect((el.querySelector('case-thread') as HTMLElement).style.display).toBe('block');
    const dialogs = ([...el.querySelectorAll('ui-dialog')] as HTMLElement[]).filter((d) =>
      /Approve permit|Request changes|Deny permit/.test(d.querySelector('button')?.textContent ?? ''),
    );
    expect(dialogs.length).toBe(3);
    expect(dialogs.every((d) => d.style.display === 'block')).toBe(true);
  });

  it('requires assignment first: reassign, then the decision buttons enable', async () => {
    const { fixture, el, store } = await mount('SEP-2026-04788');
    expect(el.textContent).toContain('Assigned to Cst. A. Romero.');
    expect(button(el, 'Approve permit')!.disabled).toBe(true);
    button(el, 'Reassign to me')!.click();
    // Rendered before the click returns, as React does (no settle yet).
    expect(button(el, 'Approve permit')!.disabled).toBe(false);
    await settle(fixture);
    expect(store.getById('SEP-2026-04788')!.assignedOfficer).toBe(OFFICER);
    expect(button(el, 'Approve permit')!.disabled).toBe(false);
    expect(el.textContent).not.toContain('Assigned to Cst. A. Romero.');
  });

  it('approves with a note: the dialog updates the SEP store and the page shows the decided state', async () => {
    const { fixture, el, store } = await mount('SEP-2026-04788');
    button(el, 'Reassign to me')!.click();
    await settle(fixture);
    button(el, 'Approve permit')!.click();
    await settle(fixture);
    expect(dialog()!.textContent).toContain('Approve permit');
    expect(dialog()!.textContent).toMatch(/Approving with \d+ conditions?\./);
    type(dialog()!.querySelector('textarea')!, 'Looks fine');
    await settle(fixture);
    button(dialog()!, 'Confirm approval')!.click();
    await settle(fixture);
    const r = store.getById('SEP-2026-04788')!;
    expect(r.status).toBe('approved');
    expect(r.history.at(-1)).toMatchObject({ action: 'Approved', actor: OFFICER, note: 'Looks fine' });
    expect(el.textContent).toContain('This permit is approved. No further action is required.');
    expect(dialog()).toBeNull();
  });

  it('refuses only with a reason', async () => {
    const { fixture, el, store } = await mount('SEP-2026-04788');
    button(el, 'Reassign to me')!.click();
    await settle(fixture);
    button(el, 'Deny permit')!.click();
    await settle(fixture);
    expect(button(dialog()!, 'Confirm denial')!.disabled).toBe(true);
    type(dialog()!.querySelector('textarea[aria-label="Reason for denial"]') as HTMLTextAreaElement, '  No site plan  ');
    await settle(fixture);
    expect(button(dialog()!, 'Confirm denial')!.disabled).toBe(false);
    button(dialog()!, 'Confirm denial')!.click();
    await settle(fixture);
    const r = store.getById('SEP-2026-04788')!;
    expect(r.status).toBe('denied');
    expect(r.history.at(-1)).toMatchObject({ action: 'Denied', note: 'No site plan' });
    expect(el.textContent).toContain('This permit is denied.');
  });

  it('requests changes only with a note', async () => {
    const { fixture, el, store } = await mount('SEP-2026-04812');
    button(el, 'Assign to me')!.click();
    await settle(fixture);
    button(el, 'Request changes')!.click();
    await settle(fixture);
    expect(button(dialog()!, 'Send request')!.disabled).toBe(true);
    type(dialog()!.querySelector('textarea')!, 'Please add a site plan');
    await settle(fixture);
    button(dialog()!, 'Send request')!.click();
    await settle(fixture);
    const r = store.getById('SEP-2026-04812')!;
    expect(r.status).toBe('changes-requested');
    expect(r.history.at(-1)).toMatchObject({ action: 'Requested changes', note: 'Please add a site plan' });
  });

  it('adds and removes standard and custom conditions through the store', async () => {
    const { fixture, el, store } = await mount('SEP-2026-04812');
    const before = store.getById('SEP-2026-04812')!.conditions.length;
    const chips = [...el.querySelectorAll('button.rounded-full')];
    expect(chips.length).toBe(STANDARD_CONDITIONS.length - before);
    (chips[0] as HTMLElement).click();
    await settle(fixture);
    expect(store.getById('SEP-2026-04812')!.conditions).toHaveLength(before + 1);

    const input = el.querySelector('input[placeholder="Add a custom condition…"]') as HTMLInputElement;
    type(input, 'No glass containers');
    await settle(fixture);
    input.dispatchEvent(new KeyboardEvent('keydown', { key: 'Enter', cancelable: true }));
    await settle(fixture);
    const conditions = store.getById('SEP-2026-04812')!.conditions;
    expect(conditions.at(-1)).toMatchObject({ text: 'No glass containers', standard: false });
    expect(input.value).toBe('');

    (el.querySelector('button[aria-label="Remove condition"]') as HTMLElement).click();
    await settle(fixture);
    expect(store.getById('SEP-2026-04812')!.conditions).toHaveLength(conditions.length - 1);
  });

  it('shows the auto-approved banner and no decision for an auto-approved permit', async () => {
    const { el } = await mount('SEP-2026-04791');
    expect(el.textContent).toContain('For awareness only — auto-approved');
    // As in the prototype, `decided` includes auto-approved, so that branch wins over the "don't require" one.
    expect(el.textContent).toContain('This permit is auto-approved. No further action is required.');
    expect(button(el, 'Approve permit')).toBeUndefined();
  });

  it('downloads the full PDF and a field copy honouring the section toggles', async () => {
    const { fixture, el } = await mount('SEP-2026-04788');
    button(el, 'Full PDF')!.click();
    expect(saved).toHaveLength(1);
    (el.querySelector('#pdf-history') as HTMLElement).click();
    await settle(fixture);
    expect(el.querySelector('#pdf-history')!.getAttribute('data-state')).toBe('checked');
    button(el, 'Download field copy')!.click();
    expect(saved).toHaveLength(2);
  });
});
