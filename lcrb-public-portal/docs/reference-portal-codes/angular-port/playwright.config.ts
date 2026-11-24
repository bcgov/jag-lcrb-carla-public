/** Playwright setup for the Playwright container (docker compose profile "test"): the Angular app at two viewports, URLs from the *_URL variables (.env.example); screenshot baselines come from the app itself (make v2-visual-baseline). */
import { defineConfig, devices } from '@playwright/test';

const targets = {
  angular: {
    baseURL: process.env['ANGULAR_URL'] ?? 'http://angular:8080',
    gatedURL: process.env['ANGULAR_GATED_URL'] ?? 'http://angular-gated:8080',
    screenshot: { maxDiffPixelRatio: 0.01 },
  },
} as const;

const viewports = {
  desktop: { width: 1280, height: 800 },
  mobile: { width: 390, height: 844 },
} as const;

export default defineConfig({
  testDir: 'e2e',
  testMatch: '**/*.spec.ts',
  outputDir: 'test-results',
  snapshotPathTemplate: '{testDir}/visual/__screenshots__/{arg}{ext}',
  fullyParallel: true,
  forbidOnly: true,
  retries: 0,
  timeout: 60_000,
  reporter: [['list'], ['html', { open: 'never', outputFolder: 'playwright-report' }]],
  use: {
    trace: 'retain-on-failure',
    // Deterministic rendering for screenshot comparison.
    locale: 'en-CA',
    timezoneId: 'America/Vancouver',
    colorScheme: 'light',
  },
  projects: Object.entries(targets).flatMap(([target, t]) =>
    Object.entries(viewports).map(([size, viewport]) => ({
      name: `${target}-${size}`,
      metadata: { target, size, gatedURL: t.gatedURL },
      use: { ...devices['Desktop Chrome'], baseURL: t.baseURL, viewport },
      expect: { toHaveScreenshot: { ...t.screenshot, animations: 'disabled' as const, caret: 'hide' as const } },
    })),
  ),
});
