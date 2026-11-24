/** The field library: every field type's DOM, classes and formData updates, required validation, hidden fields, licence and business fields, and the file field's document check. */
import { Component, signal, viewChild } from "@angular/core"
import { type ComponentFixture, TestBed } from "@angular/core/testing"
import { afterEach, beforeEach, describe, expect, it, vi } from "vitest"
import { stubFetch } from "../../../../tests/support/fetch-stub"
import { AssistantProvider } from "@core/assistant/assistant-provider"
import {
  ApplicationFlowState,
  ApplicationProvider,
  injectApplicationContext,
  type ApplicationStep,
} from "./index"
import {
  CheckboxFieldF,
  ConfigField,
  Conditional,
  deriveDocContext,
  isPresetField,
  proseHelpsFor,
  TextFieldF,
  type FieldConfig,
} from "./fields"

@Component({ selector: "ctx-probe", template: "" })
class Probe {
  readonly ctx = injectApplicationContext()
}

const STEPS: ApplicationStep[] = [{ id: "one", title: "One" }]
const ENTITY_OPTIONS = [
  { value: "corporation", label: "Corporation" },
  { value: "partnership", label: "Partnership" },
]

@Component({
  imports: [ApplicationProvider, ConfigField, CheckboxFieldF, TextFieldF, Conditional, Probe],
  template: `
    <application-provider [steps]="steps" [initialData]="initial">
      <ctx-probe />
      <div class="space-y-6" id="wrap">
        @for (f of fields; track f.name) {
          <div configField [field]="f"></div>
        }
        <div checkboxFieldF [name]="'__confirm'" [label]="'I confirm'" [required]="true"></div>
        <div textFieldF [name]="'direct'" [label]="'Direct field'" [validation]="{ minLength: 3 }"></div>
        <div *conditional="showCond()" textFieldF [name]="'cond'" [label]="'Cond field'" [required]="true"></div>
      </div>
    </application-provider>
  `,
})
class Host {
  steps = STEPS
  initial: Record<string, unknown> = {}
  fields: FieldConfig[] = []
  readonly showCond = signal(false)
  readonly probe = viewChild.required(Probe)
}

let fixture: ComponentFixture<Host>
let el: HTMLElement
const ctx = (): ApplicationFlowState => fixture.componentInstance.probe().ctx

async function settle() {
  fixture.detectChanges()
  await fixture.whenStable()
  await new Promise((r) => setTimeout(r, 0))
  fixture.detectChanges()
}

async function mount(fields: FieldConfig[], initial: Record<string, unknown> = {}) {
  fixture = TestBed.createComponent(Host)
  fixture.componentInstance.fields = fields
  fixture.componentInstance.initial = initial
  document.body.appendChild(fixture.nativeElement)
  el = fixture.nativeElement
  await settle()
}

const q = <T extends Element = HTMLElement>(sel: string) => el.querySelector(sel) as T
const shell = (name: string) => q(`[data-field="${name}"]`)

async function type(sel: string, value: string) {
  const input = q<HTMLInputElement | HTMLTextAreaElement>(sel)
  input.value = value
  input.dispatchEvent(new Event("input", { bubbles: true }))
  await settle()
}
async function blur(sel: string) {
  q(sel).dispatchEvent(new Event("blur"))
  await settle()
}
async function click(sel: string) {
  q(sel).click()
  await settle()
}
const options = () => [...document.querySelectorAll<HTMLElement>(".cdk-overlay-container [role='option']")]
async function pick(triggerSel: string, label: string) {
  await click(triggerSel)
  const opt = options().find((o) => o.textContent?.trim() === label)!
  opt.click()
  await settle()
}

beforeEach(() => {
  sessionStorage.clear()
  localStorage.clear()
})
afterEach(() => {
  vi.unstubAllGlobals()
  document.querySelectorAll(".cdk-overlay-container").forEach((c) => (c.innerHTML = ""))
  fixture?.nativeElement.remove()
})

