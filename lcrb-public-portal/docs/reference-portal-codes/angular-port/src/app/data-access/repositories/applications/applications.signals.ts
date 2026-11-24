import { computed, type Signal } from "@angular/core"
import { read, type Arg } from "@shared/util/external-store"
import { cancellationCollection, getCancellation, listCancellations, type ApplicationCancellation } from "./cancellation-service"
import { injectCollectionVersion } from "../mock-storage/collection-version"

export function injectCancellation(jobNumber: Arg<string | undefined>): Signal<ApplicationCancellation | undefined> {
  const version = injectCollectionVersion(cancellationCollection)
  return computed(() => {
    version()
    const j = read(jobNumber)
    return j ? getCancellation(j) : undefined
  })
}

export function injectCancellations(): Signal<ApplicationCancellation[]> {
  const version = injectCollectionVersion(cancellationCollection)
  return computed(() => (version(), listCancellations()))
}
