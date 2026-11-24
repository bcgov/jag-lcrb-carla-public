/** Map experience specs. map-view reads container size from offsetWidth/offsetHeight, which are 0 in jsdom, so the specs stub them. */
import { TestBed } from '@angular/core/testing';
import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { AuthProvider } from './map-auth';
import { MapExperience } from './map-experience';
import { SessionProvider } from '@core/session/session-provider';
import { allLicenses } from '@data-access/repositories/map/mock/license-data';

const tick = () => new Promise((r) => setTimeout(r, 0));

async function mount(role: 'licensee' | 'lcrb' | 'police' | 'local-government' = 'licensee') {
  TestBed.inject(SessionProvider).login({ role, firstName: 'Pat', lastName: 'Lee', organization: 'Org' } as never);
  const fixture = TestBed.createComponent(MapExperience);
  document.body.appendChild(fixture.nativeElement);
  const settle = async () => {
    for (let i = 0; i < 3; i++) {
      fixture.detectChanges();
      await fixture.whenStable();
      await tick();
    }
    fixture.detectChanges();
  };
  await settle();
  return { fixture, el: fixture.nativeElement as HTMLElement, settle };
}

const buttonByText = (el: ParentNode, text: string) =>
  Array.from(el.querySelectorAll('button')).find((b) => b.textContent?.trim() === text) as HTMLButtonElement | undefined;

function type(input: HTMLInputElement, value: string) {
  input.value = value;
  input.dispatchEvent(new Event('input', { bubbles: true }));
}

beforeEach(() => {
  localStorage.clear();
  Object.defineProperty(HTMLElement.prototype, 'offsetWidth', { configurable: true, get: () => 1200 });
  Object.defineProperty(HTMLElement.prototype, 'offsetHeight', { configurable: true, get: () => 800 });
  TestBed.configureTestingModule({ providers: [AuthProvider] });
});
afterEach(() => {
  delete (HTMLElement.prototype as unknown as Record<string, unknown>)['offsetWidth'];
  delete (HTMLElement.prototype as unknown as Record<string, unknown>)['offsetHeight'];
  document.body.querySelectorAll('.cdk-overlay-container').forEach((c) => (c.innerHTML = ''));
  document.body.querySelectorAll('map-experience').forEach((n) => n.remove());
  vi.restoreAllMocks();
});

