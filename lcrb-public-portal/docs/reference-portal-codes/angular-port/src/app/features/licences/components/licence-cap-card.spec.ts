import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { beforeEach, describe, expect, it } from 'vitest';
import { SessionProvider } from '@core/session/session-provider';
import { LicenceCapCard } from './licence-cap-card';

const settle = async (f: { detectChanges(): void; whenStable(): Promise<unknown> }) => {
  for (let i = 0; i < 3; i++) {
    f.detectChanges();
    await f.whenStable();
  }
};

function create() {
  TestBed.configureTestingModule({ providers: [provideRouter([])] });
  TestBed.inject(SessionProvider).login({ role: 'licensee', firstName: 'Dana', lastName: 'Lee', organization: 'Cedar' } as never);
  return TestBed.createComponent(LicenceCapCard);
}
const button = (el: HTMLElement, text: string) => [...el.querySelectorAll('button')].find((b) => b.textContent!.includes(text))!;

beforeEach(() => {
  localStorage.clear();
  sessionStorage.clear();
});

describe('LicenceCapCard', () => {
  it('settles from the skeleton to the collapsed card', async () => {
    const fixture = create();
    const el = fixture.nativeElement as HTMLElement;
    await settle(fixture);
    expect(el.querySelector('section[aria-busy]')).toBeNull();
    const header = el.querySelector('#licence-cap-heading button')!;
    expect(header.getAttribute('aria-expanded')).toBe('false');
    expect(header.textContent).toContain('Licence cap status');
    expect(header.textContent).toMatch(/Cannabis \d+ of 8 · Liquor \d+ \(no cap\)/);
    expect(el.querySelector('#licence-cap-panel')!.hasAttribute('hidden')).toBe(true);
  });

  it('expands the panel and toggles the counting detail', async () => {
    const fixture = create();
    await settle(fixture);
    const el = fixture.nativeElement as HTMLElement;
    (el.querySelector('#licence-cap-heading button') as HTMLButtonElement).click();
    await settle(fixture);
    const header = el.querySelector('#licence-cap-heading button')!;
    expect(header.getAttribute('aria-expanded')).toBe('true');
    expect(header.textContent).toContain('Maximum 8 cannabis retail licences per business group. Liquor licences are not capped.');
    expect(el.querySelector('#licence-cap-panel')!.hasAttribute('hidden')).toBe(false);
    expect(el.textContent).toContain('Cannabis licences');
    expect(el.textContent).toContain('There is no limit on the number of liquor licences');
    expect(el.textContent).not.toContain('Effective business group');
    button(el, 'Show how this is counted').click();
    await settle(fixture);
    expect(el.textContent).toContain('Effective business group');
    expect(el.textContent).toContain('Contributing licences');
    expect(el.querySelector('a[href="/legal-entity"]')).toBeTruthy();
    expect(el.querySelector('a[href="/tied-house"]')).toBeTruthy();
    button(el, 'Hide how this is counted').click();
    await settle(fixture);
    expect(el.textContent).not.toContain('Effective business group');
  });
});
