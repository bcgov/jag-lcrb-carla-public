/** the /map page shows the "Loading map…" fallback, then the lazily loaded experience. */
import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { describe, expect, it } from 'vitest';
import { MapPage } from './map.page';

describe('MapPage', () => {
  it('renders the shell with the loading fallback, then the map experience in the same main', async () => {
    localStorage.clear();
    TestBed.configureTestingModule({ providers: [provideRouter([])] });
    const fixture = TestBed.createComponent(MapPage);
    const el = fixture.nativeElement as HTMLElement;
    fixture.detectChanges();
    const main = el.querySelector('main#main-content')!;
    expect(main.getAttribute('class')).toBe('flex-1');
    expect(el.querySelector('bc-gov-header')).toBeTruthy();
    expect(main.textContent).toContain('Loading map…');
    await fixture.whenStable();
    fixture.detectChanges();
    await new Promise((r) => setTimeout(r, 50));
    fixture.detectChanges();
    expect(main.textContent).not.toContain('Loading map…');
    expect(main.querySelector('map-experience header h1')?.textContent).toBe('Licensing & Events Map');
  });
});
