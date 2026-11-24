/** Assistant state: panel open state, active form bridge, field highlight and a pending prompt. */
import { inject, Injectable, signal } from '@angular/core';

export interface AssistantField {
  name: string;
  label: string;
  type: string;
  required?: boolean;
  help?: string;
  options?: { value: string; label: string }[];
  value: unknown;
  /** Step index this field lives on (present in the full field catalog). */
  step?: number;
}

/** The AI SDK's default generateId (16 characters, no prefix), so `ai` stays out of the initial bundle. */
const CHAT_ID_ALPHABET = '0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz';
export function generateChatId(size = 16): string {
  const chars = new Array<string>(size);
  for (let i = 0; i < size; i++) chars[i] = CHAT_ID_ALPHABET[(Math.random() * CHAT_ID_ALPHABET.length) | 0];
  return chars.join('');
}

/** Bridge an application form publishes so the assistant can read its fields and values, set values and change steps. */
export interface FormBridge {
  flowTitle: string;
  currentStep: number;
  stepTitle: string;
  totalSteps: number;
  getFields: () => AssistantField[];
  /** Every field across all steps, including its step index. */
  getAllFields: () => AssistantField[];
  /** Returns the step index a field lives on, or null if unknown. */
  stepOfField: (name: string) => number | null;
  setFieldValue: (name: string, value: unknown) => boolean;
  goToStep: (step: number) => void;
  /** Moves the wizard through the same validation gate as "Next"; returns the blocking fields when refused. */
  tryGoToStep: (step: number) => {
    ok: boolean;
    blockedBy: { name: string; label: string; error: string }[];
  };
  /** What would block a forward move right now, without flagging any field. */
  blockersPreview: () => { name: string; label: string; error: string }[];
  nextStep: () => void;
  prevStep: () => void;
}

@Injectable({ providedIn: 'root' })
export class AssistantProvider {
  private readonly openState = signal(false);
  private readonly bridgeState = signal<FormBridge | null>(null);
  private readonly highlightState = signal<string | null>(null);
  private readonly promptState = signal<string | null>(null);
  private highlightTimer: ReturnType<typeof setTimeout> | null = null;

  /** Chat id, drawn when a session first exists; the panel is deferred, so the id is drawn here in the prototype's random-draw order. */
  private chatIdValue: string | null = null;
  get chatId(): string | null {
    return this.chatIdValue;
  }
  readonly ensureChatId = (): void => {
    this.chatIdValue ??= generateChatId();
  };

  readonly open = this.openState.asReadonly();
  // Active form bridge (null when not on an application page).
  readonly bridge = this.bridgeState.asReadonly();
  // Field highlight coordination (assistant -> form UI).
  readonly highlightedField = this.highlightState.asReadonly();
  // Seed text dropped into the composer when the panel opens.
  readonly pendingPrompt = this.promptState.asReadonly();

  readonly setOpen = (open: boolean): void => this.openState.set(open);
  readonly toggle = (): void => this.openState.update((o) => !o);
  readonly registerFormBridge = (b: FormBridge | null): void => this.bridgeState.set(b);
  readonly setPendingPrompt = (prompt: string | null): void => this.promptState.set(prompt);

  readonly highlightField = (name: string | null): void => {
    this.highlightState.set(name);
    if (this.highlightTimer) clearTimeout(this.highlightTimer);
    if (name) {
      // Scroll the field into view when highlighted.
      if (typeof document !== 'undefined') {
        const el = document.getElementById(`field-${name}`);
        el?.scrollIntoView({ behavior: 'smooth', block: 'center' });
      }
      // Auto-clear the highlight after a while so it doesn't linger forever.
      this.highlightTimer = setTimeout(() => this.highlightState.set(null), 6000);
    }
  };

  readonly askAssistant = (prompt: string): void => {
    this.promptState.set(prompt);
    this.openState.set(true);
  };
}

/** The prototype's useAssistant(). */
export function injectAssistant(): AssistantProvider {
  return inject(AssistantProvider);
}
