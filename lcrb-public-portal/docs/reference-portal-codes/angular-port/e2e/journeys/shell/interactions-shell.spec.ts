/** Interaction checks: header bell, global search, accessibility bar and sign out. */
import { expect, test } from '@playwright/test';
import { preparePage } from '../../support/parity';
import { desktopOnly, expectStorage, readStorage } from '../../support/flow-helpers';

test.beforeEach(({}, testInfo) => desktopOnly(testInfo));

test('header bell: mark all read and mute the notice chime; Ctrl+K search opens a licence', async ({ page }) => {
  const api = await preparePage(page, { role: 'licensee' });
  await page.goto('/dashboard');

  await page.getByRole('button', { name: /^Notifications, \d+ unread$/ }).click();
  await page.getByRole('button', { name: 'Mark all read' }).click();
  await expect(page.getByRole('button', { name: 'Notifications', exact: true })).toBeVisible();
  const mute = page.getByRole('button', { name: 'Turn notice chime off' });
  await mute.click();
  await expect(page.getByRole('button', { name: 'Turn notice chime on' })).toHaveAttribute('aria-pressed', 'true');
  expect(await page.evaluate(() => localStorage.getItem('lcrb.notice-chime.muted'))).toBe('1');
  await page.keyboard.press('Escape');

  await page.keyboard.press('Control+k');
  const search = page.getByRole('dialog', { name: 'Search the portal' });
  await search.locator('input[placeholder="Search licences, applications, permits, referrals…"]').fill('326739');
  await search.getByRole('option').first().click();
  await expect(page).toHaveURL(/\/licences\/326739$/);
  api.assertNoUnmatched();
});

test('accessibility bar: text size and contrast persist; Sign Out returns to login', async ({ page }) => {
  const api = await preparePage(page, { role: 'licensee' });
  await page.goto('/dashboard');
  const fontSize = () => page.evaluate(() => getComputedStyle(document.documentElement).fontSize);
  const base = await fontSize();

  await page.getByRole('button', { name: 'Increase text size' }).click();
  await expect.poll(fontSize).not.toBe(base);
  await page.getByRole('button', { name: 'Reset text size' }).click();
  await expect.poll(fontSize).toBe(base);

  await page.getByRole('button', { name: /High contrast/ }).click();
  await expect(page.locator('html')).toHaveClass(/high-contrast/);
  await expect(page.getByRole('button', { name: /Standard contrast/ })).toHaveAttribute('aria-pressed', 'true');
  await expectStorage<{ highContrast?: boolean }>(page, 'lcrb.a11y.prefs', (v) => v?.highContrast === true);

  await page.getByRole('button', { name: /^MH/ }).click();
  await page.getByRole('menuitem', { name: 'Sign Out' }).click();
  await expect(page).toHaveURL(/\/login$/);
  expect(await readStorage(page, 'lcrb.mock.session')).toBeNull();
  api.assertNoUnmatched();
});
