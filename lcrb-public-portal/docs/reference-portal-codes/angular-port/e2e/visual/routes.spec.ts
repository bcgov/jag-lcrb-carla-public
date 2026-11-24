/** Full-page screenshot of every route in the matrix. */
import { expect, test } from '@playwright/test';
import { PORTED_ROUTES } from '../support/ported';
import { fastForwardAnimations, settle, waitForCharts } from '../support/determinism';
import { preparePage, skipUnlessPorted, targetOf } from '../support/parity';
import { ECHARTS_SHAPES, ROUTES } from './routes';

for (const entry of ROUTES) {
  test(`route ${entry.id}`, async ({ page }, testInfo) => {
    skipUnlessPorted(testInfo, PORTED_ROUTES, entry.id);
    const api = await preparePage(page, { role: entry.role, storage: entry.storage });
    if (entry.charts) await fastForwardAnimations(page);
    await page.goto(entry.path);
    await settle(page);
    const size = testInfo.project.metadata['size'] as string;
    if (entry.charts) {
      // A full-page capture resizes the viewport and remounts the charts, so grow the viewport to the page height, let them redraw, then take a viewport screenshot.
      const height = await page.evaluate(() => document.documentElement.scrollHeight);
      await page.setViewportSize({ width: page.viewportSize()!.width, height });
      await settle(page);
      await waitForCharts(page, targetOf(testInfo) === 'angular' ? ECHARTS_SHAPES : entry.charts);
      await expect(page).toHaveScreenshot(`${entry.id}-${size}.png`);
    } else {
      await expect(page).toHaveScreenshot(`${entry.id}-${size}.png`, { fullPage: true });
    }
    api.assertNoUnmatched();
  });
}
