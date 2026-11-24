import { computed, type Signal } from "@angular/core"
import { read, type Arg } from "@shared/util/external-store"
import { documentCollection, listDocuments, type StoredDocument } from "./document-service"
import { injectCollectionVersion } from "../mock-storage/collection-version"

export function injectDocuments(subjectId: Arg<string>): Signal<StoredDocument[]> {
  const version = injectCollectionVersion(documentCollection)
  return computed(() => (version(), listDocuments(read(subjectId))))
}
