/** Embeddable Licensing & Events map; roles come from map-auth, so the host page lists `AuthProvider`. `heightClassName` bounds embedded height. */
import { ChangeDetectionStrategy, Component, computed, inject, input, signal } from '@angular/core';
import { ApplicationPipeline } from './application-pipeline';
import { ALL_APPLICATION_TYPES, FilterPanel, type FilterState } from './filter-panel';
import { FollowingPanel } from './following-panel';
import { InsightsPanel } from './insights-panel';
import { InspectionPlanner } from './inspection-planner';
import { injectAuth } from './map-auth';
import { MapHeader } from './map-header';
import { MapLegend } from './map-legend';
import { MapView } from './map-view';
import { SearchPanel } from './search-panel';
import { MAP_REPOSITORY } from '@data-access/repositories/map/map.repository';
import { type License } from '@domain/models/map';

@Component({
  selector: 'map-experience',
  imports: [MapHeader, MapView, SearchPanel, FilterPanel, InspectionPlanner, MapLegend, InsightsPanel, ApplicationPipeline, FollowingPanel],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <div [class]="'flex w-full flex-col overflow-hidden ' + heightClassName()">
      <map-header
        (searchToggle)="isSearchOpen.set(!isSearchOpen()); isFilterOpen.set(false)"
        (filterToggle)="isFilterOpen.set(!isFilterOpen()); isSearchOpen.set(false)"
        (inspectionPlannerToggle)="handleInspectionPlannerToggle()"
        (locateUser)="handleLocateUser()"
        (insightsToggle)="handleInsightsToggle()"
        (pipelineToggle)="handlePipelineToggle()"
        (followingToggle)="handleFollowingToggle()"
        [isSearchOpen]="isSearchOpen()"
        [isFilterOpen]="isFilterOpen()"
        [isInspectionPlannerOpen]="isInspectionPlannerOpen()"
        [isInsightsOpen]="isInsightsOpen()"
        [isPipelineOpen]="isPipelineOpen()"
        [isFollowingOpen]="isFollowingOpen()"
      />

      <main class="relative flex-1">
        <map-view
          [licenses]="licenses"
          [filters]="filters()"
          [selectedLicense]="selectedLicense()"
          (selectLicense)="handleSelectLicense($event)"
          [userLocation]="userLocation()"
          [followedLicenses]="followedLicenses()"
          (toggleFollow)="handleToggleFollow($event)"
        />

        <search-panel
          [licenses]="licenses"
          (selectLicense)="handleSelectLicense($event)"
          [isOpen]="isSearchOpen()"
          (closed)="isSearchOpen.set(false)"
          [filters]="filters()"
        />

        <filter-panel [filters]="filters()" (filtersChange)="filters.set($event)" [isOpen]="isFilterOpen()" (closed)="isFilterOpen.set(false)" />

        <map-legend
          [totalLiquor]="stats().totalLiquor"
          [totalCannabis]="stats().totalCannabis"
          [totalSpecialEvents]="stats().totalSpecialEvents"
          [totalWithContraventions]="stats().totalWithContraventions"
          [totalApplications]="stats().totalApplications"
        />

        @if (auth.isStaff()) {
          <inspection-planner
            [isOpen]="isInspectionPlannerOpen()"
            (closed)="isInspectionPlannerOpen.set(false)"
            [licenses]="licenses"
            (navigateToLicense)="handleNavigateToLicense($event)"
          />
        }

        @if (!auth.isPolice() && !auth.isLocalGovernment()) {
          <insights-panel
            [isOpen]="isInsightsOpen()"
            (closed)="isInsightsOpen.set(false)"
            [followedLicenses]="followedLicenses()"
            (selectLicense)="handleNavigateToLicense($event)"
          />
        }

        @if (auth.isPolice() || auth.isLocalGovernment()) {
          <following-panel
            [isOpen]="isFollowingOpen()"
            (closed)="isFollowingOpen.set(false)"
            [followedLicenses]="followedLicenses()"
            (selectLicense)="handleNavigateToLicense($event)"
            (toggleFollow)="handleToggleFollow($event)"
          />
        }

        <application-pipeline [isOpen]="isPipelineOpen()" (closed)="isPipelineOpen.set(false)" (selectLicense)="handleNavigateToLicense($event)" />
      </main>
    </div>
  `,
})
export class MapExperience {
  private readonly map = inject(MAP_REPOSITORY);
  readonly heightClassName = input('h-[calc(100vh-3.5rem)]');

  protected readonly auth = injectAuth();
  protected readonly licenses = this.map.licenses();
  protected readonly selectedLicense = signal<License | null>(null);
  protected readonly userLocation = signal<{ lat: number; lng: number } | null>(null);
  protected readonly isSearchOpen = signal(false);
  protected readonly isFilterOpen = signal(false);
  protected readonly isInspectionPlannerOpen = signal(false);
  protected readonly isInsightsOpen = signal(false);
  protected readonly isPipelineOpen = signal(false);
  protected readonly isFollowingOpen = signal(false);
  protected readonly followedLicenses = signal<string[]>([]);

  protected readonly filters = signal<FilterState>({
    showLiquor: true,
    showCannabis: true,
    showSpecialEvents: true,
    showContraventions: false,
    showApplications: true,
    showSuspended: false,
    specialEventDateFrom: '',
    specialEventDateTo: '',
    applicationTypes: [...ALL_APPLICATION_TYPES],
  });

  protected readonly stats = computed(() => {
    const isStaff = this.auth.isStaff();
    const canViewSpecialEvents = this.auth.canViewSpecialEvents();
    const visibleLicenses = this.map.licenses().filter((l) => {
      if (l.licenseType === 'special-event' && !canViewSpecialEvents) return false;
      if (!isStaff && l.status === 'application') return false;
      return true;
    });
    return {
      totalLiquor: visibleLicenses.filter((l) => l.licenseType === 'liquor' && l.status === 'licensed').length,
      totalCannabis: visibleLicenses.filter((l) => l.licenseType === 'cannabis' && l.status === 'licensed').length,
      totalSpecialEvents: visibleLicenses.filter((l) => l.licenseType === 'special-event').length,
      totalWithContraventions: visibleLicenses.filter((l) => l.contraventions > 0).length,
      totalApplications: this.map.licenses().filter((l) => l.status === 'application').length,
    };
  });

  protected handleLocateUser(): void {
    if ('geolocation' in navigator) {
      navigator.geolocation.getCurrentPosition(
        (position) => {
          this.userLocation.set({ lat: position.coords.latitude, lng: position.coords.longitude });
        },
        (error) => {
          console.error('Geolocation error:', error.message);
          this.userLocation.set({ lat: 49.2827, lng: -123.1207 });
        },
        { enableHighAccuracy: true, timeout: 10000, maximumAge: 0 },
      );
    } else {
      this.userLocation.set({ lat: 49.2827, lng: -123.1207 });
    }
  }

  protected handleSelectLicense(license: License | null): void {
    this.selectedLicense.set(license);
  }

  protected handleNavigateToLicense(license: License): void {
    this.selectedLicense.set(license);
    this.isInspectionPlannerOpen.set(false);
    this.isInsightsOpen.set(false);
    this.isPipelineOpen.set(false);
    this.isFollowingOpen.set(false);
  }

  protected handleToggleFollow(licenseId: string): void {
    this.followedLicenses.update((prev) => (prev.includes(licenseId) ? prev.filter((id) => id !== licenseId) : [...prev, licenseId]));
  }

  protected handleInsightsToggle(): void {
    const wasOpen = this.isInsightsOpen();
    this.isInsightsOpen.set(!wasOpen);
    if (!wasOpen) {
      this.isPipelineOpen.set(false);
      this.isInspectionPlannerOpen.set(false);
      this.isFollowingOpen.set(false);
    }
  }

  protected handlePipelineToggle(): void {
    const wasOpen = this.isPipelineOpen();
    this.isPipelineOpen.set(!wasOpen);
    if (!wasOpen) {
      this.isInsightsOpen.set(false);
      this.isInspectionPlannerOpen.set(false);
      this.isFollowingOpen.set(false);
    }
  }

  protected handleInspectionPlannerToggle(): void {
    const wasOpen = this.isInspectionPlannerOpen();
    this.isInspectionPlannerOpen.set(!wasOpen);
    if (!wasOpen) {
      this.isInsightsOpen.set(false);
      this.isPipelineOpen.set(false);
      this.isFollowingOpen.set(false);
    }
  }

  protected handleFollowingToggle(): void {
    const wasOpen = this.isFollowingOpen();
    this.isFollowingOpen.set(!wasOpen);
    if (!wasOpen) {
      this.isInsightsOpen.set(false);
      this.isPipelineOpen.set(false);
      this.isInspectionPlannerOpen.set(false);
    }
  }
}
