import { computed, type Signal } from "@angular/core"
import { read, type Arg } from "@shared/util/external-store"
import { chatCollection, chatReadCollection, listMessages, listThreads, unreadForThread, type ChatMessage, type ThreadSummary } from "./chat-service"
import { type ThreadKind } from "@domain/models/messaging"
import type { UserRole } from "@domain/roles"
import { injectCollectionVersion } from "../mock-storage/collection-version"

export function injectThreadMessages(threadId: Arg<string>): Signal<ChatMessage[]> {
  const version = injectCollectionVersion(chatCollection)
  return computed(() => (version(), listMessages(read(threadId))))
}

export function injectThreadUnread(threadId: Arg<string>, role: Arg<UserRole>): Signal<number> {
  const messages = injectCollectionVersion(chatCollection)
  const reads = injectCollectionVersion(chatReadCollection)
  return computed(() => (messages(), reads(), unreadForThread(read(threadId), read(role))))
}

export function injectThreads(role: Arg<UserRole>, kind?: Arg<ThreadKind | undefined>): Signal<ThreadSummary[]> {
  const messages = injectCollectionVersion(chatCollection)
  const reads = injectCollectionVersion(chatReadCollection)
  return computed(() => (messages(), reads(), listThreads(read(role), kind === undefined ? undefined : read(kind))))
}
