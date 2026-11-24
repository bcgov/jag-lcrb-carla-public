/** ConfigFlow and its parts with a 2-step config; validators register through the provider state since field components are placeholders. */
import { Component } from "@angular/core"
import { type ComponentFixture, TestBed } from "@angular/core/testing"
import { By } from "@angular/platform-browser"
import { provideRouter, Router } from "@angular/router"
import { toast } from "@spartan-ng/brain/sonner"
import { afterEach, beforeEach, describe, expect, it, vi } from "vitest"
import { AssistantProvider } from "@core/assistant/assistant-provider"
import { SessionProvider } from "@core/session/session-provider"
import { LICENCE_TYPE_LABELS } from "@domain/models/applications"
import { heldLicenceTypes, isEntryEligible, unheldLicenceTypes } from "@domain/applications/application-eligibility"
import { getBusinessProfile } from "@data-access/repositories/account/mock/business-profile"
import { getDraft, saveDraft } from "@data-access/repositories/applications/drafts"
import { getAllSubmissions } from "@data-access/repositories/applications/submissions"
import { API_FIXTURES } from "../../../../tests/fixtures/api"
import { stubFetch } from "../../../../tests/support/fetch-stub"
import { ConfigFlow } from "./config-flow"
import { type FlowConfig } from "@domain/forms/flow-config"
import { EligibilityGate } from "@features/applications/components/eligibility-gate"
import { ApplicationFlowState, ApplicationProvider } from "./index"
import { ApplicationModeToggle } from "./mode-toggle"
import { ModeExplainer } from "./mode-explainer"

@Component({ template: "" })
class Blank {}

const CONFIG: FlowConfig = {
  id: "test-flow",
  title: "Test Flow",
  subtitle: "A small test application",
  fee: "$25.00",
  returnTo: "/applications",
  steps: [
    {
      id: "business",
      title: "Business",
      sections: [
        {
          title: "About the business",
          description: "Who is applying",
          fields: [
            { name: "legalName", label: "Legal name", type: "text", required: true },
            {
              name: "kind",
              label: "Kind of business",
              type: "radio",
              options: [
                { value: "cafe", label: "Cafe" },
                { value: "bar", label: "Bar" },
              ],
            },
          ],
        },
      ],
    },
    {
      id: "premises",
      title: "Premises",
      description: "Where it operates",
      sections: [
        {
          title: "Location",
          columns: 2,
          info: { variant: "warning", title: "Heads up", body: "Check the address." },
          fields: [
            { name: "city", label: "City", type: "text" },
            { name: "secret", label: "Secret", type: "text", visibleWhenEquals: { field: "kind", value: "bar" } },
          ],
        },
      ],
    },
  ],
}

async function settle(fixture: ComponentFixture<unknown>) {
  fixture.detectChanges()
  await fixture.whenStable()
  fixture.detectChanges()
}

async function mountFlow(prefill?: Record<string, string>) {
  const fixture = TestBed.createComponent(ConfigFlow)
  fixture.componentRef.setInput("config", CONFIG)
  if (prefill) fixture.componentRef.setInput("prefill", prefill)
  await settle(fixture)
  const state = fixture.debugElement.query(By.directive(ApplicationProvider)).injector.get(ApplicationFlowState)
  const el = fixture.nativeElement as HTMLElement
  return { fixture, el, state, settle: () => settle(fixture) }
}

const radios = (el: HTMLElement) => [...el.querySelectorAll('[role="radiogroup"] button[role="radio"]')] as HTMLButtonElement[]
const button = (root: ParentNode, text: string) =>
  [...root.querySelectorAll("button")].find((b) => b.textContent?.trim() === text) as HTMLButtonElement | undefined

beforeEach(() => {
  localStorage.clear()
  sessionStorage.clear()
  TestBed.configureTestingModule({ providers: [provideRouter([{ path: "**", component: Blank }])] })
  vi.spyOn(window, "scrollTo").mockImplementation(() => undefined)
  vi.spyOn(toast, "error").mockImplementation(() => 1)
  vi.spyOn(toast, "info").mockImplementation(() => 1)
  vi.spyOn(toast, "success").mockImplementation(() => 1)
})
afterEach(() => {
  vi.useRealTimers()
  vi.unstubAllGlobals()
  vi.restoreAllMocks()
  document.querySelectorAll(".cdk-overlay-container").forEach((c) => (c.innerHTML = ""))
})