describe('MapExperience (licensee)', () => {
  it('renders the header, the tile map, markers from seed data and the legend counts', async () => {
    const { el } = await mount();
    expect(el.querySelector('header h1')?.textContent).toBe('Licensing & Events Map');
    expect(el.firstElementChild?.classList.contains('h-[calc(100vh-3.5rem)]')).toBe(true);
    expect(el.querySelectorAll('img[src^="https://tile.openstreetmap.org/10/"]').length).toBeGreaterThan(0);
    const markers = el.querySelectorAll('.map-marker');
    expect(markers.length).toBeGreaterThan(0);
    // Public view: no special events or applications on the map.
    expect(markers.length).toBeLessThanOrEqual(allLicenses.filter((l) => l.status === 'licensed').length);
    expect(el.textContent).toContain('© OpenStreetMap contributors');
    // Public header controls only.
    expect(buttonByText(el, 'Search')).toBeTruthy();
    expect(buttonByText(el, 'Login')).toBeTruthy();
    for (const name of ['Pipeline', 'Inspections', 'Insights', 'Following', 'Settings']) expect(buttonByText(el, name)).toBeUndefined();
  });

  it('honours heightClassName', async () => {
    TestBed.inject(SessionProvider).login({ role: 'licensee', firstName: 'A', lastName: 'B', organization: 'O' } as never);
    const fixture = TestBed.createComponent(MapExperience);
    fixture.componentRef.setInput('heightClassName', 'h-[600px]');
    await fixture.whenStable();
    const root = (fixture.nativeElement as HTMLElement).firstElementChild!;
    expect([...root.classList].sort()).toEqual(['flex', 'flex-col', 'h-[600px]', 'overflow-hidden', 'w-full']);
  });

  it('opens the popup when a marker is clicked and closes it again', async () => {
    const { el, settle } = await mount();
    (el.querySelector('.map-marker') as HTMLElement).click();
    await settle();
    const popup = el.querySelector('.map-popup');
    expect(popup).toBeTruthy();
    expect(popup!.querySelector('h3')?.textContent).toBeTruthy();
    expect(popup!.textContent).toContain('File a Complaint');
    (popup!.querySelectorAll('button')[0] as HTMLButtonElement).click();
    await settle();
    // Following requires login: the first button is the close button.
    expect(el.querySelector('.map-popup')).toBeNull();
  });

  it('search filters results by name and "Show on Map" selects the licence', async () => {
    const { el, settle } = await mount();
    const target = allLicenses.find((l) => l.status === 'licensed')!;
    buttonByText(el, 'Search')!.click();
    await settle();
    const input = el.querySelector('search-panel input') as HTMLInputElement;
    expect(input).toBeTruthy();
    type(input, target.businessName.toLowerCase());
    await settle();
    const text = el.querySelector('search-panel')!.textContent!;
    expect(text).toContain(target.businessName);
    expect(text).toMatch(/Showing \d+ of \d+ results?/);
    type(input, 'zzzz-no-such-licence');
    await settle();
    expect(el.querySelector('search-panel')!.textContent).toContain('No results found for "zzzz-no-such-licence"');
    type(input, target.businessName.toLowerCase());
    await settle();
    buttonByText(el.querySelector('search-panel')!, 'Show on Map')!.click();
    await settle();
    expect(el.querySelector('search-panel input')).toBeNull();
    expect(el.querySelector('.map-popup')).toBeTruthy();
  });

  it('the filter panel toggles marker types', async () => {
    const { el, settle } = await mount();
    const before = el.querySelectorAll('.map-marker').length;
    buttonByText(el, 'Filter')!.click();
    await settle();
    expect(el.querySelector('filter-panel')!.textContent).toContain('Filter Licenses');
    // A licensee cannot see special events or application filters.
    expect(el.querySelector('#special-events')).toBeNull();
    expect(el.querySelector('#applications')).toBeNull();
    (el.querySelector('#liquor') as HTMLButtonElement).click();
    await settle();
    expect(el.querySelectorAll('.map-marker').length).toBeLessThan(before);
    (el.querySelector('#cannabis') as HTMLButtonElement).click();
    await settle();
    expect(el.querySelectorAll('.map-marker').length).toBe(0);
    buttonByText(el.querySelector('filter-panel')!, 'Reset Filters')!.click();
    await settle();
    expect(el.querySelectorAll('.map-marker').length).toBe(before);
  });

  it('My Location falls back to Vancouver when geolocation fails', async () => {
    const getCurrentPosition = vi.fn((_ok: unknown, fail: (e: { message: string }) => void) => fail({ message: 'denied' }));
    vi.stubGlobal('navigator', { geolocation: { getCurrentPosition } });
    vi.spyOn(console, 'error').mockImplementation(() => undefined);
    const { el, settle } = await mount();
    buttonByText(el, 'My Location')!.click();
    await settle();
    expect(getCurrentPosition).toHaveBeenCalled();
    expect(el.querySelectorAll('.animate-pulse').length).toBeGreaterThan(0);
    vi.unstubAllGlobals();
  });
});

