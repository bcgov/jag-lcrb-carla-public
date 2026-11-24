/** Interaction checks: police queue filters and assignment, the daily report, scheduled report delivery and the deny decision. */
import { expect, test, type Page } from '@playwright/test';
import { preparePage } from '../../support/parity';
import { desktopOnly, downloadName, expectStorage } from '../../support/flow-helpers';

test.beforeEach(({}, testInfo) => desktopOnly(testInfo));

interface SepRecord {
  id: string;
  status: string;
  assignedOfficer?: string;
  history: { action: string; actor: string; note?: string }[];
}

/** Event names of the queue rows, in display order. */
const queueNames = async (page: Page) =>
  (await page.locator('main a[href^="/police/review/"]').allInnerTexts()).map((t) => t.trim()).filter((t) => t !== 'Review');

test('queue: search, view filters, "Assign to me" and sort', async ({ page }) => {
  const api = await preparePage(page, { role: 'police' });
  await page.goto('/police/queue');
  const view = page.getByRole('combobox').nth(0);
  const sort = page.getByRole('combobox').nth(1);

  await page.getByPlaceholder('Search by event name, applicant, venue, address, or permit #...').fill('SEP-2026-04812');
  await expect.poll(() => queueNames(page)).toEqual(['Harbour City Music Festival']);
  await page.getByPlaceholder('Search by event name, applicant, venue, address, or permit #...').fill('');

  await view.click();
  await page.getByRole('option', { name: 'Unassigned' }).click();
  await expect(page.getByRole('link', { name: 'Harbour City Music Festival' })).toBeVisible();
  await page.getByRole('button', { name: 'Assign to me' }).first().click();
  await expectStorage<SepRecord[]>(page, 'lcrb.sep.records', (rs) =>
    Boolean(rs?.some((r) => r.assignedOfficer === 'Cst. Dana Okafor')),
  );

  await view.click();
  await page.getByRole('option', { name: 'My jobs' }).click();
  await expect.poll(async () => (await queueNames(page)).length).toBeGreaterThan(0);

  await view.click();
  await page.getByRole('option', { name: 'All applications' }).click();
  await sort.click();
  await page.getByRole('option', { name: 'Event name (A–Z)' }).click();
  await expect.poll(async () => {
    const names = await queueNames(page);
    return names.length > 1 && names.join('|') === [...names].sort((a, b) => a.localeCompare(b)).join('|');
  }).toBe(true);
  api.assertNoUnmatched();
});

test('events: the daily permit report counts the day and downloads its PDF', async ({ page }) => {
  const api = await preparePage(page, { role: 'police' });
  await page.goto('/police/events');
  await expect(page.getByText('Pick a day to export all its Special Event Permits.')).toBeVisible();
  await page.getByLabel('Daily permit report (PDF)').fill('2026-11-20');
  await expect(page.getByText(/^\d+ permits? on this day$/)).toBeVisible();
  const name = await downloadName(page, () => page.getByRole('button', { name: 'Download PDF' }).click());
  expect(name).toBe('SEP-daily-report-2026-11-20.pdf');
  api.assertNoUnmatched();
});

test('events: schedule a weekly report, send it now, pause, resume and remove it', async ({ page }) => {
  const api = await preparePage(page, { role: 'police' });
  await page.goto('/police/events');
  await page.locator('#cadence').click();
  await page.getByRole('option', { name: 'Weekly — the next 7 days' }).click();
  await page.getByRole('button', { name: 'Schedule delivery' }).click();
  await expect(page.getByText('Weekly report', { exact: true })).toBeVisible();
  await expectStorage<unknown>(page, 'lcrb.v2.sep-report-schedules', (v) => JSON.stringify(v ?? '').includes('"weekly"'));

  const name = await downloadName(page, () => page.getByRole('button', { name: 'Send now' }).click());
  expect(name).toMatch(/^SEP-report-\d{4}-\d{2}-\d{2}_to_\d{4}-\d{2}-\d{2}\.pdf$/);
  await expect(page.getByText(/Last sent .+\(\d+ permits?\)/)).toBeVisible();

  await page.getByRole('switch', { name: 'Pause schedule' }).click();
  await expect(page.getByRole('switch', { name: 'Resume schedule' })).toBeVisible();
  await page.getByRole('switch', { name: 'Resume schedule' }).click();
  await expect(page.getByRole('switch', { name: 'Pause schedule' })).toBeVisible();

  await page.getByRole('button', { name: 'Delete schedule' }).click();
  await page.getByRole('alertdialog', { name: 'Remove this scheduled report?' }).getByRole('button', { name: 'Remove' }).click();
  await expect(page.getByText('Weekly report', { exact: true })).toHaveCount(0);
  api.assertNoUnmatched();
});

test('review: deny a permit with a reason', async ({ page }) => {
  const api = await preparePage(page, { role: 'police' });
  await page.goto('/police/review/SEP-2026-04812');
  await page.getByRole('button', { name: 'Assign to me' }).first().click();
  await page.getByRole('button', { name: 'Deny permit' }).click();
  const dialog = page.getByRole('dialog', { name: 'Deny permit' });
  await expect(dialog.getByRole('button', { name: 'Confirm denial' })).toBeDisabled();
  await dialog.getByPlaceholder('Reason for denial (required)…').fill('Insufficient security plan');
  await dialog.getByRole('button', { name: 'Confirm denial' }).click();

  await expect(page.getByText('This permit is denied. No further action is required.')).toBeVisible();
  await expectStorage<SepRecord[]>(page, 'lcrb.sep.records', (records) => {
    const r = records?.find((x) => x.id === 'SEP-2026-04812');
    return r?.status === 'denied' && r.history.some((h) => h.action === 'Denied' && h.note === 'Insufficient security plan');
  });
  api.assertNoUnmatched();
});
