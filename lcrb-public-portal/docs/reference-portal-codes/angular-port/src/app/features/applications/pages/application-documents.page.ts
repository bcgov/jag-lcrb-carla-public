/** "Required documents"; `id` arrives as a route-param input and unknown ids go to the not-found page via the route. */
import { ChangeDetectionStrategy, Component, computed, inject, input } from '@angular/core';
import { DocumentManager } from '@shared/ui/documents/document-manager';
import { BCGovFooter } from '@core/layout/footer';
import { BCGovHeader } from '@core/layout/header';
import { PageHero } from '@shared/ui/bc-gov/page-hero';
import { injectSession } from '@core/session/session-provider';
import { Badge } from '@shared/ui/primitives/badge';
import { CARD } from '@shared/ui/primitives/card';
import { LucideIcon } from '@shared/ui/primitives/lucide-icon';
import { APPLICATIONS_REPOSITORY } from '@data-access/repositories/applications/applications.repository';
import { path } from '@/app/route-registry';

@Component({
  selector: 'application-documents-page',
  imports: [BCGovHeader, BCGovFooter, PageHero, ...CARD, Badge, LucideIcon, DocumentManager],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    @if (app(); as a) {
      <div class="flex min-h-screen flex-col bg-background">
        <bc-gov-header currentPage="/applications" />

        <main id="main-content" class="flex-1">
          <page-hero
            containerClassName="container mx-auto px-4"
            [breadcrumbs]="breadcrumbs()"
            title="Required documents"
            [description]="a.type + ' · Job No: ' + a.id"
          />

          <div class="container mx-auto max-w-3xl px-4 py-8 space-y-6">
            @if (a.reviewNote) {
              <div uiCard class="border-yellow-300 bg-yellow-50">
                <div uiCardContent class="flex gap-3 p-5">
                  <svg lucide="AlertCircle" class="h-5 w-5 shrink-0 text-yellow-700"></svg>
                  <p class="text-sm text-yellow-800">{{ a.reviewNote }}</p>
                </div>
              </div>
            }

            @if (a.requiredDocuments.length === 0) {
              <div uiCard>
                <div uiCardContent class="flex flex-col items-center gap-3 p-12 text-center">
                  <svg lucide="CheckCircle2" class="h-8 w-8 text-green-600"></svg>
                  <p class="font-semibold">No documents required</p>
                  <p class="text-sm text-muted-foreground">
                    There are no outstanding document requests for this application. We'll let you know if we need anything further.
                  </p>
                </div>
              </div>
            } @else {
              <p class="text-sm text-muted-foreground">
                LCRB has requested the following {{ a.requiredDocuments.length }} document{{ a.requiredDocuments.length === 1 ? '' : 's' }}. Upload
                each item below. Files are scanned for viruses before they are accepted.
              </p>
              @for (doc of a.requiredDocuments; track doc.id) {
                <div uiCard>
                  <div uiCardHeader>
                    <div class="flex flex-wrap items-center gap-2">
                      <div uiCardTitle class="text-base">{{ doc.name }}</div>
                      @if (doc.required) {
                        <span uiBadge variant="outline" class="border-red-200 bg-red-50 text-red-700">Required</span>
                      } @else {
                        <span uiBadge variant="outline" class="text-muted-foreground">Optional</span>
                      }
                    </div>
                    <p class="text-sm text-muted-foreground">{{ doc.reason }}</p>
                  </div>
                  <div uiCardContent>
                    <document-manager [subjectId]="a.id + ':' + doc.id" [actor]="actor()" [category]="doc.name" />
                  </div>
                </div>
              }

              <div uiCard class="border-blue-200 bg-blue-50">
                <div uiCardContent class="flex gap-3 p-4">
                  <svg lucide="AlertCircle" class="h-5 w-5 shrink-0 text-blue-600"></svg>
                  <p class="text-sm text-blue-800">
                    Once all required documents are uploaded and pass the virus scan, your application automatically returns to the review queue. No
                    further action is needed.
                  </p>
                </div>
              </div>
            }
          </div>
        </main>

        <bc-gov-footer />
      </div>
    }
  `,
})
export class ApplicationDocumentsPage {
  private readonly applicationsRepository = inject(APPLICATIONS_REPOSITORY);
  readonly id = input.required<string>();

  private readonly sessionProvider = injectSession();
  protected readonly app = computed(() => this.applicationsRepository.submittedApplication(this.id()));
  protected readonly breadcrumbs = computed(() => {
    const a = this.app()!;
    return [
      { label: 'Dashboard', href: path('dashboard.home') },
      { label: 'My Applications', href: path('applications.list') },
      { label: a.type, href: path('applications.submitted', { id: a.id }) },
      { label: 'Required documents' },
    ];
  });
  protected readonly actor = computed(() => {
    const session = this.sessionProvider.session();
    return session ? `${session.firstName} ${session.lastName}`.trim() || 'Applicant' : 'Applicant';
  });
}
