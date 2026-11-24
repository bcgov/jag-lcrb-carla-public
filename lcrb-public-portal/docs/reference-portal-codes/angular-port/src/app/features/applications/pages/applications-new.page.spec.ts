import { TestBed } from '@angular/core/testing';
import { ActivatedRoute, convertToParamMap, provideRouter } from '@angular/router';
import { of } from 'rxjs';
import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { SessionProvider } from '@core/session/session-provider';
import { APPLICATION_CATALOG } from '@data-access/repositories/applications/mock/applications';
import { stubFetch } from '../../../../../tests/support/fetch-stub';
import { ApplicationsNewPage } from './applications-new.page';

const settle = async (f: { detectChanges(): void; whenStable(): Promise<unknown> }) => {
  for (let i = 0; i < 3; i++) {
    f.detectChanges();
    await f.whenStable();
    await new Promise((r) => setTimeout(r, 0));
  }
  f.detectChanges();
};
async function mount(params: Record<string, string> = {}) {
  TestBed.configureTestingModule({
    providers: [
      provideRouter([]),
      { provide: ActivatedRoute, useValue: { queryParamMap: of(convertToParamMap(params)), snapshot: { queryParamMap: convertToParamMap(params) } } },
    ],
  });
  TestBed.inject(SessionProvider).login({
    role: 'licensee',
    firstName: 'Dana',
    lastName: 'Lee',
    organization: 'LCRB Training Business 5 Ltd.',
  } as never);
  const fixture = TestBed.createComponent(ApplicationsNewPage);
  document.body.appendChild(fixture.nativeElement);
  await settle(fixture);
  return { fixture, el: fixture.nativeElement as HTMLElement };
}
const search = (el: HTMLElement) => el.querySelector('#application-search') as HTMLInputElement;
const count = (el: HTMLElement) => el.querySelector('#application-search-count')!.textContent!.trim();
const type = async (f: Parameters<typeof settle>[0], el: HTMLElement, text: string) => {
  search(el).value = text;
  search(el).dispatchEvent(new Event('input'));
  await settle(f);
};

describe('ApplicationsNewPage', () => {
  beforeEach(() => {
    localStorage.clear();
    sessionStorage.clear();
    stubFetch({});
  });
  afterEach(() => {
    vi.unstubAllGlobals();
    document.body.innerHTML = '';
  });

  it('renders the search-first layout, banner, suggestions and catalogue sections', async () => {
    const { el } = await mount();
    expect(el.firstElementChild!.className).toBe('min-h-screen flex flex-col');
    expect(el.querySelector('main#main-content')!.className).toBe('flex-1');
    expect(el.querySelector('h1')!.textContent).toContain('Start a New Application or Authorization');
    expect(el.textContent).toContain('Applications available for the licences on your account, most relevant first.');
    const form = el.querySelector('search')!;
    expect(form.getAttribute('role')).toBe('search');
    expect(form.className).toBe('mb-8');
    expect(form.querySelector('label')!.getAttribute('for')).toBe('application-search');
    expect(form.querySelector('label')!.textContent).toBe('What do you need to do?');
    expect(search(el).type).toBe('search');
    expect(search(el).getAttribute('aria-describedby')).toBe('application-search-count');
    expect(search(el).className).toContain('h-14 rounded-xl border-2');
    expect(document.activeElement).toBe(search(el));
    expect(el.querySelector('processing-times-banner section')).toBeTruthy();
    expect(el.querySelector('suggest-applications')).toBeTruthy();
    expect(el.querySelector('#most-used-heading')!.textContent).toBe('Most used');
    expect(el.textContent).toContain('All applications and authorizations');
    expect(count(el)).toMatch(/^\d+ available$/);
    expect(el.querySelector('footer')).toBeTruthy();
  });

  it('links each single-variant card to /applications/new/<slug> and gives multi-variant cards one link per licence type', async () => {
    const { el } = await mount();
    const hrefs = [...el.querySelectorAll('a[href^="/applications/new/"]')].map((a) => a.getAttribute('href')!);
    expect(hrefs.length).toBeGreaterThan(0);
    const slugs = new Set(APPLICATION_CATALOG.map((e) => e.slug));
    for (const h of hrefs) expect(slugs.has(h.replace('/applications/new/', ''))).toBe(true);
    const card = el.querySelector('a.group')!;
    expect(card.className).toBe('group rounded-lg focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-[#013366]');
    expect(card.textContent).toMatch(/Fee:|No application fee/);
    expect(card.textContent).toContain('Start');
    const multi = [...el.querySelectorAll('div.group')].find((d) => d.textContent!.includes('Choose a licence type'));
    if (multi) expect(multi.querySelectorAll('a[href^="/applications/new/"]').length).toBeGreaterThan(1);
  });

  it('filters the catalogue as you type, shows the match count and hides Most used while searching', async () => {
    const { fixture, el } = await mount();
    const before = Number(count(el).split(' ')[0]);
    await type(fixture, el, 'patio');
    expect(count(el)).toMatch(/^\d+ (match|matches) for “patio”$/);
    expect(Number(count(el).split(' ')[0])).toBeLessThan(before);
    expect(el.querySelector('#most-used-heading')).toBeNull();
    expect(el.textContent).not.toContain('All applications and authorizations');
    expect(el.querySelector('a[href^="/applications/new/"]')).toBeTruthy();
    const clear = [...el.querySelectorAll('button')].find((b) => b.textContent!.trim() === 'Clear') as HTMLButtonElement;
    expect(clear.className).toContain('absolute right-2 top-1/2');
    clear.click();
    await settle(fixture);
    expect(search(el).value).toBe('');
    expect(count(el)).toBe(`${before} available`);
    expect(el.querySelector('#most-used-heading')).toBeTruthy();
  });

  it('shows the empty state for a query with no match', async () => {
    const { fixture, el } = await mount();
    await type(fixture, el, 'qqqxxxzzz');
    expect(count(el)).toBe('0 matches for “qqqxxxzzz”');
    expect(el.textContent).toContain('No available applications match “qqqxxxzzz”.');
    expect(el.querySelector('div.text-center.py-12.bg-white.border.rounded-lg')).toBeTruthy();
  });

  it('primes the search from ?q= and scopes to a recognised ?type=', async () => {
    const primed = await mount({ q: 'renew' });
    expect(search(primed.el).value).toBe('renew');
    expect(count(primed.el)).toMatch(/for “renew”$/);
    TestBed.resetTestingModule();
    document.body.innerHTML = '';
    const scoped = await mount({ type: 'liquor-primary' });
    expect(count(scoped.el)).toMatch(/^\d+ available for .+$/);
    TestBed.resetTestingModule();
    document.body.innerHTML = '';
    const ignored = await mount({ type: 'not-a-type' });
    expect(count(ignored.el)).toMatch(/^\d+ available$/);
  });

  it('collapses the licences-you-do-not-hold section and toggles it', async () => {
    const { fixture, el } = await mount();
    const toggle = el.querySelector('button[aria-controls="unavailable-panel"]') as HTMLButtonElement | null;
    if (!toggle) return;
    expect(toggle.getAttribute('aria-expanded')).toBe('false');
    expect(el.querySelector('#unavailable-panel')).toBeNull();
    expect(toggle.textContent).toContain("Requires a licence you don't hold");
    toggle.click();
    await settle(fixture);
    expect(toggle.getAttribute('aria-expanded')).toBe('true');
    expect(el.querySelector('#unavailable-panel')!.textContent).toContain('Requires an active');
    expect(toggle.querySelector('svg:last-child')!.getAttribute('class')).toContain('rotate-0');
  });
});
