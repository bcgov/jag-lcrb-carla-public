/** Provider services: prototype-equivalent behaviour per method, same storage keys, side effects (audit, notify) and the loading-then-settle rule. */
import { Component } from '@angular/core';
import { TestBed } from '@angular/core/testing';
import { listAudit } from '@data-access/repositories/activity/audit-service';
import { listNotifications } from '@data-access/repositories/notifications/notification-service';
import { A11yProvider } from '@core/a11y/a11y-provider';
import { generateId } from 'ai';
import { AssistantProvider, generateChatId } from '@core/assistant/assistant-provider';
import { LgCommentingProvider } from '@data-access/repositories/lg-review/lg-commenting-provider';
import { LgStoreProvider } from '@data-access/repositories/lg-review/lg-store-provider';
import { SepStoreProvider } from '@data-access/repositories/special-events/sep-store-provider';
import { type Session, SessionProvider } from '@core/session/session-provider';

/** Creates and renders a host so afterNextRender hooks run. */
async function render() {
  @Component({ template: '' })
  class Host {}
  const fixture = TestBed.createComponent(Host);
  fixture.detectChanges();
  await fixture.whenStable();
}

const session = { userId: 'u1', name: 'Pat', email: 'p@x.ca', role: 'licensee' } as unknown as Session;

beforeEach(() => {
  localStorage.clear();
  sessionStorage.clear();
  document.documentElement.removeAttribute('style');
  document.documentElement.classList.remove('high-contrast');
});

describe('SessionProvider', () => {
  it('is loading with no session on first render, then reads lcrb.mock.session', async () => {
    localStorage.setItem('lcrb.mock.session', JSON.stringify(session));
    const s = TestBed.inject(SessionProvider);
    expect(s.loading()).toBe(true);
    expect(s.session()).toBeNull();
    await render();
    expect(s.loading()).toBe(false);
    expect(s.session()).toEqual(session);
  });

  it('login persists and logout clears', async () => {
    const s = TestBed.inject(SessionProvider);
    await render();
    s.login(session);
    expect(s.session()).toEqual(session);
    expect(JSON.parse(localStorage.getItem('lcrb.mock.session')!)).toEqual(session);
    s.logout();
    expect(s.session()).toBeNull();
    expect(localStorage.getItem('lcrb.mock.session')).toBeNull();
  });

  it('ignores malformed storage', async () => {
    localStorage.setItem('lcrb.mock.session', '{bad');
    const s = TestBed.inject(SessionProvider);
    await render();
    expect(s.session()).toBeNull();
    expect(s.loading()).toBe(false);
  });
});

describe('A11yProvider', () => {
  it('loads lcrb.a11y.prefs, applies font size and high-contrast class, persists changes', async () => {
    localStorage.setItem('lcrb.a11y.prefs', JSON.stringify({ textScale: 'large', highContrast: true }));
    const a = TestBed.inject(A11yProvider);
    await render();
    expect(a.textScale()).toBe('large');
    expect(document.documentElement.classList.contains('high-contrast')).toBe(true);
    a.resetText();
    a.toggleContrast();
    TestBed.tick();
    expect(document.documentElement.style.fontSize).toBe('100%');
    expect(document.documentElement.classList.contains('high-contrast')).toBe(false);
    expect(JSON.parse(localStorage.getItem('lcrb.a11y.prefs')!)).toEqual({ textScale: 'normal', highContrast: false });
  });

  it('clamps text scale at both ends', async () => {
    const a = TestBed.inject(A11yProvider);
    await render();
    a.decreaseText();
    expect(a.textScale()).toBe('normal');
    for (let i = 0; i < 10; i++) a.increaseText();
    const top = a.textScale();
    a.increaseText();
    expect(a.textScale()).toBe(top);
    expect(top).not.toBe('normal');
  });
});

describe('LgCommentingProvider', () => {
  it('is not loaded on first render, then reads lcrb.lg.commenting', async () => {
    localStorage.setItem('lcrb.lg.commenting', JSON.stringify({ optedOut: true }));
    const p = TestBed.inject(LgCommentingProvider);
    expect(p.loaded()).toBe(false);
    expect(p.preference().optedOut).toBe(false);
    await render();
    expect(p.loaded()).toBe(true);
    expect(p.preference().optedOut).toBe(true);
  });

  it('setOptedOut persists, audits and notifies the branch', async () => {
    const p = TestBed.inject(LgCommentingProvider);
    await render();
    p.setOptedOut(true, 'Clerk', 'Victoria');
    expect(p.preference()).toMatchObject({ optedOut: true, updatedBy: 'Clerk' });
    expect(p.preference().effectiveFrom).toMatch(/^\d{4}-\d{2}-\d{2}$/);
    expect(JSON.parse(localStorage.getItem('lcrb.lg.commenting')!).optedOut).toBe(true);
    expect(listAudit().some((e) => e.action === 'lg.commenting.opted-out' && e.subjectId === 'Victoria')).toBe(true);
    expect(listNotifications('lcrb').some((n) => n.title === 'Local government opted out of commenting')).toBe(true);
    p.setOptedOut(false, 'Clerk', 'Victoria');
    expect(p.preference().effectiveFrom).toBeNull();
  });
});

