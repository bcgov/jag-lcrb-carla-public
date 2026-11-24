/** Links each application to the same applicant's or business's earlier applications and licences, matching business first, then applicant. */
import { ChangeDetectionStrategy, Component, computed, inject, input } from '@angular/core';
import { RouterLink } from '@angular/router';
import { LgStatusBadge } from './lg-status-badge';
import { Badge } from '@shared/ui/primitives/badge';
import { LucideIcon } from '@shared/ui/primitives/lucide-icon';
import { type LgApplication } from '@domain/models/lg-review';
import { formatLgDate } from '@domain/lg-review/due-dates';
import { formatLicenceDate } from '@domain/licences/licences';
import { type Licence } from '@domain/models/licences';
import { LG_REVIEW_REPOSITORY } from '@data-access/repositories/lg-review/lg-review.repository';
import { LICENCES_REPOSITORY } from '@data-access/repositories/licences/licences.repository';
import { path } from '@/app/route-registry';

function normalise(name: string): string {
  return name
    .toLowerCase()
    .replace(/\b(ltd|inc|corp|co|limited|incorporated|holdings?|group)\b\.?/g, '')
    .replace(/[^a-z0-9]/g, '');
}

/** The person in the jurisdiction who recorded the comment, if anyone has. */
function handledBy(app: LgApplication): string | null {
  const entry = [...app.history]
    .reverse()
    .find((h) => /comment submitted|sent to community review|documents requested/i.test(h.label));
  return entry?.actor ?? null;
}

@Component({
  selector: 'applicant-history',
  imports: [RouterLink, LgStatusBadge, Badge, LucideIcon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <div class="space-y-6">
      <section class="rounded-lg border border-border bg-card p-5">
        <div class="flex flex-wrap items-center justify-between gap-2">
          <h2 class="text-sm font-semibold uppercase tracking-wide text-muted-foreground">
            Previous applications — {{ current().businessName }}
          </h2>
          <span uiBadge variant="outline" class="border-border text-muted-foreground">
            <svg lucide="Users2" class="mr-1 h-3 w-3"></svg>
            All representatives
          </span>
        </div>
        <p class="mt-1 text-xs text-muted-foreground">
          Every referral to {{ current().jurisdiction }} from this business or applicant, including files handled by other staff in your office.
        </p>
        @if (related().length === 0) {
          <p class="mt-4 text-sm text-muted-foreground">No other applications from this business or applicant.</p>
        } @else {
          <ul class="mt-4 divide-y divide-border">
            @for (a of related(); track a.jobNumber) {
              <li class="flex flex-wrap items-center justify-between gap-3 py-3">
                <div class="min-w-0">
                  <a
                    [routerLink]="path('lg.review', { id: a.jobNumber })"
                    class="inline-flex items-center gap-1 font-medium text-[#013366] underline-offset-2 hover:underline"
                  >
                    <svg lucide="FileText" class="h-4 w-4 shrink-0"></svg>
                    {{ a.establishmentName }}
                    <svg lucide="ArrowUpRight" class="h-3.5 w-3.5"></svg>
                  </a>
                  <p class="text-xs text-muted-foreground">
                    Job #{{ a.jobNumber }} · {{ a.licenceType }} · {{ a.applicationType }} · submitted {{ formatLgDate(a.submittedDate) }}
                  </p>
                  <p class="text-xs text-muted-foreground">
                    {{ handledBy(a) ? 'Handled by ' + handledBy(a) : 'Not yet commented on' }}
                  </p>
                </div>
                <lg-status-badge [status]="a.status" />
              </li>
            }
          </ul>
        }
      </section>

      <section class="rounded-lg border border-border bg-card p-5">
        <h2 class="text-sm font-semibold uppercase tracking-wide text-muted-foreground">Licences held by this business</h2>
        @if (licences().length === 0) {
          <p class="mt-4 text-sm text-muted-foreground">No current licences on record for {{ current().businessName }}.</p>
        } @else {
          <ul class="mt-4 divide-y divide-border">
            @for (l of licences(); track l.id) {
              <li class="flex flex-wrap items-center justify-between gap-3 py-3">
                <div class="min-w-0">
                  <p class="inline-flex items-center gap-1 font-medium text-foreground">
                    <svg lucide="ScrollText" class="h-4 w-4 shrink-0 text-[#013366]"></svg>
                    {{ l.name }}
                  </p>
                  <p class="text-xs text-muted-foreground">Licence #{{ l.id }} · {{ l.type }} · {{ l.address }}</p>
                  <p class="text-xs text-muted-foreground">Expires {{ formatLicenceDate(l.expiryDate) }}</p>
                </div>
                <span
                  uiBadge
                  variant="outline"
                  [class]="l.status === 'Active' ? 'border-green-200 bg-green-50 text-green-800' : 'border-amber-200 bg-amber-50 text-amber-900'"
                >
                  {{ l.status }}
                </span>
              </li>
            }
          </ul>
        }
      </section>
    </div>
  `,
})
export class ApplicantHistory {
  protected readonly path = path;
  private readonly lgReviewRepository = inject(LG_REVIEW_REPOSITORY);
  private readonly licencesRepository = inject(LICENCES_REPOSITORY);
  readonly current = input.required<LgApplication>();

  private readonly applications = this.lgReviewRepository.store().applications;

  protected readonly related = computed(() => {
    const current = this.current();
    const biz = normalise(current.businessName);
    const person = current.applicantName.trim().toLowerCase();
    return this.applications()
      .filter((a) => a.jobNumber !== current.jobNumber)
      .filter((a) => normalise(a.businessName) === biz || a.applicantName.trim().toLowerCase() === person)
      .sort((a, b) => b.submittedDate.localeCompare(a.submittedDate));
  });

  protected readonly licences = computed<Licence[]>(() => {
    const biz = normalise(this.current().businessName);
    const seen = new Set<string>();
    const out: Licence[] = [];
    for (const l of [...this.licencesRepository.licences(), ...this.licencesRepository.operatedLicences(), ...this.licencesRepository.affiliateLicences()]) {
      if (seen.has(l.id)) continue;
      const owners = [l.licencedTo, l.operatedBy].filter(Boolean).map((n) => normalise(n as string));
      if (owners.includes(biz)) {
        seen.add(l.id);
        out.push(l);
      }
    }
    return out;
  });

  protected readonly handledBy = handledBy;
  protected readonly formatLgDate = formatLgDate;
  protected readonly formatLicenceDate = formatLicenceDate;
}
