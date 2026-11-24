/** Licensee dashboard and its components (start-here, drafts-panel, inline AI assistant); screenshots and the assistant-open state run in the visual suite. */
import { Component, type Type } from '@angular/core';
import { type ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { AIAssistant } from '../components/ai-assistant';
import { DraftsPanel } from '../components/drafts-panel';
import { StartHere } from '../components/start-here';
import { SessionProvider } from '@core/session/session-provider';
import { type ApplicationDraft, saveDraft } from '@data-access/repositories/applications/drafts';
import { NOTICES } from '@data-access/repositories/notifications/mock/notices';
import { stubFetch } from '../../../../../tests/support/fetch-stub';
import { DashboardPage, RenewalsBox } from './dashboard.page';

@Component({ template: '' })
class Blank {}

async function mount<T>(type: Type<T>): Promise<{ fixture: ComponentFixture<T>; el: HTMLElement }> {
  const fixture = TestBed.createComponent(type);
  fixture.detectChanges();
  await fixture.whenStable();
  fixture.detectChanges();
  await fixture.whenStable();
  return { fixture, el: fixture.nativeElement as HTMLElement };
}

const draft = (n: number, extra: Partial<ApplicationDraft> = {}): ApplicationDraft => ({
  draftId: `d${n}`,
  flowId: 'sep',
  title: `Draft ${n}`,
  resumeHref: `/special-events/apply?draft=d${n}`,
  formData: {},
  currentStep: 1,
  progress: 40.4,
  updatedAt: '',
  ...extra,
});

beforeEach(() => {
  localStorage.clear();
  sessionStorage.clear();
  TestBed.configureTestingModule({ providers: [provideRouter([{ path: '**', component: Blank }])] });
});
afterEach(() => {
  vi.useRealTimers();
  vi.unstubAllGlobals();
});

describe('DashboardPage', () => {
  it('renders the hero, quick actions, stats and recent activity with the prototype links', async () => {
    stubFetch({});
    TestBed.inject(SessionProvider).login({ role: 'licensee', firstName: 'Matt', lastName: 'Harding', organization: 'X' } as never);
    const { el } = await mount(DashboardPage);
    expect(el.querySelector('h1')?.textContent).toContain('Welcome to the LCRB Portal');
    expect(el.textContent).toContain('What would you like to do today?');
    const quick = [...el.querySelectorAll('quick-action-card a')].map((a) => a.getAttribute('href'));
    expect(quick).toEqual(['/applications/new', '/establishment-planner', '/licence-finder', '/applications']);

    const stats = [...el.querySelectorAll('dashboard-stat-card a')];
    expect(stats.map((a) => a.getAttribute('href'))).toEqual([
      '/licences',
      '/applications?status=pending',
      '/licences?filter=renewal',
      '/notices',
    ]);
    const unread = NOTICES.filter((n) => !n.read).length;
    expect(stats[3].textContent).toContain(String(unread));
    expect(stats[1].getAttribute('class')).toBe(
      'block p-6 rounded-lg border transition-colors hover:border-[#013366] bg-[#FEF1D8] border-[#F5A623]',
    );
    expect(stats[0].getAttribute('class')).toBe('block p-6 rounded-lg border transition-colors hover:border-[#013366] bg-white');

    const activity = [...el.querySelectorAll('a[dashboardActivityItem]')];
    expect(activity.map((a) => a.getAttribute('href'))).toEqual(['/applications/submitted/LP-125998', '/licences/326739']);
    expect(activity[0].querySelector('span')?.getAttribute('class')).toBe(
      'text-xs font-medium px-2.5 py-1 rounded-full bg-blue-100 text-blue-800',
    );

    // Smart reminders only read the cache on mount: no AI request.
    expect(el.querySelector('smart-reminders')).toBeTruthy();
    expect((globalThis.fetch as ReturnType<typeof vi.fn>).mock.calls).toHaveLength(0);
  });
});

describe('RenewalsBox', () => {
  async function renewals(now: string, licences: { expiryDate: string }[]) {
    vi.useFakeTimers({ toFake: ['Date'] });
    vi.setSystemTime(new Date(now));
    const fixture = TestBed.createComponent(RenewalsBox);
    fixture.componentRef.setInput('licences', licences);
    await fixture.whenStable();
    return fixture.nativeElement.querySelector('a') as HTMLAnchorElement;
  }

  it('shows expired plus due-soon on the visual-suite clock', async () => {
    const a = await renewals('2026-11-02T10:00:00-08:00', [
      { expiryDate: '2026-06-20' },
      { expiryDate: '2026-12-01' },
      { expiryDate: '2027-03-15' },
    ]);
    expect(a.getAttribute('href')).toBe('/licences?filter=renewal');
    expect(a.getAttribute('class')).toBe(
      'flex items-center gap-4 text-left p-5 rounded-lg border transition-shadow hover:shadow-md bg-red-50 border-red-300',
    );
    expect(a.textContent).toContain('1 licence has expired');
    expect(a.textContent).toContain('Renew immediately. 1 more is due within 30 days.');
    expect(a.textContent).toContain('2');
  });

  it('shows due soon and all-clear states', async () => {
    expect((await renewals('2026-11-02', [{ expiryDate: '2026-11-20' }, { expiryDate: '2026-11-25' }])).textContent).toContain(
      '2 licences up for renewal',
    );
    const ok = await renewals('2026-11-02', [{ expiryDate: '2027-11-20' }]);
    expect(ok.textContent).toContain('No renewals needed');
    expect(ok.querySelectorAll('div.text-3xl')).toHaveLength(0);
  });
});

describe('StartHere', () => {
  it('shows after the first render, links the three steps and dismisses once', async () => {
    const { fixture, el } = await mount(StartHere);
    expect(el.querySelector('aside')?.getAttribute('aria-label')).toBe('New here? Start with the Planner');
    const links = [...el.querySelectorAll('ol a')].map((a) => [a.textContent?.trim(), a.getAttribute('href')]);
    expect(links).toEqual([
      ['Plan your establishment', '/establishment-planner'],
      ['Find the right licence type', '/licence-finder'],
      ['Start the application', '/applications/new'],
    ]);
    const action = el.querySelector('a[data-slot="button"]') as HTMLAnchorElement;
    expect(action.getAttribute('href')).toBe('/establishment-planner');
    expect(action.className).toContain('bg-[#013366] text-white hover:bg-[#013366]/90');
    (el.querySelector('button[aria-label="Dismiss: New here? Start with the Planner"]') as HTMLButtonElement).click();
    fixture.detectChanges();
    expect(el.querySelector('aside')).toBeNull();
    expect(localStorage.length).toBe(1);
  });
});

describe('DraftsPanel', () => {
  it('renders nothing without drafts', async () => {
    const { el } = await mount(DraftsPanel);
    expect(el.querySelector('section')).toBeNull();
  });

  it('lists drafts with resume links, shows 3 then expands, and deletes', async () => {
    for (let i = 1; i <= 4; i++) saveDraft(draft(i, i === 1 ? { formData: { eventName: ' Summer Fair ' } } : {}));
    const { fixture, el } = await mount(DraftsPanel);
    expect(el.querySelector('#drafts-heading')?.textContent).toBe('Pick up where you left off');
    expect(el.textContent).toContain('4 drafts');
    expect(el.querySelectorAll('#drafts-list li')).toHaveLength(3);

    const toggle = [...el.querySelectorAll('button')].find((b) => b.textContent?.includes('Show 1 more draft'))!;
    expect(toggle.getAttribute('aria-expanded')).toBe('false');
    toggle.click();
    fixture.detectChanges();
    expect(el.querySelectorAll('#drafts-list li')).toHaveLength(4);
    expect(el.textContent).toContain('Show fewer');

    // A draft with a captured name shows it as the subtitle and the save date below.
    const named = [...el.querySelectorAll('#drafts-list li')].find((li) => li.textContent?.includes('Summer Fair'))!;
    expect(named.textContent).toContain('Saved today');
    expect(named.textContent).toContain('40%');
    expect(named.querySelector('a')?.getAttribute('href')).toBe('/special-events/apply?draft=d1');

    (el.querySelector('button[aria-label="Delete draft: Draft 2"]') as HTMLButtonElement).click();
    await fixture.whenStable();
    fixture.detectChanges();
    expect(el.textContent).toContain('3 drafts');
  });
});

describe('AIAssistant (inline)', () => {
  it('renders the prompt box and clears it on submit without showing messages', async () => {
    vi.useFakeTimers();
    const fixture = TestBed.createComponent(AIAssistant);
    fixture.detectChanges();
    const el = fixture.nativeElement as HTMLElement;
    const input = el.querySelector('input') as HTMLInputElement;
    expect(input.placeholder).toBe('Enter a prompt here');
    expect(input.className).toContain('h-14 pl-4 pr-24 text-base rounded-lg border-2 border-muted');
    expect([...el.querySelectorAll('.sr-only')].map((s) => s.textContent)).toEqual(['AI suggestions', 'Voice input']);
    input.value = 'renew';
    input.dispatchEvent(new Event('input'));
    fixture.detectChanges();
    el.querySelector('form')!.dispatchEvent(new Event('submit', { cancelable: true }));
    fixture.detectChanges();
    expect(input.value).toBe('');
    vi.advanceTimersByTime(1000);
    fixture.detectChanges();
    expect(el.textContent).not.toContain('I can help you with');
  });
});
