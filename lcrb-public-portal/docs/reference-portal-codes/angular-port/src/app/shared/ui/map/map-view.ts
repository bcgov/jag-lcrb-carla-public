/** Dependency-free slippy map with OSM tiles, markers and its own pan/zoom. TODO: use a real viewport geo query and verified role claims instead of the auth mock. */
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
  output,
  signal,
  untracked,
  viewChild,
} from '@angular/core';
import type { FilterState } from './filter-panel';
import { LicensePopup } from './license-popup';
import { injectAuth } from './map-auth';
import type { License } from '@domain/models/map';

const DEFAULT_CENTER = { lat: 49.2827, lng: -123.1207 };
const DEFAULT_ZOOM = 10;
const TILE_SIZE = 256;

@Component({
  selector: 'svg[liquorMarker]',
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: {
    '[attr.width]': 'isSelected() ? 48 : 40',
    '[attr.height]': 'isSelected() ? 48 : 40',
    viewBox: '0 0 40 40',
    fill: 'none',
    xmlns: 'http://www.w3.org/2000/svg',
    '[class]': 'isSelected() ? "drop-shadow-lg" : "drop-shadow-md"',
  },
  template: `
    <svg:circle cx="20" cy="20" r="18" [attr.fill]="bgColor()" [attr.stroke]="fillColor()" [attr.stroke-width]="isSelected() ? 3 : 2" />
    <svg:path d="M20 28V24" [attr.stroke]="fillColor()" stroke-width="2" stroke-linecap="round" />
    <svg:path
      d="M20 24C17 24 14 21 14 17V12H26V17C26 21 23 24 20 24Z"
      [attr.stroke]="fillColor()"
      stroke-width="2"
      stroke-linecap="round"
      stroke-linejoin="round"
    />
    <svg:path d="M15 28H25" [attr.stroke]="fillColor()" stroke-width="2" stroke-linecap="round" />
    <svg:path d="M14 15H26" [attr.stroke]="fillColor()" stroke-width="1.5" stroke-linecap="round" />
    <svg:path d="M15 17C15 20 17 22 20 22C23 22 25 20 25 17V15H15V17Z" [attr.fill]="fillColor()" opacity="0.3" />
    @if (isApplication()) {
      <svg:circle cx="32" cy="8" r="7" fill="#F59E0B" stroke="white" stroke-width="2" />
      <svg:path d="M29 6h6M29 8h6M29 10h4" stroke="white" stroke-width="1" stroke-linecap="round" />
    }
    @if (hasContraventions() && !isApplication()) {
      <svg:circle cx="32" cy="8" r="6" fill="#DC2626" stroke="white" stroke-width="2" />
    }
  `,
})
export class LiquorMarker {
  readonly hasContraventions = input.required<boolean>();
  readonly isApplication = input.required<boolean>();
  readonly isSelected = input.required<boolean>();
  protected readonly fillColor = computed(() => (this.hasContraventions() ? '#DC2626' : '#C4784A'));
  protected readonly bgColor = computed(() => (this.hasContraventions() ? '#FEE2E2' : '#FFF4ED'));
}

@Component({
  selector: 'svg[cannabisMarker]',
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: {
    '[attr.width]': 'isSelected() ? 48 : 40',
    '[attr.height]': 'isSelected() ? 48 : 40',
    viewBox: '0 0 40 40',
    fill: 'none',
    xmlns: 'http://www.w3.org/2000/svg',
    '[class]': 'isSelected() ? "drop-shadow-lg" : "drop-shadow-md"',
  },
  template: `
    <svg:circle cx="20" cy="20" r="18" [attr.fill]="bgColor()" [attr.stroke]="fillColor()" [attr.stroke-width]="isSelected() ? 3 : 2" />
    <svg:path d="M20 30V22" [attr.stroke]="fillColor()" stroke-width="2" stroke-linecap="round" />
    <svg:path
      d="M20 22C20 22 14 20 12 15C14 16 17 16 20 14C23 16 26 16 28 15C26 20 20 22 20 22Z"
      [attr.fill]="fillColor()"
      opacity="0.3"
      [attr.stroke]="fillColor()"
      stroke-width="1.5"
      stroke-linecap="round"
      stroke-linejoin="round"
    />
    <svg:path
      d="M20 14C20 14 16 12 15 8C17 10 19 10 20 10C21 10 23 10 25 8C24 12 20 14 20 14Z"
      [attr.fill]="fillColor()"
      opacity="0.3"
      [attr.stroke]="fillColor()"
      stroke-width="1.5"
      stroke-linecap="round"
      stroke-linejoin="round"
    />
    @if (isApplication()) {
      <svg:circle cx="32" cy="8" r="7" fill="#F59E0B" stroke="white" stroke-width="2" />
      <svg:path d="M29 6h6M29 8h6M29 10h4" stroke="white" stroke-width="1" stroke-linecap="round" />
    }
    @if (hasContraventions() && !isApplication()) {
      <svg:circle cx="32" cy="8" r="6" fill="#DC2626" stroke="white" stroke-width="2" />
    }
  `,
})
export class CannabisMarker {
  readonly hasContraventions = input.required<boolean>();
  readonly isApplication = input.required<boolean>();
  readonly isSelected = input.required<boolean>();
  protected readonly fillColor = computed(() => (this.hasContraventions() ? '#DC2626' : '#22C55E'));
  protected readonly bgColor = computed(() => (this.hasContraventions() ? '#FEE2E2' : '#DCFCE7'));
}

