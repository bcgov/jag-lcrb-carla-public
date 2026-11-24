/** One Door chat over canned streams: starters, hand-off card, referral link and reset. */
import { Component } from '@angular/core';
import { type ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { replyWithChips, uiStream } from '../../../../../tests/fixtures/api/stream';
import { stubFetch } from '../../../../../tests/support/fetch-stub';
import type { ApiFixture } from '../../../../../tests/support/api-fixtures';
import { OneDoorExperience } from './one-door-experience';

@Component({ template: '' })
class Blank {}

let fixture: ComponentFixture<OneDoorExperience>;
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
async function setup(fixtures: Record<string, ApiFixture> = {}) {
  Element.prototype.scrollTo = vi.fn() as never;
  const api = stubFetch(fixtures);
  TestBed.configureTestingModule({ providers: [provideRouter([{ path: '**', component: Blank }])] });
  fixture = TestBed.createComponent(OneDoorExperience);
  el = fixture.nativeElement;
  await render();
  await render();
  return api;
}
const button = (name: string) =>
  [...el.querySelectorAll('button')].find((b) => (b.getAttribute('aria-label') ?? b.textContent?.trim()) === name);

beforeEach(() => {
  localStorage.clear();
  sessionStorage.clear();
});
afterEach(() => {
  vi.useRealTimers();
  vi.unstubAllGlobals();
});

describe('OneDoorExperience', () => {
  it('renders the greeting, starters and the connected branches list', async () => {
    await setup();
    expect(el.querySelector('h1')!.textContent).toContain('How can we help you today?');
    expect(el.textContent).toContain("Hello, and welcome. I'm the Province of British Columbia's assistant");
    expect(button('I want to open a restaurant that serves wine')).toBeTruthy();
    expect(el.querySelectorAll('aside li')).toHaveLength(6);
    expect(el.querySelector('aside li')!.className).toContain('border-[#013366]/40');
    expect(el.textContent).toContain('Connected in this prototype');
    expect(el.textContent).toContain('Recognised — destination not simulated');
    expect(button('Start over')).toBeUndefined();
    expect(button('Send')!.hasAttribute('disabled')).toBe(true);
  });

  it('sends a starter, shows a live LCRB hand-off card that enables its link after the beat, then resets', async () => {
    vi.useFakeTimers({ toFake: ['setTimeout', 'clearTimeout'] });
    const api = await setup({
      '/api/one-door': uiStream('Connecting you to LCRB.', [
        {
          toolName: 'handoffToBranch',
          input: { branch: 'lcrb', summary: 'Open a restaurant', keywords: ['restaurant', 'wine'] },
          output: {},
        },
      ]),
    });
    button('I want to open a restaurant that serves wine')!.click();
    for (let i = 0; i < 100 && !el.querySelector('[role="status"]'); i++) {
      await vi.advanceTimersByTimeAsync(5);
      await render();
    }
    expect(api.callsTo('/api/one-door')[0].body).toMatchObject({ messages: [{ role: 'assistant' }, { role: 'user' }] });
    const card = el.querySelector('[role="status"]') as HTMLElement;
    expect(card.textContent).toContain('Connecting you to');
    expect(card.textContent).toContain('Travelling with you');
    expect(card.textContent).toContain('Open a restaurant');
    expect(card.textContent).toContain('restaurant');
    const link = card.querySelector('a') as HTMLAnchorElement;
    expect(link.textContent).toContain('Continue to the LCRB licensing portal');
    expect(link.getAttribute('href')).toBe('/login?from=one-door');
    expect(link.getAttribute('aria-disabled')).toBe('true');
    expect(link.getAttribute('tabindex')).toBe('-1');
    expect(link.className).toContain('pointer-events-none');
    expect(link.className).toContain('opacity-50');

    await vi.advanceTimersByTimeAsync(1500);
    await render();
    expect(card.textContent).toContain('Connected to');
    expect(link.getAttribute('aria-disabled')).toBe('false');
    expect(link.getAttribute('tabindex')).toBe('0');
    expect(link.className).toContain('opacity-100');

    button('Start over')!.click();
    await render();
    expect(el.querySelector('[role="status"]')).toBeNull();
    expect(button('Start over')).toBeUndefined();
  });

  it('shows a non-live branch as an explanation and chips as quick replies', async () => {
    await setup({
      '/api/one-door': uiStream('That is ICBC.', [
        { toolName: 'handoffToBranch', input: { branch: 'icbc' }, output: {} },
        { toolName: 'suggestReplies', input: { replies: ['Something else'] }, output: {} },
      ]),
    });
    button("Renew my driver's licence")!.click();
    await until(() => !!el.querySelector('[role="status"]'), 'card');
    expect(el.querySelector('[role="status"] a')).toBeNull();
    expect(el.textContent).toContain("That destination isn't part of this prototype");
    expect(button('Something else')).toBeTruthy();
  });

  it('shows an error alert on a failed request and dismisses it', async () => {
    await setup({ '/api/one-door': { status: 500, body: 'boom', contentType: 'text/plain' } });
    button('Enrol my newborn in MSP')!.click();
    await until(() => !!el.querySelector('[role="alert"]'), 'alert');
    button('Dismiss')!.click();
    await render();
    expect(el.querySelector('[role="alert"]')).toBeNull();
  });
});
