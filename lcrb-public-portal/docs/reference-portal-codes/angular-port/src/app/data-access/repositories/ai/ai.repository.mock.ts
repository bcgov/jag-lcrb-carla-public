import { Injectable } from '@angular/core';
import { requestAiJson, readAiJsonCache, hashKey } from './ai-client';
import { gatherPortalSnapshot } from './gather-snapshot';
import { runPreScreenAgent, buildDeterministicAgentLog } from './prescreen-agent';
import { getStartableApplications } from './application-actions';
import type { AiRepository } from './ai.repository';
import { injectAiAutoRetry } from './use-ai-auto-retry';

/** Mock AiRepository over the prototype's seed data and mock services. */
@Injectable()
export class MockAiRepository implements AiRepository {
  readonly requestJson = requestAiJson;

  readonly readJsonCache = readAiJsonCache;

  readonly hashKey = hashKey;

  readonly portalSnapshot = gatherPortalSnapshot;

  readonly preScreen = runPreScreenAgent;

  readonly deterministicAgentLog = buildDeterministicAgentLog;

  readonly startableApplications = getStartableApplications;

  fetchApi(path: `/api/${string}`, init?: RequestInit): Promise<Response> {
    return fetch(path, init);
  }

  readonly autoRetry = injectAiAutoRetry;
}
