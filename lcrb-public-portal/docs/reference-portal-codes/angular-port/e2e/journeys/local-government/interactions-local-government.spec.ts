/** Interaction checks: local government queue filters, sort and downloads, and the "Recommend rejection" decision. */
import { expect, test, type Page } from '@playwright/test';
import { preparePage } from '../../support/parity';
import { desktopOnly, downloadName, expectStorage } from '../../support/flow-helpers';

test.beforeEach(({}, testInfo) => desktopOnly(testInfo));

/** Job numbers of the desktop table rows, in display order. */
const jobs = async (page: Page) =>
  (await page.locator('table tbody tr').allInnerTexts()).map((t) => t.match(/Job #(\S+)/)?.[1] ?? '');

test('queue: search, status filter, column sort and "Clear all"', async ({ page }) => {
  const api = await preparePage(page, { role: 'local-government' });
  await page.goto('/local-government/queue');
  const all = await jobs(page);
  expect(all.length).toBeGreaterThan(2);

  await page.getByPlaceholder('Search by job #, applicant, business, or establishment...').fill('LP-125998');
  await expect.poll(() => jobs(page)).toEqual(['LP-125998']);
  await expect(page.getByText('Showing 1 application', { exact: true })).toBeVisible();
  await page.getByRole('button', { name: 'Clear all' }).click();
  await expect.poll(async () => (await jobs(page)).length).toBe(all.length);

  await page.getByRole('combobox').first().click(); // the status select has no accessible name
  await page.getByRole('option', { name: 'Documents Requested' }).click();
  await expect.poll(() => jobs(page)).toContain('142536');
  expect(await jobs(page)).not.toContain('LP-125998');
  await page.getByRole('button', { name: 'Clear all' }).click();
  await expect(page.getByRole('button', { name: 'Clear all' })).toHaveCount(0);

  // Applicant A–Z, then Z–A (orders recorded from the reference).
  const ascending = ['938362', 'LP-125998', '089817', '305834', '560021', '142536'];
  await page.getByRole('button', { name: /Applicant \/ Business/ }).click();
  await expect.poll(() => jobs(page)).toEqual(ascending);
  await page.getByRole('button', { name: /Applicant \/ Business/ }).click();
  await expect.poll(() => jobs(page)).toEqual([...ascending].reverse());
  api.assertNoUnmatched();
});

test('queue: download the deadlines calendar and the PDF list', async ({ page }) => {
  const api = await preparePage(page, { role: 'local-government' });
  await page.goto('/local-government/queue');
  expect(await downloadName(page, () => page.getByRole('button', { name: 'Download calendar (.ics)' }).click())).toBe(
    'lcrb-review-deadlines.ics',
  );
  expect(await downloadName(page, () => page.getByRole('button', { name: 'Download PDF list' }).click())).toMatch(
    /^LG-review-due-dates-\d{4}-\d{2}-\d{2}\.pdf$/,
  );
  api.assertNoUnmatched();
});

test('review: recommend rejection', async ({ page }) => {
  const api = await preparePage(page, { role: 'local-government' });
  await page.goto('/local-government/review/LP-125998');
  await page.getByLabel('Comments to LCRB').fill('Does not fit the neighbourhood plan');
  await page.getByRole('button', { name: 'Recommend rejection' }).click();
  await expect(page.getByText('Comment recorded: Reject.')).toBeVisible();
  await expectStorage<{ jobNumber: string; status: string }[]>(page, 'lcrb.lg.applications', (records) =>
    records?.find((x) => x.jobNumber === 'LP-125998')?.status === 'reject',
  );
  api.assertNoUnmatched();
});
