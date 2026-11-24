/** Shared steps for the "Liquor Establishment Name Change Application" (fp-name-change), the shortest application flow. Used by flows 2, 3 and 7. */
import { expect, type Page } from '@playwright/test';
import { chooseOption } from './flow-helpers';

export const NAME_CHANGE_PATH = '/applications/new/fp-name-change';
export const NAME_CHANGE_TITLE = 'Liquor Establishment Name Change Application';

export async function fillEstablishmentStep(page: Page): Promise<void> {
  await chooseOption(page, 'Licence Number', /326740/);
  const name = page.getByLabel('Establishment Name', { exact: false }).first();
  if (!(await name.inputValue())) await name.fill('Restaurant ABC');
  await page.getByLabel('Street Address').fill('456 Government St');
  await page.getByLabel('City / Town').fill('Victoria');
  await chooseOption(page, 'Province', 'British Columbia');
  await page.getByLabel('Postal Code').fill('V8V 2L4');
}

export async function fillNameChangeStep(page: Page): Promise<void> {
  await page.getByLabel('Current establishment name').fill('Restaurant ABC');
  await page.getByLabel('Proposed establishment name').fill('Restaurant XYZ');
}

export async function fillContactStep(page: Page): Promise<void> {
  await page.getByLabel('Contact Name').fill('Matt Harding');
  await page.getByLabel('Phone Number').fill('250-555-1234');
  await page.getByLabel('Email').fill('matt@example.ca');
}

export async function next(page: Page): Promise<void> {
  await page.getByRole('button', { name: 'Next', exact: true }).click();
}

export async function confirmAndSubmit(page: Page, submitName: string): Promise<void> {
  await page.getByLabel('I confirm that all information provided is accurate and complete.').check();
  await page.getByLabel('I understand that providing false information may result in refusal or revocation.').check();
  await page.getByRole('button', { name: submitName }).click();
}

export async function expectSubmitted(page: Page): Promise<void> {
  await expect(page).toHaveURL(/\/applications\/confirmation\//);
  await expect(page.getByRole('heading', { name: 'Application submitted', level: 1 })).toBeVisible();
}
