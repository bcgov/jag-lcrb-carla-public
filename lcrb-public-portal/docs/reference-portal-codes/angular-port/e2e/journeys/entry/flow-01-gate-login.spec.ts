/** Flow 1: access gate, login and role routing, on the gated app (ACCESS_CODE = GATE_TEST_CODE, a dummy value). */
import { expect, test } from '@playwright/test';
import { PORTED_FLOWS } from '../../support/ported';
import { gatedUrlOf, preparePage, SESSION_KEY, skipUnlessPorted } from '../../support/parity';
import { readStorage } from '../../support/flow-helpers';

const GATE_TEST_CODE = process.env['GATE_TEST_CODE'] ?? 'parity-gate-code';

const ROLES = [
  { card: 'Sign in as Licensee', role: 'licensee', home: '/dashboard', heading: 'Welcome to the LCRB Portal' },
  { card: 'Sign in as SEP Applicant', role: 'applicant', home: '/special-events', heading: 'Special Event Permits' },
  { card: 'Sign in as Police', role: 'police', home: '/police', heading: 'Special Event Permit Review' },
  { card: 'Sign in as Local Government', role: 'local-government', home: '/local-government', heading: 'Local Government Review' },
  { card: 'Sign in as LCRB Staff', role: 'lcrb', home: '/lcrb', heading: 'Intake Overview' },
];

test('flow 1: gate, login and role routing', async ({ page, context }, testInfo) => {
  skipUnlessPorted(testInfo, PORTED_FLOWS, 'flow-01');
  const api = await preparePage(page);
  const base = gatedUrlOf(testInfo);


  // No cookie: redirected to the gate.
  await page.goto(`${base}/`);
  await expect(page).toHaveURL(/\/gate/);
  await expect(page.getByRole('heading', { name: 'Restricted access' })).toBeVisible();

  // Wrong code: error shown, still gated.
  await page.getByLabel('Access code').fill('not-the-code');
  await page.getByRole('button', { name: 'Unlock' }).click();
  await expect(page.getByRole('alert').filter({ hasText: 'That access code is not correct' })).toBeVisible();
  await expect(page).toHaveURL(/error=1/);

  // Correct code: the gate answers with the access cookie.
  // The cookie is Secure, so the browser drops it over plain HTTP; check the header, then store the same cookie without Secure.
  const gateResponse = page.waitForResponse(
    (r) => r.request().method() === 'POST' && new URL(r.url()).pathname === '/gate',
  );
  await page.getByLabel('Access code').fill(GATE_TEST_CODE);
  await page.getByRole('button', { name: 'Unlock' }).click();
  const setCookie = (await (await gateResponse).headerValue('set-cookie')) ?? '';
  expect(setCookie).toMatch(/^lcrb_access=[0-9a-f]{64};/);
  expect(setCookie).toMatch(/HttpOnly/i);
  expect(setCookie).toMatch(/SameSite=lax/i);
  expect(setCookie).toMatch(/Max-Age=18000/);
  const value = setCookie.split(';')[0].split('=')[1];
  await context.addCookies([{ name: 'lcrb_access', value, url: base, httpOnly: true, sameSite: 'Lax' }]);
  await page.goto(`${base}/login`);
  await expect(page.getByRole('heading', { name: 'Sign in to Liquor and Cannabis Licensing' })).toBeVisible();

  // Each role lands on its home page with its session stored.
  for (const r of ROLES) {
    await page.goto(`${base}/login`);
    await page.getByRole('button', { name: r.card }).click();
    await page.getByRole('button', { name: 'Sign in', exact: true }).click();
    await expect(page).toHaveURL(new RegExp(`${r.home}$`));
    await expect(page.getByRole('heading', { name: r.heading }).first()).toBeVisible();
    const session = await readStorage<{ role: string }>(page, SESSION_KEY);
    expect(session?.role).toBe(r.role);
  }
  api.assertNoUnmatched();
});
