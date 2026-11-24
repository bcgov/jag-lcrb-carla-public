/** Flow for one catalogue application; `slug` and prefill query params arrive as route inputs. Unknown slugs are handled by the routes, so the null handling is a fallback. */
import { ChangeDetectionStrategy, Component, computed, inject, input } from "@angular/core"
import { EligibilityGate } from "../components/eligibility-gate"
import { APPLICATIONS_REPOSITORY } from "@data-access/repositories/applications/applications.repository"

/** Fields prefilled from query params when an application starts from a licence; the licence number and business name carry through. */
const PREFILLABLE_FIELDS = ["licenceNumber", "establishmentName", "legalEntityName", "legalName"] as const

type QueryValue = string | string[] | undefined

@Component({
  selector: "application-flow-page",
  imports: [EligibilityGate],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: "display: contents" },
  template: `
    @if (entry(); as e) {
      <!-- Prerequisites are enforced here, not just in the catalogue, so a direct link or AI suggestion cannot bypass them. -->
      <eligibility-gate
        [slug]="e.slug"
        [title]="e.title"
        [description]="e.description"
        [licenceType]="e.licenceType"
        [config]="e.config"
        [prefill]="prefill()"
      />
    }
  `,
})
export class ApplicationFlowPage {
  private readonly applicationsRepository = inject(APPLICATIONS_REPOSITORY)
  readonly slug = input.required<string>()
  readonly licenceNumber = input<QueryValue>(undefined)
  readonly establishmentName = input<QueryValue>(undefined)
  readonly legalEntityName = input<QueryValue>(undefined)
  readonly legalName = input<QueryValue>(undefined)

  protected readonly entry = computed(() => this.applicationsRepository.catalogueEntry(this.slug()))

  protected readonly prefill = computed<Record<string, string> | undefined>(() => {
    const sp: Record<(typeof PREFILLABLE_FIELDS)[number], QueryValue> = {
      licenceNumber: this.licenceNumber(),
      establishmentName: this.establishmentName(),
      legalEntityName: this.legalEntityName(),
      legalName: this.legalName(),
    }
    const prefill: Record<string, string> = {}
    for (const field of PREFILLABLE_FIELDS) {
      const value = sp[field]
      if (typeof value === "string" && value.trim() !== "") {
        prefill[field] = value
      }
    }
    return Object.keys(prefill).length ? prefill : undefined
  })
}
