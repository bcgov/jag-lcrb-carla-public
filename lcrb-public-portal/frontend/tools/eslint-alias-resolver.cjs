// Resolves the tsconfig path aliases for eslint-plugin-boundaries (import resolver interface v2).
const fs = require('node:fs');
const path = require('node:path');
const { createRequire } = require('node:module');

const root = path.resolve(__dirname, '..');
const ALIASES = [
  ['@core/', 'src/app/core/'],
  ['@domain/', 'src/app/domain/'],
  ['@data-access/', 'src/app/data-access/'],
  ['@shared/', 'src/app/shared/'],
  ['@features/', 'src/app/features/'],
  ['@/', 'src/'],
];

function firstFile(base) {
  for (const candidate of [`${base}.ts`, path.join(base, 'index.ts'), base]) {
    if (fs.existsSync(candidate) && fs.statSync(candidate).isFile()) return candidate;
  }
  return null;
}

module.exports = {
  interfaceVersion: 2,
  resolve(source, file) {
    const alias = ALIASES.find(([prefix]) => source.startsWith(prefix));
    if (alias) {
      const found = firstFile(path.join(root, alias[1], source.slice(alias[0].length)));
      return found ? { found: true, path: found } : { found: false };
    }
    if (source.startsWith('.')) {
      const found = firstFile(path.resolve(path.dirname(file), source));
      return found ? { found: true, path: found } : { found: false };
    }
    try {
      return { found: true, path: createRequire(file).resolve(source) };
    } catch {
      return { found: false };
    }
  },
};
