import { TestBed } from '@angular/core/testing';
import { ActivatedRoute, convertToParamMap, provideRouter, Router } from '@angular/router';
import { BehaviorSubject } from 'rxjs';
import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { SessionProvider } from '@core/session/session-provider';
import { MY_APPLICATIONS } from '@data-access/repositories/applications/mock/my-applications';
import { listMessages, seedChatIfEmpty } from '@data-access/repositories/messaging/chat-service';
import { stubFetch } from '../../../../../tests/support/fetch-stub';
import { LcrbConversationsPage } from './lcrb-conversations.page';

const settle = async (f: { detectChanges(): void; whenStable(): Promise<unknown> }) => {
  for (let i = 0; i < 4; i++) {
    f.detectChanges();
    await f.whenStable();
    await new Promise((r) => setTimeout(r, 0));
  }
  f.detectChanges();
};

async function mount(query: Record<string, string> = {}, role: 'lcrb' | 'licensee' = 'lcrb') {
  const params = new BehaviorSubject(convertToParamMap(query));
  TestBed.configureTestingModule({
    providers: [
      provideRouter([]),
      { provide: ActivatedRoute, useValue: { snapshot: { queryParamMap: params.value }, queryParamMap: params.asObservable() } },
    ],
  });
  TestBed.inject(SessionProvider).login({ role, firstName: 'Sam', lastName: 'Rowe', organization: 'LCRB' } as never);
  const navigate = vi.spyOn(TestBed.inject(Router), 'navigateByUrl').mockResolvedValue(true);
  const fixture = TestBed.createComponent(LcrbConversationsPage);
  document.body.appendChild(fixture.nativeElement);
  await settle(fixture);
  return { fixture, el: fixture.nativeElement as HTMLElement, navigate, params };
}

const items = (el: HTMLElement) => [...el.querySelectorAll('ul[aria-label="Conversations"] > li > button')] as HTMLButtonElement[];

describe('LcrbConversationsPage', () => {
  beforeEach(() => {
    localStorage.clear();
    sessionStorage.clear();
    stubFetch({});
  });
  afterEach(() => {
    vi.unstubAllGlobals();
    document.body.innerHTML = '';
  });

  it('renders the hero, the kind tabs and a row for every submitted licence file and in-process SEP', async () => {
    const { el } = await mount();
    expect(el.querySelector('h1')?.textContent).toContain('Case conversations');
    expect(el.textContent).toContain('Live threads with applicants and partner agencies');
    expect([...el.querySelectorAll('[role="tab"]')].map((t) => t.textContent!.trim())).toEqual(['All', 'Licence applications', 'Special Event Permits']);
    expect(el.querySelector('[role="tab"][aria-selected="true"]')?.textContent).toContain('All');
    const rows = items(el);
    expect(rows.length).toBeGreaterThanOrEqual(MY_APPLICATIONS.submitted.length);
    for (const a of MY_APPLICATIONS.submitted) expect(el.textContent).toContain(`${a.id} · ${a.reference}`);
    expect(el.textContent).toContain('LCRB only');
    expect(el.textContent).toContain('Select a file to open its conversation');
    expect(el.querySelector('case-thread')).toBeNull();
  });

  it('seeds the chat once after the first render and floats unread threads to the top with a badge', async () => {
    const { el } = await mount();
    expect(listMessages(MY_APPLICATIONS.submitted[0].id).length).toBeGreaterThan(0);
    const unreadBadges = [...el.querySelectorAll('ul[aria-label="Conversations"] [data-slot="badge"].bg-bc-gold')];
    const heroBadge = el.querySelector('h1')?.parentElement?.querySelector('[data-slot="badge"]');
    if (unreadBadges.length > 0) {
      expect(heroBadge?.textContent).toMatch(/\d+ unread/);
      const first = items(el)[0];
      expect(first.querySelector('[data-slot="badge"].bg-bc-gold')).toBeTruthy();
    } else {
      expect(heroBadge).toBeFalsy();
    }
  });

  it('opens the thread of the ?thread= selection and marks the row current', async () => {
    seedChatIfEmpty();
    const id = MY_APPLICATIONS.submitted[0].id;
    const { el } = await mount({ thread: id });
    const row = items(el).find((b) => b.textContent!.includes(id))!;
    expect(row.getAttribute('aria-current')).toBe('true');
    expect(row.classList.contains('border-[#013366]')).toBe(true);
    const thread = el.querySelector('case-thread');
    expect(thread).toBeTruthy();
    expect(el.textContent).toContain(`${MY_APPLICATIONS.submitted[0].type} — ${id}`);
    expect(el.textContent).not.toContain('Select a file to open its conversation');
  });

  it('selecting a row and a tab replace the URL with the thread and kind parameters', async () => {
    const { fixture, el, navigate } = await mount({ kind: 'application' });
    expect(el.querySelector('[role="tab"][aria-selected="true"]')?.textContent).toContain('Licence applications');
    expect(items(el).every((b) => b.textContent!.includes('LG/IN'))).toBe(true);
    items(el)[0].click();
    await settle(fixture);
    expect(navigate).toHaveBeenCalledWith(expect.stringMatching(/^\/lcrb\/conversations\?kind=application&thread=/), { replaceUrl: true });

    const sepTab = [...el.querySelectorAll<HTMLButtonElement>('[role="tab"]')].find((t) => t.textContent!.includes('Special Event Permits'))!;
    sepTab.click();
    await settle(fixture);
    expect(navigate).toHaveBeenLastCalledWith('/lcrb/conversations?kind=sep', { replaceUrl: true });
    const allTab = [...el.querySelectorAll<HTMLButtonElement>('[role="tab"]')].find((t) => t.textContent!.trim() === 'All')!;
    allTab.click();
    await settle(fixture);
    expect(navigate).toHaveBeenLastCalledWith('/lcrb/conversations?', { replaceUrl: true });
  });

  it('is gated for other roles', async () => {
    const { el } = await mount({}, 'licensee');
    expect(el.textContent).toContain('LCRB staff access only');
    expect(el.querySelector('lcrb-conversations-content')).toBeNull();
  });
});
