/** Behaviour of the converted lib hook files: the same storage gives the same values, store changes update the signal */
import { Component } from '@angular/core';
import { TestBed } from '@angular/core/testing';
import { injectExternalStore, injectHydrated } from '@shared/util/external-store';
import { injectDrafts, saveDraft, type ApplicationDraft } from '@data-access/repositories/applications/drafts';
import { injectUnreadAnnouncements } from '@data-access/repositories/notifications/mock/announcements';
import { ANNOUNCEMENTS } from '@data-access/repositories/notifications/mock/announcements';
import { injectSubmission } from '@data-access/repositories/applications/submissions';
import { injectBusinessProfile } from '@core/session/business-profile';
import { injectApplicationMode } from '@shared/forms/application-mode';
import { injectNotifications } from '@data-access/repositories/notifications/notifications.signals';
import { notify } from '@data-access/repositories/notifications/notification-service';
import { injectInboxFeed } from '@data-access/repositories/notifications/inbox';
import { injectEnforcementActions } from '@data-access/repositories/compliance/enforcement-store';
import { injectScreeningPeople } from '@features/legal-entity/state/screening-store';
import { injectReceiptsReady } from '@data-access/repositories/payments/payments.signals';
import { injectLegalEntityBuild } from '@features/legal-entity/state/legal-entity-store';
import { injectAiAutoRetry } from '@data-access/repositories/ai/use-ai-auto-retry';
import { createPlan, injectEstablishmentPlan, injectEstablishmentPlans, renamePlan } from '@features/public-tools/state/establishment-plan-store';
import { injectTiedHouseApplication } from '@features/legal-entity/state/tied-house-store';
import { injectTiedHouseOverlay } from '@features/legal-entity/state/legal-entity-store';
import { injectNoticesFeed, markNoticeRead } from '@data-access/repositories/notifications/notices-store';
import { injectNoticeChime } from '@core/notifications/notice-chime';
import { SessionProvider } from '@core/session/session-provider';
import { signal } from '@angular/core';
import { vi } from 'vitest';

/** Run `setup` inside a component constructor; returns its result, the first-render snapshot and the fixture. */
async function inHost<T>(setup: () => T) {
  let result!: T;
  @Component({ template: '' })
  class Host {
    constructor() {
      result = setup();
    }
  }
  const fixture = TestBed.createComponent(Host);
  return { result, fixture, render: async () => {
    fixture.detectChanges();
    await fixture.whenStable();
  } };
}

const draft = (id: string, flowId: string, updatedAt: string): ApplicationDraft =>
  ({ draftId: id, flowId, title: 'T', resumeHref: '/x', formData: {}, currentStep: 0, progress: 0, updatedAt }) as unknown as ApplicationDraft;

beforeEach(() => {
  localStorage.clear();
  sessionStorage.clear();
});

describe('external store adapter', () => {
  it('follows the store and starts with the server snapshot when given', async () => {
    let value = 'a';
    const listeners = new Set<() => void>();
    const { result, render } = await inHost(() =>
      injectExternalStore((cb) => (listeners.add(cb), () => listeners.delete(cb)), () => value, () => 'server'),
    );
    expect(result()).toBe('server');
    await render();
    expect(result()).toBe('a');
    value = 'b';
    listeners.forEach((l) => l());
    expect(result()).toBe('b');
  });

  it('injectHydrated is false on first render, then true, running onHydrate once', async () => {
    let calls = 0;
    const { result, render } = await inHost(() => injectHydrated(() => calls++));
    expect(result()).toBe(false);
    await render();
    expect(result()).toBe(true);
    expect(calls).toBe(1);
  });
});

