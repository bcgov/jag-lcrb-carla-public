/** Flow 8: AI card request, cache and reuse. The reviewer brief is click-gated: the first run needs a click, then the cached result shows without a new call. */
import { expect, test } from '@playwright/test';
import { API_FIXTURES } from '../../../tests/fixtures/api';
import { PORTED_FLOWS } from '../../support/ported';
import { preparePage, skipUnlessPorted } from '../../support/parity';

const SUMMARY = (API_FIXTURES['/api/reviewer-brief'].json as { summary: string }).summary;

test('flow 8: reviewer brief, cache and reuse', async ({ page }, testInfo) => {
  skipUnlessPorted(testInfo, PORTED_FLOWS, 'flow-08');
  const api = await preparePage(page, { role: 'police' });
  await page.goto('/police/review/SEP-2026-04812');

  const brief = page.getByRole('region', { name: 'AI reviewer brief' });
  await brief.getByRole('button', { name: 'Generate brief' }).click();
  await expect(brief.getByText(SUMMARY)).toBeVisible();
  await expect(brief.getByText('Low risk')).toBeVisible();

  const cacheKeys = await page.evaluate(() => Object.keys(sessionStorage).filter((k) => k.startsWith('lcrb.brief.')));
  expect(cacheKeys).toHaveLength(1);

  // Reload: the cached brief shows straight away, with no second request.
  await page.reload();
  await expect(page.getByRole('region', { name: 'AI reviewer brief' }).getByText(SUMMARY)).toBeVisible();
  expect(api.calls['/api/reviewer-brief']).toBe(1);
  api.assertNoUnmatched();
});