describe("text-like fields", () => {
  const FIELDS: FieldConfig[] = [
    { name: "street", label: "Street Address", type: "text", required: true, placeholder: "1 Main St", hint: "Civic address" },
    { name: "mail", label: "Email", type: "email" },
    { name: "phone", label: "Phone", type: "tel" },
    { name: "seats", label: "Seats", type: "number" },
    { name: "start", label: "Start date", type: "date" },
    { name: "open", label: "Opening time", type: "time" },
    { name: "notes", label: "Notes", type: "textarea" },
  ]

  it("renders the wrapper, label, input and class strings like the prototype", async () => {
    await mount(FIELDS)
    const wrap = shell("street")
    expect(wrap.id).toBe("field-street")
    expect(wrap.getAttribute("class")).toBe("scroll-mt-24 rounded-lg transition-all")
    expect(wrap.parentElement!.id).toBe("wrap")
    expect(wrap.parentElement!.querySelector(":scope > div[configfield]")).toBe(wrap)
    const label = wrap.querySelector("label")!
    expect(label.getAttribute("for")).toBe("street")
    expect(label.textContent).toBe("Street Address*")
    const input = wrap.querySelector("input")!
    expect(input.id).toBe("street")
    expect(input.type).toBe("text")
    expect(input.getAttribute("placeholder")).toBe("1 Main St")
    expect(input.getAttribute("aria-invalid")).toBe("false")
    expect(input.getAttribute("data-slot")).toBe("input")
    expect(input.className).toContain("h-9")
    expect(input.className).toContain("w-full")
    expect(input.className.split(" ")).not.toContain("border-destructive")
    expect(wrap.querySelector("p")!.textContent).toBe("Civic address")
    for (const [name, type] of [["mail", "email"], ["phone", "tel"], ["seats", "number"], ["start", "date"], ["open", "time"]]) {
      expect(q<HTMLInputElement>(`#${name}`).type).toBe(type)
    }
    const area = q<HTMLTextAreaElement>("textarea#notes")
    expect(area.rows).toBe(4)
    expect(area.getAttribute("aria-invalid")).toBe("false")
    expect(area.hasAttribute("placeholder")).toBe(false)
  })

  it("writes typed values into formData and shows them back", async () => {
    await mount(FIELDS)
    await type("#street", "12 Wharf St")
    await type("#notes", "Some notes")
    await type("#seats", "40")
    expect(ctx().formData()).toMatchObject({ street: "12 Wharf St", notes: "Some notes", seats: "40" })
    expect(q<HTMLInputElement>("#street").value).toBe("12 Wharf St")
    expect(q<HTMLTextAreaElement>("#notes").value).toBe("Some notes")
  })

  it("seeds from initial data", async () => {
    await mount(FIELDS, { street: "Seeded" })
    expect(q<HTMLInputElement>("#street").value).toBe("Seeded")
  })

  it("shows a touched, required error with destructive classes and clears it once valid", async () => {
    await mount(FIELDS)
    expect(q("#street").getAttribute("aria-invalid")).toBe("false")
    await blur("#street")
    const input = q("#street")
    expect(input.getAttribute("aria-invalid")).toBe("true")
    expect(input.className).toContain("border-destructive focus-visible:ring-destructive")
    const err = shell("street").querySelector("p.text-destructive")!
    expect(err.textContent!.trim()).toBe("This field is required.")
    // The hint is replaced by the error.
    expect(shell("street").textContent).not.toContain("Civic address")
    await type("#street", "x")
    expect(q("#street").getAttribute("aria-invalid")).toBe("false")
    expect(shell("street").querySelector("p.text-destructive")).toBeNull()
  })

  it("renders a live warning for warnIfWithinDays on date fields", async () => {
    const soon = new Date()
    soon.setDate(soon.getDate() + 1)
    await mount([
      { name: "ev", label: "Event date", type: "date", validation: { warnIfWithinDays: { days: 30, message: "Short notice." } } },
    ])
    await type("#ev", soon.toISOString().slice(0, 10))
    const warn = shell("ev").querySelector("p[role='status']")!
    expect(warn.textContent!.trim()).toBe("Short notice.")
  })

  it("applies the validation rules of the config (minLength)", async () => {
    await mount([{ name: "n", label: "Name", type: "text", validation: { minLength: 3 } }])
    await type("#n", "ab")
    await blur("#n")
    expect(shell("n").querySelector("p.text-destructive")!.textContent).toContain("3")
  })
})

