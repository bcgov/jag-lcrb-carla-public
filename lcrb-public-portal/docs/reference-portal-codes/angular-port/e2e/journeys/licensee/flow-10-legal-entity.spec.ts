/** Flow 10: legal entity build submitted to LCRB. The simulated Dynamics sync fails 8% of the time; with the seeded Math.random the outcome is fixed, and either is accepted. */
import { expect, test } from '@playwright/test';
import { PORTED_FLOWS } from '../../support/ported';
import { preparePage, skipUnlessPorted } from '../../support/parity';
import { expectStorage } from '../../support/flow-helpers';

interface Build {
  status: string;
  version: number;
}

interface SyncRecord {
  caseType: string;
  reference: string;
}

test('flow 10: legal entity build submit', async ({ page }, testInfo) => {
  skipUnlessPorted(testInfo, PORTED_FLOWS, 'flow-10');
  const api = await preparePage(page, { role: 'licensee' });
  await page.goto('/legal-entity');

  await expect(page.getByRole('heading', { name: 'Legal Entity Builder' })).toBeVisible();
  await page.getByRole('button', { name: 'Submit to LCRB' }).click();

  await expect(page.getByText(/Submitted to LCRB|Submitted — sync pending/).first()).toBeVisible();
  await expect(page.getByRole('button', { name: 'Reopen draft' })).toBeVisible();
  await expectStorage<Record<string, Build>>(page, 'lcrb.v2.legal-entity-builds', (builds) => {
    const b = builds?.['build_BC0987654'];
    return b?.status === 'submitted' && b.version === 2;
  });
  await expectStorage<Record<string, SyncRecord>>(page, 'lcrb.v2.dynamics-sync', (records) =>
    Object.values(records ?? {}).some((r) => r.caseType === 'Legal Entity Build' && r.reference === 'build_BC0987654'),
  );
  api.assertNoUnmatched();
});
