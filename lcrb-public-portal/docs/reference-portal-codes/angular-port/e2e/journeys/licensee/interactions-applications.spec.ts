/** Interaction checks: /applications row menus, tabs, stat tiles and filters. */
import { expect, test } from '@playwright/test';
import { preparePage } from '../../support/parity';
import { desktopOnly, expectStorage } from '../../support/flow-helpers';

test.beforeEach(({}, testInfo) => desktopOnly(testInfo));

test('withdraw a submitted application from its row menu', async ({ page }) => {
  const api = await preparePage(page, { role: 'licensee' });
  await page.goto('/applications');
  await page.getByRole('tab', { name: 'Submitted (2)' }).click();

  const row = page.getByRole('row', { name: /LP-125998/ });
  await row.getByRole('button', { name: 'Actions' }).click();
  await page.getByRole('menuitem', { name: 'Cancel Application' }).click();

  const dialog = page.getByRole('dialog', { name: 'Cancel this application?' });
  const confirm = dialog.getByRole('button', { name: 'Cancel application' });
  await expect(confirm).toBeDisabled();
  await dialog.getByRole('radio', { name: 'Plans have changed — no longer proceeding' }).check();
  await dialog.getByRole('checkbox').click();
  await confirm.click();

  await expect(page.getByText('Liquor Primary Licence (LP-125998) has been withdrawn. Reviewers have been notified.')).toBeVisible();
  await expect(page.getByRole('tab', { name: 'Submitted (1)' })).toBeVisible();
  await page.getByRole('tab', { name: 'Completed (3)' }).click();
  await expect(page.getByRole('row', { name: /LP-125998/ })).toBeVisible();
  await expectStorage<unknown>(page, 'lcrb.v2.application-cancellations', (v) => JSON.stringify(v ?? '').includes('LP-125998'));
  api.assertNoUnmatched();
});

test('stat tiles drive the tabs; search and type filter do not filter rows (prototype behaviour)', async ({ page }) => {
  const api = await preparePage(page, { role: 'licensee' });
  await page.goto('/applications');

  const rows = page.getByRole('tabpanel').getByRole('row');
  const inProgressRows = await rows.count();
  expect(inProgressRows).toBeGreaterThan(1);

  // The prototype keeps the search text and type in state but never applies them.
  await page.getByPlaceholder('Search by application type, reference, or ID...').fill('zzz-no-match');
  await expect(rows).toHaveCount(inProgressRows);

  await page.getByRole('button', { name: /Action Required/ }).click();
  await expect(page.getByRole('tab', { name: 'Submitted (2)' })).toHaveAttribute('aria-selected', 'true');
  await expect(page.getByRole('tabpanel').getByRole('row', { name: /CM-125876/ })).toBeVisible();
  await expect(page.getByRole('tabpanel').getByRole('row', { name: /LP-125998/ })).toHaveCount(0);

  await page.getByRole('button', { name: /Under Review/ }).click();
  await expect(page.getByRole('tabpanel').getByRole('row', { name: /LP-125998/ })).toBeVisible();

  await page.getByRole('button', { name: /^2\s*Completed/ }).click();
  await expect(page.getByRole('tab', { name: 'Completed (2)' })).toHaveAttribute('aria-selected', 'true');
  api.assertNoUnmatched();
});