describe("provider validation and hidden fields", () => {
  it("blocks on mounted required fields (validateVisible) and names them", async () => {
    await mount([{ name: "a", label: "Field A", type: "text", required: true }])
    expect(ctx().validateVisible()).toBe(false)
    await settle()
    expect(ctx().invalidFields().map((f) => f.label).sort()).toEqual(["Field A", "I confirm"])
    expect(q("#a").getAttribute("aria-invalid")).toBe("true")
    await type("#a", "ok")
    await click("#__confirm")
    expect(ctx().validateVisible()).toBe(true)
  })

  it("does not register or render a field that is not visible, and registers it when it appears", async () => {
    await mount([
      { name: "kind", label: "Kind", type: "radio", options: [{ value: "a", label: "A" }, { value: "b", label: "B" }] },
      { name: "extra", label: "Extra", type: "text", required: true, visibleWhenEquals: { field: "kind", value: "b" } },
    ])
    expect(shell("extra")).toBeNull()
    const host = q("div[configfield] ~ div[configfield], div[configfield]:nth-of-type(2)")
    expect(host.hasAttribute("hidden")).toBe(true)
    expect(host.children.length).toBe(0)
    expect(ctx().peekBlockers().map((b) => b.name)).toEqual(["__confirm"])
    await click("#kind-b")
    expect(shell("extra")).not.toBeNull()
    expect(host.hasAttribute("hidden")).toBe(false)
    // A field that mounts later registers later, as in the prototype.
    expect(ctx().peekBlockers().map((b) => b.name)).toEqual(["__confirm", "extra"])
    await click("#kind-a")
    expect(shell("extra")).toBeNull()
    expect(ctx().peekBlockers().map((b) => b.name)).toEqual(["__confirm"])
  })

  it("Conditional mounts and unmounts its content, registering only while mounted", async () => {
    await mount([])
    expect(shell("cond")).toBeNull()
    fixture.componentInstance.showCond.set(true)
    await settle()
    expect(shell("cond")).not.toBeNull()
    expect(ctx().peekBlockers().map((b) => b.name)).toContain("cond")
    fixture.componentInstance.showCond.set(false)
    await settle()
    expect(shell("cond")).toBeNull()
    expect(ctx().peekBlockers().map((b) => b.name)).not.toContain("cond")
  })

  it("standalone fields are their own wrapper", async () => {
    await mount([])
    const direct = shell("direct")
    expect(direct.tagName).toBe("DIV")
    expect(direct.id).toBe("field-direct")
    expect(direct.parentElement!.id).toBe("wrap")
  })
})

