// Pub/sub that lets any part of the portal drive the shared top progress bar.
type ProgressEvent = "start" | "done"

type Listener = (event: ProgressEvent) => void

const listeners = new Set<Listener>()

export function subscribeProgress(listener: Listener): () => void {
  listeners.add(listener)
  return () => listeners.delete(listener)
}

function emit(event: ProgressEvent) {
  // Deliver on a microtask so callers inside a patched history.pushState never update state synchronously.
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

// Briefly show then complete the bar for instant client transitions with no async boundary.
export function pulseProgress(durationMs = 400) {
  startProgress()
  if (typeof window !== "undefined") {
    window.setTimeout(() => doneProgress(), durationMs)
  }
}
