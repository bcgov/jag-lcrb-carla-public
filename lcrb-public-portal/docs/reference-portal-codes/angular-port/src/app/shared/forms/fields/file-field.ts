/** FileFieldF: upload control with the AI document completeness check. */
import { ChangeDetectionStrategy, Component, computed, inject, input, signal } from "@angular/core"
import { DOCUMENTS_REPOSITORY } from "@data-access/repositories/documents/documents.repository"
import { LucideIcon } from "@shared/ui/primitives/lucide-icon"
import { cn } from "@shared/util/utils"
import type { FieldValidation } from "@domain/forms/field-validation"
import { FormField } from "../layout"
import { injectApplicationContext } from "../provider"
import { DocCheckFeedback, type DocCheckResult } from "./doc-check-readouts"
import { FieldShell, injectField } from "./field-binding"

export function fileToDataUrl(file: File): Promise<string> {
  return new Promise((resolve, reject) => {
    const reader = new FileReader()
    reader.onload = () => resolve(String(reader.result))
    reader.onerror = () => reject(reader.error)
    reader.readAsDataURL(file)
  })
}

/** Derives cross-check context (address, establishment name, area, capacity) by matching field-name patterns, since names vary across flows. */
export function deriveDocContext(formData: Record<string, unknown>): {
  applicationAddress?: string
  establishmentName?: string
  declaredArea?: number
  declaredAreaUnit?: "sqft" | "sqm"
  currentApprovedCapacity?: number
} {
  const ctx: {
    applicationAddress?: string
    establishmentName?: string
    declaredArea?: number
    declaredAreaUnit?: "sqft" | "sqm"
    currentApprovedCapacity?: number
  } = {}
  const strOf = (v: unknown) => (typeof v === "string" ? v.trim() : "")
  const numOf = (v: unknown) => {
    const n = typeof v === "number" ? v : parseFloat(strOf(v).replace(/[^0-9.]/g, ""))
    return Number.isFinite(n) ? n : undefined
  }
  // Prefer a new/proposed/premises address (e.g. relocation) over a generic one.
  const addressCandidates: { key: string; value: string }[] = []
  for (const [rawKey, val] of Object.entries(formData)) {
    if (rawKey.startsWith("__")) continue
    const key = rawKey.toLowerCase()
    const s = strOf(val)
    if (s && /(address|premises|civic|location)/.test(key) && !/email/.test(key) && !/(city|province|postal|country)/.test(key))
      addressCandidates.push({ key, value: s })
    if (!ctx.establishmentName && s && /(establishment|tradename|operatingname|storename|businessname|legalname|venuename)/.test(key))
      ctx.establishmentName = s
    if (ctx.declaredArea == null && /(floorarea|squarefoot|sqft|sqm|\barea\b)/.test(key)) {
      const n = numOf(val)
      if (n != null) {
        ctx.declaredArea = n
        ctx.declaredAreaUnit = /sqm|metre|meter/.test(key) ? "sqm" : "sqft"
      }
    }
    if (ctx.currentApprovedCapacity == null && /(current|approved|existing).*(capacity|occup)/.test(key)) {
      const n = numOf(val)
      if (n != null) ctx.currentApprovedCapacity = n
    }
  }
  if (addressCandidates.length) {
    const preferred = addressCandidates.find((c) => /(new|proposed|premises|relocat|site)/.test(c.key))
    ctx.applicationAddress = (preferred ?? addressCandidates[0]).value
  }
  return ctx
}

