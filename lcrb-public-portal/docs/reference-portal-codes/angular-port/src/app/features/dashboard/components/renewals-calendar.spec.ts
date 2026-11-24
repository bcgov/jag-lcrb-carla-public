import { Component } from '@angular/core';
import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { buildCalendarEvents } from '@data-access/repositories/compliance/compliance-calendar';
import { toICS } from '@domain/compliance/compliance-calendar';
import { RenewalsCalendar } from './renewals-calendar';

@Component({ template: '' })
class Blank {}

const tick = () => new Promise((r) => setTimeout(r, 0));
async function settle(f: { detectChanges(): void; whenStable(): Promise<unknown> }) {
  f.detectChanges();
  await f.whenStable();
  await tick();
  f.detectChanges();
}

let blobs: Blob[];
let downloads: string[];
const realCreate = URL.createObjectURL;
const realRevoke = URL.revokeObjectURL;
beforeEach(() => {
  localStorage.clear();
  sessionStorage.clear();
  blobs = [];
  downloads = [];
  // Stub the download: no blob URL, no anchor click, no file.
  URL.createObjectURL = (b: Blob) => (blobs.push(b), 'blob:stub');
  URL.revokeObjectURL = () => undefined;
  vi.spyOn(HTMLAnchorElement.prototype, 'click').mockImplementation(function (this: HTMLAnchorElement) {
    downloads.push(this.download);
  });
  TestBed.configureTestingModule({ providers: [provideRouter([{ path: '**', component: Blank }])] });
});
afterEach(() => {
  vi.restoreAllMocks();
  URL.createObjectURL = realCreate;
  URL.revokeObjectURL = realRevoke;
  document.querySelectorAll('.cdk-overlay-container').forEach((c) => (c.innerHTML = ''));
});

async function mount() {
  const fixture = TestBed.createComponent(RenewalsCalendar);
  document.body.appendChild(fixture.nativeElement);
  fixture.detectChanges();
  const el = fixture.nativeElement as HTMLElement;
  await settle(fixture);
  return { fixture, el };
}
const byText = (root: ParentNode, sel: string, text: string) =>
  [...root.querySelectorAll<HTMLElement>(sel)].find((e) => e.textContent!.trim().startsWith(text))!;

describe('RenewalsCalendar', () => {
  it('renders summary pills, filter counts and agenda rows', async () => {
    const { el } = await mount();
    const total = buildCalendarEvents().length;
    expect(el.textContent).toContain('Total tracked');
    expect(byText(el, 'button[aria-pressed]', 'All').textContent).toContain(`All (${total})`);
    expect(el.querySelectorAll('div[eventrow]').length).toBe(total);
    expect(byText(el, 'button', 'Agenda').getAttribute('aria-pressed')).toBe('true');
  });

  it('filters by type and shows the empty message', async () => {
    const { fixture, el } = await mount();
    const hearings = buildCalendarEvents().filter((e) => e.type === 'hearing').length;
    byText(el, 'button[aria-pressed]', 'Hearings').click();
    await settle(fixture);
    expect(el.querySelectorAll('div[eventrow]').length).toBe(hearings);
    if (hearings === 0) expect(el.textContent).toContain('No hearing events to show.');
  });

  it('switches to the month grid and flips months', async () => {
    const { fixture, el } = await mount();
    byText(el, 'button', 'Month').click();
    await settle(fixture);
    expect(el.querySelector('[role="grid"]')).not.toBeNull();
    expect(el.querySelector('div[eventrow]')).toBeNull();
    const h2 = el.querySelector('month-grid h2')!;
    const before = h2.textContent;
    (el.querySelector('[aria-label="Next month"]') as HTMLElement).click();
    await settle(fixture);
    expect(h2.textContent).not.toBe(before);
  });

  it('downloads a single event as <id>.ics via Add to calendar', async () => {
    const { el } = await mount();
    byText(el, 'button', 'Add to calendar').click();
    expect(downloads[0]).toBe(`${buildCalendarEvents()[0].id}.ics`);
    expect(blobs[0].type).toBe('text/calendar;charset=utf-8');
  });

  it('exports everything from the dialog with the same content and filename', async () => {
    const { fixture, el } = await mount();
    byText(el, 'button', 'Export to calendar').click();
    await settle(fixture);
    const dialog = document.querySelector('[role="dialog"]')!;
    expect(dialog.textContent).toContain('Export to your calendar');
    byText(dialog, 'button', 'Download .ics').click();
    await settle(fixture);
    const events = buildCalendarEvents();
    expect(downloads).toEqual(['lcrb-compliance-calendar-all.ics']);
    expect(await blobs[0].text()).toBe(toICS(events));
    expect(document.querySelector('[role="dialog"]')).toBeNull();
  });

  it('validates a custom range', async () => {
    const { fixture, el } = await mount();
    byText(el, 'button', 'Export to calendar').click();
    await settle(fixture);
    const dialog = document.querySelector('[role="dialog"]')!;
    byText(dialog, 'button[role="radio"]', 'Custom range').click();
    await settle(fixture);
    const to = dialog.querySelector('#export-to') as HTMLInputElement;
    to.value = '2000-01-01';
    to.dispatchEvent(new Event('input'));
    await settle(fixture);
    expect(dialog.querySelector('[role="alert"]')!.textContent).toContain('end date must be on or after');
    expect(byText(dialog, 'button', 'Download .ics').hasAttribute('disabled')).toBe(true);
  });
});
