// Playwright smoke tests, run in the Playwright container against the nginx container (BASE_URL).
import { defineConfig, devices } from '@playwright/test';

export default defineConfig({
  testDir: 'e2e',
  testMatch: '**/*.spec.ts',
  outputDir: 'test-results',
  forbidOnly: true,
  retries: 0,
  timeout: 30_000,
  reporter: [['list']],
  use: {
    ...devices['Desktop Chrome'],
    baseURL: process.env['BASE_URL'] ?? 'http://web:8080',
    trace: 'retain-on-failure',
  },
});
