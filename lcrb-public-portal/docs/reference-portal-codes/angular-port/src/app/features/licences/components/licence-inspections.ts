/** Per-licence inspections panel: upcoming visits with add-to-calendar, then closed inspection history. */
import { ChangeDetectionStrategy, Component, computed, inject, input } from '@angular/core';
import { Badge } from '@shared/ui/primitives/badge';
import { LucideIcon } from '@shared/ui/primitives/lucide-icon';
import { daysUntil, formatEnforcementDate, OUTCOME_META } from '@domain/compliance/enforcement';
import { formatInspectionDateTime, INSPECTION_KIND_META, INSPECTION_STATUS_META } from '@domain/licences/inspections';
import { type ScheduledInspection } from '@domain/models/licences';
import { downloadICS } from '@shared/util/compliance-ics-download';
import { type CalendarEvent } from '@domain/models/compliance';
import { LICENCES_REPOSITORY } from '@data-access/repositories/licences/licences.repository';
import { path } from '@/app/route-registry';

function countdownLabel(iso: string): { text: string; tone: string } {
  const d = daysUntil(iso);
  if (d < 0) return { text: `${Math.abs(d)} day${Math.abs(d) === 1 ? '' : 's'} ago`, tone: 'text-muted-foreground' };
  if (d === 0) return { text: 'Today', tone: 'text-red-700 font-semibold' };
  if (d <= 14) return { text: `In ${d} day${d === 1 ? '' : 's'}`, tone: 'text-orange-700 font-medium' };
  return { text: `In ${d} days`, tone: 'text-muted-foreground' };
}

/** Convert a scheduled inspection into the shared calendar event shape. */
function inspectionToCalendarEvent(inspection: ScheduledInspection): CalendarEvent {
  return {
    id: inspection.id,
    type: 'inspection',
    title: `${INSPECTION_KIND_META[inspection.kind].label} inspection — ${inspection.establishmentName}`,
    date: inspection.scheduledDate,
    description: `${inspection.window}. Conducted by ${inspection.office}${
      inspection.officer ? ` (${inspection.officer})` : ''
    }. Focus: ${inspection.focusAreas.join('; ')}.`,
    licenceId: inspection.licenceId,
    establishmentName: inspection.establishmentName,
    href: path('licences.detail', { id: inspection.licenceId }) + '?tab=inspections',
  };
}