describe("choice fields", () => {
  const SEL: FieldConfig = {
    name: "kindSel",
    label: "Premises type",
    type: "select",
    required: true,
    options: [{ value: "", label: "Blank" }, { value: "pub", label: "Pub" }, { value: "bar", label: "Bar" }],
  }

  it("select: placeholder, GUIDED_OPTION_CLASS content, blank option filtered, value to formData", async () => {
    await mount([SEL])
    const trigger = q("#kindSel")
    expect(trigger.getAttribute("role")).toBe("combobox")
    expect(trigger.getAttribute("aria-invalid")).toBe("false")
    expect(trigger.textContent!.trim()).toBe("Select an option")
    await click("#kindSel")
    expect(options().map((o) => o.textContent!.trim())).toEqual(["Pub", "Bar"])
    expect(document.querySelector(".cdk-overlay-container [data-slot='select-content']")!.className).toContain("guided-options")
    options()[1].click()
    await settle()
    expect(ctx().formData()["kindSel"]).toBe("bar")
    expect(q("#kindSel").textContent!.trim()).toBe("Bar")
    // Picking also touches the field, so a cleared required value would now show an error.
    ctx().updateFormData({ kindSel: "" })
    await settle()
    expect(q("#kindSel").getAttribute("aria-invalid")).toBe("true")
    expect(q("#kindSel").className).toContain("border-destructive")
  })

  it("radio: controlled group with ids, labels and YES_NO default", async () => {
    await mount([{ name: "alc", label: "Serve alcohol?", type: "radio", required: true }])
    const group = shell("alc").querySelector("[role='radiogroup']")!
    expect(group.getAttribute("class")).toContain("flex flex-col gap-2")
    expect(shell("alc").querySelector("label[for='alc']")).toBeNull()
    expect(q("label[for='alc-yes']").textContent).toBe("Yes")
    expect(q("label[for='alc-yes']").className).toContain("font-normal")
    await click("#alc-no")
    expect(ctx().formData()["alc"]).toBe("no")
    expect(q("#alc-no").getAttribute("aria-checked")).toBe("true")
    expect(q("#alc-yes").getAttribute("aria-checked")).toBe("false")
  })

  it("checkbox: stores a boolean, touches the field and shows the required error", async () => {
    await mount([{ name: "agree", label: "I agree", type: "checkbox", required: true }])
    const box = q("#agree")
    expect(box.getAttribute("role")).toBe("checkbox")
    expect(box.className).toContain("mt-1")
    expect(q("label[for='agree']").className).toContain("font-normal leading-relaxed")
    expect(q("label[for='agree'] span.text-destructive")!.textContent).toBe("*")
    await click("#agree")
    expect(ctx().formData()["agree"]).toBe(true)
    await click("#agree")
    expect(ctx().formData()["agree"]).toBe(false)
    expect(shell("agree").querySelector("p.text-xs.text-destructive.mt-1")!.textContent).toBe(
      "You must check this box to continue.",
    )
    expect(q("#agree").getAttribute("aria-invalid")).toBe("true")
  })

  it("checkbox-group: toggles values in order and validates required", async () => {
    await mount([
      {
        name: "types",
        label: "Types",
        type: "checkbox-group",
        required: true,
        options: [{ value: "a", label: "Alpha" }, { value: "b", label: "Beta" }],
      },
    ])
    await click("#types-b")
    await click("#types-a")
    expect(ctx().formData()["types"]).toEqual(["b", "a"])
    await click("#types-b")
    expect(ctx().formData()["types"]).toEqual(["a"])
    await click("#types-a")
    expect(shell("types").querySelector("p.text-destructive")!.textContent!.trim()).toBe("This field is required.")
  })
})

