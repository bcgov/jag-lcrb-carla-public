/** Assistant panel tests over canned UI message streams from tests/fixtures/api, the same fixtures the Playwright flows use. */
import { Component } from '@angular/core';
import { type ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter, Router } from '@angular/router';
import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { replyWithChips, uiStream } from '../../../../tests/fixtures/api/stream';
import type { ApiFixture } from '../../../../tests/support/api-fixtures';
import { stubFetch } from '../../../../tests/support/fetch-stub';
import { AssistantProvider, type FormBridge } from './assistant-provider';
import { type Session, SessionProvider } from '@core/session/session-provider';
import { AssistantPanel } from './assistant-panel';

@Component({ template: '' })
class Blank {}

const session = (role: Session['role']): Session =>
  ({ role, firstName: 'Test', lastName: 'User', organization: 'Fixture Ltd.' }) as Session;

function fakeBridge(overrides: Partial<FormBridge> = {}): FormBridge {
  return {
    flowTitle: 'Name change',
    currentStep: 0,
    stepTitle: 'Details',
    totalSteps: 3,
    getFields: () => [{ name: 'proposedName', label: 'Proposed establishment name', type: 'text', value: '' }],
    getAllFields: () => [{ name: 'proposedName', label: 'Proposed establishment name', type: 'text', value: '', step: 0 }],
    stepOfField: () => 0,
    setFieldValue: vi.fn(() => true),
    goToStep: vi.fn(),
    tryGoToStep: vi.fn(() => ({ ok: true, blockedBy: [] })),
    blockersPreview: () => [{ name: 'proposedName', label: 'Proposed establishment name', error: 'Required' }],
    nextStep: vi.fn(),
    prevStep: vi.fn(),
    ...overrides,
  };
}

let fixture: ComponentFixture<AssistantPanel>;
let el: HTMLElement;

async function render() {
  fixture.detectChanges();
  await fixture.whenStable();
}

/** Re-render until the condition holds (streams resolve over several ticks). */
async function until(cond: () => boolean, label: string) {
  for (let i = 0; i < 100; i++) {
    await render();
    if (cond()) return;
    await new Promise((r) => setTimeout(r, 5));
  }
  throw new Error(`Timed out waiting for: ${label}`);
}

async function setup(role: Session['role'], fixtures: Record<string, ApiFixture>, bridge?: FormBridge) {
  const api = stubFetch(fixtures);
  TestBed.configureTestingModule({ providers: [provideRouter([{ path: '**', component: Blank }])] });
  TestBed.inject(SessionProvider).login(session(role));
  const assistant = TestBed.inject(AssistantProvider);
  if (bridge) assistant.registerFormBridge(bridge);
  assistant.setOpen(true);
  fixture = TestBed.createComponent(AssistantPanel);
  el = fixture.nativeElement;
  await render();
  return { api, assistant, router: TestBed.inject(Router) };
}

const panel = () => el.querySelector('[role="complementary"][aria-label="AI assistant"]') as HTMLElement | null;
const button = (name: string) =>
  [...el.querySelectorAll('button')].find((b) => (b.getAttribute('aria-label') ?? b.textContent?.trim()) === name);
const textarea = () => el.querySelector('textarea[aria-label="Message the assistant"]') as HTMLTextAreaElement;
async function type(text: string) {
  textarea().value = text;
  textarea().dispatchEvent(new Event('input'));
  await render();
}

beforeEach(() => {
  localStorage.clear();
  sessionStorage.clear();
});
afterEach(() => vi.unstubAllGlobals());

