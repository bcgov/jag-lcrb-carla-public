/** Staff application pipeline slide-over with a detail panel. */
import { ChangeDetectionStrategy, Component, computed, inject, input, output, signal } from '@angular/core';
import { Badge } from '../primitives/badge';
import { Button } from '../primitives/button';
import { CARD } from '../primitives/card';
import { Input } from '../primitives/input';
import { LucideIcon } from '../primitives/lucide-icon';
import { ScrollArea } from '../primitives/scroll-area';
import { MAP_REPOSITORY } from '@data-access/repositories/map/map.repository';
import { getApplicationPhaseDisplayName, getApplicationTypeDisplayName } from '@domain/map/license-data';
import { type License } from '@domain/models/map';

const PIPELINE_STAGES = [
  { id: 'application-assessment', label: 'Application Assessment', color: 'bg-slate-500' },
  { id: 'incomplete-info-requested', label: 'Incomplete/Info Requested', color: 'bg-amber-500' },
  { id: 'pending-lg-in-approval', label: 'Pending LG/IN Approval', color: 'bg-purple-500' },
  { id: 'under-review', label: 'Under Review', color: 'bg-blue-500' },
  { id: 'approved-in-principle', label: 'Approved in Principle', color: 'bg-green-500' },
];

@Component({
  selector: 'application-pipeline',
  imports: [Badge, Button, ...CARD, Input, LucideIcon, ScrollArea],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    @if (isOpen()) {
      <div class="fixed inset-y-0 right-0 w-full sm:w-[600px] bg-background border-l shadow-xl z-50 flex flex-col mt-14">
        <div class="flex items-center justify-between p-4 border-b shrink-0">
          <div class="flex items-center gap-2">
            <svg lucide="FileText" class="w-5 h-5 text-primary"></svg>
            <h2 class="font-semibold text-lg">Application Pipeline</h2>
          </div>
          <button uiButton variant="ghost" size="icon" (click)="closed.emit()">
            <svg lucide="X" class="w-4 h-4"></svg>
          </button>
        </div>

        <div class="px-4 py-2 bg-blue-50 dark:bg-blue-950/30 border-b text-xs text-blue-700 dark:text-blue-300 flex items-center gap-2 shrink-0">
          <div class="w-2 h-2 bg-blue-500 rounded-full animate-pulse"></div>
          Real-time sync with MS Dynamics
        </div>

        <div class="p-4 border-b shrink-0">
          <div class="relative">
            <svg lucide="Search" class="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-muted-foreground"></svg>
            <input
              uiInput
              placeholder="Search applications or type..."
              [value]="searchQuery()"
              (input)="searchQuery.set($any($event.target).value)"
              class="pl-9"
            />
          </div>
        </div>

        <div class="flex-1 overflow-hidden flex min-h-0">
          <div class="flex-1 overflow-hidden flex flex-col min-h-0">
            <div uiScrollArea class="h-full">
              <div class="p-4 pb-8 space-y-4">
                @for (stage of stages; track stage.id) {
                  <div>
                    <button
                      class="flex items-center gap-2 mb-3 w-full text-left hover:bg-muted/50 rounded p-1 -ml-1 transition-colors"
                      (click)="toggleStageCollapse(stage.id)"
                    >
                      @if (collapsedStages()[stage.id]) {
                        <svg lucide="ChevronRight" class="w-4 h-4 text-muted-foreground"></svg>
                      } @else {
                        <svg lucide="ChevronDown" class="w-4 h-4 text-muted-foreground"></svg>
                      }
                      <div [class]="'w-3 h-3 ' + stage.color + ' rounded-full'"></div>
                      <h3 class="font-medium text-sm">{{ stage.label }}</h3>
                      <span uiBadge variant="secondary" class="text-xs">
                        {{ stageApps(stage.id).length }}
                      </span>
                    </button>
                    @if (!collapsedStages()[stage.id]) {
                      <div class="space-y-2 ml-5">
                        @for (app of displayApps(stage.id); track app.id) {
                          <div
                            uiCard
                            [class]="'cursor-pointer hover:bg-muted/50 transition-colors ' + (selectedApplication()?.id === app.id ? 'ring-2 ring-primary' : '')"
                            (click)="selectedApplication.set(app)"
                          >
                            <div uiCardContent class="p-3">
                              <div class="flex items-center justify-between">
                                <div class="min-w-0">
                                  <h4 class="font-medium text-sm truncate">{{ app.businessName }}</h4>
                                  <p class="text-xs text-muted-foreground flex items-center gap-1">
                                    <svg lucide="MapPin" class="w-3 h-3"></svg>
                                    {{ app.city }}
                                  </p>
                                  @if (app.applicationType) {
                                    <p class="text-xs text-muted-foreground mt-1">
                                      {{ typeName(app.applicationType) }}
                                    </p>
                                  }
                                </div>
                                <span uiBadge variant="outline" class="text-xs shrink-0">
                                  {{ app.licenseType }}
                                </span>
                              </div>
                            </div>
                          </div>
                        }
                        @if (hiddenCount(stage.id) > 0) {
                          <p class="text-xs text-muted-foreground ml-2">+{{ hiddenCount(stage.id) }} more</p>
                        }
                        @if (stageApps(stage.id).length === 0) {
                          <p class="text-xs text-muted-foreground ml-2 italic">No applications</p>
                        }
                      </div>
                    }
                  </div>
                }
              </div>
            </div>
          </div>

          @if (selectedApplication(); as selected) {
            <div class="w-64 border-l bg-muted/30 flex flex-col shrink-0 min-h-0">
              <div class="p-3 border-b flex items-start justify-between gap-2">
                <div class="min-w-0">
                  <h3 class="font-medium text-sm truncate">{{ selected.businessName }}</h3>
                  <p class="text-xs text-muted-foreground">{{ selected.licenseNumber }}</p>
                </div>
                <button uiButton variant="ghost" size="icon" class="h-6 w-6 shrink-0" (click)="selectedApplication.set(null)">
                  <svg lucide="X" class="w-3 h-3"></svg>
                </button>
              </div>

              <div uiScrollArea class="flex-1">
                <div class="p-3 space-y-4">
                  <div>
                    <p class="text-xs font-medium text-muted-foreground mb-1">Location</p>
                    <p class="text-sm">{{ selected.address }}</p>
                    <p class="text-sm text-muted-foreground">{{ selected.city }}, BC</p>
                  </div>

                  <div>
                    <p class="text-xs font-medium text-muted-foreground mb-1">License Type</p>
                    <span uiBadge variant="outline">{{ selected.licenseType }}</span>
                  </div>

                  @if (selected.applicationType) {
                    <div>
                      <p class="text-xs font-medium text-muted-foreground mb-1">Application Type</p>
                      <span uiBadge variant="secondary" class="text-xs">
                        {{ typeName(selected.applicationType) }}
                      </span>
                    </div>
                  }

                  @if (selected.applicationPhase) {
                    <div>
                      <p class="text-xs font-medium text-muted-foreground mb-1">Status</p>
                      <span
                        uiBadge
                        variant="outline"
                        [class]="
                          selected.applicationPhase === 'approved-in-principle'
                            ? 'bg-green-50 text-green-700 border-green-300'
                            : selected.applicationPhase === 'incomplete-info-requested'
                              ? 'bg-amber-50 text-amber-700 border-amber-300'
                              : 'bg-blue-50 text-blue-700 border-blue-300'
                        "
                      >
                        {{ phaseName(selected.applicationPhase) }}
                      </span>
                    </div>
                  }

                  <button uiButton variant="outline" size="sm" class="w-full bg-transparent" (click)="selectLicense.emit(selected); closed.emit()">
                    <svg lucide="MapPin" class="w-3 h-3 mr-1"></svg>
                    Show on Map
                  </button>

                  <div class="pt-3 border-t">
                    <div class="flex items-center gap-2 mb-2">
                      <svg lucide="Sparkles" class="w-4 h-4 text-amber-600"></svg>
                      <p class="text-xs font-medium">AI: Similar Locations</p>
                    </div>
                    @if (similarApplications().length > 0) {
                      <div class="space-y-2">
                        @for (sim of similarApplications(); track sim.id) {
                          <div
                            class="p-2 bg-background rounded border text-xs cursor-pointer hover:bg-muted/50"
                            (click)="selectLicense.emit(sim); closed.emit()"
                          >
                            <p class="font-medium truncate">{{ sim.businessName }}</p>
                            <p class="text-muted-foreground">{{ sim.city }}</p>
                          </div>
                        }
                      </div>
                    } @else {
                      <p class="text-xs text-muted-foreground">No similar locations found</p>
                    }
                  </div>
                </div>
              </div>
            </div>
          }
        </div>
      </div>
    }
  `,
})
export class ApplicationPipeline {
  private readonly map = inject(MAP_REPOSITORY);
  readonly isOpen = input.required<boolean>();
  readonly closed = output<void>();
  readonly selectLicense = output<License>();

  protected readonly stages = PIPELINE_STAGES;
  protected readonly typeName = getApplicationTypeDisplayName;
  protected readonly phaseName = getApplicationPhaseDisplayName;
  protected readonly searchQuery = signal('');
  protected readonly selectedApplication = signal<License | null>(null);
  protected readonly collapsedStages = signal<Record<string, boolean>>({});

  private readonly applications = this.map.licenses().filter((l) => l.status === 'application');

  private getStageForPhase(phase: string | undefined): string {
    if (!phase) return 'application-assessment';
    return phase;
  }

  private matchesSearch(app: License, query: string): boolean {
    if (!query) return true;
    const lowerQuery = query.toLowerCase();

    // Match on application properties
    if (app.businessName.toLowerCase().includes(lowerQuery)) return true;
    if (app.address.toLowerCase().includes(lowerQuery)) return true;
    if (app.city.toLowerCase().includes(lowerQuery)) return true;
    if (app.applicationType && getApplicationTypeDisplayName(app.applicationType)?.toLowerCase().includes(lowerQuery)) return true;
    if (app.licenseNumber.toLowerCase().includes(lowerQuery)) return true;

    // Match on stage name
    const stageName = PIPELINE_STAGES.find((s) => s.id === this.getStageForPhase(app.applicationPhase))?.label || '';
    if (stageName.toLowerCase().includes(lowerQuery)) return true;

    return false;
  }

  private readonly filteredApplications = computed(() => {
    const searchQuery = this.searchQuery();
    if (!searchQuery) return this.applications;
    return this.applications.filter((a) => this.matchesSearch(a, searchQuery));
  });

  private readonly pipelineData = computed<Record<string, License[]>>(() => {
    const filteredApplications = this.filteredApplications();
    const applicationAssessment = filteredApplications.filter((a) => a.applicationPhase === 'application-assessment' || !a.applicationPhase);
    const incompleteInfoRequested = filteredApplications.filter((a) => a.applicationPhase === 'incomplete-info-requested');
    const pendingLgInApproval = filteredApplications.filter((a) => a.applicationPhase === 'pending-lg-in-approval');
    const underReview = filteredApplications.filter((a) => a.applicationPhase === 'under-review');
    const approvedInPrinciple = filteredApplications.filter((a) => a.applicationPhase === 'approved-in-principle');

    return {
      'application-assessment': applicationAssessment,
      'incomplete-info-requested': incompleteInfoRequested,
      'pending-lg-in-approval': pendingLgInApproval,
      'under-review': underReview,
      'approved-in-principle': approvedInPrinciple,
    };
  });

  // Find similar applications
  protected readonly similarApplications = computed(() => {
    const selectedApplication = this.selectedApplication();
    if (!selectedApplication) return [];

    return this.map.licenses()
      .filter(
        (l) =>
          l.id !== selectedApplication.id &&
          (l.city === selectedApplication.city || l.address.split(' ').some((word) => selectedApplication.address.includes(word))) &&
          l.status === 'licensed',
      )
      .slice(0, 5);
  });

  protected stageApps(stageId: string): License[] {
    return this.pipelineData()[stageId] || [];
  }

  // Show all when searching, otherwise limit to 5
  protected displayApps(stageId: string): License[] {
    const apps = this.stageApps(stageId);
    return this.searchQuery().length > 0 ? apps : apps.slice(0, 5);
  }

  protected hiddenCount(stageId: string): number {
    return this.searchQuery().length > 0 ? 0 : Math.max(0, this.stageApps(stageId).length - 5);
  }

  protected toggleStageCollapse(stageId: string): void {
    this.collapsedStages.update((prev) => ({
      ...prev,
      [stageId]: !prev[stageId],
    }));
  }
}
