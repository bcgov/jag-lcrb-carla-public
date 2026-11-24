/** CDK overlay points are rounded to whole device pixels, as floating-ui does in Radix. */
import { FlexibleConnectedPositionStrategy } from '@angular/cdk/overlay';
import { ROUNDED_METHODS, roundOverlayPositions } from './overlay-pixel-rounding';

type Proto = Record<string, (...args: unknown[]) => { x: number; y: number }>;
const proto = FlexibleConnectedPositionStrategy.prototype as unknown as Proto;
const ltr = { _isRtl: () => false };
const rect = { left: 974.59375, right: 1010.59375, top: 59, bottom: 95, width: 36, height: 36 };
const container = { left: 0, top: 0 };
const endBottom = { originX: 'end', originY: 'bottom', overlayX: 'end', overlayY: 'top' };

describe('overlay pixel rounding', () => {
  it('finds the private CDK methods it wraps (fails loudly after a CDK upgrade)', () => {
    for (const name of ROUNDED_METHODS) expect(typeof proto[name]).toBe('function');
  });

  it('rounds the origin and overlay points, once', () => {
    roundOverlayPositions();
    roundOverlayPositions();
    const origin = proto['_getOriginPoint'].call(ltr, rect, container, endBottom);
    expect(origin).toEqual({ x: 1011, y: 95 });
    const overlay = proto['_getOverlayPoint'].call(ltr, { x: 1010.59375, y: 95 }, { width: 384, height: 468 }, endBottom);
    expect(overlay).toEqual({ x: 627, y: 95 });
  });
});
