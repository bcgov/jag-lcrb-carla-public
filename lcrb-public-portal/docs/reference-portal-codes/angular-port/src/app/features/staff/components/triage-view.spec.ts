import { Component } from '@angular/core';
import { TestBed } from '@angular/core/testing';
import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { SessionProvider } from '@core/session/session-provider';
import { QUEUE_META } from '@domain/staff/agent-log';
import { loadCases } from '@data-access/repositories/staff/lcrb-analytics';
import { stubFetch } from '../../../../../tests/support/fetch-stub';
import { CaseAgentLog } from './case-agent-log';
import { FitAndProperPanel } from './fit-and-proper-panel';
import { TriageView } from './triage-view';

async function settle(fixture: { detectChanges(): void; whenStable(): Promise<unknown> }) {
  for (let i = 0; i < 3; i++) {
    fixture.detectChanges();
    await fixture.whenStable();
    await new Promise((r) => setTimeout(r, 0));
  }
  fixture.detectChanges();
}

@Component({
  imports: [TriageView],
  template: `<triage-view [cases]="cases" [initialCaseId]="caseId" />`,
})
class TriageHost {
  cases = loadCases();
  caseId: string | null = null;
}

async function mountTriage(caseId: string | null = null, mutate?: (h: TriageHost) => void) {
  const fixture = TestBed.createComponent(TriageHost);
  fixture.componentInstance.caseId = caseId;
  mutate?.(fixture.componentInstance);
  document.body.appendChild(fixture.nativeElement);
  await settle(fixture);
  return { fixture, el: fixture.nativeElement as HTMLElement };
}

const tabs = (el: HTMLElement) => [...el.querySelectorAll<HTMLButtonElement>('button[triageQueueTab]')];
const rows = (el: HTMLElement) => [...el.querySelectorAll('[triageCaseRow]')];

beforeEach(() => {
  localStorage.clear();
  sessionStorage.clear();
  stubFetch({});
  TestBed.inject(SessionProvider).login({ role: 'lcrb', firstName: 'Sam', lastName: 'Rowe', organization: 'LCRB' } as never);
});
afterEach(() => {
  vi.unstubAllGlobals();
  vi.restoreAllMocks();
  document.body.innerHTML = '';
});

describe('TriageView', () => {
  it('renders the queue tabs with counts, the case count and the cases sorted by queue risk', async () => {
    const cases = loadCases();
    const { el } = await mountTriage();
    const t = tabs(el);
    expect(t.map((b) => b.textContent!.replace(/\s+/g, ' ').trim())).toEqual([
      `All cases ${cases.length}`,
      ...(['beyond-licence-cap', 'section-119-review', 'enhanced-review', 'standard-processing'] as const).map(
        (q) => `${QUEUE_META[q].label} ${cases.filter((c) => (c.queue ?? 'standard-processing') === q).length}`,
      ),
    ]);
    for (const c of ['bg-[#013366]', 'text-white', 'border-[#013366]']) expect(t[0].classList.contains(c)).toBe(true);
    expect(t[1].classList.contains('hover:border-[#013366]')).toBe(true);
    expect(el.textContent).toContain(`Showing ${cases.length} case${cases.length === 1 ? '' : 's'}`);
    expect(rows(el)).toHaveLength(cases.length);
    expect(rows(el)[0].classList.contains('hover:shadow-md')).toBe(true);
    expect(rows(el)[0].querySelector('button')?.textContent).toContain(
      [...cases].sort((a, b) => {
        const order = ['beyond-licence-cap', 'section-119-review', 'enhanced-review', 'standard-processing'];
        const d = order.indexOf(a.queue ?? 'standard-processing') - order.indexOf(b.queue ?? 'standard-processing');
        return d || (a.lastAttemptAt < b.lastAttemptAt ? 1 : -1);
      })[0].reference,
    );
  });

  it('filters by queue tab (with its description) and by search text', async () => {
    const cases = loadCases();
    const { fixture, el } = await mountTriage();
    const queue = (cases.find((c) => c.queue && c.queue !== 'standard-processing')?.queue ?? 'standard-processing') as keyof typeof QUEUE_META;
    const idx = ['beyond-licence-cap', 'section-119-review', 'enhanced-review', 'standard-processing'].indexOf(queue) + 1;
    tabs(el)[idx].click();
    await settle(fixture);
    expect(el.textContent).toContain(QUEUE_META[queue].description);
    expect(rows(el)).toHaveLength(cases.filter((c) => (c.queue ?? 'standard-processing') === queue).length);

    tabs(el)[0].click();
    await settle(fixture);
    const input = el.querySelector('input')!;
    input.value = 'zzz-no-match';
    input.dispatchEvent(new Event('input'));
    await settle(fixture);
    expect(rows(el)).toHaveLength(0);
    expect(el.textContent).toContain('No cases in this queue.');
    expect(el.textContent).toContain('Showing 0 cases');

    input.value = cases[0].reference.toUpperCase();
    input.dispatchEvent(new Event('input'));
    await settle(fixture);
    expect(rows(el).length).toBeGreaterThan(0);
  });

  it('opens the agent-log drawer from Open, shows the log, and closes it', async () => {
    const cases = loadCases();
    const { fixture, el } = await mountTriage();
    const open = [...rows(el)[0].querySelectorAll('button')].find((b) => b.textContent!.includes('Open'))!;
    open.click();
    await settle(fixture);
    const sheet = document.querySelector('ui-sheet-content') as HTMLElement;
    expect(sheet.getAttribute('data-state')).toBe('open');
    for (const c of ['w-full', 'sm:max-w-xl', 'overflow-y-auto']) expect(sheet.classList.contains(c)).toBe(true);
    expect(sheet.textContent).toContain('Pre-screening summary');
    expect(sheet.textContent).toContain('Checks performed');
    expect(sheet.textContent).toContain('The agent triages only');
    expect(cases.some((c) => sheet.textContent!.includes(c.reference))).toBe(true);
    (document.querySelector('ui-sheet-content button.absolute') as HTMLButtonElement).click();
    await settle(fixture);
    expect(document.querySelector('ui-sheet-content')).toBeNull();
  });

  it('opens the drawer on mount for a valid initial case id and ignores an unknown one', async () => {
    const cases = loadCases();
    const { fixture } = await mountTriage(cases[1].id);
    expect(document.querySelector('ui-sheet-content')?.textContent).toContain(cases[1].reference);
    fixture.destroy();
    document.body.innerHTML = '';
    TestBed.resetTestingModule();
    TestBed.inject(SessionProvider).login({ role: 'lcrb', firstName: 'Sam', lastName: 'Rowe', organization: 'LCRB' } as never);
    await mountTriage('does-not-exist');
    expect(document.querySelector('ui-sheet-content')).toBeNull();
  });

  it('retries a failed sync: the pill changes and the Retry button goes away', async () => {
    vi.spyOn(Math, 'random').mockReturnValue(0.5);
    const { fixture, el } = await mountTriage(null, (h) => {
      h.cases = h.cases.map((c, i) => (i === 0 ? { ...c, state: 'failed' as const, dynamicsCaseId: undefined } : c));
    });
    const failedRow = rows(el).find((r) => r.textContent!.includes('Sync failed'))!;
    expect(failedRow.textContent).toContain('Retry sync');
    [...failedRow.querySelectorAll('button')].find((b) => b.textContent!.includes('Retry sync'))!.click();
    await settle(fixture);
    expect(el.textContent).not.toContain('Retry sync');
    expect(el.textContent).not.toContain('Sync failed');
  });
});

