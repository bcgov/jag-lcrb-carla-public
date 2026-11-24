import { type ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { API_FIXTURES } from '../../../../../tests/fixtures/api';
import { stubFetch } from '../../../../../tests/support/fetch-stub';
import { ReviewerSuggestedActions } from './reviewer-suggested-actions';

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

let configured = false;

function make(extra: Record<string, unknown> = {}) {
  if (!configured) {
    TestBed.configureTestingModule({ providers: [provideRouter([])] });
    configured = true;
  }
  const fixture = TestBed.createComponent(ReviewerSuggestedActions);
  const base = {
    role: 'police',
    itemTitle: 'Community picnic',
    itemId: 'SEP-1',
    status: 'submitted',
    facts: [{ label: 'Date', value: '2026-07-01' }],
    reviewHref: '/police/review/SEP-1?tab=docs',
    ...extra,
  };
  for (const [k, v] of Object.entries(base)) fixture.componentRef.setInput(k, v);
  return fixture;
}

describe('ReviewerSuggestedActions', () => {
  beforeEach(() => {
    configured = false;
    sessionStorage.clear();
    localStorage.clear();
  });
  afterEach(() => vi.unstubAllGlobals());

  it('renders the collapsed trigger first, with dense and default padding', async () => {
    stubFetch(API_FIXTURES);
    const fixture = make();
    await render(fixture);
    const el = fixture.nativeElement as HTMLElement;
    const btn = el.querySelector('button')!;
    expect(btn.textContent).toContain('AI: Suggest next actions');
    expect(btn.className).toBe(
      'flex w-full items-center justify-between gap-2 rounded-lg border border-[#013366]/20 bg-[#013366]/5 text-left transition-colors hover:bg-[#013366]/10 px-4 py-2.5',
    );
    const dense = make({ dense: true });
    await render(dense);
    expect((dense.nativeElement as HTMLElement).querySelector('button')!.className).toContain('px-3 py-1.5');
  });

  it('fetches suggestions, posts the body and renders the Open review link', async () => {
    const stub = stubFetch(API_FIXTURES);
    const fixture = make();
    await render(fixture);
    const el = fixture.nativeElement as HTMLElement;
    el.querySelector('button')!.click();
    await until(fixture, () => el.textContent!.includes('Assign the file to yourself'), 'result');
    expect(stub.callsTo('/api/reviewer-actions')[0].body).toEqual({
      role: 'police',
      itemTitle: 'Community picnic',
      itemId: 'SEP-1',
      status: 'submitted',
      facts: [{ label: 'Date', value: '2026-07-01' }],
    });
    expect(el.textContent).toContain('Suggested next actions');
    expect(el.textContent).toContain('Urgent');
    expect(el.querySelector('div.rounded-lg')!.className).toBe('rounded-lg border border-[#013366]/20 bg-card p-4 text-left');
    const link = byText(el, 'a', 'Open review') as HTMLAnchorElement;
    expect(link.getAttribute('href')).toBe('/police/review/SEP-1?tab=docs');
    expect(el.textContent).toContain('AI-generated guidance · you make the decision.');
  });

  it('collapses and re-expands from memory without a new request', async () => {
    const stub = stubFetch(API_FIXTURES);
    const fixture = make();
    await render(fixture);
    const el = fixture.nativeElement as HTMLElement;
    el.querySelector('button')!.click();
    await until(fixture, () => !!el.querySelector('[aria-label="Collapse suggestions"]'), 'done');
    (el.querySelector('[aria-label="Collapse suggestions"]') as HTMLElement).click();
    await until(fixture, () => el.textContent!.includes('AI: Show suggested actions'), 'collapsed');
    el.querySelector('button')!.click();
    await until(fixture, () => el.textContent!.includes('Suggested next actions'), 'expanded');
    expect(stub.callsTo('/api/reviewer-actions')).toHaveLength(1);
  });

  it('shows the empty message and hides links when nothing opens a review', async () => {
    stubFetch({ '/api/reviewer-actions': { json: { suggestions: [] } } });
    const fixture = make();
    await render(fixture);
    const el = fixture.nativeElement as HTMLElement;
    el.querySelector('button')!.click();
    await until(fixture, () => el.textContent!.includes('Nothing needs your attention on this file right now.'), 'empty');
  });

  it('shows the friendly error and retries', async () => {
    const stub = stubFetch({
      '/api/reviewer-actions': { sequence: [{ status: 500, json: { error: 'x' } }, API_FIXTURES['/api/reviewer-actions']] },
    });
    const fixture = make();
    await render(fixture);
    const el = fixture.nativeElement as HTMLElement;
    el.querySelector('button')!.click();
    await until(fixture, () => el.textContent!.includes("Couldn't generate suggestions right now."), 'error');
    byText(el, 'button', 'Retry')!.click();
    await until(fixture, () => el.textContent!.includes('Assign the file to yourself'), 'retry');
    expect(stub.callsTo('/api/reviewer-actions')).toHaveLength(2);
  });
});
