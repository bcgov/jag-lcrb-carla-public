/** Flow 3: draft save and resume, and Guided Mode (one question at a time) through to submission. */
import { expect, test, type Page } from '@playwright/test';
import { PORTED_FLOWS } from '../../support/ported';
import { preparePage, skipUnlessPorted } from '../../support/parity';
import { chooseOption, expectStorage, readStorage } from '../../support/flow-helpers';
import { confirmAndSubmit, expectSubmitted, NAME_CHANGE_PATH } from '../../support/name-change';

interface Draft {
  flowId: string;
  resumeHref: string;
  formData: Record<string, unknown>;
}

const DRAFT_KEY = 'draft:fp-name-change';

/** Answers for the guided questions, keyed by the question heading. Optional questions are skipped. */
const GUIDED_ANSWERS: Record<string, (page: Page) => Promise<void>> = {
  'Licence Number': (page) => chooseOption(page, 'Licence Number', /326740/),
  'Establishment Name': async (page) => {
    const input = page.getByLabel('Establishment Name').first();
    if (!(await input.inputValue())) await input.fill('Restaurant ABC');
  },
  'Street Address': (page) => page.getByLabel('Street Address').fill('456 Government St'),
  'City / Town': (page) => page.getByLabel('City / Town').fill('Victoria'),
  Province: (page) => chooseOption(page, 'Province', 'British Columbia'),
  'Postal Code': (page) => page.getByLabel('Postal Code').fill('V8V 2L4'),
  'Current establishment name': (page) => page.getByLabel('Current establishment name').fill('Restaurant ABC'),
  'Proposed establishment name': (page) => page.getByLabel('Proposed establishment name').fill('Restaurant XYZ'),
  'Contact Name': (page) => page.getByLabel('Contact Name').fill('Matt Harding'),
  'Phone Number': (page) => page.getByLabel('Phone Number').fill('250-555-1234'),
  Email: (page) => page.getByLabel('Email').fill('matt@example.ca'),
};

test.describe('flow 3: drafts and Guided Mode', () => {
  test.beforeEach(async ({}, testInfo) => skipUnlessPorted(testInfo, PORTED_FLOWS, 'flow-03'));

  test('draft is saved and resumed', async ({ page }) => {
    const api = await preparePage(page, { role: 'licensee' });
    await page.goto(NAME_CHANGE_PATH);
    await chooseOption(page, 'Licence Number', /326740/);
    await page.getByLabel('Street Address').fill('456 Government St');

    // The silent save is debounced (600 ms).
    await expectStorage<Record<string, Draft>>(
      page,
      'lcrb.app.drafts',
      (drafts) => drafts?.[DRAFT_KEY]?.formData?.['addressLine1'] === '456 Government St',
    );
    const draft = (await readStorage<Record<string, Draft>>(page, 'lcrb.app.drafts'))![DRAFT_KEY];
    expect(draft.flowId).toBe('fp-name-change');
    expect(draft.resumeHref).toBe(NAME_CHANGE_PATH);

    await page.reload();
    const dialog = page.getByRole('alertdialog', { name: 'Continue where you left off?' });
    await expect(dialog).toBeVisible();
    await dialog.getByRole('button', { name: 'Resume draft' }).click();
    await expect(page.getByLabel('Street Address')).toHaveValue('456 Government St');
    api.assertNoUnmatched();
  });

  test('Guided Mode answers one question at a time and submits', async ({ page }) => {
    const api = await preparePage(page, { role: 'licensee' });
    await page.goto(NAME_CHANGE_PATH);
    await page.getByRole('radio', { name: 'Guided' }).click();
    await expect.poll(() => page.evaluate(() => localStorage.getItem('lcrb.app.mode'))).toBe('guided');
    await expect(page.getByText(/Question 1 of \d+/).first()).toBeVisible();

    for (let i = 0; i < 25; i++) {
      const heading = (await page.getByRole('heading', { level: 1 }).first().innerText()).trim();
      if (heading === 'Review your application') break;
      const answer = Object.entries(GUIDED_ANSWERS).find(([label]) => heading.startsWith(label))?.[1];
      if (answer) await answer(page);
      await page.getByRole('button', { name: /^(Continue|Review)$/ }).click();
      await expect(page.getByRole('heading', { level: 1 }).first()).not.toHaveText(heading);
    }

    await expect(page.getByRole('heading', { name: 'Review your application', level: 1 })).toBeVisible();
    await confirmAndSubmit(page, 'Submit application');
    await expectSubmitted(page);

    // Switching back to the full form is remembered.
    await page.goto(NAME_CHANGE_PATH);
    // In Guided Mode the toggle is a plain "Full form" button in the guided toolbar.
    await page.getByRole('button', { name: 'Full form', exact: true }).click();
    await expect.poll(() => page.evaluate(() => localStorage.getItem('lcrb.app.mode'))).toBe('form');
    api.assertNoUnmatched();
  });
});
