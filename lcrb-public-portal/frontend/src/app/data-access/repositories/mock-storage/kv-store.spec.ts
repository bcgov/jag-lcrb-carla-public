import { afterEach, describe, expect, it, vi } from 'vitest';
import { createCollection } from './kv-store';

afterEach(() => vi.restoreAllMocks());

describe('createCollection snapshot', () => {
  it('returns an empty snapshot when localStorage is blocked', () => {
    vi.spyOn(Storage.prototype, 'getItem').mockImplementation(() => {
      throw new DOMException('blocked', 'SecurityError');
    });
    expect(createCollection('blocked-test').snapshot()).toBe('{}');
  });
});
