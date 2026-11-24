/** the Local Government regional map. */
import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { beforeEach, describe, expect, it } from 'vitest';
import { SessionProvider } from '@core/session/session-provider';
import { LG_APPLICATIONS } from '@data-access/repositories/lg-review/mock/lg-applications';
import { LocalGovernmentMapPage } from './local-government-map.page';

describe('LocalGovernmentMapPage', () => {
  beforeEach(() => {
    localStorage.clear();
    sessionStorage.clear();
    TestBed.configureTestingModule({ providers: [provideRouter([])] });
  });

  it('shows the loading fallback, then the map experience, then the establishment and event lists', async () => {
    TestBed.inject(SessionProvider).login({ role: 'local-government', firstName: 'P', lastName: 'S', organization: 'City of Victoria' } as never);
    const fixture = TestBed.createComponent(LocalGovernmentMapPage);
    const el = fixture.nativeElement as HTMLElement;
    fixture.detectChanges();
    const wrap = el.querySelector('main#main-content .overflow-hidden.rounded-lg')!;
    expect(wrap.textContent).toContain('Loading map…');
    await fixture.whenStable();
    fixture.detectChanges();
    await new Promise((r) => setTimeout(r, 50));
    fixture.detectChanges();
    expect(wrap.textContent).not.toContain('Loading map…');
    expect(wrap.querySelector('map-experience')).toBeTruthy();

    expect(el.querySelector('h1')!.textContent).toBe('Regional Map');
    expect(el.textContent).toContain(`Establishments (${LG_APPLICATIONS.length})`);
    expect(el.textContent).toContain('Special events (');
    const first = LG_APPLICATIONS[0];
    expect(el.querySelector(`main a[href="/local-government/review/${first.jobNumber}"]`)!.textContent).toContain(first.establishmentName);
    expect(el.textContent).toContain('Open application');
    expect(el.textContent).toContain(`${first.riskLevel} risk`);
  });
});
