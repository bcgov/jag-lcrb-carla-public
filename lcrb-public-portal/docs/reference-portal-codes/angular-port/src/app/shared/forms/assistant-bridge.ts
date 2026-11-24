/** Publishes the active form (fields, values, setters) and per-field metadata to the AI assistant so it can read context, fill fields and change steps. */
import { afterRenderEffect, ChangeDetectionStrategy, Component, input, untracked } from "@angular/core"
import { injectAssistant, type AssistantField } from "@core/assistant/assistant-provider"
import { injectApplicationContext } from "./index"

export interface BridgeFieldMeta {
  name: string
  label: string
  type: string
  required?: boolean
  help?: string
  options?: { value: string; label: string }[]
  /** Which step index this field belongs to. */
  step: number
}

@Component({
  selector: "assistant-form-bridge",
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: "display: contents" },
  template: ``,
})
export class AssistantFormBridge {
  readonly flowTitle = input.required<string>()
  /** Static description of every field across all steps. */
  readonly fieldMeta = input.required<BridgeFieldMeta[]>()

  private readonly ctx = injectApplicationContext()
  private readonly assistant = injectAssistant()

  constructor() {
    // Registers after first render and when the flow title or step count changes, and clears on destroy; methods read live state when called.
    afterRenderEffect((onCleanup) => {
      const flowTitle = this.flowTitle()
      const totalSteps = this.ctx.steps().length
      const { ctx } = this
      const fieldMeta = untracked(this.fieldMeta)
      const steps = untracked(ctx.steps)
      const currentStep = () => untracked(ctx.currentStep)
      const formData = () => untracked(ctx.formData)

      this.assistant.registerFormBridge({
        flowTitle,
        get currentStep() {
          return currentStep()
        },
        get stepTitle() {
          return steps[currentStep()]?.title ?? "Review"
        },
        totalSteps,
        getFields(): AssistantField[] {
          const step = currentStep()
          return fieldMeta
            .filter((f) => f.step === step)
            .map((f) => ({
              name: f.name,
              label: f.label,
              type: f.type,
              required: f.required,
              help: f.help,
              options: f.options,
              value: formData()[f.name],
            }))
        },
        setFieldValue(name: string, value: unknown) {
          const meta = fieldMeta.find((f) => f.name === name)
          if (!meta) return false
          // Go to the field's step so the user sees the change.
          if (meta.step !== currentStep()) {
            ctx.goToStep(meta.step)
          }
          ctx.updateFormData({ [name]: value })
          return true
        },
        stepOfField(name: string) {
          return fieldMeta.find((f) => f.name === name)?.step ?? null
        },
        getAllFields(): AssistantField[] {
          return fieldMeta.map((f) => ({
            name: f.name,
            label: f.label,
            type: f.type,
            required: f.required,
            help: f.help,
            options: f.options,
            step: f.step,
            value: formData()[f.name],
          }))
        },
        goToStep: ctx.goToStep,
        tryGoToStep(step: number) {
          if (step <= currentStep()) {
            ctx.goToStep(step)
            return { ok: true, blockedBy: [] }
          }
          // Forward: apply the Next button's gate, which also shows the inline field errors on the form.
          const blockedBy = ctx.blockersOnCurrentStep()
          if (blockedBy.length > 0) return { ok: false, blockedBy }
          ctx.goToStep(step)
          return { ok: true, blockedBy: [] }
        },
        blockersPreview: ctx.peekBlockers,
        nextStep: ctx.nextStep,
        prevStep: ctx.prevStep,
      })
      onCleanup(() => this.assistant.registerFormBridge(null))
    })
  }
}
