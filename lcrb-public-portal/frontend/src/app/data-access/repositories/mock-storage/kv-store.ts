import { type Entity } from "@domain/models/entity"

// Mock persistence primitive: every service reads and writes through a `Collection<T>`; replace `createCollection` for a real API.

const PREFIX = "lcrb.v2"

type Listener = () => void

const globalListeners = new Map<string, Set<Listener>>()
let storageBound = false

function keyFor(name: string): string {
  return `${PREFIX}.${name}`
}

function notify(name: string) {
  globalListeners.get(name)?.forEach((l) => l())
}

function ensureStorageListener() {
  if (storageBound || typeof window === "undefined") return
  window.addEventListener("storage", (e) => {
    if (!e.key) return
    for (const [name] of globalListeners) {
      if (e.key === keyFor(name)) notify(name)
    }
  })
  storageBound = true
}

export interface Collection<T extends Entity> {
  /** The namespaced storage key (useful for debugging / export). */
  readonly storageKey: string
  /** All records, newest-first insertion order preserved by the caller. */
  list(): T[]
  get(id: string): T | undefined
  /** Insert or replace by id. Returns the saved record. */
  upsert(record: T): T
  /** Patch an existing record by id. No-op if missing. Returns the updated record. */
  update(id: string, patch: Partial<T> | ((prev: T) => T)): T | undefined
  remove(id: string): void
  /** Replace the entire contents (used for seeding). */
  replaceAll(records: T[]): void
  /** Seed the collection only if it is currently empty. */
  seedIfEmpty(records: T[]): void
  /** Subscribe to changes; returns an unsubscribe fn. For useSyncExternalStore. */
  subscribe(cb: Listener): () => void
  /** Stable JSON snapshot for useSyncExternalStore getSnapshot. */
  snapshot(): string
}

export function createCollection<T extends Entity>(name: string): Collection<T> {
  const storageKey = keyFor(name)
  if (!globalListeners.has(name)) globalListeners.set(name, new Set())

  function readMap(): Record<string, T> {
    if (typeof window === "undefined") return {}
    try {
      const raw = localStorage.getItem(storageKey)
      return raw ? (JSON.parse(raw) as Record<string, T>) : {}
    } catch {
      return {}
    }
  }

  function writeMap(map: Record<string, T>) {
    try {
      localStorage.setItem(storageKey, JSON.stringify(map))
    } catch {
      // ignore quota / availability errors in the mock
    }
    notify(name)
  }

  // Preserve insertion order using an array form.
  function readList(): T[] {
    const map = readMap()
    return Object.values(map)
  }

  return {
    storageKey,
    list() {
      return readList()
    },
    get(id) {
      return readMap()[id]
    },
    upsert(record) {
      const map = readMap()
      map[record.id] = record
      writeMap(map)
      return record
    },
    update(id, patch) {
      const map = readMap()
      const prev = map[id]
      if (!prev) return undefined
      const next = typeof patch === "function" ? patch(prev) : { ...prev, ...patch }
      map[id] = next
      writeMap(map)
      return next
    },
    remove(id) {
      const map = readMap()
      if (map[id]) {
        delete map[id]
        writeMap(map)
      }
    },
    replaceAll(records) {
      const map: Record<string, T> = {}
      for (const r of records) map[r.id] = r
      writeMap(map)
    },
    seedIfEmpty(records) {
      const map = readMap()
      if (Object.keys(map).length === 0) {
        this.replaceAll(records)
      }
    },
    subscribe(cb) {
      ensureStorageListener()
      const set = globalListeners.get(name)!
      set.add(cb)
      return () => set.delete(cb)
    },
    snapshot() {
      return typeof window === "undefined" ? "{}" : localStorage.getItem(storageKey) ?? "{}"
    },
  }
}

/** Generate a short, readable, reasonably-unique id with a prefix. */
export function makeId(prefix: string): string {
  const rand = Math.random().toString(36).slice(2, 8)
  const ts = Date.now().toString(36).slice(-4)
  return `${prefix}_${ts}${rand}`
}
