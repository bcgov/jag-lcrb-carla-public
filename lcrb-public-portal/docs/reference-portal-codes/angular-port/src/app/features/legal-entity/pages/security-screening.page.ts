/** Security Screening Requirements. The prototype has no page metadata, so the route keeps the root title. */
import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';
import { BCGovFooter } from '@core/layout/footer';
import { BCGovHeader } from '@core/layout/header';
import { PAGE_HERO } from '@shared/ui/bc-gov/page-hero';
import { injectSession } from '@core/session/session-provider';
import { ScreeningPersonCard } from '../components/screening-person-card';
import { CARD } from '@shared/ui/primitives/card';
import { LucideIcon } from '@shared/ui/primitives/lucide-icon';
import { TABS } from '@shared/ui/primitives/tabs';
import { injectScreeningPeople, type ScreeningCategory } from '../state/screening-store';
import { path } from '@/app/route-registry';

@Component({
  selector: 'category-panel',
  imports: [...CARD, LucideIcon, ScreeningPersonCard],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <div uiCard>
      <div uiCardHeader>
        <div uiCardTitle class="flex items-center justify-between text-lg">
          <span>{{ title() }}</span>
          @if (list().length > 0) {
            <span class="text-sm font-normal text-muted-foreground">
              {{ outstanding() === 0 ? 'All screenings submitted' : outstanding() + ' outstanding' }}
            </span>
          }
        </div>
      </div>
      <div uiCardContent>
        @if (list().length === 0) {
          <div class="text-center py-8">
            <div class="w-12 h-12 rounded-full bg-muted flex items-center justify-center mx-auto mb-4">
              <svg lucide="FileText" class="h-6 w-6 text-muted-foreground"></svg>
            </div>
            <p class="text-muted-foreground">No Information Required</p>
            <p class="text-sm text-muted-foreground mt-1">
              LCRB has not requested any Personal History Summary forms for individuals associated with {{ category() }} related licences or applications at this time.
            </p>
          </div>
        } @else {
          <div class="space-y-4">
            @for (person of list(); track person.id) {
              <div screeningPersonCard [person]="person" [actor]="actor()"></div>
            }
          </div>
        }
      </div>
    </div>
  `,
})
export class CategoryPanel {
  readonly title = input.required<string>();
  readonly category = input.required<ScreeningCategory>();
  readonly actor = input.required<string>();

  private readonly people = injectScreeningPeople();
  protected readonly list = computed(() => this.people().filter((p) => p.category === this.category()));
  protected readonly outstanding = computed(() => this.list().filter((p) => p.status !== 'synced').length);
}

@Component({
  selector: 'security-screening-page',
  imports: [BCGovHeader, BCGovFooter, ...PAGE_HERO, ...CARD, ...TABS, LucideIcon, CategoryPanel],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <div class="min-h-screen flex flex-col bg-background">
      <bc-gov-header />

      <main class="flex-1">
        <!-- Hero Section -->
        <page-hero
          containerClassName="container mx-auto px-4"
          [breadcrumbs]="breadcrumbs"
          description="Manage Personal History Summary screening for each associated person"
        >
          <span heroTitle class="flex items-center gap-3">
            <svg lucide="ShieldCheck" class="h-8 w-8"></svg>
            Security Screening Requirements
          </span>
        </page-hero>

        <div class="container mx-auto px-4 py-8">
          <div uiTabs value="liquor" class="w-full">
            <div uiTabsList class="mb-6">
              <button uiTabsTrigger value="liquor">Liquor Requirements</button>
              <button uiTabsTrigger value="cannabis">Cannabis Requirements</button>
            </div>

            <div uiTabsContent value="liquor">
              <category-panel [title]="'Liquor Licence Security Screening'" category="liquor" [actor]="actor()" />
            </div>

            <div uiTabsContent value="cannabis">
              <category-panel [title]="'Cannabis Licence Security Screening'" category="cannabis" [actor]="actor()" />
            </div>
          </div>

          <!-- Information Card -->
          <div uiCard class="mt-6 border-blue-200 bg-blue-50">
            <div uiCardContent class="p-4">
              <div class="flex gap-3">
                <svg lucide="ShieldCheck" class="h-5 w-5 text-blue-600 shrink-0"></svg>
                <div class="text-sm">
                  <p class="font-medium text-blue-900">About Security Screening</p>
                  <p class="text-blue-700 mt-1">
                    The LCRB requires security screening for individuals associated with liquor and cannabis licences — including directors, shareholders, partners, and key personnel. Send each person a screening request, then upload their completed Personal History Summary on their behalf. Submitted forms are sent to LCRB (via Dynamics) for review. For questions, contact LCRB at 1-866-209-2111.
                  </p>
                </div>
              </div>
            </div>
          </div>
        </div>
      </main>

      <bc-gov-footer />
    </div>
  `,
})
export class SecurityScreeningPage {
  private readonly session = injectSession().session;
  protected readonly breadcrumbs = [{ label: 'Dashboard', href: path('dashboard.home') }, { label: 'Security Screening Requirements' }];
  protected readonly actor = computed(() => {
    const s = this.session();
    const name = [s?.firstName, s?.lastName].filter(Boolean).join(' ');
    return name || 'Business user';
  });
}
