/** Radix-style controllable state: a bound input wins and a user action only emits the change; unbound primitives keep their own state seeded from the default; `[(x)]` still works. */
import { ApplicationRef, computed, inject, signal, type Signal } from '@angular/core';

export function controllable<T>(prop: Signal<T | undefined>, defaultProp: Signal<T>, onChange: (value: T) => void) {
  const own = signal<{ value: T } | null>(null);
  const value = computed<T>(() => {
    const bound = prop();
    if (bound !== undefined) return bound;
    return own()?.value ?? defaultProp();
  });
  const set = (next: T): void => {
    const previous = value();
    if (prop() === undefined) own.set({ value: next });
    if (next !== previous) onChange(next);
  };
  return { value, set };
}

/** React applies state before a click handler returns but zoneless Angular renders later; call after a click to sync aria-checked/data-state. */
export function injectFlushSync(): () => void {
  const app = inject(ApplicationRef);
  let flushing = false;
  return () => {
    if (flushing) return;
    flushing = true;
    try {
      app.tick();
    } catch {
      // A click dispatched during a render: the scheduled render picks it up.
    } finally {
      flushing = false;
    }
  };
}
