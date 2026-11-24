/** Flow 7: an assistant tool call fills a form field. The first stream asks the client to run setField; the second answers with text after the tool result. */
import { expect, test } from '@playwright/test';
import { uiStream } from '../../../tests/fixtures/api/stream';
import { PORTED_FLOWS } from '../../support/ported';
import { preparePage, skipUnlessPorted } from '../../support/parity';
import { expectStorage } from '../../support/flow-helpers';
import { NAME_CHANGE_PATH } from '../../support/name-change';

interface Draft {
  formData: Record<string, unknown>;
}

test('flow 7: assistant tool call drives the form', async ({ page }, testInfo) => {
  skipUnlessPorted(testInfo, PORTED_FLOWS, 'flow-07');
  const api = await preparePage(page, {
    role: 'licensee',
    fixtures: {
      '/api/assistant': {
        sequence: [
          uiStream('[Fixture] Filling that in for you.', [
            { toolName: 'setField', input: { name: 'proposedName', value: 'Restaurant XYZ' } },
          ]),
          uiStream('[Fixture] Done: I set the proposed establishment name to Restaurant XYZ.'),
        ],
      },
    },
  });
  await page.goto(NAME_CHANGE_PATH);

  await page.getByRole('button', { name: 'Open the licensing assistant' }).click();
  const panel = page.getByRole('complementary', { name: 'AI assistant' });
  await panel.getByLabel('Message the assistant').fill('fill my proposed establishment name as Restaurant XYZ');
  await panel.getByRole('button', { name: 'Send message' }).click();

  await expect(page.getByLabel('Proposed establishment name')).toHaveValue('Restaurant XYZ');
  await expect(panel.getByText('Done: I set the proposed establishment name')).toBeVisible();
  await expectStorage<Record<string, Draft>>(
    page,
    'lcrb.app.drafts',
    (drafts) => drafts?.['draft:fp-name-change']?.formData?.['proposedName'] === 'Restaurant XYZ',
  );
  expect(api.calls['/api/assistant']).toBe(2);
  api.assertNoUnmatched();
});