@Component({
  selector: 'svg[specialEventMarker]',
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: {
    '[attr.width]': 'isSelected() ? 48 : 40',
    '[attr.height]': 'isSelected() ? 48 : 40',
    viewBox: '0 0 40 40',
    fill: 'none',
    xmlns: 'http://www.w3.org/2000/svg',
    '[class]': 'isSelected() ? "drop-shadow-lg" : "drop-shadow-md"',
  },
  template: `
    <svg:circle cx="20" cy="20" r="18" fill="#EDE7F6" stroke="#7C4DFF" [attr.stroke-width]="isSelected() ? 3 : 2" />
    <svg:path d="M20 10L12 28H28L20 10Z" fill="#7C4DFF" opacity="0.3" stroke="#7C4DFF" stroke-width="2" stroke-linejoin="round" />
    <svg:path d="M16 22L18.5 15" stroke="#7C4DFF" stroke-width="1.5" stroke-linecap="round" />
    <svg:path d="M20 24L20 12" stroke="#7C4DFF" stroke-width="1.5" stroke-linecap="round" />
    <svg:path d="M24 22L21.5 15" stroke="#7C4DFF" stroke-width="1.5" stroke-linecap="round" />
    <svg:circle cx="20" cy="9" r="2" fill="#F59E0B" />
    <svg:circle cx="10" cy="14" r="1.5" fill="#EC407A" />
    <svg:circle cx="30" cy="14" r="1.5" fill="#29B6F6" />
    <svg:circle cx="8" cy="22" r="1" fill="#66BB6A" />
    <svg:circle cx="32" cy="22" r="1" fill="#FFA726" />
  `,
})
export class SpecialEventMarker {
  readonly isSelected = input.required<boolean>();
}

