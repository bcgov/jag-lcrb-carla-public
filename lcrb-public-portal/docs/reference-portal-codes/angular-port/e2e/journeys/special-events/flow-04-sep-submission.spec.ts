/** Flow 4: SEP submission, auto-approved when the police policy is on, referred when off. Small private event, so no referral trigger. */
import { expect, test, type Page } from '@playwright/test';
import { PORTED_FLOWS } from '../../support/ported';
import { preparePage, skipUnlessPorted } from '../../support/parity';
import { chooseOption, expectStorage } from '../../support/flow-helpers';

interface SepRecord {
  id: string;
  status: string;
  conditions: unknown[];
  history: { action: string }[];
}

const EVENT_DATE = '2026-11-22';

const POLICY_ON = {
  'lcrb.v2.sep-auto-approval': JSON.stringify({
    policy: { id: 'policy', enabled: true, minNoticeDays: 14, updatedAt: null, updatedBy: null, agency: null },
  }),
};

async function next(page: Page): Promise<void> {
  await page.getByRole('button', { name: 'Next', exact: true }).click();
}

async function fillAndSubmit(page: Page): Promise<void> {
  // Step 0: terms
  await page.getByLabel('Event Name').fill('Summer BBQ');
  await page.getByLabel('I have read and agree to these terms and conditions').check();
  await next(page);

  // Step 1: eligibility
  await page.locator('#isPrivateResidence-no').click();
  await page.locator('#isPublicProperty-no').click();
  await page.locator('#isSignificantEvent-no').click();
  await chooseOption(page, 'Is your event private or public?', 'Private - Invited guests only');
  await page.getByLabel('When does your event start?').fill(EVENT_DATE);
  await page.locator('#hasCertificate-no').click();
  await page.getByLabel("What's the occasion of your event? (maximum 255 characters)").fill('Community picnic');
  await page.locator('#hasAdmissionCharge-no').click();
  await next(page);

  // Step 2: location
  await page.getByLabel('Special event city').fill('Victoria');
  await page.locator('#isAnnualEvent-no').click();
  await page.getByLabel('Event Location Name').fill('Beacon Hill Park');
  await page.getByLabel('Venue Type').fill('park');
  await page.getByLabel('Address Line 1').fill('100 Cook St');
  await page.getByLabel('Postal Code').fill('V8V 3W6');
  await page.getByLabel('Maximum number of guests at this location at any time').fill('50');
  await page.getByLabel('Date of your event').fill(EVENT_DATE);
  await page.getByLabel('Event starts').fill('12:00');
  await page.getByLabel('Event ends').fill('20:00');
  await next(page);

  // Step 3: liquor
  await page.getByLabel('Beer', { exact: true }).check();
  await page.locator('#liquorSaleType-selling').click();
  await page.getByLabel('Estimated total number of standard drinks to be served').fill('200');
  await page.locator('#hasSecurity-no').click();
  await next(page);

  // Step 4: summary and declarations
  await page.getByLabel('I confirm that all information provided is accurate and complete').check();
  await page.getByLabel('I understand that providing false information may result in permit denial or revocation').check();
  await page.getByRole('button', { name: 'Submit Application' }).click();
  await expect(page).toHaveURL(/\/special-events\?submitted=SEP-\d{4}-\d+/);
}

function submittedId(page: Page): string {
  return new URL(page.url()).searchParams.get('submitted')!;
}

test.describe('flow 4: SEP submission and auto-approval', () => {
  test.beforeEach(async ({}, testInfo) => skipUnlessPorted(testInfo, PORTED_FLOWS, 'flow-04'));

  test('auto-approved when the police policy is on', async ({ page }) => {
    const api = await preparePage(page, { role: 'applicant', storage: POLICY_ON });
    await page.goto('/special-events/apply');
    await fillAndSubmit(page);
    const id = submittedId(page);

    await expect(page.getByRole('status').filter({ hasText: `Application ${id} approved instantly` })).toBeVisible();
    await expectStorage<SepRecord[]>(page, 'lcrb.sep.records', (records) => {
      const r = records?.find((x) => x.id === id);
      return r?.status === 'auto-approved' && r.conditions.length === 5 && r.history.some((h) => h.action === 'Auto-approved');
    });
    api.assertNoUnmatched();
  });

  test('referred for review when the policy is off', async ({ page }) => {
    const api = await preparePage(page, { role: 'applicant' });
    await page.goto('/special-events/apply');
    await fillAndSubmit(page);
    const id = submittedId(page);

    await expect(page.getByRole('status').filter({ hasText: `Application ${id} submitted for review` })).toBeVisible();
    await expect(page.getByText('Auto-approval is not enabled by the police agency for this jurisdiction')).toBeVisible();
    await expectStorage<SepRecord[]>(page, 'lcrb.sep.records', (records) => {
      const r = records?.find((x) => x.id === id);
      return r?.status === 'submitted' && r.history.some((h) => h.action === 'Referred for review');
    });
    api.assertNoUnmatched();
  });
});
