import { TestBed } from '@angular/core/testing';
import { Router, provideRouter } from '@angular/router';
import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { SessionProvider } from '@core/session/session-provider';
import { computeFees } from '@data-access/repositories/tied-house/mock/tied-house';
import { type TiedHouseApplication } from '@domain/models/tied-house';
import { TiedHousePage } from './tied-house.page';

const settle = async (f: { detectChanges(): void; whenStable(): Promise<unknown> }, n = 3) => {
  for (let i = 0; i < n; i++) {
    f.detectChanges();
    await f.whenStable();
    await new Promise((r) => setTimeout(r, 0));
  }
  f.detectChanges();
};

async function mount(role = 'licensee') {
  TestBed.configureTestingModule({ providers: [provideRouter([])] });
  const navigate = vi.spyOn(TestBed.inject(Router), 'navigateByUrl').mockResolvedValue(true);
  TestBed.inject(SessionProvider).login({ role, firstName: 'Alex', lastName: 'Rivera', organization: 'Harding Hospitality Group Ltd.' } as never);
  const fixture = TestBed.createComponent(TiedHousePage);
  document.body.appendChild(fixture.nativeElement);
  await settle(fixture);
  return { fixture, el: fixture.nativeElement as HTMLElement, navigate, next: () => settle(fixture) };
}

const apps = () => JSON.parse(localStorage.getItem('lcrb.v2.tied-house-applications') ?? '{}') as Record<string, TiedHouseApplication>;
const app = (source = 'voluntary') => apps()[`th_${source}_BC0987654`];
const buttons = (root: ParentNode) => [...root.querySelectorAll('button')] as HTMLButtonElement[];
const button = (root: ParentNode, text: string) => buttons(root).find((b) => b.textContent!.trim().includes(text));
const setValue = (input: HTMLInputElement | HTMLTextAreaElement, value: string) => {
  input.value = value;
  input.dispatchEvent(new Event('input'));
};

