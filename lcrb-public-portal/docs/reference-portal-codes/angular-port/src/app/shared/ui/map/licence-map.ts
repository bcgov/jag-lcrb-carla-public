/** Generic role-aware Leaflet map with marker layers on OpenStreetMap tiles. Unused, as in the prototype (pages use MapExperience); loads Leaflet's CSS. */
import {
  afterNextRender,
  ChangeDetectionStrategy,
  Component,
  computed,
  DestroyRef,
  effect,
  ElementRef,
  inject,
  input,
  signal,
  untracked,
  viewChild,
  ViewEncapsulation,
} from '@angular/core';
import L from 'leaflet';
import { LucideIcon } from '../primitives/lucide-icon';
import type { MapLayerConfig } from '@domain/models/map';

export function markerIcon(color: string): L.DivIcon {
  return L.divIcon({
    className: '',
    html: `<span style="
      display:block;width:18px;height:18px;border-radius:50% 50% 50% 0;
      background:${color};transform:rotate(-45deg);
      border:2px solid #fff;box-shadow:0 1px 3px rgba(0,0,0,0.4);"></span>`,
    iconSize: [18, 18],
    iconAnchor: [9, 18],
    popupAnchor: [0, -18],
  });
}

@Component({
  selector: 'licence-map',
  imports: [LucideIcon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  encapsulation: ViewEncapsulation.None,
  styleUrl: '../../../../../node_modules/leaflet/dist/leaflet.css',
  host: { style: 'display: contents' },
  template: `
    <div class="space-y-3">
      @if (showToggles()) {
        <div class="flex flex-wrap items-center gap-2">
          <span class="flex items-center gap-1.5 text-sm font-medium text-foreground">
            <svg lucide="Layers" class="h-4 w-4" aria-hidden="true"></svg>
            Layers
          </span>
          @for (l of layers(); track l.id) {
            <button
              type="button"
              (click)="toggleLayer(l.id)"
              [attr.aria-pressed]="activeLayers().includes(l.id)"
              [class]="
                'flex items-center gap-1.5 rounded-full border px-3 py-1 text-xs font-medium transition-colors ' +
                (activeLayers().includes(l.id)
                  ? 'border-[#013366] bg-[#013366] text-white'
                  : 'border-border bg-background text-muted-foreground hover:bg-muted')
              "
            >
              <span class="h-2.5 w-2.5 rounded-full" [style.background-color]="l.accent" aria-hidden="true"></span>
              {{ l.label }}
              <span [class]="activeLayers().includes(l.id) ? 'text-white/70' : 'text-muted-foreground/70'">({{ l.markers.length }})</span>
            </button>
          }
        </div>
      }

      <div #container [style.height]="height()" class="w-full rounded-lg border border-border" role="application" [attr.aria-label]="ariaLabel()"></div>

      @if (legend().length > 0) {
        <div class="flex flex-wrap items-center gap-x-4 gap-y-1.5 text-xs text-muted-foreground">
          @for (e of legend(); track e.label + '-' + e.color) {
            <span class="flex items-center gap-1.5">
              <span class="h-2.5 w-2.5 rounded-full" [style.background-color]="e.color" aria-hidden="true"></span>
              {{ e.label }}
            </span>
          }
        </div>
      }
    </div>
  `,
})
export class LicenceMap {
  readonly layers = input.required<MapLayerConfig[]>();
  readonly selectedId = input<string | undefined>(undefined);
  readonly height = input('420px');
  readonly ariaLabel = input('Map of licensing locations');
  readonly center = input<[number, number]>([49.5, -123.5]);
  readonly zoom = input(6);

  private readonly container = viewChild.required<ElementRef<HTMLDivElement>>('container');
  private readonly map = signal<L.Map | null>(null);
  private markers: Record<string, L.Marker> = {};

  // Which layers are currently visible (default: all).
  protected readonly activeLayers = signal<string[]>([]);

  private readonly visibleMarkers = computed(() =>
    this.layers()
      .filter((l) => this.activeLayers().includes(l.id))
      .flatMap((l) => l.markers),
  );

  protected readonly showToggles = computed(() => this.layers().length > 1);
  protected readonly legend = computed(() => {
    const entries: { label: string; color: string }[] = [];
    const seen = new Set<string>();
    this.layers()
      .filter((l) => this.activeLayers().includes(l.id))
      .forEach((l) =>
        (l.legend ?? []).forEach((e) => {
          const key = `${e.label}-${e.color}`;
          if (!seen.has(key)) {
            seen.add(key);
            entries.push(e);
          }
        }),
      );
    return entries;
  });

  constructor() {
    const destroyRef = inject(DestroyRef);

    // Keep active layers in sync if the set of layer ids changes (first run: all visible).
    effect(() => {
      const ids = this.layers().map((l) => l.id);
      untracked(() => {
        this.activeLayers.update((prev) => {
          const next = prev.filter((id) => ids.includes(id));
          // Add any brand-new layers as visible by default.
          ids.forEach((id) => {
            if (!prev.includes(id)) next.push(id);
          });
          return next.length ? next : ids;
        });
      });
    });

    // Initialize the map once the container exists.
    afterNextRender(() => {
      const map = L.map(this.container().nativeElement, { center: this.center(), zoom: this.zoom(), scrollWheelZoom: false });
      L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '&copy; OpenStreetMap contributors',
        maxZoom: 19,
      }).addTo(map);
      this.map.set(map);
      // The container may still be settling its layout; recalculate pane positions on the next frame so fitBounds/setView don't hit an undefined `_leaflet_pos`.
      const raf = requestAnimationFrame(() => map.invalidateSize(false));
      destroyRef.onDestroy(() => {
        cancelAnimationFrame(raf);
        map.remove();
        this.map.set(null);
      });
    });

    // Sync markers when the visible markers change.
    effect(() => {
      const map = this.map();
      const visibleMarkers = this.visibleMarkers();
      if (!map) return;

      Object.values(this.markers).forEach((m) => m.remove());
      this.markers = {};

      if (visibleMarkers.length === 0) return;

      const bounds = L.latLngBounds([]);
      visibleMarkers.forEach((m) => {
        const marker = L.marker([m.lat, m.lng], { icon: markerIcon(m.color) }).addTo(map);
        marker.bindPopup(m.popupHtml);
        this.markers[m.id] = marker;
        bounds.extend([m.lat, m.lng]);
      });

      if (bounds.isValid()) {
        map.fitBounds(bounds, { padding: [40, 40], maxZoom: 12, animate: false });
      }
    });

    // Open popup for the selected marker.
    effect(() => {
      const selectedId = this.selectedId();
      const map = this.map();
      if (!selectedId) return;
      const marker = this.markers[selectedId];
      if (marker && map) {
        map.setView(marker.getLatLng(), 12, { animate: false });
        marker.openPopup();
      }
    });
  }

  protected toggleLayer(id: string): void {
    this.activeLayers.update((prev) => (prev.includes(id) ? prev.filter((x) => x !== id) : [...prev, id]));
  }
}

export default LicenceMap;