@Component({
  selector: "div[fileFieldF]",
  imports: [FormField, LucideIcon, DocCheckFeedback],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div formField [label]="label()" [required]="required()" [hint]="hint()" [error]="f.error()" [htmlFor]="name()">
      <label [attr.for]="name()" [attr.class]="dropClass()">
        @if (f.value()) {
          <span class="flex items-center gap-2 font-medium text-foreground break-all"><svg lucide="FileText" class="h-4 w-4 shrink-0"></svg>{{ f.value() }}</span>
        } @else {
          <span class="font-medium text-foreground">Drag a file here or browse</span>
          <span class="text-xs">PDF or image · max file size 25MB</span>
        }
        <input
          [attr.id]="name()"
          type="file"
          accept="application/pdf,image/*"
          class="sr-only"
          (change)="onChange($event)"
          (blur)="f.onBlur()"
        />
      </label>

      @if (checking()) {
        <div class="mt-2 flex items-center gap-2 rounded-md border border-border bg-muted/40 px-3 py-2 text-sm text-muted-foreground">
          <svg lucide="Loader2" class="h-4 w-4 animate-spin shrink-0"></svg>
          Checking your document against the requirements…
        </div>
      }
      @if (checkError() && !checking()) {
        <p class="mt-2 text-xs text-muted-foreground">{{ checkError() }}</p>
      }
      @if (result(); as r) {
        @if (!checking()) {
          <div docCheckFeedback [result]="r"></div>
        }
      }
    </div>
  `,
})
export class FileFieldF extends FieldShell {
  private readonly documentsRepository = inject(DOCUMENTS_REPOSITORY)
  readonly name = input.required<string>()
  readonly label = input.required<string>()
  readonly required = input<boolean | undefined>(undefined)
  readonly hint = input<string | undefined>(undefined)
  readonly validation = input<FieldValidation | undefined>(undefined)
  readonly criteria = input<string[] | undefined>(undefined)
  readonly docType = input<
    "floor-plan" | "director-info" | "incorporation" | "lease" | "lg-approval" | "certificate" | "generic" | undefined
  >(undefined)
  readonly autoPopulateField = input<string | undefined>(undefined)

  protected readonly f = injectField(() => ({
    name: this.name(),
    rules: this.validation(),
    required: this.required(),
    label: this.label(),
  }))
  private readonly ctx = injectApplicationContext()
  protected readonly checking = signal(false)
  protected readonly result = signal<DocCheckResult | null>(null)
  protected readonly checkError = signal<string | null>(null)

  protected readonly dropClass = computed(() =>
    cn(
      "flex flex-col items-center justify-center gap-1 rounded-md border-2 border-dashed border-input bg-muted/30 px-4 py-6 text-center text-sm text-muted-foreground cursor-pointer hover:bg-muted/50 transition-colors",
      this.f.error() && "border-destructive",
    ),
  )

  private effectiveCriteria(): string[] {
    const criteria = this.criteria()
    const hint = this.hint()
    return criteria && criteria.length ? criteria : hint ? [hint] : []
  }

  protected onChange(e: Event): void {
    // As in the prototype, the handler closes over the render current when the file was picked, so the async check sees that snapshot.
    const formData = this.ctx.formData()
    const flowTitle = this.assistant.bridge()?.flowTitle
    const file = (e.target as HTMLInputElement).files?.[0]
    this.f.set(file?.name ?? "")
    this.f.onBlur()
    if (file) void this.runCheck(file, formData, flowTitle)
  }

  /** Stores a one-line advisory for this document in the form data (`__doccheck:*`) and updates the running `__documentReview` note the chatbot reads. */
  private recordDocCheck(res: DocCheckResult, formData: Record<string, unknown>): void {
    const name = this.name()
    const label = this.label()
    // Use the server's unified findings, or fall back to the summary.
    const notes = res.advisories && res.advisories.length ? res.advisories : []
    const line = `${label}: ${res.status}${notes.length ? ` — ${notes.join("; ")}` : ` — ${res.summary}`}`

    // Rebuild the combined review string from all recorded docs so it stays current.
    const combined: Record<string, string> = {}
    for (const [k, v] of Object.entries(formData)) {
      if (k.startsWith("__doccheck:") && typeof v === "string") {
        try {
          const parsed = JSON.parse(v) as { note?: string }
          if (parsed.note) combined[k] = parsed.note
        } catch {
          /* ignore */
        }
      }
    }
    combined[`__doccheck:${name}`] = line
    const reviewText = Object.values(combined).join("\n")

    this.ctx.updateFormData({
      [`__doccheck:${name}`]: JSON.stringify({
        label,
        status: res.status,
        summary: res.summary,
        note: line,
        advisories: notes,
      }),
      __documentReview: reviewText,
    })
  }

  private async runCheck(file: File, formData: Record<string, unknown>, flowTitle: string | undefined) {
    this.result.set(null)
    this.checkError.set(null)
    const supported = file.type === "application/pdf" || file.type.startsWith("image/") || file.type === ""
    if (!supported) return
    this.checking.set(true)
    try {
      const fileData = await fileToDataUrl(file)
      const res = await this.documentsRepository.fetchApi("/api/document-check", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          fileData,
          filename: file.name,
          mediaType: file.type || "application/pdf",
          label: this.label(),
          criteria: this.effectiveCriteria(),
          applicationTitle: flowTitle,
          docType: this.docType() ?? "generic",
          context: deriveDocContext(formData),
        }),
      })
      if (!res.ok) {
        const data = (await res.json().catch(() => ({}))) as { error?: string }
        this.checkError.set(data.error ?? "Could not check this document right now.")
        return
      }
      const data = (await res.json()) as DocCheckResult
      this.result.set(data)
      this.recordDocCheck(data, formData)

      const autoPopulateField = this.autoPopulateField()
      if (autoPopulateField && data.occupancy?.totalLoad != null) {
        this.ctx.updateFormData({
          [autoPopulateField]: String(data.occupancy.totalLoad),
          [`${autoPopulateField}__source`]: "floor-plan-stamp",
        })
      }
    } catch {
      this.checkError.set("Could not check this document right now. You can still continue.")
    } finally {
      this.checking.set(false)
    }
  }
}