describe('TiedHousePage', () => {
  beforeEach(() => {
    localStorage.clear();
    sessionStorage.clear();
  });
  afterEach(() => {
    document.querySelectorAll('.cdk-overlay-container').forEach((c) => (c.innerHTML = ''));
    document.body.innerHTML = '';
  });

  it('shows the licensee-only notice to another role', async () => {
    const { el } = await mount('applicant');
    expect(el.textContent).toContain('Licensee tool');
    expect(el.textContent).toContain('Tied House declarations are available to licensee (business) accounts.');
    expect(el.querySelector('a[href="/login"]')).not.toBeNull();
    expect(el.querySelector('tied-house-declaration-section')).toBeNull();
  });

  it('renders the hero, the context switcher and the gating question', async () => {
    const { el, next } = await mount();
    expect(el.querySelector('h1')!.textContent).toContain('Tied House Declarations');
    expect(el.textContent).toContain('Harding Hospitality Group Ltd.');
    expect(el.querySelector('page-hero a[href="/legal-entity"]')!.textContent).toContain('Legal Entity build');
    const contexts = [...el.querySelectorAll('main button[aria-pressed]')];
    expect(contexts.map((c) => c.textContent!.trim())).toEqual(['Voluntary update', 'Legal Entity Review', 'New application']);
    expect(contexts[0].getAttribute('aria-pressed')).toBe('true');
    expect(el.textContent).toContain('You are updating your tied house declarations outside of another application.');
    expect(el.textContent).toContain('Do you or any of your shareholders, directors, or officers have any amount of interest');
    expect(el.querySelector('a[href="/applications"]')!.textContent).toContain('My Applications');
    // The application is created after the first render.
    expect(app().jobNumber).toMatch(/^Tied House Update Job No\. /);

    (contexts[2] as HTMLElement).click();
    await next();
    expect(el.textContent).toContain('Declare any tied house connections as part of your new licence application.');
    expect(app('new-application')).toBeTruthy();
    // The gate's initial value is read once (a React useState initialiser), so switching to the Legal Entity Review context later does not default it to Yes.
    (contexts[1] as HTMLElement).click();
    await next();
    expect(el.textContent).toContain('A Legal Entity Review of a permanent change requires you to declare');
    expect(el.textContent).not.toContain('Existing Tied House Relationships');
    expect(app('le-review')).toBeTruthy();
  });

  it('answers the gate: No explains, Yes reveals the existing relationships and declarations', async () => {
    const { el, next } = await mount();
    expect(el.textContent).not.toContain('Existing Tied House Relationships');
    button(el, 'no')!.click();
    await next();
    expect(el.textContent).toContain('No tied house connections to declare. No fee applies.');
    button(el, 'yes')!.click();
    await next();
    expect(el.textContent).toContain('Existing Tied House Relationships');
    expect(el.textContent).toContain('Morgan Blake – 14/03/1979');
    expect(el.textContent).toContain('Spouse · Harbourside Taproom (#441882)');
    expect(el.textContent).toContain('Island Craft Beverages Inc.');
    expect(el.textContent).toContain('Date tied: September 12, 2024');
    expect(el.textContent).toContain('No declarations added yet.');
    expect(button(el, 'Add New Tied House')).toBeTruthy();
  });

  it('changes and removes existing relationships', async () => {
    const { el, next } = await mount();
    button(el, 'yes')!.click();
    await next();
    button(el, 'Change')!.click();
    await next();
    expect(el.textContent).toContain('You are changing the details of an existing tied house relationship.');
    expect(app().declarations.map((d) => d.changeType)).toEqual(['change-details']);
    button(el, 'Remove')!.click();
    await next();
    expect(app().existingRelationships.length).toBe(1);
    expect(app().declarations.some((d) => d.changeType === 'remove' && d.status === 'saved')).toBe(true);
  });

  it('adds a declaration, validates it, saves it and pays both fee streams before submitting', async () => {
    const { el, next, navigate } = await mount();
    button(el, 'yes')!.click();
    await next();
    button(el, 'Add New Tied House')!.click();
    await next();
    expect(el.textContent).toContain('Select Tied House Type');
    const save = button(el, 'Save declaration')!;
    expect(save.disabled).toBe(true);

    // Individual (default): the required fields.
    setValue(el.querySelector('#th-dob')!, '01/02/1980');
    setValue(el.querySelector('#th-first')!, 'Jamie');
    setValue(el.querySelector('#th-last')!, 'Fox');
    (el.querySelector('#th-ind-rel') as HTMLButtonElement).click();
    await next();
    [...document.querySelectorAll('.cdk-overlay-container [role="option"]')].find((o) => o.textContent!.trim() === 'Spouse')!.dispatchEvent(new MouseEvent('click', { bubbles: true }));
    await next();
    expect(save.disabled).toBe(true);

    // The licence combo: search by name, choose a result.
    const search = el.querySelector('#th-assoc') as HTMLInputElement;
    search.dispatchEvent(new Event('focus'));
    setValue(search, 'summit');
    await next();
    [...el.querySelectorAll('[associatedLicenceCombo] button')].find((b) => b.textContent!.includes('Summit Rooftop Lounge'))!.dispatchEvent(new MouseEvent('click', { bubbles: true }));
    await next();
    expect(el.textContent).toContain('#512900 · Liquor Primary · Summit Hospitality Group Ltd.');
    expect(el.textContent).toContain('Saving this declaration will require:');
    expect(save.disabled).toBe(false);
    save.click();
    await next();

    // Saved: a locked summary with a Remove action, the consent line and the payment panel.
    const saved = app().declarations.find((d) => d.status === 'saved')!;
    expect(saved.individual!.firstName).toBe('Jamie');
    expect(el.textContent).toContain('Jamie Fox');
    expect(el.textContent).toContain('Saved · Add tied house declaration (individual)');
    expect(el.textContent).toContain('Supporting documents required');
    expect(el.textContent).toContain('By saving a declaration you consent to a dated tied house connection');
    const fees = computeFees(app().declarations, app().jobNumber);
    expect(fees.grandTotal).toBeGreaterThan(0);
    expect(el.textContent).toContain('Liquor fees');
    expect(el.textContent).toContain('Pay all outstanding fees to enable submission.');
    const submit = button(el, 'Submit to the Branch')!;
    expect(submit.disabled).toBe(true);

    button(el, 'Pay now')!.click();
    await next();
    expect(app().liquorReceipt).toBeTruthy();
    expect(el.textContent).toContain(`Paid · ${app().liquorReceipt}`);
    expect(button(el, 'Submit to the Branch')!.disabled).toBe(false);
    button(el, 'Submit to the Branch')!.click();
    await next();
    expect(app().status).toBe('submitted');
    expect(navigate).toHaveBeenCalledWith('/tied-house/confirmation/th_voluntary_BC0987654');
  });

  it('cancels a draft and removes a saved declaration (hiding the payment panel)', async () => {
    const { el, next } = await mount();
    button(el, 'yes')!.click();
    await next();
    button(el, 'Add New Tied House')!.click();
    await next();
    expect(app().declarations.length).toBe(1);
    button(el, 'Cancel')!.click();
    await next();
    expect(app().declarations.length).toBe(0);
    expect(el.textContent).toContain('No declarations added yet.');
  });

  it('reshapes the draft to the Legal Entity variant', async () => {
    const { el, next } = await mount();
    button(el, 'yes')!.click();
    await next();
    button(el, 'Add New Tied House')!.click();
    await next();
    expect(el.querySelector('#th-dob')).not.toBeNull();
    button(el.querySelector('fieldset')!, 'Legal Entity')!.click();
    await next();
    expect(el.querySelector('#th-dob')).toBeNull();
    for (const id of ['#th-biztype', '#th-ent-rel', '#th-entname']) expect(el.querySelector(id)).not.toBeNull();
    expect(el.textContent).toContain('Legal Entity Name');
  });
});
