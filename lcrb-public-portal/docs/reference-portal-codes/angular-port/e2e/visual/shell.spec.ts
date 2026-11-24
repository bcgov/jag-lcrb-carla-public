/** Element screenshots of the shell (header, footer, header overlays). */
import { expect, test } from '@playwright/test';
import { PORTED_STATES } from '../support/ported';
import { settle } from '../support/determinism';
import { preparePage, skipUnlessPorted } from '../support/parity';
import { SHELL, SHELL_PATH } from './shell';

for (const entry of SHELL) {
  test(`shell ${entry.id}`, async ({ page }, testInfo) => {
    skipUnlessPorted(testInfo, PORTED_STATES, entry.id);
    const size = testInfo.project.metadata['size'] as 'desktop' | 'mobile';
    test.skip(!!entry.viewports && !entry.viewports.includes(size), `Not captured on ${size}`);
    const api = await preparePage(page, { role: entry.role });
    await page.goto(SHELL_PATH);
    await settle(page);
    if (entry.style) await page.addStyleTag({ content: entry.style });
    if (entry.act) {
      await entry.act(page);
      await settle(page);
    }
    await expect(entry.target(page)).toHaveScreenshot(`${entry.id}-${size}.png`);
    api.assertNoUnmatched();
  });
}
