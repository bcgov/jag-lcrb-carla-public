/** Flow 9: document upload on the confirmation page. /api/upload is answered by a fixture (the real route needs a Blob token). */
import { expect, test } from '@playwright/test';
import { PORTED_FLOWS } from '../../support/ported';
import { preparePage, skipUnlessPorted } from '../../support/parity';
import { CONFIRMATION_ID, SUBMISSION_SEED } from '../../visual/routes';
import { expectStorage } from '../../support/flow-helpers';

interface DocumentRecord {
  subjectId: string;
  name: string;
  contentType: string;
  scanState: string;
}

test('flow 9: document upload', async ({ page }, testInfo) => {
  skipUnlessPorted(testInfo, PORTED_FLOWS, 'flow-09');
  const api = await preparePage(page, { role: 'licensee', storage: SUBMISSION_SEED });
  await page.goto(`/applications/confirmation/${CONFIRMATION_ID}`);

  await expect(page.getByText('No documents uploaded yet.')).toBeVisible();
  await page.locator('input[type="file"]').setInputFiles({
    name: 'test.pdf',
    mimeType: 'application/pdf',
    buffer: Buffer.from('%PDF-1.4\n% parity fixture\n'),
  });

  await expect(page.getByText('test.pdf')).toBeVisible();
  await expect(page.getByText('Clean', { exact: true })).toBeVisible({ timeout: 5_000 });
  await expectStorage<Record<string, DocumentRecord>>(page, 'lcrb.v2.documents', (docs) =>
    Object.values(docs ?? {}).some(
      (d) => d.name === 'test.pdf' && d.subjectId === 'FP-126001' && d.contentType === 'application/pdf' && d.scanState === 'clean',
    ),
  );
  expect(api.calls['/api/upload']).toBe(1);
  api.assertNoUnmatched();
});
