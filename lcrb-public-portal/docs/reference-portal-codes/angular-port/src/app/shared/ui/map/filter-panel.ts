/** Map filter controls; exports FilterState, the single source of truth for map filters (consumed by MapView). Special-event and enforcement filters are role-gated via injectAuth(). */
import { ChangeDetectionStrategy, Component, computed, input, output, signal } from '@angular/core';
import { injectAuth } from './map-auth';
import { Button } from '../primitives/button';
import { Checkbox } from '../primitives/checkbox';
import { Input } from '../primitives/input';
import { Label } from '../primitives/label';
import { LucideIcon } from '../primitives/lucide-icon';
import { Separator } from '../primitives/separator';
import { getApplicationTypeDisplayName } from '@domain/map/license-data';
import { type LiquorApplicationType } from '@domain/models/map';

export interface FilterState {
  showLiquor: boolean;
  showCannabis: boolean;
  showSpecialEvents: boolean;
  showContraventions: boolean;
  showApplications: boolean;
  showSuspended: boolean;
  specialEventDateFrom: string;
  specialEventDateTo: string;
  applicationTypes: LiquorApplicationType[];
}

export const ALL_APPLICATION_TYPES: LiquorApplicationType[] = [
  'agent',
  'catering',
  'catering-endorsement',
  'catering-license-relocation',
  'family-food-service',
  'food-primary',
  'food-primary-relocation',
  'liquor-establishment-name-change',
  'liquor-license-transfer',
  'permanent-change-to-licensee',
  'liquor-primary',
  'third-party-operator',
  'manufacturer',
];

