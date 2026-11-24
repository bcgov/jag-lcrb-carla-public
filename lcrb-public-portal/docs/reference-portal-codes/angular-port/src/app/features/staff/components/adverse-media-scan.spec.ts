import { type ComponentFixture, TestBed } from '@angular/core/testing';
import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { API_FIXTURES } from '../../../../../tests/fixtures/api';
import type { ApiFixture } from '../../../../../tests/support/api-fixtures';
import { stubFetch } from '../../../../../tests/support/fetch-stub';
import { AdverseMediaScan } from './adverse-media-scan';

const PEOPLE = [{ name: 'Jane Doe', roles: ['Director'], entity: 'Fixture Holdings Ltd.' }];

const FLAGGED = {
  program: 'cannabis',
  escalationTarget: 'the SPD',
  scannedSubjects: [],
  overall: { outcome: 'flag', escalates: true, summary: 'Serious concern.' },
  subjects: [
    {
      name: 'Jane Doe',
      type: 'person',
      outcome: 'flag',
      matchConfidence: 'possible',
      summary: 'Found an article.',
      articles: [
        { title: 'Headline', publisher: 'Daily', date: '2024-01-02', url: 'https://example.com/a', relevance: 'Relevant.' },
        { title: 'Bare', publisher: null, date: null, url: 'https://example.com/b', relevance: 'Less.' },
      ],
    },
    { name: 'Fixture Holdings Ltd.', type: 'business', outcome: 'clear', matchConfidence: 'confirmed', summary: 'Nothing.', articles: [] },
  ],
};

let fixture: ComponentFixture<AdverseMediaScan>;
let el: HTMLElement;

async function render() {
  fixture.detectChanges();
  await fixture.whenStable();
}
async function until(cond: () => boolean, label: string) {
  for (let i = 0; i < 100; i++) {
    await render();
    if (cond()) return;
    await new Promise((r) => setTimeout(r, 5));
  }
  throw new Error(`Timed out waiting for: ${label}`);
}
async function mount(fixtures: Record<string, ApiFixture> = API_FIXTURES, autoRun?: boolean) {
  const api = stubFetch(fixtures);
  TestBed.configureTestingModule({});
  fixture = TestBed.createComponent(AdverseMediaScan);
  fixture.componentRef.setInput('program', 'cannabis');
  fixture.componentRef.setInput('applicantName', 'Jane Doe');
  fixture.componentRef.setInput('businessName', 'Fixture Holdings Ltd.');
  fixture.componentRef.setInput('people', PEOPLE);
  if (autoRun !== undefined) fixture.componentRef.setInput('autoRun', autoRun);
  el = fixture.nativeElement;
  await render();
  return api;
}
const btn = (re: RegExp) => [...el.querySelectorAll('button')].find((b) => re.test(b.getAttribute('aria-label') ?? b.textContent?.trim() ?? ''));

describe('AdverseMediaScan', () => {
  beforeEach(() => {
    sessionStorage.clear();
    localStorage.clear();
  });
  afterEach(() => vi.unstubAllGlobals());

  it('first render is the idle trigger and makes no request', async () => {
    const api = await mount();
    const trigger = el.querySelector('button')!;
    expect(trigger.textContent).toContain('Run adverse-media scan');
    expect(trigger.className).toBe(
      'flex w-full items-center justify-between gap-2 rounded-lg border border-[#013366]/20 bg-[#013366]/5 px-4 py-2.5 text-left transition-colors hover:bg-[#013366]/10',
    );
    expect(api.calls).toHaveLength(0);
  });

  it('click posts the subject, shows loading, then the clear result', async () => {
    const api = await mount();
    el.querySelector('button')!.click();
    await render();
    expect(el.textContent).toContain('Searching open sources for the business and associated individuals…');
    expect(el.textContent).toContain('RAG');
    await until(() => /Overall: Clear/.test(el.textContent ?? ''), 'result');
    expect(api.callsTo('/api/adverse-media')[0].body).toEqual({
      program: 'cannabis',
      applicantName: 'Jane Doe',
      businessName: 'Fixture Holdings Ltd.',
      people: PEOPLE,
    });
    expect(el.firstElementChild?.className).toBe('rounded-lg border border-[#013366]/20 bg-card p-4 text-left');
    expect(el.textContent).toContain('AI open-source scan');
  });

  it('renders escalation, match chips and article links for a flagged result', async () => {
    await mount({ '/api/adverse-media': { json: FLAGGED } });
    el.querySelector('button')!.click();
    await until(() => /Overall: Flag/.test(el.textContent ?? ''), 'flagged');
    expect(el.textContent).toContain('May warrant the SPD');
    expect(el.textContent).toContain('Possible match');
    expect(el.textContent).not.toContain('Match confirmed');
    const links = el.querySelectorAll('a[target="_blank"]');
    expect(links).toHaveLength(2);
    expect(links[0].getAttribute('href')).toBe('https://example.com/a');
    expect(links[0].getAttribute('rel')).toBe('noopener noreferrer');
    expect(links[0].textContent?.replace(/\s+/g, ' ').trim()).toBe('Headline — Daily (2024-01-02)');
    expect(links[1].textContent?.trim()).toBe('Bare');
    expect(el.querySelector('svg.lucide-building-2')).toBeTruthy();
    expect(el.querySelector('svg.lucide-user')).toBeTruthy();
  });

  it('autoRun fires the scan once on mount', async () => {
    const api = await mount(API_FIXTURES, true);
    await until(() => /Overall: Clear/.test(el.textContent ?? ''), 'auto result');
    expect(api.callsTo('/api/adverse-media')).toHaveLength(1);
  });

  it('serves a repeat mount from the session cache without a request', async () => {
    const api = await mount(API_FIXTURES, true);
    await until(() => /Overall: Clear/.test(el.textContent ?? ''), 'auto result');
    TestBed.resetTestingModule();
    await mount(API_FIXTURES, true);
    await until(() => /Overall: Clear/.test(el.textContent ?? ''), 'cached result');
    expect(api.callsTo('/api/adverse-media')).toHaveLength(1);
  });

  it('collapses to "Show adverse-media scan" and re-expands without a new request', async () => {
    const api = await mount();
    el.querySelector('button')!.click();
    await until(() => !!btn(/Collapse adverse-media scan/), 'done');
    btn(/Collapse adverse-media scan/)!.click();
    await render();
    expect(el.textContent).toContain('Show adverse-media scan');
    el.querySelector('button')!.click();
    await render();
    expect(el.textContent).toContain('Adverse-media scan');
    expect(api.callsTo('/api/adverse-media')).toHaveLength(1);
  });

  it('shows the error state and Retry recovers', async () => {
    const api = await mount({
      '/api/adverse-media': { sequence: [{ status: 500, json: { error: 'x' } }, API_FIXTURES['/api/adverse-media']] },
    });
    el.querySelector('button')!.click();
    await until(() => /Couldn't complete the scan right now\./.test(el.textContent ?? ''), 'error');
    btn(/^Retry$/)!.click();
    await until(() => /Overall: Clear/.test(el.textContent ?? ''), 'recovered');
    expect(api.callsTo('/api/adverse-media')).toHaveLength(2);
  });
});
