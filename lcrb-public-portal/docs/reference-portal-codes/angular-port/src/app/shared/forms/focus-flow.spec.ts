/** Guided Mode: numbering, progress, validation gating, conditional questions, review and submit, focus, body flags, full-screen and interpret-answer; fields are driven through the provider state. */
import { Component, signal } from "@angular/core"
import { type ComponentFixture, TestBed } from "@angular/core/testing"
import { By } from "@angular/platform-browser"
import { provideRouter, Router } from "@angular/router"
import { toast } from "@spartan-ng/brain/sonner"
import { afterEach, beforeEach, describe, expect, it, vi } from "vitest"
import { getDraft } from "@data-access/repositories/applications/drafts"
import { stubFetch } from "../../../../tests/support/fetch-stub"
import { ConfigFlow } from "./config-flow"
import { type FlowConfig } from "@domain/forms/flow-config"
import { FocusFlow } from "./focus-flow"
import { ApplicationFlowState, ApplicationProvider, type ApplicationStep } from "./index"

@Component({ template: "" })
class Blank {}

const CONFIG: FlowConfig = {
  id: "guided-test",
  title: "Guided Test",
  returnTo: "/applications",
  steps: [
    {
      id: "business",
      title: "Business",
      sections: [
        {
          title: "About the business",
          fields: [
            { name: "tradeName", label: "Trade name", type: "text", required: true, help: "As registered." },
            {
              name: "hasBar",
              label: "Do you have a bar?",
              type: "radio",
              options: [
                { value: "yes", label: "Yes" },
                { value: "no", label: "No" },
              ],
            },
            {
              name: "barName",
              label: "Bar name",
              type: "text",
              required: true,
              visibleWhenEquals: { field: "hasBar", value: "yes" },
            },
          ],
        },
      ],
    },
    {
      id: "details",
      title: "Details",
      sections: [
        {
          title: "More",
          fields: [{ name: "notes", label: "Notes", type: "textarea" }],
        },
      ],
    },
  ],
}

const STEPS: ApplicationStep[] = [
  ...CONFIG.steps.map((s) => ({ id: s.id, title: s.title })),
  { id: "review", title: "Review & Submit" },
]

@Component({
  imports: [ApplicationProvider, FocusFlow],
  template: `
    <ng-template #rs><p id="review-slot">REVIEW SLOT</p></ng-template>
    <application-provider [steps]="steps" draftId="draft:guided-test" [draftMeta]="meta">
      <focus-flow
        [config]="config"
        [submitting]="submitting()"
        [reviewSlot]="rs"
        (exitMode)="exits = exits + 1"
        (submitFlow)="submits = submits + 1"
      />
    </application-provider>
  `,
})
class Host {
  config = CONFIG
  steps = STEPS
  meta = { flowId: "guided-test", title: "Guided Test", resumeHref: "/applications/new/guided-test" }
  submitting = signal(false)
  exits = 0
  submits = 0
}

async function settle(fixture: ComponentFixture<unknown>) {
  for (let i = 0; i < 3; i++) {
    fixture.detectChanges()
    await fixture.whenStable()
  }
  fixture.detectChanges()
}

async function mount() {
  const fixture = TestBed.createComponent(Host)
  await settle(fixture)
  const state = fixture.debugElement.query(By.directive(ApplicationProvider)).injector.get(ApplicationFlowState)
  const el = fixture.nativeElement as HTMLElement
  const s = () => settle(fixture)
  const text = (sel: string) => el.querySelector(sel)?.textContent?.replace(/\s+/g, " ").trim()
  const button = (re: RegExp) =>
    [...el.querySelectorAll("button")].find((b) => re.test(b.textContent?.trim() ?? "")) as HTMLButtonElement | undefined
  const click = async (re: RegExp) => {
    const b = button(re)
    if (!b) throw new Error(`no button ${re}`)
    b.click()
    await s()
  }
  return { fixture, host: fixture.componentInstance, state, el, settle: s, text, button, click }
}

