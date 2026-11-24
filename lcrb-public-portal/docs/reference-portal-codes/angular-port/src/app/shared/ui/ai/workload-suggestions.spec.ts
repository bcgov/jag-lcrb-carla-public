import { type ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { API_FIXTURES } from '../../../../../tests/fixtures/api';
import type { ApiFixture } from '../../../../../tests/support/api-fixtures';
import { stubFetch } from '../../../../../tests/support/fetch-stub';
import { WorkloadSuggestions, type WorkloadLink } from './workload-suggestions';

const STATS = [{ label: 'Open', value: '4' }];
const ITEMS = [{ label: 'File A', detail: 'Unassigned' }];

let fixture: ComponentFixture<WorkloadSuggestions>;
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
async function mount(
  fixtures: Record<string, ApiFixture> = API_FIXTURES,
  inputs: Record<string, unknown> = {},
) {
  const api = stubFetch(fixtures);
  TestBed.configureTestingModule({ providers: [provideRouter([])] });
  fixture = TestBed.createComponent(WorkloadSuggestions);
  fixture.componentRef.setInput('scope', 'compliance');
  fixture.componentRef.setInput('stats', STATS);
  fixture.componentRef.setInput('items', ITEMS);
  for (const [k, v] of Object.entries(inputs)) fixture.componentRef.setInput(k, v);
  el = fixture.nativeElement;
  await render();
  return api;
}
const btn = (re: RegExp) => [...el.querySelectorAll('button')].find((b) => re.test(b.getAttribute('aria-label') ?? b.textContent?.trim() ?? ''));

describe('WorkloadSuggestions', () => {
  beforeEach(() => {
    sessionStorage.clear();
    localStorage.clear();
  });
  afterEach(() => vi.unstubAllGlobals());

  it('first render is the collapsed trigger with the default label and no request', async () => {
    const api = await mount();
    const trigger = el.querySelector('button')!;
    expect(trigger.textContent).toContain('AI: Suggest priorities');
    expect(trigger.className).toBe(
      'flex w-full items-center justify-between gap-2 rounded-lg border border-[#013366]/20 bg-[#013366]/5 px-4 py-2.5 text-left transition-colors hover:bg-[#013366]/10',
    );
    expect(api.calls).toHaveLength(0);
  });

  it('uses a custom trigger label and heading', async () => {
    await mount(API_FIXTURES, { triggerLabel: 'AI: Plan my week', heading: 'My plan' });
    expect(el.textContent).toContain('AI: Plan my week');
    el.querySelector('button')!.click();
    await until(() => /My plan/.test(el.textContent ?? ''), 'heading');
    await until(() => /Triage the unassigned files/.test(el.textContent ?? ''), 'done');
  });

  it('posts scope, stats, items and links, then renders suggestions', async () => {
    const links: WorkloadLink[] = [{ label: 'Open queue', href: '/queue?x=1' }];
    const api = await mount(API_FIXTURES, { links });
    el.querySelector('button')!.click();
    await render();
    expect(el.textContent).toContain('Reviewing your workload…');
    await until(() => /Triage the unassigned files/.test(el.textContent ?? ''), 'suggestions');
    expect(api.callsTo('/api/workload-suggest')[0].body).toEqual({ scope: 'compliance', stats: STATS, items: ITEMS, links });
    expect(el.textContent).toContain('Urgent');
    expect(el.firstElementChild?.className).toBe('rounded-lg border border-[#013366]/20 bg-card p-4 text-left');
    // The fixture suggestion has no linkLabel, so no link is rendered.
    expect(el.querySelector('a')).toBeNull();
  });

  it('renders a link only when linkLabel matches a link label', async () => {
    await mount(
      {
        '/api/workload-suggest': {
          json: {
            suggestions: [
              { title: 'A', reason: 'r', urgency: 'soon', linkLabel: 'Open queue' },
              { title: 'B', reason: 'r', urgency: 'routine', linkLabel: 'Unknown' },
            ],
          },
        },
      },
      { links: [{ label: 'Open queue', href: '/queue?x=1' }] },
    );
    el.querySelector('button')!.click();
    await until(() => el.querySelectorAll('li').length === 2, 'two suggestions');
    const links = el.querySelectorAll('a');
    expect(links).toHaveLength(1);
    expect(links[0].getAttribute('href')).toBe('/queue?x=1');
    expect(links[0].textContent).toContain('Open queue');
  });

  it('shows the empty message', async () => {
    await mount({ '/api/workload-suggest': { json: { suggestions: [] } } });
    el.querySelector('button')!.click();
    await until(() => /Nothing needs your attention right now\./.test(el.textContent ?? ''), 'empty');
  });

  it('collapses to "Show suggested priorities" and re-expands without a new request', async () => {
    const api = await mount();
    el.querySelector('button')!.click();
    await until(() => !!btn(/Collapse suggestions/), 'done');
    btn(/Collapse suggestions/)!.click();
    await render();
    expect(el.textContent).toContain('AI: Show suggested priorities');
    el.querySelector('button')!.click();
    await render();
    expect(el.textContent).toContain('AI-generated guidance · you make the decision.');
    expect(api.callsTo('/api/workload-suggest')).toHaveLength(1);
  });

  it('shows the error state and Retry recovers', async () => {
    const api = await mount({
      '/api/workload-suggest': { sequence: [{ status: 500, json: { error: 'x' } }, API_FIXTURES['/api/workload-suggest']] },
    });
    el.querySelector('button')!.click();
    await until(() => /Couldn't generate suggestions right now\./.test(el.textContent ?? ''), 'error');
    btn(/^Retry$/)!.click();
    await until(() => /Triage the unassigned files/.test(el.textContent ?? ''), 'recovered');
    expect(api.callsTo('/api/workload-suggest')).toHaveLength(2);
  });

  it('Refresh re-requests', async () => {
    const api = await mount();
    el.querySelector('button')!.click();
    await until(() => !!btn(/Refresh/), 'done');
    btn(/Refresh/)!.click();
    await until(() => api.callsTo('/api/workload-suggest').length === 2 && !!btn(/Refresh/), 'second request');
  });
});