describe('converted hooks', () => {
  it('injectDrafts lists, filters and removes drafts', async () => {
    saveDraft(draft('d1', 'sep', '2026-01-01'));
    saveDraft(draft('d2', 'fp-name-change', '2026-01-02'));
    const { result, render } = await inHost(() => ({ all: injectDrafts(), sep: injectDrafts('sep') }));
    expect(result.all().drafts).toEqual([]); // server snapshot "{}" on first render
    await render();
    expect(result.all().drafts.map((d) => d.draftId)).toEqual(['d2', 'd1']);
    expect(result.sep().drafts.map((d) => d.draftId)).toEqual(['d1']);
    result.all().remove('d1');
    expect(result.all().drafts.map((d) => d.draftId)).toEqual(['d2']);
  });

  it('injectUnreadAnnouncements counts all as unread until marked seen', async () => {
    const { result, render } = await inHost(() => injectUnreadAnnouncements());
    await render();
    expect(result().count).toBe(ANNOUNCEMENTS.length);
    result().markAllSeen();
    expect(result().count).toBe(0);
  });

  it('injectSubmission reads the record for a changing id', async () => {
    localStorage.setItem('lcrb.app.submissions', JSON.stringify({ a: { id: 'a', title: 'A' }, b: { id: 'b', title: 'B' } }));
    const id = signal('a');
    const { result, render } = await inHost(() => injectSubmission(id));
    await render();
    expect(result()?.title).toBe('A');
    id.set('b');
    expect(result()?.title).toBe('B');
  });

  it('injectApplicationMode starts as form, reads storage after render, and persists changes', async () => {
    localStorage.setItem('lcrb.app.mode', 'guided');
    const { result, render } = await inHost(() => injectApplicationMode());
    expect(result.mode()).toBe('form');
    expect(result.hydrated()).toBe(false);
    await render();
    expect(result.mode()).toBe('guided');
    result.setMode('form');
    expect(localStorage.getItem('lcrb.app.mode')).toBe('form');
  });

  it('injectNotifications updates when a notification is sent', async () => {
    const { result, render } = await inHost(() => injectNotifications('licensee'));
    await render();
    expect(result().unread).toBe(0);
    notify({ recipient: 'licensee', category: 'system', title: 'Hello', body: 'World' });
    expect(result().unread).toBe(1);
    expect(result().notifications[0].title).toBe('Hello');
  });

  it('injectInboxFeed includes notices for licensees only', async () => {
    const { result, render } = await inHost(() => ({ licensee: injectInboxFeed('licensee'), police: injectInboxFeed('police') }));
    await render();
    expect(result.licensee().showNotices).toBe(true);
    expect(result.licensee().items.some((i) => i.source === 'notice')).toBe(true);
    expect(result.police().showNotices).toBe(false);
    expect(result.police().items.some((i) => i.source === 'notice')).toBe(false);
  });

  it('hydrated flags are false on first render (enforcement, receipts)', async () => {
    const { result, render } = await inHost(() => ({ enforcement: injectEnforcementActions(), ready: injectReceiptsReady() }));
    expect(result.enforcement().hydrated).toBe(false);
    expect(result.ready()).toBe(false);
    await render();
    expect(result.enforcement().hydrated).toBe(true);
    expect(result.ready()).toBe(true);
  });

  it('injectScreeningPeople seeds the store after render', async () => {
    const { result, render } = await inHost(() => injectScreeningPeople());
    const seeded = result().length;
    expect(seeded).toBeGreaterThan(0);
    await render();
    expect(localStorage.getItem('lcrb.v2.security-screening')).not.toBeNull();
    expect(result().length).toBe(seeded);
  });

  it('injectLegalEntityBuild creates the sample build after render', async () => {
    const { result, render } = await inHost(() => injectLegalEntityBuild('Harding Hospitality Group', 'BC0987654'));
    await render();
    expect(result().id).toBe('build_BC0987654');
    expect(JSON.parse(localStorage.getItem('lcrb.v2.legal-entity-builds') ?? '{}')['build_BC0987654']).toBeTruthy();
  });

  it('injectBusinessProfile follows the session organization', async () => {
    const session = TestBed.inject(SessionProvider);
    const { result, render } = await inHost(() => injectBusinessProfile());
    await render();
    expect(result().legalName).toBe('LCRB Training Business 5 Ltd.');
    session.login({ role: 'licensee', firstName: 'A', lastName: 'B', organization: 'Fixture Ltd.' });
    expect(result().legalName).toBe('Fixture Ltd.');
  });

  it('injectAiAutoRetry surfaces a quota error at once, without retrying', async () => {
    const error = signal<Error | undefined>(undefined);
    let regenerated = 0;
    const { result, render } = await inHost(() =>
      injectAiAutoRetry({ status: signal('error'), error, busy: signal(false), regenerate: () => regenerated++ }),
    );
    await render();
    error.set(new Error('You have exceeded your quota: insufficient_quota'));
    await render();
    expect(result.isQuota()).toBe(true);
    expect(result.showError()).toBe(true);
    expect(regenerated).toBe(0);
  });
});

