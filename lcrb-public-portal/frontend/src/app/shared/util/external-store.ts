/** Signal helpers for external stores, used by the `injectX()` functions. */
import { afterNextRender, DestroyRef, inject, signal, type Signal } from '@angular/core';

/** A hook argument: a plain value, or a getter (for example a signal) so the result stays reactive. */
export type Arg<T> = T | (() => T);

export function read<T>(arg: Arg<T>): T {
  return typeof arg === 'function' ? (arg as () => T)() : arg;
}

// Signal of getSnapshot(), refreshed on store changes; must run in an injection context.
export function injectExternalStore<T>(
  subscribe: (cb: () => void) => () => void,
  getSnapshot: () => T,
  getServerSnapshot?: () => T,
): Signal<T> {
  // With a server snapshot, render it first and switch to the client snapshot right after (loading first).
  const value = signal(getServerSnapshot ? getServerSnapshot() : getSnapshot());
  const unsubscribe = subscribe(() => value.set(getSnapshot()));
  inject(DestroyRef).onDestroy(unsubscribe);
  if (getServerSnapshot) afterNextRender(() => value.set(getSnapshot()));
  return value.asReadonly();
}

/** False on the first render, true after it (calls onHydrate first). */
export function injectHydrated(onHydrate?: () => void): Signal<boolean> {
  const hydrated = signal(false);
  afterNextRender(() => {
    onHydrate?.();
    hydrated.set(true);
  });
  return hydrated.asReadonly();
}
