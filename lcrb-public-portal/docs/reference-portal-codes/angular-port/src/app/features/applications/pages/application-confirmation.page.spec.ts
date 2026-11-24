import { TestBed } from "@angular/core/testing"
import { provideRouter } from "@angular/router"
import { beforeEach, describe, expect, it } from "vitest"
import { ApplicationConfirmationPage } from "./application-confirmation.page"

describe("ApplicationConfirmationPage", () => {
  beforeEach(() => {
    localStorage.clear()
    TestBed.configureTestingModule({ providers: [provideRouter([])] })
  })

  it("wraps the receipt for the id in the page chrome", async () => {
    localStorage.setItem(
      "lcrb.app.submissions",
      JSON.stringify({
        c1: {
          id: "c1",
          confirmationNumber: "PAY-2026-X",
          jobNumber: "LP-1",
          flowId: "f",
          title: "T",
          applicant: "A",
          lineItems: [],
          total: 0,
          currency: "CAD",
          paymentMethod: "none",
          submittedAt: "2026-11-02T17:00:00.000Z",
          summary: [],
          syncStatus: "synced",
          assessment: null,
        },
      }),
    )
    const fixture = TestBed.createComponent(ApplicationConfirmationPage)
    fixture.componentRef.setInput("id", "c1")
    for (let i = 0; i < 3; i++) {
      fixture.detectChanges()
      await fixture.whenStable()
    }
    const el = fixture.nativeElement as HTMLElement
    expect(el.querySelector("bc-gov-header")).toBeTruthy()
    expect(el.querySelector("bc-gov-footer")).toBeTruthy()
    const main = el.querySelector("main#main-content")!
    expect(main.className).toBe("flex-1 bg-muted/20")
    expect(main.querySelector("h1")!.textContent!.trim()).toBe("Application submitted")
    expect(main.textContent).toContain("Confirmation number")
    expect(main.textContent).toContain("PAY-2026-X")
  })
})
