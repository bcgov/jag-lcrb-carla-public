import { InjectionToken } from '@angular/core';
import type { requestAiJson, readAiJsonCache, hashKey } from './ai-client';
import type { gatherPortalSnapshot } from './gather-snapshot';
import type { runPreScreenAgent, buildDeterministicAgentLog } from './prescreen-agent';
import type { getStartableApplications } from './application-actions';
import { MockAiRepository } from './ai.repository.mock';
import type { injectAiAutoRetry } from './use-ai-auto-retry';

/** Ai repository (backend area: 3.5 AI routes). */
export interface AiRepository {
  readonly requestJson: typeof requestAiJson;
  readonly readJsonCache: typeof readAiJsonCache;
  readonly hashKey: typeof hashKey;
  readonly portalSnapshot: typeof gatherPortalSnapshot;
  readonly preScreen: typeof runPreScreenAgent;
  readonly deterministicAgentLog: typeof buildDeterministicAgentLog;
  readonly startableApplications: typeof getStartableApplications;
  /** The AI API routes the components call directly (`/api/<route>`). */
  fetchApi(path: `/api/${string}`, init?: RequestInit): Promise<Response>;
  readonly autoRetry: typeof injectAiAutoRetry;
}

export const AI_REPOSITORY = new InjectionToken<AiRepository>('AiRepository', {
  providedIn: 'root',
  factory: () => new MockAiRepository(),
});
