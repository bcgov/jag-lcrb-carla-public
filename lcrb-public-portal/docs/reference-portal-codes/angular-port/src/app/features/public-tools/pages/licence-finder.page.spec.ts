/** the licence finder page and chat over canned UI message streams. */
import { Component } from '@angular/core';
import { type ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter, Router } from '@angular/router';
import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { replyWithChips, uiStream } from '../../../../../tests/fixtures/api/stream';
import { stubFetch } from '../../../../../tests/support/fetch-stub';
import type { ApiFixture } from '../../../../../tests/support/api-fixtures';
import { LicenceFinderPage } from './licence-finder.page';

@Component({ template: '' })
class Blank {}

let fixture: ComponentFixture<LicenceFinderPage>;
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
  const api = stubFetch(fixtures);
  TestBed.configureTestingModule({ providers: [provideRouter([{ path: '**', component: Blank }])] });
  fixture = TestBed.createComponent(LicenceFinderPage);
  el = fixture.nativeElement;
  await render();
  await render();
  return { api, router: TestBed.inject(Router) };
}
const button = (name: string) =>
  [...el.querySelectorAll('button')].find((b) => (b.getAttribute('aria-label') ?? b.textContent?.trim().replace(/\s+/g, ' ')) === name);
const textarea = () => el.querySelector('textarea[aria-label="Describe what you need"]') as HTMLTextAreaElement;

beforeEach(() => {
  localStorage.clear();
  sessionStorage.clear();
});
afterEach(() => vi.unstubAllGlobals());

describe('LicenceFinderPage', () => {
  it('renders the shell, intro and starter cards', async () => {
    await setup();
    expect(el.firstElementChild!.className).toBe('flex min-h-screen flex-col bg-background');
    expect(el.querySelector('main')!.className).toBe('flex-1 px-4 py-6 sm:px-6 lg:px-8');
    expect(el.querySelector('h1')!.textContent).toBe('Find the right licence');
    expect(el.textContent).toContain('What are you planning to open?');
    expect(el.textContent).toContain('Cannabis retail store');
    expect(el.textContent).toContain('Something else');
    expect(button('Send')!.hasAttribute('disabled')).toBe(true);
    expect(el.textContent).not.toContain('Start over');
  });

  it('sends a starter prompt with the applicant context and shows the reply, chips and recommendation', async () => {
    const { api, router } = await setup({
      '/api/licence-finder': uiStream('Here is what I suggest.', [
        {
          toolName: 'recommendApplications',
          input: {
            recommendations: [
              { slug: 'food-primary', title: 'Food Primary', reason: 'Restaurant.', confidence: 'high', establishmentName: 'Blue Anchor' },
            ],
          },
          output: {},
        },
        { toolName: 'suggestReplies', input: { replies: ['Tell me more', 'Fees?'] }, output: { replies: [] } },
      ]),
    });
    const nav = vi.spyOn(router, 'navigateByUrl').mockResolvedValue(true);
    button('RestaurantServe liquor with meals')!.click();
    await until(() => el.textContent!.includes('Here is what I suggest.'), 'reply');
    const body = api.callsTo('/api/licence-finder')[0].body as { messages: { role: string }[]; context: unknown };
    expect(body.messages).toHaveLength(1);
    expect(body.context).toEqual({ role: 'applicant' });
    expect(el.textContent).toContain('I want to open a restaurant that serves wine, beer, and cocktails with meals.');
    expect(el.textContent).toContain('high match');
    expect(button('Tell me more')).toBeTruthy();
    expect(button('Start over')).toBeTruthy();

    button('Start Food Primary')!.click();
    expect(nav).toHaveBeenCalledOnce();
    const href = nav.mock.calls[0][0] as string;
    expect(href).toContain('establishmentName=Blue+Anchor');
  });

  it('sends typed text on Enter, then returns to the intro on Start over', async () => {
    await setup({ '/api/licence-finder': replyWithChips('Sure.', ['Okay']) });
    textarea().value = 'A pub in Victoria';
    textarea().dispatchEvent(new Event('input'));
    await render();
    expect(button('Send')!.hasAttribute('disabled')).toBe(false);
    textarea().dispatchEvent(new KeyboardEvent('keydown', { key: 'Enter', bubbles: true, cancelable: true }));
    await until(() => el.textContent!.includes('Sure.'), 'reply');
    expect(el.textContent).toContain('A pub in Victoria');
    button('Start over')!.click();
    await render();
    expect(el.querySelector('h1')!.textContent).toBe('Find the right licence');
  });
});