describe("business-bound fields", () => {
  it("licence select: combobox named by its label, fills the establishment name", async () => {
    await mount([
      { name: "licenceNumber", label: "Licence Number", type: "text", required: true },
      { name: "establishmentName", label: "Establishment Name", type: "text" },
    ])
    const trigger = q("#licenceNumber")
    expect(trigger.getAttribute("role")).toBe("combobox")
    expect(q("label[for='licenceNumber']").textContent).toBe("Licence Number*")
    expect(trigger.textContent!.trim()).toBe("Select one of your licences")
    await click("#licenceNumber")
    const label = options()[0].textContent!.trim()
    expect(label).toMatch(/^326739 — OneStop Test 1 \(Liquor Primary\)$/)
    options()[0].click()
    await settle()
    expect(ctx().formData()).toMatchObject({ licenceNumber: "326739", establishmentName: "OneStop Test 1" })
    expect(q("#licenceNumber").textContent!.trim()).toBe(label)
    expect(q<HTMLInputElement>("#establishmentName").value).toBe("OneStop Test 1")
  })

  it("licence select keeps a historical value selectable", async () => {
    await mount([{ name: "licenseNumber", label: "Licence Number", type: "text" }], { licenseNumber: "OLD-1" })
    expect(q("#licenseNumber").textContent!.trim()).toBe("OLD-1")
  })

  it("establishment select (type select) offers the business's establishments; free text otherwise", async () => {
    await mount([{ name: "establishmentName", label: "Establishment Name", type: "select" }])
    expect(q("#establishmentName").getAttribute("role")).toBe("combobox")
    await pick("#establishmentName", "OneStop Test 1")
    expect(ctx().formData()["establishmentName"]).toBe("OneStop Test 1")
    fixture.nativeElement.remove()
    await mount([{ name: "establishmentName", label: "Proposed establishment name", type: "text" }])
    expect(q("input#establishmentName")).not.toBeNull()
    expect(q("input#establishmentName").getAttribute("role")).toBeNull()
  })

  it("auto-fills and locks business identity fields", async () => {
    await mount([
      { name: "legalName", label: "Legal name", type: "text" },
      { name: "businessNumber", label: "Business number", type: "text" },
      { name: "incorporationNumber", label: "Incorporation number", type: "text" },
    ])
    const name = q<HTMLInputElement>("#legalName")
    expect(name.value).toBe("LCRB Training Business 5 Ltd.")
    expect(name.readOnly).toBe(true)
    expect(name.getAttribute("aria-readonly")).toBe("true")
    expect(name.tabIndex).toBe(-1)
    expect(name.className).toContain("bg-muted text-muted-foreground cursor-not-allowed pr-9")
    expect(shell("legalName").querySelector("svg.lucide-lock")!.getAttribute("class")).toContain(
      "pointer-events-none absolute right-3 top-1/2",
    )
    expect(shell("legalName").textContent).toContain("Auto-filled from your BCeID business account · cannot be changed")
    expect(ctx().formData()).toMatchObject({
      legalName: "LCRB Training Business 5 Ltd.",
      businessNumber: "BC0987654",
      incorporationNumber: "BC1234567",
    })
  })

  it("entity type: locked to the resolved option value when it matches, an editable select otherwise", async () => {
    await mount([{ name: "entityType", label: "Entity type", type: "select", options: ENTITY_OPTIONS }])
    const input = q<HTMLInputElement>("input#entityType")
    expect(input.value).toBe("Corporation")
    expect(ctx().formData()["entityType"]).toBe("corporation")
    expect(shell("entityType").textContent).toContain("Auto-filled from your BC Registries record · cannot be changed")
    fixture.nativeElement.remove()
    await mount([{ name: "entityType", label: "Entity type", type: "select", options: [{ value: "x", label: "Other" }] }])
    expect(q("button#entityType")!.getAttribute("role")).toBe("combobox")
    expect(ctx().formData()["entityType"]).toBeUndefined()
  })

  it("classifies preset fields for Guided Mode", () => {
    expect(isPresetField({ name: "legalName", label: "", type: "text" })).toBe(true)
    expect(isPresetField({ name: "licenceNumber", label: "", type: "text" })).toBe(true)
    expect(isPresetField({ name: "establishmentName", label: "", type: "select" })).toBe(true)
    expect(isPresetField({ name: "establishmentName", label: "", type: "text" })).toBe(false)
    expect(proseHelpsFor({ name: "legalName", label: "", type: "text" })).toBe(false)
  })
})

describe("assistant highlight", () => {
  it("adds the highlight classes for the highlighted field and clears them on focus of another", async () => {
    await mount([
      { name: "one", label: "One", type: "text" },
      { name: "two", label: "Two", type: "text" },
    ])
    Element.prototype.scrollIntoView = vi.fn()
    const assistant = TestBed.inject(AssistantProvider)
    assistant.highlightField("one")
    await settle()
    expect(shell("one").getAttribute("class")).toBe(
      "scroll-mt-24 rounded-lg transition-all ring-2 ring-offset-2 ring-[#1A5A96] bg-[#1A5A96]/5 p-3 -m-3 shadow-[0_0_0_4px_rgba(26,90,150,0.15)]",
    )
    // Focusing the highlighted field keeps it; another field clears it.
    q("#one").dispatchEvent(new FocusEvent("focusin", { bubbles: true }))
    expect(assistant.highlightedField()).toBe("one")
    q("#two").dispatchEvent(new FocusEvent("focusin", { bubbles: true }))
    await settle()
    expect(assistant.highlightedField()).toBeNull()
    expect(shell("one").getAttribute("class")).toBe("scroll-mt-24 rounded-lg transition-all")
  })
})