describe('AssistantProvider', () => {
  it('toggles, asks with a pending prompt, and clears a highlight after a delay', () => {
    vi.useFakeTimers();
    try {
      const a = TestBed.inject(AssistantProvider);
      expect(a.open()).toBe(false);
      a.toggle();
      expect(a.open()).toBe(true);
      a.setOpen(false);
      a.askAssistant('What is a licence?');
      expect(a.open()).toBe(true);
      expect(a.pendingPrompt()).toBe('What is a licence?');
      a.highlightField('name');
      expect(a.highlightedField()).toBe('name');
      vi.runAllTimers();
      expect(a.highlightedField()).toBeNull();
    } finally {
      vi.useRealTimers();
    }
  });

  it('draws the chat id once, with the AI SDK generateId algorithm', () => {
    const seq = () => {
      let i = 0;
      return () => ((i++ * 0.37) % 1);
    };
    const spy = vi.spyOn(Math, 'random').mockImplementation(seq());
    const fromSdk = generateId();
    spy.mockImplementation(seq());
    expect(generateChatId()).toBe(fromSdk);
    spy.mockClear();
    const a = TestBed.inject(AssistantProvider);
    expect(a.chatId).toBeNull();
    a.ensureChatId();
    const id = a.chatId;
    expect(id).toMatch(/^[0-9A-Za-z]{16}$/);
    a.ensureChatId();
    expect(a.chatId).toBe(id);
    expect(spy).toHaveBeenCalledTimes(16);
    spy.mockRestore();
  });
});

describe('SepStoreProvider', () => {
  it('starts from the seed, assigns an officer and persists to lcrb.sep.records', async () => {
    const s = TestBed.inject(SepStoreProvider);
    await render();
    const first = s.records()[0];
    s.assignOfficer(first.id, 'Cst. Lee');
    TestBed.tick();
    const updated = s.getById(first.id)!;
    expect(updated.assignedOfficer).toBe('Cst. Lee');
    expect(updated.history.at(-1)?.action).toBe('Assigned to self');
    const stored = JSON.parse(localStorage.getItem('lcrb.sep.records')!) as { id: string; assignedOfficer?: string }[];
    expect(stored.find((r) => r.id === first.id)?.assignedOfficer).toBe('Cst. Lee');
  });

  it('hydrates stored records over the seed after first render', async () => {
    const s0 = TestBed.inject(SepStoreProvider);
    await render();
    const id = s0.records()[0].id;
    s0.setConditions(id, [{ id: 'c1', text: 'Kept', standard: false }]);
    TestBed.tick();
    TestBed.resetTestingModule();
    const s = TestBed.inject(SepStoreProvider);
    expect(s.getById(id)!.conditions).not.toEqual([{ id: 'c1', text: 'Kept', standard: false }]);
    await render();
    expect(s.getById(id)!.conditions).toEqual([{ id: 'c1', text: 'Kept', standard: false }]);
  });
});

describe('LgStoreProvider', () => {
  it('records a decision with history, audit and storage under lcrb.lg.applications', async () => {
    const s = TestBed.inject(LgStoreProvider);
    await render();
    const open = s.applications().find((a) => a.status !== 'cancelled')!;
    s.recordDecision(open.jobNumber, 'no-comment', 'Fine', 'Clerk');
    TestBed.tick();
    const after = s.getByJob(open.jobNumber)!;
    expect(after.status).toBe('no-comment');
    expect(after.history.at(-1)?.label).toContain('Fine');
    expect(listAudit().some((e) => e.subjectId === open.jobNumber)).toBe(true);
    const stored = JSON.parse(localStorage.getItem('lcrb.lg.applications')!) as { jobNumber: string; status: string }[];
    expect(stored.find((a) => a.jobNumber === open.jobNumber)?.status).toBe('no-comment');
  });

  it('simulateIncomingReferral adds a file', async () => {
    const s = TestBed.inject(LgStoreProvider);
    await render();
    const before = s.applications().length;
    s.simulateIncomingReferral('Clerk');
    expect(s.applications().length).toBe(before + 1);
  });
});