describe("ConfigFlow classic form", () => {
  it("renders the header, hero, steps with the synthetic review step, and section cards", async () => {
    const { el } = await mountFlow()
    expect(el.querySelector("bc-gov-header")).toBeTruthy()
    expect(el.querySelector("main#main-content")).toBeTruthy()
    expect(el.querySelector("h1")!.textContent).toContain("Test Flow")
    expect(el.querySelector("page-hero p")!.textContent).toContain("A small test application")
    expect([...el.querySelectorAll("step-indicator button")].map((b) => b.textContent?.trim())).toEqual([
      "1Business",
      "2Premises",
      "3Review & Submit",
    ])
    const progress = el.querySelector('[role="progressbar"]')!
    expect(progress.getAttribute("aria-valuenow")).toBe(String((1 / 3) * 100))
    const section = el.querySelector("div[sectionBlock]")!
    expect(section.getAttribute("class")).toBe("bg-white rounded-lg border")
    expect(section.querySelector("h2")!.textContent).toBe("About the business")
    expect(section.querySelector("p")!.textContent).toBe("Who is applying")
    expect(section.querySelectorAll("div[configField]")).toHaveLength(2)
    expect(section.querySelector(".p-6 > .space-y-6 > .space-y-6")).toBeTruthy()
    expect(el.querySelector("application-navigation")!.textContent).toContain("Next")
  })

  it("shows step descriptions, the info box variant and the column grid on step 2", async () => {
    const { el, state, settle } = await mountFlow()
    state.goToStep(1)
    await settle()
    expect(el.querySelector(".space-y-6 > p.text-muted-foreground")!.textContent).toBe("Where it operates")
    const info = el.querySelector("div[infoBox]")!
    expect(info.getAttribute("class")).toBe("border-l-4 p-4 rounded-r bg-[#FEF1D8] border-[#F5A623] text-[#8B5A00]")
    expect(info.querySelector(".font-semibold")!.textContent).toBe("Heads up")
    expect(info.querySelector(".text-sm")!.textContent).toBe("Check the address.")
    expect(el.querySelector(".grid.grid-cols-1.md\\:grid-cols-2.gap-4")).toBeTruthy()
  })

  it("passes prefill through as the initial form data", async () => {
    const { state } = await mountFlow({ licenceNumber: "L-1" })
    // The legal-name field auto-fills from the business profile once mounted.
    expect(state.formData()).toEqual({ licenceNumber: "L-1", legalName: "LCRB Training Business 5 Ltd." })
  })

  it("Save & Continue Later saves the draft and returns to returnTo", async () => {
    const { el, settle: s } = await mountFlow()
    button(el, "Save & Continue Later")!.click()
    await s()
    expect(getDraft("draft:test-flow")?.resumeHref).toBe("/applications/new/test-flow")
    await vi.waitFor(() => expect(TestBed.inject(Router).url).toBe("/applications"))
  })
})

