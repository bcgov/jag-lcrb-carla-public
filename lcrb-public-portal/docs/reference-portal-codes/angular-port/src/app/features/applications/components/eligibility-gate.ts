/** Licence-prerequisite gate for the application flow, so a direct link, bookmark or AI suggestion cannot bypass the catalogue's filtering. */
import { ChangeDetectionStrategy, Component, computed, input } from "@angular/core"
import { RouterLink } from "@angular/router"
import { BCGovFooter } from "@core/layout/footer"
import { BCGovHeader } from "@core/layout/header"
import { PageHero } from "@shared/ui/bc-gov/page-hero"
import { Button } from "@shared/ui/primitives/button"
import { Card, CardContent } from "@shared/ui/primitives/card"
import { LucideIcon } from "@shared/ui/primitives/lucide-icon"
import { injectBusinessProfile } from "@core/session/business-profile"
import { LICENCE_TYPE_LABELS } from "@domain/models/applications"
import type { LicenceType } from "@domain/models/applications"
import { heldLicenceTypes, isEntryEligible } from "@domain/applications/application-eligibility"
import { ConfigFlow } from "@shared/forms/config-flow"
import { type FlowConfig } from "@domain/forms/flow-config"
import { path } from "@/app/route-registry"

@Component({
  selector: "eligibility-gate",
  imports: [BCGovFooter, BCGovHeader, Button, Card, CardContent, ConfigFlow, LucideIcon, PageHero, RouterLink],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: "display: contents" },
  template: `
    @if (eligible()) {
      <config-flow [config]="config()" [prefill]="prefill()" />
    } @else {
      <div class="min-h-screen flex flex-col">
        <bc-gov-header currentPage="/applications" />
        <main id="main-content" class="flex-1">
          <page-hero [breadcrumbs]="breadcrumbs()" [title]="title()" description="This application is not available for your current licences." />

          <div class="max-w-3xl mx-auto px-4 sm:px-6 lg:px-8 py-10">
            <div uiCard class="border-l-4 border-l-amber-500">
              <div uiCardContent class="p-6">
                <div class="flex items-start gap-4">
                  <div class="rounded-lg bg-amber-100 p-2 text-amber-700 shrink-0">
                    <svg lucide="ShieldAlert" class="h-6 w-6" aria-hidden="true"></svg>
                  </div>
                  <div class="flex-1">
                    <h2 class="text-lg font-semibold text-[#013366] text-balance">You need a {{ requiredLabel() }} licence first</h2>
                    <p class="mt-2 text-sm leading-relaxed text-muted-foreground">
                      <span class="font-medium text-foreground">{{ title() }}</span> can only be added to an active
                      {{ requiredLabel() }} licence. Your business account does not currently hold one, so this
                      application cannot be submitted.
                    </p>

                    <div class="mt-5 rounded-md border bg-muted/40 p-4">
                      <h3 class="text-sm font-semibold text-foreground">Licences on your account</h3>
                      @if (heldLabels().length > 0) {
                        <ul class="mt-2 flex flex-wrap gap-2">
                          @for (label of heldLabels(); track label) {
                            <li class="rounded-full border bg-background px-3 py-1 text-xs font-medium text-foreground">{{ label }}</li>
                          }
                        </ul>
                      } @else {
                        <p class="mt-2 text-sm text-muted-foreground">
                          No licences are currently associated with this business account.
                        </p>
                      }
                    </div>

                    <div class="mt-6 flex flex-col gap-3 sm:flex-row">
                      <a uiButton [routerLink]="path('applications.new')" class="bg-[#013366] hover:bg-[#013366]/90">
                        <svg lucide="ArrowLeft" class="mr-2 h-4 w-4" aria-hidden="true"></svg>
                        Back to available applications
                      </a>
                      <!-- Only reachable when the prerequisite is missing, so the
                           licence list is always the useful next step here. -->
                      <a uiButton variant="outline" [routerLink]="path('licences.list')">
                        View my licences
                        <svg lucide="ArrowRight" class="ml-2 h-4 w-4" aria-hidden="true"></svg>
                      </a>
                    </div>
                  </div>
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
export class EligibilityGate {
  protected readonly path = path;
  readonly slug = input.required<string>()
  readonly title = input.required<string>()
  readonly description = input.required<string>()
  readonly licenceType = input.required<LicenceType>()
  readonly config = input.required<FlowConfig>()
  readonly prefill = input<Record<string, string> | undefined>(undefined)

  private readonly profile = injectBusinessProfile()
  protected readonly eligible = computed(() => {
    const held = heldLicenceTypes(this.profile().licences)
    return isEntryEligible(
      { slug: this.slug(), licenceType: this.licenceType(), description: this.description() },
      held,
    )
  })
  protected readonly requiredLabel = computed(() => LICENCE_TYPE_LABELS[this.licenceType()])
  // The application that would grant the missing prerequisite, when one exists.
  protected readonly heldLabels = computed(() =>
    this.profile()
      .licences.map((l) => l.type)
      .filter((v, i, a) => a.indexOf(v) === i),
  )
  protected readonly breadcrumbs = computed(() => [
    { label: "Dashboard", href: path('dashboard.home') },
    { label: "My Applications", href: path('applications.list') },
    { label: "Start a new application", href: path('applications.new') },
    { label: this.title() },
  ])
}
