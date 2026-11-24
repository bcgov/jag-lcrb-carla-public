import { injectExternalStore } from "@shared/util/external-store"

export function injectCollectionVersion(collection: { subscribe: (cb: () => void) => () => void; snapshot: () => string }) {
  return injectExternalStore(collection.subscribe, collection.snapshot)
}