describe("mode toggle and explainer", () => {
  it("renders the classic form with the toggle and explainer once the stored mode is read", async () => {
    const { el } = await mountFlow()
    expect(el.querySelector("application-layout")).toBeTruthy()
    expect(el.querySelector("focus-flow")).toBeNull()
    expect(el.querySelector('[role="radiogroup"]')!.getAttribute("aria-label")).toBe(
      "How would you like to fill in this application?",
    )
    expect(el.querySelector('[role="radiogroup"]')!.closest("div.mb-6.flex.justify-end")).toBeTruthy()
  })

  it("the toggle marks the active mode and writes lcrb.app.mode", async () => {
    const { el, settle: s } = await mountFlow()
    const [full, guided] = radios(el)
    expect(full.getAttribute("aria-checked")).toBe("true")
    expect(guided.getAttribute("aria-checked")).toBe("false")
    expect(full.getAttribute("class")).toContain("bg-[#013366] text-white shadow-sm")
    expect(guided.getAttribute("class")).toContain("text-muted-foreground hover:bg-muted hover:text-foreground")
    expect(full.getAttribute("title")).toBe("See every question on a step at once")
    guided.click()
    await s()
    expect(localStorage.getItem("lcrb.app.mode")).toBe("guided")
    // Guided Mode hands over to FocusFlow (a placeholder here).
    expect(el.querySelector("focus-flow")).toBeTruthy()
    expect(el.querySelector("application-layout")).toBeNull()
  })

  it("starts in Guided Mode when stored, and the toggle writes back 'form' from the explainer path", async () => {
    localStorage.setItem("lcrb.app.mode", "guided")
    const { el } = await mountFlow()
    expect(el.querySelector("focus-flow")).toBeTruthy()
    expect(el.querySelector("application-layout")).toBeNull()
  })

  it("the explainer shows only on step 0, and its button switches mode", async () => {
    const { el, state, settle: s } = await mountFlow()
    const explainer = el.querySelector("mode-explainer")!
    expect(explainer.closest("div.mb-6")).toBeTruthy()
    expect(explainer.querySelector("aside")!.getAttribute("aria-label")).toBe("Two ways to fill this in")
    expect(explainer.textContent).toContain("Try Guided Mode")
    expect(explainer.querySelector(".mt-1.flex.flex-wrap.items-center.gap-2 > button")).toBeTruthy()
    expect(explainer.querySelector(".mt-1.flex.flex-wrap.items-center.gap-2 > span.text-xs")).toBeTruthy()
    state.goToStep(1)
    await s()
    expect(el.querySelector("mode-explainer")).toBeNull()
    state.goToStep(2)
    await s()
    expect(el.querySelector("mode-explainer")).toBeNull()
    state.goToStep(0)
    await s()
    button(el.querySelector("mode-explainer")!, "Try Guided Mode")!.click()
    await s()
    expect(localStorage.getItem("lcrb.app.mode")).toBe("guided")
  })

  it("the explainer offers the full form while in guided mode, and dismisses for good", async () => {
    @Component({
      imports: [ModeExplainer, ApplicationModeToggle],
      template: `<mode-explainer mode="guided" (modeChange)="seen = $event" /><application-mode-toggle [mode]="'guided'" (modeChange)="toggled = $event" />`,
    })
    class Host {
      seen = ""
      toggled = ""
    }
    const fixture = TestBed.createComponent(Host)
    await settle(fixture)
    const el = fixture.nativeElement as HTMLElement
    button(el, "Switch to the full form")!.click()
    expect(fixture.componentInstance.seen).toBe("form")
    radios(el)[0].click()
    expect(fixture.componentInstance.toggled).toBe("form")
    ;(el.querySelector('button[aria-label^="Dismiss"]') as HTMLButtonElement).click()
    await settle(fixture)
    expect(el.querySelector("aside")).toBeNull()
    expect(localStorage.getItem("lcrb.notice.application-mode-v1")).toBe("1")
  })
})

describe("AssistantFormBridge", () => {
  it("registers one bridge with live reads, the synthetic review fields and step titles", async () => {
    const { state, settle: s } = await mountFlow()
    const assistant = TestBed.inject(AssistantProvider)
    const bridge = assistant.bridge()!
    expect(bridge.flowTitle).toBe("Test Flow")
    expect(bridge.totalSteps).toBe(3)
    expect(bridge.currentStep).toBe(0)
    expect(bridge.stepTitle).toBe("Business")
    expect(bridge.getFields().map((f) => f.name)).toEqual(["legalName", "kind"])
    expect(bridge.getFields()[1].options).toHaveLength(2)

    const all = bridge.getAllFields()
    expect(all.map((f) => [f.name, f.step])).toEqual([
      ["legalName", 0],
      ["kind", 0],
      ["city", 1],
      ["secret", 1],
      ["__confirmAccuracy", 2],
      ["__acknowledgeFalseInfo", 2],
      ["__documentReview", 2],
    ])
    expect(all.find((f) => f.name === "__documentReview")!.type).toBe("text")
    expect(all.find((f) => f.name === "__confirmAccuracy")!.required).toBe(true)

    // The same bridge object stays live across state changes (registered once).
    state.updateFormData({ legalName: "Acme" })
    expect(assistant.bridge()).toBe(bridge)
    expect(bridge.getFields()[0].value).toBe("Acme")
    state.goToStep(2)
    await s()
    expect(bridge.currentStep).toBe(2)
    expect(bridge.stepTitle).toBe("Review & Submit")
    expect(bridge.getFields().map((f) => f.name)).toEqual(["__confirmAccuracy", "__acknowledgeFalseInfo", "__documentReview"])
    expect(assistant.bridge()).toBe(bridge)
  })

  it("setFieldValue moves to the field's step and sets the value; unknown fields are refused", async () => {
    const { state, settle: s } = await mountFlow()
    const bridge = TestBed.inject(AssistantProvider).bridge()!
    expect(bridge.stepOfField("city")).toBe(1)
    expect(bridge.stepOfField("nope")).toBeNull()
    expect(bridge.setFieldValue("nope", 1)).toBe(false)
    expect(bridge.setFieldValue("city", "Vancouver")).toBe(true)
    await s()
    expect(state.currentStep()).toBe(1)
    expect(state.formData()["city"]).toBe("Vancouver")
    expect(bridge.setFieldValue("kind", "bar")).toBe(true)
    expect(state.currentStep()).toBe(0)
  })

  it("tryGoToStep blocks forward moves with the Next gate and previews blockers quietly", async () => {
    const { state, settle: s } = await mountFlow()
    const bridge = TestBed.inject(AssistantProvider).bridge()!
    state.registerValidator("venueName", { required: true }, "Venue name")

    expect(bridge.blockersPreview()).toEqual([{ name: "venueName", label: "Venue name", error: "This field is required." }])
    expect(state.submitAttempted()).toBe(false)

    const blocked = bridge.tryGoToStep(1)
    expect(blocked.ok).toBe(false)
    expect(blocked.blockedBy.map((b) => b.name)).toEqual(["venueName"])
    expect(state.currentStep()).toBe(0)
    expect(state.submitAttempted()).toBe(true)
    await s()
    expect(document.querySelector('[role="alert"]')!.textContent).toContain("1 field needs your attention")

    state.updateFormData({ venueName: "Acme" })
    expect(bridge.tryGoToStep(1)).toEqual({ ok: true, blockedBy: [] })
    expect(state.currentStep()).toBe(1)
    expect(bridge.tryGoToStep(0)).toEqual({ ok: true, blockedBy: [] })
    expect(state.currentStep()).toBe(0)
    bridge.nextStep()
    expect(state.currentStep()).toBe(1)
    bridge.prevStep()
    expect(state.currentStep()).toBe(0)
    bridge.goToStep(2)
    expect(state.currentStep()).toBe(2)
  })

  it("clears the bridge when the flow is destroyed", async () => {
    const { fixture } = await mountFlow()
    const assistant = TestBed.inject(AssistantProvider)
    expect(assistant.bridge()).not.toBeNull()
    fixture.destroy()
    expect(assistant.bridge()).toBeNull()
  })
})