@Component({
  selector: 'filter-panel',
  imports: [Button, Checkbox, Input, Label, LucideIcon, Separator],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    @if (isOpen()) {
      <div class="absolute top-16 right-4 z-20 w-72 bg-card rounded-lg shadow-lg border max-h-[calc(100vh-5rem)] overflow-y-auto">
        <div class="p-4 border-b flex items-center justify-between sticky top-0 bg-card z-10">
          <h3 class="font-semibold">Filter Licenses</h3>
          <button uiButton variant="ghost" size="icon" (click)="closed.emit()">
            <svg lucide="X" class="w-4 h-4"></svg>
          </button>
        </div>

        <div class="p-4 space-y-4">
          <div>
            <h4 class="text-sm font-medium mb-3">License Type</h4>
            <div class="space-y-3">
              <div class="flex items-center gap-3">
                <button uiCheckbox id="liquor" [checked]="filters().showLiquor" (checkedChange)="updateFilter('showLiquor', !!$event)"></button>
                <label uiLabel for="liquor" class="flex items-center gap-2 cursor-pointer">
                  <div class="p-1.5 bg-[#FFF4ED] rounded">
                    <svg lucide="Wine" class="w-4 h-4 text-[#C4784A]"></svg>
                  </div>
                  <span>Liquor Licenses</span>
                </label>
              </div>
              <div class="flex items-center gap-3">
                <button uiCheckbox id="cannabis" [checked]="filters().showCannabis" (checkedChange)="updateFilter('showCannabis', !!$event)"></button>
                <label uiLabel for="cannabis" class="flex items-center gap-2 cursor-pointer">
                  <div class="p-1.5 bg-[#E8F5E9] rounded">
                    <svg lucide="Leaf" class="w-4 h-4 text-[#4A9C6D]"></svg>
                  </div>
                  <span>Cannabis Licenses</span>
                </label>
              </div>
              @if (auth.canViewSpecialEvents()) {
                <div class="flex items-center gap-3">
                  <button
                    uiCheckbox
                    id="special-events"
                    [checked]="filters().showSpecialEvents"
                    (checkedChange)="updateFilter('showSpecialEvents', !!$event)"
                  ></button>
                  <label uiLabel for="special-events" class="flex items-center gap-2 cursor-pointer">
                    <div class="p-1.5 bg-[#EDE7F6] rounded">
                      <svg lucide="PartyPopper" class="w-4 h-4 text-[#7C4DFF]"></svg>
                    </div>
                    <span>Special Event Permits</span>
                  </label>
                </div>
                @if (filters().showSpecialEvents) {
                  <div class="ml-8 space-y-2 p-3 bg-muted/50 rounded-md">
                    <button
                      (click)="isEventDateExpanded.set(!isEventDateExpanded())"
                      class="flex items-center justify-between w-full text-sm text-muted-foreground hover:text-foreground transition-colors"
                    >
                      <span class="flex items-center gap-2">
                        <svg lucide="Calendar" class="w-3.5 h-3.5"></svg>
                        <span>Event Date Range</span>
                        @if (hasDateFilter()) {
                          <span class="text-xs bg-primary/10 text-primary px-1.5 py-0.5 rounded">Active</span>
                        }
                      </span>
                      @if (isEventDateExpanded()) {
                        <svg lucide="ChevronUp" class="w-4 h-4"></svg>
                      } @else {
                        <svg lucide="ChevronDown" class="w-4 h-4"></svg>
                      }
                    </button>

                    @if (isEventDateExpanded()) {
                      <div class="mt-2 space-y-2">
                        <div>
                          <label uiLabel for="date-from" class="text-xs text-muted-foreground"> From </label>
                          <input
                            uiInput
                            id="date-from"
                            type="date"
                            [value]="filters().specialEventDateFrom"
                            (input)="updateFilter('specialEventDateFrom', $any($event.target).value)"
                            class="h-8 text-sm"
                          />
                        </div>
                        <div>
                          <label uiLabel for="date-to" class="text-xs text-muted-foreground"> To </label>
                          <input
                            uiInput
                            id="date-to"
                            type="date"
                            [value]="filters().specialEventDateTo"
                            (input)="updateFilter('specialEventDateTo', $any($event.target).value)"
                            class="h-8 text-sm"
                          />
                        </div>
                        @if (hasDateFilter()) {
                          <button uiButton variant="ghost" size="sm" class="w-full h-7 text-xs mt-1" (click)="clearDates()">Clear Dates</button>
                        }
                      </div>
                    }
                  </div>
                }
              }
            </div>
          </div>

          <div uiSeparator></div>

          <div>
            <h4 class="text-sm font-medium mb-3">License Status</h4>
            <div class="space-y-3">
              <!-- Contraventions filter for logged-in users -->
              @if (auth.isLoggedIn()) {
                <div class="flex items-center gap-3">
                  <button
                    uiCheckbox
                    id="contraventions"
                    [checked]="filters().showContraventions"
                    (checkedChange)="updateFilter('showContraventions', !!$event)"
                  ></button>
                  <label uiLabel for="contraventions" class="flex items-center gap-2 cursor-pointer">
                    <div class="p-1.5 bg-red-50 rounded">
                      <svg lucide="AlertTriangle" class="w-4 h-4 text-red-500"></svg>
                    </div>
                    <span>With Contraventions</span>
                  </label>
                </div>
              }
              <div class="flex items-center gap-3">
                <button uiCheckbox id="suspended" [checked]="filters().showSuspended" (checkedChange)="updateFilter('showSuspended', !!$event)"></button>
                <label uiLabel for="suspended" class="flex items-center gap-2 cursor-pointer">
                  <div class="p-1.5 bg-red-50 rounded">
                    <svg lucide="Ban" class="w-4 h-4 text-red-500"></svg>
                  </div>
                  <span>Suspended Only</span>
                </label>
              </div>
            </div>
          </div>

          @if (auth.isStaff()) {
            <div uiSeparator></div>

            <!-- Status Filters - Staff only -->
            <div>
              <h4 class="text-sm font-medium mb-3">Application Status</h4>
              <div class="space-y-3">
                <div class="flex items-center gap-3">
                  <button
                    uiCheckbox
                    id="applications"
                    [checked]="filters().showApplications"
                    (checkedChange)="updateFilter('showApplications', !!$event)"
                  ></button>
                  <label uiLabel for="applications" class="flex items-center gap-2 cursor-pointer">
                    <div class="p-1.5 bg-amber-50 rounded">
                      <svg lucide="FileText" class="w-4 h-4 text-amber-500"></svg>
                    </div>
                    <span>Applications</span>
                  </label>
                </div>

                @if (filters().showApplications) {
                  <div class="ml-8 space-y-2 p-3 bg-muted/50 rounded-md">
                    <button
                      (click)="isAppTypesExpanded.set(!isAppTypesExpanded())"
                      class="flex items-center justify-between w-full text-sm text-muted-foreground hover:text-foreground transition-colors"
                    >
                      <span class="flex items-center gap-2">
                        <svg lucide="FileText" class="w-3.5 h-3.5"></svg>
                        <span>Application Types</span>
                        <span class="text-xs bg-muted px-1.5 py-0.5 rounded">{{ currentAppTypes().length }}/{{ allTypes.length }}</span>
                      </span>
                      @if (isAppTypesExpanded()) {
                        <svg lucide="ChevronUp" class="w-4 h-4"></svg>
                      } @else {
                        <svg lucide="ChevronDown" class="w-4 h-4"></svg>
                      }
                    </button>

                    @if (isAppTypesExpanded()) {
                      <div class="mt-2 space-y-2">
                        <div class="flex gap-2 mb-2">
                          <button
                            uiButton
                            variant="outline"
                            size="sm"
                            class="h-6 text-xs flex-1 bg-transparent"
                            (click)="selectAllAppTypes()"
                            [disabled]="allSelected()"
                          >
                            Select All
                          </button>
                          <button
                            uiButton
                            variant="outline"
                            size="sm"
                            class="h-6 text-xs flex-1 bg-transparent"
                            (click)="deselectAllAppTypes()"
                            [disabled]="noneSelected()"
                          >
                            Clear All
                          </button>
                        </div>
                        <div class="space-y-1.5 max-h-48 overflow-y-auto">
                          @for (type of allTypes; track type) {
                            <div class="flex items-center gap-2">
                              <button
                                uiCheckbox
                                [attr.id]="'app-type-' + type"
                                [checked]="currentAppTypes().includes(type)"
                                (checkedChange)="toggleApplicationType(type)"
                                class="h-3.5 w-3.5"
                              ></button>
                              <label uiLabel [attr.for]="'app-type-' + type" class="text-xs cursor-pointer leading-tight">
                                {{ typeName(type) }}
                              </label>
                            </div>
                          }
                        </div>
                      </div>
                    }
                  </div>
                }
              </div>
            </div>
          }
        </div>

        <div class="p-4 border-t sticky bottom-0 bg-card">
          <button uiButton variant="outline" size="sm" class="w-full bg-transparent" (click)="reset()">Reset Filters</button>
        </div>
      </div>
    }
  `,
})
export class FilterPanel {
  readonly filters = input.required<FilterState>();
  readonly filtersChange = output<FilterState>();
  readonly isOpen = input.required<boolean>();
  readonly closed = output<void>();

  protected readonly auth = injectAuth();
  protected readonly isAppTypesExpanded = signal(false);
  protected readonly isEventDateExpanded = signal(false);
  protected readonly allTypes = ALL_APPLICATION_TYPES;
  protected readonly typeName = getApplicationTypeDisplayName;

  protected readonly currentAppTypes = computed(() => this.filters().applicationTypes || ALL_APPLICATION_TYPES);
  protected readonly allSelected = computed(() => this.currentAppTypes().length === ALL_APPLICATION_TYPES.length);
  protected readonly noneSelected = computed(() => this.currentAppTypes().length === 0);
  protected readonly hasDateFilter = computed(() => this.filters().specialEventDateFrom || this.filters().specialEventDateTo);

  protected updateFilter(key: keyof FilterState, value: boolean | string | LiquorApplicationType[]): void {
    this.filtersChange.emit({ ...this.filters(), [key]: value });
  }

  protected toggleApplicationType(type: LiquorApplicationType): void {
    const current = this.filters().applicationTypes || ALL_APPLICATION_TYPES;
    if (current.includes(type)) {
      this.updateFilter(
        'applicationTypes',
        current.filter((t) => t !== type),
      );
    } else {
      this.updateFilter('applicationTypes', [...current, type]);
    }
  }

  protected selectAllAppTypes(): void {
    this.updateFilter('applicationTypes', [...ALL_APPLICATION_TYPES]);
  }

  protected deselectAllAppTypes(): void {
    this.updateFilter('applicationTypes', []);
  }

  protected clearDates(): void {
    this.filtersChange.emit({ ...this.filters(), specialEventDateFrom: '', specialEventDateTo: '' });
  }

  protected reset(): void {
    this.filtersChange.emit({
      showLiquor: true,
      showCannabis: true,
      showSpecialEvents: this.auth.canViewSpecialEvents(),
      showContraventions: false,
      showApplications: this.auth.isStaff(),
      showSuspended: false,
      specialEventDateFrom: '',
      specialEventDateTo: '',
      applicationTypes: [...ALL_APPLICATION_TYPES],
    });
  }
}
