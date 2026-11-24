/** Smoke test: both apps are reachable from the Playwright container and the determinism helpers work in a real browser. */
import { expect, test } from '@playwright/test';
import { routeApi } from '../../tests/support/api-fixtures';
import { FIXED_NOW, freezeClock, seedStorage, stubMapTiles } from '../support/determinism';

test('app boots', async ({ page }, testInfo) => {
  const target = testInfo.project.metadata['target'] as string;
  if (target === 'reference') {
    const res = await page.goto('/login');
    expect(res?.status()).toBe(200);
    await expect(page.locator('body')).toContainText(/\S/);
  } else {
    const res = await page.goto('/');
    expect(res?.status()).toBe(200);
    await expect(page.locator('app-root')).toBeAttached();
  }
});

test('determinism helpers work', async ({ page }) => {
  await seedStorage(page, { 'lcrb.test.seed': 'seeded' }, { 'lcrb.test.session': 'yes' });
  await freezeClock(page);
  await stubMapTiles(page);
  const api = await routeApi(page, { '/api/fixture-check': { json: { ok: true } } });

  await page.goto('/');
  const result = await page.evaluate(async () => ({
    local: localStorage.getItem('lcrb.test.seed'),
    session: sessionStorage.getItem('lcrb.test.session'),
    now: Date.now(),
    fixture: await (await fetch('/api/fixture-check', { method: 'POST' })).json(),
    unmatched: await fetch('/api/not-stubbed').then(
      () => 'reached',
      () => 'aborted',
    ),
    tile: (await fetch('https://a.tile.openstreetmap.org/0/0/0.png')).headers.get('content-type'),
  }));

  expect(result.local).toBe('seeded');
  expect(result.session).toBe('yes');
  expect(result.now).toBe(new Date(FIXED_NOW).getTime());
  expect(result.fixture).toEqual({ ok: true });
  expect(result.unmatched).toBe('aborted');
  expect(result.tile).toBe('image/png');
  expect(api.unmatched).toEqual(['GET /api/not-stubbed']);
  expect(() => api.assertNoUnmatched()).toThrow(/not-stubbed/);
});
