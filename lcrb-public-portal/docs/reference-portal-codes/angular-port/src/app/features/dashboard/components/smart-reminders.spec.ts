import { type ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { API_FIXTURES } from '../../../../../tests/fixtures/api';
import { stubFetch } from '../../../../../tests/support/fetch-stub';
import { SmartReminders, type ReminderContextItem } from './smart-reminders';

const ITEMS: ReminderContextItem[] = [
  { kind: 'application', label: 'Name change', detail: 'Awaiting documents', href: '/applications/1' },
  { kind: 'notices', label: 'Notices', detail: '3 unread', href: '/notices' },
  { kind: 'application', label: 'Transfer', detail: 'Submitted', href: '/applications/2' },
];

let fixture: ComponentFixture<SmartReminders>;
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
async function mount(items: ReminderContextItem[]) {
  TestBed.configureTestingModule({ providers: [provideRouter([])] });
  fixture = TestBed.createComponent(SmartReminders);
  fixture.componentRef.setInput('items', items);
  el = fixture.nativeElement;
  await render();
}
const rewrite = () => [...el.querySelectorAll('button')].find((b) => /Rewrite/.test(b.textContent ?? ''));

describe('SmartReminders', () => {
  beforeEach(() => {
    sessionStorage.clear();
    localStorage.clear();
  });
  afterEach(() => vi.unstubAllGlobals());

  it('renders deterministic reminders on first render without calling the API', async () => {
    const api = stubFetch(API_FIXTURES);
    await mount(ITEMS);
    const text = el.textContent ?? '';
    expect(text).toContain('Smart reminders');
    expect(text).toContain('Action needed: Name change');
    expect(text).toContain('You have 3 unread notices');
    expect(text).toContain('Transfer in progress');
    expect(rewrite()).toBeTruthy();
    expect(el.textContent).not.toContain('AI\n');
    expect(api.callsTo('/api/nudges')).toHaveLength(0);
  });

  it('orders high before medium before low and links the CTA', async () => {
    stubFetch(API_FIXTURES);
    await mount(ITEMS);
    const chips = [...el.querySelectorAll('span.rounded-full')].map((s) => s.textContent?.trim());
    expect(chips.filter((c) => ['high', 'medium', 'low'].includes(c ?? ''))).toEqual(['high', 'medium', 'low']);
    const link = el.querySelector('a[href="/applications/1"]') as HTMLAnchorElement;
    expect(link.textContent).toContain('Complete now');
    expect(link.className).toBe('mt-2 inline-flex items-center gap-1 text-sm font-medium text-[#013366] hover:underline');
  });

  it('shows the all caught up state with no reminders', async () => {
    stubFetch(API_FIXTURES);
    await mount([]);
    expect(el.textContent).toContain("You're all caught up");
    expect(el.firstElementChild?.className).toBe(
      'flex items-center gap-4 rounded-lg border border-green-300 bg-green-50 p-5 text-left',
    );
  });

  it('Rewrite with AI posts items and today, then shows the AI list and badge', async () => {
    const api = stubFetch(API_FIXTURES);
    await mount(ITEMS);
    expect(el.firstElementChild?.className).toBe('space-y-3 text-left');
    rewrite()!.click();
    await until(() => /Renew your licence/.test(el.textContent ?? ''), 'refined nudges');
    const body = api.callsTo('/api/nudges')[0].body as { items: unknown; today: string };
    expect(body.items).toEqual(ITEMS);
    expect(body.today).toBe(new Date().toISOString().slice(0, 10));
    expect(rewrite()).toBeUndefined();
    expect(el.textContent).toContain('AI');
    expect(el.querySelector('a[href="/licences"]')).toBeTruthy();
  });

  it('shows the loading label while rewriting', async () => {
    stubFetch({ '/api/nudges': { ...API_FIXTURES['/api/nudges'] } });
    await mount(ITEMS);
    rewrite()!.click();
    await render();
    expect(el.textContent).toContain('Rewriting…');
    expect((el.querySelector('button[disabled]') as HTMLButtonElement | null)?.className).toContain('disabled:opacity-70');
    await until(() => !/Rewriting…/.test(el.textContent ?? ''), 'rewrite done');
  });

  it('keeps the deterministic list when the request fails', async () => {
    stubFetch({ '/api/nudges': { status: 500, json: { error: 'boom' } } });
    await mount(ITEMS);
    rewrite()!.click();
    await until(() => !/Rewriting…/.test(el.textContent ?? '') && !!rewrite(), 'failure settled');
    expect(el.textContent).toContain('Action needed: Name change');
  });

  it('serves a second mount from the session cache without a request', async () => {
    const api = stubFetch(API_FIXTURES);
    await mount(ITEMS);
    rewrite()!.click();
    await until(() => /Renew your licence/.test(el.textContent ?? ''), 'refined nudges');
    expect(api.callsTo('/api/nudges')).toHaveLength(1);
    TestBed.resetTestingModule();
    await mount(ITEMS);
    await until(() => /Renew your licence/.test(el.textContent ?? ''), 'cached nudges');
    expect(rewrite()).toBeUndefined();
    expect(api.callsTo('/api/nudges')).toHaveLength(1);
  });
});
