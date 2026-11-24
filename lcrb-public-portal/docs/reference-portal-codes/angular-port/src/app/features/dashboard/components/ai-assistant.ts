/** Dashboard prompt box (inline) plus an unused floating variant; the reply is simulated after 1 s and the inline variant never shows the collected messages. */
import { ChangeDetectionStrategy, Component, type ElementRef, input, type OnInit, signal, viewChild } from '@angular/core';
import { Button } from '@shared/ui/primitives/button';
import { Input } from '@shared/ui/primitives/input';
import { LucideIcon } from '@shared/ui/primitives/lucide-icon';
import { cn } from '@shared/util/utils';

interface Message {
  id: string;
  role: 'user' | 'assistant';
  content: string;
}

const defaultSuggestions = [
  'Check upcoming licence renewals',
  'Start a Liquor Primary application',
  'Validate application documents',
  'What licence type do I need?',
];

@Component({
  selector: 'bc-ai-assistant',
  imports: [Button, Input, LucideIcon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    @if (variant() === 'floating') {
      @if (!isOpen()) {
        <button
          uiButton
          (click)="isOpen.set(true)"
          class="fixed bottom-6 right-6 h-14 w-14 rounded-full bg-[#013366] hover:bg-[#38598A] shadow-lg z-50"
        >
          <svg lucide="MessageSquare" class="h-6 w-6"></svg>
          <span class="sr-only">Open AI Assistant</span>
        </button>
      }
      @if (isOpen()) {
        <div
          class="fixed bottom-6 right-6 w-96 max-w-[calc(100vw-3rem)] bg-white rounded-lg shadow-2xl border z-50 flex flex-col max-h-[600px]"
        >
          <div class="flex items-center justify-between p-4 border-b bg-[#013366] text-white rounded-t-lg">
            <div class="flex items-center gap-2">
              <svg lucide="Sparkles" class="h-5 w-5"></svg>
              <span class="font-semibold">AI Assistant (Beta)</span>
            </div>
            <button
              uiButton
              variant="ghost"
              size="icon"
              class="text-white hover:bg-[#38598A] h-8 w-8"
              (click)="isOpen.set(false)"
            >
              <svg lucide="X" class="h-4 w-4"></svg>
            </button>
          </div>

          <div class="flex-1 overflow-y-auto p-4 space-y-4 min-h-[200px]">
            @if (messages().length === 0) {
              <div class="text-center text-muted-foreground py-8">
                <svg lucide="Sparkles" class="h-8 w-8 mx-auto mb-2 text-[#013366]"></svg>
                <p class="text-sm">
                  I am an AI assistant that can help you find information, navigate the portal, and complete applications.
                </p>
                <p class="text-xs mt-2 text-muted-foreground">
                  This information is AI generated and may be incomplete or contain errors.
                </p>
              </div>
            } @else {
              @for (message of messages(); track message.id) {
                <div [attr.class]="messageClass(message)">{{ message.content }}</div>
              }
            }
            @if (isLoading()) {
              <div class="bg-muted p-3 rounded-lg mr-8">
                <div class="flex gap-1">
                  <span class="w-2 h-2 bg-[#013366] rounded-full animate-bounce"></span>
                  <span class="w-2 h-2 bg-[#013366] rounded-full animate-bounce [animation-delay:0.1s]"></span>
                  <span class="w-2 h-2 bg-[#013366] rounded-full animate-bounce [animation-delay:0.2s]"></span>
                </div>
              </div>
            }
          </div>

          @if (messages().length === 0) {
            <div class="px-4 pb-2 flex flex-wrap gap-2">
              @for (suggestion of suggestions().slice(0, 3); track suggestion) {
                <button
                  (click)="handleSuggestionClick(suggestion)"
                  class="text-xs px-3 py-1.5 bg-secondary rounded-full hover:bg-secondary/80 transition-colors"
                >
                  {{ suggestion }}
                </button>
              }
            </div>
          }

          <form (submit)="handleSubmit($event)" class="p-4 border-t">
            <div class="flex gap-2">
              <input
                uiInput
                #inputEl
                [value]="input()"
                (input)="input.set($any($event.target).value)"
                placeholder="Ask or say what you need..."
                class="flex-1"
              />
              <button
                uiButton
                type="submit"
                size="icon"
                [disabled]="!input().trim() || isLoading()"
                class="bg-[#013366] hover:bg-[#38598A]"
              >
                <svg lucide="Send" class="h-4 w-4"></svg>
              </button>
            </div>
          </form>
        </div>
      }
    } @else {
      <!-- Inline variant (for dashboard) -->
      <div class="w-full">
        <form (submit)="handleSubmit($event)" class="relative">
          <input
            uiInput
            #inputEl
            [value]="input()"
            (input)="input.set($any($event.target).value)"
            [placeholder]="placeholder()"
            class="h-14 pl-4 pr-24 text-base rounded-lg border-2 border-muted focus:border-[#013366] transition-colors"
          />
          <div class="absolute right-2 top-1/2 -translate-y-1/2 flex items-center gap-1">
            <button uiButton type="button" variant="ghost" size="icon" class="h-10 w-10 text-muted-foreground hover:text-[#013366]">
              <svg lucide="Sparkles" class="h-5 w-5"></svg>
              <span class="sr-only">AI suggestions</span>
            </button>
            <button uiButton type="button" variant="ghost" size="icon" class="h-10 w-10 text-muted-foreground hover:text-[#013366]">
              <svg lucide="Mic" class="h-5 w-5"></svg>
              <span class="sr-only">Voice input</span>
            </button>
          </div>
        </form>
      </div>
    }
  `,
})
export class AIAssistant implements OnInit {
  readonly variant = input<'inline' | 'floating'>('inline');
  readonly placeholder = input('Enter a prompt here');
  readonly suggestions = input<string[]>(defaultSuggestions);

  // useState(variant === "inline"): the initial value only, read once.
  protected readonly isOpen = signal(false);
  protected readonly input = signal('');
  protected readonly messages = signal<Message[]>([]);
  protected readonly isLoading = signal(false);
  private readonly inputEl = viewChild<ElementRef<HTMLInputElement>>('inputEl');

  ngOnInit(): void {
    this.isOpen.set(this.variant() === 'inline');
  }

  protected messageClass(message: Message): string {
    return cn('p-3 rounded-lg text-sm', message.role === 'user' ? 'bg-[#013366] text-white ml-8' : 'bg-muted mr-8');
  }

  protected handleSubmit(e: Event): void {
    e.preventDefault();
    const input = this.input();
    if (!input.trim()) return;

    const userMessage: Message = { id: Date.now().toString(), role: 'user', content: input };
    this.messages.update((prev) => [...prev, userMessage]);
    this.input.set('');
    this.isLoading.set(true);

    setTimeout(() => {
      const assistantMessage: Message = {
        id: (Date.now() + 1).toString(),
        role: 'assistant',
        content: `I can help you with "${input}". Here are some options:\n\n1. Navigate to the relevant section\n2. Start a new application\n3. Check your existing licences\n\nWould you like me to guide you through any of these?`,
      };
      this.messages.update((prev) => [...prev, assistantMessage]);
      this.isLoading.set(false);
    }, 1000);
  }

  protected handleSuggestionClick(suggestion: string): void {
    this.input.set(suggestion);
    this.inputEl()?.nativeElement.focus();
  }
}
