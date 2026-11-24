import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { afterEach, beforeEach, describe, expect, it } from 'vitest';
import { SessionProvider } from '@core/session/session-provider';
import { getAccountConfig } from '@data-access/repositories/account/mock/account-settings';
import { getAutoApprovalPolicy } from '@data-access/repositories/special-events/sep-auto-approval';
import { AccountPage } from './account.page';

type Role = 'licensee' | 'applicant' | 'police' | 'local-government' | 'lcrb';

const SESSIONS: Record<Role, Record<string, unknown>> = {
  licensee: { role: 'licensee', firstName: 'Matt', lastName: 'Harding', organization: 'Harding Hospitality Group', email: 'matt.harding@hardinghospitality.ca' },
  applicant: { role: 'applicant', firstName: 'Alex', lastName: 'Rivera', organization: 'Community Events Society', email: 'alex+x@events.ca' },
  police: { role: 'police', firstName: 'Pat', lastName: 'Lee', organization: 'Vancouver Police Department', email: 'pat@vpd.ca' },
  'local-government': { role: 'local-government', firstName: 'Lee', lastName: 'Chan', organization: 'City of Kelowna', email: 'lee@kelowna.ca' },
  lcrb: { role: 'lcrb', firstName: 'Sam', lastName: 'Ng', organization: 'LCRB', email: 'sam@gov.bc.ca' },
};

const EXPECTED_TABS: Record<Role, string[]> = {
  licensee: ['Profile', 'Business', 'Notifications', 'Delegates'],
  applicant: ['Profile', 'Organization', 'Notifications', 'Delegates'],
  police: ['Profile', 'Agency', 'Notifications', 'Agency Users', 'Auto-approval'],
  'local-government': ['Profile', 'Jurisdiction', 'Notifications', 'Delegates', 'Commenting'],
  lcrb: ['Profile', 'Branch', 'Notifications'],
};

async function settle(fixture: { detectChanges(): void; whenStable(): Promise<unknown> }) {
  fixture.detectChanges();
  await fixture.whenStable();
  await new Promise((r) => setTimeout(r, 0));
  fixture.detectChanges();
}

async function mount(role: Role | null = 'licensee', extra: Record<string, unknown> = {}) {
  TestBed.configureTestingModule({ providers: [provideRouter([])] });
  if (role) TestBed.inject(SessionProvider).login({ ...SESSIONS[role], ...extra } as never);
  const fixture = TestBed.createComponent(AccountPage);
  document.body.appendChild(fixture.nativeElement);
  await settle(fixture);
  const el = fixture.nativeElement as HTMLElement;
  const tabs = () => [...el.querySelectorAll('[role="tab"]')] as HTMLButtonElement[];
  const open = async (label: string) => {
    tabs().find((t) => t.textContent?.trim() === label)!.click();
    await settle(fixture);
  };
  return { fixture, el, tabs, open };
}
const overlay = () => document.querySelector('.cdk-overlay-container') as HTMLElement;
const btn = (root: ParentNode, text: string) => [...root.querySelectorAll('button')].find((b) => b.textContent?.trim() === text) as HTMLButtonElement;
const type = (input: HTMLInputElement, value: string) => {
  input.value = value;
  input.dispatchEvent(new Event('input', { bubbles: true }));
};
const rows = (el: HTMLElement) => [...el.querySelectorAll('tbody tr')] as HTMLElement[];

