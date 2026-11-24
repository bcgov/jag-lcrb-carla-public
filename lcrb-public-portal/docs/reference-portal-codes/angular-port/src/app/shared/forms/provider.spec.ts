/** ApplicationProvider and layout pieces: required-field gating, hidden fields, furthestStep and completed checks, the draft dialog and autosave. */
import { Component, input, signal, type OnDestroy, type OnInit } from "@angular/core"
import { type ComponentFixture, TestBed } from "@angular/core/testing"
import { By } from "@angular/platform-browser"
import { toast } from "@spartan-ng/brain/sonner"
import { afterEach, beforeEach, describe, expect, it, vi } from "vitest"
import { deleteDraft, getDraft, saveDraft } from "@data-access/repositories/applications/drafts"
import type { FieldValidation } from "@domain/forms/field-validation"
import {
  ApplicationFlowState,
  ApplicationLayout,
  ApplicationNavigation,
  ApplicationProvider,
  injectApplicationContext,
  type ApplicationStep,
} from "./index"

/** Stands in for a framework field: registers a validator while mounted. */
@Component({
  selector: "div[testField]",
  host: { "[id]": "'field-' + name()" },
  template: `<input [id]="name()" [value]="value()" (input)="onInput($event)" />`,
})
class TestField implements OnInit, OnDestroy {
  readonly name = input.required<string>()
  readonly label = input.required<string>()
  readonly rules = input<FieldValidation>({ required: true })
  protected readonly ctx = injectApplicationContext()
  protected onInput(e: Event) {
    this.ctx.updateFormData({ [this.name()]: (e.target as HTMLInputElement).value })
  }
  protected value = () => (this.ctx.formData()[this.name()] as string | undefined) ?? ""
  ngOnInit() {
    this.ctx.registerValidator(this.name(), this.rules(), this.label())
  }
  ngOnDestroy() {
    this.ctx.unregisterValidator(this.name())
  }
}

const STEPS: ApplicationStep[] = [
  { id: "one", title: "Business", description: "About you" },
  { id: "two", title: "Premises" },
  { id: "review", title: "Review & Submit" },
]

@Component({
  imports: [ApplicationProvider, ApplicationLayout, ApplicationNavigation, TestField],
  template: `
    <application-provider
      [steps]="steps()"
      [initialData]="initial()"
      [draftId]="draftId()"
      [draftMeta]="draftId() ? { flowId: 'f', title: 'Flow F', resumeHref: '/applications/new/f' } : undefined"
    >
      <application-layout [title]="'Flow F'" [applicationName]="'Sub'">
        @if (ctx().currentStep === 0) {
          <div testField [name]="'legalName'" [label]="'Legal name'"></div>
          @if (showOptional()) {
            <div testField [name]="'optional'" [label]="'Optional detail'"></div>
          }
        }
        <application-navigation [onSave]="onSave" />
      </application-layout>
    </application-provider>
  `,
})
class Host {
  steps = signal(STEPS)
  initial = signal<Record<string, unknown> | undefined>(undefined)
  draftId = signal<string | undefined>(undefined)
  showOptional = signal(true)
  onSave = vi.fn()
  // Filled by the spec once the provider exists (read in the template).
  ctx = signal({ currentStep: 0 })
}

async function settle(fixture: ComponentFixture<unknown>) {
  fixture.detectChanges()
  await fixture.whenStable()
  fixture.detectChanges()
}

async function mount(setup?: (host: Host) => void) {
  const fixture = TestBed.createComponent(Host)
  setup?.(fixture.componentInstance)
  await settle(fixture)
  const state = fixture.debugElement.query(By.directive(ApplicationProvider)).injector.get(ApplicationFlowState)
  // Mirror the step into the host so the template can swap step content.
  const sync = () => fixture.componentInstance.ctx.set({ currentStep: state.currentStep() })
  sync()
  await settle(fixture)
  const el = fixture.nativeElement as HTMLElement
  const refresh = async () => {
    sync()
    await settle(fixture)
  }
  return { fixture, el, state, refresh }
}

