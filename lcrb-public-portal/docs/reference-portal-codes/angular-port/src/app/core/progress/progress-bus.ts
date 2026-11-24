// Tiny pub/sub so any part of the portal can drive the shared top progress bar; route changes are handled in the bar component.
type ProgressEvent = "start" | "done"

type Listener = (event: ProgressEvent) => void

const listeners = new Set<Listener>()

export function subscribeProgress(listener: Listener): () => void {
  listeners.add(listener)
  return () => listeners.delete(listener)
}

function emit(event: ProgressEvent) {
  // Deliver in a microtask, not synchronously; the prototype needed this to avoid scheduling updates during React's commit phase.
  const run = () => listeners.forEach((l) => l(event))
  if (typeof queueMicrotask === "function") {
    queueMicrotask(run)
  } else {
    Promise.resolve().then(run)
  }
}

/** Begin (or restart) the progress animation. */
export function startProgress() {
  emit("start")
}

/** Finish the progress animation. */
export function doneProgress() {
  emit("done")
}

/** For instant client transitions with no async boundary: briefly show then complete the bar. */
export function pulseProgress(durationMs = 400) {
  startProgress()
  if (typeof window !== "undefined") {
    window.setTimeout(() => doneProgress(), durationMs)
  }
}
