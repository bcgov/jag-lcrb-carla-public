import { TestBed } from '@angular/core/testing';
import { jsPDF } from 'jspdf';
import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { SessionProvider } from '@core/session/session-provider';
import { LG_APPLICATIONS } from '@data-access/repositories/lg-review/mock/lg-applications';
import { summarizeDeadlines } from '@domain/lg-review/lg-review-calendar';
import { listSchedules } from '@data-access/repositories/lg-review/lg-report-schedule-service';
import { ReviewQueueTools } from './review-queue-tools';

const LG = { role: 'local-government', firstName: 'Lee', lastName: 'Chan', organization: 'City of Victoria', email: 'lee@victoria.ca' } as const;
const OWNER = 'lee@victoria.ca';

/** jsPDF's initialized hook swaps save() for a no-op so no PDF is written. */
const noSave = [
  'initialized',
  function (this: { save: (name: string) => void }) {
    this.save = () => undefined;
  },
];
const pdfEvents = () => (jsPDF.API as unknown as { events: unknown[] }).events;

async function settle(fixture: { detectChanges(): void; whenStable(): Promise<unknown> }) {
  fixture.detectChanges();
  await fixture.whenStable();
  await new Promise((r) => setTimeout(r, 0));
  fixture.detectChanges();
}

async function mount(session: Record<string, unknown> | null = LG) {
  if (session) TestBed.inject(SessionProvider).login(session as never);
  const fixture = TestBed.createComponent(ReviewQueueTools);
  fixture.componentRef.setInput('filteredApplications', LG_APPLICATIONS);
  fixture.componentRef.setInput('allApplications', LG_APPLICATIONS);
  fixture.componentRef.setInput('filters', { status: 'all', query: '' });
  fixture.componentRef.setInput('filterSummary', 'All statuses');
  document.body.appendChild(fixture.nativeElement);
  await settle(fixture);
  return { fixture, el: fixture.nativeElement as HTMLElement };
}
const button = (root: ParentNode, text: string) =>
  [...root.querySelectorAll('button')].find((b) => b.textContent?.trim() === text) as HTMLButtonElement;

describe('ReviewQueueTools', () => {
  beforeEach(() => {
    localStorage.clear();
    sessionStorage.clear();
    pdfEvents().push(noSave);
  });
  afterEach(() => {
    pdfEvents().splice(pdfEvents().indexOf(noSave), 1);
    vi.restoreAllMocks();
    document.querySelectorAll('.cdk-overlay-container').forEach((c) => (c.innerHTML = ''));
  });

  it('renders the deadline summary, downloads, email line and create controls', async () => {
    const { el } = await mount();
    const stats = summarizeDeadlines(LG_APPLICATIONS);
    expect(el.querySelector('h2')!.textContent).toBe('Deadline tools');
    expect(el.textContent).toContain(`${stats.open} application${stats.open === 1 ? '' : 's'} awaiting your comment`);
    expect(el.textContent).toContain('. Export your deadlines, download a due-date list, or have it emailed on a schedule.');
    expect(button(el, 'Download calendar (.ics)')).toBeTruthy();
    expect(button(el, 'Download PDF list')).toBeTruthy();
    expect(el.textContent).toContain('Scheduled emails send to lee@victoria.ca');
    expect(el.querySelector('label[for="lg-cadence"]')!.textContent).toBe('Frequency');
    expect(el.querySelector('#lg-cadence')!.textContent).toContain('Weekly — deadlines due within 7 days');
    expect(el.textContent).toContain('Filters captured: All statuses');
    expect(el.textContent).toContain('Next run would include');
    expect(el.textContent).toContain('(plus any overdue)');
    expect(el.querySelector('ul')).toBeNull();
    expect(el.textContent).toContain('Prototype: emails are simulated');
  });

  it('without an account email, delivery is unavailable', async () => {
    const { el } = await mount({ ...LG, email: undefined });
    expect(el.textContent).toContain('No email is associated with your account');
    expect(button(el, 'Schedule delivery').disabled).toBe(true);
  });

  it('downloads the calendar through a blob link', async () => {
    const { el } = await mount();
    const create = vi.spyOn(URL, 'createObjectURL').mockReturnValue('blob:x');
    vi.spyOn(URL, 'revokeObjectURL').mockImplementation(() => undefined);
    const click = vi.spyOn(HTMLAnchorElement.prototype, 'click').mockImplementation(() => undefined);
    button(el, 'Download calendar (.ics)').click();
    expect(create).toHaveBeenCalledTimes(1);
    expect(click).toHaveBeenCalledTimes(1);
  });

  it('schedules a delivery, pauses and deletes it behind a confirmation', async () => {
    const { fixture, el } = await mount();
    button(el, 'Schedule delivery').click();
    await settle(fixture);
    const created = listSchedules(OWNER);
    expect(created.length).toBe(1);
    expect(created[0].cadence).toBe('weekly');
    expect(el.textContent).toContain('Weekly report');
    expect(el.querySelector('li [data-slot="badge"]')!.textContent!.trim()).toBe('Active');

    const sw = el.querySelector('button[role="switch"]') as HTMLButtonElement;
    expect(sw.getAttribute('aria-label')).toBe('Pause schedule');
    sw.click();
    await settle(fixture);
    expect(listSchedules(OWNER)[0].active).toBe(false);
    expect(el.querySelector('button[role="switch"]')!.getAttribute('aria-label')).toBe('Resume schedule');
    expect(el.querySelector('li [data-slot="badge"]')!.textContent!.trim()).toBe('Paused');

    (el.querySelector('button[aria-label="Delete schedule"]') as HTMLButtonElement).click();
    await settle(fixture);
    const dialog = document.querySelector('ui-alert-dialog-content') as HTMLElement;
    expect(dialog.textContent).toContain('Remove this scheduled report?');
    expect(dialog.textContent).toContain('You will stop receiving the weekly email to lee@victoria.ca.');
    button(dialog, 'Remove').click();
    await settle(fixture);
    expect(listSchedules(OWNER).length).toBe(0);
    expect(el.querySelector('ul')).toBeNull();
  });

  it('"Send now" records a run with the PDF stubbed', async () => {
    const { fixture, el } = await mount();
    button(el, 'Schedule delivery').click();
    await settle(fixture);
    button(el, 'Send now').click();
    await new Promise((r) => setTimeout(r, 200));
    await settle(fixture);
    const s = listSchedules(OWNER)[0];
    expect(s.lastRunAt).toBeTruthy();
    expect(s.lastRunFilename).toMatch(/^LG-review-due-dates-.*\.pdf$/);
    expect(el.textContent).toContain('Last sent');
  });
});
