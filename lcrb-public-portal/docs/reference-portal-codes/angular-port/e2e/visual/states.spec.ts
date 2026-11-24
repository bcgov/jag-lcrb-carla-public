/** Screenshots of interaction states (dialogs, menus, panels, toasts), viewport-sized so overlays are captured where they show. */
import { expect, test } from '@playwright/test';
import { PORTED_STATES } from '../support/ported';
import { settle } from '../support/determinism';
import { gatedUrlOf, preparePage, skipUnlessPorted } from '../support/parity';
import { STATES } from './states';

for (const entry of STATES) {
  test(`state ${entry.id}`, async ({ page }, testInfo) => {
    skipUnlessPorted(testInfo, PORTED_STATES, entry.id);
    const size = testInfo.project.metadata['size'] as 'desktop' | 'mobile';
    test.skip(!!entry.viewports && !entry.viewports.includes(size), `Not captured on ${size}`);
    const api = await preparePage(page, { role: entry.role, storage: entry.storage });
    await page.goto(entry.gated ? `${gatedUrlOf(testInfo)}${entry.path}` : entry.path);
    await settle(page);
    await entry.act(page);
    await settle(page);
    await expect(page).toHaveScreenshot(`${entry.id}-${size}.png`);
    api.assertNoUnmatched();
  });
}
