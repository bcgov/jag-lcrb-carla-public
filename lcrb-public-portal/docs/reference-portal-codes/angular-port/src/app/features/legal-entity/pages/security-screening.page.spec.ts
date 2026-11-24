import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { SessionProvider } from '@core/session/session-provider';
import { completeScan, uploadDocument } from '@data-access/repositories/documents/document-service';
import { screeningSubjectId } from '../state/screening-store';
import { SecurityScreeningPage } from './security-screening.page';

const settle = async (f: { detectChanges(): void; whenStable(): Promise<unknown> }, n = 3) => {
  for (let i = 0; i < n; i++) {
    f.detectChanges();
    await f.whenStable();
    await new Promise((r) => setTimeout(r, 0));
  }
  f.detectChanges();
};

async function mount() {
  TestBed.configureTestingModule({ providers: [provideRouter([])] });
  TestBed.inject(SessionProvider).login({ role: 'licensee', firstName: 'Alex', lastName: 'Rivera', organization: 'Society' } as never);
  const fixture = TestBed.createComponent(SecurityScreeningPage);
  document.body.appendChild(fixture.nativeElement);
  await settle(fixture);
  return { fixture, el: fixture.nativeElement as HTMLElement, next: () => settle(fixture) };
}

const people = () => JSON.parse(localStorage.getItem('lcrb.v2.security-screening') ?? '{}') as Record<string, { id: string; name: string; status: string; category: string }>;
const buttons = (root: ParentNode) => [...root.querySelectorAll('button')] as HTMLButtonElement[];
const button = (root: ParentNode, text: string) => buttons(root).find((b) => b.textContent!.trim().includes(text));
const cards = (el: HTMLElement) => [...el.querySelectorAll('[screeningPersonCard]')] as HTMLElement[];
const card = (el: HTMLElement, name: string) => cards(el).find((c) => c.textContent!.includes(name))!;
const tab = (el: HTMLElement, text: string) => [...el.querySelectorAll('[role="tab"]')].find((t) => t.textContent!.includes(text)) as HTMLElement;

describe('SecurityScreeningPage', () => {
  beforeEach(() => {
    localStorage.clear();
    sessionStorage.clear();
  });
  afterEach(() => {
    vi.restoreAllMocks();
    document.body.innerHTML = '';
  });

  it('renders the hero, the two tabs and the information card', async () => {
    const { el, next } = await mount();
    expect(el.querySelector('h1')!.textContent).toContain('Security Screening Requirements');
    expect(el.textContent).toContain('Manage Personal History Summary screening for each associated person');
    const tabs = [...el.querySelectorAll('[role="tab"]')];
    expect(tabs.map((t) => t.textContent!.trim())).toEqual(['Liquor Requirements', 'Cannabis Requirements']);
    expect(tabs[0].getAttribute('aria-selected')).toBe('true');
    expect(el.textContent).toContain('Liquor Licence Security Screening');
    expect(el.textContent).toContain('About Security Screening');
    expect(el.textContent).toContain('1-866-209-2111');
    tab(el, 'Cannabis').click();
    await next();
    expect(el.textContent).toContain('Cannabis Licence Security Screening');
    expect(el.textContent).not.toContain('Liquor Licence Security Screening');
  });

  it('lists the liquor people with their status, then the cannabis people', async () => {
    const { el, next } = await mount();
    expect(cards(el).map((c) => c.querySelector('p.font-semibold')!.textContent)).toEqual(['John Smith', 'Maria Chan']);
    expect(el.textContent).toContain('1 outstanding');
    const john = card(el, 'John Smith');
    expect(john.textContent).toContain('Submitted to LCRB');
    expect(john.textContent).toContain('Director');
    expect(john.textContent).toContain('john.smith@example.com · Personal History Summary');
    expect(john.textContent).toContain('Submitted to LCRB for review — case LCRB-482910 on Mar 15, 2026.');
    expect(button(john, 'Reset (demo)')).toBeTruthy();
    const maria = card(el, 'Maria Chan');
    expect(maria.textContent).toContain('Not started');
    expect(button(maria, 'Send screening request')).toBeTruthy();
    expect(button(maria, 'Reset (demo)')).toBeUndefined();
    expect(maria.classList.contains('rounded-lg')).toBe(true);

    tab(el, 'Cannabis').click();
    await next();
    expect(cards(el).map((c) => c.querySelector('p.font-semibold')!.textContent)).toEqual(['Jane Doe', 'Raj Patel']);
    expect(el.textContent).toContain('2 outstanding');
    expect(card(el, 'Jane Doe').textContent).toContain('Invitation sent');
    expect(card(el, 'Jane Doe').textContent).toContain('Invitation emailed on May 10, 2026.');
  });

  it('sends a screening request, which shows the invitation and the upload area', async () => {
    const { el, next } = await mount();
    button(card(el, 'Maria Chan'), 'Send screening request')!.click();
    await next();
    expect(people()['person-maria-chan'].status).toBe('invited');
    const maria = card(el, 'Maria Chan');
    expect(maria.textContent).toContain('Invitation sent');
    expect(maria.textContent).toContain('Invitation emailed on ');
    expect(maria.textContent).toContain('Completed Personal History Summary');
    expect(maria.querySelector('document-manager')).not.toBeNull();
    expect(button(maria, 'Submit completed form to LCRB')!.disabled).toBe(true);
    expect(maria.textContent).toContain('Upload the completed form (and wait for the virus scan) to enable submission.');
    expect(button(maria, 'Send screening request')).toBeUndefined();

    // Reset (demo) returns the person to Not started.
    button(maria, 'Reset (demo)')!.click();
    await next();
    expect(people()['person-maria-chan'].status).toBe('not_started');
    expect(button(card(el, 'Maria Chan'), 'Send screening request')).toBeTruthy();
  });

  it('submits the completed form once a clean document is uploaded', async () => {
    vi.spyOn(Math, 'random').mockReturnValue(0.5);
    const { el, next } = await mount();
    tab(el, 'Cannabis').click();
    await next();
    const jane = () => card(el, 'Jane Doe');
    const submit = () => button(jane(), 'Submit completed form to LCRB')!;
    expect(submit().disabled).toBe(true);

    // A pending scan does not enable submission; a clean one does.
    const doc = uploadDocument({
      subjectId: screeningSubjectId('person-jane-doe'),
      name: 'phs.pdf',
      contentType: 'application/pdf',
      size: 1000,
      uploadedBy: 'Alex Rivera',
    });
    await next();
    expect(submit().disabled).toBe(true);
    completeScan(doc.id);
    await next();
    expect(submit().disabled).toBe(false);
    expect(jane().textContent).not.toContain('Upload the completed form');

    submit().click();
    await next();
    // The store syncs at once, so the card already shows the submitted state (no Submit button).
    expect(people()['person-jane-doe'].status).toBe('synced');
    await new Promise((r) => setTimeout(r, 500));
    await next();
    expect(people()['person-jane-doe'].status).toBe('synced');
    expect(jane().textContent).toContain('Submitted to LCRB');
    expect(jane().textContent).toContain('Submitted to LCRB for review — case ');
    expect(button(jane(), 'Submit completed form to LCRB')).toBeUndefined();
  });
});
