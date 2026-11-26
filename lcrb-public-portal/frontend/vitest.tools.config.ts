// Vitest config for Node-side repo tests (tools and boundaries). Angular tests run through `ng test`.
import path from 'node:path';
import { defineConfig } from 'vitest/config';

const dir = (p: string) => path.resolve(import.meta.dirname, p) + '/';

export default defineConfig({
  resolve: {
    alias: [
      { find: /^@core\//, replacement: dir('src/app/core') },
      { find: /^@domain\//, replacement: dir('src/app/domain') },
      { find: /^@data-access\//, replacement: dir('src/app/data-access') },
      { find: /^@shared\//, replacement: dir('src/app/shared') },
      { find: /^@features\//, replacement: dir('src/app/features') },
      { find: /^@\//, replacement: dir('src') },
    ],
  },
  test: {
    environment: 'node',
    include: ['tests/*.test.ts'],
  },
});
