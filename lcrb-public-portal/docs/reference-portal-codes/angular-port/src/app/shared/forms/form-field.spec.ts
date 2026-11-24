/** FormField, FieldHelp (popover + assistant hand-off), FieldHelpContext and InfoBox. */
import { Component } from "@angular/core"
import { type ComponentFixture, TestBed } from "@angular/core/testing"
import { afterEach, describe, expect, it } from "vitest"
import { AssistantProvider } from "@core/assistant/assistant-provider"
import { FieldHelpContext, FormField, InfoBox } from "@shared/forms/index"

async function settle(f: ComponentFixture<unknown>) {
  f.detectChanges()
  await f.whenStable()
  await new Promise((r) => setTimeout(r, 0))
  f.detectChanges()
}

@Component({
  imports: [FormField, InfoBox],
  providers: [{ provide: FieldHelpContext, useValue: () => "From context" }],
  template: `
    <div class="space-y-6" id="wrap">
      <div formField [label]="'Legal name'" [required]="true" [hint]="'As registered'" [htmlFor]="'legalName'" class="extra">
        <input id="legalName" />
      </div>
      <div formField [label]="'City'" [hint]="'Hint'" [error]="'Required'" [warning]="'Warn'" [help]="'Explicit help'"></div>
      <div formField [label]="'Postal'" [warning]="'Check this'"></div>
      <div infoBox variant="success" [title]="'Done'">Body</div>
      <div infoBox>Plain</div>
    </div>
  `,
})
class Host {}

afterEach(() => document.querySelectorAll(".cdk-overlay-container").forEach((c) => (c.innerHTML = "")))

describe("FormField", () => {
  it("renders the label, required star, hint, error, warning and root classes", async () => {
    const f = TestBed.createComponent(Host)
    await settle(f)
    const el = f.nativeElement as HTMLElement
    const [a, b, c] = [...el.querySelectorAll("div[formField]")]
    expect(a.getAttribute("class")).toBe("space-y-2 extra")
    expect(a.parentElement!.id).toBe("wrap")
    const label = a.querySelector("label")!
    expect(label.hasAttribute("data-field-label")).toBe(true)
    expect(label.getAttribute("for")).toBe("legalName")
    expect(label.getAttribute("class")).toBe("block text-sm font-medium text-foreground")
    expect(label.textContent).toBe("Legal name*")
    expect(label.querySelector("span")!.getAttribute("class")).toBe("text-destructive ml-1")
    expect(a.querySelector("p")!.textContent).toBe("As registered")
    // Help comes from the context when no explicit prop is given.
    expect(a.querySelector("[data-field-help] button")!.getAttribute("aria-label")).toBe('What does "Legal name" mean?')

    // An error hides the hint and the warning.
    expect([...b.querySelectorAll("p")].map((p) => p.textContent?.trim())).toEqual(["Required"])
    expect(b.querySelector("p")!.getAttribute("class")).toBe("text-xs text-destructive flex items-center gap-1")
    expect(b.querySelector("p svg")!.getAttribute("class")).toContain("h-3 w-3")

    const warn = c.querySelector("p[role='status']")!
    expect(warn.getAttribute("class")).toBe("flex items-start gap-1.5 rounded-md bg-amber-50 px-2.5 py-2 text-xs text-amber-900")
    expect(warn.textContent?.trim()).toBe("Check this")
  })

  it("opens the help popover and hands the question to the assistant", async () => {
    const f = TestBed.createComponent(Host)
    await settle(f)
    const el = f.nativeElement as HTMLElement
    const trigger = el.querySelectorAll("div[formField]")[1].querySelector("[data-field-help] button") as HTMLButtonElement
    trigger.click()
    await settle(f)
    const content = document.querySelector("[data-slot='popover-content']")!
    expect(content.className).toContain("w-80 text-sm leading-relaxed")
    expect(content.querySelector("p.font-semibold")!.textContent).toBe("City")
    expect(content.querySelector("p.text-muted-foreground")!.textContent).toBe("Explicit help")
    ;(content.querySelector("button[data-slot='button']") as HTMLButtonElement).click()
    const assistant = TestBed.inject(AssistantProvider)
    expect(assistant.open()).toBe(true)
    expect(assistant.pendingPrompt()).toBe(
      'Can you explain "City" in the context of my application, and what I should enter here?',
    )
  })
})

describe("InfoBox", () => {
  it("applies the variant colours and the optional title", async () => {
    const f = TestBed.createComponent(Host)
    await settle(f)
    const [ok, plain] = [...(f.nativeElement as HTMLElement).querySelectorAll("div[infoBox]")]
    expect(ok.getAttribute("class")).toBe("border-l-4 p-4 rounded-r bg-[#F6FFF8] border-[#2E8540] text-[#1E5631]")
    expect(ok.querySelector(".font-semibold.mb-1")!.textContent).toBe("Done")
    expect(plain.getAttribute("class")).toBe("border-l-4 p-4 rounded-r bg-[#F7F9FC] border-[#255A90] text-[#013366]")
    expect(plain.querySelector(".font-semibold")).toBeNull()
    expect(plain.querySelector(".text-sm")!.textContent).toBe("Plain")
  })
})
