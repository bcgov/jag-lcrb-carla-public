/** licence-map (never imported by the prototype) creates a Leaflet map with toggleable layers. */
import { TestBed } from '@angular/core/testing';
import { describe, expect, it } from 'vitest';
import { LicenceMap, markerIcon } from './licence-map';
import type { MapLayerConfig } from '@domain/models/map';

const layers: MapLayerConfig[] = [
  {
    id: 'a',
    label: 'Layer A',
    accent: '#111111',
    markers: [{ id: 'a1', lat: 49.28, lng: -123.12, color: '#ff0000', popupHtml: '<b>A1</b>' } as never],
    legend: [{ label: 'Red', color: '#ff0000' }],
  },
  {
    id: 'b',
    label: 'Layer B',
    accent: '#222222',
    markers: [{ id: 'b1', lat: 48.43, lng: -123.36, color: '#00ff00', popupHtml: '<b>B1</b>' } as never],
    legend: [{ label: 'Green', color: '#00ff00' }],
  },
];

describe('markerIcon', () => {
  it('builds a pin div icon in the given colour', () => {
    const icon = markerIcon('#abcdef');
    expect(icon.options.html).toContain('background:#abcdef');
    expect(icon.options.iconAnchor).toEqual([9, 18]);
  });
});

describe('LicenceMap', () => {
  it('renders layer toggles, the legend and one Leaflet marker per active layer', async () => {
    const fixture = TestBed.createComponent(LicenceMap);
    fixture.componentRef.setInput('layers', layers);
    const el = fixture.nativeElement as HTMLElement;
    await fixture.whenStable();
    fixture.detectChanges();
    await fixture.whenStable();
    fixture.detectChanges();
    const container = el.querySelector('[role="application"]') as HTMLElement;
    expect(container.getAttribute('aria-label')).toBe('Map of licensing locations');
    expect(container.classList.contains('leaflet-container')).toBe(true);
    expect(container.style.height).toBe('420px');
    const toggles = Array.from(el.querySelectorAll('button[aria-pressed]')) as HTMLButtonElement[];
    expect(toggles.map((t) => t.textContent?.replace(/\s+/g, ' ').trim())).toEqual(['Layer A (1)', 'Layer B (1)']);
    expect(container.querySelectorAll('.leaflet-marker-icon').length).toBe(2);
    expect(el.textContent).toContain('Red');
    expect(el.textContent).toContain('Green');

    toggles[1].click();
    fixture.detectChanges();
    await fixture.whenStable();
    fixture.detectChanges();
    expect(toggles[1].getAttribute('aria-pressed')).toBe('false');
    expect(container.querySelectorAll('.leaflet-marker-icon').length).toBe(1);
    expect(el.textContent).not.toContain('Green');
  });
});
