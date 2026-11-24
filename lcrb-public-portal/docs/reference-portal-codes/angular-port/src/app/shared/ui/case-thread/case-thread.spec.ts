import { Component, signal } from '@angular/core';
import { TestBed } from '@angular/core/testing';
import { afterEach, beforeEach, describe, expect, it } from 'vitest';
import { SessionProvider } from '@core/session/session-provider';
import { type UserRole } from '@domain/roles';
import { listMessages, unreadForThread } from '@data-access/repositories/messaging/chat-service';
import { CaseThread } from './case-thread';

@Component({
  imports: [CaseThread],
  template: `<case-thread
    [threadId]="threadId()"
    [kind]="kind()"
    [status]="status()"
    subjectLabel="Liquor Primary Licence LP-125998"
    [hrefFor]="{ licensee: '/applications/submitted/LP-125998' }"
    [title]="title()"
    [compact]="compact()"
  />`,
})
class Host {
  threadId = signal('LP-125998');
  kind = signal<'application' | 'sep'>('application');
  status = signal('under_review');
  title = signal('Case conversation');
  compact = signal(false);
}

const SESSION = (role: UserRole) => ({ role, firstName: 'Dana', lastName: 'Lee', organization: 'The Cedar Room Ltd.' }) as never;

async function mount(role: UserRole | null = 'licensee', setup?: (h: Host) => void) {
  if (role) TestBed.inject(SessionProvider).login(SESSION(role));
  const fixture = TestBed.createComponent(Host);
  setup?.(fixture.componentInstance);
  const settle = async () => {
    for (let i = 0; i < 3; i++) {
      fixture.detectChanges();
      await fixture.whenStable();
      await new Promise((r) => setTimeout(r, 0));
    }
    fixture.detectChanges();
  };
  await settle();
  return { fixture, el: fixture.nativeElement as HTMLElement, settle };
}

