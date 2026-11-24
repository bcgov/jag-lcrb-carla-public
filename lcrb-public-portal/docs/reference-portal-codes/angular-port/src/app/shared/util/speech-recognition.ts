// Typing and accessor for the Web Speech API; declares only what the partial DOM lib lacks, since redeclaring shipped types conflicts.

declare global {
  interface SpeechRecognitionEvent extends Event {
    readonly resultIndex: number
    readonly results: SpeechRecognitionResultList
  }

  interface SpeechRecognitionErrorEvent extends Event {
    // TypeScript 6's DOM lib types this error code, which conflicts with the prototype's `string`; type-only.
    readonly error: SpeechRecognitionErrorCode
    readonly message: string
  }

  interface SpeechRecognition extends EventTarget {
    continuous: boolean
    interimResults: boolean
    lang: string
    onresult: ((event: SpeechRecognitionEvent) => void) | null
    onerror: ((event: SpeechRecognitionErrorEvent) => void) | null
    onend: (() => void) | null
    start(): void
    stop(): void
    abort(): void
  }
}

/** Constructor type for a SpeechRecognition instance. */
export type SpeechRecognitionConstructor = new () => SpeechRecognition

/** The browser's SpeechRecognition constructor (standard or webkit), or `null` on the server or when unsupported. */
export function getSpeechRecognitionCtor(): SpeechRecognitionConstructor | null {
  if (typeof window === "undefined") return null
  const w = window as unknown as {
    SpeechRecognition?: SpeechRecognitionConstructor
    webkitSpeechRecognition?: SpeechRecognitionConstructor
  }
  return w.SpeechRecognition ?? w.webkitSpeechRecognition ?? null
}

/** Whether speech-to-text is available in the current environment. */
export function isSpeechRecognitionSupported(): boolean {
  return getSpeechRecognitionCtor() !== null
}
