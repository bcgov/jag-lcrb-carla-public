/** Angular equivalents of the React primitives the prototype's hooks use; `useX()` hooks become `injectX()` functions returning signals. */
import { afterNextRender, DestroyRef, inject, signal, type Signal } from '@angular/core';

/** A hook argument: a plain value, or a getter (for example a signal) so the result stays reactive. */
export type Arg<T> = T | (() => T);

export function read<T>(arg: Arg<T>): T {
  return typeof arg === 'function' ? (arg as () => T)() : arg;
}

/** Like useSyncExternalStore: a signal holding `getSnapshot()`, refreshed on store notify; must run in an injection context. */
export function injectExternalStore<T>(
  subscribe: (cb: () => void) => () => void,
  getSnapshot: () => T,
  getServerSnapshot?: () => T,
): Signal<T> {
  // With a server snapshot, React renders it first and switches to the client snapshot right after; same here.
  const value = signal(getServerSnapshot ? getServerSnapshot() : getSnapshot());
  const unsubscribe = subscribe(() => value.set(getSnapshot()));
  inject(DestroyRef).onDestroy(unsubscribe);
  if (getServerSnapshot) afterNextRender(() => value.set(getSnapshot()));
  return value.asReadonly();
}

/** False on the first render, true after it, like the prototype's `useState(false)` + `useEffect`. */
export function injectHydrated(onHydrate?: () => void): Signal<boolean> {
  const hydrated = signal(false);
  afterNextRender(() => {
    onHydrate?.();
    hydrated.set(true);
  });
  return hydrated.asReadonly();
}
