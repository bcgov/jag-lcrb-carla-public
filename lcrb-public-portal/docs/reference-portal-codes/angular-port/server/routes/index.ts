/** The 21 API route handlers, one file each; a route's `maxDuration` (Vercel's function limit) becomes its request timeout here. */
import express, { type Request, type Response, type Router } from 'express';
import { adapt, type WebHandler } from '../web-adapter';
import * as activitySearch from './activity-search';
import * as adverseMedia from './adverse-media';
import * as assessment from './assessment';
import * as assistant from './assistant';
import * as decisionRationale from './decision-rationale';
import * as documentCheck from './document-check';
import * as entityDirectory from './entity-directory';
import * as file from './file';
import * as files from './files';
import * as interpretAnswer from './interpret-answer';
import * as licenceActions from './licence-actions';
import * as licenceFinder from './licence-finder';
import * as nudges from './nudges';
import * as oneDoor from './one-door';
import * as plannerAdvisor from './planner-advisor';
import * as reviewerActions from './reviewer-actions';
import * as reviewerBrief from './reviewer-brief';
import * as statusExplainer from './status-explainer';
import * as suggestApplications from './suggest-applications';
import * as upload from './upload';
import * as workloadSuggest from './workload-suggest';

interface RouteModule {
  GET?: WebHandler;
  POST?: WebHandler;
  maxDuration?: number;
}

/** Path (under `/api`) to handler module. */
export const API_ROUTES: Readonly<Record<string, RouteModule>> = {
  'activity-search': activitySearch,
  'adverse-media': adverseMedia,
  assessment,
  assistant,
  'decision-rationale': decisionRationale,
  'document-check': documentCheck,
  'entity-directory': entityDirectory,
  file,
  files,
  'interpret-answer': interpretAnswer,
  'licence-actions': licenceActions,
  'licence-finder': licenceFinder,
  nudges,
  'one-door': oneDoor,
  'planner-advisor': plannerAdvisor,
  'reviewer-actions': reviewerActions,
  'reviewer-brief': reviewerBrief,
  'status-explainer': statusExplainer,
  'suggest-applications': suggestApplications,
  upload,
  'workload-suggest': workloadSuggest,
};

/** Once `maxDuration` passes, a response not yet started gets a 504 and a started stream is ended. */
export function timeout(seconds: number) {
  return (_req: Request, res: Response, next: () => void) => {
    res.setTimeout(seconds * 1000, () => {
      if (res.headersSent) res.end();
      else res.status(504).type('text/plain').send('Function timed out');
    });
    next();
  };
}

export function apiRouter(): Router {
  const router = express.Router();
  for (const [name, mod] of Object.entries(API_ROUTES)) {
    const limits = mod.maxDuration ? [timeout(mod.maxDuration)] : [];
    if (mod.GET) router.get(`/${name}`, ...limits, adapt(mod.GET));
    if (mod.POST) router.post(`/${name}`, ...limits, adapt(mod.POST));
  }
  return router;
}
