/** PDF save() is stubbed. */
import { TestBed } from '@angular/core/testing';
import { jsPDF } from 'jspdf';
import { afterEach, beforeEach, describe, expect, it } from 'vitest';
import { SessionProvider } from '@core/session/session-provider';
import { SEP_RECORDS } from '@data-access/repositories/special-events/mock/seps';
import { listSchedules } from '@data-access/repositories/police-review/sep-report-schedule-service';
import { ScheduledReportsCard } from './scheduled-reports-card';

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

const EMAIL = 'officer@police.example';

async function mount(email: string | undefined = EMAIL) {
  TestBed.inject(SessionProvider).login({ role: 'police', firstName: 'Dana', lastName: 'Okafor', organization: 'Victoria PD', email } as never);
  const fixture = TestBed.createComponent(ScheduledReportsCard);
  fixture.componentRef.setInput('records', SEP_RECORDS);
  fixture.componentRef.setInput('filters', { city: 'all', status: 'all', query: '' });
  document.body.appendChild(fixture.nativeElement);
  await settle(fixture);
  return { fixture, el: fixture.nativeElement as HTMLElement };
}
const button = (el: ParentNode, text: string) => [...el.querySelectorAll('button')].find((b) => b.textContent!.trim() === text) as HTMLButtonElement | undefined;

describe('ScheduledReportsCard', () => {
  beforeEach(() => {
    localStorage.clear();
    saved.length = 0;
    pdfEvents().push(noSave);
  });
  afterEach(() => {
    pdfEvents().splice(pdfEvents().indexOf(noSave), 1);
    document.body.innerHTML = '';
    document.querySelectorAll('.cdk-overlay-container').forEach((c) => (c.innerHTML = ''));
  });

  it('renders nothing without a session', async () => {
    const fixture = TestBed.createComponent(ScheduledReportsCard);
    fixture.componentRef.setInput('records', SEP_RECORDS);
    fixture.componentRef.setInput('filters', { city: 'all', status: 'all', query: '' });
    await settle(fixture);
    expect(fixture.nativeElement.querySelector('h2')).toBeNull();
  });

  it('shows the heading, the recipient, the captured filters and the run preview', async () => {
    const { el } = await mount();
    expect(el.querySelector('h2')!.textContent).toBe('Automatic report delivery');
    expect(el.textContent).toContain(`Sends to ${EMAIL}`);
    expect(el.textContent).toContain('Filters captured: All cities  ·  All statuses');
    expect(el.textContent).toMatch(/Next run would cover .* \(\d+ permits? right now\)/);
    expect(el.textContent).toContain('Prototype: emails are simulated');
    expect(el.querySelector('ul')).toBeNull();
  });

  it('disables scheduling when the account has no email', async () => {
    const { el } = await mount('');
    expect(el.textContent).toContain('No email is associated with your account, so automatic delivery is unavailable.');
    expect(button(el, 'Schedule delivery')!.disabled).toBe(true);
  });

  it('creates a schedule, pauses it, sends it now (saving the PDF) and removes it', async () => {
    const { fixture, el } = await mount();
    button(el, 'Schedule delivery')!.click();
    await settle(fixture);
    expect(el.querySelectorAll('ul > li')).toHaveLength(1);
    expect(el.textContent).toContain('Daily report');
    expect(el.textContent).toContain('Active');
    expect(listSchedules(EMAIL)).toHaveLength(1);

    const toggle = el.querySelector('button[role="switch"]') as HTMLButtonElement;
    expect(toggle.getAttribute('aria-label')).toBe('Pause schedule');
    toggle.click();
    await settle(fixture);
    expect(el.textContent).toContain('Paused');
    expect(el.querySelector('button[role="switch"]')!.getAttribute('aria-label')).toBe('Resume schedule');

    button(el, 'Send now')!.click();
    await settle(fixture);
    await new Promise((r) => setTimeout(r, 50));
    await settle(fixture);
    expect(saved).toHaveLength(1);
    expect(saved[0]).toMatch(/^SEP-daily-report-/);
    expect(el.textContent).toContain('Last sent');

    (el.querySelector('button[aria-label="Delete schedule"]') as HTMLButtonElement).click();
    await settle(fixture);
    const dialog = document.querySelector('[role="alertdialog"], [role="dialog"]')!;
    expect(dialog.textContent).toContain('Remove this scheduled report?');
    expect(dialog.textContent).toContain(`You will stop receiving the daily email to ${EMAIL}.`);
    button(dialog, 'Remove')!.click();
    await settle(fixture);
    expect(listSchedules(EMAIL)).toHaveLength(0);
    expect(el.querySelector('ul')).toBeNull();
  });
});
