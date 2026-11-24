import { computed, type Signal } from "@angular/core"
import { autoApprovalCollection, getAutoApprovalPolicy, type SepAutoApprovalPolicy } from "./sep-auto-approval"
import { injectCollectionVersion } from "../mock-storage/collection-version"

export function injectAutoApprovalPolicy(): Signal<SepAutoApprovalPolicy> {
  const version = injectCollectionVersion(autoApprovalCollection)
  return computed(() => (version(), getAutoApprovalPolicy()))
}