@Component({
  selector: 'map-view',
  imports: [LicensePopup, LiquorMarker, CannabisMarker, SpecialEventMarker],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <div
      #container
      class="relative w-full h-full overflow-hidden bg-muted select-none"
      style="min-height: 400px"
      [style.cursor]="isDragging() ? 'grabbing' : 'grab'"
      (mousedown)="handleMouseDown($event)"
      (mousemove)="handleMouseMove($event)"
      (mouseup)="handleMouseUp()"
      (mouseleave)="handleMouseUp()"
      (touchstart)="handleTouchStart($event)"
      (touchmove)="handleTouchMove($event)"
      (touchend)="handleTouchEnd()"
    >
      <div class="absolute inset-0 overflow-hidden">
        @for (tile of visibleTiles().tiles; track tile.key) {
          <img
            [src]="'https://tile.openstreetmap.org/' + visibleTiles().tileZoom + '/' + tile.x + '/' + tile.y + '.png'"
            alt=""
            class="absolute"
            [style.left.px]="tile.pixelX"
            [style.top.px]="tile.pixelY"
            [style.width.px]="tileSize"
            [style.height.px]="tileSize"
            style="image-rendering: auto"
            draggable="false"
          />
        }
      </div>

      <div class="absolute inset-0 pointer-events-none">
        @for (marker of markers(); track marker.license.id) {
          <div
            class="map-marker absolute transform -translate-x-1/2 -translate-y-full cursor-pointer transition-transform hover:scale-110 z-10 pointer-events-auto"
            [style.left.px]="marker.x"
            [style.top.px]="marker.y"
            (click)="handleMarkerClick(marker.license, $event)"
          >
            @if (marker.license.licenseType === 'liquor') {
              <svg liquorMarker [hasContraventions]="marker.hasContraventions" [isApplication]="marker.isApplication" [isSelected]="marker.isSelected"></svg>
            } @else if (marker.license.licenseType === 'cannabis') {
              <svg cannabisMarker [hasContraventions]="marker.hasContraventions" [isApplication]="marker.isApplication" [isSelected]="marker.isSelected"></svg>
            } @else {
              <svg specialEventMarker [isSelected]="marker.isSelected"></svg>
            }
          </div>
        }

        @if (userLocationPixel(); as pixel) {
          <div
            class="absolute transform -translate-x-1/2 -translate-y-1/2 z-20 pointer-events-none"
            [style.left.px]="pixel.x"
            [style.top.px]="pixel.y"
          >
            <div class="w-4 h-4 bg-blue-500 border-[3px] border-white rounded-full shadow-lg animate-pulse"></div>
          </div>
        }
      </div>

      <div class="map-controls absolute bottom-4 right-4 flex flex-col gap-1 z-30 pointer-events-auto">
        <button
          (click)="handleZoom(1)"
          class="w-10 h-10 bg-background border border-border rounded-t-md flex items-center justify-center hover:bg-muted transition-colors shadow-md"
        >
          <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <path d="M12 5v14M5 12h14" />
          </svg>
        </button>
        <button
          (click)="handleZoom(-1)"
          class="w-10 h-10 bg-background border border-border rounded-b-md flex items-center justify-center hover:bg-muted transition-colors shadow-md"
        >
          <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <path d="M5 12h14" />
          </svg>
        </button>
      </div>

      @if (selectedLicense() && showPopup()) {
        <div class="map-popup absolute top-4 left-4 z-40 max-w-sm shadow-xl pointer-events-auto">
          <license-popup
            [license]="selectedLicense()!"
            (closed)="closePopup()"
            [isLoggedIn]="auth.isLoggedIn()"
            [isStaff]="auth.isStaff()"
            [isFollowed]="followedLicenses().includes(selectedLicense()!.id)"
            (toggleFollow)="toggleFollow.emit($event)"
            [isPolice]="auth.isPolice()"
            [isLocalGovernment]="auth.isLocalGovernment()"
          />
        </div>
      }

      <div class="absolute bottom-1 left-1 text-[10px] text-muted-foreground bg-background/80 px-1 rounded z-20 pointer-events-none">
        © OpenStreetMap contributors
      </div>
    </div>
  `,
})
export class MapView {
  readonly licenses = input.required<License[]>();
  readonly filters = input.required<FilterState>();
  readonly selectedLicense = input.required<License | null>();
  readonly selectLicense = output<License | null>();
  readonly userLocation = input.required<{ lat: number; lng: number } | null>();
  readonly followedLicenses = input<string[]>([]);
  readonly toggleFollow = output<string>();

  protected readonly auth = injectAuth();
  protected readonly tileSize = TILE_SIZE;
  private readonly container = viewChild.required<ElementRef<HTMLDivElement>>('container');
  private readonly destroyRef = inject(DestroyRef);

  private readonly mapCenter = signal(DEFAULT_CENTER);
  private readonly zoom = signal(DEFAULT_ZOOM);
  private readonly containerSize = signal({ width: 800, height: 600 });
  protected readonly isDragging = signal(false);
  private readonly dragStart = signal({ x: 0, y: 0 });
  protected readonly showPopup = signal(false);
  private readonly shouldPanToSelection = signal(false);
  private prevSelectedId: string | null = null;

  private readonly filteredLicenses = computed(() => {
    const filters = this.filters();
    const { isStaff, canViewSpecialEvents } = this.auth;
    return this.licenses().filter((license) => {
      if (license.licenseType === 'liquor' && !filters.showLiquor) return false;
      if (license.licenseType === 'cannabis' && !filters.showCannabis) return false;
      if (license.licenseType === 'special-event') {
        if (!canViewSpecialEvents()) return false;
        if (!filters.showSpecialEvents) return false;
        if (license.eventDate) {
          const eventDate = new Date(license.eventDate);
          if (filters.specialEventDateFrom) {
            const fromDate = new Date(filters.specialEventDateFrom);
            if (eventDate < fromDate) return false;
          }
          if (filters.specialEventDateTo) {
            const toDate = new Date(filters.specialEventDateTo);
            if (eventDate > toDate) return false;
          }
        }
      }
      if (license.status === 'application') {
        if (!isStaff()) return false;
        if (!filters.showApplications) return false;
        if (license.applicationType && filters.applicationTypes) {
          if (!filters.applicationTypes.includes(license.applicationType)) return false;
        }
      }
      if (filters.showContraventions && license.contraventions === 0) {
        return false;
      }
      if (filters.showSuspended && license.activeStatus !== 'suspended') {
        return false;
      }
      return true;
    });
  });

  private latLngToWorld(lat: number, lng: number): { x: number; y: number } {
    const scale = Math.pow(2, this.zoom()) * TILE_SIZE;
    const x = ((lng + 180) / 360) * scale;
    const siny = Math.sin((lat * Math.PI) / 180);
    const y = (0.5 - Math.log((1 + siny) / (1 - siny)) / (4 * Math.PI)) * scale;
    return { x, y };
  }

  private latLngToPixel(lat: number, lng: number): { x: number; y: number } {
    if (isNaN(lat) || isNaN(lng)) {
      return { x: -1000, y: -1000 };
    }
    const world = this.latLngToWorld(lat, lng);
    const mapCenter = this.mapCenter();
    const containerSize = this.containerSize();
    const centerWorld = this.latLngToWorld(mapCenter.lat, mapCenter.lng);
    return {
      x: containerSize.width / 2 + (world.x - centerWorld.x),
      y: containerSize.height / 2 + (world.y - centerWorld.y),
    };
  }

  private pixelToLatLng(dx: number, dy: number): { lat: number; lng: number } {
    const scale = Math.pow(2, this.zoom()) * TILE_SIZE;
    const mapCenter = this.mapCenter();
    const centerWorld = this.latLngToWorld(mapCenter.lat, mapCenter.lng);

    const newWorldX = centerWorld.x - dx;
    const newWorldY = centerWorld.y - dy;

    const lng = (newWorldX / scale) * 360 - 180;
    const latRad = Math.atan(Math.sinh(Math.PI * (1 - (2 * newWorldY) / scale)));
    const lat = (latRad * 180) / Math.PI;

    return { lat, lng };
  }

  protected readonly visibleTiles = computed(() => {
    const zoom = this.zoom();
    const mapCenter = this.mapCenter();
    const containerSize = this.containerSize();
    const tileZoom = Math.round(zoom);
    const scale = Math.pow(2, tileZoom);

    const tileScale = Math.pow(2, tileZoom) * TILE_SIZE;
    const centerX = ((mapCenter.lng + 180) / 360) * tileScale;
    const siny = Math.sin((mapCenter.lat * Math.PI) / 180);
    const centerY = (0.5 - Math.log((1 + siny) / (1 - siny)) / (4 * Math.PI)) * tileScale;

    const centerTileX = centerX / TILE_SIZE;
    const centerTileY = centerY / TILE_SIZE;

    const tilesX = Math.ceil(containerSize.width / TILE_SIZE) + 2;
    const tilesY = Math.ceil(containerSize.height / TILE_SIZE) + 2;

    const tiles: Array<{ key: string; x: number; y: number; pixelX: number; pixelY: number }> = [];

    const startTileX = Math.floor(centerTileX - tilesX / 2);
    const startTileY = Math.floor(centerTileY - tilesY / 2);

    for (let i = 0; i < tilesX; i++) {
      for (let j = 0; j < tilesY; j++) {
        const tileX = startTileX + i;
        const tileY = startTileY + j;

        const wrappedTileX = ((tileX % scale) + scale) % scale;

        if (tileY < 0 || tileY >= scale) continue;

        const pixelX = containerSize.width / 2 + (tileX * TILE_SIZE - centerX);
        const pixelY = containerSize.height / 2 + (tileY * TILE_SIZE - centerY);

        tiles.push({ key: `${tileZoom}-${wrappedTileX}-${tileY}`, x: wrappedTileX, y: tileY, pixelX, pixelY });
      }
    }

    return { tiles, tileZoom };
  });

  protected readonly markers = computed(() => {
    const containerSize = this.containerSize();
    const selected = this.selectedLicense();
    const isLoggedIn = this.auth.isLoggedIn();
    const result: Array<{ license: License; x: number; y: number; hasContraventions: boolean; isApplication: boolean; isSelected: boolean }> = [];
    for (const license of this.filteredLicenses()) {
      const pos = this.latLngToPixel(license.lat, license.lng);

      if (pos.x < -50 || pos.x > containerSize.width + 50 || pos.y < -50 || pos.y > containerSize.height + 50) {
        continue;
      }

      result.push({
        license,
        x: pos.x,
        y: pos.y,
        hasContraventions: isLoggedIn && license.contraventions > 0,
        isApplication: license.status === 'application',
        isSelected: selected?.id === license.id,
      });
    }
    return result;
  });

  protected readonly userLocationPixel = computed(() => {
    const location = this.userLocation();
    return location ? this.latLngToPixel(location.lat, location.lng) : null;
  });

  constructor() {
    afterNextRender(() => {
      const el = this.container().nativeElement;
      const updateSize = () => {
        this.containerSize.set({ width: el.offsetWidth, height: el.offsetHeight });
      };
      updateSize();
      window.addEventListener('resize', updateSize);
      // React attaches wheel listeners as passive, so preventDefault never stops the page scroll.
      el.addEventListener('wheel', this.handleWheel, { passive: true });
      this.destroyRef.onDestroy(() => {
        window.removeEventListener('resize', updateSize);
        el.removeEventListener('wheel', this.handleWheel);
      });
    });

    effect(() => {
      const selectedLicense = this.selectedLicense();
      const shouldPanToSelection = this.shouldPanToSelection();
      untracked(() => {
        if (selectedLicense && selectedLicense.id !== this.prevSelectedId) {
          if (shouldPanToSelection) {
            this.mapCenter.set({ lat: selectedLicense.lat, lng: selectedLicense.lng });
            this.zoom.set(14);
            this.shouldPanToSelection.set(false);
          }
          this.showPopup.set(true);
          this.prevSelectedId = selectedLicense.id;
        } else if (!selectedLicense) {
          this.prevSelectedId = null;
        }
      });
    });

    effect(() => {
      const userLocation = this.userLocation();
      if (userLocation) {
        untracked(() => {
          this.mapCenter.set(userLocation);
          this.zoom.set(13);
        });
      }
    });

    (window as any).__mapPanToLicense = this.handleExternalSelect;
    this.destroyRef.onDestroy(() => {
      delete (window as any).__mapPanToLicense;
    });
  }

  protected handleZoom(delta: number): void {
    this.zoom.update((prev) => {
      const newZoom = prev + delta;
      return Math.max(8, Math.min(18, newZoom));
    });
  }

  private readonly handleWheel = (e: WheelEvent): void => {
    e.preventDefault();
    const delta = e.deltaY > 0 ? -0.5 : 0.5;
    this.handleZoom(delta);
  };

  protected handleMouseDown(e: MouseEvent): void {
    const target = e.target as HTMLElement;
    if (!target.closest('.map-marker') && !target.closest('.map-popup') && !target.closest('.map-controls')) {
      this.isDragging.set(true);
      this.dragStart.set({ x: e.clientX, y: e.clientY });
      e.preventDefault();
    }
  }

  protected handleMouseMove(e: MouseEvent): void {
    if (!this.isDragging()) return;
    e.preventDefault();

    const dragStart = this.dragStart();
    const dx = e.clientX - dragStart.x;
    const dy = e.clientY - dragStart.y;

    const newCenter = this.pixelToLatLng(dx, dy);
    this.mapCenter.set({
      lat: Math.max(-85, Math.min(85, newCenter.lat)),
      lng: newCenter.lng,
    });

    this.dragStart.set({ x: e.clientX, y: e.clientY });
  }

  protected handleMouseUp(): void {
    this.isDragging.set(false);
  }

  protected handleTouchStart(e: TouchEvent): void {
    const target = e.target as HTMLElement;
    if (!target.closest('.map-marker') && !target.closest('.map-popup') && !target.closest('.map-controls')) {
      const touch = e.touches[0];
      this.isDragging.set(true);
      this.dragStart.set({ x: touch.clientX, y: touch.clientY });
    }
  }

  protected handleTouchMove(e: TouchEvent): void {
    if (!this.isDragging()) return;

    const touch = e.touches[0];
    const dragStart = this.dragStart();
    const dx = touch.clientX - dragStart.x;
    const dy = touch.clientY - dragStart.y;

    const newCenter = this.pixelToLatLng(dx, dy);
    this.mapCenter.set({
      lat: Math.max(-85, Math.min(85, newCenter.lat)),
      lng: newCenter.lng,
    });

    this.dragStart.set({ x: touch.clientX, y: touch.clientY });
  }

  protected handleTouchEnd(): void {
    this.isDragging.set(false);
  }

  protected handleMarkerClick(license: License, e: MouseEvent): void {
    e.stopPropagation();
    this.selectLicense.emit(license);
    this.showPopup.set(true);
  }

  protected closePopup(): void {
    this.showPopup.set(false);
    this.selectLicense.emit(null);
  }

  private readonly handleExternalSelect = (license: License): void => {
    this.shouldPanToSelection.set(true);
    this.selectLicense.emit(license);
  };
}
