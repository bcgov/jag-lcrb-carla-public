/** Name / licence number search with sort, voice input and "Show on Map". */
import { ChangeDetectionStrategy, Component, computed, effect, ElementRef, input, output, signal, viewChild } from '@angular/core';
import { injectAuth } from './map-auth';
import type { FilterState } from './filter-panel';
import { Badge } from '../primitives/badge';
import { Button } from '../primitives/button';
import { Input } from '../primitives/input';
import { LucideIcon } from '../primitives/lucide-icon';
import { SELECT } from '../primitives/select';
import { calculateRiskScore, getAnnualSales, getApplicationAgeDays } from '@domain/map/license-data';
import { type License } from '@domain/models/map';
import { getSpeechRecognitionCtor } from '@shared/util/speech-recognition';

/** Numeric risk weight so licences can be sorted high→low (or low→high). */
const RISK_WEIGHT: Record<'high' | 'medium' | 'low', number> = { high: 3, medium: 2, low: 1 };
function riskValue(license: License): number {
  return RISK_WEIGHT[calculateRiskScore(license.contraventions, license.complaints)];
}

type SortOption =
  | 'default'
  | 'sales-high'
  | 'sales-low'
  | 'age-old'
  | 'age-new'
  | 'contraventions-high'
  | 'contraventions-low'
  | 'risk-high'
  | 'risk-low';