beforeEach(() => {
  localStorage.clear()
  sessionStorage.clear()
  TestBed.configureTestingModule({ providers: [provideRouter([{ path: "**", component: Blank }])] })
  vi.spyOn(window, "scrollTo").mockImplementation(() => undefined)
  vi.spyOn(toast, "error").mockImplementation(() => 1)
  vi.spyOn(toast, "success").mockImplementation(() => 1)
})
afterEach(() => {
  vi.useRealTimers()
  vi.unstubAllGlobals()
  vi.restoreAllMocks()
  document.body.style.overflow = ""
})

describe("FocusFlow questions", () => {
  it("shows Question N of M, the heading with (required), help text and progress", async () => {
    const { el, text, state, settle: s } = await mount()
    expect(text("h1")).toBe("Trade name(required)")
    expect(el.querySelector("h1")!.getAttribute("tabindex")).toBe("-1")
    expect(el.textContent).toContain("Guided Test")
    expect(el.textContent).toContain("Business · Question 1 of 3")
    expect(text("h1 + p")).toBe("As registered.")
    expect(el.querySelector("p.uppercase")!.textContent).toBe("About the business")
    expect(el.querySelector("div[configField]")).toBeTruthy()
    const bar = () => el.querySelector('[role="progressbar"]')!.getAttribute("aria-valuenow")
    expect(bar()).toBe("0")
    state.updateFormData({ tradeName: "Acme" })
    await s()
    expect(bar()).toBe(String((1 / 3) * 100))
  })

  it("blocks Continue on a required empty answer and shows the error", async () => {
    const { el, text, click, state } = await mount()
    await click(/^Continue$/)
    expect(text('p[role="alert"]')).toBe("This field is required.")
    expect(el.textContent).toContain("Question 1 of 3")
    expect(state.currentStep()).toBe(0)
  })

  it("Continue advances, focuses the new heading and clears the error", async () => {
    const { el, text, click, state, settle: s } = await mount()
    await click(/^Continue$/)
    state.updateFormData({ tradeName: "Acme" })
    await s()
    await click(/^Continue$/)
    expect(el.textContent).toContain("Question 2 of 3")
    expect(text("h1")).toBe("Do you have a bar?")
    expect(el.querySelector('p[role="alert"]')).toBeNull()
    expect(document.activeElement).toBe(el.querySelector("h1"))
    expect(el.querySelector("div[configField]")).toBeTruthy()
  })

  it("Back is disabled on the first question and returns on later ones", async () => {
    const { el, click, state, settle: s, button } = await mount()
    expect(button(/^Back$/)!.disabled).toBe(true)
    state.updateFormData({ tradeName: "Acme" })
    await s()
    await click(/^Continue$/)
    expect(button(/^Back$/)!.disabled).toBe(false)
    await click(/^Back$/)
    expect(el.textContent).toContain("Question 1 of 3")
  })

  it("adds and removes conditional questions and keeps the cursor on its field", async () => {
    const { el, text, click, state, settle: s } = await mount()
    state.updateFormData({ tradeName: "Acme", hasBar: "yes" })
    await s()
    await click(/^Continue$/)
    expect(el.textContent).toContain("Question 2 of 4")
    state.updateFormData({ hasBar: "no" })
    await s()
    expect(text("h1")).toBe("Do you have a bar?")
    expect(el.textContent).toContain("Question 2 of 3")
  })

  it("falls back to the current step's first question when the anchored field is hidden", async () => {
    const { el, text, click, state, settle: s } = await mount()
    state.updateFormData({ tradeName: "Acme", hasBar: "yes" })
    await s()
    await click(/^Continue$/)
    state.updateFormData({ barName: "Tap" })
    await s()
    await click(/^Continue$/)
    expect(text("h1")).toBe("Bar name(required)")
    state.updateFormData({ hasBar: "no" })
    await s()
    expect(text("h1")).toBe("Trade name(required)")
    expect(el.textContent).toContain("Question 1 of 3")
  })

  it("syncs the provider step only to the question's step, and re-checks the step it leaves", async () => {
    const { el, click, state, settle: s } = await mount()
    state.updateFormData({ tradeName: "Acme", hasBar: "yes" })
    await s()
    await click(/^Continue$/)
    await click(/^Continue$/)
    // On "Bar name" (required, empty): still step 0.
    expect(state.currentStep()).toBe(0)
    state.updateFormData({ barName: "Tap" })
    await s()
    await click(/^Continue$/)
    expect(el.textContent).toContain("Details")
    expect(state.currentStep()).toBe(1)
  })

  it("blocks a step boundary when an earlier answer was cleared, and lists it", async () => {
    const { el, click, state, settle: s } = await mount()
    state.updateFormData({ tradeName: "Acme", hasBar: "no" })
    await s()
    await click(/^Continue$/)
    state.updateFormData({ tradeName: "" })
    await s()
    await click(/^Continue$/)
    expect(toast.error).toHaveBeenCalledWith("Some earlier answers need attention", {
      description: "We've listed what's missing on this step below.",
    })
    const alert = el.querySelector('div[role="alert"]')!
    expect(alert.textContent).toContain("Before moving on, these answers on this step need attention:")
    const link = alert.querySelector("button")!
    expect(link.textContent!.trim()).toBe("Trade name — This field is required.")
    link.click()
    await s()
    expect(el.querySelector("h1")!.textContent).toContain("Trade name")
  })

  it("the step jump lists steps with counts and blocks a forward jump", async () => {
    const { el, click, settle: s } = await mount()
    await click(/^All steps$/)
    const items = [...document.querySelectorAll("li button")].map((b) => b.textContent!.replace(/\s+/g, " ").trim())
    expect(items).toEqual(["1. Business2", "2. Details1"])
    ;(document.querySelectorAll("li button")[1] as HTMLButtonElement).click()
    await s()
    expect(toast.error).toHaveBeenCalledWith("Finish this step first", {
      description: "We've listed what's missing below.",
    })
    expect(el.textContent).toContain("Question 1 of 3")
    expect(el.querySelector('div[role="alert"]')!.textContent).toContain("Trade name — This field is required.")
  })
})

