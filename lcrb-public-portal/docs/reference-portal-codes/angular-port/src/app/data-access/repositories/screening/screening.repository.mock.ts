import { Injectable } from '@angular/core';
import { computeFitAndProper } from './fit-and-proper-engine';
import type { ScreeningRepository } from './screening.repository';

/** Mock ScreeningRepository over the prototype's seed data and mock services. */
@Injectable()
export class MockScreeningRepository implements ScreeningRepository {
  readonly fitAndProper = computeFitAndProper;
}
