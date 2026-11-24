/** Screens reached by an interaction (dialogs, menus, panels, toasts, guided mode). Only Playwright types are imported, so Vitest can load this file. */
import type { Page } from '@playwright/test';
import type { Role } from '../support/roles';

export interface StateEntry {
  id: string;
  path: string;
  role: Role | null;
  storage?: Record<string, string>;
  /** Open on the gated app (ACCESS_CODE set) instead of the normal one. */
  gated?: boolean;
  /** Viewports to capture; both by default. */
  viewports?: Array<'desktop' | 'mobile'>;
  act: (page: Page) => Promise<void>;
}

const NAME_CHANGE = '/applications/new/fp-name-change';

export const STATES: StateEntry[] = [
  {
    id: 'state-assistant-open',
    path: '/dashboard',
    role: 'licensee',
    act: async (page) => {
      await page.getByRole('button', { name: 'Open the licensing assistant' }).click();
      await page.getByRole('complementary', { name: 'AI assistant' }).waitFor();
    },
  },
  {
    id: 'state-notifications-open',
    path: '/dashboard',
    role: 'licensee',
    // The bell sits in the collapsed header menu on mobile.
    viewports: ['desktop'],
    act: async (page) => {
      await page.getByRole('button', { name: /^Notifications/ }).first().click();
      await page.getByRole('dialog').or(page.getByRole('menu')).first().waitFor();
    },
  },
  {
    id: 'state-applications-actions-menu',
    path: '/applications',
    role: 'licensee',
    viewports: ['desktop'],
    act: async (page) => {
      await page.getByRole('button', { name: 'Actions' }).first().click();
      await page.getByRole('menu').waitFor();
    },
  },
  {
    // "Cancel Application" on the first (In Progress) row removes the draft with a toast and no dialog; on the Submitted tab it opens the dialog.
    id: 'state-cancel-application-click',
    path: '/applications',
    role: 'licensee',
    viewports: ['desktop'],
    act: async (page) => {
      await page.getByRole('button', { name: 'Actions' }).first().click();
      await page.getByRole('menuitem', { name: 'Cancel Application' }).click();
      await page.getByRole('menu').waitFor({ state: 'detached' });
    },
  },
  {
    id: 'state-add-entity-dialog',
    path: '/legal-entity',
    role: 'licensee',
    act: async (page) => {
      await page.getByRole('button', { name: 'Add entity' }).first().click();
      await page.getByRole('dialog').waitFor();
    },
  },
  {
    id: 'state-police-approve-dialog',
    path: '/police/review/SEP-2026-04812',
    role: 'police',
    act: async (page) => {
      await page.getByRole('button', { name: 'Assign to me' }).first().click();
      await page.getByRole('button', { name: 'Approve permit' }).click();
      await page.getByRole('dialog', { name: 'Approve permit' }).waitFor();
    },
  },
  {
    id: 'state-compliance-respond-dialog',
    path: '/compliance',
    role: 'licensee',
    act: async (page) => {
      await page.getByRole('button', { name: 'Respond to this action' }).first().click();
      await page.getByRole('dialog').waitFor();
    },
  },
  {
    id: 'state-licence-select-open',
    path: NAME_CHANGE,
    role: 'licensee',
    act: async (page) => {
      await page.getByRole('combobox', { name: 'Licence Number' }).click();
      await page.getByRole('listbox').waitFor();
    },
  },
  {
    id: 'state-validation-toast',
    path: NAME_CHANGE,
    role: 'licensee',
    act: async (page) => {
      await page.getByRole('button', { name: 'Next', exact: true }).click();
      await page.getByText('Some required fields need attention').first().waitFor();
    },
  },
  {
    id: 'state-guided-mode',
    path: NAME_CHANGE,
    role: 'licensee',
    storage: { 'lcrb.app.mode': 'guided' },
    act: async (page) => {
      await page.getByText(/Question 1 of/).first().waitFor();
    },
  },
  {
    id: 'state-reviewer-brief-result',
    path: '/police/review/SEP-2026-04812',
    role: 'police',
    act: async (page) => {
      await page.getByRole('button', { name: 'Generate brief' }).click();
      await page.getByText('AI suggestion only').first().waitFor();
    },
  },
  {
    id: 'state-gate-error',
    path: '/gate?error=1',
    role: null,
    gated: true,
    act: async (page) => {
      await page.getByRole('alert').filter({ hasText: 'access code' }).waitFor();
    },
  },
];
