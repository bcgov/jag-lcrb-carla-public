/** Flow 6: local government records a comment or requests documents. */
import { expect, test } from '@playwright/test';
import { PORTED_FLOWS } from '../../support/ported';
import { preparePage, skipUnlessPorted } from '../../support/parity';
import { expectStorage } from '../../support/flow-helpers';

interface LgRecord {
  jobNumber: string;
  status: string;
  history: { label: string; actor?: string }[];
}

const REVIEW = '/local-government/review/LP-125998';

test.describe('flow 6: local government decision', () => {
  test.beforeEach(async ({}, testInfo) => skipUnlessPorted(testInfo, PORTED_FLOWS, 'flow-06'));

  test('no comment / no objection', async ({ page }) => {
    const api = await preparePage(page, { role: 'local-government' });
    await page.goto(REVIEW);
    await expect(page.getByText('Reviewing as Priya Sandhu')).toBeVisible();
    await page.getByLabel('Comments to LCRB').fill('No concerns');
    await page.getByRole('button', { name: 'No comment / No objection' }).click();

    await expect(page.getByText('Comment recorded: No Comment.')).toBeVisible();
    await expectStorage<LgRecord[]>(page, 'lcrb.lg.applications', (records) => {
      const r = records?.find((x) => x.jobNumber === 'LP-125998');
      return r?.status === 'no-comment' && r.history.some((h) => h.label.includes('No concerns') && h.actor === 'Priya Sandhu');
    });
    api.assertNoUnmatched();
  });

  test('request documents', async ({ page }) => {
    const api = await preparePage(page, { role: 'local-government' });
    await page.goto(REVIEW);
    await page.getByLabel('Request documents from applicant').fill('Please provide the updated floor plan.');
    await page.getByRole('button', { name: 'Request documents' }).click();

    await expect(page.getByText('Documents requested from the applicant.')).toBeVisible();
    await expectStorage<LgRecord[]>(page, 'lcrb.lg.applications', (records) =>
      records?.find((x) => x.jobNumber === 'LP-125998')?.status === 'documents-requested',
    );
    api.assertNoUnmatched();
  });
});
