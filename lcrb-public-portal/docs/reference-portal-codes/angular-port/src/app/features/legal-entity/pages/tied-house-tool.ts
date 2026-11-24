/** Tied House tool: hosts the reusable declaration section and switches between its three embedding contexts (Legal Entity Review, voluntary update, New Application). */
import { ChangeDetectionStrategy, Component, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { PAGE_HERO } from '@shared/ui/bc-gov/page-hero';
import { injectSession } from '@core/session/session-provider';
import { TiedHouseDeclarationSection } from '../components/tied-house-declaration-section';
import { Button } from '@shared/ui/primitives/button';
import { CARD } from '@shared/ui/primitives/card';
import type { IconName } from '@shared/ui/primitives/icons';
import { LucideIcon } from '@shared/ui/primitives/lucide-icon';
import { injectBusinessProfile } from '@core/session/business-profile';
import { SOURCE_META } from '@domain/tied-house/tied-house';
import { type TiedHouseSource } from '@domain/models/tied-house';
import { cn } from '@shared/util/utils';
import { path } from '@/app/route-registry';

const CONTEXTS: { source: TiedHouseSource; icon: IconName; blurb: string }[] = [
  { source: 'voluntary', icon: 'FilePlus2', blurb: 'Update a Change' },
  { source: 'le-review', icon: 'Building2', blurb: 'Legal Entity Review' },
  { source: 'new-application', icon: 'Link2', blurb: 'New Application' },
];

@Component({
  selector: 'tied-house-tool',
  imports: [RouterLink, ...PAGE_HERO, ...CARD, Button, LucideIcon, TiedHouseDeclarationSection],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <!-- Licensee-only tool, mirroring the Legal Entity Builder. -->
    @if (session() && session()!.role !== 'licensee') {
      <div class="mx-auto max-w-2xl px-4 py-16">
        <div uiCard>
          <div uiCardHeader>
            <div uiCardTitle>Licensee tool</div>
          </div>
          <div uiCardContent class="space-y-4 text-sm text-muted-foreground">
            <p>Tied House declarations are available to licensee (business) accounts.</p>
            <a uiButton variant="outline" [routerLink]="path('account.login')">Switch account</a>
          </div>
        </div>
      </div>
    } @else {
      <page-hero [breadcrumbs]="breadcrumbs" title="Tied House Declarations">
        <ng-container heroMeta>{{ profile().legalName }}</ng-container>
        <ng-container heroDescription
          >Declare interests and associations you, your shareholders, directors, officers, or immediate family hold in other B.C. liquor or cannabis licences. Declarations draw on the same relationship data as your
          <a [routerLink]="path('legal-entity.builder')" class="font-medium text-white underline underline-offset-2">Legal Entity build</a> and are reviewed by the Branch.</ng-container
        >
      </page-hero>

      <div class="mx-auto max-w-4xl px-4 py-6 sm:px-6 lg:px-8">
        <!-- Context switcher (prototype: shows where this section embeds) -->
        <div class="mb-6 rounded-lg border border-dashed border-border bg-muted/30 p-3">
          <p class="mb-2 px-1 text-xs font-medium uppercase tracking-wide text-muted-foreground">Prototype: this section embeds in these flows</p>
          <div class="grid gap-2 sm:grid-cols-3">
            @for (c of contexts; track c.source) {
              <button
                type="button"
                (click)="source.set(c.source)"
                [class]="
                  cn(
                    'flex items-center gap-2 rounded-md border px-3 py-2 text-left text-sm transition-colors',
                    source() === c.source
                      ? 'border-[#013366] bg-[#013366]/5 text-[#013366]'
                      : 'border-border bg-card text-muted-foreground hover:bg-muted'
                  )
                "
                [attr.aria-pressed]="source() === c.source"
              >
                <svg [lucide]="c.icon" class="h-4 w-4 shrink-0"></svg>
                <span class="font-medium">{{ sourceMeta[c.source].label }}</span>
              </button>
            }
          </div>
        </div>

        <!-- The reusable, embeddable section -->
        <tied-house-declaration-section [source]="source()" [requiredByReview]="source() === 'le-review'" />

        <!-- Cross-links -->
        <div class="mt-8 flex flex-wrap gap-3 border-t border-border pt-6 text-sm">
          <a [routerLink]="path('legal-entity.builder')" class="inline-flex items-center gap-1.5 font-medium text-[#013366] hover:underline">
            Legal Entity Builder <svg lucide="ExternalLink" class="h-3.5 w-3.5"></svg>
          </a>
          <a [routerLink]="path('applications.list')" class="inline-flex items-center gap-1.5 font-medium text-[#013366] hover:underline">
            My Applications <svg lucide="ExternalLink" class="h-3.5 w-3.5"></svg>
          </a>
        </div>
      </div>
    }
  `,
})
export class TiedHouseTool {
  protected readonly path = path;
  protected readonly session = injectSession().session;
  protected readonly profile = injectBusinessProfile();
  protected readonly source = signal<TiedHouseSource>('voluntary');

  protected readonly contexts = CONTEXTS;
  protected readonly sourceMeta = SOURCE_META;
  protected readonly cn = cn;
  protected readonly breadcrumbs = [{ label: 'Dashboard', href: path('dashboard.home') }, { label: 'Business Structure' }, { label: 'Tied House Declarations' }];
}
