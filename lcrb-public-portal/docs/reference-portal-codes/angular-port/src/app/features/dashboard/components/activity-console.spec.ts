import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { recordAudit } from '@data-access/repositories/activity/audit-service';
import { API_FIXTURES } from '../../../../../tests/fixtures/api';
import { stubFetch } from '../../../../../tests/support/fetch-stub';
import { ActivityConsole } from './activity-console';

const SESSION = (role: string) =>
  JSON.stringify({ role, firstName: 'Dana', lastName: 'Lee', organization: 'Org', email: 'd@example.com' });

async function mount(role: string) {
  localStorage.setItem('lcrb.mock.session', SESSION(role));
  TestBed.configureTestingModule({ providers: [provideRouter([])] });
  const fixture = TestBed.createComponent(ActivityConsole);
  const el = fixture.nativeElement as HTMLElement;
  const settle = async () => {
    for (let i = 0; i < 4; i++) {
      await new Promise((r) => setTimeout(r, 5));
      fixture.detectChanges();
      await fixture.whenStable();
    }
  };
  await settle();
  return { fixture, el, settle };
}

describe('ActivityConsole', () => {
  beforeEach(() => {
    localStorage.clear();
    sessionStorage.clear();
    recordAudit({ category: 'payment', action: 'paid', summary: 'Paid the fee', actor: 'Dana Lee', subjectId: 'JOB-1' });
  });
  afterEach(() => vi.unstubAllGlobals());

  for (const role of ['police', 'local-government']) {
    it(`renders the console for ${role}`, async () => {
      const { el } = await mount(role);
      expect(el.textContent).toContain('Activity & Integrations');
      expect(el.querySelector('[data-slot="card-title"]')!.className).toContain('flex items-center gap-2 text-[#013366]');
      expect(el.textContent).toMatch(/Audit trail \(\d+\)/);
      expect(el.textContent).toContain('Notifications');
      expect(el.textContent).toContain('Dynamics sync');
      expect(el.textContent).toContain('Paid the fee');
      expect(el.textContent).toContain('Dana Lee · JOB-1');
      const tabs = el.querySelectorAll('[role="tab"]');
      expect(tabs).toHaveLength(3);
      expect(tabs[0].getAttribute('data-state')).toBe('active');
      expect(el.querySelector('ul.divide-y')!.children[0].tagName).toBe('LI');
    });
  }

  it('posts the AI search to /api/activity-search and shows the answer', async () => {
    const api = stubFetch(API_FIXTURES);
    const { fixture, el, settle } = await mount('police');
    const input = el.querySelector('input[aria-label="Ask about activity in natural language"]') as HTMLInputElement;
    const search = [...el.querySelectorAll('button')].find((b) => b.textContent?.trim() === 'Search') as HTMLButtonElement;
    expect(search.disabled).toBe(true);
    input.value = 'payments by Dana Lee';
    input.dispatchEvent(new Event('input'));
    fixture.detectChanges();
    expect(search.disabled).toBe(false);
    search.click();
    await settle();
    const calls = api.callsTo('/api/activity-search');
    expect(calls).toHaveLength(1);
    const body = calls[0].body as { query: string; entries: unknown[]; today: string };
    expect(body.query).toBe('payments by Dana Lee');
    expect(body.entries.length).toBeGreaterThan(0);
    expect(el.textContent).toContain('Searched the audit trail.');
    expect(el.textContent).toContain('0 matching entries shown below.');
    expect(el.textContent).toContain('No activity matched your question.');
    const clear = [...el.querySelectorAll('button')].find((b) => b.textContent?.trim() === 'Clear')!;
    clear.click();
    await settle();
    expect(el.textContent).not.toContain('0 matching entries');
  });

  it('shows the failure text when the AI search fails', async () => {
    stubFetch({ '/api/activity-search': { status: 500, json: {} } });
    const { fixture, el, settle } = await mount('police');
    const input = el.querySelector('input[aria-label="Ask about activity in natural language"]') as HTMLInputElement;
    input.value = 'x';
    input.dispatchEvent(new Event('input'));
    fixture.detectChanges();
    input.dispatchEvent(new KeyboardEvent('keydown', { key: 'Enter' }));
    await settle();
    expect(el.textContent).toContain('Search failed. Try rephrasing your question.');
  });

  it('filters the audit trail by search text', async () => {
    const { fixture, el } = await mount('police');
    const input = el.querySelector('input[aria-label="Search audit trail"]') as HTMLInputElement;
    input.value = 'zzz-no-match';
    input.dispatchEvent(new Event('input'));
    fixture.detectChanges();
    expect(el.textContent).toContain('No activity recorded yet.');
  });
});
