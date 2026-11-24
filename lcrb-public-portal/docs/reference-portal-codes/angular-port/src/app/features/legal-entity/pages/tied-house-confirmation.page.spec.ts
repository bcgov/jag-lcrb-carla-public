import { Component, signal } from '@angular/core';
import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { formatCurrency } from '@domain/tied-house/tied-house';
import { computeFees } from '@data-access/repositories/tied-house/mock/tied-house';
import { type TiedHouseApplication, type TiedHouseDeclaration } from '@domain/models/tied-house';
import { TiedHouseConfirmationPage } from './tied-house-confirmation.page';

const settle = async (f: { detectChanges(): void; whenStable(): Promise<unknown> }, n = 3) => {
  for (let i = 0; i < n; i++) {
    f.detectChanges();
    await f.whenStable();
    await new Promise((r) => setTimeout(r, 0));
  }
  f.detectChanges();
};

const decl = (over: Partial<TiedHouseDeclaration>): TiedHouseDeclaration => ({
  id: 'decl_1',
  type: 'individual',
  status: 'saved',
  changeType: 'add-individual',
  associatedLicenceNumber: '512900',
  associatedEstablishmentName: 'Summit Rooftop Lounge',
  domain: 'liquor',
  requiredDocs: [],
  createdAt: '2026-11-02T16:00:00.000Z',
  individual: { dateOfBirth: '01/02/1980', firstName: 'Jamie', lastName: 'Fox', relationship: 'spouse' },
  ...over,
});

const seed = (id: string, declarations: TiedHouseDeclaration[], extra: Partial<TiedHouseApplication> = {}) => {
  const app: TiedHouseApplication = {
    id,
    businessLegalName: 'Harding Hospitality Group Ltd.',
    businessNumber: 'BC0987654',
    source: 'voluntary',
    jobNumber: 'Tied House Update Job No. 120001',
    status: 'submitted',
    declarations,
    existingRelationships: [],
    submittedAt: '2026-11-02T17:00:00.000Z',
    createdAt: '2026-11-02T16:00:00.000Z',
    updatedAt: '2026-11-02T17:00:00.000Z',
    ...extra,
  };
  localStorage.setItem('lcrb.v2.tied-house-applications', JSON.stringify({ [id]: app }));
  return app;
};

@Component({ imports: [TiedHouseConfirmationPage], template: `<tied-house-confirmation-page [id]="id()" />` })
class Host {
  id = signal('th_voluntary_BC0987654');
}

async function mount(id?: string) {
  TestBed.configureTestingModule({ providers: [provideRouter([])] });
  const fixture = TestBed.createComponent(Host);
  if (id) fixture.componentInstance.id.set(id);
  document.body.appendChild(fixture.nativeElement);
  fixture.detectChanges();
  const el = fixture.nativeElement as HTMLElement;
  await settle(fixture);
  return { fixture, el, next: () => settle(fixture) };
}

describe('TiedHouseConfirmationPage', () => {
  beforeEach(() => localStorage.clear());
  afterEach(() => {
    vi.restoreAllMocks();
    document.body.innerHTML = '';
  });

  it('shows the payment receipt', async () => {
    const app = seed('th_voluntary_BC0987654', [decl({}), decl({ id: 'decl_2', type: 'legal-entity', individual: undefined, entity: { businessType: 'private-corporation', legalEntityName: 'Island Craft Beverages Inc.', relationshipType: 'shareholding' }, changeType: 'add-legal-entity', domain: 'cannabis', associatedEstablishmentName: 'Harbour Cannabis Co.', associatedLicenceNumber: '608114' })], {
      liquorReceipt: 'RCPT-LIQ-1',
      cannabisReceipt: 'RCPT-CAN-1',
    });
    const { el } = await mount();
    expect(el.querySelector('h1')!.textContent).toContain('Payment received');
    expect(el.textContent).toContain('and your payment has been received. A copy has been sent to your email.');
    expect(el.querySelector('.font-mono')!.textContent).toContain('Tied House Update Job No. 120001');
    expect(el.textContent).toContain('November 2, 2026');
    expect(el.textContent).toContain('Jamie Fox');
    expect(el.textContent).toContain('Add tied house declaration (individual) · Summit Rooftop Lounge (#512900)');
    expect(el.textContent).toContain('Island Craft Beverages Inc.');
    const fees = computeFees(app.declarations, app.jobNumber);
    expect(el.textContent).toContain('Liquor fees · RCPT-LIQ-1');
    expect(el.textContent).toContain('Cannabis fees · RCPT-CAN-1');
    expect(el.textContent).toContain(`Total paid${formatCurrency(fees.grandTotal)}`);
    expect(el.textContent).toContain('Government payment provider');
    expect(el.textContent).toContain('What happens next');
    expect(el.textContent).toContain('Confirmation email');
    expect(el.textContent).toContain('Review by the Branch');
    expect(el.querySelector('a[href="/tied-house"]')!.textContent).toContain('Back to Tied House');
  });

  it('confirms a submission with no fee without a payment section', async () => {
    seed('th_voluntary_BC0987654', [decl({ changeType: 'remove' })]);
    const { el } = await mount();
    expect(el.querySelector('h1')!.textContent).toContain('Declarations submitted');
    expect(el.textContent).toContain('Branch. A copy has been sent to your email.');
    expect(el.textContent).not.toContain('Payment details');
  });

  it('says the confirmation was not found for an unknown id', async () => {
    const { el } = await mount('nope');
    expect(el.querySelector('h1')!.textContent).toContain('Confirmation not found');
    expect(el.textContent).toContain("We couldn't find this submission. It may have been made on another device.");
    expect(el.querySelector('a[href="/tied-house"]')!.textContent).toContain('Back to Tied House');
  });

  it('follows the id input and prints the receipt', async () => {
    seed('th_voluntary_BC0987654', [decl({})], { liquorReceipt: 'R1' });
    const print = vi.spyOn(window, 'print').mockImplementation(() => undefined);
    const { fixture, el, next } = await mount('other');
    expect(el.querySelector('h1')!.textContent).toContain('Confirmation not found');
    fixture.componentInstance.id.set('th_voluntary_BC0987654');
    await next();
    expect(el.querySelector('h1')!.textContent).toContain('Payment received');
    [...el.querySelectorAll('button')].find((b) => b.textContent!.includes('Print receipt'))!.click();
    expect(print).toHaveBeenCalledTimes(1);
  });
});
