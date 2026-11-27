import { expect, test } from '@playwright/test';

test.beforeEach(async ({ page }) => {
  await page.goto('/login');
  await page.evaluate(() => localStorage.clear());
});

test('login shows the role picker for all five roles', async ({ page }) => {
  await page.goto('/login');
  await expect(page.getByRole('heading', { level: 1, name: 'Sign in to Liquor and Cannabis Licensing' })).toBeVisible();
  for (const role of ['Licensee', 'SEP Applicant', 'Police', 'Local Government', 'LCRB Staff']) {
    await expect(page.getByRole('button', { name: new RegExp(`Sign in as ${role}`) })).toBeVisible();
  }
});

test('signing in as a licensee lands on the dashboard', async ({ page }) => {
  await page.goto('/login');
  await page.getByRole('button', { name: /Sign in as Licensee/ }).click();
  await page.getByRole('button', { name: 'Sign in', exact: true }).click();
  await expect(page).toHaveURL(/\/dashboard$/);
  await expect(page.getByText('Welcome to the LCRB Portal')).toBeVisible();
  await expect(page.getByText('What would you like to do today?')).toBeVisible();
  await expect(page.getByText('Start a new application')).toBeVisible();
  await expect(page.getByText('Recent Activity')).toBeVisible();

  await page.reload();
  await expect(page.getByText('Welcome to the LCRB Portal')).toBeVisible();
});

test('an unbuilt or unknown route shows Not Found', async ({ page }) => {
  for (const url of ['/no-such-page', '/licences']) {
    await page.goto(url);
    await expect(page.getByRole('heading', { name: "We couldn't find that page" })).toBeVisible();
  }
});

test('the notification list scrolls within its height', async ({ page }) => {
  await page.goto('/login');
  await page.getByRole('button', { name: /Sign in as Licensee/ }).click();
  await page.getByRole('button', { name: 'Sign in', exact: true }).click();
  await page.getByRole('button', { name: /^Notifications/ }).click();
  const viewport = page.locator('[data-slot=scroll-area-viewport]');
  await expect(viewport).toBeVisible();
  const box = await viewport.evaluate((el) => ({ client: el.clientHeight, scroll: el.scrollHeight }));
  expect(box.client).toBeLessThanOrEqual(384);
  expect(box.scroll).toBeGreaterThan(box.client);
});
