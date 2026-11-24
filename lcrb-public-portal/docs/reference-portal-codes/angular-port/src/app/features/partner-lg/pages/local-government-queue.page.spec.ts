/** the Local Government review queue. */
import { TestBed } from '@angular/core/testing';
import { ActivatedRoute, provideRouter } from '@angular/router';
import { beforeEach, describe, expect, it } from 'vitest';
import { SessionProvider } from '@core/session/session-provider';
import { LG_APPLICATIONS } from '@data-access/repositories/lg-review/mock/lg-applications';
import { LocalGovernmentQueuePage } from './local-government-queue.page';

const LG = { role: 'local-government', firstName: 'Priya', lastName: 'Sandhu', organization: 'City of Victoria', email: 'p@victoria.ca' } as const;

async function settle(fixture: { detectChanges(): void; whenStable(): Promise<unknown> }) {
  fixture.detectChanges();
  await fixture.whenStable();
  await new Promise((r) => setTimeout(r, 0));
  fixture.detectChanges();
}

async function mount(status?: string) {
  TestBed.configureTestingModule({
    providers: [
      provideRouter([]),
      { provide: ActivatedRoute, useValue: { snapshot: { queryParamMap: new Map(status ? [['status', status]] : []) } } },
    ],
  });
  TestBed.inject(SessionProvider).login(LG as never);
  const fixture = TestBed.createComponent(LocalGovernmentQueuePage);
  document.body.appendChild(fixture.nativeElement);
  await settle(fixture);
  return { fixture, el: fixture.nativeElement as HTMLElement };
}
const rowNames = (el: HTMLElement) => [...el.querySelectorAll('tbody tr td:nth-child(3) a')].map((a) => a.textContent!.trim());
const setValue = (input: HTMLInputElement, value: string) => {
  input.value = value;
  input.dispatchEvent(new Event('input'));
};

describe('LocalGovernmentQueuePage', () => {
  beforeEach(() => {
    localStorage.clear();
    sessionStorage.clear();
  });

  it('renders the hero, deadline tools and all applications, newest submitted first', async () => {
    const { el } = await mount();
    expect(el.querySelector('h1')!.textContent).toBe('Applications Awaiting Review');
    expect(el.querySelector('review-queue-tools')!.textContent).toContain('Deadline tools');
    expect(el.textContent).toContain(`Showing ${LG_APPLICATIONS.length} application${LG_APPLICATIONS.length === 1 ? '' : 's'}`);
    const expected = [...LG_APPLICATIONS].sort((a, b) => b.submittedDate.localeCompare(a.submittedDate)).map((a) => a.establishmentName);
    expect(rowNames(el)).toEqual(expected);
    expect(el.querySelector('tbody tr a[href^="/local-government/review/"]')).toBeTruthy();
    expect(el.querySelectorAll('md\\:hidden, .md\\:hidden [data-slot="card"]').length).toBeGreaterThan(0);
  });

  it('starts from the ?status= filter and Clear all resets it', async () => {
    const { fixture, el } = await mount('community-review');
    const expected = LG_APPLICATIONS.filter((a) => a.status === 'community-review').length;
    expect(rowNames(el).length).toBe(expected);
    expect(el.textContent).toContain('Community Review');
    const clear = [...el.querySelectorAll('button')].find((b) => b.textContent!.trim() === 'Clear all')!;
    clear.click();
    await settle(fixture);
    expect(rowNames(el).length).toBe(LG_APPLICATIONS.length);
    expect([...el.querySelectorAll('button')].some((b) => b.textContent!.trim() === 'Clear all')).toBe(false);
  });

  it('searches, shows the empty state, and toggles sort direction from the headings', async () => {
    const { fixture, el } = await mount();
    const target = LG_APPLICATIONS[1];
    setValue(el.querySelector('input')!, target.jobNumber);
    await settle(fixture);
    expect(rowNames(el)).toContain(target.establishmentName);
    expect(el.textContent).toContain('Showing ');

    setValue(el.querySelector('input')!, 'zzzz-no-match');
    await settle(fixture);
    expect(el.textContent).toContain('No applications match your search.');
    el.querySelector<HTMLButtonElement>('button')!; // keep the DOM alive
    [...el.querySelectorAll('button')].find((b) => b.textContent!.trim() === 'Clear all')!.click();
    await settle(fixture);

    const sort = (label: string) => el.querySelector<HTMLButtonElement>(`button[aria-label="Sort by ${label}"]`)!;
    sort('Establishment').click();
    await settle(fixture);
    expect(rowNames(el)).toEqual([...LG_APPLICATIONS].map((a) => a.establishmentName).sort((a, b) => a.localeCompare(b)));
    sort('Establishment').click();
    await settle(fixture);
    expect(rowNames(el)).toEqual([...LG_APPLICATIONS].map((a) => a.establishmentName).sort((a, b) => b.localeCompare(a)));
    expect(sort('Establishment').querySelector('svg')!.getAttribute('class')).toContain('lucide-arrow-down');
  });
});
