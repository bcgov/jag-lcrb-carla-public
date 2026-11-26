// Asset-free "new notice" chime synthesized with Web Audio; fails silently when audio is blocked.

import { effect } from "@angular/core"
import { read, type Arg } from "@shared/util/external-store"

let audioCtx: AudioContext | null = null

function getContext(): AudioContext | null {
  if (typeof window === "undefined") return null
  const Ctor: typeof AudioContext | undefined =
    window.AudioContext ?? (window as unknown as { webkitAudioContext?: typeof AudioContext }).webkitAudioContext
  if (!Ctor) return null
  if (!audioCtx) {
    try {
      audioCtx = new Ctor()
    } catch {
      return null
    }
  }
  return audioCtx
}

const CHIME_MUTE_KEY = "lcrb.notice-chime.muted"
const CHIME_LOGIN_KEY = "lcrb.notice-chime.login-played"

/** Whether the user has muted the notice chime (persists across sessions). */
export function isNoticeChimeMuted(): boolean {
  if (typeof window === "undefined") return false
  return window.localStorage.getItem(CHIME_MUTE_KEY) === "1"
}

/** Persist the user's mute preference for the notice chime. */
export function setNoticeChimeMuted(muted: boolean): void {
  if (typeof window === "undefined") return
  if (muted) window.localStorage.setItem(CHIME_MUTE_KEY, "1")
  else window.localStorage.removeItem(CHIME_MUTE_KEY)
}

// Play the chime once; no-op when muted or when the browser blocks audio.
export function playNoticeChime(): void {
  if (isNoticeChimeMuted()) return
  const ctx = getContext()
  if (!ctx) return
  // Browsers suspend the context until a user gesture; resume best-effort.
  if (ctx.state === "suspended") void ctx.resume().catch(() => {})

  const now = ctx.currentTime
  const master = ctx.createGain()
  master.gain.value = 0.6
  master.connect(ctx.destination)

  // Two soft notes: A5 then D6, a pleasant rising interval.
  const notes = [
    { freq: 880, start: 0 },
    { freq: 1174.66, start: 0.13 },
  ]
  for (const { freq, start } of notes) {
    const osc = ctx.createOscillator()
    const gain = ctx.createGain()
    osc.type = "sine"
    osc.frequency.value = freq
    const t = now + start
    // Quick attack, gentle exponential decay for a bell-like tone.
    gain.gain.setValueAtTime(0.0001, t)
    gain.gain.exponentialRampToValueAtTime(0.35, t + 0.02)
    gain.gain.exponentialRampToValueAtTime(0.0001, t + 0.55)
    osc.connect(gain)
    gain.connect(master)
    osc.start(t)
    osc.stop(t + 0.6)
  }
}

// Chimes once on login when unread notices exist (after hydration) and whenever the unread count rises.
export function injectNoticeChime(unreadArg: Arg<number>, readyArg: Arg<boolean>, enabledArg: Arg<boolean>): void {
  const prevUnread: { current: number | null } = { current: null }

  effect(() => {
    const unread = read(unreadArg)
    const ready = read(readyArg)
    const enabled = read(enabledArg)
    if (!enabled || !ready) return

    // First reading after hydration: chime at most once per browser session, then track increases.
    if (prevUnread.current === null) {
      prevUnread.current = unread
      if (unread > 0 && window.sessionStorage.getItem(CHIME_LOGIN_KEY) !== "1") {
        window.sessionStorage.setItem(CHIME_LOGIN_KEY, "1")
        playNoticeChime()
      }
      return
    }

    // A new notice became available in real time.
    if (unread > prevUnread.current) {
      playNoticeChime()
    }
    prevUnread.current = unread
  })
}
