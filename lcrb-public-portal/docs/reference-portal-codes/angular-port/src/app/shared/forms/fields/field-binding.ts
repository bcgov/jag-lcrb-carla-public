/** Binding helpers shared by field components: `useField` becomes `injectField`, `FieldShell` a base directive, `Conditional` a structural directive. */
import {
  booleanAttribute,
  computed,
  Directive,
  effect,
  inject,
  input,
  TemplateRef,
  untracked,
  ViewContainerRef,
  type EmbeddedViewRef,
  type Signal,
} from "@angular/core"
import { injectAssistant } from "@core/assistant/assistant-provider"
import { cn } from "@shared/util/utils"
import { warnValue } from "../validation"
import { type FieldValidation } from "@domain/forms/field-validation"
import { injectApplicationContext, type ApplicationFlowState } from "../provider"

export interface FieldSpec {
  name: string
  rules?: FieldValidation
  required?: boolean
  label?: string
}

/** Registers `rules` with the provider while mounted, re-registering when `deps()` changes and unregistering on destroy. */
export function registerFieldValidator(
  ctx: ApplicationFlowState,
  name: () => string,
  rules: () => FieldValidation | undefined,
  label: () => string | undefined,
  deps: () => unknown[],
): void {
  const key = computed(() => JSON.stringify([name(), ...deps(), label()]))
  effect((onCleanup) => {
    key()
    const r = untracked(rules)
    if (!r) return
    const n = untracked(name)
    ctx.registerValidator(n, r, untracked(label))
    onCleanup(() => ctx.unregisterValidator(n))
  })
}

export function injectField(spec: () => FieldSpec) {
  const ctx = injectApplicationContext()

  const effectiveRules = computed<FieldValidation | undefined>(() => {
    const { rules, required } = spec()
    return rules || required ? { ...(rules ?? {}), required: required ?? rules?.required } : undefined
  })

  registerFieldValidator(
    ctx,
    () => spec().name,
    effectiveRules,
    () => spec().label,
    () => [JSON.stringify(spec().rules), spec().required],
  )

  const value = computed(() => (ctx.formData()[spec().name] as string) ?? "")
  return {
    value,
    set: (v: unknown) => ctx.updateFormData({ [spec().name]: v }),
    onBlur: () => ctx.setFieldTouched(spec().name),
    error: computed(() => ctx.getFieldError(spec().name, effectiveRules())),
    // Live, not touch-gated: a warning is advice, shown as soon as the date is picked.
    warning: computed(() => warnValue(value(), effectiveRules())),
  }
}

/** Renders and validates its content only when `when` is true: `<div *conditional="when">`. */
@Directive({ selector: "[conditional]" })
export class Conditional {
  readonly conditional = input.required<boolean>()
  private readonly template = inject(TemplateRef)
  private readonly container = inject(ViewContainerRef)
  private view: EmbeddedViewRef<unknown> | null = null

  constructor() {
    effect(() => {
      const on = this.conditional()
      untracked(() => {
        if (on && !this.view) this.view = this.container.createEmbeddedView(this.template)
        else if (!on && this.view) {
          this.container.clear()
          this.view = null
        }
      })
    })
  }
}

/** Wrapper giving each field a stable id/`data-field` hook and an AI highlight; the host is the wrapper, and `bare` turns it off when ConfigField is already the wrapper. */
@Directive({
  host: {
    "[attr.data-field]": "shellActive() ? name() : null",
    "[attr.id]": "shellActive() ? 'field-' + name() : null",
    "[attr.class]": "shellClass()",
    "[style.display]": "displayStyle()",
    "(focusin)": "onFocusIn()",
  },
})
export abstract class FieldShell {
  abstract readonly name: Signal<string>
  readonly bare = input(false, { transform: booleanAttribute })
  protected readonly assistant = injectAssistant()
  protected readonly shellActive: Signal<boolean> = computed(() => !this.bare())
  protected readonly displayStyle: Signal<string | null> = computed(() => (this.bare() ? "contents" : null))
  protected readonly shellClass = computed(() => {
    if (!this.shellActive()) return null
    const active = this.assistant.highlightedField() === this.name()
    return cn(
      "scroll-mt-24 rounded-lg transition-all",
      active &&
        "ring-2 ring-offset-2 ring-[#1A5A96] bg-[#1A5A96]/5 p-3 -m-3 shadow-[0_0_0_4px_rgba(26,90,150,0.15)]",
    )
  })

  // If the AI highlighted one field but the user edits a different one, clear the stale highlight.
  protected onFocusIn(): void {
    if (!this.shellActive()) return
    const highlighted = this.assistant.highlightedField()
    if (highlighted && highlighted !== this.name()) this.assistant.highlightField(null)
  }
}
