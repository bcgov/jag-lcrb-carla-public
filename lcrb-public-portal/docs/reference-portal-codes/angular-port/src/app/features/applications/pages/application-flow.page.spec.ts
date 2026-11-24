import { TestBed } from "@angular/core/testing"
import { By } from "@angular/platform-browser"
import { provideRouter } from "@angular/router"
import { afterEach, beforeEach, describe, expect, it, vi } from "vitest"
import { ApplicationFlowState, ApplicationProvider } from "@shared/forms"
import { ApplicationFlowPage } from "./application-flow.page"

async function mount(inputs: Record<string, unknown>) {
  const fixture = TestBed.createComponent(ApplicationFlowPage)
  for (const [k, v] of Object.entries(inputs)) fixture.componentRef.setInput(k, v)
  for (let i = 0; i < 3; i++) {
    fixture.detectChanges()
    await fixture.whenStable()
  }
  fixture.detectChanges()
  return { fixture, el: fixture.nativeElement as HTMLElement }
}

describe("ApplicationFlowPage", () => {
  beforeEach(() => {
    localStorage.clear()
    sessionStorage.clear()
    TestBed.configureTestingModule({ providers: [provideRouter([])] })
    vi.spyOn(window, "scrollTo").mockImplementation(() => undefined)
  })
  afterEach(() => {
    vi.unstubAllGlobals()
    vi.restoreAllMocks()
  })

  it("renders the eligibility gate wrapping the flow for a known slug", async () => {
    const { el } = await mount({ slug: "food-primary" })
    expect(el.querySelector("eligibility-gate config-flow")).toBeTruthy()
    expect(el.querySelector("h1")!.textContent).toContain("Food Primary Licence Application")
  })

  it("prefills the form from the query params, ignoring blank and unknown values", async () => {
    const { fixture } = await mount({
      slug: "food-primary",
      licenceNumber: "L-123",
      establishmentName: "Cafe One",
      legalEntityName: "   ",
      legalName: ["a", "b"],
    })
    const state = fixture.debugElement.query(By.directive(ApplicationProvider)).injector.get(ApplicationFlowState)
    expect(state.formData()).toEqual({ licenceNumber: "L-123", establishmentName: "Cafe One" })
  })

  it("passes no prefill when no query param is usable", async () => {
    const { fixture } = await mount({ slug: "food-primary" })
    const state = fixture.debugElement.query(By.directive(ApplicationProvider)).injector.get(ApplicationFlowState)
    expect(state.formData()).toEqual({})
  })

  it("shows the eligibility block for lrs-relocation", async () => {
    const { el } = await mount({ slug: "lrs-relocation" })
    expect(el.querySelector("config-flow")).toBeNull()
    expect(el.querySelector("h2")!.textContent).toContain("You need a")
    expect(el.textContent).toContain("Back to available applications")
  })

  it("renders nothing for an unknown slug (null handling kept)", async () => {
    const { el } = await mount({ slug: "no-such-slug" })
    expect(el.querySelector("eligibility-gate")).toBeNull()
  })
})
