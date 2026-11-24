"use client"

import { computed, effect, signal, untracked, type Signal } from "@angular/core"
import {
  CLIENT_AUTO_RETRIES,
  CLIENT_RETRY_DELAY_MS,
  aiErrorMessage,
  classifyAiError,
  isQuotaExhausted,
} from "@domain/ai/ai-errors"

/** Shared transparent-retry behaviour for the streaming AI panels (assistant, licence finder, planner advisor). */
export interface UseAiAutoRetryArgs {
  status: Signal<string>
  error: Signal<Error | undefined>
  busy: Signal<boolean>
  clearError?: () => void
  regenerate: () => void
}

export function injectAiAutoRetry({ status, error, busy, clearError, regenerate }: UseAiAutoRetryArgs) {
  const attemptRef = { current: 0 }
  const reconnecting = signal(false)
  const waitSeconds = signal(0)

  const quota = computed(() => isQuotaExhausted(error()))

  // Reset once a request succeeds.
  effect(() => {
    if (status() === "ready" && !error()) {
      attemptRef.current = 0
      reconnecting.set(false)
      waitSeconds.set(0)
    }
  })

  effect((onCleanup) => {
    const err = error()
    if (!err || busy()) return

    // A hard cap cannot be retried away - stop immediately so the user gets a straight answer instead of a spinner that never resolves.
    if (quota()) {
      reconnecting.set(false)
      waitSeconds.set(0)
      return
    }

    if (attemptRef.current >= CLIENT_AUTO_RETRIES) {
      reconnecting.set(false)
      // Give the user a concrete number rather than a vague "try later".
      waitSeconds.set(Math.ceil(classifyAiError(err).retryAfterMs / 1000))
      return
    }

    attemptRef.current += 1
    reconnecting.set(true)
    const delay = Math.max(classifyAiError(err).retryAfterMs, CLIENT_RETRY_DELAY_MS * attemptRef.current)
    const timer = setTimeout(() => {
      clearError?.()
      regenerate()
    }, delay)
    onCleanup(() => clearTimeout(timer))
  })

  // Tick the "try again in Ns" countdown down to zero.
  effect((onCleanup) => {
    if (waitSeconds() <= 0) return
    const t = setTimeout(() => waitSeconds.update((s) => Math.max(0, s - 1)), 1000)
    onCleanup(() => clearTimeout(t))
  })

  const retry = () => {
    attemptRef.current = 0
    waitSeconds.set(0)
    clearError?.()
    regenerate()
  }

  return {
    /** Quiet retry in progress - show a spinner, not an error. */
    reconnecting: reconnecting.asReadonly(),
    /** Retries are done (or pointless); show the message. */
    showError: computed(
      () => !!error() && !busy() && !reconnecting() && (quota() || untracked(() => attemptRef.current) >= CLIENT_AUTO_RETRIES),
    ),
    /** Honest, classified copy for the user. */
    message: computed(() => {
      const err = error()
      return err ? aiErrorMessage(err) : ""
    }),
    /** True when the failure is a usage cap rather than a blip. */
    isQuota: quota,
    /** Seconds until another attempt is worth making (0 = now). */
    waitSeconds: waitSeconds.asReadonly(),
    /** Manual retry, resetting the attempt budget. */
    retry,
  }
}