describe('MapExperience role gating', () => {
  it('police see Following (no Insights, Pipeline or Inspections)', async () => {
    const { el, settle } = await mount('police');
    expect(buttonByText(el, 'Following')).toBeTruthy();
    for (const name of ['Insights', 'Pipeline', 'Inspections']) expect(buttonByText(el, name)).toBeUndefined();
    expect(el.textContent).toContain('Cst. Pat Lee');
    buttonByText(el, 'Following')!.click();
    await settle();
    expect(el.querySelector('following-panel')!.textContent).toContain('No followed locations');
    // Police and local government never get the insights panel.
    expect(el.querySelector('insights-panel')).toBeNull();
  });

  it('local government see Following and the special events filter', async () => {
    const { el, settle } = await mount('local-government');
    expect(buttonByText(el, 'Following')).toBeTruthy();
    expect(buttonByText(el, 'Inspections')).toBeUndefined();
    buttonByText(el, 'Filter')!.click();
    await settle();
    expect(el.querySelector('#special-events')).toBeTruthy();
    expect(el.querySelector('#applications')).toBeNull();
  });

  it('LCRB staff see Insights, Pipeline and Inspections (no Following) and can open the pipeline', async () => {
    const { el, settle } = await mount('lcrb');
    for (const name of ['Insights', 'Pipeline', 'Inspections']) expect(buttonByText(el, name)).toBeTruthy();
    expect(buttonByText(el, 'Following')).toBeUndefined();
    buttonByText(el, 'Pipeline')!.click();
    await settle();
    expect(el.querySelector('application-pipeline')!.textContent).toContain('Application Pipeline');
    expect(el.querySelector('application-pipeline')!.textContent).toContain('Application Assessment');
    // Opening Insights closes the pipeline.
    buttonByText(el, 'Insights')!.click();
    await settle();
    expect(el.querySelector('application-pipeline')!.textContent?.trim()).toBe('');
    expect(el.querySelector('insights-panel')!.textContent).toContain('Real-time alerts prioritized by AI');
  });

  it('the inspection planner adds a stop to the scheduled list', async () => {
    const { el, settle } = await mount('lcrb');
    buttonByText(el, 'Inspections')!.click();
    await settle();
    const planner = el.querySelector('inspection-planner')!;
    expect(planner.textContent).toContain('Schedule New Inspection');
    const pick = async (index: number) => {
      (planner.querySelectorAll('button[uiSelectTrigger]')[index] as HTMLButtonElement).click();
      await settle();
      (document.querySelector('.cdk-overlay-container [role="option"]') as HTMLElement).click();
      await settle();
    };
    const add = buttonByText(planner, 'Add')!;
    expect(add.disabled).toBe(true);
    await pick(0); // region
    await pick(1); // establishment
    await pick(2); // inspector
    type(planner.querySelector('input[type="date"]') as HTMLInputElement, '2030-01-15');
    await settle();
    expect(buttonByText(planner, 'Add')!.disabled).toBe(false);
    buttonByText(planner, 'Add')!.click();
    await settle();
    expect(planner.textContent).toContain('Scheduled Inspections');
    expect(planner.textContent).toContain('2030-01-15');
    expect(buttonByText(planner, 'Start Inspection')).toBeTruthy();
    // Starting it opens the active inspection with the checklist.
    buttonByText(planner, 'Start Inspection')!.click();
    await settle();
    expect(planner.textContent).toContain('Inspection Checklist');
    expect(planner.textContent).toContain('00:00:00');
    buttonByText(planner, 'Back to Inspection List')!.click();
    await settle();
    // A paused inspection leaves the New tab list and shows under Soon with a Resume button.
    expect(buttonByText(planner, 'Resume Inspection')).toBeUndefined();
    (Array.from(planner.querySelectorAll('button[role="tab"]')).find((t) => t.textContent?.includes('Soon')) as HTMLButtonElement).click();
    await settle();
    expect(buttonByText(planner, 'Resume Inspection')).toBeTruthy();
  });
});

describe('map login dialog', () => {
  it('signs in with demo credentials and shows the account', async () => {
    const { el, settle } = await mount();
    buttonByText(el, 'Login')!.click();
    await settle();
    const dialog = document.querySelector('.cdk-overlay-container')!;
    expect(dialog.textContent).toContain('LCRB Login');
    type(dialog.querySelector('#email') as HTMLInputElement, 'staff@gov.bc.ca');
    type(dialog.querySelector('#password') as HTMLInputElement, 'wrong');
    (dialog.querySelector('button[type="submit"]') as HTMLButtonElement).click();
    await new Promise((r) => setTimeout(r, 500));
    await settle();
    expect(dialog.textContent).toContain('Invalid credentials. Please try again.');
    type(dialog.querySelector('#password') as HTMLInputElement, 'lcrb2024');
    (dialog.querySelector('button[type="submit"]') as HTMLButtonElement).click();
    await new Promise((r) => setTimeout(r, 500));
    await settle();
    // The map now has a staff user: the Pipeline control appears.
    expect(buttonByText(el, 'Pipeline')).toBeTruthy();
    expect(el.textContent).toContain('Jane Doe');
  });
});