describe('AccountPage', () => {
  beforeEach(() => {
    localStorage.clear();
    sessionStorage.clear();
  });
  afterEach(() => {
    document.querySelectorAll('.cdk-overlay-container').forEach((c) => (c.innerHTML = ''));
    document.body.innerHTML = '';
  });

  for (const role of Object.keys(EXPECTED_TABS) as Role[]) {
    it(`renders the ${role} config: heading, breadcrumb and tabs`, async () => {
      const { el, tabs } = await mount(role);
      const config = getAccountConfig(role);
      expect(el.querySelector('h1')!.textContent).toBe('Account Settings');
      expect(el.textContent).toContain(config.description);
      expect(el.querySelector('nav[aria-label="Breadcrumb"]')!.textContent).toContain(config.home.label);
      expect(tabs().map((t) => t.textContent!.trim())).toEqual(EXPECTED_TABS[role]);
      expect(tabs()[0].getAttribute('aria-selected')).toBe('true');
      expect(el.querySelector('[role="tablist"]')!.getAttribute('aria-label')).toBe('Account settings sections');
      expect(el.textContent).toContain(config.identity.cardTitle);
      expect(el.textContent).toContain(SESSIONS[role]['firstName'] as string);
      if (role === 'lcrb') expect(el.textContent).toContain('Delegate Access');
      else expect(el.textContent).not.toContain('Delegate Access');
    });
  }

  it('defaults to the licensee config with no session', async () => {
    const { el, tabs } = await mount(null);
    expect(tabs().map((t) => t.textContent!.trim())).toEqual(EXPECTED_TABS.licensee);
    expect(el.textContent).toContain('Matt');
    expect(el.textContent).toContain('matt.harding@hardinghospitality.ca');
  });

  it('keeps the prototype classes on the tab list, triggers and icons', async () => {
    const { el, tabs } = await mount('licensee');
    expect(el.querySelector('[role="tablist"]')!.className).toContain('grid w-full grid-cols-2 sm:grid-cols-3 lg:w-auto lg:inline-flex');
    expect(tabs()[0].className).toContain('gap-2');
    expect(tabs()[0].querySelector('svg')!.getAttribute('class')).toBe('lucide lucide-user h-4 w-4 hidden sm:block');
    expect(tabs()[1].querySelector('svg')!.classList.contains('lucide-building-2')).toBe(true);
    expect(el.querySelector('main')!.className).toBe('flex-1');
    expect(el.querySelector('main > div:last-child')!.className).toBe('max-w-5xl mx-auto px-4 sm:px-6 lg:px-8 py-8');
  });

  it('maps the organization icon key per role', async () => {
    for (const [role, icon] of [['licensee', 'building2'], ['applicant', 'briefcase'], ['police', 'shield'], ['local-government', 'landmark'], ['lcrb', 'building2']] as const) {
      TestBed.resetTestingModule();
      localStorage.clear();
      const { tabs } = await mount(role);
      expect(tabs()[1].querySelector('svg')!.getAttribute('class')).toContain(`lucide-${icon}`);
      document.body.innerHTML = '';
    }
  });

  it('profile: edit toggles inputs, username derives from the session email', async () => {
    const { fixture, el } = await mount('applicant');
    expect(el.textContent).toContain('alex_x');
    expect(el.querySelectorAll('input').length).toBe(0);
    btn(el, 'Edit').click();
    await settle(fixture);
    const inputs = [...el.querySelectorAll('input')] as HTMLInputElement[];
    expect(inputs.map((i) => i.value)).toEqual(['Alex', 'Rivera', 'alex+x@events.ca', '(250) 555-1234']);
    expect(btn(el, 'Save Changes')).toBeTruthy();
    expect(btn(el, 'Save Changes').className).toContain('bg-[#013366] hover:bg-[#38598A]');
    btn(el, 'Cancel').click();
    await settle(fixture);
    expect(el.querySelectorAll('input').length).toBe(0);
    expect(btn(el, 'Edit')).toBeTruthy();
  });

  it('organization: session organization overrides the fixture and name gets font-medium', async () => {
    const { el, open } = await mount('police');
    await open('Agency');
    const name = [...el.querySelectorAll('p')].find((p) => p.textContent!.trim() === 'Vancouver Police Department')!;
    expect(name.getAttribute('class')).toBe('text-sm font-medium');
    const other = [...el.querySelectorAll('p')].find((p) => p.textContent!.trim() === 'Municipal Police Department')!;
    expect(other.getAttribute('class')).toBe('text-sm ');
    expect(el.textContent).toContain('Address on file');
  });

  it('organization: licensee shows two addresses and the external link', async () => {
    const { el, open } = await mount('licensee');
    await open('Business');
    expect(el.textContent).toContain('Mailing and physical addresses on file');
    const a = el.querySelector('a[target="_blank"]') as HTMLAnchorElement;
    expect(a.href).toBe('https://www.bcregistry.gov.bc.ca/');
    expect(a.rel).toBe('noopener noreferrer');
    expect(a.className).toContain('text-[#255A90] gap-1 p-0 h-auto mt-2');
  });

  it('notifications: switches, saved flag and per-role defaults; tab remount resets state', async () => {
    const { fixture, el, open } = await mount('licensee');
    await open('Notifications');
    const email = el.querySelector('[aria-label="Email notifications"]') as HTMLButtonElement;
    const sms = el.querySelector('[aria-label="SMS notifications"]') as HTMLButtonElement;
    expect(email.getAttribute('aria-checked')).toBe('true');
    expect(sms.getAttribute('aria-checked')).toBe('false');
    expect(el.textContent).toContain('Receive text messages to (250) 555-1234');
    const types = getAccountConfig('licensee').notificationTypes;
    const first = el.querySelector(`[aria-label="${types[0].label}"]`) as HTMLButtonElement;
    expect(first.getAttribute('aria-checked')).toBe(String(types[0].defaultOn));
    expect(el.textContent).not.toContain('Preferences saved');
    btn(el, 'Save Preferences').click();
    await settle(fixture);
    expect(el.textContent).toContain('Preferences saved');
    sms.click();
    await settle(fixture);
    expect(sms.getAttribute('aria-checked')).toBe('true');
    expect(el.textContent).not.toContain('Preferences saved');
    const rowsEls = [...el.querySelectorAll('[data-slot="card-content"].space-y-0 > div')];
    expect(rowsEls[0].getAttribute('class')).toBe('flex items-center justify-between gap-4 py-4 border-b');
    expect(rowsEls[rowsEls.length - 1].getAttribute('class')).toBe('flex items-center justify-between gap-4 py-4 ');

    await open('Profile');
    await open('Notifications');
    expect((el.querySelector('[aria-label="SMS notifications"]') as HTMLButtonElement).getAttribute('aria-checked')).toBe('false');
  });

  it('delegates: adds a delegate through the dialog with validation', async () => {
    const { fixture, el, open } = await mount('licensee');
    await open('Delegates');
    const copy = getAccountConfig('licensee').delegates!;
    expect(rows(el).map((r) => r.querySelector('td')!.textContent)).toEqual(['Jane Smith', 'Bob Johnson']);
    btn(el, copy.addButtonLabel).click();
    await settle(fixture);
    const dlg = document.querySelector('ui-dialog-content') as HTMLElement;
    expect(dlg.textContent).toContain(copy.addDialogTitle);
    btn(dlg, copy.addButtonLabel).click();
    await settle(fixture);
    expect(dlg.textContent).toContain('Please enter both a name and an email address.');
    type(dlg.querySelector('#delegate-name') as HTMLInputElement, 'New Person');
    type(dlg.querySelector('#delegate-email') as HTMLInputElement, 'nope');
    await settle(fixture);
    btn(dlg, copy.addButtonLabel).click();
    await settle(fixture);
    expect(dlg.textContent).toContain('Please enter a valid email address.');
    type(dlg.querySelector('#delegate-email') as HTMLInputElement, 'new@example.com');
    await settle(fixture);
    btn(dlg, copy.addButtonLabel).click();
    await settle(fixture);
    expect(rows(el).length).toBe(3);
    const added = rows(el)[2];
    expect(added.textContent).toContain('New Person');
    expect(added.textContent).toContain('new@example.com');
    expect(added.textContent).toContain('Pending');
    expect(added.textContent).toContain('View Only');
    expect(added.textContent).toContain(new Date().toLocaleDateString('en-CA', { month: 'short', day: 'numeric', year: 'numeric' }));
    expect(document.querySelector('ui-dialog-content')?.getAttribute('data-state') ?? 'closed').not.toBe('open');
  });

  it('delegates: Cancel keeps the typed form, Escape clears it', async () => {
    const { fixture, el, open } = await mount('licensee');
    await open('Delegates');
    const label = getAccountConfig('licensee').delegates!.addButtonLabel;
    btn(el, label).click();
    await settle(fixture);
    type(document.querySelector('#delegate-name') as HTMLInputElement, 'Typed');
    await settle(fixture);
    btn(document.querySelector('ui-dialog-content') as HTMLElement, 'Cancel').click();
    await settle(fixture);
    btn(el, label).click();
    await settle(fixture);
    expect((document.querySelector('#delegate-name') as HTMLInputElement).value).toBe('Typed');
    (document.querySelector('ui-dialog-content') as HTMLElement).dispatchEvent(new KeyboardEvent('keydown', { key: 'Escape', bubbles: true }));
    await settle(fixture);
    btn(el, label).click();
    await settle(fixture);
    expect((document.querySelector('#delegate-name') as HTMLInputElement).value).toBe('');
  });

  it('delegates: removes a delegate through the alert dialog, Cancel keeps it', async () => {
    const { fixture, el, open } = await mount('licensee');
    await open('Delegates');
    const trash = () => [...el.querySelectorAll('tbody button[data-slot="button"]')].filter((b) => b.querySelector('.lucide-trash-2')) as HTMLButtonElement[];
    expect(trash().length).toBe(2);
    trash()[0].click();
    await settle(fixture);
    const alert = document.querySelector('ui-alert-dialog-content') as HTMLElement;
    expect(alert.closest('[role="alertdialog"]')).not.toBeNull();
    expect(alert.textContent).toContain('Remove Delegate?');
    expect(alert.textContent).toContain("This will revoke Jane Smith's access to your account.");
    btn(alert, 'Cancel').click();
    await settle(fixture);
    expect(rows(el).length).toBe(2);

    trash()[0].click();
    await settle(fixture);
    const confirm = btn(document.querySelector('ui-alert-dialog-content') as HTMLElement, 'Remove');
    expect(confirm.className).toContain('bg-destructive hover:bg-destructive/90');
    confirm.click();
    await settle(fixture);
    expect(rows(el).map((r) => r.querySelector('td')!.textContent)).toEqual(['Bob Johnson']);
    trash()[0].click();
    await settle(fixture);
    btn(document.querySelector('ui-alert-dialog-content') as HTMLElement, 'Remove').click();
    await settle(fixture);
    expect(rows(el).length).toBe(1);
    expect(rows(el)[0].textContent).toContain('No delegates yet. Use “Add Delegate” to invite someone to your account.');
    expect(rows(el)[0].querySelector('td')!.getAttribute('colspan')).toBe('6');
  });

  it('delegates: changes an access level through the select', async () => {
    const { fixture, el, open } = await mount('licensee');
    await open('Delegates');
    const trigger = rows(el)[0].querySelector('[role="combobox"]') as HTMLButtonElement;
    expect(trigger.textContent).toContain('Full Access');
    expect(trigger.className).toContain('w-[130px]');
    trigger.click();
    await settle(fixture);
    const option = [...overlay().querySelectorAll('[role="option"]')].find((o) => o.textContent!.trim() === 'Submit Only') as HTMLElement;
    option.click();
    await settle(fixture);
    expect((rows(el)[0].querySelector('[role="combobox"]') as HTMLElement).textContent).toContain('Submit Only');
  });

  it('police: auto-approval tab shows the settings and a toggle persists', async () => {
    const { fixture, el, open } = await mount('police');
    await open('Auto-approval');
    expect(el.textContent).toContain('Special Event Permit auto-approval');
    (el.querySelector('#auto-approval') as HTMLButtonElement).click();
    await settle(fixture);
    btn(document.querySelector('ui-dialog-content') as HTMLElement, 'Turn on').click();
    await settle(fixture);
    expect(getAutoApprovalPolicy().enabled).toBe(true);
    expect(el.querySelector('#auto-approval')!.getAttribute('aria-checked')).toBe('true');
  });

  it('local government: commenting tab shows the settings and a change persists', async () => {
    const { fixture, el, open } = await mount('local-government');
    await open('Commenting');
    expect(el.textContent).toContain('Commenting on applications');
    (el.querySelector('#opt-out-commenting') as HTMLButtonElement).click();
    await settle(fixture);
    btn(document.querySelector('ui-alert-dialog-content') as HTMLElement, 'Turn off commenting').click();
    await settle(fixture);
    expect(JSON.parse(localStorage.getItem('lcrb.lg.commenting')!).optedOut).toBe(true);
    expect(el.querySelector('#opt-out-commenting')!.getAttribute('aria-checked')).toBe('true');
  });

  it('only the active tab body is rendered', async () => {
    const { el, open } = await mount('police');
    expect(el.querySelector('#auto-approval')).toBeNull();
    await open('Auto-approval');
    expect(el.textContent).not.toContain('Personal Information');
  });
});