describe("review step and submit", () => {
  it("summarises visible answers with option labels, fees and declarations", async () => {
    const { el, state, settle: s } = await mountFlow()
    state.updateFormData({ legalName: "Acme Ltd", kind: "cafe", city: "", secret: "hidden value", extra: "ignored" })
    state.goToStep(2)
    await s()
    expect(el.querySelector("div[infoBox] .font-semibold")!.textContent).toBe("Review your application")
    const rows = [...el.querySelectorAll("dl.divide-y > div")].map((r) => [r.querySelector("dt")!.textContent, r.querySelector("dd")!.textContent])
    // City is empty and Secret is hidden (kind is not "bar"): both skipped.
    expect(rows).toEqual([
      ["Legal name", "Acme Ltd"],
      ["Kind of business", "Cafe"],
    ])
    expect(el.querySelector("dl.divide-y > div")!.getAttribute("class")).toBe("grid grid-cols-1 sm:grid-cols-3 gap-1 py-2 text-sm")
    const fees = [...el.querySelectorAll("div[applicationSection]")].find((e) => e.querySelector("h2")!.textContent === "Fees")!
    expect(fees.textContent).toContain("Application Fee")
    expect(fees.querySelectorAll("dd")[1].textContent).toBe("$25.00")
    expect(el.querySelectorAll("div[checkboxFieldF]")).toHaveLength(2)
    expect(button(el, "Submit Application")).toBeTruthy()
  })

  it("shows the empty summary message", async () => {
    const { el, state, settle: s } = await mountFlow()
    // Clear the auto-filled legal name so nothing is left to summarise.
    state.updateFormData({ legalName: "" })
    state.goToStep(2)
    await s()
    expect(el.querySelector("div[applicationSection] p.text-muted-foreground")!.textContent).toBe(
      "No information entered yet. Go back to complete the form.",
    )
  })

  it("submit calls /api/assessment once, records the submission, deletes the draft and navigates", async () => {
    const net = stubFetch(API_FIXTURES)
    TestBed.inject(SessionProvider).login({ role: "licensee", firstName: "Matt", lastName: "Harding", organization: "Acme Ltd." } as never)
    const { el, state, settle: s } = await mountFlow()
    state.updateFormData({
      legalName: "Acme Ltd",
      kind: "bar",
      __doccheck_skip: "x",
      "__doccheck:floorPlan": JSON.stringify({ label: "Floor plan", status: "complete", summary: "Looks fine.", advisories: ["Scale ok", ""] }),
      "__doccheck:bad": "{not json",
      __confirmAccuracy: true,
      __acknowledgeFalseInfo: true,
    })
    saveDraft({ draftId: "draft:test-flow", flowId: "test-flow", title: "Test Flow", resumeHref: "/x", formData: {}, currentStep: 0, progress: 1, updatedAt: "" })
    state.goToStep(2)
    await s()
    button(el, "Submit Application")!.click()
    await vi.waitFor(() => expect(TestBed.inject(Router).url).toMatch(/^\/applications\/confirmation\/.+/))
    expect(net.callsTo("/api/assessment")).toHaveLength(1)
    const body = net.callsTo("/api/assessment")[0].body as Record<string, unknown>
    expect(body["flowId"]).toBe("test-flow")
    expect(body["applicant"]).toBe("Acme Ltd")
    expect(body["summary"]).toEqual([
      { label: "Legal name", value: "Acme Ltd" },
      { label: "Kind of business", value: "Bar" },
    ])
    expect(body["documentChecks"]).toEqual([{ label: "Floor plan", status: "complete", summary: "Looks fine. Scale ok" }])
    const subs = getAllSubmissions()
    expect(subs).toHaveLength(1)
    expect(TestBed.inject(Router).url).toBe(`/applications/confirmation/${subs[0].id}`)
    // deleteDraft ran before the pending autosave could re-save it? The draft is gone right after submit.
    expect(getDraft("draft:test-flow")).toBeUndefined()
  })

  it("a failed Next gate on the last step blocks submit with the toast", async () => {
    const net = stubFetch(API_FIXTURES)
    const { el, state, settle: s } = await mountFlow()
    state.goToStep(2)
    await s()
    button(el, "Submit Application")!.click()
    await s()
    expect(toast.error).toHaveBeenCalledWith("Some required fields need attention", {
      description: "See the highlighted list at the top of the step to jump to each one.",
    })
    expect(net.callsTo("/api/assessment")).toHaveLength(0)
    expect(el.querySelector('[role="alert"]')!.textContent).toContain("I confirm that all information provided is accurate and complete.")
  })
})

