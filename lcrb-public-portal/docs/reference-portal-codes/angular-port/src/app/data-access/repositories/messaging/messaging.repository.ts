import { InjectionToken } from '@angular/core';
import type { postMessage, markThreadRead, seedChatIfEmpty } from './chat-service';
import type { injectThreadMessages, injectThreadUnread, injectThreads } from './messaging.signals';
import { MockMessagingRepository } from './messaging.repository.mock';

/** Messaging repository (backend area: none (case threads)). */
export interface MessagingRepository {
  readonly threadMessages: typeof injectThreadMessages;
  readonly threadUnread: typeof injectThreadUnread;
  readonly threads: typeof injectThreads;
  readonly postMessage: typeof postMessage;
  readonly markThreadRead: typeof markThreadRead;
  readonly seedIfEmpty: typeof seedChatIfEmpty;
}

export const MESSAGING_REPOSITORY = new InjectionToken<MessagingRepository>('MessagingRepository', {
  providedIn: 'root',
  factory: () => new MockMessagingRepository(),
});
