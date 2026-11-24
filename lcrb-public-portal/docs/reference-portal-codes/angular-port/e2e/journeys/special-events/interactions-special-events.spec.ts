/** Interaction checks: SEP list row actions and the servings calculator hand-off. */
import { expect, test } from '@playwright/test';
import { preparePage } from '../../support/parity';
import { desktopOnly, expectStorage } from '../../support/flow-helpers';

test.beforeEach(({}, testInfo) => desktopOnly(testInfo));

test('SEP list: download an approved permit and cancel a pending one', async ({ page }) => {
  const api = await preparePage(page, { role: 'applicant' });
  await page.goto('/special-events');

  await page.getByRole('row', { name: /SEP-2026-04699/ }).getByRole('button', { name: 'Actions' }).click();
  await page.getByRole('menuitem', { name: 'Download Permit' }).click();
  await expect(page.getByText('Permit downloaded')).toBeVisible();
  await expect(page.getByText('Lakeside Beer & BBQ (SEP-2026-04699)')).toBeVisible();

  await page.getByRole('row', { name: /SEP-2026-04812/ }).getByRole('button', { name: 'Actions' }).click();
  await page.getByRole('menuitem', { name: 'Cancel Permit' }).click();
  const dialog = page.getByRole('dialog', { name: 'Cancel this permit?' });
  await dialog.getByRole('radio', { name: 'Other' }).check();
  await expect(dialog.getByRole('button', { name: 'Cancel permit' })).toBeDisabled();
  await dialog.getByPlaceholder('Anything LCRB should know for the record…').fill('Venue fell through');
  await dialog.getByRole('checkbox').click();
  await dialog.getByRole('button', { name: 'Cancel permit' }).click();

  await expect(page.getByText('Permit cancelled')).toBeVisible();
  await expectStorage<{ id: string; status: string }[]>(page, 'lcrb.sep.records', (records) =>
    records?.find((r) => r.id === 'SEP-2026-04812')?.status === 'cancelled',
  );
  api.assertNoUnmatched();
});

test('servings calculator: the estimate updates and carries over to the SEP application', async ({ page }) => {
  const api = await preparePage(page, { role: 'applicant' });
  await page.goto('/special-events/servings-calculator');
  const total = page.getByText('Total standard drinks').locator('xpath=following-sibling::p[1]');
  await expect(total).toHaveText('330');

  await page.getByLabel(/Expected guests|Guests/).first().fill('100');
  await page.getByRole('button', { name: /Lively/ }).click();
  await expect(total).toHaveText('537'); // 75 drinkers x 6.5 drinks, +10% buffer, rounded up

  await page.getByRole('button', { name: 'Apply for a Special Event Permit' }).click();
  await expect(page).toHaveURL(/\/special-events\/apply$/);
  await expect(page.getByText('Prefilled from the Servings Calculator')).toBeVisible();
  api.assertNoUnmatched();
});