describe("FocusFlow review", () => {
  async function toReview(m: Awaited<ReturnType<typeof mount>>) {
    m.state.updateFormData({ tradeName: "Acme", hasBar: "no" })
    await m.settle()
    await m.click(/^Continue$/)
    await m.click(/^Continue$/)
    expect(m.button(/^Review$/)).toBeTruthy()
    await m.click(/^Review$/)
  }

  it("shows the last question's button as Review, then the review screen with the slot", async () => {
    const m = await mount()
    await toReview(m)
    expect(m.text("h1")).toBe("Review your application")
    expect(m.text("#review-slot")).toBe("REVIEW SLOT")
    expect(m.el.textContent).toContain("Final step · Review & submit")
    expect(m.el.querySelector('[role="progressbar"]')!.getAttribute("aria-valuenow")).toBe("100")
    expect(m.button(/^Submit application$/)).toBeTruthy()
    expect(document.activeElement).toBe(m.el.querySelector("h1"))
  })

  it("Back from review returns to the last question", async () => {
    const m = await mount()
    await toReview(m)
    await m.click(/^Back$/)
    expect(m.text("h1")).toBe("Notes")
    expect(m.el.textContent).toContain("Question 3 of 3")
  })

  it("submit emits submitFlow when nothing blocks", async () => {
    const m = await mount()
    await toReview(m)
    await m.click(/^Submit application$/)
    expect(m.host.submits).toBe(1)
  })

  it("submit is gated by the registered validators (declarations) and lists blockers", async () => {
    const m = await mount()
    await toReview(m)
    m.state.registerValidator("__confirmAccuracy", { required: true }, "Accuracy declaration")
    await m.click(/^Submit application$/)
    expect(m.host.submits).toBe(0)
    const alert = m.el.querySelector('div[role="alert"]')!
    expect(alert.textContent).toContain("Before submitting, these need attention:")
    expect(alert.querySelector("li")!.textContent).toBe("Accuracy declaration — This field is required.")
    m.state.unregisterValidator("__confirmAccuracy")
    await m.click(/^Submit application$/)
    expect(m.host.submits).toBe(1)
  })

  it("shows Submitting… and disables the button while submitting", async () => {
    const m = await mount()
    await toReview(m)
    m.host.submitting.set(true)
    await m.settle()
    const submit = [...m.el.querySelectorAll("button")].find((b) => b.textContent!.includes("Submitting…"))!
    expect(submit.disabled).toBe(true)
    expect(m.button(/^Submit application$/)).toBeUndefined()
  })

  it("no visible questions shows Nothing left to answer, and Go to review works", async () => {
    const fixture = TestBed.createComponent(Host)
    fixture.componentInstance.config = {
      ...CONFIG,
      steps: [
        {
          id: "a",
          title: "A",
          sections: [
            {
              title: "S",
              fields: [{ name: "x", label: "X", type: "text", visibleWhenEquals: { field: "y", value: "1" } }],
            },
          ],
        },
      ],
    }
    fixture.componentInstance.steps = [{ id: "a", title: "A" }, { id: "review", title: "Review" }]
    await settle(fixture)
    const el = fixture.nativeElement as HTMLElement
    expect(el.querySelector("h1")!.textContent).toBe("Nothing left to answer")
    expect(el.textContent).toContain("Guided Test · Ready to review")
    ;[...el.querySelectorAll("button")].find((b) => b.textContent!.includes("Go to review"))!.click()
    await settle(fixture)
    expect(el.querySelector("h1")!.textContent!.trim()).toBe("Review your application")
  })
})

