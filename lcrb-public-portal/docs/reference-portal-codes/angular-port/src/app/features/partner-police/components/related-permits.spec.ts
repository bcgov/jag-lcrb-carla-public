import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { afterEach, beforeEach, describe, expect, it } from 'vitest';
import { SEP_RECORDS } from '@data-access/repositories/special-events/mock/seps';
import { type SepRecord } from '@domain/models/special-events';
import { RelatedPermits } from './related-permits';

const byId = (id: string) => SEP_RECORDS.find((r) => r.id === id)!;

async function mount(current: SepRecord) {
  TestBed.configureTestingModule({ providers: [provideRouter([])] });
  const fixture = TestBed.createComponent(RelatedPermits);
  fixture.componentRef.setInput('current', current);
  document.body.appendChild(fixture.nativeElement);
  fixture.detectChanges();
  await fixture.whenStable();
  return { el: fixture.nativeElement as HTMLElement };
}

describe('RelatedPermits', () => {
  beforeEach(() => localStorage.clear());
  afterEach(() => (document.body.innerHTML = ''));

  it('shows the empty states when nothing else is linked', async () => {
    const { el } = await mount(byId('SEP-2026-04788'));
    expect(el.querySelector('h2')!.textContent).toBe('Permit history');
    expect(el.textContent).toContain('Same applicant / Westshore Rotary Club');
    expect(el.textContent).toContain('First permit from this applicant.');
    expect(el.textContent).toContain('Same venue — Westshore Conference Centre');
    expect(el.textContent).toContain('No other permits on record at this venue.');
    expect(el.querySelectorAll('li')).toHaveLength(0);
  });

  it('lists earlier permits by applicant organization with a review link', async () => {
    const { el } = await mount({ ...byId('SEP-2026-04788'), applicantName: 'Nobody', applicantOrg: 'Fernwood NRG' });
    const rows = el.querySelectorAll('li');
    expect(rows).toHaveLength(1);
    expect(rows[0].querySelector('a')!.getAttribute('href')).toBe('/police/review/SEP-2026-04910');
    expect(rows[0].textContent).toContain('Fernwood Neighbourhood Block Party');
    expect(rows[0].textContent).toContain('1300-block Gladstone Ave (closed), Victoria');
    expect(rows[0].textContent).toContain('expected');
  });

  it('lists permits at the same venue that are not already linked by applicant, with the reviewer', async () => {
    const { el } = await mount({ ...byId('SEP-2026-04788'), applicantName: 'Nobody', applicantOrg: undefined, venueName: 'Cathedral Square' });
    expect(el.textContent).toContain('First permit from this applicant.');
    const rows = el.querySelectorAll('li');
    expect(rows).toHaveLength(1);
    expect(rows[0].textContent).toContain('Cathedral Square Craft Beer Night');
    expect(rows[0].textContent).toContain('Reviewed by Sgt. K. Olsen');
  });
});
