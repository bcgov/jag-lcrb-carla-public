/** Case Triage; `?case=` opens that case's drawer on mount. */
import { ChangeDetectionStrategy, Component, computed, inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { BCGovFooter } from '@core/layout/footer';
import { BCGovHeader } from '@core/layout/header';
import { PAGE_HERO } from '@shared/ui/bc-gov/page-hero';
import { LcrbGate } from '@core/permissions/lcrb-gate';
import { TriageView } from '../components/triage-view';
import { injectSession } from '@core/session/session-provider';
import { STAFF_REPOSITORY } from '@data-access/repositories/staff/staff.repository';
import { path } from '@/app/route-registry';

@Component({
  selector: 'lcrb-triage-content',
  imports: [...PAGE_HERO, TriageView],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <page-hero
      [breadcrumbs]="[{ label: 'Intake Overview', href: path('staff.home') }, { label: 'Case Triage' }]"
      title="Case Triage"
      description="Cases pre-screened by the AI agent and synced to MS Dynamics, grouped by the queue they were routed to. Open a case to review the full agent log; the analyst makes the final determination."
    />
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <triage-view [cases]="cases" [initialCaseId]="initialCaseId" />
    </div>
  `,
})
export class LcrbTriageContent {
  protected readonly path = path;
  private readonly staffRepository = inject(STAFF_REPOSITORY);
  protected readonly cases = this.staffRepository.cases();
  protected readonly initialCaseId = inject(ActivatedRoute).snapshot.queryParamMap.get('case');
}

@Component({
  selector: 'lcrb-triage-page',
  imports: [BCGovHeader, BCGovFooter, LcrbGate, LcrbTriageContent],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <div class="min-h-screen flex flex-col bg-background">
      <bc-gov-header currentPage="/lcrb/triage" />
      <main class="flex-1">
        <lcrb-gate>
          @if (allowed()) {
            <lcrb-triage-content />
          }
        </lcrb-gate>
      </main>
      <bc-gov-footer />
    </div>
  `,
})
export class LcrbTriagePage {
  private readonly session = injectSession();
  /** What `LcrbGate` lets through: its content is created only then, as in React. */
  protected readonly allowed = computed(() => !this.session.loading() && this.session.session()?.role === 'lcrb');

}