describe('CaseThread', () => {
  beforeEach(() => {
    localStorage.clear();
    sessionStorage.clear();
  });
  afterEach(() => localStorage.clear());

  it('seeds and renders the live thread with the prototype classes, badge and composer', async () => {
    const { el } = await mount();
    const section = el.querySelector('section')!;
    expect(section.getAttribute('aria-label')).toBe('Case conversation');
    expect(section.getAttribute('class')).toBe('flex flex-col rounded-lg border border-border bg-card ');
    expect(el.querySelector('header')!.getAttribute('class')).toBe('flex flex-wrap items-center justify-between gap-2 border-b border-border px-4 py-3');
    expect(el.querySelector('h2')!.getAttribute('class')).toBe('font-semibold text-foreground text-base');
    expect(el.querySelector('h2 + span')!.textContent!.trim()).toBe('Live');
    expect(el.textContent).toContain('With: Applicant · LCRB staff · Local government');
    const log = el.querySelector('[role="log"]')!;
    expect(log.getAttribute('class')).toBe('flex flex-col gap-3 overflow-y-auto px-4 py-4 max-h-96');
    expect(log.getAttribute('aria-live')).toBe('polite');
    expect(log.textContent).toContain('Kasey Poole');
    expect(log.textContent).toContain('Ricardo Tubbs');
    expect(log.textContent).toContain('City of Victoria');
    const bubble = log.querySelector('p.whitespace-pre-wrap')!;
    expect(bubble.className).toBe('whitespace-pre-wrap rounded-lg px-3 py-2 text-left text-sm leading-relaxed bg-muted text-foreground');
    expect(el.querySelector('textarea')!.getAttribute('aria-label')).toBe('Message');
    expect((el.querySelector('button[aria-label="Send message"]') as HTMLButtonElement).disabled).toBe(true);
  });

  it('marks the thread read for a participant and uses compact sizing', async () => {
    const { el } = await mount('lcrb', (h) => h.compact.set(true));
    expect(unreadForThread('LP-125998', 'lcrb')).toBe(0);
    expect(el.querySelector('header')!.getAttribute('class')).toContain('py-2.5');
    expect(el.querySelector('h2')!.getAttribute('class')).toContain('text-sm');
    expect(el.querySelector('[role="log"]')!.getAttribute('class')).toContain('max-h-72');
  });

  it('sends on Enter (not Shift+Enter), clears the draft and shows the message as "You"', async () => {
    const { el, settle } = await mount();
    const ta = el.querySelector('textarea') as HTMLTextAreaElement;
    ta.value = 'The site plan is correct.';
    ta.dispatchEvent(new Event('input'));
    await settle();
    expect((el.querySelector('button[aria-label="Send message"]') as HTMLButtonElement).disabled).toBe(false);
    ta.dispatchEvent(new KeyboardEvent('keydown', { key: 'Enter', shiftKey: true, bubbles: true, cancelable: true }));
    await settle();
    expect(listMessages('LP-125998').some((m) => m.body === 'The site plan is correct.')).toBe(false);
    const enter = new KeyboardEvent('keydown', { key: 'Enter', bubbles: true, cancelable: true });
    ta.dispatchEvent(enter);
    await settle();
    expect(enter.defaultPrevented).toBe(true);
    const sent = listMessages('LP-125998').find((m) => m.body === 'The site plan is correct.')!;
    expect(sent).toMatchObject({ authorRole: 'licensee', authorName: 'Dana Lee', authorOrg: 'The Cedar Room Ltd.' });
    expect(ta.value).toBe('');
    const mine = [...el.querySelectorAll('[role="log"] > div')].pop()!;
    expect(mine.className).toBe('flex justify-end');
    expect(mine.textContent).toContain('You');
    expect(mine.querySelector('p')!.className).toContain('bg-[#013366] text-white');
  });

  it('sends with the Send button', async () => {
    const { el, settle } = await mount();
    const ta = el.querySelector('textarea') as HTMLTextAreaElement;
    ta.value = '  hello  ';
    ta.dispatchEvent(new Event('input'));
    await settle();
    (el.querySelector('button[aria-label="Send message"]') as HTMLButtonElement).click();
    await settle();
    expect(listMessages('LP-125998').some((m) => m.body === 'hello')).toBe(true);
  });

  it('shows the closed state when the file is no longer in process', async () => {
    const { el } = await mount('licensee', (h) => h.status.set('approved'));
    expect(el.querySelector('h2 + span')!.textContent!.trim()).toBe('Closed');
    expect(el.querySelector('textarea')).toBeNull();
    expect(el.textContent).toContain('This conversation is closed because the file is no longer in process. Messages stay on the record.');
  });

  it('says so when an empty thread is open or closed', async () => {
    const open = await mount('licensee', (h) => h.threadId.set('NONE-1'));
    expect(open.el.querySelector('[role="log"] p')!.textContent).toContain('No messages yet. Use this thread to resolve questions on the file without email back-and-forth.');
    TestBed.resetTestingModule();
    localStorage.clear();
    const closed = await mount('licensee', (h) => {
      h.threadId.set('NONE-1');
      h.status.set('approved');
    });
    expect(closed.el.querySelector('[role="log"] p')!.textContent).toContain('No messages were exchanged on this file.');
  });

  it('tells a non-participant their role is not part of the conversation', async () => {
    const { el } = await mount('police');
    expect(el.textContent).toContain('Your role is not a participant in this conversation.');
    expect(el.querySelector('textarea')).toBeNull();
    expect(unreadForThread('LP-125998', 'police')).toBeGreaterThanOrEqual(0);
  });

  it('lists SEP participants including the applicant and police', async () => {
    const { el } = await mount('applicant', (h) => {
      h.kind.set('sep');
      h.threadId.set('SEP-2026-04788');
      h.status.set('submitted');
    });
    expect(el.textContent).toContain('With: Applicant · LCRB staff · Police');
    expect(el.querySelector('[role="log"]')!.textContent).toContain('Cst. Dan Whitmore');
  });
});
