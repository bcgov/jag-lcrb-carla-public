import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { beforeEach, describe, expect, it } from 'vitest';
import { SessionProvider } from '@core/session/session-provider';
import { LcrbMapPage } from './lcrb-map.page';

const flush = async (fixture: { detectChanges(): void; whenStable(): Promise<unknown> }) => {
  fixture.detectChanges();
  await fixture.whenStable();
  fixture.detectChanges();
  await new Promise((r) => setTimeout(r, 50));
  fixture.detectChanges();
};

describe('LcrbMapPage', () => {
  beforeEach(() => {
    localStorage.clear();
    sessionStorage.clear();
    TestBed.configureTestingModule({ providers: [provideRouter([])] });
  });

  it('shows the loading fallback then the map experience (with the staff role) for LCRB staff', async () => {
    TestBed.inject(SessionProvider).login({ role: 'lcrb', firstName: 'Sam', lastName: 'Rowe', organization: 'LCRB' } as never);
    const fixture = TestBed.createComponent(LcrbMapPage);
    const el = fixture.nativeElement as HTMLElement;
    fixture.detectChanges();
    const main = el.querySelector('main#main-content')!;
    expect(main.getAttribute('class')).toBe('flex-1');
    expect(el.querySelector('bc-gov-header')).toBeTruthy();
    expect(el.querySelector('bc-gov-footer')).toBeNull();
    await fixture.whenStable();
    await flush(fixture);
    await flush(fixture);
    expect(main.textContent).not.toContain('Loading map…');
    expect(main.querySelector('map-experience header h1')?.textContent).toBe('Licensing & Events Map');
  });

  it('is gated: other roles see the access message and the map is never loaded', async () => {
    TestBed.inject(SessionProvider).login({ role: 'licensee', firstName: 'Dana', lastName: 'Lee', organization: 'Cedar' } as never);
    const fixture = TestBed.createComponent(LcrbMapPage);
    await flush(fixture);
    const el = fixture.nativeElement as HTMLElement;
    expect(el.textContent).toContain('LCRB staff access only');
    expect(el.querySelector('map-experience')).toBeNull();
    expect(el.textContent).not.toContain('Loading map…');
  });
});
