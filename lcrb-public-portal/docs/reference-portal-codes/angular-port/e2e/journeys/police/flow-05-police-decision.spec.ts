/** Flow 5: police officer assigns and approves a permit. */
import { expect, test } from '@playwright/test';
import { PORTED_FLOWS } from '../../support/ported';
import { preparePage, skipUnlessPorted } from '../../support/parity';
import { expectStorage } from '../../support/flow-helpers';

interface SepRecord {
  id: string;
  status: string;
  history: { action: string; actor: string; note?: string }[];
}

test('flow 5: police decision', async ({ page }, testInfo) => {
  skipUnlessPorted(testInfo, PORTED_FLOWS, 'flow-05');
  const api = await preparePage(page, { role: 'police' });
  await page.goto('/police/review/SEP-2026-04812');

  await page.getByRole('button', { name: 'Assign to me' }).first().click();
  await page.getByRole('button', { name: 'Approve permit' }).click();
  const dialog = page.getByRole('dialog', { name: 'Approve permit' });
  await dialog.getByPlaceholder('Optional note…').fill('Approved with standard conditions');
  await dialog.getByRole('button', { name: 'Confirm approval' }).click();

  await expect(page.getByText('This permit is approved. No further action is required.')).toBeVisible();
  await expectStorage<SepRecord[]>(page, 'lcrb.sep.records', (records) => {
    const r = records?.find((x) => x.id === 'SEP-2026-04812');
    return (
      r?.status === 'approved' &&
      r.history.some((h) => h.action === 'Approved' && h.actor === 'Cst. Dana Okafor' && h.note === 'Approved with standard conditions')
    );
  });
  api.assertNoUnmatched();
});