describe("help context", () => {
  it("shows the config's help beside the label via FieldHelpContext", async () => {
    await mount([
      { name: "h", label: "Helpful", type: "text", help: "Plain words" },
      { name: "nohelp", label: "Plain", type: "text" },
    ])
    expect(shell("h").querySelector("[data-field-help] button")!.getAttribute("aria-label")).toBe('What does "Helpful" mean?')
    expect(shell("nohelp").querySelector("[data-field-help]")).toBeNull()
  })
})

describe("file field and document check", () => {
  const FILE_FIELD: FieldConfig = {
    name: "plan",
    label: "Floor plan",
    type: "file",
    required: true,
    hint: "Upload the plan",
    docType: "floor-plan",
    autoPopulateField: "occupancy",
  }
  const RESULT = {
    status: "incomplete",
    summary: "The plan is mostly readable.",
    missing: ["Scale"],
    advisories: ["Add a scale"],
    occupancy: {
      stampFound: true,
      spaces: [{ label: "Dining", load: 40 }],
      totalLoad: 40,
      stampDate: "2025-01-01",
      validUntil: "2026-01-01",
      monthsOld: 14,
      stampExpired: true,
      floorArea: { value: 1200, unit: "sqft" },
      scaleNoted: false,
    },
  }

  async function pickFile(file: File) {
    const input = q<HTMLInputElement>("input#plan")
    Object.defineProperty(input, "files", { value: [file], configurable: true })
    input.dispatchEvent(new Event("change", { bubbles: true }))
    await settle()
    for (let i = 0; i < 50 && q("div[doccheckfeedback]") == null && !shell("plan").textContent!.includes("Could not"); i++) {
      await new Promise((r) => setTimeout(r, 10))
      await settle()
    }
  }

  it("renders the drop zone and the native file input", async () => {
    await mount([FILE_FIELD])
    const label = shell("plan").querySelectorAll("label")[1]
    expect(label.getAttribute("for")).toBe("plan")
    expect(label.getAttribute("class")).toBe(
      "flex flex-col items-center justify-center gap-1 rounded-md border-2 border-dashed border-input bg-muted/30 px-4 py-6 text-center text-sm text-muted-foreground cursor-pointer hover:bg-muted/50 transition-colors",
    )
    expect(label.textContent).toContain("Drag a file here or browse")
    expect(label.textContent).toContain("PDF or image · max file size 25MB")
    const input = q<HTMLInputElement>("input#plan")
    expect(input.type).toBe("file")
    expect(input.accept).toBe("application/pdf,image/*")
    expect(input.className).toBe("sr-only")
  })

  it("posts the document, stores __doccheck JSON, auto-populates and renders the readouts", async () => {
    const api = stubFetch({ "/api/document-check": { json: RESULT } })
    await mount([FILE_FIELD, { name: "occupancy", label: "Occupancy", type: "text" }, { name: "premisesAddress", label: "Address", type: "text" }], {
      premisesAddress: "1 Main St",
    })
    await pickFile(new File(["%PDF-1.4"], "plan.pdf", { type: "application/pdf" }))
    const calls = api.callsTo("/api/document-check")
    expect(calls).toHaveLength(1)
    const body = calls[0].body as Record<string, unknown>
    expect(body).toMatchObject({
      filename: "plan.pdf",
      mediaType: "application/pdf",
      label: "Floor plan",
      criteria: ["Upload the plan"],
      docType: "floor-plan",
      context: { applicationAddress: "1 Main St" },
    })
    expect(String(body["fileData"])).toMatch(/^data:application\/pdf;base64,/)

    const stored = JSON.parse(ctx().formData()["__doccheck:plan"] as string)
    expect(stored).toEqual({
      label: "Floor plan",
      status: "incomplete",
      summary: "The plan is mostly readable.",
      note: "Floor plan: incomplete — Add a scale",
      advisories: ["Add a scale"],
    })
    expect(ctx().formData()["__documentReview"]).toBe("Floor plan: incomplete — Add a scale")
    expect(ctx().formData()).toMatchObject({ plan: "plan.pdf", occupancy: "40", occupancy__source: "floor-plan-stamp" })
    expect(shell("plan").textContent).toContain("plan.pdf")

    const fb = q("div[doccheckfeedback]")
    expect(fb.getAttribute("role")).toBe("status")
    expect(fb.getAttribute("aria-live")).toBe("polite")
    expect(fb.getAttribute("class")).toBe(
      "mt-2 rounded-md border px-3 py-2.5 text-sm border-[#F2A900]/50 bg-[#F2A900]/10 text-[#8A6100]",
    )
    expect(fb.textContent).toContain("May be missing something")
    expect(fb.textContent).toContain("Please check that your document includes:")
    const readout = fb.querySelector("div[occupancyreadout]")!
    expect(readout.getAttribute("class")).toBe("mt-1 rounded border border-border bg-background/60 px-2.5 py-2")
    expect(readout.textContent).toContain("Occupancy load read from stamp: 40 persons")
    expect(readout.textContent).toContain("Dining: 40")
    expect(readout.textContent).toContain("This occupancy stamp is dated 2025-01-01 and is more than 12 months old")
    expect(readout.textContent).toContain("Floor area read from the plan: 1200 sqft.")
    expect(readout.textContent).toContain("No scale or labelled dimensions were found")
  })

  it("renders the corporate, lease, LG approval and certificate readouts", async () => {
    stubFetch({
      "/api/document-check": {
        json: {
          status: "complete",
          summary: "Fine.",
          corporate: {
            found: true,
            companyName: "Acme Ltd.",
            incorporationNumber: "BC1",
            directors: ["A", "B"],
            companyStatus: "Active",
            registry: { status: "verified", simulated: true, directors: [], summary: "All matched." },
            fitAndProper: {
              simulated: false,
              people: [
                { name: "A", found: true, status: "cleared", note: "ok" },
                { name: "B", found: true, status: "flagged", note: "see file" },
              ],
              outstanding: 0,
              flagged: 1,
              summary: "One flagged.",
            },
          },
          lease: { premisesAddress: "9 St", landlord: "L", tenant: "T", termStart: null, termEnd: "2020-01-01", signed: false, termExpired: true, addressMatchesApplication: false },
          lgApproval: { governmentBody: "City of X", establishmentName: "Pub", establishmentAddress: null, decision: "oppose", decisionDate: "2025-02-02", addressMatchesApplication: false, policyGuidance: { note: "Guidance", source: "Policy 1" } },
          certificate: { status: "expired", simulated: true, program: "selling-it-right", holderName: "H", certificateNumber: "C9", summary: "Expired cert." },
        },
      },
    })
    await mount([{ ...FILE_FIELD, autoPopulateField: undefined }])
    await pickFile(new File(["x"], "doc.png", { type: "image/png" }))
    const fb = q("div[doccheckfeedback]")
    expect(fb.getAttribute("class")).toContain("border-[#2E8540]/40 bg-[#2E8540]/10 text-[#1E5631]")
    const corp = fb.querySelector("div[corporatereadout]")!.textContent!
    expect(corp).toContain("Acme Ltd. (BC1)")
    expect(corp).toContain("Registration status on document: Active.")
    expect(corp).toContain("Directors: A, B")
    expect(corp).toContain("Simulated BC Registries check — All matched.")
    expect(corp).toContain("Fit-and-proper / screening — One flagged.")
    expect(corp).toContain("B (needs review): see file")
    expect(corp).not.toContain("A (")
    const lease = fb.querySelector("div[leasereadout]")!.textContent!
    expect(lease).toContain("Landlord: L · Tenant: T")
    expect(lease).toContain("Term: ? to 2020-01-01")
    expect(lease).toContain("This lease term appears to have ended (2020-01-01).")
    expect(lease).toContain("The lease does not appear to be signed by both parties.")
    const lg = fb.querySelector("div[lgapprovalreadout]")!.textContent!
    expect(lg).toContain("City of X opposes the application (dated 2025-02-02).")
    expect(lg).toContain("Guidance (Policy 1)")
    expect(fb.querySelector("div[certificatereadout]")!.textContent).toContain("Selling It Right certificate")
    expect(fb.querySelector("div[certificatereadout]")!.textContent).toContain("Simulated registry check — Expired cert.")
    expect(fb.querySelector("div[certificatereadout]")!.textContent).toContain("This certificate has expired.")
  })

  it("shows the server's error text, and a generic one when the request throws", async () => {
    stubFetch({ "/api/document-check": { status: 500, json: { error: "Too large." } } })
    await mount([FILE_FIELD])
    await pickFile(new File(["x"], "a.pdf", { type: "application/pdf" }))
    expect(shell("plan").querySelector("p.text-xs.text-muted-foreground.mt-2")!.textContent).toBe("Too large.")
    expect(ctx().formData()["__doccheck:plan"]).toBeUndefined()
    fixture.nativeElement.remove()

    vi.stubGlobal("fetch", vi.fn(async () => Promise.reject(new Error("offline"))))
    await mount([FILE_FIELD])
    await pickFile(new File(["x"], "a.pdf", { type: "application/pdf" }))
    expect(shell("plan").textContent).toContain("Could not check this document right now. You can still continue.")
  })

  it("does not check unsupported file types, but still records the file name", async () => {
    const api = stubFetch({ "/api/document-check": { json: RESULT } })
    await mount([FILE_FIELD])
    const input = q<HTMLInputElement>("input#plan")
    Object.defineProperty(input, "files", { value: [new File(["x"], "a.txt", { type: "text/plain" })], configurable: true })
    input.dispatchEvent(new Event("change", { bubbles: true }))
    await settle()
    expect(api.calls).toHaveLength(0)
    expect(ctx().formData()["plan"]).toBe("a.txt")
  })

  it("shows the checking indicator while the request is in flight", async () => {
    let release!: () => void
    vi.stubGlobal(
      "fetch",
      vi.fn(() => new Promise<Response>((r) => (release = () => r(new Response(JSON.stringify({ status: "unclear", summary: "?" }), { status: 200 }))))),
    )
    await mount([FILE_FIELD])
    const input = q<HTMLInputElement>("input#plan")
    Object.defineProperty(input, "files", { value: [new File(["x"], "a.pdf", { type: "application/pdf" })], configurable: true })
    input.dispatchEvent(new Event("change", { bubbles: true }))
    for (let i = 0; i < 50 && !shell("plan").textContent!.includes("Checking your document"); i++) {
      await new Promise((r) => setTimeout(r, 10))
      await settle()
    }
    expect(shell("plan").textContent).toContain("Checking your document against the requirements…")
    release()
    for (let i = 0; i < 50 && q("div[doccheckfeedback]") == null; i++) {
      await new Promise((r) => setTimeout(r, 10))
      await settle()
    }
    expect(q("div[doccheckfeedback]").getAttribute("class")).toContain("border-border bg-muted/50 text-foreground")
    expect(q("div[doccheckfeedback]").textContent).toContain("Couldn't fully read this file")
  })
})

describe("deriveDocContext", () => {
  it("derives address, name, area and capacity from loosely named fields", () => {
    expect(
      deriveDocContext({
        streetAddress: "1 A St",
        newPremisesAddress: "2 B St",
        city: "Vic",
        email_address: "x@y.z",
        establishmentName: "Pub",
        floorAreaSqm: "120 m2",
        currentCapacity: 80,
        __hidden: "skip address",
      }),
    ).toEqual({
      applicationAddress: "2 B St",
      establishmentName: "Pub",
      declaredArea: 1202,
      declaredAreaUnit: "sqm",
      currentApprovedCapacity: 80,
    })
  })
})
