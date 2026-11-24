import { TestBed } from "@angular/core/testing"
import { provideRouter } from "@angular/router"
import { afterEach, beforeEach, describe, expect, it, vi } from "vitest"
import { listReceipts } from "@data-access/repositories/payments/receipt-model"
import type { SubmissionRecord } from "@data-access/repositories/applications/submissions"
import { PaymentReceipt } from "./payment-receipt"

const FREE: SubmissionRecord = {
  id: "fixture-confirmation",
  confirmationNumber: "PAY-2026-FIX001",
  jobNumber: "FP-126001",
  flowId: "fp-name-change",
  title: "Liquor Establishment Name Change Application",
  applicant: "Restaurant ABC",
  lineItems: [],
  total: 0,
  currency: "CAD",
  paymentMethod: "No payment required",
  submittedAt: "2026-11-02T17:00:00.000Z",
  summary: [{ label: "Proposed establishment name", value: "Restaurant XYZ" }],
  syncStatus: "synced",
  assessment: null,
}

function seed(record: SubmissionRecord) {
  localStorage.setItem("lcrb.app.submissions", JSON.stringify({ [record.id]: record }))
}

async function mount(id: string) {
  const fixture = TestBed.createComponent(PaymentReceipt)
  fixture.componentRef.setInput("id", id)
  for (let i = 0; i < 3; i++) {
    fixture.detectChanges()
    await fixture.whenStable()
  }
  fixture.detectChanges()
  return { fixture, el: fixture.nativeElement as HTMLElement }
}

const norm = (s: string | null | undefined) => (s ?? "").replace(/\s+/g, " ").trim()

describe("PaymentReceipt", () => {
  beforeEach(() => {
    localStorage.clear()
    sessionStorage.clear()
    TestBed.configureTestingModule({ providers: [provideRouter([])] })
  })
  afterEach(() => {
    vi.unstubAllGlobals()
    vi.restoreAllMocks()
  })

  it("shows Confirmation not found for an unknown id", async () => {
    const { el } = await mount("nope")
    expect(el.querySelector("h1")!.textContent).toBe("Confirmation not found")
    expect(norm(el.querySelector("p")!.textContent)).toBe(
      "We couldn't find this confirmation. It may have been submitted on another device.",
    )
    const link = el.querySelector("a")!
    expect(link.getAttribute("href")).toBe("/applications")
    expect(link.className).toContain("mt-6 bg-[#013366] hover:bg-[#38598A]")
    expect(link.textContent!.trim()).toBe("Go to My Applications")
  })

  it("renders a free submission: Application submitted, references, summary, no payment block", async () => {
    seed(FREE)
    const { el } = await mount(FREE.id)
    expect(el.querySelector("h1")!.textContent!.trim()).toBe("Application submitted")
    expect(norm(el.querySelector("h1 + p")!.textContent)).toBe(
      "Your application has been submitted to the Liquor and Cannabis Regulation Branch. A confirmation has been sent to your email.",
    )
    expect(el.textContent).toContain("Confirmation number")
    expect(el.textContent).toContain("PAY-2026-FIX001")
    expect(el.textContent).toContain("FP-126001")
    expect(el.textContent).toContain("Restaurant ABC")
    expect(el.textContent).toContain("Proposed establishment name")
    expect(el.textContent).not.toContain("Payment details")
    expect(el.querySelector("receipt-download-button")).toBeNull()
    expect(norm(el.querySelector("li p.leading-relaxed")!.textContent)).toBe(
      "We've emailed your confirmation number to your account email. You can download any receipt again from Receipts.",
    )
    expect(el.querySelector('a[href="/receipts"]')!.textContent).toBe("Receipts")
  })

  it("renders a paid submission with line items, receipt actions and the payer email", async () => {
    const receipt = listReceipts()[0]
    seed({
      ...FREE,
      lineItems: [{ label: "Application fee", amount: 25 }],
      total: 25,
      paymentMethod: "Visa ending in 4242",
      receiptNumber: receipt.receiptNumber,
      payerEmail: "dana@example.ca",
    })
    const { el } = await mount(FREE.id)
    expect(el.querySelector("h1")!.textContent!.trim()).toBe("Payment successful")
    expect(el.textContent).toContain("Payment details")
    expect(el.textContent).toContain("Application fee")
    expect(el.textContent).toContain("$25.00")
    expect(el.textContent).toContain("Visa ending in 4242")
    expect(el.textContent).toContain(receipt.receiptNumber)
    expect(el.textContent).toContain("Official PDF receipt")
    expect(el.querySelectorAll("receipt-download-button")).toHaveLength(2)
    expect(el.querySelector("span.font-mono")).toBeTruthy()
    expect(el.textContent).toContain("dana@example.ca")
    expect(norm(el.querySelector("li p.leading-relaxed")!.textContent)).toContain(
      "confirmation number and official receipt (PDF attached) to dana@example.ca.",
    )
  })

  it("the Print receipt button calls window.print()", async () => {
    seed(FREE)
    const print = vi.spyOn(window, "print").mockImplementation(() => undefined)
    const { el } = await mount(FREE.id)
    const btn = [...el.querySelectorAll("button")].find((b) => b.textContent!.includes("Print receipt"))!
    btn.click()
    expect(print).toHaveBeenCalledTimes(1)
  })

  it("embeds the document manager keyed by the job number", async () => {
    seed(FREE)
    const { el } = await mount(FREE.id)
    expect(el.querySelector("document-manager")).toBeTruthy()
    expect(el.textContent).toContain("No documents uploaded yet.")
    expect(el.querySelector('input[type="file"]')).toBeTruthy()
    expect(el.textContent).toContain("Supporting documents")
  })
})
