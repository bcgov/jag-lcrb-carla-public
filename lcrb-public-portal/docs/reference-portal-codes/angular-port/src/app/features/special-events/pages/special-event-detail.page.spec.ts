import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { jsPDF } from 'jspdf';
import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { SessionProvider } from '@core/session/session-provider';
import { SepStoreProvider } from '@data-access/repositories/special-events/sep-store-provider';
import { SEP_RECORDS } from '@data-access/repositories/special-events/mock/seps';
import { SpecialEventDetailPage } from './special-event-detail.page';

const settle = async (f: { detectChanges(): void; whenStable(): Promise<unknown> }) => {
  for (let i = 0; i < 3; i++) {
    f.detectChanges();
    await f.whenStable();
    await new Promise((r) => setTimeout(r, 0));
  }
  f.detectChanges();
};

// jsPDF's initialized hook swaps save() for a no-op so no PDF is written.
const saved: string[] = [];
const noSave = [
  'initialized',
  function (this: { save: (name: string) => void }) {
    this.save = (name: string) => void saved.push(name);
  },
];
const pdfEvents = () => (jsPDF.API as unknown as { events: unknown[] }).events;

async function mount(id: string) {
  TestBed.configureTestingModule({ providers: [provideRouter([])] });
  TestBed.inject(SessionProvider).login({ role: 'applicant', firstName: 'Alex', lastName: 'Rivera', organization: 'Society' } as never);
  const fixture = TestBed.createComponent(SpecialEventDetailPage);
  fixture.componentRef.setInput('id', id);
  document.body.appendChild(fixture.nativeElement);
  await settle(fixture);
  return { fixture, el: fixture.nativeElement as HTMLElement };
}
const button = (el: HTMLElement | Document, text: string) =>
  [...el.querySelectorAll('button')].find((b) => b.textContent!.trim().includes(text)) as HTMLButtonElement | undefined;

describe('SpecialEventDetailPage', () => {
  beforeEach(() => {
    localStorage.clear();
    sessionStorage.clear();
    saved.length = 0;
    pdfEvents().push(noSave);
  });
  afterEach(() => {
    pdfEvents().splice(pdfEvents().indexOf(noSave), 1);
    vi.unstubAllGlobals();
    document.body.innerHTML = '';
    document.querySelectorAll('.cdk-overlay-container').forEach((c) => (c.innerHTML = ''));
  });

  it('renders the permit-not-found state for an unknown id', async () => {
    const { el } = await mount('SEP-0000-00000');
    expect(el.querySelector('h1')!.textContent).toBe('Permit not found');
    expect(el.textContent).toContain("We couldn't find a Special Event Permit with ID SEP-0000-00000.");
    expect(el.querySelector('main a[href="/special-events"]')!.textContent).toContain('Back to my permits');
  });

  it('renders the pending permit with details, history, conversation and the cancel trigger', async () => {
    const r = SEP_RECORDS.find((x) => x.id === 'SEP-2026-04812')!;
    const { el } = await mount(r.id);
    expect(el.querySelector('h1')!.textContent).toContain(r.eventName);
    expect(el.textContent).toContain(`${r.id} · ${r.eventType} · ${r.jurisdiction}`);
    expect(el.textContent).toContain('Event details');
    expect(el.textContent).toContain(String(r.expectedAttendance));
    expect(el.textContent).toContain('Permit conditions');
    expect(el.textContent).toContain('No conditions have been applied yet.');
    expect(el.querySelectorAll('ol.space-y-4 > li').length).toBe(r.history.length);
    expect(el.querySelector('#conversation case-thread')).toBeTruthy();
    // A direct child of space-y-6, so it needs a real box to take the margin.
    expect((el.querySelector('status-explainer') as HTMLElement).style.display).toBe('block');
    expect(button(el, 'Cancel permit')).toBeTruthy();
    expect(button(el, 'Download permit')).toBeUndefined();
    expect(el.textContent).not.toContain('Your permit is approved');
  });

  it('cancels the permit through the dialog, updating the store and the banner', async () => {
    const r = SEP_RECORDS.find((x) => x.id === 'SEP-2026-04812')!;
    const { fixture, el } = await mount(r.id);
    button(el, 'Cancel permit')!.click();
    await settle(fixture);
    const dialog = document.querySelector('ui-dialog-content') as HTMLElement;
    expect(dialog.querySelector('h2')!.textContent).toBe('Cancel this permit?');
    const radio = dialog.querySelector('input[type="radio"]') as HTMLInputElement;
    radio.checked = true;
    radio.dispatchEvent(new Event('change'));
    await settle(fixture);
    (document.querySelector('[role="checkbox"]') as HTMLButtonElement).click();
    await settle(fixture);
    const confirm = [...dialog.querySelectorAll('button')].find((b) => b.textContent!.trim() === 'Cancel permit') as HTMLButtonElement;
    expect(confirm.disabled).toBe(false);
    confirm.click();
    await settle(fixture);
    expect(TestBed.inject(SepStoreProvider).getById(r.id)!.status).toBe('cancelled');
    expect(el.textContent).toContain('You cancelled this permit');
    expect(button(el, 'Cancel permit')).toBeUndefined();
  });

  it('shows the approved banner, conditions, and downloads the permit PDF', async () => {
    const r = SEP_RECORDS.find((x) => x.id === 'SEP-2026-04791')!;
    const { fixture, el } = await mount(r.id);
    expect(el.textContent).toContain('Your permit is approved');
    expect(el.textContent).toContain(`${r.conditions.length} applied`);
    button(el, 'Download permit')!.click();
    await settle(fixture);
    expect(saved.length).toBe(1);
  });
});
