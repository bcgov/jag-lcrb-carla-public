/** Shadcn toast store kept as in the prototype: its only caller raises toasts but no Toaster is ever mounted, so they never appear. */
import { signal } from '@angular/core';

const TOAST_LIMIT = 1;

export interface ToasterToast {
  id: string;
  title?: string;
  description?: string;
  open?: boolean;
}

let count = 0;
function genId() {
  count = (count + 1) % Number.MAX_SAFE_INTEGER;
  return count.toString();
}

const toasts = signal<ToasterToast[]>([]);

export function toast(props: Omit<ToasterToast, 'id'>) {
  const id = genId();
  const dismiss = () => toasts.update((list) => list.map((t) => (t.id === id ? { ...t, open: false } : t)));
  toasts.update((list) => [{ ...props, id, open: true }, ...list].slice(0, TOAST_LIMIT));
  return { id, dismiss };
}

/** The prototype's useToast(): the toast list (no outlet renders it) and the toast function. */
export function injectToast() {
  return {
    toasts: toasts.asReadonly(),
    toast,
    dismiss: (toastId?: string) =>
      toasts.update((list) => list.map((t) => (toastId === undefined || t.id === toastId ? { ...t, open: false } : t))),
  };
}
