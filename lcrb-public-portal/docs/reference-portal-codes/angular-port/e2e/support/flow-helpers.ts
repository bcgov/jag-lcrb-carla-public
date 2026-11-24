/** Shared helpers for the functional flow suite. */
import { expect, test, type Page, type TestInfo } from '@playwright/test';

/** Interaction checks run at the desktop viewport only; the visual suite covers mobile layout. */
export function desktopOnly(testInfo: TestInfo): void {
  test.skip(testInfo.project.metadata['size'] !== 'desktop', 'Interaction checks run at desktop only');
}

/** Click `trigger` and return the suggested file name of the download it starts. */
export async function downloadName(page: Page, trigger: () => Promise<unknown>): Promise<string> {
  const [download] = await Promise.all([page.waitForEvent('download'), trigger()]);
  return download.suggestedFilename();
}

/** Read and parse a localStorage key in the page. */
export async function readStorage<T = unknown>(page: Page, key: string): Promise<T | null> {
  return page.evaluate((k) => {
    const raw = window.localStorage.getItem(k);
    return raw === null ? null : JSON.parse(raw);
  }, key);
}

/** Poll a localStorage key until the predicate holds (stores persist in effects, after the UI updates). */
export async function expectStorage<T>(page: Page, key: string, predicate: (value: T | null) => boolean): Promise<T> {
  let last: T | null = null;
  await expect
    .poll(async () => {
      last = await readStorage<T>(page, key);
      return predicate(last);
    })
    .toBe(true);
  return last as T;
}

/** Pick an option in a Radix select (combobox) by its accessible name. */
export async function chooseOption(page: Page, combobox: string, option: string | RegExp): Promise<void> {
  await page.getByRole('combobox', { name: combobox }).click();
  await page.getByRole('option', { name: option }).click();
}
