import { type ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { API_FIXTURES } from '../../../../../tests/fixtures/api';
import { stubFetch } from '../../../../../tests/support/fetch-stub';
import { StatusExplainer } from './status-explainer';

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

function make(extra: Record<string, unknown> = {}) {
  const fixture = TestBed.createComponent(StatusExplainer);
  const base = { subjectType: 'application', title: 'Renewal', status: 'under-review', ...extra };
  for (const [k, v] of Object.entries(base)) fixture.componentRef.setInput(k, v);
  return fixture;
}

describe('StatusExplainer', () => {
  beforeEach(() => {
    sessionStorage.clear();
    localStorage.clear();
  });
  afterEach(() => vi.unstubAllGlobals());

  it('renders the idle prompt button', async () => {
    const stub = stubFetch(API_FIXTURES);
    const fixture = make();
    await render(fixture);
    const btn = (fixture.nativeElement as HTMLElement).querySelector('button')!;
    expect(btn.textContent).toContain('What does this status mean?');
    expect(btn.className).toBe(
      'flex w-full items-center justify-between gap-2 rounded-lg border border-[#013366]/20 bg-[#013366]/5 px-4 py-3 text-left transition-colors hover:bg-[#013366]/10',
    );
    expect(stub.calls).toHaveLength(0);
  });

  it('posts the props and renders the explanation (no action needed)', async () => {
    const stub = stubFetch(API_FIXTURES);
    const fixture = make({ statusLabel: 'Under review', history: [{ action: 'Submitted' }], extra: 'note' });
    await render(fixture);
    const el = fixture.nativeElement as HTMLElement;
    el.querySelector('button')!.click();
    await until(fixture, () => el.textContent!.includes('Your application is being reviewed.'), 'result');
    expect(stub.callsTo('/api/status-explainer')[0].body).toEqual({
      subjectType: 'application',
      title: 'Renewal',
      status: 'under-review',
      statusLabel: 'Under review',
      history: [{ action: 'Submitted' }],
      extra: 'note',
    });
    expect(el.textContent).toContain('What this means');
    expect(el.textContent).toContain('No action needed from you right now.');
    expect(el.textContent).toContain('A reviewer checks your documents.');
    expect(el.querySelectorAll('li')).toHaveLength(2);
    expect(el.querySelector('div.rounded-lg')!.className).toBe('rounded-lg border border-[#013366]/20 bg-card p-4');
  });

  it('shows the action-required banner', async () => {
    stubFetch({ '/api/status-explainer': { json: { meaning: 'm', whatsNext: [], actionRequired: true } } });
    const fixture = make();
    await render(fixture);
    const el = fixture.nativeElement as HTMLElement;
    el.querySelector('button')!.click();
    await until(fixture, () => el.textContent!.includes('This needs an action from you'), 'action');
    expect(el.textContent).not.toContain('What happens next');
  });

  it('shows the loading state', async () => {
    let release!: () => void;
    vi.stubGlobal(
      'fetch',
      vi.fn(() => new Promise<Response>((r) => (release = () => r(new Response('{}', { status: 500 }))))),
    );
    const fixture = make();
    await render(fixture);
    const el = fixture.nativeElement as HTMLElement;
    el.querySelector('button')!.click();
    await until(fixture, () => el.textContent!.includes('Reading your status and history…'), 'loading');
    release();
  });

  it('shows the friendly error and retries', async () => {
    const stub = stubFetch({
      '/api/status-explainer': { sequence: [{ status: 500, json: { error: 'x' } }, API_FIXTURES['/api/status-explainer']] },
    });
    const fixture = make();
    await render(fixture);
    const el = fixture.nativeElement as HTMLElement;
    el.querySelector('button')!.click();
    await until(fixture, () => el.textContent!.includes('Please try again in a moment.'), 'error');
    byText(el, 'button', 'Retry')!.click();
    await until(fixture, () => el.textContent!.includes('Your application is being reviewed.'), 'retry');
    expect(stub.callsTo('/api/status-explainer')).toHaveLength(2);
  });
});
