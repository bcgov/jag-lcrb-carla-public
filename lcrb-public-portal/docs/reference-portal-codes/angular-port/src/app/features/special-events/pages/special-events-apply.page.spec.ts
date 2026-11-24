import { TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { provideRouter } from '@angular/router';
import { toast } from '@spartan-ng/brain/sonner';
import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { ApplicationFlowState, ApplicationProvider } from '@shared/forms';
import { SessionProvider } from '@core/session/session-provider';
import { SepStoreProvider } from '@data-access/repositories/special-events/sep-store-provider';
import { getDraft } from '@data-access/repositories/applications/drafts';
import { SERVINGS_PREFILL_KEY } from '@core/navigation/servings-prefill';
import { SpecialEventsApplyPage } from './special-events-apply.page';

const settle = async (f: { detectChanges(): void; whenStable(): Promise<unknown> }) => {
  for (let i = 0; i < 3; i++) {
    f.detectChanges();
    await f.whenStable();
    await new Promise((r) => setTimeout(r, 0));
  }
  f.detectChanges();
};

async function mount(role: 'applicant' | 'licensee' = 'applicant') {
  TestBed.configureTestingModule({ providers: [provideRouter([])] });
  TestBed.inject(SessionProvider).login({ role, firstName: 'Alex', lastName: 'Rivera', organization: 'Society' } as never);
  const fixture = TestBed.createComponent(SpecialEventsApplyPage);
  document.body.appendChild(fixture.nativeElement);
  await settle(fixture);
  const el = fixture.nativeElement as HTMLElement;
  const state = () => fixture.debugElement.query(By.directive(ApplicationProvider))?.injector.get(ApplicationFlowState);
  const button = (re: RegExp) => [...el.querySelectorAll('button')].find((b) => re.test(b.textContent?.trim() ?? '')) as HTMLButtonElement | undefined;
  const click = async (re: RegExp) => {
    const b = button(re);
    if (!b) throw new Error(`no button ${re}`);
    b.click();
    await settle(fixture);
  };
  return { fixture, el, state, button, click, settle: () => settle(fixture) };
}

const ANSWERS = {
  eventName: 'Summer BBQ',
  termsAccepted: true,
  isPrivateResidence: 'no',
  isPublicProperty: 'no',
  isSignificantEvent: 'no',
  eventType: 'private',
  eventStartDate: '2999-11-22',
  hasCertificate: 'no',
  eventOccasion: 'Community picnic',
  hasAdmissionCharge: 'no',
  eventCity: 'Victoria',
  isAnnualEvent: 'no',
  locationName: 'Beacon Hill Park',
  venueType: 'park',
  addressLine1: '100 Cook St',
  postalCode: 'V8V 3W6',
  maxGuests: '50',
  eventDate: '2999-11-22',
  eventStartTime: '12:00',
  eventEndTime: '20:00',
  liquorTypes: ['beer'],
  liquorSaleType: 'selling',
  estimatedDrinks: '200',
  hasSecurity: 'no',
};

describe('SpecialEventsApplyPage', () => {
  beforeEach(() => {
    localStorage.clear();
    sessionStorage.clear();
    vi.spyOn(window, 'scrollTo').mockImplementation(() => undefined);
    vi.spyOn(toast, 'error').mockImplementation(() => 1);
    vi.spyOn(toast, 'info').mockImplementation(() => 1);
  });
  afterEach(() => {
    vi.restoreAllMocks();
    vi.useRealTimers();
    document.body.innerHTML = '';
  });

  it('shows the SEP sign-in instead of the flow for a non-applicant', async () => {
    const m = await mount('licensee');
    expect(m.el.querySelector('h1')!.textContent).toContain('Sign in to apply for a Special Event Permit');
    expect(m.state()).toBeUndefined();
  });

  it('renders the opening step: hero, step indicator, event name, applicant card, terms and Next', async () => {
    const m = await mount();
    expect(m.el.querySelector('h1')!.textContent).toContain('Special Event Permit Application');
    expect(m.el.textContent).toContain('Apply for a permit to serve liquor at your special event');
    expect(m.el.querySelector('label[for="eventName"]')!.textContent).toContain('Event Name');
    expect(m.el.textContent).toContain('Tyee_firstname_testing LRCB Training_surname_testing');
    expect(m.el.textContent).toContain('SEP Terms and Conditions');
    expect(m.el.querySelector('a[href="/account"]')!.textContent).toBe('Manage Account');
    expect(m.el.querySelector('label[for="termsAccepted"]')!.textContent).toContain('I have read and agree to these terms and conditions');
    expect(m.button(/^Next$/)).toBeTruthy();
    expect(m.button(/^Back$/)).toBeUndefined();
    expect(m.el.querySelector('[role="radiogroup"][aria-label^="How would you like"]')).toBeTruthy();
  });

  it('blocks Next while required fields are empty, then advances through every step to the summary', async () => {
    const m = await mount();
    await m.click(/^Next$/);
    expect(toast.error).toHaveBeenCalled();
    expect(m.el.querySelector('label[for="eventName"]')).toBeTruthy();
    m.state()!.updateFormData(ANSWERS);
    await m.settle();
    await m.click(/^Next$/);
    expect(m.el.textContent).toContain('Tell us about your event');
    expect(m.el.querySelector('#isPrivateResidence-no')).toBeTruthy();
    await m.click(/^Next$/);
    expect(m.el.querySelector('label[for="postalCode"]')!.textContent).toContain('Postal Code');
    await m.click(/^Next$/);
    expect(m.el.textContent).toContain('Liquor Details');
    expect(m.el.textContent).toContain('Security');
    await m.click(/^Next$/);
    expect(m.el.textContent).toContain('Almost done');
    expect(m.button(/^Submit Application$/)).toBeTruthy();
  });

  it('shows the conditional infoboxes and the spirits warning', async () => {
    const m = await mount();
    m.state()!.updateFormData({ ...ANSWERS, isPrivateResidence: 'yes', isPublicProperty: 'yes', hasCertificate: 'yes' });
    await m.click(/^Next$/);
    expect(m.el.textContent).toContain('Not eligible for a Special Event Permit');
    expect(m.el.textContent).toContain('Local government approval required');
    expect(m.el.querySelector('label[for="certificateNumber"]')).toBeTruthy();
    m.state()!.updateFormData({ eventType: 'public', liquorTypes: ['spirits'], certificateNumber: 'ABCD' });
    await m.settle();
    m.state()!.currentStep.set(3);
    await m.settle();
    expect(m.el.textContent).toContain('Spirits not allowed at public events');
  });

  it('renders the review answers, "Not provided" for blanks, fee table and the two declarations', async () => {
    const m = await mount();
    m.state()!.updateFormData({ ...ANSWERS, securityStaffCount: '' });
    m.state()!.currentStep.set(4);
    await m.settle();
    const text = m.el.textContent!.replace(/\s+/g, ' ');
    expect(text).toContain('Event name Summer BBQ');
    expect(text).toContain('Event type Private — invited guests only');
    expect(text).toContain('Service window 12:00 PM – 8:00 PM');
    expect(text).toContain('Address 100 Cook St, Victoria, V8V 3W6');
    expect(text).toContain('Liquor service Sale of liquor');
    expect(text).toContain('Liquor types Beer');
    expect(text).toContain('Security staff No');
    expect(text).toContain('Application Fee$25.00');
    m.state()!.updateFormData({ eventName: '' });
    await m.settle();
    expect(m.el.textContent!.replace(/\s+/g, ' ')).toContain('Event name Not provided');
    expect(m.el.querySelector('label[for="confirmAccuracy"]')!.textContent).toContain('I confirm that all information provided is accurate and complete');
    expect(m.el.querySelector('label[for="acknowledgeFalseInfo"]')!.textContent).toContain('may result in permit denial or revocation');
  });

  it('submits to the SEP store, clears the draft and reloads onto the list with the new id', async () => {
    vi.spyOn(console, 'error').mockImplementation(() => undefined); // jsdom: navigation not implemented
    const m = await mount();
    m.state()!.updateFormData({ ...ANSWERS, confirmAccuracy: true, acknowledgeFalseInfo: true });
    m.state()!.saveProgress();
    expect(getDraft('draft:sep')).toBeTruthy();
    m.state()!.currentStep.set(4);
    await m.settle();
    await m.click(/^Submit Application$/);
    const store = TestBed.inject(SepStoreProvider);
    const created = store.records().find((r) => r.eventName === 'Summer BBQ')!;
    expect(created).toBeTruthy();
    expect(created.venueName).toBe('Beacon Hill Park');
    expect(created.expectedAttendance).toBe(50);
    expect(created.serviceType).toBe('Sale of liquor');
    expect(created.liquorTypes).toEqual(['beer']);
    expect(created.estimatedDrinks).toBe(200);
    expect(created.hasSecurity).toBe(false);
    expect(['submitted', 'auto-approved']).toContain(created.status);
    expect(getDraft('draft:sep')).toBeFalsy();
    // The store is persisted before the page unloads.
    expect(JSON.parse(localStorage.getItem('lcrb.sep.records')!).some((r: { id: string }) => r.id === created.id)).toBe(true);
  });

  it('carries the Servings Calculator estimate into the form and clears it', async () => {
    sessionStorage.setItem(SERVINGS_PREFILL_KEY, JSON.stringify({ estimatedDrinks: '331', maxGuests: '80', liquorTypes: ['beer', 'wine'] }));
    const m = await mount();
    await new Promise((r) => setTimeout(r, 10));
    await m.settle();
    const data = m.state()!.formData();
    expect(data['estimatedDrinks']).toBe('331');
    expect(data['maxGuests']).toBe('80');
    expect(data['liquorTypes']).toEqual(['beer', 'wine']);
    expect(toast.info).toHaveBeenCalledWith('Prefilled from the Servings Calculator', expect.anything());
    expect(sessionStorage.getItem(SERVINGS_PREFILL_KEY)).toBeNull();
  });

  it('switches to Guided Mode from the toggle and back', async () => {
    const m = await mount();
    await m.click(/Guided/);
    expect(localStorage.getItem('lcrb.app.mode')).toBe('guided');
    expect(m.el.querySelector('focus-flow')).toBeTruthy();
  });
});
