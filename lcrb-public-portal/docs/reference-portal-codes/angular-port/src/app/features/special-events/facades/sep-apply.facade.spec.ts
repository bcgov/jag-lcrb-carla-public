import { TestBed } from '@angular/core/testing';
import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { APPLICATIONS_REPOSITORY } from '@data-access/repositories/applications/applications.repository';
import { SPECIAL_EVENTS_REPOSITORY } from '@data-access/repositories/special-events/special-events.repository';
import { SepApplyFacade } from './sep-apply.facade';

describe('SepApplyFacade', () => {
  let href = '';
  beforeEach(() => {
    localStorage.clear();
    href = '';
    // Capture the navigation instead of letting jsdom try to load the page.
    vi.stubGlobal('location', { set href(value: string) { href = value; }, get href() { return href; } });
  });
  afterEach(() => vi.unstubAllGlobals());

  it('submits the permit from the form, deletes the draft and opens the list with the new id', () => {
    const applications = TestBed.inject(APPLICATIONS_REPOSITORY);
    applications.saveDraft({
      draftId: 'draft:sep',
      flowId: 'sep',
      title: 'Special Event Permit',
      resumeHref: '/special-events/apply',
      formData: {},
      currentStep: 0,
    } as never);
    expect(applications.draft('draft:sep')).toBeDefined();

    const id = TestBed.inject(SepApplyFacade).submit({
      eventName: 'Harbour Gala',
      eventType: 'public',
      eventCity: 'Victoria',
      locationName: 'Ship Point',
      eventDate: '2026-12-12',
      eventStartTime: '18:00',
      eventEndTime: '23:00',
      maxGuests: '120',
      liquorSaleType: 'selling',
      isPublicProperty: 'yes',
    });

    const store = TestBed.runInInjectionContext(() => TestBed.inject(SPECIAL_EVENTS_REPOSITORY).store());
    const record = store.getById(id)!;
    expect(record.eventName).toBe('Harbour Gala');
    expect(record.city).toBe('Victoria');
    expect(record.expectedAttendance).toBe(120);
    expect(record.serviceType).toBe('Sale of liquor');
    expect(applications.draft('draft:sep')).toBeUndefined();
    expect(href).toBe(`/special-events?submitted=${encodeURIComponent(id)}`);
  });
});
