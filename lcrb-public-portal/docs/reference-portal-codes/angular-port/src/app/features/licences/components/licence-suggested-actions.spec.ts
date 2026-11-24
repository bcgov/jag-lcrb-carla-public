import { type ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { API_FIXTURES } from '../../../../../tests/fixtures/api';
import { stubFetch } from '../../../../../tests/support/fetch-stub';
import { LicenceSuggestedActions } from './licence-suggested-actions';

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
const actions = [{ slug: 'renewal', title: 'Licence renewal', description: 'Renew it' }];

function make(extra: Record<string, unknown> = {}) {
  if (!configured) {
    TestBed.configureTestingModule({ providers: [provideRouter([])] });
    configured = true;
  }
  const fixture = TestBed.createComponent(LicenceSuggestedActions);
  const base = {
    licenceType: 'liquor',
    licenceName: 'Pub 1',
    status: 'Active',
    expiryDate: '2030-01-01',
    flags: ['late-fee'],
    isOperated: false,
    hasScheduledInspection: false,
    availableActions: actions,
    actionHref: (slug: string) => `/licensee/apply/${slug}?licence=1&x=a%20b`,
    ...extra,
  };
  for (const [k, v] of Object.entries(base)) fixture.componentRef.setInput(k, v);
  return fixture;
}

describe('LicenceSuggestedActions', () => {
  beforeEach(() => {
    configured = false;
    sessionStorage.clear();
    localStorage.clear();
  });
  afterEach(() => vi.unstubAllGlobals());

  it('renders the collapsed trigger first', async () => {
    const stub = stubFetch(API_FIXTURES);
    const fixture = make();
    await render(fixture);
    const btn = (fixture.nativeElement as HTMLElement).querySelector('button')!;
    expect(btn.textContent).toContain('AI: Suggest next actions');
    expect(btn.className).toBe(
      'flex w-full items-center justify-between gap-2 rounded-lg border border-[#013366]/20 bg-[#013366]/5 px-4 py-2.5 text-left transition-colors hover:bg-[#013366]/10',
    );
    expect(stub.calls).toHaveLength(0);
  });

  it('fetches, posts the full body and renders the result', async () => {
    const stub = stubFetch(API_FIXTURES);
    const fixture = make();
    await render(fixture);
    const el = fixture.nativeElement as HTMLElement;
    el.querySelector('button')!.click();
    await until(fixture, () => el.textContent!.includes('Renew before expiry'), 'result');
    const body = stub.callsTo('/api/licence-actions')[0].body as Record<string, unknown>;
    expect(body).toMatchObject({
      licenceType: 'liquor',
      licenceName: 'Pub 1',
      status: 'Active',
      expiryDate: '2030-01-01',
      flags: ['late-fee'],
      isOperated: false,
      hasScheduledInspection: false,
      availableActions: actions,
    });
    expect(typeof body['daysToExpiry']).toBe('number');
    expect(el.textContent).toContain('Soon');
    expect(el.textContent).toContain('AI-generated guidance · verify with official policy.');
    expect(el.querySelector('div.rounded-lg')!.className).toBe('rounded-lg border border-[#013366]/20 bg-card p-4');
    // The fixture suggestion has an empty actionSlug, so no link.
    expect(el.querySelector('a')).toBeNull();
  });

  it('renders the Start link with the identical href, label from availableActions, and hides it when operated', async () => {
    const json = { suggestions: [{ title: 'Do it', reason: 'why', urgency: 'routine', actionSlug: 'renewal' }] };
    stubFetch({ '/api/licence-actions': { json } });
    const fixture = make();
    await render(fixture);
    const el = fixture.nativeElement as HTMLElement;
    el.querySelector('button')!.click();
    await until(fixture, () => !!el.querySelector('a'), 'link');
    const a = el.querySelector('a')!;
    expect(a.getAttribute('href')).toBe('/licensee/apply/renewal?licence=1&x=a%20b');
    expect(a.textContent).toContain('Start: Licence renewal');

    stubFetch({ '/api/licence-actions': { json: { suggestions: [{ ...json.suggestions[0], actionSlug: 'unknown' }] } } });
    const other = make();
    await render(other);
    const oel = other.nativeElement as HTMLElement;
    oel.querySelector('button')!.click();
    await until(other, () => !!oel.querySelector('a'), 'link2');
    expect(oel.querySelector('a')!.textContent).toContain('Start this request');

    stubFetch({ '/api/licence-actions': { json } });
    const operated = make({ isOperated: true });
    await render(operated);
    const pel = operated.nativeElement as HTMLElement;
    pel.querySelector('button')!.click();
    await until(operated, () => pel.textContent!.includes('Do it'), 'operated');
    expect(pel.querySelector('a')).toBeNull();
  });

  it('collapses, re-expands, shows the empty message and the error state', async () => {
    const stub = stubFetch({ '/api/licence-actions': { sequence: [{ json: { suggestions: [] } }, { status: 500, json: {} }] } });
    const fixture = make();
    await render(fixture);
    const el = fixture.nativeElement as HTMLElement;
    el.querySelector('button')!.click();
    await until(fixture, () => el.textContent!.includes('Nothing needs your attention on this licence right now.'), 'empty');
    (el.querySelector('[aria-label="Collapse suggestions"]') as HTMLElement).click();
    await until(fixture, () => el.textContent!.includes('AI: Show suggested actions'), 'collapsed');
    el.querySelector('button')!.click();
    await until(fixture, () => el.textContent!.includes('Suggested next actions'), 'expanded');
    expect(stub.callsTo('/api/licence-actions')).toHaveLength(1);
    byText(el, 'button', 'Refresh')!.click();
    await until(fixture, () => el.textContent!.includes("Couldn't generate suggestions right now."), 'error');
  });
});
