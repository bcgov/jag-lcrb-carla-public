/** Read-only submitted-application page; `id` arrives as a route-param input and unknown ids go to the not-found page via the route. */
import { ChangeDetectionStrategy, Component, computed, inject, input } from '@angular/core';
import { RouterLink } from '@angular/router';
import { SubmittedFilePanel, SubmittedStatusBadge } from '../components/submitted-file-panel';
import { BCGovFooter } from '@core/layout/footer';
import { BCGovHeader } from '@core/layout/header';
import { PAGE_HERO } from '@shared/ui/bc-gov/page-hero';
import { ApplicationReceiptCard } from '../components/application-receipt-card';
import { Button } from '@shared/ui/primitives/button';
import { CARD } from '@shared/ui/primitives/card';
import { LucideIcon } from '@shared/ui/primitives/lucide-icon';
import { APPLICATIONS_REPOSITORY } from '@data-access/repositories/applications/applications.repository';
import { path } from '@/app/route-registry';

function formatDate(value: string) {
  const d = new Date(value);
  return Number.isNaN(d.getTime()) ? value : d.toLocaleDateString('en-CA', { year: 'numeric', month: 'long', day: 'numeric' });
}

/** On the grid's own `div` because the `dl` grid places its direct children. */
@Component({
  selector: 'div[submittedDetail]',
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <dt class="text-xs uppercase tracking-wide text-muted-foreground">{{ label() }}</dt>
    <dd class="mt-0.5 text-sm font-medium text-foreground">{{ value() }}</dd>
  `,
})
export class SubmittedDetail {
  readonly label = input.required<string>();
  readonly value = input.required<string>();
}

@Component({
  selector: 'application-submitted-page',
  imports: [
    RouterLink,
    BCGovHeader,
    BCGovFooter,
    ...PAGE_HERO,
    ...CARD,
    Button,
    LucideIcon,
    SubmittedStatusBadge,
    SubmittedFilePanel,
    SubmittedDetail,
    ApplicationReceiptCard,
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    @if (app(); as a) {
      <div class="flex min-h-screen flex-col bg-background">
        <bc-gov-header currentPage="/applications" />

        <main id="main-content" class="flex-1">
          <page-hero containerClassName="container mx-auto px-4" [breadcrumbs]="breadcrumbs()" [title]="a.type">
            <submitted-status-badge heroBadge [id]="a.id" [status]="a.status" />
            <ng-container heroMeta>Job No: {{ a.id }} | Ref: {{ a.reference }}</ng-container>
          </page-hero>

          <div class="container mx-auto max-w-4xl px-4 py-8 space-y-6">
            <!-- Documents pending banner -->
            @if (a.status === 'pending_documents') {
              <div uiCard class="border-yellow-300 bg-yellow-50">
                <div uiCardContent class="p-5">
                  <div class="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
                    <div class="flex gap-3">
                      <svg lucide="AlertCircle" class="h-5 w-5 shrink-0 text-yellow-700"></svg>
                      <div>
                        <p class="font-semibold text-yellow-900">Documents required</p>
                        <p class="mt-1 text-sm text-yellow-800">
                          {{ a.reviewNote ?? 'LCRB has requested additional documents before your review can continue.' }}
                        </p>
                      </div>
                    </div>
                    <a uiButton class="shrink-0 bg-bc-blue hover:bg-bc-blue/90" [routerLink]="path('applications.documents', { id: a.id })">
                      <svg lucide="Upload" class="mr-2 h-4 w-4"></svg>
                      Upload documents{{ outstandingDocs() > 0 ? ' (' + outstandingDocs() + ')' : '' }}
                    </a>
                  </div>
                </div>
              </div>
            }

            <!-- Withdraw / cancelled state and case conversation -->
            <submitted-file-panel [id]="a.id" [type]="a.type" [status]="a.status" [applicant]="a.applicant" />

            <!-- Key details -->
            <div uiCard>
              <div uiCardHeader>
                <div uiCardTitle class="text-lg">Application details</div>
              </div>
              <div uiCardContent>
                <dl class="grid grid-cols-1 gap-4 sm:grid-cols-2">
                  <div submittedDetail label="Applicant" [value]="a.applicant"></div>
                  <div submittedDetail label="Confirmation number" [value]="a.confirmationNumber"></div>
                  <div submittedDetail label="Submitted" [value]="formatDate(a.submittedDate)"></div>
                  <div submittedDetail label="Expected completion" [value]="formatDate(a.expectedCompletion)"></div>
                </dl>
              </div>
            </div>

            <!-- What you submitted -->
            <div uiCard>
              <div uiCardHeader>
                <div uiCardTitle class="text-lg">What you submitted</div>
              </div>
              <div uiCardContent class="space-y-6">
                @for (section of a.summary; track section.heading) {
                  <div>
                    <h3 class="text-sm font-semibold uppercase tracking-wide text-[#38598A]">{{ section.heading }}</h3>
                    <dl class="mt-3 grid grid-cols-1 gap-3 sm:grid-cols-2">
                      @for (f of section.fields; track f.label) {
                        <div submittedDetail [label]="f.label" [value]="f.value"></div>
                      }
                    </dl>
                  </div>
                }
              </div>
            </div>

            <!-- Progress timeline -->
            <div uiCard>
              <div uiCardHeader>
                <div uiCardTitle class="text-lg">Progress</div>
              </div>
              <div uiCardContent>
                <ol class="space-y-4">
                  @for (step of a.timeline; track step.label) {
                    <li class="flex items-start gap-3">
                      <span
                        [attr.class]="
                          'mt-0.5 flex h-6 w-6 shrink-0 items-center justify-center rounded-full ' +
                          (step.state === 'done'
                            ? 'bg-green-100 text-green-700'
                            : step.state === 'current'
                              ? 'bg-blue-100 text-blue-700'
                              : 'bg-gray-100 text-gray-400')
                        "
                      >
                        @if (step.state === 'done') {
                          <svg lucide="CheckCircle2" class="h-4 w-4"></svg>
                        } @else if (step.state === 'current') {
                          <svg lucide="Clock" class="h-4 w-4"></svg>
                        } @else {
                          <span class="h-2 w-2 rounded-full bg-current"></span>
                        }
                      </span>
                      <div>
                        <p [attr.class]="'text-sm font-medium ' + (step.state === 'upcoming' ? 'text-muted-foreground' : 'text-foreground')">
                          {{ step.label }}
                        </p>
                        <p class="text-xs text-muted-foreground">{{ formatDate(step.date) }}</p>
                      </div>
                    </li>
                  }
                </ol>
              </div>
            </div>

            <!-- Payment summary -->
            <div uiCard>
              <div uiCardHeader>
                <div uiCardTitle class="text-lg">Payment summary</div>
              </div>
              <div uiCardContent>
                <ul class="divide-y">
                  @for (f of a.fee; track f.label) {
                    <li class="flex items-center justify-between py-2 text-sm">
                      <span class="text-muted-foreground">{{ f.label }}</span>
                      <span class="font-medium">{{ money(f.amount) }} {{ a.currency }}</span>
                    </li>
                  }
                  <li class="flex items-center justify-between py-2 text-sm font-semibold">
                    <span>Total paid</span>
                    <span>{{ money(total()) }} {{ a.currency }}</span>
                  </li>
                </ul>
                <p class="mt-3 flex items-center gap-2 text-xs text-muted-foreground">
                  <svg lucide="FileText" class="h-3.5 w-3.5"></svg>
                  Paid with {{ a.paymentMethod }}
                </p>
                <div class="mt-4">
                  <application-receipt-card [jobNumber]="a.reference" />
                </div>
              </div>
            </div>
          </div>
        </main>

        <bc-gov-footer />
      </div>
    }
  `,
})
export class ApplicationSubmittedPage {
  protected readonly path = path;
  private readonly applicationsRepository = inject(APPLICATIONS_REPOSITORY);
  readonly id = input.required<string>();

  protected readonly app = computed(() => this.applicationsRepository.submittedApplication(this.id()));
  protected readonly total = computed(() => this.app()!.fee.reduce((sum, f) => sum + f.amount, 0));
  protected readonly outstandingDocs = computed(() => this.app()!.requiredDocuments.filter((d) => d.required).length);
  protected readonly breadcrumbs = computed(() => [
    { label: 'Dashboard', href: path('dashboard.home') },
    { label: 'My Applications', href: path('applications.list') },
    { label: this.app()!.type },
  ]);
  protected readonly formatDate = formatDate;
  protected money(amount: number): string {
    return `$${amount.toLocaleString()}`;
  }
}
