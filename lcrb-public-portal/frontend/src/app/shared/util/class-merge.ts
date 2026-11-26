// Base for primitive directives: host class is cn(base, consumerClass), so consumer classes win.
import { computed, Directive, input } from '@angular/core';
import { cn } from './utils';

@Directive({ host: { '[class]': 'hostClass()' } })
export abstract class ClassMerge {
  /** The consumer's classes (static `class="…"` or bound `[class]`). */
  readonly userClass = input<string | undefined>(undefined, { alias: 'class' });
  protected abstract baseClass(): string;
  protected readonly hostClass = computed(() => cn(this.baseClass(), this.userClass()));
}
