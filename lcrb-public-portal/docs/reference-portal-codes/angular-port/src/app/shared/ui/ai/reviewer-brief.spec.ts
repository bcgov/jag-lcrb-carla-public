import { type ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { API_FIXTURES } from '../../../../../tests/fixtures/api';
import { stubFetch } from '../../../../../tests/support/fetch-stub';
import { ReviewerBrief } from './reviewer-brief';

async function render(fixture: ComponentFixture<unknown>) {
  fixture.detectChanges();
  await fixture.whenStable();
}

async function until(fixture: ComponentFixture<unknown>, cond: () => boolean, label: string) {
  for (let i = 0; i < 100; i++) {
    await render(fixture);
    if (cond()) return;
    await new Promise((r) => setTimeout(r, 5));
  }
  throw new Error(`Timed out waiting for: ${label}`);
}

const byText = (el: HTMLElement, sel: string, text: string) =>
  Array.from(el.querySelectorAll<HTMLElement>(sel)).find((e) => e.textContent?.includes(text));

const inputs = {
  subjectType: 'sep',
  title: 'Community picnic',
  reviewerRole: 'police',
  facts: [{ label: 'Attendance', value: '50' }],
  decisionOptions: ['Approve permit', 'Deny'],
};

function make(extra: Record<string, unknown> = {}) {
  const fixture = TestBed.createComponent(ReviewerBrief);
  for (const [k, v] of Object.entries({ ...inputs, ...extra })) fixture.componentRef.setInput(k, v);
  return fixture;
}

describe('ReviewerBrief', () => {
  beforeEach(() => {
    sessionStorage.clear();
    localStorage.clear();
  });
  afterEach(() => vi.unstubAllGlobals());

  it('renders the idle state with no request', async () => {
    const stub = stubFetch(API_FIXTURES);
    const fixture = make();
    await render(fixture);
    const el = fixture.nativeElement as HTMLElement;
    expect(el.textContent).toContain('Generate brief');
    expect(el.textContent).toContain('Summarizes the risk signals and history for this submission.');
    expect(stub.calls).toHaveLength(0);
    const section = el.querySelector('section')!;
    expect(section.getAttribute('aria-label')).toBe('AI reviewer brief');
    // Verbatim template string, trailing space included, as React renders it.
    expect(section.getAttribute('class')).toBe('rounded-lg border p-5 border-[#013366]/20 bg-[#013366]/5 ');
    expect(byText(el, 'button', 'Generate brief')!.className).toBe(
      'inline-flex items-center gap-1.5 rounded-md bg-[#013366] px-3 py-1.5 text-sm font-medium text-white hover:bg-[#013366]/90',
    );
  });

  it('generates a brief: posts the serialized payload and shows the low-risk result', async () => {
    const stub = stubFetch(API_FIXTURES);
    const fixture = make({ className: 'extra' });
    await render(fixture);
    const el = fixture.nativeElement as HTMLElement;
    byText(el, 'button', 'Generate brief')!.click();
    await until(fixture, () => el.textContent!.includes('Suggested action'), 'result');
    expect(stub.callsTo('/api/reviewer-brief')).toHaveLength(1);
    expect(stub.callsTo('/api/reviewer-brief')[0].body).toEqual(inputs);
    expect(el.textContent).toContain('Low risk');
    expect(el.textContent).toContain('Check before deciding');
    expect(el.textContent).toContain('Confirm the serving times end before 20:00.');
    expect(el.textContent).toContain('Approve permit');
    expect(el.textContent).toContain('AI suggestion only — you make the decision. Verify against policy.');
    expect(el.querySelector('section')!.className).toContain('border-[#2E8540]/30 bg-[#2E8540]/5');
    expect(el.querySelector('section')!.className).toContain('extra');
    expect(el.querySelector('svg.lucide-shield-check')).not.toBeNull();
    expect(byText(el, 'button', 'Regenerate')).toBeDefined();
  });

  it('shows the high-risk styling and the warning icon', async () => {
    stubFetch({
      '/api/reviewer-brief': {
        json: { summary: 's', riskLevel: 'high', attentionItems: [], recommendedDecision: 'Deny', rationale: 'r' },
      },
    });
    const fixture = make();
    await render(fixture);
    const el = fixture.nativeElement as HTMLElement;
    byText(el, 'button', 'Generate brief')!.click();
    await until(fixture, () => el.textContent!.includes('High risk'), 'high');
    expect(el.querySelector('section')!.className).toContain('border-red-300 bg-red-50');
    expect(el.querySelector('svg.lucide-triangle-alert, svg.lucide-alert-triangle')).not.toBeNull();
    expect(el.textContent).not.toContain('Check before deciding');
  });

  it('shows the loading state while the request is pending', async () => {
    let release!: () => void;
    vi.stubGlobal(
      'fetch',
      vi.fn(() => new Promise<Response>((r) => (release = () => r(new Response(JSON.stringify({}), { status: 500 }))))),
    );
    const fixture = make();
    await render(fixture);
    const el = fixture.nativeElement as HTMLElement;
    byText(el, 'button', 'Generate brief')!.click();
    await until(fixture, () => el.textContent!.includes('Assessing this submission…'), 'loading');
    release();
  });

  it('shows the friendly error and retries', async () => {
    const stub = stubFetch({
      '/api/reviewer-brief': {
        sequence: [{ status: 500, json: { error: 'boom' } }, API_FIXTURES['/api/reviewer-brief']],
      },
    });
    const fixture = make();
    await render(fixture);
    const el = fixture.nativeElement as HTMLElement;
    byText(el, 'button', 'Generate brief')!.click();
    await until(fixture, () => el.textContent!.includes("Couldn't generate the brief."), 'error');
    byText(el, 'button', 'Retry')!.click();
    await until(fixture, () => el.textContent!.includes('Suggested action'), 'retry result');
    expect(stub.callsTo('/api/reviewer-brief')).toHaveLength(2);
  });

  it('serves a second mount from the session cache without a request', async () => {
    const stub = stubFetch(API_FIXTURES);
    const first = make();
    await render(first);
    byText(first.nativeElement, 'button', 'Generate brief')!.click();
    await until(first, () => (first.nativeElement as HTMLElement).textContent!.includes('Suggested action'), 'first');
    first.destroy();
    const second = make();
    await until(second, () => (second.nativeElement as HTMLElement).textContent!.includes('Suggested action'), 'cached');
    expect(stub.callsTo('/api/reviewer-brief')).toHaveLength(1);
    expect(Object.keys(sessionStorage).some((k) => k.startsWith('lcrb.brief.'))).toBe(true);
  });

  it('renders nothing when disabled', async () => {
    stubFetch(API_FIXTURES);
    const fixture = make({ disabled: true });
    await render(fixture);
    expect((fixture.nativeElement as HTMLElement).querySelector('section')).toBeNull();
  });
});
