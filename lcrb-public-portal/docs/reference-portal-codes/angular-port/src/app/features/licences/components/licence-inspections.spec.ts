import { TestBed } from '@angular/core/testing';
import { afterEach, describe, expect, it, vi } from 'vitest';
import { getPastInspectionsForLicence, getScheduledInspectionsForLicence } from '@data-access/repositories/licences/mock/inspections';
import { LicenceInspections } from './licence-inspections';

function mount(licenceId: string) {
  const fixture = TestBed.createComponent(LicenceInspections);
  fixture.componentRef.setInput('licenceId', licenceId);
  fixture.detectChanges();
  return fixture.nativeElement as HTMLElement;
}

afterEach(() => vi.restoreAllMocks());

describe('LicenceInspections', () => {
  it('lists the scheduled inspections and the history for the licence', () => {
    const el = mount('326739');
    const upcoming = getScheduledInspectionsForLicence('326739');
    const past = getPastInspectionsForLicence('326739');
    expect(el.querySelector('div.space-y-8')).toBeTruthy();
    expect(el.textContent).toContain('Compliance & Enforcement');
    const sections = el.querySelectorAll('section');
    expect(sections[0].getAttribute('aria-label')).toBe('Upcoming inspections');
    expect(sections[0].querySelector('h2')!.textContent).toBe('Scheduled inspections');
    expect(sections[0].querySelector('[data-slot="badge"]')!.textContent!.trim()).toBe(String(upcoming.length));
    expect(sections[0].textContent).toContain(upcoming[0].window);
    expect(sections[0].textContent).toContain('What the inspector will review');
    expect(sections[1].querySelector('h2')!.textContent).toBe('Inspection history');
    expect(sections[1].querySelectorAll('.divide-y > div').length).toBe(past.length);
    expect(sections[1].textContent).toContain(past[0].description);
  });

  it('shows the empty states for a licence with no inspections', () => {
    const el = mount('441882');
    expect(el.textContent).toContain('No inspections are currently scheduled for this licence.');
    expect(el.textContent).toContain('No past inspections are on record for this licence.');
  });

  it('Add to calendar downloads an .ics named after the inspection', () => {
    const names: string[] = [];
    vi.stubGlobal('URL', Object.assign(URL, { createObjectURL: () => 'blob:x', revokeObjectURL: () => undefined }));
    vi.spyOn(HTMLAnchorElement.prototype, 'click').mockImplementation(function (this: HTMLAnchorElement) {
      names.push(this.download);
    });
    const el = mount('326739');
    const button = [...el.querySelectorAll('button')].find((b) => b.textContent!.includes('Add to calendar'))!;
    button.click();
    expect(names).toEqual([`${getScheduledInspectionsForLicence('326739')[0].id}.ics`]);
  });
});
