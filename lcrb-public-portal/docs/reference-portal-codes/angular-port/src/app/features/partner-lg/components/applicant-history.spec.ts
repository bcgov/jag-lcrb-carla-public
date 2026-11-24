import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { beforeEach, describe, expect, it } from 'vitest';
import { LG_APPLICATIONS } from '@data-access/repositories/lg-review/mock/lg-applications';
import { ApplicantHistory } from './applicant-history';

async function mount(jobNumber: string) {
  const current = LG_APPLICATIONS.find((a) => a.jobNumber === jobNumber)!;
  const fixture = TestBed.createComponent(ApplicantHistory);
  fixture.componentRef.setInput('current', current);
  fixture.detectChanges();
  await fixture.whenStable();
  fixture.detectChanges();
  return { current, el: fixture.nativeElement as HTMLElement };
}

describe('ApplicantHistory', () => {
  beforeEach(() => {
    localStorage.clear();
    TestBed.configureTestingModule({ providers: [provideRouter([])] });
  });

  it('links the other applications from the same business with who handled them', async () => {
    const { current, el } = await mount('089817'); // B.B. Holding Inc has three referrals
    expect(el.textContent).toContain(`Previous applications — ${current.businessName}`);
    expect(el.textContent).toContain('All representatives');
    const links = [...el.querySelectorAll('li a')].map((a) => a.getAttribute('href'));
    expect(links).toEqual(expect.arrayContaining(['/local-government/review/142536', '/local-government/review/938362']));
    expect(links).not.toContain('/local-government/review/089817');
    expect(el.textContent).toMatch(/Job #142536 · .* · submitted /);
    expect(el.textContent).toMatch(/Handled by |Not yet commented on/);
    expect(el.querySelectorAll('lg-status-badge').length).toBeGreaterThanOrEqual(2);
  });

  it('shows the empty states when nothing else matches', async () => {
    const { current, el } = await mount('560021'); // Harbour Retail Group
    expect(el.textContent).toContain('No other applications from this business or applicant.');
    expect(el.textContent).toContain(`No current licences on record for ${current.businessName}.`);
  });

  it('matches licences by normalised business name', async () => {
    const { el } = await mount('LP-125998'); // LCRB Training Business 5 Ltd.
    expect(el.textContent).toContain('Licences held by this business');
    expect(el.querySelector('[data-slot="badge"]')).toBeTruthy();
  });
});