describe("FocusFlow chrome, body flags and navigation", () => {
  it("sets and clears body.dataset.guidedMode", async () => {
    const m = await mount()
    expect(document.body.dataset["guidedMode"]).toBe("true")
    m.fixture.destroy()
    expect(document.body.dataset["guidedMode"]).toBeUndefined()
  })

  it("full screen overlays, locks body scroll, keeps the question, and Escape exits", async () => {
    const m = await mount()
    document.body.style.overflow = "scroll"
    await m.click(/Full screen/)
    const region = m.el.querySelector('[role="region"]')!
    expect(region.getAttribute("aria-label")).toBe("Guided Test — full screen")
    expect(region.className).toBe("fixed inset-0 z-[60] flex flex-col overflow-y-auto bg-background")
    expect(document.body.style.overflow).toBe("hidden")
    expect(m.text('[role="region"] p.truncate')).toBe("Question 1 of 3")
    expect(m.el.querySelector("h1")).toBeTruthy()
    window.dispatchEvent(new KeyboardEvent("keydown", { key: "Escape" }))
    await m.settle()
    expect(m.el.querySelector('[role="region"]')).toBeNull()
    expect(document.body.style.overflow).toBe("scroll")
  })

  it("Exit full screen restores the body overflow", async () => {
    const m = await mount()
    await m.click(/Full screen/)
    await m.click(/Exit full screen/)
    expect(m.el.querySelector('[role="region"]')).toBeNull()
    expect(document.body.style.overflow).not.toBe("hidden")
  })

  it("Full form emits exitMode", async () => {
    const m = await mount()
    await m.click(/Full form/)
    expect(m.host.exits).toBe(1)
  })

  it("Save & exit saves the draft and returns to returnTo", async () => {
    const m = await mount()
    m.state.updateFormData({ tradeName: "Acme" })
    await m.settle()
    await m.click(/Save & exit/)
    expect(getDraft("draft:guided-test")?.formData).toEqual({ tradeName: "Acme" })
    await vi.waitFor(() => expect(TestBed.inject(Router).url).toBe("/applications"))
  })
})

