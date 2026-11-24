import { InjectionToken } from '@angular/core';
import type { computeFitAndProper } from './fit-and-proper-engine';
import { MockScreeningRepository } from './screening.repository.mock';

/** Screening repository (backend area: 6.5 Associate security screening). */
export interface ScreeningRepository {
  readonly fitAndProper: typeof computeFitAndProper;
}

export const SCREENING_REPOSITORY = new InjectionToken<ScreeningRepository>('ScreeningRepository', {
  providedIn: 'root',
  factory: () => new MockScreeningRepository(),
});
