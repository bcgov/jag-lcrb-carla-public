import { Component } from '@angular/core';
import { TestBed } from '@angular/core/testing';
import { ActivatedRoute, convertToParamMap, provideRouter } from '@angular/router';
import { jsPDF } from 'jspdf';
import { afterEach, beforeEach, describe, expect, it } from 'vitest';
import { getLicenceById } from '@data-access/repositories/licences/mock/licences';
import { getScheduledInspectionsForLicence } from '@data-access/repositories/licences/mock/inspections';
import { listAuditForSubject } from '@data-access/repositories/activity/audit-service';
import { LicenceDetailPage } from './licence-detail.page';

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

let saves = 0;
const countSave = [
  'initialized',
  function (this: { save: (name: string) => void }) {
    this.save = () => void saves++;
  },
];
const pdfEvents = () => (jsPDF.API as unknown as { events: unknown[] }).events;

async function mount(id: string, query: Record<string, string> = {}) {
  TestBed.configureTestingModule({
    providers: [
      provideRouter([{ path: '**', component: Blank }]),
      { provide: ActivatedRoute, useValue: { snapshot: { queryParamMap: convertToParamMap(query) } } },
    ],
  });
  const fixture = TestBed.createComponent(LicenceDetailPage);
  fixture.componentRef.setInput('id', id);
  document.body.appendChild(fixture.nativeElement);
  await settle(fixture);
  return { fixture, el: fixture.nativeElement as HTMLElement };
}
const tab = (el: HTMLElement, text: string) => [...el.querySelectorAll('button[role="tab"]')].find((b) => b.textContent!.includes(text)) as HTMLButtonElement;
const panel = (el: HTMLElement) => el.querySelector('[role="tabpanel"]:not([hidden])') as HTMLElement;

beforeEach(() => {
  localStorage.clear();
  sessionStorage.clear();
  saves = 0;
  pdfEvents().push(countSave);
});
afterEach(() => {
  pdfEvents().splice(pdfEvents().indexOf(countSave), 1);
  document.body.innerHTML = '';
});

describe('LicenceDetailPage', () => {
  it('renders the hero, actions and the overview for licence 326739', async () => {
    const { el } = await mount('326739');
    const l = getLicenceById('326739')!;
    expect(el.querySelector('h1')!.textContent).toContain(l.name);
    expect(el.textContent).toContain('Liquor Primary | Licence #326739');
    expect(el.querySelector('a[href="/licences"]')!.textContent).toContain('Licences & Authorizations');
    const hrefs = [...el.querySelectorAll('a[data-slot="button"]')].map((a) => a.getAttribute('href'));
    expect(hrefs).toContain(
      '/applications/new/licence-renewal?licenceNumber=326739&establishmentName=OneStop%20Test%201&legalEntityName=LCRBTrainingBusiness5',
    );
    expect(hrefs).toContain(
      '/applications/new/lp-transfer?licenceNumber=326739&establishmentName=OneStop%20Test%201&legalEntityName=LCRBTrainingBusiness5',
    );
    expect(hrefs.some((h) => h!.startsWith('/applications/new?q=endorsement&type='))).toBe(true);
    for (const flag of l.flags) expect(el.textContent).toContain(flag);
    expect(panel(el).textContent).toContain(l.address);
    expect(panel(el).textContent).toContain('Endorsements');
    expect(panel(el).textContent).toContain(l.endorsements![0].name);
    expect(panel(el).textContent).toContain('Next scheduled inspection');
    expect(panel(el).querySelector('status-explainer')).toBeTruthy();
    expect(tab(el, 'Inspections').textContent).toContain(String(getScheduledInspectionsForLicence('326739').length));
  });

  it('the next-inspection card switches to the Inspections tab', async () => {
    const { fixture, el } = await mount('326739');
    [...panel(el).querySelectorAll('button')].find((b) => b.textContent!.includes('View inspections'))!.click();
    await settle(fixture);
    expect(tab(el, 'Inspections').getAttribute('aria-selected')).toBe('true');
    expect(panel(el).querySelector('licence-inspections')).toBeTruthy();
  });

  it('opens the tab named by ?tab= and renders the terms with a PDF button that audits the download', async () => {
    const { fixture, el } = await mount('326739', { tab: 'terms' });
    expect(tab(el, 'Terms').getAttribute('aria-selected')).toBe('true');
    expect(panel(el).querySelectorAll('h2').length).toBeGreaterThan(0);
    [...panel(el).querySelectorAll('button')].find((b) => b.textContent!.includes('Download Licence Copy (PDF)'))!.click();
    await settle(fixture);
    expect(saves).toBe(1);
    expect(listAuditForSubject('326739').some((a) => a.action === 'licence.certificate-downloaded')).toBe(true);
  });

  it('Download Certificate in the hero also builds the PDF', async () => {
    const { el } = await mount('326739');
    [...el.querySelectorAll('button')].find((b) => b.textContent!.includes('Download Certificate'))!.click();
    expect(saves).toBe(1);
  });

  it('shows the inline "Licence not found" message for an unknown id', async () => {
    const { el } = await mount('nope');
    expect(el.querySelector('h1')!.textContent).toBe('Licence not found');
    expect(el.textContent).toContain("We couldn't find a licence with number nope.");
    expect(el.querySelector('main a[href="/licences"]')!.textContent).toContain('Back to Licences');
    expect(el.querySelector('page-hero')).toBeNull();
  });
});
