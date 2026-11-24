import { Injectable } from '@angular/core';
import { postMessage, markThreadRead, seedChatIfEmpty } from './chat-service';
import { injectThreadMessages, injectThreadUnread, injectThreads } from './messaging.signals';
import type { MessagingRepository } from './messaging.repository';

/** Mock MessagingRepository over the prototype's seed data and mock services. */
@Injectable()
export class MockMessagingRepository implements MessagingRepository {
  readonly threadMessages = injectThreadMessages;

  readonly threadUnread = injectThreadUnread;

  readonly threads = injectThreads;

  readonly postMessage = postMessage;

  readonly markThreadRead = markThreadRead;

  readonly seedIfEmpty = seedChatIfEmpty;
}