@Component({
  selector: 'search-panel',
  imports: [Badge, Button, Input, LucideIcon, ...SELECT],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    @if (isOpen()) {
      <div class="absolute top-16 left-4 z-20 w-full max-w-sm bg-card rounded-lg shadow-lg border overflow-hidden flex flex-col max-h-[70vh]">
        <div class="p-3 border-b bg-card shrink-0">
          <div class="relative">
            <svg lucide="Search" class="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-muted-foreground"></svg>
            <input
              #searchInput
              uiInput
              [attr.placeholder]="isListening() ? 'Listening...' : 'Search by name or license number...'"
              [value]="query()"
              (input)="query.set($any($event.target).value)"
              [class]="'pl-9 pr-16 ' + (isListening() ? 'border-red-500 bg-red-50/50' : '')"
            />
            <div class="absolute right-1 top-1/2 -translate-y-1/2 flex items-center gap-0.5">
              @if (query()) {
                <button uiButton variant="ghost" size="icon" class="h-7 w-7" (click)="query.set('')">
                  <svg lucide="X" class="w-4 h-4"></svg>
                </button>
              }
              <button
                uiButton
                variant="ghost"
                size="icon"
                [class]="'h-7 w-7 ' + (isListening() ? 'text-red-500 animate-pulse' : 'text-muted-foreground hover:text-foreground')"
                (click)="toggleSpeechRecognition()"
                [attr.title]="isListening() ? 'Stop listening' : 'Voice search'"
              >
                @if (isListening()) {
                  <svg lucide="MicOff" class="w-4 h-4"></svg>
                } @else {
                  <svg lucide="Mic" class="w-4 h-4"></svg>
                }
              </button>
            </div>
          </div>
          @if (hasActiveFilters()) {
            <div class="mt-2 flex items-center gap-1.5 text-xs text-muted-foreground">
              <svg lucide="Filter" class="w-3 h-3"></svg>
              <span>Search results filtered by current map filters</span>
            </div>
          }
        </div>

        @if (query().trim()) {
          @if (allFilteredLicenses().length > 0) {
            <div class="px-4 py-2 border-b bg-muted/30 shrink-0 flex items-center justify-between gap-2">
              <p class="text-xs text-muted-foreground">
                Showing {{ displayedLicenses().length }} of {{ allFilteredLicenses().length }} result{{ allFilteredLicenses().length !== 1 ? 's' : '' }}
              </p>
              <ui-select [value]="sortOption()" (valueChange)="sortOption.set($any($event))">
                <button uiSelectTrigger class="h-7 w-[140px] text-xs bg-background">
                  <svg lucide="ArrowUpDown" class="w-3 h-3 mr-1"></svg>
                  <span uiSelectValue placeholder="Sort by..."></span>
                </button>
                <ng-template uiSelectContent>
                  <ui-select-item value="default">Default</ui-select-item>
                  <ui-select-item value="sales-high">Sales (High to Low)</ui-select-item>
                  <ui-select-item value="sales-low">Sales (Low to High)</ui-select-item>
                  <ui-select-item value="age-old">App Age (Oldest)</ui-select-item>
                  <ui-select-item value="age-new">App Age (Newest)</ui-select-item>
                  <ui-select-item value="contraventions-high">Contraventions (Most)</ui-select-item>
                  <ui-select-item value="contraventions-low">Contraventions (Least)</ui-select-item>
                  <ui-select-item value="risk-high">Risk (Highest)</ui-select-item>
                  <ui-select-item value="risk-low">Risk (Lowest)</ui-select-item>
                </ng-template>
              </ui-select>
            </div>
          }

          <div class="flex-1 min-h-0 overflow-auto">
            @if (allFilteredLicenses().length === 0) {
              <div class="p-4 text-center text-muted-foreground text-sm bg-card">No results found for "{{ query() }}"</div>
            } @else {
              <div class="p-2 bg-card">
                @for (license of displayedLicenses(); track license.id) {
                  <div class="w-full p-3 rounded-lg hover:bg-muted transition-colors bg-card">
                    <div class="flex items-start gap-3">
                      <div
                        [class]="
                          'p-2 rounded-lg shrink-0 ' +
                          (license.licenseType === 'liquor' ? 'bg-[#FFF4ED]' : license.licenseType === 'cannabis' ? 'bg-[#E8F5E9]' : 'bg-[#EDE7F6]')
                        "
                      >
                        @if (license.licenseType === 'liquor') {
                          <svg lucide="Wine" class="w-4 h-4 text-[#C4784A]"></svg>
                        } @else if (license.licenseType === 'cannabis') {
                          <svg lucide="Leaf" class="w-4 h-4 text-[#4A9C6D]"></svg>
                        } @else {
                          <svg lucide="PartyPopper" class="w-4 h-4 text-[#7C4DFF]"></svg>
                        }
                      </div>
                      <div class="flex-1 min-w-0">
                        <div class="flex items-center gap-2 mb-1">
                          <p class="font-medium text-sm truncate">{{ license.businessName }}</p>
                          @if (auth.isLoggedIn() && license.contraventions > 0) {
                            <span uiBadge variant="destructive" class="text-xs shrink-0">
                              {{ license.contraventions }}
                            </span>
                          }
                          @if (license.status === 'application') {
                            <span uiBadge variant="outline" class="text-xs shrink-0 bg-amber-50 text-amber-700 border-amber-200"> Application </span>
                          }
                        </div>
                        <p class="text-xs text-muted-foreground">{{ license.licenseNumber }}</p>
                        <div class="flex items-center gap-1 mt-1 text-xs text-muted-foreground">
                          <svg lucide="MapPin" class="w-3 h-3"></svg>
                          <span class="truncate"> {{ license.address }}, {{ license.city }} </span>
                        </div>
                        <button uiButton variant="outline" size="sm" class="mt-2 w-full text-xs h-8 bg-transparent" (click)="handleShowOnMap(license, $event)">
                          <svg lucide="Navigation" class="w-3 h-3 mr-1.5"></svg>
                          Show on Map
                        </button>
                      </div>
                    </div>
                  </div>
                }
              </div>
            }
          </div>

          @if (hasMoreResults()) {
            <div class="p-3 border-t bg-card shrink-0">
              <button uiButton variant="outline" size="sm" class="w-full bg-transparent" (click)="handleLoadMore()">
                Load More ({{ allFilteredLicenses().length - displayedLicenses().length }} remaining)
              </button>
            </div>
          }
        }
      </div>
    }
  `,
})
export class SearchPanel {
  readonly licenses = input.required<License[]>();
  readonly selectLicense = output<License>();
  readonly isOpen = input.required<boolean>();
  readonly closed = output<void>();
  readonly filters = input<FilterState | undefined>(undefined);

  protected readonly auth = injectAuth();
  protected readonly query = signal('');
  protected readonly isListening = signal(false);
  protected readonly displayCount = signal(20);
  protected readonly sortOption = signal<SortOption>('default');
  private recognition: SpeechRecognition | null = null;
  private readonly searchInput = viewChild<ElementRef<HTMLInputElement>>('searchInput');

  constructor() {
    effect(() => {
      this.query();
      this.displayCount.set(20);
    });
    // React's autoFocus: focus the search box each time the panel opens.
    effect(() => this.searchInput()?.nativeElement.focus());
  }

  protected readonly hasActiveFilters = computed(() => {
    const filters = this.filters();
    if (!filters) return false;
    const defaultFilters = {
      showLiquor: true,
      showCannabis: true,
      showSpecialEvents: true,
      showApplications: true,
      showContraventions: false,
      showSuspended: false,
    };
    return (
      filters.showLiquor !== defaultFilters.showLiquor ||
      filters.showCannabis !== defaultFilters.showCannabis ||
      filters.showSpecialEvents !== defaultFilters.showSpecialEvents ||
      filters.showApplications !== defaultFilters.showApplications ||
      filters.showContraventions !== defaultFilters.showContraventions ||
      filters.showSuspended !== defaultFilters.showSuspended ||
      filters.specialEventDateFrom ||
      filters.specialEventDateTo ||
      (filters.applicationTypes && filters.applicationTypes.length < 13)
    );
  });

  private readonly filteredLicenses = computed(() => {
    const query = this.query();
    const filters = this.filters();
    const { isStaff, canViewSpecialEvents } = { isStaff: this.auth.isStaff(), canViewSpecialEvents: this.auth.canViewSpecialEvents() };
    if (!query.trim()) return [];

    const searchTerm = query.toLowerCase();
    return this.licenses().filter((license) => {
      if (!isStaff && license.status === 'application') return false;
      if (license.licenseType === 'special-event' && !canViewSpecialEvents) return false;

      if (filters) {
        if (license.licenseType === 'liquor' && !filters.showLiquor) return false;
        if (license.licenseType === 'cannabis' && !filters.showCannabis) return false;
        if (license.licenseType === 'special-event') {
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
      }

      return (
        license.businessName.toLowerCase().includes(searchTerm) ||
        license.licenseNumber.toLowerCase().includes(searchTerm) ||
        license.address.toLowerCase().includes(searchTerm) ||
        license.city.toLowerCase().includes(searchTerm)
      );
    });
  });

  protected readonly allFilteredLicenses = computed(() => {
    const sortOption = this.sortOption();
    const filteredLicenses = this.filteredLicenses();
    if (sortOption === 'default') return filteredLicenses;

    const sorted = [...filteredLicenses].sort((a, b) => {
      switch (sortOption) {
        case 'sales-high': {
          const salesA = getAnnualSales(a.monthlySales) || 0;
          const salesB = getAnnualSales(b.monthlySales) || 0;
          return salesB - salesA;
        }
        case 'sales-low': {
          const salesA = getAnnualSales(a.monthlySales) || 0;
          const salesB = getAnnualSales(b.monthlySales) || 0;
          return salesA - salesB;
        }
        case 'age-old': {
          const isAppA = a.status === 'application';
          const isAppB = b.status === 'application';
          if (isAppA && !isAppB) return -1;
          if (!isAppA && isAppB) return 1;
          if (isAppA && isAppB) {
            const ageA = getApplicationAgeDays(a.applicationReceivedDate) || 0;
            const ageB = getApplicationAgeDays(b.applicationReceivedDate) || 0;
            return ageB - ageA;
          }
          return 0;
        }
        case 'age-new': {
          const isAppA = a.status === 'application';
          const isAppB = b.status === 'application';
          if (isAppA && !isAppB) return -1;
          if (!isAppA && isAppB) return 1;
          if (isAppA && isAppB) {
            const ageA = getApplicationAgeDays(a.applicationReceivedDate) || 0;
            const ageB = getApplicationAgeDays(b.applicationReceivedDate) || 0;
            return ageA - ageB;
          }
          return 0;
        }
        case 'contraventions-high': {
          const contA = a.contraventions || 0;
          const contB = b.contraventions || 0;
          return contB - contA;
        }
        case 'contraventions-low': {
          const contA = a.contraventions || 0;
          const contB = b.contraventions || 0;
          return contA - contB;
        }
        case 'risk-high': {
          const riskA = riskValue(a);
          const riskB = riskValue(b);
          return riskB - riskA;
        }
        case 'risk-low': {
          const riskA = riskValue(a);
          const riskB = riskValue(b);
          return riskA - riskB;
        }
        default:
          return 0;
      }
    });

    return sorted;
  });

  protected readonly displayedLicenses = computed(() => this.allFilteredLicenses().slice(0, this.displayCount()));
  protected readonly hasMoreResults = computed(() => this.displayedLicenses().length < this.allFilteredLicenses().length);

  protected toggleSpeechRecognition(): void {
    const SpeechRecognition = getSpeechRecognitionCtor();
    if (!SpeechRecognition) {
      alert('Speech recognition is not supported in your browser. Please use Chrome, Edge, or Safari.');
      return;
    }

    if (this.isListening()) {
      this.recognition?.stop();
      this.isListening.set(false);
      return;
    }

    if (this.recognition) {
      this.recognition.stop();
    }

    const recognition = new SpeechRecognition();
    recognition.continuous = false;
    recognition.interimResults = true;
    recognition.lang = 'en-CA';

    recognition.onresult = (event: SpeechRecognitionEvent) => {
      let finalTranscript = '';
      let interimTranscript = '';

      for (let i = event.resultIndex; i < event.results.length; i++) {
        const transcript = event.results[i][0].transcript;
        if (event.results[i].isFinal) {
          finalTranscript += transcript;
        } else {
          interimTranscript += transcript;
        }
      }

      if (interimTranscript) {
        this.query.set(interimTranscript);
      }

      if (finalTranscript) {
        this.query.set(finalTranscript);
      }
    };

    recognition.onerror = (event: SpeechRecognitionErrorEvent) => {
      console.error('Speech recognition error:', event.error);
      if (event.error === 'not-allowed') {
        alert('Microphone access denied. Please allow microphone access to use voice search.');
      }
      this.isListening.set(false);
    };

    recognition.onend = () => {
      this.isListening.set(false);
    };

    this.recognition = recognition;
    recognition.start();
    this.isListening.set(true);
  }

  protected handleShowOnMap(license: License, e: Event): void {
    e.stopPropagation();
    const panFn = (window as any).__mapPanToLicense;
    if (panFn) {
      panFn(license);
    } else {
      this.selectLicense.emit(license);
    }
    this.query.set('');
    this.closed.emit();
  }

  protected handleLoadMore(): void {
    this.displayCount.update((prev) => prev + 20);
  }
}
