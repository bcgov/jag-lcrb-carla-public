/** Vitest config for Node-side tests: the Express server, build-output checks and the bundle secrets scan. Angular component tests run through `ng test`. */
import path from 'node:path';
import { defineConfig } from 'vitest/config';

export default defineConfig({
  resolve: {
    // Same aliases as tsconfig.json, most specific first.
    alias: [
      { find: /^@core\//, replacement: path.resolve(import.meta.dirname, 'src/app/core') + '/' },
      { find: /^@domain\//, replacement: path.resolve(import.meta.dirname, 'src/app/domain') + '/' },
      { find: /^@data-access\//, replacement: path.resolve(import.meta.dirname, 'src/app/data-access') + '/' },
      { find: /^@shared\//, replacement: path.resolve(import.meta.dirname, 'src/app/shared') + '/' },
      { find: /^@features\//, replacement: path.resolve(import.meta.dirname, 'src/app/features') + '/' },
      { find: /^@server-lib\//, replacement: path.resolve(import.meta.dirname, 'server/lib') + '/' },
      { find: /^@\//, replacement: path.resolve(import.meta.dirname, 'src') + '/' },
    ],
  },
  test: {
    environment: 'node',
    include: ['tests/server/**/*.test.ts', 'tests/*.test.ts', 'e2e/visual/*.test.ts'],
  },
});
