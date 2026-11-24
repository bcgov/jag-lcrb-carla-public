/** Interaction checks: /licences filters and downloads, licence detail tabs and certificate. */
import { expect, test } from '@playwright/test';
import { preparePage } from '../../support/parity';
import { chooseOption, desktopOnly, downloadName, expectStorage } from '../../support/flow-helpers';

test.beforeEach(({}, testInfo) => desktopOnly(testInfo));

const card = (page: import('@playwright/test').Page, name: string) => page.getByText(name, { exact: true });

test('licence search, status and type filters narrow the cards; Clear resets the search', async ({ page }) => {
  const api = await preparePage(page, { role: 'licensee' });
  await page.goto('/licences');
  await expect(card(page, 'Restaurant ABC').first()).toBeVisible();

  const search = page.getByPlaceholder('Search by establishment, licence number, address or type…');
  await search.fill('Green Leaf');
  await expect(card(page, 'Green Leaf Victoria').first()).toBeVisible();
  await expect(card(page, 'Restaurant ABC')).toHaveCount(0);
  await page.getByRole('button', { name: 'Clear' }).click();
  await expect(search).toHaveValue('');
  await expect(card(page, 'Restaurant ABC').first()).toBeVisible();

  await chooseOption(page, 'Filter by status', 'Pending renewal');
  await expect(card(page, 'Green Leaf Victoria').first()).toBeVisible();
  await expect(card(page, 'Restaurant ABC')).toHaveCount(0);
  await chooseOption(page, 'Filter by status', 'All statuses');

  await chooseOption(page, 'Filter by licence type', 'Food Primary');
  await expect(card(page, 'Restaurant ABC').first()).toBeVisible();
  await expect(card(page, 'Green Leaf Victoria')).toHaveCount(0);
  api.assertNoUnmatched();
});

test('licence card "Download Licence Copy" downloads the licence PDF', async ({ page }) => {
  const api = await preparePage(page, { role: 'licensee' });
  await page.goto('/licences');
  await page.getByPlaceholder('Search by establishment, licence number, address or type…').fill('Restaurant ABC');
  await page.getByRole('button', { name: 'Actions' }).first().click();
  const name = await downloadName(page, () => page.getByRole('menuitem', { name: 'Download Licence Copy' }).click());
  expect(name).toBe('326740-Food-Primary.pdf');
  api.assertNoUnmatched();
});

test('licence detail: certificate download shows a toast and records an audit entry', async ({ page }) => {
  const api = await preparePage(page, { role: 'licensee' });
  await page.goto('/licences/326739');
  const name = await downloadName(page, () => page.getByRole('button', { name: 'Download Certificate' }).click());
  expect(name).toBe('326739-Liquor-Primary.pdf');
  await expect(page.getByText('Certificate downloaded')).toBeVisible();
  await expect(page.getByText('Liquor Primary — 326739')).toBeVisible();
  await expectStorage<unknown>(page, 'lcrb.v2.audit', (v) => JSON.stringify(v ?? '').includes('licence.certificate-downloaded'));
  api.assertNoUnmatched();
});

test('licence detail tabs open from ?tab= and switch on click', async ({ page }) => {
  const api = await preparePage(page, { role: 'licensee' });
  await page.goto('/licences/326739?tab=terms');
  await expect(page.getByRole('tab', { name: 'Terms & Conditions' })).toHaveAttribute('aria-selected', 'true');
  await page.getByRole('tab', { name: /Inspections/ }).click();
  await expect(page.getByRole('tab', { name: /Inspections/ })).toHaveAttribute('aria-selected', 'true');
  await expect(page.getByRole('tabpanel')).toBeVisible();
  await page.getByRole('tab', { name: 'Overview' }).click();
  await expect(page.getByRole('tab', { name: 'Overview' })).toHaveAttribute('aria-selected', 'true');

  await page.goto('/licences/326739?tab=inspections');
  await expect(page.getByRole('tab', { name: /Inspections/ })).toHaveAttribute('aria-selected', 'true');
  api.assertNoUnmatched();
});
