import { TestBed } from '@angular/core/testing';
import { ActivatedRoute, provideRouter } from '@angular/router';
import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { SessionProvider } from '@core/session/session-provider';
import { loadCases } from '@data-access/repositories/staff/lcrb-analytics';
import { stubFetch } from '../../../../../tests/support/fetch-stub';
import { LcrbTriagePage } from './lcrb-triage.page';

async function mount(role: 'lcrb' | 'police' = 'lcrb', caseParam: string | null = null) {
  TestBed.configureTestingModule({
    providers: [
      provideRouter([]),
      { provide: ActivatedRoute, useValue: { snapshot: { queryParamMap: { get: (k: string) => (k === 'case' ? caseParam : null) } } } },
    ],
  });
  TestBed.inject(SessionProvider).login({ role, firstName: 'Sam', lastName: 'Rowe', organization: 'LCRB' } as never);
  const fixture = TestBed.createComponent(LcrbTriagePage);
  document.body.appendChild(fixture.nativeElement);
  for (let i = 0; i < 4; i++) {
    fixture.detectChanges();
    await fixture.whenStable();
    await new Promise((r) => setTimeout(r, 0));
  }
  fixture.detectChanges();
  return { fixture, el: fixture.nativeElement as HTMLElement };
}

describe('LcrbTriagePage', () => {
  beforeEach(() => {
    localStorage.clear();
    sessionStorage.clear();
    stubFetch({});
  });
  afterEach(() => {
    vi.unstubAllGlobals();
    document.body.innerHTML = '';
  });

  it('renders the hero and the triage view with every case', async () => {
    const { el } = await mount();
    expect(el.querySelector('h1')?.textContent).toContain('Case Triage');
    expect(el.textContent).toContain('Cases pre-screened by the AI agent');
    expect(el.querySelector('nav[aria-label], nav')?.textContent).toContain('Intake Overview');
    expect(el.querySelectorAll('[triageCaseRow]')).toHaveLength(loadCases().length);
    expect(document.querySelector('ui-sheet-content')).toBeNull();
  });

  it('opens the drawer for the ?case= query parameter', async () => {
    const cases = loadCases();
    const { el } = await mount('lcrb', cases[0].id);
    expect(el.querySelectorAll('[triageCaseRow]').length).toBeGreaterThan(0);
    expect(document.querySelector('ui-sheet-content')?.textContent).toContain(cases[0].reference);
  });

  it('is gated for non-staff roles', async () => {
    const { el } = await mount('police');
    expect(el.textContent).toContain('LCRB staff access only');
    expect(el.querySelector('lcrb-triage-content')).toBeNull();
  });
});