describe("FocusFlow free text (interpret-answer)", () => {
  async function onNotes() {
    const m = await mount()
    m.state.updateFormData({ tradeName: "Acme", hasBar: "no" })
    await m.settle()
    await m.click(/^Continue$/)
    await m.click(/^Continue$/)
    expect(m.text("h1")).toBe("Notes")
    return m
  }
  /** Click "Fill in for me" and wait for the request to settle. */
  const fill = async (m: Awaited<ReturnType<typeof mount>>) => {
    await m.click(/^Fill in for me$/)
    await vi.waitFor(async () => {
      await m.settle()
      expect(m.el.textContent).not.toContain("Reading…")
    })
  }
  const type = async (m: Awaited<ReturnType<typeof mount>>, value: string) => {
    const ta = m.el.querySelector("#guided-prose") as HTMLTextAreaElement
    ta.value = value
    ta.dispatchEvent(new Event("input"))
    await m.settle()
  }

  it("offers the prose box only for question types that benefit", async () => {
    const m = await mount()
    expect(m.el.querySelector("#guided-prose")).toBeNull()
    m.state.updateFormData({ tradeName: "Acme" })
    await m.settle()
    await m.click(/^Continue$/)
    expect(m.el.querySelector("#guided-prose")).toBeNull() // two-way radio
    await m.click(/^Continue$/)
    expect(m.el.querySelector("#guided-prose")).toBeTruthy()
    expect(m.el.querySelector("label[for=guided-prose]")!.textContent!.trim()).toBe("Prefer to describe it in your own words?")
  })

  it("writes a confident reading and shows the AI note", async () => {
    const m = await onNotes()
    const api = stubFetch({
      "/api/interpret-answer": { json: { value: "Open late", confidence: 0.9, note: "" } },
    })
    expect(m.button(/^Fill in for me$/)!.disabled).toBe(true)
    await type(m, "we open late")
    await fill(m)
    expect(api.callsTo("/api/interpret-answer")[0].body).toEqual({
      field: { label: "Notes", type: "textarea" },
      text: "we open late",
    })
    expect(m.state.formData()["notes"]).toBe("Open late")
    expect(m.el.textContent).toContain("Filled in from what you wrote — check it's right.")
    expect((m.el.querySelector("#guided-prose") as HTMLTextAreaElement).value).toBe("")
  })

  it("never writes a reading below 0.5 confidence and shows the note instead", async () => {
    const m = await onNotes()
    stubFetch({ "/api/interpret-answer": { json: { value: "Maybe", confidence: 0.3 } } })
    await type(m, "hmm")
    await fill(m)
    expect(m.state.formData()["notes"]).toBeUndefined()
    expect(m.text('div.bg-muted\\/30 p[role="alert"]')).toBe(
      "I couldn't tell from that. Try rephrasing, or answer using the field above.",
    )
    await type(m, "hmm2")
    expect(m.el.querySelector('div.bg-muted\\/30 p[role="alert"]')).toBeNull()
  })

  it("reports a server error and a network failure", async () => {
    const m = await onNotes()
    stubFetch({ "/api/interpret-answer": { status: 500, json: { error: "Nope" } } })
    await type(m, "x")
    await fill(m)
    expect(m.text('div.bg-muted\\/30 p[role="alert"]')).toBe("Nope")
    vi.stubGlobal("fetch", vi.fn().mockRejectedValue(new Error("down")))
    await fill(m)
    expect(m.text('div.bg-muted\\/30 p[role="alert"]')).toBe("Something went wrong reading that. Please answer directly.")
  })
})

describe("FocusFlow inside ConfigFlow", () => {
  it("Full form switches the stored mode back to the classic form", async () => {
    localStorage.setItem("lcrb.app.mode", "guided")
    const fixture = TestBed.createComponent(ConfigFlow)
    fixture.componentRef.setInput("config", CONFIG)
    await settle(fixture)
    const el = fixture.nativeElement as HTMLElement
    expect(el.querySelector("focus-flow h1")!.textContent).toContain("Trade name")
    ;[...el.querySelectorAll("button")].find((b) => b.textContent!.includes("Full form"))!.click()
    await settle(fixture)
    expect(localStorage.getItem("lcrb.app.mode")).toBe("form")
    expect(el.querySelector("focus-flow")).toBeNull()
    expect(el.querySelector("application-layout")).toBeTruthy()
  })
})