const nextButton = (el: HTMLElement) => [...el.querySelectorAll("button")].find((b) => b.textContent?.trim() === "Next")!
const button = (root: ParentNode, text: string) =>
  [...root.querySelectorAll("button")].find((b) => b.textContent?.trim() === text) as HTMLButtonElement | undefined

beforeEach(() => {
  localStorage.clear()
  sessionStorage.clear()
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

describe("required-field gating", () => {
  it("blocks Next, toasts and lists the failing fields in the summary", async () => {
    const { el, state, refresh } = await mount()
    nextButton(el).click()
    await refresh()
    expect(state.currentStep()).toBe(0)
    expect(toast.error).toHaveBeenCalledWith("Some required fields need attention", {
      description: "See the highlighted list at the top of the step to jump to each one.",
    })
    const summary = el.querySelector('[role="alert"]')!
    expect(summary.getAttribute("aria-live")).toBe("assertive")
    expect(summary.querySelector("h3")!.textContent).toContain("2 fields need your attention before you can continue")
    expect([...summary.querySelectorAll("li button")].map((b) => b.textContent?.trim())).toEqual(["Legal name", "Optional detail"])
    expect(summary.querySelector("li")!.textContent).toContain("— This field is required.")
    expect(state.submitAttempted()).toBe(true)
    expect(state.invalidFields().map((f) => f.name)).toEqual(["legalName", "optional"])
    expect(window.scrollTo).not.toHaveBeenCalled()
  })

  it("uses the singular heading for one blocker", async () => {
    const { el, refresh } = await mount((h) => h.showOptional.set(false))
    nextButton(el).click()
    await refresh()
    expect(el.querySelector('[role="alert"] h3')!.textContent).toContain("1 field needs your attention before you can continue")
  })

  it("scrolls the field into view and focuses its control after 150 ms", async () => {
    const { el, refresh } = await mount()
    nextButton(el).click()
    await refresh()
    const shell = el.querySelector("#field-legalName") as HTMLElement
    const scrollIntoView = vi.fn()
    shell.scrollIntoView = scrollIntoView
    vi.useFakeTimers({ toFake: ["setTimeout", "clearTimeout"] })
    button(el.querySelector('[role="alert"]')!, "Legal name")!.click()
    expect(scrollIntoView).toHaveBeenCalledWith({ behavior: "smooth", block: "center" })
    expect(document.activeElement).not.toBe(shell.querySelector("input"))
    vi.advanceTimersByTime(150)
    expect(document.activeElement).toBe(shell.querySelector("input"))
  })

  it("does not validate fields that are not mounted", async () => {
    const { el, state, fixture, refresh } = await mount((h) => h.showOptional.set(false))
    expect(state.peekBlockers().map((b) => b.name)).toEqual(["legalName"])
    state.updateFormData({ legalName: "Acme" })
    await refresh()
    nextButton(el).click()
    await refresh()
    expect(state.currentStep()).toBe(1)
    expect(toast.error).not.toHaveBeenCalled()
    expect(fixture.componentInstance.showOptional()).toBe(false)
  })

  it("peekBlockers is read-only; blockersOnCurrentStep flags the step", async () => {
    const { state } = await mount()
    expect(state.peekBlockers()).toHaveLength(2)
    expect(state.submitAttempted()).toBe(false)
    expect(state.getFieldError("legalName")).toBeUndefined()
    expect(state.blockersOnCurrentStep()).toHaveLength(2)
    expect(state.submitAttempted()).toBe(true)
    expect(state.hasErrors()).toBe(true)
    expect(state.getFieldError("legalName")).toBe("This field is required.")
  })

  it("a failed next step does not call pulseProgress or scroll; a successful one does both", async () => {
    const { el, state, refresh } = await mount((h) => h.showOptional.set(false))
    const progress = await import("@core/progress/progress-bus")
    const seen: string[] = []
    const off = progress.subscribeProgress((e) => seen.push(e))
    nextButton(el).click()
    await refresh()
    await Promise.resolve()
    expect(seen).toEqual([])
    state.updateFormData({ legalName: "Acme" })
    nextButton(el).click()
    await refresh()
    await Promise.resolve()
    expect(seen.length).toBeGreaterThan(0)
    expect(window.scrollTo).toHaveBeenCalledWith({ top: 0, behavior: "smooth" })
    off()
  })
})

describe("furthestStep and completed check marks", () => {
  it("only steps moved beyond earn a check, and stepping resets the summary", async () => {
    const { el, state, refresh } = await mount((h) => h.showOptional.set(false))
    nextButton(el).click()
    await refresh()
    expect(state.invalidFields()).toHaveLength(1)
    state.updateFormData({ legalName: "Acme" })
    await refresh()
    expect(state.completedSteps()).toEqual([])
    nextButton(el).click()
    await refresh()
    await refresh()
    expect(state.currentStep()).toBe(1)
    expect(state.furthestStep()).toBe(1)
    expect(state.invalidFields()).toEqual([])
    expect(state.submitAttempted()).toBe(false)
    expect(state.completedSteps()).toEqual([0])
    const circles = el.querySelectorAll("step-indicator button > div:first-child")
    expect(circles[0].querySelector("svg.lucide-check")).toBeTruthy()
    expect(circles[0].getAttribute("class")).toContain("bg-[#2E8540] border-[#2E8540] text-white")
    expect(circles[1].getAttribute("class")).toContain("bg-[#013366] border-[#013366] text-white")
    expect(circles[2].getAttribute("class")).toContain("bg-white border-gray-300 text-gray-500")
    expect(circles[1].textContent?.trim()).toBe("2")
    expect(el.querySelectorAll("step-indicator button")[1].getAttribute("aria-current")).toBe("step")
    expect((el.querySelectorAll("step-indicator button")[1] as HTMLButtonElement).disabled).toBe(true)
  })

  it("going back keeps furthestStep; attemptGoToStep forward is gated, backward is free", async () => {
    const { el, state, refresh } = await mount((h) => h.showOptional.set(false))
    state.attemptGoToStep(2)
    expect(state.currentStep()).toBe(0)
    expect(toast.error).toHaveBeenCalledTimes(1)
    state.updateFormData({ legalName: "Acme" })
    state.attemptGoToStep(2)
    await refresh()
    expect(state.currentStep()).toBe(2)
    expect(state.furthestStep()).toBe(2)
    expect(el.querySelector("step-indicator")).toBeTruthy()
    state.attemptGoToStep(0)
    await refresh()
    expect(state.currentStep()).toBe(0)
    expect(state.furthestStep()).toBe(2)
    expect(state.completedSteps()).toEqual([0, 1])
    state.goToStep(9)
    expect(state.currentStep()).toBe(0)
  })

  it("steps with a validate fn gate Next and the check mark", async () => {
    const steps: ApplicationStep[] = [
      { id: "a", title: "A", validate: (d) => Boolean(d["x"]) },
      { id: "b", title: "B" },
    ]
    const { state, refresh } = await mount((h) => {
      h.steps.set(steps)
      h.showOptional.set(false)
    })
    state.updateFormData({ legalName: "ok" })
    state.nextStep()
    expect(state.currentStep()).toBe(0)
    expect(toast.error).toHaveBeenLastCalledWith("Please complete all required fields on this step before continuing.")
    state.updateFormData({ x: "1" })
    state.nextStep()
    await refresh()
    await refresh()
    expect(state.completedSteps()).toEqual([0])
    state.updateFormData({ x: "" })
    expect(state.completedSteps()).toEqual([])
  })
})

describe("navigation", () => {
  it("shows Back after the first step and Submit on the last; Save needs a draft or onSave", async () => {
    const { el, state, refresh } = await mount((h) => h.showOptional.set(false))
    expect(button(el, "Back")).toBeUndefined()
    expect(button(el, "Save & Continue Later")).toBeTruthy() // onSave is bound
    state.updateFormData({ legalName: "Acme" })
    state.goToStep(2)
    await refresh()
    expect(button(el, "Back")).toBeTruthy()
    expect(button(el, "Next")).toBeUndefined()
    const submit = button(el, "Submit Application")!
    expect(submit.getAttribute("class")).toContain("bg-[#013366] hover:bg-[#38598A]")
  })

  it("Save & Continue Later saves the draft with a toast and calls onSave", async () => {
    const { fixture, el } = await mount((h) => h.draftId.set("draft:f"))
    button(el, "Save & Continue Later")!.click()
    expect(getDraft("draft:f")?.title).toBe("Flow F")
    expect(toast.success).toHaveBeenCalledWith("Progress saved", {
      description: "You can resume this application from your dashboard.",
    })
    expect(fixture.componentInstance.onSave).toHaveBeenCalledTimes(1)
  })

  it("submit validates first, then awaits onSubmit with a pending label", async () => {
    let release!: () => void
    const onSubmit = vi.fn(() => new Promise<void>((r) => (release = r)))
    @Component({
      imports: [ApplicationProvider, ApplicationNavigation],
      template: `<application-provider [steps]="steps"><application-navigation [onSubmit]="onSubmit" /></application-provider>`,
    })
    class Mini {
      steps = [{ id: "only", title: "Only" }]
      onSubmit = onSubmit
    }
    const fixture = TestBed.createComponent(Mini)
    await settle(fixture)
    const el = fixture.nativeElement as HTMLElement
    button(el, "Submit Application")!.click()
    await settle(fixture)
    expect(onSubmit).toHaveBeenCalledTimes(1)
    expect(button(el, "Submitting & assessing…")!.disabled).toBe(true)
    expect(el.querySelector("svg.animate-spin")).toBeTruthy()
    release()
    await settle(fixture)
    expect(button(el, "Submit Application")!.disabled).toBe(false)
  })
})

describe("draft resume dialog", () => {
  const draft = (extra: Record<string, unknown> = {}) => ({
    draftId: "draft:f",
    flowId: "f",
    title: "Flow F",
    resumeHref: "/applications/new/f",
    formData: { legalName: "Saved Ltd", fromDraft: "yes" },
    currentStep: 1,
    progress: 67,
    updatedAt: new Date().toISOString(),
    ...extra,
  })
  const dialog = () => document.querySelector("ui-alert-dialog-content") as HTMLElement | null

  it("does not ask without a draft, or for an untouched first-step draft", async () => {
    await mount((h) => h.draftId.set("draft:f"))
    expect(dialog()).toBeNull()
    saveDraft(draft({ currentStep: 0, formData: {} }))
    await mount((h) => h.draftId.set("draft:f"))
    expect(dialog()).toBeNull()
  })

  it("asks once on mount, then Resume merges the draft over initialData and clamps the step", async () => {
    saveDraft(draft({ currentStep: 9 }))
    const { fixture, state } = await mount((h) => {
      h.draftId.set("draft:f")
      h.initial.set({ licenceNumber: "L-1", legalName: "Prefill" })
    })
    await new Promise((r) => setTimeout(r, 0))
    fixture.detectChanges()
    const d = dialog()!
    expect(d).not.toBeNull()
    expect(d.closest('[role="alertdialog"]')).toBeTruthy()
    expect(d.querySelector("h2")!.textContent).toBe("Continue where you left off?")
    expect(d.querySelector("p")!.textContent).toBe(
      "You have a saved draft of this application from today, paused at step 10 of 3. Resume it, or start a fresh application (this will delete the draft).",
    )
    expect(state.currentStep()).toBe(0)
    button(document.body, "Resume draft")!.click()
    fixture.detectChanges()
    expect(state.formData()).toEqual({ licenceNumber: "L-1", legalName: "Saved Ltd", fromDraft: "yes" })
    expect(state.currentStep()).toBe(2)
    expect(state.furthestStep()).toBe(2)
    expect(state.pendingDraft()).toBeNull()
    expect(toast.info).toHaveBeenCalledWith("Draft restored", { description: "We picked up where you left off." })
  })

  it("Start fresh deletes the draft and keeps the empty form", async () => {
    // saveDraft stamps updatedAt itself, so write an older draft straight to storage.
    localStorage.setItem(
      "lcrb.app.drafts",
      JSON.stringify({ "draft:f": draft({ updatedAt: new Date(Date.now() - 3 * 86_400_000).toISOString() }) }),
    )
    const { fixture, state } = await mount((h) => h.draftId.set("draft:f"))
    await new Promise((r) => setTimeout(r, 0))
    fixture.detectChanges()
    expect(dialog()!.querySelector("p")!.textContent).toContain("from 3 days ago, paused at step 2 of 3.")
    button(document.body, "Start fresh")!.click()
    fixture.detectChanges()
    expect(getDraft("draft:f")).toBeUndefined()
    expect(state.pendingDraft()).toBeNull()
    expect(state.formData()).toEqual({})
    expect(state.currentStep()).toBe(0)
    deleteDraft("draft:f")
  })
})

describe("autosave", () => {
  it("saves 600 ms after a change (silently) and every 30 s, only once hydrated", async () => {
    const { fixture, state } = await mount((h) => h.draftId.set("draft:f"))
    vi.useFakeTimers({ toFake: ["setTimeout", "clearTimeout", "setInterval", "clearInterval", "Date"] })
    deleteDraft("draft:f")
    state.updateFormData({ legalName: "Typing" })
    fixture.detectChanges()
    vi.advanceTimersByTime(599)
    expect(getDraft("draft:f")).toBeUndefined()
    vi.advanceTimersByTime(1)
    const saved = getDraft("draft:f")!
    expect(saved.formData).toEqual({ legalName: "Typing" })
    expect(saved.progress).toBe(33)
    expect(saved.resumeHref).toBe("/applications/new/f")
    expect(toast.success).not.toHaveBeenCalled()

    // A second change inside the window restarts the debounce.
    state.updateFormData({ a: 1 })
    fixture.detectChanges()
    vi.advanceTimersByTime(400)
    state.updateFormData({ a: 2 })
    fixture.detectChanges()
    vi.advanceTimersByTime(400)
    expect(getDraft("draft:f")!.formData).toEqual({ legalName: "Typing" })
    vi.advanceTimersByTime(200)
    expect(getDraft("draft:f")!.formData).toEqual({ legalName: "Typing", a: 2 })

    // The interval saves on its own cadence. Changing the step re-creates the
    // interval (as the prototype's effect does) so it runs on the fake clock.
    state.goToStep(1)
    fixture.detectChanges()
    vi.advanceTimersByTime(600)
    deleteDraft("draft:f")
    vi.advanceTimersByTime(30_000)
    expect(getDraft("draft:f")).toBeTruthy()
  })

  it("does nothing without a draft id", async () => {
    const { fixture, state } = await mount()
    vi.useFakeTimers({ toFake: ["setTimeout", "clearTimeout", "setInterval", "clearInterval", "Date"] })
    state.updateFormData({ legalName: "x" })
    fixture.detectChanges()
    vi.advanceTimersByTime(60_000)
    expect(localStorage.getItem("lcrb.app.drafts")).toBeNull()
  })
})

describe("injectApplicationContext", () => {
  it("throws outside an ApplicationProvider", () => {
    @Component({ template: "" })
    class Lonely {
      ctx = injectApplicationContext()
    }
    expect(() => TestBed.createComponent(Lonely)).toThrow("useApplicationContext must be used within ApplicationProvider")
  })
})
