import { describe, expect, it } from 'vitest';
import { SOURCE_META as LIB_SOURCE_META } from '@server-lib/data/entity-directory';
import { SOURCE_META } from './add-entity-dialog';

describe('AddEntityDialog SOURCE_META copy', () => {
  it('matches the server-only lib constant it copies', () => {
    expect(SOURCE_META).toEqual(LIB_SOURCE_META);
  });
});