// : hooks with state beyond the plain adapter, checked against the prototype's hook bodies.
describe('converted hooks', () => {
  it('injectEstablishmentPlans is empty until hydrated, then newest first; injectEstablishmentPlan follows its id', async () => {
    const first = createPlan('First');
    const second = createPlan('Second');
    renamePlan(first.id, 'First renamed'); // now the most recently updated
    const id = signal<string | undefined>(second.id);
    const { result, render } = await inHost(() => ({ all: injectEstablishmentPlans(), one: injectEstablishmentPlan(id) }));
    expect(result.all()).toEqual([]);
    expect(result.one()).toBeUndefined();
    await render();
    expect(result.all().map((p) => p.title)).toEqual(['First renamed', 'Second']);
    expect(result.one()?.title).toBe('Second');
    id.set(undefined);
    expect(result.one()).toBeUndefined();
  });

  it('injectTiedHouseApplication is undefined until hydrated, then ensures the application again when its inputs change', async () => {
    const source = signal<'voluntary' | 'le-review'>('voluntary');
    const { result, render } = await inHost(() => injectTiedHouseApplication(source, 'Harding Hospitality Group', 'BC0987654'));
    expect(result()).toBeUndefined();
    await render();
    expect(result()?.id).toBe('th_voluntary_BC0987654');
    source.set('le-review');
    await render();
    expect(result()?.id).toBe('th_le-review_BC0987654');
  });

  it('injectLegalEntityBuild ensures a build again when the business number changes after hydration', async () => {
    const number = signal('BC0987654');
    const { result, render } = await inHost(() => injectLegalEntityBuild('Harding Hospitality Group', number));
    await render();
    number.set('BC1111111');
    await render();
    expect(result().id).toBe('build_BC1111111');
    expect(JSON.parse(localStorage.getItem('lcrb.v2.legal-entity-builds') ?? '{}')['build_BC1111111']).toBeTruthy();
  });

  it('injectTiedHouseOverlay is empty until hydrated', async () => {
    const { result, render } = await inHost(() => injectTiedHouseOverlay('BC0987654', 'applicant'));
    expect(result()).toEqual({ nodes: [], edges: [] });
    await render();
    expect(result()).toEqual({ nodes: [], edges: [] }); // no tied-house application stored yet
  });

  it('injectNoticesFeed shows the default feed until hydrated, then follows read state', async () => {
    const { result, render } = await inHost(() => injectNoticesFeed());
    expect(result().hydrated).toBe(false);
    await render();
    expect(result().hydrated).toBe(true);
    const unread = result().notices.find((n) => !n.read);
    expect(unread).toBeDefined();
    const before = result().unread;
    markNoticeRead(unread!.id);
    expect(result().unread).toBe(before - 1);
  });

  it('injectNoticeChime plays once for unread notices at login, then only when the count rises', async () => {
    let oscillators = 0;
    const param = { value: 0, setValueAtTime: () => param, exponentialRampToValueAtTime: () => param };
    const node = () => ({ gain: param, frequency: param, type: '', connect: () => undefined, start: () => undefined, stop: () => undefined });
    class FakeAudioContext {
      state = 'running';
      currentTime = 0;
      destination = {};
      createGain = node;
      createOscillator = () => (oscillators++, node());
    }
    vi.stubGlobal('AudioContext', FakeAudioContext);
    const plays = () => oscillators / 2; // two notes per chime
    const unread = signal(2);
    const ready = signal(false);
    const { render } = await inHost(() => injectNoticeChime(unread, ready, true));
    await render();
    expect(plays()).toBe(0); // not before the read state has hydrated
    ready.set(true);
    await render();
    expect(plays()).toBe(1); // unread on login
    expect(sessionStorage.getItem('lcrb.notice-chime.login-played')).toBe('1');
    unread.set(1);
    await render();
    expect(plays()).toBe(1); // a drop never chimes
    unread.set(3);
    await render();
    expect(plays()).toBe(2); // a new notice arrived
    vi.unstubAllGlobals();
  });
});
