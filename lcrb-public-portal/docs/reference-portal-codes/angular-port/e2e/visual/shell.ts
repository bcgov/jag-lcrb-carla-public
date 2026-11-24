/** Element screenshots of the BC Gov header, footer and header overlays. Only Playwright types are imported, so Vitest can load this file. */
import type { Locator, Page } from '@playwright/test';
import type { Role } from '../support/roles';

export interface ShellEntry {
  id: string;
  role: Role | null;
  /** Viewports to capture; both by default. */
  viewports?: Array<'desktop' | 'mobile'>;
  /** Interaction before the screenshot. */
  act?: (page: Page) => Promise<void>;
  /** CSS added to both apps before the capture, to take the element out of the page flow. */
  style?: string;
  /** The element to capture. */
  target: (page: Page) => Locator;
}

/** A page with the full header in every role and no active nav link. */
export const SHELL_PATH = '/accessibility';

const ROLES: Array<Role | null> = [null, 'licensee', 'applicant', 'police', 'local-government', 'lcrb'];
const header = (page: Page) => page.locator('header').first();

export const SHELL: ShellEntry[] = [
  ...ROLES.map((role) => ({ id: `shell-header-${role ?? 'signed-out'}`, role, target: header })),
  {
    id: 'shell-footer',
    role: 'licensee',
    // The footer's y depends on the page above it and shifts the clip by a pixel; pin it to the viewport top.
    style: 'footer { position: fixed !important; top: 0; left: 0; right: 0; z-index: 2147483647; }',
    target: (page) => page.locator('footer').first(),
  },
  {
    id: 'shell-user-menu-open',
    role: 'licensee',
    viewports: ['desktop'],
    act: async (page) => {
      // The user menu is the first menu trigger; the nav group triggers follow it.
      await header(page).locator('button[aria-haspopup="menu"]').first().click();
      await page.getByRole('menu').waitFor();
    },
    target: (page) => page.locator('[data-slot="dropdown-menu-content"]'),
  },
  {
    id: 'shell-mobile-menu-open',
    role: 'licensee',
    viewports: ['mobile'],
    act: async (page) => {
      await page.getByRole('button', { name: 'Open menu' }).click();
      await page.getByRole('dialog').waitFor();
    },
    target: (page) => page.locator('[data-slot="sheet-content"]'),
  },
  {
    id: 'shell-notifications-open',
    role: 'licensee',
    viewports: ['desktop'],
    act: async (page) => {
      await page.getByRole('button', { name: /^Notifications/ }).first().click();
      await page.getByRole('dialog').waitFor();
    },
    target: (page) => page.locator('[data-slot="popover-content"]'),
  },
];
