/** the Local Government review page and its decision flow. */
import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { jsPDF } from 'jspdf';
import { afterEach, beforeEach, describe, expect, it } from 'vitest';
import { LgStoreProvider } from '@data-access/repositories/lg-review/lg-store-provider';
import { SessionProvider } from '@core/session/session-provider';
import { LG_APPLICATIONS } from '@data-access/repositories/lg-review/mock/lg-applications';
import { LG_STATUS_LABELS } from '@domain/models/lg-review';
import { LocalGovernmentReviewPage } from './local-government-review.page';

const LG = { role: 'local-government', firstName: 'Priya', lastName: 'Sandhu', organization: 'City of Victoria', email: 'p@victoria.ca' } as const;
const JOB = 'LP-125998';

/** No file is written: jsPDF's initialized hook swaps each new document's save() for a no-op. */
let saved: string[] = [];
const noSave = [
  'initialized',
  function (this: { save: (name: string) => void }) {
    this.save = (name: string) => void saved.push(name);
  },
];
const pdfEvents = () => (jsPDF.API as unknown as { events: unknown[] }).events;

async function settle(fixture: { detectChanges(): void; whenStable(): Promise<unknown> }) {
  fixture.detectChanges();
  await fixture.whenStable();
  await new Promise((r) => setTimeout(r, 0));
  fixture.detectChanges();
}

async function mount(id = JOB) {
  TestBed.inject(SessionProvider).login(LG as never);
  const fixture = TestBed.createComponent(LocalGovernmentReviewPage);
  fixture.componentRef.setInput('id', id);
  document.body.appendChild(fixture.nativeElement);
  await settle(fixture);
  return { fixture, el: fixture.nativeElement as HTMLElement };
}
const button = (root: ParentNode, text: string) =>
  [...root.querySelectorAll('button')].find((b) => b.textContent?.trim() === text) as HTMLButtonElement;
const type = (el: HTMLTextAreaElement, value: string) => {
  el.value = value;
  el.dispatchEvent(new Event('input'));
};
const record = () => TestBed.inject(LgStoreProvider).getByJobNumber(JOB)!;

describe('LocalGovernmentReviewPage', () => {
  beforeEach(() => {
    localStorage.clear();
    sessionStorage.clear();
    saved = [];
    pdfEvents().push(noSave);
    TestBed.configureTestingModule({ providers: [provideRouter([])] });
  });
  afterEach(() => pdfEvents().splice(pdfEvents().indexOf(noSave), 1));

  it('renders the hero, due banner, tabs and the decision panel for a pending referral', async () => {
    const { el } = await mount();
    const app = LG_APPLICATIONS.find((a) => a.jobNumber === JOB)!;
    expect(el.querySelector('h1')!.textContent).toContain(app.establishmentName);
    expect(el.textContent).toContain(`Job #${JOB} · ${app.applicationType} · ${app.licenceType}`);
    expect(el.textContent).toContain('Comment due ');
    expect(el.textContent).toContain('Reviewing as Priya Sandhu');
    expect([...el.querySelectorAll('[role="tab"]')].map((t) => t.textContent!.trim())).toEqual([
      'Overview',
      'Establishment',
      'Documents',
      'History',
      'Applicant history',
      'Messages',
    ]);
    expect(el.textContent).toContain('Hours of operation');
    expect(el.querySelector('#comment')).toBeTruthy();
    expect(el.querySelector('label[for="comment"]')!.textContent).toBe('Comments to LCRB');
    expect(button(el, 'Request documents').disabled).toBe(true);
  });

  it('switches tabs', async () => {
    const { fixture, el } = await mount();
    const tab = (name: string) => [...el.querySelectorAll<HTMLButtonElement>('[role="tab"]')].find((t) => t.textContent!.trim() === name)!;
    tab('Establishment').click();
    await settle(fixture);
    expect(el.textContent).toContain('Compliance snapshot');
    tab('Documents').click();
    await settle(fixture);
    expect(el.textContent).toContain('Attached documents');
    tab('History').click();
    await settle(fixture);
    expect(el.textContent).toContain('Status history');
    tab('Applicant history').click();
    await settle(fixture);
    expect(el.querySelector('applicant-history')!.textContent).toContain('Previous applications');
    tab('Messages').click();
    await settle(fixture);
    expect(el.querySelector('case-thread')!.textContent).toContain('Conversation with applicant and LCRB');
  });

  it('records a no-comment decision into the LG store and shows the confirmation', async () => {
    const { fixture, el } = await mount();
    type(el.querySelector('#comment')!, 'No concerns');
    await settle(fixture);
    button(el, 'No comment / No objection').click();
    await settle(fixture);
    expect(el.textContent).toContain('Comment recorded: No Comment.');
    expect(record().status).toBe('no-comment');
    expect(record().history.some((h) => h.label.includes('No concerns') && h.actor === 'Priya Sandhu')).toBe(true);
    expect(JSON.parse(localStorage.getItem('lcrb.lg.applications')!).find((r: { jobNumber: string }) => r.jobNumber === JOB).status).toBe('no-comment');
    expect(el.querySelector('#comment')).toBeNull();
    expect(LG_STATUS_LABELS['no-comment']).toBe('No Comment');
  });

  it('requests documents once a message is entered', async () => {
    const { fixture, el } = await mount();
    type(el.querySelector('#docs')!, 'Please provide the updated floor plan.');
    await settle(fixture);
    expect(button(el, 'Request documents').disabled).toBe(false);
    button(el, 'Request documents').click();
    await settle(fixture);
    expect(el.textContent).toContain('Documents requested from the applicant. The comment clock is paused.');
    expect(record().status).toBe('documents-requested');
  });

  it('recommends community review and rejection', async () => {
    const first = await mount();
    button(first.el, 'Recommend community review').click();
    await settle(first.fixture);
    expect(first.el.textContent).toContain('Comment recorded: Community Review.');
    expect(record().status).toBe('community-review');

    const second = await mount('089817');
    button(second.el, 'Recommend rejection').click();
    await settle(second.fixture);
    expect(TestBed.inject(LgStoreProvider).getByJobNumber('089817')!.status).toBe('reject');
  });

  it('downloads the application PDF with save() stubbed', async () => {
    const { el } = await mount();
    button(el, 'Download full application (PDF)').click();
    await new Promise((r) => setTimeout(r, 100));
    expect(saved.length).toBe(1);
  });

  it('shows the already-recorded state for a completed referral', async () => {
    const { el } = await mount('560021');
    expect(el.textContent).toContain('A comment has already been recorded for this application (No Comment).');
    expect(el.querySelector('#comment')).toBeNull();
  });

  it('renders the not-found state for an unknown id', async () => {
    const { el } = await mount('NOPE-1');
    expect(el.querySelector('h1')!.textContent).toBe('Application not found');
    expect(el.textContent).toContain('We couldn’t find an application with job #NOPE-1.'.replace('’', "'"));
    expect(el.querySelector('main a[href="/local-government/queue"]')!.textContent).toContain('Back to review queue');
  });
});
