/** Flow 2: licence application from the catalogue to the confirmation screen. /api/assessment is answered by a fixture. */
import { expect, test } from '@playwright/test';
import { PORTED_FLOWS } from '../../support/ported';
import { preparePage, skipUnlessPorted } from '../../support/parity';
import { expectStorage } from '../../support/flow-helpers';
import {
  confirmAndSubmit,
  expectSubmitted,
  fillContactStep,
  fillEstablishmentStep,
  fillNameChangeStep,
  NAME_CHANGE_PATH,
  NAME_CHANGE_TITLE,
  next,
} from '../../support/name-change';

interface Submission {
  flowId: string;
  title: string;
  total: number;
  paymentMethod: string;
  confirmationNumber: string;
}

test('flow 2: licence application, catalogue to confirmation', async ({ page }, testInfo) => {
  skipUnlessPorted(testInfo, PORTED_FLOWS, 'flow-02');
  const api = await preparePage(page, { role: 'licensee' });

  // Catalogue: search, then open the Food Primary variant of the name change.
  await page.goto('/applications/new');
  await expect(page.getByRole('heading', { name: 'Start a New Application or Authorization' })).toBeVisible();
  await page.locator('#application-search').fill('name change');
  await expect(page.getByText(NAME_CHANGE_TITLE).first()).toBeVisible();
  await page.locator(`a[href="${NAME_CHANGE_PATH}"]`).first().click();
  await expect(page).toHaveURL(new RegExp(`${NAME_CHANGE_PATH}$`));

  await fillEstablishmentStep(page);
  await next(page);
  await fillNameChangeStep(page);
  await next(page);
  await fillContactStep(page);
  await next(page);
  await expect(page.getByText('Review your application').first()).toBeVisible();
  await confirmAndSubmit(page, 'Submit Application');

  await expectSubmitted(page);
  await expect(page.getByText('Confirmation number', { exact: true })).toBeVisible();
  await expectStorage<Record<string, Submission>>(page, 'lcrb.app.submissions', (subs) =>
    Object.values(subs ?? {}).some(
      (s) =>
        s.flowId === 'fp-name-change' &&
        s.title === NAME_CHANGE_TITLE &&
        s.total === 0 &&
        s.paymentMethod === 'No payment required' &&
        /^PAY-\d{4}-[A-Z0-9]{6}$/.test(s.confirmationNumber),
    ),
  );
  expect(api.calls['/api/assessment']).toBe(1);
  api.assertNoUnmatched();
});