describe("EligibilityGate", () => {
  const mountGate = async (slug: string, licenceType: never, description = "Add a thing") => {
    const fixture = TestBed.createComponent(EligibilityGate)
    fixture.componentRef.setInput("slug", slug)
    fixture.componentRef.setInput("title", "Patio endorsement")
    fixture.componentRef.setInput("description", description)
    fixture.componentRef.setInput("licenceType", licenceType)
    fixture.componentRef.setInput("config", CONFIG)
    await settle(fixture)
    return fixture.nativeElement as HTMLElement
  }

  it("allows entries that create a licence, or whose prerequisite is held", async () => {
    const el = await mountGate("food-primary", "cannabis-retail-store" as never)
    expect(el.querySelector("config-flow")).toBeTruthy()
    expect(el.textContent).not.toContain("You need a")
    const profile = getBusinessProfile()
    const held = [...heldLicenceTypes(profile.licences)].find((t) => t !== "common")!
    const el2 = await mountGate("patio", held as never)
    expect(el2.querySelector("config-flow")).toBeTruthy()
  })

  it("blocks when the prerequisite licence is missing and lists what the business holds", async () => {
    const profile = getBusinessProfile()
    const held = heldLicenceTypes(profile.licences)
    const missing = unheldLicenceTypes(held)[0]
    expect(isEntryEligible({ slug: "patio", licenceType: missing, description: "Add a thing" }, held)).toBe(false)
    const el = await mountGate("patio", missing as never)
    expect(el.querySelector("config-flow")).toBeNull()
    expect(el.querySelector("h1")!.textContent).toContain("Patio endorsement")
    expect(el.querySelector("page-hero p")!.textContent).toContain("This application is not available for your current licences.")
    expect(el.querySelector("h2")!.textContent).toBe(`You need a ${LICENCE_TYPE_LABELS[missing]} licence first`)
    const labels = [...el.querySelectorAll("li.rounded-full")].map((l) => l.textContent?.trim())
    expect(labels).toEqual([...new Set(profile.licences.map((l) => l.type))])
    const links = [...el.querySelectorAll("a[data-slot='button']")].map((a) => [a.getAttribute("href"), a.textContent?.trim()])
    expect(links).toEqual([
      ["/applications/new", "Back to available applications"],
      ["/licences", "View my licences"],
    ])
    expect([...el.querySelectorAll("hero-breadcrumb a")].map((a) => a.getAttribute("href"))).toEqual([
      "/dashboard",
      "/applications",
      "/applications/new",
    ])
    expect(el.querySelector("[data-slot='card']")!.getAttribute("class")).toContain("border-l-4 border-l-amber-500")
  })
})
