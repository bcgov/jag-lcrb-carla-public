import { Component } from '@angular/core';
import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { jsPDF } from 'jspdf';
import { afterEach, beforeEach, describe, expect, it } from 'vitest';
import { SessionProvider } from '@core/session/session-provider';
import { MOCK_LICENCES, MOCK_OPERATED_LICENCES } from '@data-access/repositories/licences/mock/licences';
import { isOverdueForRenewal } from '@domain/licences/licences';
import { LicencesPage } from './licences.page';

@Component({ template: '' })
class Blank {}

const settle = async (f: { detectChanges(): void; whenStable(): Promise<unknown> }) => {
  for (let i = 0; i < 3; i++) {
    f.detectChanges();
    await f.whenStable();
    await new Promise((r) => setTimeout(r, 0));
  }
  f.detectChanges();
};

/** jsPDF's initialized hook swaps each new document's save() for a no-op so no PDF is written. */
const noSave = [
  'initialized',
  function (this: { save: (name: string) => void }) {
    this.save = () => undefined;
  },
];
const pdfEvents = () => (jsPDF.API as unknown as { events: unknown[] }).events;
let saves = 0;
const countSave = [
  'initialized',
  function (this: { save: (name: string) => void }) {
    this.save = () => void saves++;
  },
];

async function mount() {
  TestBed.configureTestingModule({ providers: [provideRouter([{ path: '**', component: Blank }])] });
  TestBed.inject(SessionProvider).login({ role: 'licensee', firstName: 'Dana', lastName: 'Lee', organization: 'Cedar' } as never);
  const fixture = TestBed.createComponent(LicencesPage);
  document.body.appendChild(fixture.nativeElement);
  await settle(fixture);
  return { fixture, el: fixture.nativeElement as HTMLElement };
}
const cards = (el: HTMLElement) => [...el.querySelectorAll('[role="tabpanel"]:not([hidden]) div[licencecard]')];
const tab = (el: HTMLElement, text: string) => [...el.querySelectorAll('button[role="tab"]')].find((b) => b.textContent!.includes(text)) as HTMLButtonElement;

beforeEach(() => {
  localStorage.clear();
  sessionStorage.clear();
  pdfEvents().push(noSave);
});
afterEach(() => {
  for (const e of [noSave, countSave]) if (pdfEvents().includes(e)) pdfEvents().splice(pdfEvents().indexOf(e), 1);
  document.body.innerHTML = '';
});

describe('LicencesPage', () => {
  it('renders the hero, the cap card, the search count and one card per owned licence', async () => {
    const { el } = await mount();
    expect(el.querySelector('h1')!.textContent).toContain('Licences & Authorizations');
    expect(el.textContent).toContain('View and manage all your liquor and cannabis licences');
    expect(el.querySelector('a[href="/applications/new"]')!.textContent).toContain('Start a new application');
    expect(el.querySelector('#licence-cap-heading')).toBeTruthy();
    expect(el.querySelector('#licence-search-count')!.textContent!.trim()).toBe(`${MOCK_LICENCES.length} licences on your account`);
    expect(cards(el).length).toBe(MOCK_LICENCES.length);
    expect(tab(el, 'My Licences').textContent).toContain(`(${MOCK_LICENCES.length})`);
    const first = cards(el)[0];
    expect(first.textContent).toContain(`Licence #${MOCK_LICENCES[0].id}`);
    expect(first.querySelector('a[href="/licences/326739"]')!.textContent).toBe('OneStop Test 1');
    expect(first.querySelector('a[href="/licences/326739?tab=terms"]')).toBeTruthy();
  });

  it('shows the overdue banner and filters to overdue licences', async () => {
    const { fixture, el } = await mount();
    const overdue = MOCK_LICENCES.filter((l) => isOverdueForRenewal(l));
    const alert = el.querySelector('[role="alert"]');
    if (overdue.length === 0) {
      expect(alert).toBeNull();
      return;
    }
    expect(alert!.textContent).toContain(overdue.length === 1 ? '1 licence is overdue for renewal.' : `${overdue.length} licences are overdue for renewal.`);
    [...alert!.querySelectorAll('button')].find((b) => b.textContent!.includes('Show overdue only'))!.click();
    await settle(fixture);
    expect(cards(el).length).toBe(overdue.length);
    expect(el.querySelector('#licence-search-count')!.textContent!.trim()).toBe(`${overdue.length} of ${MOCK_LICENCES.length} licences match`);
  });

  it('filters by search text, clears it, and shows the empty state', async () => {
    const { fixture, el } = await mount();
    const input = el.querySelector('#licence-search') as HTMLInputElement;
    input.value = 'taproom nothing';
    input.dispatchEvent(new Event('input'));
    await settle(fixture);
    expect(cards(el).length).toBe(0);
    expect(el.textContent).toContain('No licences found matching your filters');
    input.value = '326741';
    input.dispatchEvent(new Event('input'));
    await settle(fixture);
    expect(cards(el).length).toBe(1);
    expect(cards(el)[0].textContent).toContain('Green Leaf Victoria');
    expect(el.querySelector('#licence-search-count')!.textContent!.trim()).toBe(`1 of ${MOCK_LICENCES.length} licences match`);
    [...el.querySelectorAll('button')].find((b) => b.textContent!.trim() === 'Clear')!.click();
    await settle(fixture);
    expect(cards(el).length).toBe(MOCK_LICENCES.length);
  });

  it('lists operated licences on the second tab with the third-party note and no common requests', async () => {
    const { fixture, el } = await mount();
    tab(el, 'Operated Licences').click();
    await settle(fixture);
    expect(cards(el).length).toBe(MOCK_OPERATED_LICENCES.length);
    expect(cards(el)[0].textContent).toContain('3rd-Party Operators cannot request changes to the licence');
  });

  it('opens the Actions menu with the prefilled renewal link', async () => {
    const { fixture, el } = await mount();
    (cards(el)[0].querySelector('button[data-slot="dropdown-menu-trigger"]') as HTMLButtonElement).click();
    await settle(fixture);
    const items = [...document.querySelectorAll('[data-slot="dropdown-menu-item"]')];
    const renew = items.find((i) => i.textContent!.trim() === 'Renew Licence')!;
    expect(renew.getAttribute('href')).toBe(
      '/applications/new/licence-renewal?licenceNumber=326739&establishmentName=OneStop%20Test%201&legalEntityName=LCRBTrainingBusiness5',
    );
    expect(items.some((i) => i.textContent!.includes('Download Licence Copy'))).toBe(true);
    expect(document.body.textContent).toContain('Common requests');
  });

  it('the Download Copy buttons build the licence PDF', async () => {
    saves = 0;
    pdfEvents().splice(pdfEvents().indexOf(noSave), 1);
    pdfEvents().push(countSave);
    const { el } = await mount();
    [...cards(el)[0].querySelectorAll('button')].find((b) => b.textContent!.includes('Download Copy'))!.click();
    expect(saves).toBe(1);
  });
});