describe('CaseAgentLog and FitAndProperPanel', () => {
  @Component({
    imports: [CaseAgentLog],
    template: `<case-agent-log [log]="log" [applicantName]="name" />`,
  })
  class LogHost {
    log = loadCases().find((c) => c.agentLog?.fitAndProper?.applicable)!.agentLog!;
    name = loadCases().find((c) => c.agentLog?.fitAndProper?.applicable)!.applicant;
  }

  it('renders the summary, confidence, checks, the fit and proper panel and the footnote', async () => {
    const fixture = TestBed.createComponent(LogHost);
    await settle(fixture);
    const el = fixture.nativeElement as HTMLElement;
    const log = fixture.componentInstance.log;
    expect(el.textContent).toContain(log.summary);
    expect(el.textContent).toContain(`${log.overallConfidence}%`);
    expect(el.textContent).toContain(log.requiresHumanReview ? 'Human review required' : 'No mandatory review flag');
    expect(el.textContent).toContain('Routed to');
    expect(el.textContent).toContain(log.queueRationale);
    for (const c of log.checks) expect(el.textContent).toContain(c.label);
    expect(el.querySelectorAll('ul.space-y-3 > li outcome-badge').length).toBe(log.checks.length);
    // Separators between checks, on small screens only.
    expect(el.querySelectorAll('[data-slot="separator"]').length).toBe(log.checks.length - 1);
    expect(el.querySelector('fit-and-proper-panel')).toBeTruthy();
    expect(el.textContent).toContain('Fit & proper:');
    expect(el.textContent).toContain(`Generated by ${log.model}`);
    expect(el.querySelectorAll('[role="progressbar"]').length).toBe(2);
  });

  it('expands a finding to show its evidence and collapses it again', async () => {
    const fixture = TestBed.createComponent(LogHost);
    await settle(fixture);
    const el = fixture.nativeElement as HTMLElement;
    const withEvidence = [...el.querySelectorAll('li[fpFindingRow]')].find((li) => {
      const fp = fixture.componentInstance.log.fitAndProper!;
      const f = fp.findings.find((x) => li.textContent!.includes(x.label));
      return f && f.evidence.length > 0;
    })!;
    const button = withEvidence.querySelector('button')!;
    expect(button.getAttribute('aria-expanded')).toBe('false');
    expect(withEvidence.querySelector('ul')).toBeNull();
    button.click();
    await settle(fixture);
    expect(button.getAttribute('aria-expanded')).toBe('true');
    expect(withEvidence.querySelector('ul')).toBeTruthy();
    expect(button.querySelector('svg:last-child')?.getAttribute('class')).toContain('rotate-180');
    button.click();
    await settle(fixture);
    expect(withEvidence.querySelector('ul')).toBeNull();
  });

  it('renders nothing for a non-applicable fit and proper result', async () => {
    @Component({ imports: [FitAndProperPanel], template: `<fit-and-proper-panel [fp]="fp" />` })
    class NaHost {
      fp = { ...loadCases().find((c) => c.agentLog?.fitAndProper?.applicable)!.agentLog!.fitAndProper!, applicable: false };
    }
    const fixture = TestBed.createComponent(NaHost);
    await settle(fixture);
    expect((fixture.nativeElement as HTMLElement).querySelector('div')).toBeNull();
  });
});