describe('AssistantPanel', () => {
  it('renders nothing while closed and the role intro with suggestions when open', async () => {
    const { assistant } = await setup('police', {});
    expect(panel()?.className).toBe(
      'fixed inset-y-0 right-0 z-50 flex w-full max-w-md flex-col border-l border-border bg-background shadow-2xl',
    );
    expect(el.textContent).toContain('How can I help, officer?');
    expect(el.textContent).toContain('Navigation & guidance');
    expect(button('Show me the review queue')).toBeTruthy();
    expect(button('Send message')?.disabled).toBe(true);
    assistant.setOpen(false);
    await render();
    expect(panel()).toBeNull();
  });

  it('sends the message with role, pathname and snapshot context, then shows the reply and chips', async () => {
    const { api } = await setup('licensee', {
      '/api/assistant': replyWithChips('Here is how renewals work.', ['Renew now', 'Fees?']),
    });
    button('When does my liquor licence expire?')!.click();
    await until(() => el.textContent!.includes('Here is how renewals work.'), 'reply text');
    const call = api.callsTo('/api/assistant')[0].body as { messages: unknown[]; context: Record<string, unknown> };
    expect(call.messages).toHaveLength(1);
    expect(call.context['role']).toBe('licensee');
    expect(call.context['pathname']).toBe('/');
    expect(call.context['snapshot']).toBeTruthy();
    expect(call.context['flowTitle']).toBeUndefined();
    expect(button('Renew now')?.className).toContain('rounded-full border border-[#013366]/30');
    expect(button('Start over')).toBeTruthy();

    // A chip sends its text as the next message.
    button('Fees?')!.click();
    await until(() => api.callsTo('/api/assistant').length === 2, 'second request');
  });

  it('answers setField through the form bridge and auto-sends the tool result', async () => {
    const bridge = fakeBridge();
    const { api } = await setup(
      'licensee',
      {
        '/api/assistant': {
          sequence: [
            uiStream('Setting it now.', [{ toolName: 'setField', input: { name: 'proposedName', value: 'Restaurant XYZ' } }]),
            uiStream('Done: the proposed name is set.'),
          ],
        },
      },
      bridge,
    );
    expect(el.textContent).toContain('Helping with: Name change');
    await type('Set the proposed name to Restaurant XYZ');
    button('Send message')!.click();
    await until(() => el.textContent!.includes('Done: the proposed name is set.'), 'second turn');
    expect(bridge.setFieldValue).toHaveBeenCalledWith('proposedName', 'Restaurant XYZ');
    expect(el.textContent).toContain('Filled "proposedName"');
    const calls = api.callsTo('/api/assistant');
    expect(calls).toHaveLength(2);
    expect((calls[0].body as { context: Record<string, unknown> }).context['allFields']).toHaveLength(1);
    expect(JSON.stringify(calls[1].body)).toContain('Set \\"proposedName\\" to \\"Restaurant XYZ\\".');
  });

  it('asks before navigating when the user only asked a question', async () => {
    const { router } = await setup('licensee', {
      '/api/assistant': {
        sequence: [
          uiStream('Your licences are listed on the licences page.', [
            { toolName: 'navigateTo', input: { href: '/licences', label: 'My licences' } },
          ]),
          uiStream('You can tap the button to go there.'),
        ],
      },
    });
    const nav = vi.spyOn(router, 'navigateByUrl');
    await type('Where are my licences?');
    button('Send message')!.click();
    await until(() => !!button('Yes, take me there') && el.textContent!.includes('tap the button'), 'consent card');
    expect(el.textContent).toContain('Want me to open My licences?');
    expect(nav).not.toHaveBeenCalled();
    button('Yes, take me there')!.click();
    expect(nav).toHaveBeenCalledWith('/licences');
  });

  it('navigates at once on an explicit request, and refuses paths outside the role', async () => {
    const { router, api } = await setup('applicant', {
      '/api/assistant': {
        sequence: [
          uiStream('Opening it.', [{ toolName: 'navigateTo', input: { href: '/special-events', label: 'Special events' } }]),
          uiStream('Done.'),
          uiStream('Trying.', [{ toolName: 'navigateTo', input: { href: '/licences', label: 'Licences' } }]),
          uiStream('That is not available to you.'),
        ],
      },
    });
    const nav = vi.spyOn(router, 'navigateByUrl');
    await type('Take me to special events');
    button('Send message')!.click();
    await until(() => el.textContent!.includes('Done.'), 'first exchange');
    expect(nav).toHaveBeenCalledWith('/special-events');

    await type('Take me to licences');
    button('Send message')!.click();
    await until(() => el.textContent!.includes('That is not available to you.'), 'refusal');
    expect(nav).toHaveBeenCalledTimes(1);
    expect(JSON.stringify(api.callsTo('/api/assistant')[3].body)).toContain('Refused — /licences is not part of this user');
  });

  it('answers a "can I continue?" goToStep with the blockers and does not move', async () => {
    const bridge = fakeBridge();
    const { api } = await setup(
      'licensee',
      {
        '/api/assistant': {
          sequence: [uiStream('Checking.', [{ toolName: 'goToStep', input: { step: 1 } }]), uiStream('Not yet.')],
        },
      },
      bridge,
    );
    await type('Can I go to the next step?');
    button('Send message')!.click();
    await until(() => el.textContent!.includes('Not yet.'), 'answer');
    expect(bridge.tryGoToStep).not.toHaveBeenCalled();
    expect(JSON.stringify(api.callsTo('/api/assistant')[1].body)).toContain('They CANNOT continue yet');
  });

  it('shows a quota failure at once without a retry button', async () => {
    await setup('licensee', {
      '/api/assistant': { status: 429, body: 'You exceeded your current quota: insufficient_quota', contentType: 'text/plain' },
    });
    await type('Hello');
    button('Send message')!.click();
    await until(() => !!el.querySelector('.border-amber-300'), 'error box');
    expect(button('Try again')).toBeUndefined();
  });

  it('submits on Enter, keeps Shift+Enter, seeds a pending prompt and starts over', async () => {
    const { assistant, api } = await setup('licensee', { '/api/assistant': uiStream('Hi there.') });
    assistant.setPendingPrompt('Help me renew');
    await render();
    expect(textarea().value).toBe('Help me renew');
    expect(assistant.pendingPrompt()).toBeNull();

    textarea().dispatchEvent(new KeyboardEvent('keydown', { key: 'Enter', shiftKey: true }));
    await render();
    expect(api.calls).toHaveLength(0);
    textarea().dispatchEvent(new KeyboardEvent('keydown', { key: 'Enter' }));
    await until(() => el.textContent!.includes('Hi there.'), 'reply');
    expect(textarea().value).toBe('');

    button('Start over')!.click();
    await render();
    expect(el.textContent).toContain('Try asking');
  });
});