@Component({
  selector: 'licence-inspections',
  imports: [Badge, LucideIcon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <div class="space-y-8">
      <!-- Context banner -->
      <div class="flex items-start gap-3 rounded-lg border border-blue-200 bg-blue-50 p-4">
        <svg lucide="ShieldCheck" class="h-5 w-5 text-[#013366] mt-0.5 shrink-0" aria-hidden="true"></svg>
        <p class="text-sm text-[#013366] leading-relaxed">
          Inspections are carried out by <strong>Compliance &amp; Enforcement</strong>. Scheduled visits for this licence appear
          below. Inspectors may also attend unannounced; keeping your licence, floor plan, and staff certifications current is
          the best way to be ready.
        </p>
      </div>

      <!-- Upcoming -->
      <section aria-label="Upcoming inspections">
        <div class="flex items-center gap-2 mb-4">
          <svg lucide="CalendarClock" class="h-5 w-5 text-[#013366]" aria-hidden="true"></svg>
          <h2 class="text-lg font-semibold text-foreground">Scheduled inspections</h2>
          <span uiBadge variant="outline" class="ml-1 bg-white">{{ upcoming().length }}</span>
        </div>

        @if (upcoming().length === 0) {
          <div class="text-center py-10 bg-white border rounded-lg">
            <svg lucide="CalendarClock" class="h-10 w-10 mx-auto text-muted-foreground mb-3" aria-hidden="true"></svg>
            <p class="text-muted-foreground">No inspections are currently scheduled for this licence.</p>
          </div>
        } @else {
          <div class="space-y-4">
            @for (inspection of upcoming(); track inspection.id) {
              @let kind = kindMeta(inspection);
              @let statusMeta = statusMetaOf(inspection);
              @let countdown = countdownOf(inspection.scheduledDate);
              @let d = dateOf(inspection.scheduledDate);
              <div class="flex gap-4 rounded-lg border border-l-4 border-l-[#013366] bg-white p-4">
                <!-- Date block -->
                <div class="shrink-0 w-14 text-center" aria-hidden="true">
                  <div class="text-xs font-medium uppercase text-muted-foreground">{{ month(d) }}</div>
                  <div class="text-2xl font-bold text-[#013366] leading-none">{{ d.getDate() }}</div>
                </div>

                <div class="flex-1 min-w-0">
                  <div class="flex flex-wrap items-center gap-2">
                    <span uiBadge variant="outline" [class]="kind.chip">{{ kind.label }}</span>
                    <span uiBadge variant="outline" [class]="statusMeta.chip">{{ statusMeta.label }}</span>
                    <span [class]="'text-xs ' + countdown.tone">{{ countdown.text }}</span>
                  </div>

                  <h3 class="font-semibold text-foreground mt-2">{{ dateTime(inspection.scheduledDate) }}</h3>
                  <p class="text-sm text-muted-foreground">{{ inspection.window }}</p>

                  <div class="mt-3 grid grid-cols-1 sm:grid-cols-2 gap-x-6 gap-y-1.5 text-sm">
                    <div class="flex items-center gap-2 text-muted-foreground">
                      <svg lucide="MapPin" class="h-4 w-4 shrink-0" aria-hidden="true"></svg>
                      <span>{{ inspection.office }}</span>
                    </div>
                    @if (inspection.officer) {
                      <div class="flex items-center gap-2 text-muted-foreground">
                        <svg lucide="UserCheck" class="h-4 w-4 shrink-0" aria-hidden="true"></svg>
                        <span>{{ inspection.officer }}</span>
                      </div>
                    }
                  </div>

                  <!-- Focus areas -->
                  <div class="mt-3">
                    <div class="flex items-center gap-1.5 text-xs font-medium uppercase tracking-wide text-muted-foreground mb-1.5">
                      <svg lucide="ClipboardCheck" class="h-3.5 w-3.5" aria-hidden="true"></svg>
                      What the inspector will review
                    </div>
                    <ul class="flex flex-wrap gap-1.5">
                      @for (area of inspection.focusAreas; track area) {
                        <li>
                          <span class="inline-block rounded-md bg-muted px-2 py-0.5 text-xs text-foreground">{{ area }}</span>
                        </li>
                      }
                    </ul>
                  </div>

                  @if (inspection.note) {
                    <div class="mt-3 flex items-start gap-2 rounded-md bg-muted/50 p-2.5 text-sm text-muted-foreground">
                      <svg lucide="Info" class="h-4 w-4 mt-0.5 shrink-0 text-[#255A90]" aria-hidden="true"></svg>
                      <span>{{ inspection.note }}</span>
                    </div>
                  }

                  <div class="mt-3 flex flex-wrap items-center gap-4">
                    <button
                      type="button"
                      (click)="addToCalendar(inspection)"
                      class="inline-flex items-center gap-1 text-sm font-medium text-[#013366] hover:underline"
                    >
                      <svg lucide="CalendarPlus" class="h-3.5 w-3.5" aria-hidden="true"></svg>
                      Add to calendar
                    </button>
                    @if (inspection.confirmationRequested) {
                      <span class="text-xs text-orange-700 font-medium">Confirmation requested — reply to your C&amp;E notice</span>
                    }
                  </div>
                </div>
              </div>
            }
          </div>
        }
      </section>

      <!-- Past -->
      <section aria-label="Inspection history">
        <div class="flex items-center gap-2 mb-4">
          <svg lucide="History" class="h-5 w-5 text-[#013366]" aria-hidden="true"></svg>
          <h2 class="text-lg font-semibold text-foreground">Inspection history</h2>
          <span uiBadge variant="outline" class="ml-1 bg-white">{{ past().length }}</span>
        </div>

        @if (past().length === 0) {
          <div class="text-center py-10 bg-white border rounded-lg">
            <svg lucide="History" class="h-10 w-10 mx-auto text-muted-foreground mb-3" aria-hidden="true"></svg>
            <p class="text-muted-foreground">No past inspections are on record for this licence.</p>
          </div>
        } @else {
          <div class="bg-white border rounded-lg divide-y">
            @for (rec of past(); track rec.id) {
              @let outcome = outcomeMeta(rec.outcome);
              <div class="p-4 flex flex-col sm:flex-row sm:items-start gap-3">
                <div class="sm:w-32 shrink-0 text-sm text-muted-foreground">{{ enforcementDate(rec.date) }}</div>
                <div class="flex-1 min-w-0">
                  <div class="flex flex-wrap items-center gap-2">
                    <span uiBadge variant="outline" [class]="outcome.badge">{{ outcome.label }}</span>
                    @if (rec.section) {
                      <span class="text-xs text-muted-foreground">{{ rec.section }}</span>
                    }
                    <span class="text-xs text-muted-foreground">· {{ rec.officer }}</span>
                  </div>
                  <p class="text-sm text-foreground mt-1.5">{{ rec.description }}</p>
                </div>
              </div>
            }
          </div>
        }
      </section>
    </div>
  `,
})
export class LicenceInspections {
  private readonly licencesRepository = inject(LICENCES_REPOSITORY);
  readonly licenceId = input.required<string>();
  protected readonly upcoming = computed(() => this.licencesRepository.scheduledInspections(this.licenceId()));
  protected readonly past = computed(() => this.licencesRepository.pastInspections(this.licenceId()));

  protected kindMeta(i: ScheduledInspection) {
    return INSPECTION_KIND_META[i.kind];
  }
  protected statusMetaOf(i: ScheduledInspection) {
    return INSPECTION_STATUS_META[i.status];
  }
  protected outcomeMeta(outcome: keyof typeof OUTCOME_META) {
    return OUTCOME_META[outcome];
  }
  protected countdownOf(iso: string) {
    return countdownLabel(iso);
  }
  protected dateOf(iso: string) {
    return new Date(iso);
  }
  protected month(d: Date) {
    return d.toLocaleDateString('en-CA', { month: 'short' });
  }
  protected dateTime(iso: string) {
    return formatInspectionDateTime(iso);
  }
  protected enforcementDate(iso: string) {
    return formatEnforcementDate(iso);
  }
  protected addToCalendar(inspection: ScheduledInspection) {
    downloadICS([inspectionToCalendarEvent(inspection)], `${inspection.id}.ics`);
  }
}
