/** Interaction checks: licensee notices, receipts, calendar export, enforcement response and case messages. */
import { expect, test } from '@playwright/test';
import { preparePage } from '../../support/parity';
import { desktopOnly, downloadName, expectStorage } from '../../support/flow-helpers';

test.beforeEach(({}, testInfo) => desktopOnly(testInfo));

test('notices: filter chips and "Mark all as read"', async ({ page }) => {
  const api = await preparePage(page, { role: 'licensee' });
  await page.goto('/notices');
  const chips = page.getByRole('group', { name: 'Filter notices' });
  const unreadDots = page.getByLabel('Unread', { exact: true });
  await expect.poll(() => unreadDots.count()).toBeGreaterThan(0);
  const unread = await unreadDots.count();

  await chips.getByRole('button', { name: `Unread (${unread})` }).click();
  await expect(chips.getByRole('button', { name: `Unread (${unread})` })).toHaveAttribute('aria-pressed', 'true');
  await chips.getByRole('button', { name: /^All \(/ }).click();

  await page.getByRole('button', { name: 'Mark all as read' }).first().click();
  await expect(unreadDots).toHaveCount(0);
  await expect(chips.getByRole('button', { name: 'Unread (0)' })).toBeVisible();
  await expect(page.getByText(/^\d+ unread$/)).toHaveCount(0);
  api.assertNoUnmatched();
});

test('notices: read the full notice and download it as PDF', async ({ page }) => {
  const api = await preparePage(page, { role: 'licensee' });
  await page.goto('/notices');
  await page.getByRole('button', { name: 'Read full notice' }).first().click();
  const dialog = page.getByRole('dialog');
  await expect(dialog).toBeVisible();
  const name = await downloadName(page, () => dialog.getByRole('button', { name: 'Download PDF' }).click());
  expect(name).toMatch(/^LCRB-Notice-[A-Za-z0-9-]+\.pdf$/);
  api.assertNoUnmatched();
});

test('receipts: download a receipt PDF and email it (logged, no visible toast)', async ({ page }) => {
  const api = await preparePage(page, { role: 'licensee' });
  await page.goto('/receipts');
  const download = page.getByRole('button', { name: /^Download PDF RCPT-/ }).first();
  const receiptNumber = ((await download.getAttribute('aria-label')) ?? '').replace('Download PDF ', '');
  expect(await downloadName(page, () => download.click())).toBe(`LCRB-Receipt-${receiptNumber}.pdf`);

  await page.getByRole('button', { name: /^(Email receipt|Resend)$/ }).first().click();
  await expectStorage<unknown>(page, 'lcrb.v2.notification-delivery', (v) => JSON.stringify(v ?? '').includes('LCRB-Receipt-'));
  api.assertNoUnmatched();
});

test('calendar: filter chips, agenda and month views, and the .ics export', async ({ page }) => {
  const api = await preparePage(page, { role: 'licensee' });
  await page.goto('/calendar');
  const chips = page.getByRole('group', { name: 'Filter events by type' });
  await expect(chips.getByRole('button', { name: /^All \(\d+\)$/ })).toHaveAttribute('aria-pressed', 'true');
  await chips.getByRole('button', { name: /^Fees \(\d+\)$/ }).click();
  await expect(chips.getByRole('button', { name: /^Fees \(\d+\)$/ })).toHaveAttribute('aria-pressed', 'true');

  const views = page.getByRole('group', { name: 'Calendar view' });
  await views.getByRole('button', { name: 'Month' }).click();
  await expect(views.getByRole('button', { name: 'Month' })).toHaveAttribute('aria-pressed', 'true');
  await views.getByRole('button', { name: 'Agenda' }).click();
  await expect(views.getByRole('button', { name: 'Agenda' })).toHaveAttribute('aria-pressed', 'true');

  await page.getByRole('button', { name: 'Export to calendar (.ics)' }).click();
  const dialog = page.getByRole('dialog', { name: 'Export to your calendar' });
  await dialog.getByRole('radio', { name: 'Next 30 days' }).click();
  const name = await downloadName(page, () => dialog.getByRole('button', { name: 'Download .ics' }).click());
  expect(name).toMatch(/^lcrb-compliance-calendar-2026-11-02_to_\d{4}-\d{2}-\d{2}\.ics$/);
  await expect(dialog).toBeHidden();
  api.assertNoUnmatched();
});

test('compliance: respond to an enforcement action with a written response', async ({ page }) => {
  const api = await preparePage(page, { role: 'licensee' });
  await page.goto('/compliance');
  await page.getByRole('button', { name: 'Respond to this action' }).first().click();
  const dialog = page.getByRole('dialog', { name: /^Respond to / });
  await dialog.getByText('Submit a written response').click();
  const submit = dialog.getByRole('button', { name: 'Submit response' });
  await dialog.getByLabel(/Your statement/).fill('Too short');
  await expect(submit).toBeDisabled();
  await dialog.getByLabel(/Your statement/).fill('We have corrected the signage issue.');
  await expect(submit).toBeEnabled();
  await submit.click();

  await expect(page.getByText('Response submitted').first()).toBeVisible();
  await expect(page.getByText('We have corrected the signage issue.').first()).toBeVisible();
  await expectStorage<unknown>(page, 'lcrb.v2.enforcement-responses', (v) =>
    JSON.stringify(v ?? '').includes('We have corrected the signage issue.'),
  );
  api.assertNoUnmatched();
});

test('case conversation: send a message on a submitted application', async ({ page }) => {
  const api = await preparePage(page, { role: 'licensee' });
  await page.goto('/applications/submitted/LP-125998');
  const message = page.getByRole('textbox', { name: 'Message' });
  await message.fill('Can you confirm the inspection date?');
  await page.getByRole('button', { name: 'Send message' }).click();
  await expect(message).toHaveValue('');
  await expect(page.getByText('Can you confirm the inspection date?')).toBeVisible();
  await expectStorage<unknown>(page, 'lcrb.v2.case-chat', (v) => JSON.stringify(v ?? '').includes('Can you confirm the inspection date?'));
  api.assertNoUnmatched();
});
