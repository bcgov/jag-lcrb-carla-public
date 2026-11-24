import { type ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { API_FIXTURES } from '../../../../../tests/fixtures/api';
import type { ApiFixture } from '../../../../../tests/support/api-fixtures';
import { stubFetch } from '../../../../../tests/support/fetch-stub';
import { type CatalogItem, type HeldLicence, SuggestApplications } from './suggest-applications';

const CATALOG: CatalogItem[] = [
  { slug: 'licence-renewal', title: 'Licence renewal', description: 'Renew', licenceType: 'liquor', kind: 'renewal' },
];
const LICENCES: HeldLicence[] = [
  { type: 'Liquor Primary', name: 'The Pub', status: 'Active', expiryDate: '2030-01-01', flags: ['f1'] },
];

let fixture: ComponentFixture<SuggestApplications>;
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
async function mount(fixtures: Record<string, ApiFixture> = API_FIXTURES) {
  const api = stubFetch(fixtures);
  TestBed.configureTestingModule({ providers: [provideRouter([])] });
  fixture = TestBed.createComponent(SuggestApplications);
  fixture.componentRef.setInput('legalName', 'Fixture Ltd.');
  fixture.componentRef.setInput('licences', LICENCES);
  fixture.componentRef.setInput('availableApplications', CATALOG);
  el = fixture.nativeElement;
  await render();
  return api;
}
const btn = (re: RegExp) => [...el.querySelectorAll('button')].find((b) => re.test(b.getAttribute('aria-label') ?? b.textContent?.trim() ?? ''));

describe('SuggestApplications', () => {
  beforeEach(() => {
    sessionStorage.clear();
    localStorage.clear();
  });
  afterEach(() => vi.unstubAllGlobals());

  it('first render is the collapsed trigger and makes no request', async () => {
    const api = await mount();
    const trigger = el.querySelector('button')!;
    expect(trigger.textContent).toContain('AI: Suggest applications for my business');
    expect(trigger.className).toBe(
      'flex w-full items-center justify-between gap-2 rounded-lg border border-[#013366]/20 bg-[#013366]/5 px-4 py-3 text-left transition-colors hover:bg-[#013366]/10',
    );
    expect(api.calls).toHaveLength(0);
  });

  it('click posts licences with daysToExpiry, then shows the suggestion and link', async () => {
    const api = await mount();
    el.querySelector('button')!.click();
    await render();
    expect(el.textContent).toContain('Reviewing your licences…');
    await until(() => /Renew your licence/.test(el.textContent ?? ''), 'suggestions');
    const body = api.callsTo('/api/suggest-applications')[0].body as any;
    expect(body.legalName).toBe('Fixture Ltd.');
    expect(body.availableApplications).toEqual(CATALOG);
    expect(body.licences[0]).toMatchObject({ type: 'Liquor Primary', name: 'The Pub', expiryDate: '2030-01-01', flags: ['f1'] });
    expect(typeof body.licences[0].daysToExpiry).toBe('number');
    const link = el.querySelector('a[href="/applications/new/licence-renewal"]')!;
    expect(link.textContent).toContain('Start: Licence renewal');
    expect(el.textContent).toContain('Urgent');
    expect(el.firstElementChild?.className).toBe('rounded-lg border border-[#013366]/20 bg-card p-4');
  });

  it('falls back to a generic link label for an unknown slug', async () => {
    await mount({
      '/api/suggest-applications': { json: { suggestions: [{ title: 'T', reason: 'R', urgency: 'soon', appSlug: 'nope' }] } },
    });
    el.querySelector('button')!.click();
    await until(() => /Start this application/.test(el.textContent ?? ''), 'fallback label');
    expect(el.querySelector('a[href="/applications/new/nope"]')).toBeTruthy();
  });

  it('shows the empty message when there are no suggestions', async () => {
    await mount({ '/api/suggest-applications': { json: { suggestions: [] } } });
    el.querySelector('button')!.click();
    await until(() => /Nothing stands out/.test(el.textContent ?? ''), 'empty');
  });

  it('collapses and re-expands without a new request', async () => {
    const api = await mount();
    el.querySelector('button')!.click();
    await until(() => !!btn(/Collapse suggestions/), 'done');
    btn(/Collapse suggestions/)!.click();
    await render();
    expect(el.textContent).toContain('AI: Show suggested applications');
    el.querySelector('button')!.click();
    await render();
    expect(el.textContent).toContain('Suggested applications for your business');
    expect(api.callsTo('/api/suggest-applications')).toHaveLength(1);
  });

  it('shows the error state and Retry recovers', async () => {
    const api = await mount({
      '/api/suggest-applications': {
        sequence: [{ status: 500, json: { error: 'x' } }, API_FIXTURES['/api/suggest-applications']],
      },
    });
    el.querySelector('button')!.click();
    await until(() => /Couldn't generate suggestions right now\./.test(el.textContent ?? ''), 'error');
    btn(/^Retry$/)!.click();
    await until(() => /Renew your licence/.test(el.textContent ?? ''), 'recovered');
    expect(api.callsTo('/api/suggest-applications')).toHaveLength(2);
  });

  it('Refresh re-requests', async () => {
    const api = await mount();
    el.querySelector('button')!.click();
    await until(() => !!btn(/Refresh/), 'done');
    btn(/Refresh/)!.click();
    await until(() => api.callsTo('/api/suggest-applications').length === 2 && !!btn(/Refresh/), 'second request');
  });
});
