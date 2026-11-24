/** Rounds CDK overlay points to whole device pixels like Radix/floating-ui. Wraps two private CDK methods; the spec fails if a CDK upgrade renames them. */
import { FlexibleConnectedPositionStrategy } from '@angular/cdk/overlay';

type Point = { x: number; y: number };
type PointMethod = (this: unknown, ...args: unknown[]) => Point;

export const ROUNDED_METHODS = ['_getOriginPoint', '_getOverlayPoint'] as const;

const PATCHED = Symbol('lcrbRoundedOverlayPoints');

/** floating-ui's roundByDPR. */
function roundByDpr(value: number): number {
  const dpr = (typeof window !== 'undefined' && window.devicePixelRatio) || 1;
  return Math.round(value * dpr) / dpr;
}

/** Idempotent: wraps the CDK methods once. */
export function roundOverlayPositions(): void {
  const proto = FlexibleConnectedPositionStrategy.prototype as unknown as Record<string | symbol, unknown>;
  if (proto[PATCHED]) return;
  for (const name of ROUNDED_METHODS) {
    const original = proto[name] as PointMethod | undefined;
    if (typeof original !== 'function') throw new Error(`CDK FlexibleConnectedPositionStrategy.${name} not found`);
    proto[name] = function (this: unknown, ...args: unknown[]): Point {
      const point = original.apply(this, args);
      return { x: roundByDpr(point.x), y: roundByDpr(point.y) };
    };
  }
  proto[PATCHED] = true;
}
