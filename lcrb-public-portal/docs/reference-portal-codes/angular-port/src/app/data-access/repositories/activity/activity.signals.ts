import { computed, type Signal } from "@angular/core"
import { read, type Arg } from "@shared/util/external-store"
import { auditCollection, listAudit, listAuditForSubject, type AuditEntry } from "./audit-service"
import { dynamicsCollection, listSyncRecords, type CaseSyncRecord } from "./dynamics-service"
import { injectCollectionVersion } from "../mock-storage/collection-version"

export function injectAuditTrail(): Signal<AuditEntry[]> {
  const version = injectCollectionVersion(auditCollection)
  return computed(() => (version(), listAudit()))
}

export function injectSubjectAudit(subjectId: Arg<string>): Signal<AuditEntry[]> {
  const version = injectCollectionVersion(auditCollection)
  return computed(() => (version(), listAuditForSubject(read(subjectId))))
}

export function injectSyncRecords(): Signal<CaseSyncRecord[]> {
  const version = injectCollectionVersion(dynamicsCollection)
  return computed(() => (version(), listSyncRecords()))
}
