/** Interaction checks: /account delegates, notification preferences and role settings. */
import { expect, test } from '@playwright/test';
import { preparePage } from '../../support/parity';
import { desktopOnly, expectStorage } from '../../support/flow-helpers';

test.beforeEach(({}, testInfo) => desktopOnly(testInfo));

test('delegates: validation, add, change access level and remove', async ({ page }) => {
  const api = await preparePage(page, { role: 'licensee' });
  await page.goto('/account');
  await page.getByRole('tab', { name: 'Delegates' }).click();

  await page.getByRole('button', { name: 'Add Delegate' }).click();
  const dialog = page.getByRole('dialog', { name: 'Add Delegate' });
  await dialog.getByRole('button', { name: 'Add Delegate' }).click();
  await expect(dialog.getByText('Please enter both a name and an email address.')).toBeVisible();
  await dialog.getByLabel('Full Name').fill('Riley Chen');
  await dialog.getByLabel('Email Address').fill('not-an-email');
  await dialog.getByRole('button', { name: 'Add Delegate' }).click();
  await expect(dialog.getByText('Please enter a valid email address.')).toBeVisible();
  await dialog.getByLabel('Email Address').fill('riley.chen@example.com');
  await dialog.getByRole('button', { name: 'Add Delegate' }).click();
  await expect(dialog).toBeHidden();

  const row = page.getByRole('row', { name: /Riley Chen/ });
  await expect(row).toContainText('riley.chen@example.com');
  await expect(row).toContainText('Pending');
  await expect(row.getByRole('combobox')).toContainText('View Only');
  await row.getByRole('combobox').click();
  await page.getByRole('option', { name: 'Submit Only' }).click();
  await expect(row.getByRole('combobox')).toContainText('Submit Only');

  await row.getByRole('button').last().click();
  const confirm = page.getByRole('alertdialog', { name: 'Remove Delegate?' });
  await expect(confirm).toContainText("This will revoke Riley Chen's access to your account.");
  await confirm.getByRole('button', { name: 'Remove' }).click();
  await expect(page.getByRole('row', { name: /Riley Chen/ })).toHaveCount(0);
  await expect(page.getByRole('row', { name: /Jane Smith/ })).toBeVisible();
  api.assertNoUnmatched();
});

test('notification preferences: save shows a confirmation that clears on the next change', async ({ page }) => {
  const api = await preparePage(page, { role: 'licensee' });
  await page.goto('/account');
  await page.getByRole('tab', { name: 'Notifications' }).click();

  const sms = page.getByRole('switch', { name: 'SMS notifications' });
  const before = await sms.getAttribute('aria-checked');
  await sms.click();
  await expect(sms).not.toHaveAttribute('aria-checked', before ?? '');
  await page.getByRole('button', { name: 'Save Preferences' }).click();
  await expect(page.getByText('Preferences saved')).toBeVisible();
  await page.getByRole('switch', { name: 'Application Updates' }).click();
  await expect(page.getByText('Preferences saved')).toHaveCount(0);
  api.assertNoUnmatched();
});

test('local government: turn commenting off and back on', async ({ page }) => {
  const api = await preparePage(page, { role: 'local-government' });
  await page.goto('/account');
  await page.getByRole('tab', { name: 'Commenting' }).click();
  await expect(page.getByText('Commenting on', { exact: true })).toBeVisible();

  await page.getByRole('switch', { name: 'Turn off commenting on applications' }).click();
  const confirm = page.getByRole('alertdialog', { name: 'Turn off commenting on applications?' });
  await confirm.getByRole('button', { name: 'Turn off commenting' }).click();
  await expect(page.getByText('Commenting turned off')).toBeVisible();
  await expect(page.getByText('Commenting off', { exact: true })).toBeVisible();
  await expectStorage<{ optedOut?: boolean }>(page, 'lcrb.lg.commenting', (v) => v?.optedOut === true);

  await page.getByRole('switch', { name: 'Turn off commenting on applications' }).click();
  await expect(page.getByText('Commenting turned back on')).toBeVisible();
  await expectStorage<{ optedOut?: boolean }>(page, 'lcrb.lg.commenting', (v) => v?.optedOut === false);
  api.assertNoUnmatched();
});

test('police: turn SEP auto-approval on, keep it, then turn it off', async ({ page }) => {
  const api = await preparePage(page, { role: 'police' });
  await page.goto('/account');
  await page.getByRole('tab', { name: 'Auto-approval' }).click();

  const toggle = page.locator('#auto-approval');
  await expect(toggle).toHaveAttribute('aria-checked', 'false');
  await toggle.click();
  await page.getByRole('dialog', { name: 'Turn auto-approval on?' }).getByRole('button', { name: 'Keep current setting' }).click();
  await expect(toggle).toHaveAttribute('aria-checked', 'false');

  await toggle.click();
  await page.getByRole('dialog', { name: 'Turn auto-approval on?' }).getByRole('button', { name: 'Turn on' }).click();
  await expect(page.getByText('Auto-approval turned on')).toBeVisible();
  await expect(toggle).toHaveAttribute('aria-checked', 'true');
  await expectStorage<unknown>(page, 'lcrb.v2.sep-auto-approval', (v) => JSON.stringify(v ?? '').includes('"enabled":true'));

  await toggle.click();
  await page.getByRole('dialog', { name: 'Turn auto-approval off?' }).getByRole('button', { name: 'Turn off' }).click();
  await expect(page.getByText('Auto-approval turned off')).toBeVisible();
  await expect(toggle).toHaveAttribute('aria-checked', 'false');
  api.assertNoUnmatched();
});
