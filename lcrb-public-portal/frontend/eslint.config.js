// @ts-check
import { fileURLToPath } from 'node:url';
import angular from 'angular-eslint';
import boundaries from 'eslint-plugin-boundaries';
import prettier from 'eslint-config-prettier/flat';
import tseslint from 'typescript-eslint';

const resolver = fileURLToPath(new URL('./tools/eslint-alias-resolver.cjs', import.meta.url));

// Layer rules R1-R6; literal routes, browser globals in domain and cycles stay in tools/check-boundaries.ts.
const layers = {
  plugins: { boundaries },
  settings: {
    'import/resolver': { [resolver]: {} },
    'boundaries/elements': [
      { type: 'seed', pattern: 'src/app/data-access/repositories/*/mock', partialMatch: false },
      { type: 'domain', pattern: 'src/app/domain', partialMatch: false },
      { type: 'data-access', pattern: 'src/app/data-access', partialMatch: false },
      { type: 'core', pattern: 'src/app/core', partialMatch: false },
      { type: 'shared-ui', pattern: 'src/app/shared/ui', partialMatch: false },
      { type: 'shared-forms', pattern: 'src/app/shared/forms', partialMatch: false },
      { type: 'shared-util', pattern: 'src/app/shared/*', partialMatch: false },
      { type: 'feature', pattern: 'src/app/features/*', capture: ['feature'], partialMatch: false },
      { type: 'app', pattern: 'src/app', partialMatch: false },
    ],
  },
  rules: {
    'boundaries/dependencies': [
      'error',
      {
        default: 'allow',
        policies: [
          {
            from: { element: { type: 'domain' } },
            disallow: [{ to: { element: { types: { noneOf: ['domain'] } } } }, { to: { module: { source: '@angular/**' } } }],
            message: 'domain imports only domain and never Angular',
          },
          {
            from: { element: { types: { anyOf: ['data-access', 'seed'] } } },
            disallow: { to: { element: { types: { noneOf: ['domain', 'data-access', 'seed', 'shared-util'] } } } },
            message: 'data-access imports only domain, data-access and shared/util',
          },
          {
            from: { element: { types: { noneOf: ['feature', 'app'] } } },
            disallow: { to: { element: { type: 'feature' } } },
            message: 'only app wiring imports a feature',
          },
          {
            from: { element: { type: 'feature' } },
            disallow: { to: { element: { type: 'feature', captured: { feature: '!{{ from.element.captured.feature }}' } } } },
            message: 'a feature never imports another feature',
          },
          {
            from: { element: { types: { noneOf: ['data-access', 'seed', 'app'] } } },
            disallow: { to: { element: { type: 'seed' } } },
            message: 'only data-access imports mock data',
          },
          {
            from: { element: { types: { noneOf: ['data-access', 'seed', 'app'] } } },
            disallow: {
              to: { element: { type: 'data-access', fileInternalPath: '!**/*.repository.ts' } },
              dependency: { kind: 'value' },
            },
            message: 'outside data-access, import data-access only through *.repository.ts files',
          },
        ],
      },
    ],
  },
};

export default tseslint.config(
  { ignores: ['dist/', '.angular/', 'node_modules/', 'test-results/', 'playwright-report/'] },
  {
    files: ['**/*.ts'],
    extends: [...tseslint.configs.recommended, ...angular.configs.tsRecommended],
    processor: angular.processInlineTemplates,
    rules: {
      // Components take a host `class` input and merge it with their own classes.
      '@angular-eslint/no-input-rename': ['error', { allowedNames: ['class'] }],
    },
  },
  { files: ['src/**/*.ts'], ignores: ['**/*.spec.ts'], ...layers },
  {
    files: ['**/*.html'],
    extends: [...angular.configs.templateRecommended, ...angular.configs.templateAccessibility],
  },
  prettier,
);
