/** The visual matrix must use unique screenshot ids. */
import { describe, expect, it } from 'vitest';
import { ROUTES } from './routes';
import { SHELL } from './shell';
import { STATES } from './states';

describe('visual matrix', () => {
  it('uses unique screenshot ids across routes, states and shell', () => {
    const ids = [...ROUTES.map((r) => r.id), ...STATES.map((s) => s.id), ...SHELL.map((s) => s.id)];
    expect(new Set(ids).size).toBe(ids.length);
  });
});
